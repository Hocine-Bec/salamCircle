using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class SwapService : ISwapService
{
    private readonly ISwapRequestRepository _swapRequestRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly ICircleRepository _circleRepository;
    private readonly IContributionCycleRepository _cycleRepository;
    private readonly ITransparencyLogService _transparencyLogService;
    private readonly INotificationService _notificationService;

    public SwapService(
        ISwapRequestRepository swapRequestRepository,
        ICircleMemberRepository circleMemberRepository,
        ICircleRepository circleRepository,
        IContributionCycleRepository cycleRepository,
        ITransparencyLogService transparencyLogService,
        INotificationService notificationService)
    {
        _swapRequestRepository = swapRequestRepository;
        _circleMemberRepository = circleMemberRepository;
        _circleRepository = circleRepository;
        _cycleRepository = cycleRepository;
        _transparencyLogService = transparencyLogService;
        _notificationService = notificationService;
    }

    public async Task<List<SwapRequest>> GetOpenSwapRequestsAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        var activeCycle = await _cycleRepository.GetActiveByCircleIdAsync(circleId);
        if (activeCycle is null)
            return new List<SwapRequest>();

        return await _swapRequestRepository.GetOpenByCycleIdAsync(activeCycle.Id);
    }

    public async Task<SwapRequest> PostSwapRequestAsync(Guid cycleId, Guid requestingUserId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.", nameof(cycleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var cycle = await _cycleRepository.GetByIdAsync(cycleId)
            ?? throw new KeyNotFoundException($"Contribution cycle with id '{cycleId}' not found.");

        var member = await _circleMemberRepository.GetByCircleAndUserAsync(cycle.CircleId, requestingUserId)
            ?? throw new KeyNotFoundException($"Member '{requestingUserId}' not found in circle '{cycle.CircleId}'.");

        var swapCount = await _swapRequestRepository.CountByMemberAndCycleAsync(member.Id, cycleId);
        if (swapCount >= 3)
            throw new InvalidOperationException("Member has reached the maximum swap limit of 3 for this cycle.");

        
        var openSwaps = await _swapRequestRepository.GetOpenByCycleIdAsync(cycleId);
        if (openSwaps.Any(s => s.RequesterId == member.Id))
            throw new InvalidOperationException("You already have an open swap request for this cycle.");


        var circle = await _circleRepository.GetByIdAsync(cycle.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{cycle.CircleId}' not found.");

        var request = new SwapRequest
        {
            CircleId = cycle.CircleId,
            CycleId = cycleId,
            RequesterId = member.Id,
            RequesterOriginalPosition = member.QueuePosition,
            Status = SwapStatus.Open
        };

        var created = await _swapRequestRepository.CreateAsync(request);

        await _transparencyLogService.LogAsync(
            created.CircleId,
            LogEventType.SwapRequested,
            $"Member '{requestingUserId}' posted a swap request for cycle '{cycleId}'.",
            actorId: requestingUserId,
            referenceId: created.Id);

        // US-17: Imam is notified when a swap request is posted
        await _notificationService.SendAsync(
            userId: circle.ImamId,
            circleId: circle.Id,
            type: NotificationType.SwapRequested,
            title: "New swap request posted",
            body: $"A member has posted an urgency swap request in circle \"{circle.Name}\" for the current cycle. Please review if needed.");

        return created;
    }

    public async Task<SwapRequest> AcceptSwapRequestAsync(Guid swapRequestId, Guid requestingUserId)
    {
        if (swapRequestId == Guid.Empty)
            throw new ArgumentException("Swap request id cannot be empty.", nameof(swapRequestId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var request = await _swapRequestRepository.GetByIdAsync(swapRequestId)
            ?? throw new KeyNotFoundException($"Swap request with id '{swapRequestId}' not found.");

        if (request.Status != SwapStatus.Open)
            throw new InvalidOperationException("Swap request is no longer open.");

        var acceptorMember = await _circleMemberRepository.GetByCircleAndUserAsync(request.CircleId, requestingUserId)
            ?? throw new KeyNotFoundException($"Member '{requestingUserId}' not found in circle '{request.CircleId}'.");

        if (acceptorMember.Id == request.RequesterId)
            throw new InvalidOperationException("Member cannot accept their own swap request.");

        var requesterMember = await _circleMemberRepository.GetByIdAsync(request.RequesterId)
            ?? throw new KeyNotFoundException($"Requester member with id '{request.RequesterId}' not found.");

        var circle = await _circleRepository.GetByIdAsync(request.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{request.CircleId}' not found.");

        request.AcceptorId = acceptorMember.Id;
        request.AcceptorOriginalPosition = acceptorMember.QueuePosition;
        request.Status = SwapStatus.Accepted;
        request.AcceptedAt = DateTime.UtcNow;

        var tempPosition = requesterMember.QueuePosition;
        requesterMember.QueuePosition = acceptorMember.QueuePosition;
        acceptorMember.QueuePosition = tempPosition;

        requesterMember.SwapCount++;
        acceptorMember.SwapCount++;

        await _circleMemberRepository.UpdateAsync(requesterMember);
        await _circleMemberRepository.UpdateAsync(acceptorMember);
        await _swapRequestRepository.UpdateAsync(request);

        await CheckSwapWarningAsync(request.CircleId, requesterMember.Id);
        await CheckSwapWarningAsync(request.CircleId, acceptorMember.Id);

        await _transparencyLogService.LogAsync(
            request.CircleId,
            LogEventType.SwapAccepted,
            $"Member '{requestingUserId}' accepted swap request '{swapRequestId}' from member '{request.RequesterId}'.",
            actorId: requestingUserId,
            targetId: request.RequesterId,
            referenceId: swapRequestId);

        // US-18: Both members receive a notification confirming the swap
        await _notificationService.SendAsync(
            userId: requesterMember.UserId,
            circleId: request.CircleId,
            type: NotificationType.SwapAccepted,
            title: "Your swap request has been accepted",
            body: $"Alhamdulillah, your urgency swap request in circle \"{circle.Name}\" has been accepted. Your queue positions have been exchanged.");

        await _notificationService.SendAsync(
            userId: acceptorMember.UserId,
            circleId: request.CircleId,
            type: NotificationType.SwapAccepted,
            title: "Swap confirmed — queue updated",
            body: $"You have successfully accepted a swap request in circle \"{circle.Name}\". Your queue positions have been exchanged. Jazak Allahu khayran.");

        return request;
    }

    // Checks if member reached 3 swaps in the current cycle,
    // which triggers a warning to the Imam (US-19).
    public async Task<bool> CheckSwapWarningAsync(Guid circleId, Guid memberId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (memberId == Guid.Empty)
            throw new ArgumentException("Member id cannot be empty.", nameof(memberId));

        var activeCycle = await _cycleRepository.GetActiveByCircleIdAsync(circleId);
        if (activeCycle is null)
            return false;

        var swapCount = await _swapRequestRepository.CountByMemberAndCycleAsync(memberId, activeCycle.Id);
        if (swapCount < 3)
            return false;

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        var member = await _circleMemberRepository.GetByIdAsync(memberId)
            ?? throw new KeyNotFoundException($"Member with id '{memberId}' not found.");

        // US-19: Imam receives a warning notification
        await _notificationService.SendAsync(
            userId: circle.ImamId,
            circleId: circleId,
            type: NotificationType.MemberFlagged,
            title: "Swap abuse warning",
            body: $"Warning: A member has reached 3 or more swap requests this cycle in circle \"{circle.Name}\". Please review their activity.");

        // US-19: Member is also notified they have been flagged
        await _notificationService.SendAsync(
            userId: member.UserId,
            circleId: circleId,
            type: NotificationType.MemberFlagged,
            title: "You have been flagged for excessive swaps",
            body: $"You have reached the maximum number of swaps allowed this cycle in circle \"{circle.Name}\". The Imam has been notified. Please reach out if you need support.");

        return true;
    }
}
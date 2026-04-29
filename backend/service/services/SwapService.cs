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

    return request;
}


    // This method checks is member reached 3 swaps in the current cycle
    //  which triggers a warning to the Imam (US-19).
    // will be implmented later when we handle required entities.
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

        await _notificationService.SendAsync(
            circle.ImamId,
            circleId,
            NotificationType.MemberFlagged,
            "Swap warning",
            $"Warning: Member '{memberId}' has made 3 or more swap requests this cycle.");

        return true;
    }
}

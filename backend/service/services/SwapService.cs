using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class SwapService : ISwapService
{
    private readonly ISwapRequestRepository _swapRequestRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly ITransparencyLogService _transparencyLogService;

    public SwapService(
        ISwapRequestRepository swapRequestRepository,
        ICircleMemberRepository circleMemberRepository,
        ITransparencyLogService transparencyLogService)
    {
        _swapRequestRepository = swapRequestRepository;
        _circleMemberRepository = circleMemberRepository;
        _transparencyLogService = transparencyLogService;
    }

    public async Task<List<SwapRequest>> GetOpenSwapRequestsAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        // TODO: resolve cycleId from circleId using current active ContributionCycle logic
        var placeholderCycleId = Guid.Empty;
        return await _swapRequestRepository.GetOpenByCycleIdAsync(placeholderCycleId);
    }

    public async Task<SwapRequest> PostSwapRequestAsync(Guid cycleId, Guid requestingUserId)
    {
        if (cycleId == Guid.Empty)
            throw new ArgumentException("Cycle id cannot be empty.", nameof(cycleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        var swapCount = await _swapRequestRepository.CountByMemberAndCycleAsync(requestingUserId, cycleId);
        if (swapCount >= 3)
            throw new InvalidOperationException("Member has reached the maximum swap limit of 3 for this cycle.");

        // TODO: resolve circleId from cycleId using ContributionCycle entity
        var placeholderCircleId = Guid.Empty;

        var request = new SwapRequest
        {
            CircleId = placeholderCircleId,
            CycleId = cycleId,
            RequesterId = requestingUserId,
            RequesterOriginalPosition = 0, // TODO: fetch from CircleMember
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

        if (requestingUserId == request.RequesterId)
            throw new InvalidOperationException("Member cannot accept their own swap request.");

        request.AcceptorId = requestingUserId;
        request.AcceptorOriginalPosition = 0; // TODO: fetch from CircleMember
        request.Status = SwapStatus.Accepted;
        request.AcceptedAt = DateTime.UtcNow;

        // TODO: swap the two members' QueuePositions in CircleMember table

        var updated = await _swapRequestRepository.UpdateAsync(request);

        await _transparencyLogService.LogAsync(
            request.CircleId,
            LogEventType.SwapAccepted,
            $"Member '{requestingUserId}' accepted swap request '{swapRequestId}' from member '{request.RequesterId}'.",
            actorId: requestingUserId,
            targetId: request.RequesterId,
            referenceId: swapRequestId);

        return updated;
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

        // TODO: resolve current cycleId from circleId using ContributionCycle logic
        var placeholderCycleId = Guid.Empty;

        // TODO: later, use CountByMemberAndCycleAsync and return true if count >= 3
        return false;
    }
}

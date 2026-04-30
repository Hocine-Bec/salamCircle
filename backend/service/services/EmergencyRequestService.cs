using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class EmergencyRequestService : IEmergencyRequestService
{
    private readonly ICircleRepository _circleRepository;
    private readonly IEmergencyRequestRepository _emergencyRequestRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly ITransparencyLogService _transparencyLogService;
    private readonly INotificationService _notificationService;

    public EmergencyRequestService(
        IEmergencyRequestRepository emergencyRequestRepository,
        ICircleMemberRepository circleMemberRepository,
        ITransparencyLogService transparencyLogService,
        ICircleRepository circleRepository,
        INotificationService notificationService)
    {
        _emergencyRequestRepository = emergencyRequestRepository;
        _circleMemberRepository = circleMemberRepository;
        _transparencyLogService = transparencyLogService;
        _circleRepository = circleRepository;
        _notificationService = notificationService;
    }

    public async Task<List<EmergencyRequest>> GetCircleRequestsAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        return await _emergencyRequestRepository.GetByCircleIdAsync(circleId);
    }

    public async Task<EmergencyRequest> SubmitRequestAsync(Guid circleId, decimal amount, string description, string? supportingContext, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        if (await _emergencyRequestRepository.HasPendingRequestAsync(requestingUserId))
            throw new InvalidOperationException("Member already has an active emergency request.");

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        var request = new EmergencyRequest
        {
            CircleId = circleId,
            RequestedById = requestingUserId,
            AmountRequested = amount,
            Description = description,
            SupportingContext = supportingContext,
            Status = EmergencyStatus.Pending
        };

        var created = await _emergencyRequestRepository.CreateAsync(request);

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.EmergencyRequested,
            $"Member '{requestingUserId}' submitted an emergency request.",
            actorId: requestingUserId);

        // US-14: Imam receives an immediate notification on submission
        await _notificationService.SendAsync(
            userId: circle.ImamId,
            circleId: circleId,
            type: NotificationType.EmergencySubmitted,
            title: "New emergency request submitted",
            body: $"A member has submitted an emergency withdrawal request in circle \"{circle.Name}\". Amount requested: {amount}. Please review it.");

        return created;
    }

    public async Task<EmergencyRequest> ApproveRequestAsync(Guid requestId, Guid imamId)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException("Request id cannot be empty.", nameof(requestId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var request = await _emergencyRequestRepository.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException($"Emergency request with id '{requestId}' not found.");

        if (request.Status != EmergencyStatus.Pending)
            throw new InvalidOperationException("Request is not pending.");

        var circle = await _circleRepository.GetByIdAsync(request.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{request.CircleId}' not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the Imam may approve emergency requests.");

        var member = await _circleMemberRepository.GetByIdAsync(request.RequestedById)
            ?? throw new KeyNotFoundException($"Member with id '{request.RequestedById}' not found.");

        request.Status = EmergencyStatus.Approved;
        request.ReviewedById = imamId;
        request.ReviewedAt = DateTime.UtcNow;

        var updated = await _emergencyRequestRepository.UpdateAsync(request);

        await _transparencyLogService.LogAsync(
            request.CircleId,
            LogEventType.EmergencyApproved,
            $"Emergency request '{requestId}' approved by Imam '{imamId}'.",
            actorId: imamId,
            targetId: request.RequestedById,
            referenceId: requestId);

        // US-15/16: Member is notified when request is approved
        await _notificationService.SendAsync(
            userId: member.UserId,
            circleId: request.CircleId,
            type: NotificationType.EmergencyApproved,
            title: "Your emergency request has been approved",
            body: $"Alhamdulillah, your emergency withdrawal request in circle \"{circle.Name}\" has been approved. The funds will be disbursed to you. Barakallahu feek.");

        return updated;
    }

    public async Task<EmergencyRequest> RejectRequestAsync(Guid requestId, string rejectionReason, Guid imamId)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException("Request id cannot be empty.", nameof(requestId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason cannot be empty.", nameof(rejectionReason));

        var request = await _emergencyRequestRepository.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException($"Emergency request with id '{requestId}' not found.");

        if (request.Status != EmergencyStatus.Pending)
            throw new InvalidOperationException("Request is not pending.");

        var circle = await _circleRepository.GetByIdAsync(request.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{request.CircleId}' not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the Imam may reject emergency requests.");

        var member = await _circleMemberRepository.GetByIdAsync(request.RequestedById)
            ?? throw new KeyNotFoundException($"Member with id '{request.RequestedById}' not found.");

        request.Status = EmergencyStatus.Rejected;
        request.ReviewedById = imamId;
        request.ReviewedAt = DateTime.UtcNow;
        request.RejectionReason = rejectionReason;

        var updated = await _emergencyRequestRepository.UpdateAsync(request);

        await _transparencyLogService.LogAsync(
            request.CircleId,
            LogEventType.EmergencyRejected,
            $"Emergency request '{requestId}' rejected by Imam '{imamId}'.",
            actorId: imamId,
            targetId: request.RequestedById,
            referenceId: requestId);

        // US-15: Member is notified when request is rejected
        await _notificationService.SendAsync(
            userId: member.UserId,
            circleId: request.CircleId,
            type: NotificationType.EmergencyRejected,
            title: "Your emergency request was not approved",
            body: $"Your emergency withdrawal request in circle \"{circle.Name}\" was not approved. Reason: {rejectionReason}. Please contact the Imam for more details.");

        return updated;
    }

    public async Task DisburseEmergencyFundsAsync(Guid requestId, Guid imamId)
    {
        if (requestId == Guid.Empty)
            throw new ArgumentException("Request id cannot be empty.", nameof(requestId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var request = await _emergencyRequestRepository.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException($"Emergency request with id '{requestId}' not found.");

        if (request.Status != EmergencyStatus.Approved)
            throw new InvalidOperationException("Request must be approved before disbursement.");

        var circle = await _circleRepository.GetByIdAsync(request.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{request.CircleId}' not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the Imam may disburse emergency funds.");

        await _transparencyLogService.LogAsync(
            request.CircleId,
            LogEventType.EmergencyDisbursed,
            $"Emergency funds disbursed for request '{requestId}' by Imam '{imamId}'.",
            actorId: imamId,
            referenceId: requestId);
    }
}
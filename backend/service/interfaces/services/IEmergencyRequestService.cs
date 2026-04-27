using service.entities;

namespace service.interfaces.services;

public interface IEmergencyRequestService
{
    Task<List<EmergencyRequest>> GetCircleRequestsAsync(Guid circleId, Guid requestingUserId);
    Task<EmergencyRequest> SubmitRequestAsync(Guid circleId, decimal amount, string description, string? supportingContext, Guid requestingUserId);
    Task<EmergencyRequest> ApproveRequestAsync(Guid requestId, Guid imamId);
    Task<EmergencyRequest> RejectRequestAsync(Guid requestId, string rejectionReason, Guid imamId);
    Task DisburseEmergencyFundsAsync(Guid requestId, Guid imamId);
}

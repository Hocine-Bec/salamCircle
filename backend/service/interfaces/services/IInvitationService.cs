using service.entities;

namespace service.interfaces.services;

public interface IInvitationService
{
    Task<CircleInvitation> SendInvitationAsync(Guid circleId, string phoneNumber, Guid imamId);
    Task<List<CircleInvitation>> GetPendingInvitationsAsync(Guid userId);
    Task AcceptInvitationAsync(Guid invitationId, Guid userId);
    Task DeclineInvitationAsync(Guid invitationId, Guid userId);
    Task<List<CircleInvitation>> GetCircleInvitationsAsync(Guid circleId, Guid imamId);
}

using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class InvitationService : IInvitationService
{
    private readonly ICircleInvitationRepository _repository;

    public InvitationService(ICircleInvitationRepository repository)
    {
        _repository = repository;
    }

    public async Task<CircleInvitation> SendInvitationAsync(Guid circleId, string phoneNumber, Guid imamId)
    {
        if (circleId == Guid.Empty || imamId == Guid.Empty)
            throw new ArgumentException("Invalid IDs");

        var invitation = new CircleInvitation
        {
            CircleId = circleId,
            InvitedById = imamId,
            // InvitedUserId normally resolved by phone lookup in UserService
            Status = InvitationStatus.Pending
        };

        return await _repository.CreateAsync(invitation);
    }

    public async Task<List<CircleInvitation>> GetPendingInvitationsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user id");

        return await _repository.GetPendingByUserIdAsync(userId);
    }

    public async Task AcceptInvitationAsync(Guid invitationId, Guid userId)
    {
        var invitation = await _repository.GetByIdAsync(invitationId)
            ?? throw new Exception("Invitation not found");

        if (invitation.InvitedUserId != userId)
            throw new Exception("Not allowed");

        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(invitation);
    }

    public async Task DeclineInvitationAsync(Guid invitationId, Guid userId)
    {
        var invitation = await _repository.GetByIdAsync(invitationId)
            ?? throw new Exception("Invitation not found");

        if (invitation.InvitedUserId != userId)
            throw new Exception("Not allowed");

        invitation.Status = InvitationStatus.Declined;
        invitation.RespondedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(invitation);
    }

    public async Task<List<CircleInvitation>> GetCircleInvitationsAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Invalid circle id");

        return await _repository.GetByCircleIdAsync(circleId);
    }
}
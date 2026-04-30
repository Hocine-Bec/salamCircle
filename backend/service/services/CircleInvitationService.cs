using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class InvitationService : IInvitationService
{
    private readonly ICircleInvitationRepository _repository;
    private readonly ICircleMemberService _circleMemberService;
    private readonly IUserRepository _userRepository;

    public InvitationService(
        ICircleInvitationRepository repository,
        ICircleMemberService circleMemberService,
        IUserRepository userRepository)
    {
        _repository = repository;
        _circleMemberService = circleMemberService;
        _userRepository = userRepository;
    }

    public async Task<CircleInvitation> SendInvitationAsync(Guid circleId, string phoneNumber, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));

        var invitedUser = await _userRepository.GetByPhoneAsync(phoneNumber)
            ?? throw new KeyNotFoundException($"No user found with phone number '{phoneNumber}'.");

        var invitation = new CircleInvitation
        {
            CircleId = circleId,
            InvitedById = imamId,
            InvitedUserId = invitedUser.Id,
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
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.InvitedUserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to accept this invitation.");

        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(invitation);
        await _circleMemberService.AddMemberAsync(invitation.CircleId, userId, invitation.InvitedById);
    }

    public async Task DeclineInvitationAsync(Guid invitationId, Guid userId)
    {
        var invitation = await _repository.GetByIdAsync(invitationId)
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.InvitedUserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to decline this invitation.");

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

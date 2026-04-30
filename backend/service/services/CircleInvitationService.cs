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
    private readonly ICircleRepository _circleRepository;
    private readonly INotificationService _notificationService;

    public InvitationService(
        ICircleInvitationRepository repository,
        ICircleMemberService circleMemberService,
        IUserRepository userRepository,
        ICircleRepository circleRepository,
        INotificationService notificationService)
    {
        _repository = repository;
        _circleMemberService = circleMemberService;
        _userRepository = userRepository;
        _circleRepository = circleRepository;
        _notificationService = notificationService;
    }

    public async Task<CircleInvitation> SendInvitationAsync(Guid circleId, string phoneNumber, Guid imamId)
{
    if (circleId == Guid.Empty)
        throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

    if (imamId == Guid.Empty)
        throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

    if (string.IsNullOrWhiteSpace(phoneNumber))
        throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));

    // US-05: user must be registered
    var invitedUser = await _userRepository.GetByPhoneAsync(phoneNumber)
        ?? throw new KeyNotFoundException($"No user found with phone number '{phoneNumber}'.");

    var circle = await _circleRepository.GetByIdAsync(circleId)
        ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

    if (circle.Status == CircleStatus.Closed)
        throw new InvalidOperationException("Cannot invite members to a closed circle.");

    // US-05: error if already an ACTIVE member — removed/exited members can be re-invited
    var alreadyMember = await _circleMemberService.IsActiveMemberAsync(circleId, invitedUser.Id);
    if (alreadyMember)
        throw new InvalidOperationException(
            $"User with phone '{phoneNumber}' is already an active member of this circle.");

    var invitation = new CircleInvitation
    {
        CircleId = circleId,
        InvitedById = imamId,
        InvitedUserId = invitedUser.Id,
        Status = InvitationStatus.Pending
    };

    var created = await _repository.CreateAsync(invitation);

    await _notificationService.SendAsync(
        userId: invitedUser.Id,
        circleId: circleId,
        type: NotificationType.InvitationReceived,
        title: "You have been invited to a circle",
        body: $"Assalamu alaikum {invitedUser.Name}, you have been invited to join the circle \"{circle.Name}\". Please check your invitations.");

    return created;
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

        var circle = await _circleRepository.GetByIdAsync(invitation.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{invitation.CircleId}' not found.");

        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(invitation);
        await _circleMemberService.AddMemberAsync(invitation.CircleId, userId, invitation.InvitedById);

        // US-06: Imam is notified that a new member has joined
        await _notificationService.SendAsync(
            userId: circle.ImamId,
            circleId: circle.Id,
            type: NotificationType.CircleUpdated,
            title: "New member joined your circle",
            body: $"A new member has accepted your invitation and joined the circle \"{circle.Name}\".");
    }

    public async Task DeclineInvitationAsync(Guid invitationId, Guid userId)
    {
        var invitation = await _repository.GetByIdAsync(invitationId)
            ?? throw new KeyNotFoundException("Invitation not found");

        if (invitation.InvitedUserId != userId)
            throw new UnauthorizedAccessException("You are not authorized to decline this invitation.");

        var circle = await _circleRepository.GetByIdAsync(invitation.CircleId)
            ?? throw new KeyNotFoundException($"Circle with id '{invitation.CircleId}' not found.");

        invitation.Status = InvitationStatus.Declined;
        invitation.RespondedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(invitation);

        // US-06: Imam is notified of decline
        await _notificationService.SendAsync(
            userId: circle.ImamId,
            circleId: circle.Id,
            type: NotificationType.CircleUpdated,
            title: "Invitation declined",
            body: $"A user has declined your invitation to join the circle \"{circle.Name}\".");
    }

    public async Task<List<CircleInvitation>> GetCircleInvitationsAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Invalid circle id");

        return await _repository.GetByCircleIdAsync(circleId);
    }
}
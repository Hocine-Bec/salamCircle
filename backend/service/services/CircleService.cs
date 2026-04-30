using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;
using service.models;

namespace service.services;

public class CircleService : ICircleService
{
    
    private readonly IContributionRepository _contributionRepository;

    private readonly ICircleRepository _circleRepository;

    private readonly ITransparencyLogService _transparencyLogService;
    private readonly ICircleMemberRepository _circleMemberRepository;
    private readonly INotificationService _notificationService;

    public CircleService(
        ICircleRepository circleRepository,
        ITransparencyLogService transparencyLogService,
        ICircleMemberRepository circleMemberRepository,
        INotificationService notificationService,
        IContributionRepository contributionRepository)   
    {
        _circleRepository = circleRepository;
        _transparencyLogService = transparencyLogService;
        _circleMemberRepository = circleMemberRepository;
        _notificationService = notificationService;
        _contributionRepository = contributionRepository; 
    }
    public async Task<Circle?> GetByIdAsync(Guid circleId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        return await _circleRepository.GetByIdAsync(circleId);
    }

    public async Task<List<Circle>> GetAllAsync()
        => await _circleRepository.GetAllAsync();

    public async Task<List<Circle>> GetByImamIdAsync(Guid imamId)
    {
        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        return await _circleRepository.GetByImamIdAsync(imamId);
    }

    public async Task<Circle> CreateAsync(Circle circle)
    {
        if (circle is null)
            throw new ArgumentNullException(nameof(circle));

        if (string.IsNullOrWhiteSpace(circle.Name))
            throw new ArgumentException("Circle name cannot be empty.", nameof(circle));

        if (circle.ImamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(circle));

        if (circle.MinimumContribution <= 0)
            throw new ArgumentException("Minimum contribution must be greater than zero.", nameof(circle));

        circle.ContributorsPerMonth = 1;
        circle.Status = CircleStatus.Active;
        circle.ClosedAt = null;

        var created = await _circleRepository.CreateAsync(circle);

        await _transparencyLogService.LogAsync(
            created.Id,
            LogEventType.CircleCreated,
            $"Circle '{created.Name}' was created by Imam '{created.ImamId}'.",
            actorId: created.ImamId);

        return created;
    }

    public async Task<Circle> UpdateAsync(Circle circle, Guid imamId)
    {
        if (circle is null)
            throw new ArgumentNullException(nameof(circle));

        if (string.IsNullOrWhiteSpace(circle.Name))
            throw new ArgumentException("Circle name cannot be empty.", nameof(circle));

        if (circle.MinimumContribution <= 0)
            throw new ArgumentException("Minimum contribution must be greater than zero.", nameof(circle));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var existing = await _circleRepository.GetByIdAsync(circle.Id)
            ?? throw new KeyNotFoundException($"Circle with id '{circle.Id}' not found.");

        if (existing.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the imam may update the circle.");

        if (existing.Status == CircleStatus.Closed)
            throw new InvalidOperationException("Cannot update a closed circle.");

        existing.Name = circle.Name;
        existing.MinimumContribution = circle.MinimumContribution;

        return await _circleRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid circleId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        var existing = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        var activeCount = await _circleMemberRepository.CountActiveAsync(circleId);
        if (activeCount > 0)
            throw new InvalidOperationException("Cannot delete a circle with active members.");

        await _circleRepository.DeleteAsync(circleId);
    }

    public async Task CloseCircleAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the imam may close the circle.");

        if (circle.Status != CircleStatus.Active)
            throw new InvalidOperationException("Circle is already closed.");

        circle.Status = CircleStatus.Closed;
        circle.ClosedAt = DateTime.UtcNow;

        await _circleRepository.UpdateAsync(circle);

        await _transparencyLogService.LogAsync(
            circleId,
            LogEventType.CircleClosed,
            $"Circle '{circle.Name}' was closed by Imam '{imamId}'.",
            actorId: imamId);

        // US-08: All members receive a notification when the circle is closed
        var activeMembers = await _circleMemberRepository.GetByCircleIdAsync(circleId);
        foreach (var member in activeMembers.Where(m => m.Status == MemberStatus.Active))
        {
            await _notificationService.SendAsync(
                userId: member.UserId,
                circleId: circleId,
                type: NotificationType.CircleClosed,
                title: "Your circle has been closed",
                body: $"The circle \"{circle.Name}\" has been officially closed by the Imam. Jazak Allahu khayran for your participation.");
        }
    }

    // US-07: Single dashboard endpoint — balance, member count, current + next contributors
public async Task<CircleDashboard> GetDashboardAsync(Guid circleId, Guid requestingUserId)
{
    if (circleId == Guid.Empty)
        throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

    if (requestingUserId == Guid.Empty)
        throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

    var circle = await _circleRepository.GetByIdAsync(circleId)
        ?? throw new KeyNotFoundException($"Circle with id '{circleId}' not found.");

    // Only members of the circle can see the dashboard
    var isMember = await _circleMemberRepository.IsActiveMemberAsync(circleId, requestingUserId);
    if (!isMember && circle.ImamId != requestingUserId)
        throw new UnauthorizedAccessException("You are not a member of this circle.");

    var balance = await _contributionRepository.GetTotalByCircleIdAsync(circleId);
    var activeMembers = (await _circleMemberRepository.GetByCircleIdAsync(circleId))
        .Where(m => m.Status == MemberStatus.Active)
        .OrderBy(m => m.QueuePosition)
        .ToList();

    var contributorsPerMonth = circle.ContributorsPerMonth;

    // Current contributors = first N members in the queue
    var current = activeMembers.Take(contributorsPerMonth).ToList();

    // Next contributors = the N members after the current batch
    var next = activeMembers.Skip(contributorsPerMonth).Take(contributorsPerMonth).ToList();

    return new CircleDashboard
    {
        CircleId = circle.Id,
        CircleName = circle.Name,
        Balance = balance,
        MemberCount = activeMembers.Count,
        ContributorsPerMonth = contributorsPerMonth,
        CurrentContributors = current,
        NextContributors = next
    };
}
}
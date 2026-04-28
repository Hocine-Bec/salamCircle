using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        return await _notificationRepository.GetByUserIdAsync(userId);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        return await _notificationRepository.CountUnreadByUserIdAsync(userId);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("Notification id cannot be empty.", nameof(notificationId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        var notification = await _notificationRepository.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification '{notificationId}' not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("User does not own this notification.");

        await _notificationRepository.MarkAsReadAsync(notificationId);
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        await _notificationRepository.MarkAllAsReadAsync(userId);
    }

    public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("Notification id cannot be empty.", nameof(notificationId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        var notification = await _notificationRepository.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification '{notificationId}' not found.");

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("User does not own this notification.");

        await _notificationRepository.DeleteAsync(notificationId);
    }

    public async Task SendAsync(Guid userId, Guid? circleId, NotificationType type, string title, string body)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id cannot be empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Body cannot be empty.", nameof(body));

        var notification = new Notification
        {
            UserId = userId,
            CircleId = circleId,
            Type = type,
            Title = title,
            Body = body,
            IsRead = false
        };

        await _notificationRepository.CreateAsync(notification);
    }
}

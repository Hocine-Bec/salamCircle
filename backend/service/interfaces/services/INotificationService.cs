using service.entities;
using service.enums;

namespace service.interfaces.services;

public interface INotificationService
{
    Task<List<Notification>> GetUserNotificationsAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid notificationId, Guid userId);
    Task SendAsync(Guid userId, Guid? circleId, NotificationType type, string title, string body);
}

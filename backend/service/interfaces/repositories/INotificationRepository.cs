using service.entities;

namespace service.interfaces.repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(Guid userId);
    Task<int> CountUnreadByUserIdAsync(Guid userId);
    Task<Notification> CreateAsync(Notification notification);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteAsync(Guid id);
}

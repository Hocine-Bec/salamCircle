namespace service.entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation
    public ICollection<CircleMember> CircleMemberships { get; set; } = [];
    public ICollection<Circle> ImamOfCircles { get; set; } = [];
    public ICollection<CircleInvitation> SentInvitations { get; set; } = [];
    public ICollection<CircleInvitation> ReceivedInvitations { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<TransparencyLog> ActorLogs { get; set; } = [];
    public ICollection<TransparencyLog> TargetLogs { get; set; } = [];
    public ICollection<EmergencyRequest> ReviewedRequests { get; set; } = [];
    public ICollection<Reminder> SentReminders { get; set; } = [];
}

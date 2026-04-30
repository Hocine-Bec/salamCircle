using service.entities;
using webAPI.DTOs.Responses;

namespace webAPI.Mapping;

public static class MappingExtensions
{
    public static UserDto ToDto(this User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Phone = u.Phone,
        CreatedAt = u.CreatedAt
    };

    public static CircleDto ToDto(this Circle c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        ImamId = c.ImamId,
        MinimumContribution = c.MinimumContribution,
        ContributorsPerMonth = c.ContributorsPerMonth,
        Status = c.Status,
        ClosedAt = c.ClosedAt,
        CreatedAt = c.CreatedAt
    };

   public static CircleMemberDto ToDto(this CircleMember m) => new()
    {
        Id = m.Id,
        CircleId = m.CircleId,
        UserId = m.UserId,
        UserName = m.User?.Name ?? string.Empty,   // ← NEW
        QueuePosition = m.QueuePosition,
        Status = m.Status.ToString(),
        SwapCount = m.SwapCount,
        JoinedAt = m.JoinedAt
    };

    public static CircleInvitationDto ToDto(this CircleInvitation i) => new()
    {
        Id = i.Id,
        CircleId = i.CircleId,
        InvitedById = i.InvitedById,
        InvitedUserId = i.InvitedUserId,
        Status = i.Status,
        RespondedAt = i.RespondedAt,
        CreatedAt = i.CreatedAt
    };

    public static ContributionDto ToDto(this Contribution c) => new()
    {
        Id = c.Id,
        CycleId = c.CycleId,
        CircleId = c.CircleId,
        MemberId = c.MemberId,
        Amount = c.Amount,
        Status = c.Status,
        ContributedAt = c.ContributedAt,
        CreatedAt = c.CreatedAt
    };

    public static ContributionCycleDto ToDto(this ContributionCycle c) => new()
    {
        Id = c.Id,
        CircleId = c.CircleId,
        CycleNumber = c.CycleNumber,
        Month = c.Month,
        Year = c.Year,
        ContributorsPerCycle = c.ContributorsPerCycle,
        Status = c.Status,
        CreatedAt = c.CreatedAt
    };

    public static SwapRequestDto ToDto(this SwapRequest s) => new()
    {
        Id = s.Id,
        CircleId = s.CircleId,
        CycleId = s.CycleId,
        RequesterId = s.RequesterId,
        AcceptorId = s.AcceptorId,
        RequesterOriginalPosition = s.RequesterOriginalPosition,
        AcceptorOriginalPosition = s.AcceptorOriginalPosition,
        Status = s.Status.ToString(),
        AcceptedAt = s.AcceptedAt,
        CreatedAt = s.CreatedAt
    };

    public static EmergencyRequestDto ToDto(this EmergencyRequest r) => new()
    {
        Id = r.Id,
        CircleId = r.CircleId,
        RequestedById = r.RequestedById,
        AmountRequested = r.AmountRequested,
        Description = r.Description,
        SupportingContext = r.SupportingContext,
        Status = r.Status.ToString(),
        ReviewedById = r.ReviewedById,
        ReviewedAt = r.ReviewedAt,
        RejectionReason = r.RejectionReason,
        CreatedAt = r.CreatedAt
    };

    public static ReminderDto ToDto(this Reminder r) => new()
    {
        Id = r.Id,
        CircleId = r.CircleId,
        CycleId = r.CycleId,
        SentById = r.SentById,
        SentToId = r.SentToId,
        CreatedAt = r.CreatedAt
    };

    public static NotificationDto ToDto(this Notification n) => new()
    {
        Id = n.Id,
        UserId = n.UserId,
        CircleId = n.CircleId,
        Type = n.Type.ToString(),
        Title = n.Title,
        Body = n.Body,
        IsRead = n.IsRead,
        ReadAt = n.ReadAt,
        CreatedAt = n.CreatedAt
    };

    public static TransparencyLogDto ToDto(this TransparencyLog t) => new()
    {
        Id = t.Id,
        CircleId = t.CircleId,
        EventType = t.EventType.ToString(),
        ActorId = t.ActorId,
        TargetId = t.TargetId,
        ReferenceId = t.ReferenceId,
        Description = t.Description,
        Metadata = t.Metadata,
        CreatedAt = t.CreatedAt
    };
}

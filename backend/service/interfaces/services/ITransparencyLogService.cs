using service.entities;
using service.enums;

namespace service.interfaces.services;

public interface ITransparencyLogService
{
    Task<List<TransparencyLog>> GetCircleLogAsync(Guid circleId, Guid requestingUserId);
    Task<List<TransparencyLog>> GetCircleLogByTypeAsync(Guid circleId, LogEventType eventType, Guid requestingUserId);
    Task LogAsync(Guid circleId, LogEventType eventType, string description, Guid? actorId = null, Guid? targetId = null, Guid? referenceId = null, string? metadata = null);
}

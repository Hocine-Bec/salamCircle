using service.entities;
using service.enums;

namespace service.interfaces.repositories;

public interface ITransparencyLogRepository
{
    Task<List<TransparencyLog>> GetByCircleIdAsync(Guid circleId);
    Task<List<TransparencyLog>> GetByCircleIdAndTypeAsync(Guid circleId, LogEventType eventType);
    Task<TransparencyLog> CreateAsync(TransparencyLog log);
}

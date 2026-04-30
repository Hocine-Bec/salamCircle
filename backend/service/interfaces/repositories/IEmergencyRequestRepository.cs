using service.entities;

namespace service.interfaces.repositories;

public interface IEmergencyRequestRepository
{
    Task<EmergencyRequest?> GetByIdAsync(Guid id);
    Task<List<EmergencyRequest>> GetByCircleIdAsync(Guid circleId);
    Task<bool> HasPendingRequestAsync(Guid memberId);
    Task<decimal> GetTotalDisbursedAsync(Guid circleId);   // ← NEW US-07
    Task<EmergencyRequest> CreateAsync(EmergencyRequest request);
    Task<EmergencyRequest> UpdateAsync(EmergencyRequest request);
    Task DeleteAsync(Guid id);
}
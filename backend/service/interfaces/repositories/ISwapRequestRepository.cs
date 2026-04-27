using service.entities;

namespace service.interfaces.repositories;

public interface ISwapRequestRepository
{
    Task<SwapRequest?> GetByIdAsync(Guid id);
    Task<List<SwapRequest>> GetOpenByCycleIdAsync(Guid cycleId);
    Task<int> CountByMemberAndCycleAsync(Guid memberId, Guid cycleId);
    Task<SwapRequest> CreateAsync(SwapRequest swapRequest);
    Task<SwapRequest> UpdateAsync(SwapRequest swapRequest);
    Task DeleteAsync(Guid id);
}

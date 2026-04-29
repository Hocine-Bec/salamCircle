using service.entities;

namespace service.interfaces.repositories;

public interface IContributionRepository
{
    Task<Contribution?> GetByIdAsync(Guid id);
    Task<Contribution?> GetByCycleAndMemberAsync(Guid cycleId, Guid memberId);

    Task<List<Contribution>> GetByCircleAndMemberAsync(Guid circleId, Guid memberId);  
    Task<List<Contribution>> GetByCycleIdAsync(Guid cycleId);
    Task<List<Contribution>> GetByMemberIdAsync(Guid memberId);
    Task<decimal> GetTotalByCircleIdAsync(Guid circleId);
    Task<Contribution> CreateAsync(Contribution contribution);
    Task<Contribution> UpdateAsync(Contribution contribution);
    Task DeleteAsync(Guid id);
}

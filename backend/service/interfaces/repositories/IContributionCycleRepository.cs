using service.entities;

namespace service.interfaces.repositories;

public interface IContributionCycleRepository
{
    Task<ContributionCycle?> GetByIdAsync(Guid id);
    Task<ContributionCycle?> GetActiveByCircleIdAsync(Guid circleId);
    Task<ContributionCycle?> GetByCircleAndMonthAsync(Guid circleId, int month, int year);
    Task<List<ContributionCycle>> GetByCircleIdAsync(Guid circleId);
    Task<ContributionCycle> CreateAsync(ContributionCycle cycle);
    Task<ContributionCycle> UpdateAsync(ContributionCycle cycle);
}

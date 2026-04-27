using service.entities;

namespace service.interfaces.services;

public interface IContributionCycleService
{
    Task<ContributionCycle> GetActiveCycleAsync(Guid circleId, Guid requestingUserId);
    Task<List<ContributionCycle>> GetAllCyclesAsync(Guid circleId, Guid requestingUserId);
    Task<ContributionCycle> StartNextCycleAsync(Guid circleId, Guid imamId);
}

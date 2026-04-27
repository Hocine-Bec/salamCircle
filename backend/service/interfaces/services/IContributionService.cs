using service.entities;

namespace service.interfaces.services;

public interface IContributionService
{
    Task<List<Contribution>> GetCycleContributionsAsync(Guid cycleId, Guid requestingUserId);
    Task<List<Contribution>> GetMemberContributionHistoryAsync(Guid circleId, Guid requestingUserId);
    Task<decimal> GetCircleBalanceAsync(Guid circleId, Guid requestingUserId);
    Task<Contribution> SubmitContributionAsync(Guid cycleId, decimal amount, Guid requestingUserId);
}

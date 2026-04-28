using service.entities;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ContributionService : IContributionService
{
    private readonly IContributionRepository _repository;

    public ContributionService(IContributionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Contribution?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid id");

        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Contribution>> GetByCycleIdAsync(Guid cycleId)
        => await _repository.GetByCycleIdAsync(cycleId);

    public async Task<List<Contribution>> GetByMemberIdAsync(Guid memberId)
        => await _repository.GetByMemberIdAsync(memberId);

    public async Task<decimal> GetTotalByCircleIdAsync(Guid circleId)
        => await _repository.GetTotalByCircleIdAsync(circleId);

    public async Task<Contribution> CreateAsync(Contribution contribution)
    {
        if (contribution.Amount <= 0)
            throw new Exception("Amount must be greater than 0");

        return await _repository.CreateAsync(contribution);
    }

    public async Task<Contribution> UpdateAsync(Contribution contribution)
        => await _repository.UpdateAsync(contribution);

    public async Task DeleteAsync(Guid id)
        => await _repository.DeleteAsync(id);
}
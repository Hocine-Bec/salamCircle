// ContributionCycleService.cs
using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ContributionCycleService : IContributionCycleService
{
    private readonly IContributionCycleRepository _cycleRepository;

    public ContributionCycleService(IContributionCycleRepository cycleRepository)
    {
        _cycleRepository = cycleRepository;
    }

    public async Task<ContributionCycle> GetActiveCycleAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.");

        var cycle = await _cycleRepository.GetActiveByCircleIdAsync(circleId)
            ?? throw new KeyNotFoundException("No active cycle found.");

        return cycle;
    }

    public async Task<List<ContributionCycle>> GetAllCyclesAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.");

        return await _cycleRepository.GetByCircleIdAsync(circleId);
    }

    public async Task<ContributionCycle> StartNextCycleAsync(Guid circleId, Guid imamId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.");

        var activeCycle = await _cycleRepository.GetActiveByCircleIdAsync(circleId);

        if (activeCycle is not null)
        {
            activeCycle.Status = CycleStatus.Completed;
            await _cycleRepository.UpdateAsync(activeCycle);
        }

        var allCycles = await _cycleRepository.GetByCircleIdAsync(circleId);

        var nextCycle = new ContributionCycle
        {
            CircleId = circleId,
            CycleNumber = allCycles.Count + 1,
            Month = DateTime.UtcNow.Month,
            Year = DateTime.UtcNow.Year,
            Status = CycleStatus.Active
        };

        return await _cycleRepository.CreateAsync(nextCycle);
    }
}
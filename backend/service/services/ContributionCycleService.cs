// ContributionCycleService.cs
using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class ContributionCycleService : IContributionCycleService
{
    private readonly IContributionCycleRepository _cycleRepository;
    private readonly ICircleRepository _circleRepository;

    public ContributionCycleService(
        IContributionCycleRepository cycleRepository,
        ICircleRepository circleRepository)
    {
        _cycleRepository = cycleRepository;
        _circleRepository = circleRepository;
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
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (imamId == Guid.Empty)
            throw new ArgumentException("Imam id cannot be empty.", nameof(imamId));

        var circle = await _circleRepository.GetByIdAsync(circleId)
            ?? throw new KeyNotFoundException("Circle not found.");

        if (circle.ImamId != imamId)
            throw new UnauthorizedAccessException("Only the imam may start the next cycle.");

        var month = DateTime.UtcNow.Month;
        var year = DateTime.UtcNow.Year;

        if (await _cycleRepository.GetByCircleAndMonthAsync(circleId, month, year) is not null)
            throw new InvalidOperationException("A cycle already exists for the current month and year.");

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
            Month = month,
            Year = year,
            ContributorsPerCycle = circle.ContributorsPerMonth,
            Status = CycleStatus.Active
        };

        return await _cycleRepository.CreateAsync(nextCycle);
    }
}
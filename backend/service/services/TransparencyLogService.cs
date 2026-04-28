using service.entities;
using service.enums;
using service.interfaces.repositories;
using service.interfaces.services;

namespace service.services;

public class TransparencyLogService : ITransparencyLogService
{
    private readonly ITransparencyLogRepository _transparencyLogRepository;
    private readonly ICircleMemberRepository _circleMemberRepository;

    public TransparencyLogService(
        ITransparencyLogRepository transparencyLogRepository,
        ICircleMemberRepository circleMemberRepository)
    {
        _transparencyLogRepository = transparencyLogRepository;
        _circleMemberRepository = circleMemberRepository;
    }

    public async Task<List<TransparencyLog>> GetCircleLogAsync(Guid circleId, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        return await _transparencyLogRepository.GetByCircleIdAsync(circleId);
    }

    public async Task<List<TransparencyLog>> GetCircleLogByTypeAsync(Guid circleId, LogEventType eventType, Guid requestingUserId)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (requestingUserId == Guid.Empty)
            throw new ArgumentException("Requesting user id cannot be empty.", nameof(requestingUserId));

        if (!await _circleMemberRepository.IsMemberAsync(circleId, requestingUserId))
            throw new KeyNotFoundException($"Requesting user '{requestingUserId}' is not a member of circle '{circleId}'.");

        return await _transparencyLogRepository.GetByCircleIdAndTypeAsync(circleId, eventType);
    }

    public async Task LogAsync(Guid circleId, LogEventType eventType, string description, Guid? actorId = null, Guid? targetId = null, Guid? referenceId = null, string? metadata = null)
    {
        if (circleId == Guid.Empty)
            throw new ArgumentException("Circle id cannot be empty.", nameof(circleId));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        var log = new TransparencyLog
        {
            CircleId = circleId,
            EventType = eventType,
            ActorId = actorId,
            TargetId = targetId,
            ReferenceId = referenceId,
            Description = description,
            Metadata = metadata,
            CreatedAt = DateTime.UtcNow
        };

        await _transparencyLogRepository.CreateAsync(log);
    }
}

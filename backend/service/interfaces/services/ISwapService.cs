using service.entities;

namespace service.interfaces.services;

public interface ISwapService
{
    Task<List<SwapRequest>> GetOpenSwapRequestsAsync(Guid circleId, Guid requestingUserId);
    Task<SwapRequest> PostSwapRequestAsync(Guid cycleId, Guid requestingUserId);
    Task<SwapRequest> AcceptSwapRequestAsync(Guid swapRequestId, Guid requestingUserId);
    Task<bool> CheckSwapWarningAsync(Guid circleId, Guid memberId);
}

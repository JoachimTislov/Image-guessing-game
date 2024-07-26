using SharedKernel;

namespace Core.Domain.GameManagementContext.Events;

public record GameFinished : BaseDomainEvent
{
    public GameFinished(Guid gameId)
    {
        GameId = gameId;
    }
    public Guid GameId { get; }
}
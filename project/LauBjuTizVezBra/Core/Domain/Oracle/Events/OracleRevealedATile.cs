using SharedKernel;

namespace Core.Domain.GameManagementContext.Events;

public record OracleRevealedATile : BaseDomainEvent
{
    public OracleRevealedATile(Guid oracleId)
    {
        OracleId = oracleId;
    }
    public Guid OracleId { get; }
}
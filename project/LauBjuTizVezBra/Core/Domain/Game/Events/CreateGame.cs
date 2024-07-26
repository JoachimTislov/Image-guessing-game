using Core.Domain.SessionContext;
using Core.Domain.UserContext;
using SharedKernel;

namespace Core.Domain.GameManagementContext.Events;

public record CreateGame : BaseDomainEvent
{
    public CreateGame(Session session)
    {
        SessionId = session.Id;
        Oracle = session.ChosenOracle;
        Users = session.SessionUsers;
        Options = session.Options;
        ImageIdentifier = session.ImageIdentifier ?? "RandomImage";
    }
    public Guid SessionId { get; }
    public Guid? Oracle { get; }
    public List<User> Users { get; }
    public Options Options { get; }
    public string ImageIdentifier { get; }
}
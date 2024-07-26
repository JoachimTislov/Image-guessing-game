using SharedKernel;

namespace Core.Domain.SessionContext.Events;

public record SessionClosed : BaseDomainEvent
{
    public SessionClosed(Session session)
    {
        Session = session;
    }
    public Session Session { get; }
}

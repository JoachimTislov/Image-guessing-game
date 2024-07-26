using Core.Domain.GameManagementContext.Events;
using Core.Domain.SessionContext.Events;
using Core.Domain.SessionContext.Pipelines;
using MediatR;

namespace Core.Domain.SessionContext.Handlers;

public class SessionClosedHandler : INotificationHandler<SessionClosed>
{
    private readonly IMediator _mediator;

    public SessionClosedHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Handle(SessionClosed notification, CancellationToken cancellationToken)
    {
        if(notification.Session != null)
        {
            notification.Session.SessionStatus = SessionStatus.Closed;
        }
        
        return Task.CompletedTask;
    }
}
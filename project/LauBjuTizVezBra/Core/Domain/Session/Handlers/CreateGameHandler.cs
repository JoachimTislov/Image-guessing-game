using Core.Domain.GameManagementContext.Events;
using Core.Domain.SessionContext.Pipelines;
using MediatR;

namespace Core.Domain.SessionContext.Handlers;

public class CreateGameHandler : INotificationHandler<CreateGame>
{
    private readonly IMediator _mediator;

    public CreateGameHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(CreateGame notification, CancellationToken cancellationToken)
    {
        var session = await _mediator.Send(new GetSessionById.Request(notification.SessionId), cancellationToken);
        
        if(session != null)
        {
            session.SessionStatus = SessionStatus.InGame;
            session.Options.NumberOfRounds--;
        }
    }
}
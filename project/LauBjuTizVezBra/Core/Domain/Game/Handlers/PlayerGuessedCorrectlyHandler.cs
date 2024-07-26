using Core.Domain.GameManagementContext.Events;
using Core.Domain.GameManagementContext.Pipelines;
using MediatR;

namespace Core.Domain.GameManagementContext.Handlers;

public class PlayerGuessedCorrectlyHandler : INotificationHandler<PlayerGuessedCorrectly> 
{
    private readonly IMediator _mediator;

    public PlayerGuessedCorrectlyHandler(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Handle(PlayerGuessedCorrectly notification, CancellationToken cancellationToken)
    {
        var game = await _mediator.Send(new GetGameById.Request(notification.GameId), cancellationToken);
        
        game.Events.Add(new GameFinished(notification.GameId));
      
        var Speed = DateTime.Now - game.Timer;
        var guesser = await _mediator.Send(new GetGuesserById.Request(notification.GuesserId), cancellationToken);
        
        await _mediator.Send(new UpdateGuesserStats.Request
        (guesser.Id, guesser.Points, Speed, guesser.Guesses, guesser.WrongGuessCounter), cancellationToken);
    }
}


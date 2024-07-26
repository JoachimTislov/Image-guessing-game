using System.Data.Common;
using Core.Domain.GameManagementContext.Events;
using Core.Domain.GameManagementContext.Pipelines;
using Infrastructure.Data;
using MediatR;

namespace Core.Domain.GameManagementContext.Handlers;

public class PlayerGuessedIncorrectlyHandler : INotificationHandler<PlayerGuessedIncorrectly> 
{
    private readonly IMediator _mediator;

    private readonly GameContext _db;

    public PlayerGuessedIncorrectlyHandler(IMediator mediator, GameContext db)
    {
        _mediator = mediator;
        _db = db;
    }
   
    public async Task Handle(PlayerGuessedIncorrectly notification, CancellationToken cancellationToken)
    {
        var guesser = await _mediator.Send(new GetGuesserById.Request(notification.GuesserId), cancellationToken);
    
        guesser.Guesses++;

        if(guesser.WrongGuessCounter == 3)
        {
            // Oracles Turn
            // await _mediator.Publish(new OraclesTurn(notification.GameId), cancellationToken);
            
            // This might be written too soon
            guesser.WrongGuessCounter = 0;
        }
        else
        {
            guesser.WrongGuessCounter++;
        }

        // This is maybe not the best place to do it
        // Move it into a pipeline later
        await _mediator.Send(new UpdateGuesserStats.Request(
        guesser.Id, guesser.Points, guesser.Speed, guesser.Guesses, guesser.WrongGuessCounter), cancellationToken);
    }
}

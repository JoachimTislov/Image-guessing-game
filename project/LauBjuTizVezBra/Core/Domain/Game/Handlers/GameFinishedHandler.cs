using Core.Domain.GameManagementContext.Events;
using Core.Domain.GameManagementContext.Pipelines;
using Core.Domain.SignalRContext.Hubs;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Core.Domain.GameManagementContext.Handlers;

public class GameFinishedHandler : INotificationHandler<GameFinished>
{
    private readonly IMediator _mediator;

    private readonly GameContext _db;

    public GameFinishedHandler(IMediator mediator, 
    IHubContext<GameHub, IGameClient> hubContext, GameContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    public async Task Handle(GameFinished notification, CancellationToken cancellationToken)
    {
        var game = await _mediator.Send(new GetGameById.Request(notification.GameId), cancellationToken);

        game.GameStatus = GameStatus.Finished;

        var guessersWhoLost = await _mediator.Send(new GetGuessersByGameIdAndAmountOfPoints.Request(game.Id), cancellationToken);

        //Remove guessers who lost
        foreach (var guesser in guessersWhoLost)
        {
            _db.Guessers.Remove(guesser);
        }

        _db.Games.Update(game);
        _db.Guessers.UpdateRange(guessersWhoLost);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
using Core.Domain.GameManagementContext.Events;
using Core.Domain.GameManagementContext.Pipelines;
using Core.Domain.ImageContext.Pipelines;
using Core.Domain.OracleContext.Pipelines;
using Core.Domain.UserContext;
using Infrastructure.Data;
using MediatR;

namespace Core.Domain.OracleContext.Handlers;

public class PlayerGuessedHandler : INotificationHandler<PlayerGuessed> 
{
    private readonly IMediator _mediator;

    private readonly GameContext _db;

    public PlayerGuessedHandler(IMediator mediator, GameContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    public async Task Handle(PlayerGuessed notification, CancellationToken cancellationToken)
    {
        /***********THIS HANDLES COMMON LOGIC FOR ALL ORACLES***********/

        var game = await _mediator.Send(new GetGameById.Request(Guid.Parse(notification.GameId)), cancellationToken);

        var Oracle = await _mediator.Send(new GetBaseOracleById.Request(game.OracleId), cancellationToken);

        Oracle.TotalGuesses++;

        _db.Oracles.Update(Oracle);
     
        var Image = await _mediator.Send(new GetImageDataByIdentifier.Request(Oracle.ImageIdentifier), cancellationToken);
        if(Image.Name == notification.Guess)
        {
            //Calculating amount of unrevealed tiles
            var points = Image.PieceCount - Oracle.NumberOfTilesRevealed;
            
            game.Events.Add(new PlayerGuessedCorrectly(points, game.Id, Guid.Parse(notification.GuesserId), game.GameMode, game.NumberOfGames));
        }
        else
        {
            game.Events.Add(new PlayerGuessedIncorrectly(Guid.Parse(notification.GuesserId)));
        } 

        await _db.SaveChangesAsync(cancellationToken);
    }
}
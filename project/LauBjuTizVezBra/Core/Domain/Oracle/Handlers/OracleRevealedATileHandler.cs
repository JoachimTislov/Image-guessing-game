using Core.Domain.GameManagementContext.Events;
using Core.Domain.OracleContext.Pipelines;
using Infrastructure.Data;
using MediatR;

namespace Core.Domain.OracleContext.Handlers;

public class OracleRevealedATileHandler : INotificationHandler<OracleRevealedATile> 
{
    private readonly IMediator _mediator;

    private readonly GameContext _db;

    public OracleRevealedATileHandler(IMediator mediator, GameContext db)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task Handle(OracleRevealedATile notification, CancellationToken cancellationToken)
    {   
        // This may cause issues
        var oracle = await _mediator.Send(new GetBaseOracleById.Request(notification.OracleId), cancellationToken);

        oracle.NumberOfTilesRevealed++;

        Console.WriteLine($"Number of tiles revealed: {oracle.NumberOfTilesRevealed}");
        
        _db.Oracles.Update(oracle);
        await _db.SaveChangesAsync(cancellationToken);


        //TODO: This is a bit of a mess, but it works for now

        // Copolit roasta meg 
    }
}
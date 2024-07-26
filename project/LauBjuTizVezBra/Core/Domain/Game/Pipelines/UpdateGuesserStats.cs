using Infrastructure.Data;
using MediatR;

namespace Core.Domain.GameManagementContext.Pipelines;

public class UpdateGuesserStats
{
    public record Request(Guid GuesserId, int Points, TimeSpan Speed, int Guesses, int WrongGuessCounter) : IRequest<Unit>;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly IMediator _mediator;

        private readonly GameContext _db;
        public Handler(IMediator mediator, GameContext db) {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _db = db ?? throw new ArgumentNullException(nameof(db));    
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var guesser = await _mediator.Send(new GetGuesserById.Request(request.GuesserId), cancellationToken);

            var game = await _mediator.Send(new GetGameById.Request(guesser.GameId), cancellationToken);

            if(game.GameStatus == GameStatus.Finished)
            {
                guesser.Points = request.Points;
                guesser.Speed = request.Speed;
                
                //Last guess is not counted, only wrong guesses
                guesser.Guesses += 1;
            }
            else // Game is still running
            {    // This is to update the guessers ongoing stats
                guesser.Guesses = request.Guesses;
                guesser.WrongGuessCounter = request.WrongGuessCounter;
            }

            _db.Guessers.Update(guesser);
            await _db.SaveChangesAsync(cancellationToken);
        
            return Unit.Value;
        }
    }
}
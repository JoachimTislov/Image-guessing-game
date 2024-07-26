using Core.Domain.UserContext;
using Infrastructure.Data;
using MediatR;


namespace Core.Domain.GameManagementContext.Pipelines;
public class CreateAndAddGuesserToDatabase
{
    public record Request(User User, Guid GameId) : IRequest<Guesser>;

    public class Handler : IRequestHandler<Request, Guesser>
    {
        private readonly GameContext _db;
        
        public Handler(GameContext db)
           => _db = db ?? throw new ArgumentNullException(nameof(db));
            
        public async Task<Guesser> Handle(Request request, CancellationToken cancellationToken)
        {
            var Guesser = new Guesser(request.User)
            {
                GameId = request.GameId
            };

            _db.Guessers.Add(Guesser);
            await _db.SaveChangesAsync(cancellationToken);

            return Guesser;
        }
    }
}
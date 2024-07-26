using Core.Domain.UserContext.Pipelines;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.GameManagementContext.Pipelines;

public class GetGuessersByGameIdAndAmountOfPoints
{
    public record Request(Guid GameId) : IRequest<List<Guesser>>;

    public class Handler : IRequestHandler<Request, List<Guesser>>
    {
        private readonly GameContext _db;

        public Handler(GameContext db) 
           => _db = db ?? throw new ArgumentNullException(nameof(db));
        
        public async Task<List<Guesser>> Handle(Request request, CancellationToken cancellationToken)
        {
            //*********GuesserId and GameId forms a unique key*********////
            var guessers = await _db.Guessers
                .Where(g => g.Points == 0 && g.GameId == request.GameId)
                .ToListAsync() ?? throw new Exception("Guesser not found");
                
            return guessers;    
        }
    }
}
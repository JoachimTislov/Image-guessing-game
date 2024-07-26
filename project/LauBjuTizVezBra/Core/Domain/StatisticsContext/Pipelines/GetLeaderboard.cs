using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.StatisticsContext.Pipelines;

public class GetLeaderboard
{
    public record Request(GameModeEnum GameMode) : IRequest<List<LeaderboardEntry>>;

    public class Handler : IRequestHandler<Request, List<LeaderboardEntry>>
    {
        private readonly GameContext _db;

        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<List<LeaderboardEntry>> Handle(Request request, CancellationToken cancellationToken)
        {
            var leaderboard = await _db.Leaderboards
                .Where(e => e.GameMode.ToLower() == request.GameMode.ToString()
                .ToLower()).OrderBy(e => e.Score)
                .ToListAsync(cancellationToken: cancellationToken);
            
            
            leaderboard.Reverse();


            // TEMPORARY
            if(leaderboard == null)
                return null;

            return leaderboard;
        }
    }
}
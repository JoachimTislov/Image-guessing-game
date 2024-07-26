using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Core.Domain.StatisticsContext.Pipelines;

public class AddRecentGameEntry
{
    public record Request(RecentGameEntry RecentGameEntry) : IRequest<Response>;

    public record Response(bool Success,RecentGameEntry LeaderboardEntry, string[] Errors);

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly GameContext _db;
        private readonly IEnumerable<IValidator<RecentGameEntry>> _validators;

        public Handler(GameContext db, IEnumerable<IValidator<RecentGameEntry>> validators)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var recentGameEntry = request.RecentGameEntry;

            _db.RecentGames.Add(recentGameEntry);

            var playerIds = new Guid?[] { recentGameEntry.GuesserId, recentGameEntry.OracleId };

            var recentGames = await _db.RecentGames.Where(rg => playerIds.Contains(rg.GuesserId)).OrderBy(rg => rg.Time).ToListAsync(cancellationToken: cancellationToken);
            if (recentGames.Count > 5)
            {
                _db.RecentGames.Remove(recentGames[0]);
            }

            var recentGames2 = await _db.RecentGames.Where(rg => playerIds.Contains(rg.OracleId)).OrderBy(rg => rg.Time).ToListAsync(cancellationToken: cancellationToken);
            if (recentGames2.Count > 5)
            {
                _db.RecentGames.Remove(recentGames2[0]);
            }
            
            await _db.SaveChangesAsync(cancellationToken);

            return new Response(true, recentGameEntry, Array.Empty<string>());
        }
    }
}
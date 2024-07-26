using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.StatisticsContext.Pipelines;

public class GetRecentGames
{
    public record Request(Guid UserId) : IRequest<List<RecentGameEntry?>>;

    public class Handler : IRequestHandler<Request, List<RecentGameEntry?>>
    {
        private readonly GameContext _db;

        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<List<RecentGameEntry?>> Handle(Request request, CancellationToken cancellationToken)
        {
            var recentGames = await _db.RecentGames
                    .Where(rg => rg.GuesserId == request.UserId)
                    .OrderBy(rg => rg.Time)
                    .ToListAsync(cancellationToken: cancellationToken);
            
            
            recentGames.Reverse();
            
            
            return recentGames;
        }
    }
}
using Infrastructure.Data;
using MediatR;
using SharedKernel;

namespace Core.Domain.StatisticsContext.Pipelines;

public class AddLeaderBoardEntry
{
    public record Request(LeaderboardEntry leaderboardEntry) : IRequest<Response>;

    public record Response(bool Success,LeaderboardEntry LeaderboardEntry, string[] Errors);

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly GameContext _db;
        private readonly IEnumerable<IValidator<LeaderboardEntry>> _validators;

        public Handler(GameContext db, IEnumerable<IValidator<LeaderboardEntry>> validators)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var leaderboardEntry = request.leaderboardEntry;

            _db.Leaderboards.Add(leaderboardEntry);
            await _db.SaveChangesAsync(cancellationToken);

            return new Response(true, leaderboardEntry, Array.Empty<string>());
        }
    }
}
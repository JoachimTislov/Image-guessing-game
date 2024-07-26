using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext.Pipelines;

public class GetAllSessions
{
    public record Request() : IRequest<List<Session>>;

    public class Handler : IRequestHandler<Request, List<Session>>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<List<Session>> Handle(Request request, CancellationToken cancellationToken)
        {
            var sessions = await _db.Sessions.OrderBy(s => s.CreationTime).ToListAsync(cancellationToken: cancellationToken);
            sessions.Reverse();
            return sessions;
        }
    }
}
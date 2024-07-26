using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext.Pipelines;

public class GetSessionBySessionHostId
{
    public record Request(Guid SessionHostId) : IRequest<Session?>;

    public class Handler : IRequestHandler<Request, Session?>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Session?> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await _db.Sessions
                .Where(s => s.SessionHostId == request.SessionHostId)
                .Include(s => s.SessionUsers)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
                // Getting only the first session,
                // because there should only be one session per user.
                // And avoid errors when there are multiple sessions per user.
            return session;
        }
    }
}
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext.Pipelines;

public class GetSessionById
{
    public record Request(Guid SessionId) : IRequest<Session>;

    public class Handler : IRequestHandler<Request, Session>
    {
        private readonly GameContext _db;
        public Handler(GameContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public async Task<Session> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = (await _db.Sessions
                .Where(s => s.Id == request.SessionId)
                .Include(s => s.SessionUsers)
                .SingleOrDefaultAsync(cancellationToken) ?? throw new Exception("Session not found"));
            
         
            return session;
        }
    }
}
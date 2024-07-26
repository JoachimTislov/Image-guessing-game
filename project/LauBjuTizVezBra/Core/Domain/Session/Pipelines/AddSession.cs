using Infrastructure.Data;
using MediatR;

namespace Core.Domain.SessionContext.Pipelines;

public class AddSession
{
    public record Request(Session Session) : IRequest<Response>;

    public record Response(bool Success, Session createdSession, string[] errors);

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly GameContext _db;
        
        public Handler(GameContext db)
            => _db = db ?? throw new ArgumentNullException(nameof(db));
        
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            _db.Sessions.Add(request.Session);
            await _db.SaveChangesAsync(cancellationToken);

            return new Response(true, request.Session, Array.Empty<string>());
        }
    }
}
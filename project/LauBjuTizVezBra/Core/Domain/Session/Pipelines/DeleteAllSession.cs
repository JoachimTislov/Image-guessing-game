using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Domain.SessionContext;

public class DeleteAllSession
{
    public record Request() : IRequest;

    public class Handler : IRequestHandler<Request>
    {
        private readonly GameContext _db;

        private readonly IMediator _mediator;

        public Handler(GameContext db, IMediator mediator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(db));
        }
      
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var sessions = await _db.Sessions.ToListAsync(cancellationToken: cancellationToken);
            
            foreach(var session in sessions)
            {
                await _mediator.Send(new DeleteSession.Request(session.Id), cancellationToken);
            }
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
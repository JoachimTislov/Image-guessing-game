using Core.Domain.SessionContext.Pipelines;
using Core.Domain.SessionContext.Services;
using Core.Domain.UserContext;
using Infrastructure.Data;
using MediatR;

namespace Core.Domain.SessionContext;

public class AddPlayerToSession
{
    public record Request(Guid SessionId, User User) : IRequest;

    public class Handler : IRequestHandler<Request>
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;
        private readonly ISessionService _sessionService;

        public Handler(GameContext db, ISessionService sessionService, IMediator mediator) 
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(_sessionService));
        }
        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            var session = await _mediator.Send(new GetSessionById.Request(request.SessionId), cancellationToken);

            if(request.User != null && session != null)
            {
                await _sessionService.JoinSession(request.User, session);
                _db.Sessions.Update(session);
            }
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
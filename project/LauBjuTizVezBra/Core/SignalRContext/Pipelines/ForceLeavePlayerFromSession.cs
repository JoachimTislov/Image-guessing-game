using Core.Domain.SessionContext;
using Core.Domain.SignalRContext.Hubs;
using Core.Domain.SignalRContext.Services;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Core.Domain.SignalRContext.Pipelines;

public class ForceLeavePlayerFromSession
{
    public record Request(User User, Session Session, bool ClosedSession) : IRequest;

    public class Handler : IRequestHandler<Request>
    {
        private readonly IMediator _mediator;
        private readonly IConnectionMappingService _connectionMappingService;
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public Handler(
                IMediator mediator, 
                IConnectionMappingService connectionMappingService,
                IHubContext<GameHub, IGameClient> hubContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _connectionMappingService = connectionMappingService ?? throw new ArgumentNullException(nameof(connectionMappingService));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
           var userConnection = _connectionMappingService.GetConnections(request.User.Id.ToString());

            if(userConnection != null) await _hubContext.Groups.RemoveFromGroupAsync(userConnection, request.Session.Id.ToString(), cancellationToken);
            
            if (request.Session.ChosenOracle == request.User.Id)
            {
                request.Session.ChosenOracle = request.Session.SessionHostId;
            }

            if(request.Session.SessionUsers.Count == 1 && request.Session.SessionHostId == request.User.Id)
            {
                await _mediator.Send(new DeleteSession.Request(request.Session.Id), cancellationToken);

            } else {

                await _mediator.Send(new RemovePlayerFromSession.Request(request.Session.Id, request.User), cancellationToken);
            }

            // Allows us to bypass the need for extensive front-end logic that is already handled by the back-end
            // await Clients.Groups(SessionId).RedirectToLink($"/Lobby/{SessionId}");
            if (!request.ClosedSession)
            {
                if(userConnection != null) await _hubContext.Clients.Client(userConnection).RedirectToLink("/");
                await _hubContext.Clients.Groups(request.Session.Id.ToString()).ReloadPage();
            }
            
            return Unit.Value;
        }
    }
}
 
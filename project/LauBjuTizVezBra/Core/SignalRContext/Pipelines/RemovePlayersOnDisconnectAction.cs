using Core.Domain.SessionContext.Pipelines;
using Core.Domain.SignalRContext.Hubs;
using Core.Domain.SignalRContext.Services;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Core.Domain.SignalRContext.Pipelines;

public class RemovePlayersOnDisconnectAction
{
    public record Request(Guid SessionId, User User) : IRequest;

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
            var sessionId = _connectionMappingService.GetGroups(request.User.Id.ToString());

            if(sessionId != string.Empty)
            {
                var session = await _mediator.Send(new GetSessionById.Request(Guid.Parse(sessionId)));
            
                if(request.User != null && session != null)
                {
                    await _connectionMappingService.RemoveFromGroup(request.User.Id, Guid.Parse(sessionId));
                    
                    if (session.SessionHostId == request.User.Id)
                    {
                        // This makes sure that the session is closed and deleted if the host leaves the session via the home button
                        var players = session.SessionUsers.ToList();
                        foreach (var player in players)
                        {
                            Console.WriteLine("Disconnected user: " + player.UserName + " from the session before closing it");

                            await _mediator.Send(new ForceLeavePlayerFromSession.Request(player, session, true), cancellationToken);
                            
                            // why do we need this ?
                            var userConnection = _connectionMappingService.GetConnections(player.Id.ToString());
                            if(userConnection != null) await _hubContext.Clients.Client(userConnection).RedirectToLink("/");
                        }
                    }
                    else
                    {
                        // Remove a single user from the session
                        await _mediator.Send(new ForceLeavePlayerFromSession.Request(request.User, session, false), cancellationToken);
                    } 
                }
            }
            return Unit.Value;
        }
    }
}
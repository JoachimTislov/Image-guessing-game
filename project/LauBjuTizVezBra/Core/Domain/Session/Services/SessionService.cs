using Core.Domain.SessionContext.Pipelines;
using Core.Domain.SignalRContext.Hubs;
using Core.Domain.UserContext;
using MediatR;
using Microsoft.AspNetCore.SignalR;

// Response record
using static Core.Domain.SessionContext.Services.ISessionService;

namespace Core.Domain.SessionContext.Services;

public class SessionService : ISessionService
{
    private readonly IMediator _mediator;
    private readonly IHubContext<GameHub, IGameClient> _hubContext;
    public SessionService(IMediator mediator, IHubContext<GameHub, IGameClient> hubContext)
    {
        _mediator = mediator;
        _hubContext = hubContext;
    }

    public async Task<Response> CreateSession(User user)
    {
        // temp to clean the database
        //await _mediator.Send(new DeleteAllSession.Request()); 

        var session = new Session(user);

        var result = await _mediator.Send(new AddSession.Request(session));

        await _hubContext.Groups.AddToGroupAsync(user.Id.ToString(), session.Id.ToString());
        
        if (result.Success) return new Response(true, result.createdSession);

        return new Response(false, result.createdSession);
    }

    public Task<bool> JoinSession(User user, Session session)
    {
        var result = session.AddUser(user);
        if (result) Task.FromResult(true);
        return Task.FromResult(false);
    }

    public Task<bool> LeaveSession(User user, Session session)
    {
        var result = session.RemoveUser(user);
        if (result != null) return Task.FromResult(true);
        return Task.FromResult(false);
    }
}
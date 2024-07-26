using Core.Domain.UserContext;

namespace Core.Domain.SessionContext.Services;

public interface ISessionService
{
    public record Response(bool Success, Session CreatedSession);
    Task<Response> CreateSession(User user);
    
    Task<bool> JoinSession(User user, Session session);

    Task<bool> LeaveSession(User user, Session session);

}
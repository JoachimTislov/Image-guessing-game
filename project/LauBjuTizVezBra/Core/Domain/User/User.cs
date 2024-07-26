using Core.Domain.OracleContext;
using Core.Domain.SessionContext;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain.UserContext;

public class User : IdentityUser<Guid>
{
    public User() { }
    public User(string username)
    {
        UserName = username;
    }
    
    public User(string username, Guid sessionId, Guid id)
    {
        Id = id;
        UserName = username;
        SessionId = sessionId;
    }
    public Guid? SessionId { get; set; }
}

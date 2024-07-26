using Core.Domain.UserContext;
using SharedKernel;

namespace Core.Domain.SessionContext;

public class Session : BaseEntity
{
    public Session() { }

    public Session(User user)
    {
        ChosenOracle = user.Id;
        SessionHostId = user.Id;
        SessionUsers.Add(user);
        Options = new Options();
        CreationTime = DateTime.Now;
        SessionStatus = SessionStatus.Lobby;
    }

    public Guid Id { get; protected set; }
    public Guid SessionHostId { get; set; }
    public Guid ChosenOracle { get; set; }
    public List<User> SessionUsers { get; set; } = new();
    public Options Options { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    public string? ImageIdentifier { get; set; }
    public string? ChosenImageName { get; set; } 
    public SessionStatus SessionStatus { get; set; } 
    public bool AddUser(User user)
    {
        if (SessionUsers.Contains(user) || user == null) return false;
        SessionUsers.Add(user);
        return true;
    }

    public string[] RemoveUser(User user)
    {
        if (!SessionUsers.Contains(user)) return new string[] { $"Could not find user {user.UserName}" };
        SessionUsers.Remove(user);
        return new string[] { "Removed" };
    }
}
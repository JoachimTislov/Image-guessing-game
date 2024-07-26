namespace Core.Domain.SignalRContext.Services;

public interface IConnectionMappingService 
{
    void AddConnection(string userId, string connectionId);
   
    Task RemoveConnection(string userId, string connectionId);
   
    string GetConnections(string userId);
    
    Task AddToGroup(Guid userId, Guid groupId);

    Task RemoveFromGroup(Guid userId, Guid groupId);

    string GetGroups(string userId);
   
}
using System.Collections.Concurrent;

namespace Core.Domain.SignalRContext.Services;

public class ConnectionMappingService : IConnectionMappingService
{
    // Added ConnectionMappingService to be able to implement consistent group connections and connectionIds for all clients in SignalR
    private readonly ConcurrentDictionary<string, string> _connections = new();
    private readonly ConcurrentDictionary<string, string> _groups = new();

    public void AddConnection(string userId, string connectionId)
    {
        if (userId != null && connectionId != null)
        {
            _connections[userId] = connectionId;
        }
    }

    public Task RemoveConnection(string userId, string connectionId)
    {
        if (userId != null && connectionId != null)
        {
            if (_connections.TryGetValue(userId, out var currentConnectionId) && currentConnectionId == connectionId)
            {
                
                _connections.TryRemove(userId, out _);
            }
        }

        return Task.CompletedTask;
    }

    public string GetConnections(string userId)
    {
        if (userId != null)
        {
            if (_connections.TryGetValue(userId, out var connectionId))
            {
                return connectionId;
            }
        }

        return string.Empty;
    }

    // Using the lock to ensure thread safety while doing async work

    public Task AddToGroup(Guid userId, Guid groupId)
    {
        // This will add the user with the groupId or update the existing one
        _groups.AddOrUpdate(userId.ToString(), groupId.ToString(), (key, oldValue) => groupId.ToString());

        return Task.CompletedTask;
    }

    public Task RemoveFromGroup(Guid userId, Guid groupId)
    {
        if (_groups.TryGetValue(userId.ToString(), out var currentGroupId) && currentGroupId == groupId.ToString())
        {
            _groups.TryRemove(userId.ToString(), out _);
        }

        return Task.CompletedTask;
    }

    public string GetGroups(string userId)
    {
        _groups.TryGetValue(userId, out var groupId);

        if (groupId != null)
        {
            return groupId;
        }
        else
        {
            return string.Empty;
        }
    }
}
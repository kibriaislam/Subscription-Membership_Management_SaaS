using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Hubs;

/// <summary>
/// SignalR Hub for real-time notifications
/// Clients connect to this hub to receive real-time notifications
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub
    /// Automatically adds the user to a group based on their user ID for targeted notifications
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            var groupName = GetUserGroupName(userId.Value);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("User {UserId} connected to NotificationHub. ConnectionId: {ConnectionId}", 
                userId.Value, Context.ConnectionId);
        }
        else
        {
            _logger.LogWarning("User connected to NotificationHub without authentication. ConnectionId: {ConnectionId}", 
                Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            var groupName = GetUserGroupName(userId.Value);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("User {UserId} disconnected from NotificationHub. ConnectionId: {ConnectionId}", 
                userId.Value, Context.ConnectionId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Error disconnecting from NotificationHub. ConnectionId: {ConnectionId}", 
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Gets the user ID from JWT claims
    /// </summary>
    private Guid? GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? Context.User?.FindFirst("sub")?.Value
            ?? Context.User?.FindFirst("userId")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Gets the group name for a specific user
    /// This allows sending notifications to specific users
    /// </summary>
    private static string GetUserGroupName(Guid userId) => $"user_{userId}";
}


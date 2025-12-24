using Application.DTOs.Notification;

namespace Application.Interfaces;

public interface INotificationService
{
    /// <summary>
    /// Sends a renewal reminder notification (legacy method for SMS/Email)
    /// </summary>
    Task SendRenewalReminderAsync(Guid membershipId, string recipientPhone, string recipientEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a real-time notification to a user via SignalR and persists it in the database
    /// </summary>
    Task SendNotificationAsync(CreateNotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to multiple users
    /// </summary>
    Task SendNotificationToUsersAsync(IEnumerable<Guid> userIds, CreateNotificationDto notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a notification as read
    /// </summary>
    Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks all notifications as read for a user
    /// </summary>
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
}


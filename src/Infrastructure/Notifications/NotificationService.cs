using Application.DTOs.Notification;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IHubClients _hubClients;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubClients hubClients,
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _hubClients = hubClients;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendRenewalReminderAsync(Guid membershipId, string recipientPhone, string recipientEmail, CancellationToken cancellationToken = default)
    {
        // MVP: Log the notification. In production, this would integrate with SMS/WhatsApp providers
        _logger.LogInformation(
            "Renewal reminder sent for Membership {MembershipId} to Phone: {Phone}, Email: {Email}",
            membershipId, recipientPhone, recipientEmail);

        // Placeholder for actual notification implementation
        // This would be replaced with actual SMS/WhatsApp service integration
        await Task.CompletedTask;
    }

    public async Task SendNotificationAsync(CreateNotificationDto notificationDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create and persist notification in database
            var notification = new Notification
            {
                UserId = notificationDto.UserId,
                BusinessId = notificationDto.BusinessId,
                Type = notificationDto.Type,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                RelatedEntityType = notificationDto.RelatedEntityType,
                RelatedEntityId = notificationDto.RelatedEntityId,
                Metadata = notificationDto.Metadata
            };

            await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send real-time notification via SignalR
            var groupName = $"user_{notificationDto.UserId}";
            await _hubClients.Group(groupName).SendAsync("ReceiveNotification", new NotificationDto
            {
                Id = notification.Id,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                RelatedEntityType = notification.RelatedEntityType,
                RelatedEntityId = notification.RelatedEntityId,
                Metadata = notification.Metadata
            }, cancellationToken);

            _logger.LogInformation(
                "Real-time notification sent to user {UserId}. NotificationId: {NotificationId}, Type: {Type}",
                notificationDto.UserId, notification.Id, notificationDto.Type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", notificationDto.UserId);
            throw;
        }
    }

    public async Task SendNotificationToUsersAsync(IEnumerable<Guid> userIds, CreateNotificationDto notificationDto, CancellationToken cancellationToken = default)
    {
        var tasks = userIds.Select(userId =>
        {
            var userNotification = new CreateNotificationDto
            {
                UserId = userId,
                BusinessId = notificationDto.BusinessId,
                Type = notificationDto.Type,
                Title = notificationDto.Title,
                Message = notificationDto.Message,
                RelatedEntityType = notificationDto.RelatedEntityType,
                RelatedEntityId = notificationDto.RelatedEntityId,
                Metadata = notificationDto.Metadata
            };
            return SendNotificationAsync(userNotification, cancellationToken);
        });

        await Task.WhenAll(tasks);
        _logger.LogInformation("Sent notification to {Count} users", userIds.Count());
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId, cancellationToken);
        
        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with ID {notificationId} not found");
        }

        if (notification.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not have access to this notification");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Notification {NotificationId} marked as read by user {UserId}", notificationId, userId);
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsDeleted && !n.IsRead,
            cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
            await _unitOfWork.Notifications.UpdateAsync(notification, cancellationToken);
        }

        if (unreadNotifications.Any())
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} notifications as read for user {UserId}", 
                unreadNotifications.Count(), userId);
        }
    }
}


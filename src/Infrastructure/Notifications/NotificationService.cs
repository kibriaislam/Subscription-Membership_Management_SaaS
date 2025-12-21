using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
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
}


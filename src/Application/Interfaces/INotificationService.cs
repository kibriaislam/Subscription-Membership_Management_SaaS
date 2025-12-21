namespace Application.Interfaces;

public interface INotificationService
{
    Task SendRenewalReminderAsync(Guid membershipId, string recipientPhone, string recipientEmail, CancellationToken cancellationToken = default);
}


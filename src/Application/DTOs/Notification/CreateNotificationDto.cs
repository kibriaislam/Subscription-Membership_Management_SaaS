using Domain.Enums;

namespace Application.DTOs.Notification;

/// <summary>
/// DTO for creating a new notification
/// </summary>
public class CreateNotificationDto
{
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? Metadata { get; set; }
}


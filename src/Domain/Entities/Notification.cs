using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a notification sent to a user
/// </summary>
public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BusinessId { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public string? Metadata { get; set; } // JSON string for additional data

    // Navigation property
    public User? User { get; set; }
}


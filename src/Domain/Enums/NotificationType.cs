namespace Domain.Enums;

/// <summary>
/// Types of notifications that can be sent to users
/// </summary>
public enum NotificationType
{
    MembershipRenewalReminder = 1,
    MembershipExpired = 2,
    PaymentReceived = 3,
    PaymentDue = 4,
    MembershipCreated = 5,
    MembershipUpdated = 6,
    MemberAdded = 7,
    SystemAlert = 8
}


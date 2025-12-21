using Domain.Enums;

namespace Application.DTOs.Membership;

public class MembershipDto
{
    public Guid Id { get; set; }
    public Guid MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public Guid SubscriptionPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public MembershipStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => TotalAmount - PaidAmount;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}


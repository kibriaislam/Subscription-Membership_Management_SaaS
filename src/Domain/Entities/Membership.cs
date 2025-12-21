using Domain.Enums;

namespace Domain.Entities;

public class Membership : BaseEntity
{
    public Guid BusinessId { get; set; }
    public Guid MemberId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Business? Business { get; set; }
    public Member? Member { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}


using Domain.Enums;

namespace Domain.Entities;

public class Payment : BaseEntity
{
    public Guid BusinessId { get; set; }
    public Guid MembershipId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Business? Business { get; set; }
    public Membership? Membership { get; set; }
}


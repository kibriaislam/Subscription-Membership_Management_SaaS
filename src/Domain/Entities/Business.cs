namespace Domain.Entities;

public class Business : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Currency { get; set; } = "USD";
    public string? TaxId { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public ICollection<Member> Members { get; set; } = new List<Member>();
    public ICollection<SubscriptionPlan> SubscriptionPlans { get; set; } = new List<SubscriptionPlan>();
}


namespace Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public Guid BusinessId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Business? Business { get; set; }
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}


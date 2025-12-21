namespace Domain.Entities;

public class Member : BaseEntity
{
    public Guid BusinessId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // Navigation properties
    public Business? Business { get; set; }
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}


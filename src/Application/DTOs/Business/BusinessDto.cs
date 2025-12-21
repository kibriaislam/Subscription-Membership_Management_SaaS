namespace Application.DTOs.Business;

public class BusinessDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public DateTime CreatedAt { get; set; }
}


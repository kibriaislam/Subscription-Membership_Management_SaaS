namespace Application.DTOs.Membership;

public class CreateMembershipDto
{
    public Guid MemberId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Notes { get; set; }
}


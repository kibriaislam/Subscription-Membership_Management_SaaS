using Application.DTOs.Plan;
using MediatR;

namespace Application.Features.Plans.UpdatePlan;

public class UpdatePlanCommand : IRequest<SubscriptionPlanDto>
{
    public Guid PlanId { get; set; }
    public UpdatePlanDto UpdateDto { get; set; } = null!;
}


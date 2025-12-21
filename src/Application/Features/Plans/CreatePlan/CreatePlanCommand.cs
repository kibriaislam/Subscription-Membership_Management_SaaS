using Application.DTOs.Plan;
using MediatR;

namespace Application.Features.Plans.CreatePlan;

public class CreatePlanCommand : IRequest<SubscriptionPlanDto>
{
    public CreatePlanDto CreateDto { get; set; } = null!;
}


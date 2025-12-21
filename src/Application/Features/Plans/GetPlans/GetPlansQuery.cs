using Application.DTOs.Plan;
using MediatR;

namespace Application.Features.Plans.GetPlans;

public class GetPlansQuery : IRequest<IEnumerable<SubscriptionPlanDto>>
{
}


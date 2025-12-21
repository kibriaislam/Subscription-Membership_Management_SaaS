using Application.DTOs.Plan;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Plans.GetPlans;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, IEnumerable<SubscriptionPlanDto>>
{
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPlansQueryHandler(
        ISubscriptionPlanRepository planRepository,
        ICurrentUserService currentUserService)
    {
        _planRepository = planRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<SubscriptionPlanDto>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var plans = await _planRepository.GetByBusinessIdAsync(_currentUserService.BusinessId.Value, cancellationToken);

        return plans.Select(p => new SubscriptionPlanDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            DurationDays = p.DurationDays,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        });
    }
}


using Application.DTOs.Plan;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Plans.UpdatePlan;

public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePlanCommandHandler(
        ISubscriptionPlanRepository planRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionPlanDto> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var plan = await _planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan == null || plan.BusinessId != _currentUserService.BusinessId.Value)
        {
            throw new KeyNotFoundException("Plan not found");
        }

        plan.Name = request.UpdateDto.Name;
        plan.Description = request.UpdateDto.Description;
        plan.Price = request.UpdateDto.Price;
        plan.DurationDays = request.UpdateDto.DurationDays;
        plan.IsActive = request.UpdateDto.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _planRepository.UpdateAsync(plan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SubscriptionPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            DurationDays = plan.DurationDays,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt
        };
    }
}


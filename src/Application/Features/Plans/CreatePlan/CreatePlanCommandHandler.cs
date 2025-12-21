using Application.DTOs.Plan;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Plans.CreatePlan;

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, SubscriptionPlanDto>
{
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlanCommandHandler(
        ISubscriptionPlanRepository planRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _planRepository = planRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<SubscriptionPlanDto> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var plan = new SubscriptionPlan
        {
            BusinessId = _currentUserService.BusinessId.Value,
            Name = request.CreateDto.Name,
            Description = request.CreateDto.Description,
            Price = request.CreateDto.Price,
            DurationDays = request.CreateDto.DurationDays,
            IsActive = true
        };

        await _planRepository.AddAsync(plan, cancellationToken);
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


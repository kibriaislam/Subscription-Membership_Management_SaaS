using FluentValidation;

namespace Application.Features.Plans.CreatePlan;

public class CreatePlanCommandValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanCommandValidator()
    {
        RuleFor(x => x.CreateDto.Name)
            .NotEmpty().WithMessage("Plan name is required")
            .MaximumLength(200).WithMessage("Plan name must not exceed 200 characters");

        RuleFor(x => x.CreateDto.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

        RuleFor(x => x.CreateDto.DurationDays)
            .GreaterThan(0).WithMessage("Duration must be greater than 0");
    }
}


using FluentValidation;

namespace Application.Features.Memberships.CreateMembership;

public class CreateMembershipCommandValidator : AbstractValidator<CreateMembershipCommand>
{
    public CreateMembershipCommandValidator()
    {
        RuleFor(x => x.CreateDto.MemberId)
            .NotEmpty().WithMessage("Member ID is required");

        RuleFor(x => x.CreateDto.SubscriptionPlanId)
            .NotEmpty().WithMessage("Subscription plan ID is required");
    }
}


using FluentValidation;

namespace Application.Features.Payments.CreatePayment;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.CreateDto.MembershipId)
            .NotEmpty().WithMessage("Membership ID is required");

        RuleFor(x => x.CreateDto.Amount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than 0");
    }
}


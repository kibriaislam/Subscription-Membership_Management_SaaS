using FluentValidation;

namespace Application.Features.Business.UpdateBusiness;

public class UpdateBusinessCommandValidator : AbstractValidator<UpdateBusinessCommand>
{
    public UpdateBusinessCommandValidator()
    {
        RuleFor(x => x.UpdateDto.Name)
            .NotEmpty().WithMessage("Business name is required")
            .MaximumLength(200).WithMessage("Business name must not exceed 200 characters");

        RuleFor(x => x.UpdateDto.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");
    }
}


using Application.DTOs.Payment;
using MediatR;

namespace Application.Features.Payments.CreatePayment;

public class CreatePaymentCommand : IRequest<PaymentDto>
{
    public CreatePaymentDto CreateDto { get; set; } = null!;
}


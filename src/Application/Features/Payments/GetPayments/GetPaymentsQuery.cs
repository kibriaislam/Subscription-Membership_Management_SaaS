using Application.DTOs.Payment;
using MediatR;

namespace Application.Features.Payments.GetPayments;

public class GetPaymentsQuery : IRequest<IEnumerable<PaymentDto>>
{
    public Guid? MembershipId { get; set; }
}


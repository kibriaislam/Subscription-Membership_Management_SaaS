using Application.DTOs.Payment;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Payments.GetPayments;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, IEnumerable<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPaymentsQueryHandler(
        IPaymentRepository paymentRepository,
        ICurrentUserService currentUserService)
    {
        _paymentRepository = paymentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        IEnumerable<Domain.Entities.Payment> payments;

        if (request.MembershipId.HasValue)
        {
            payments = await _paymentRepository.GetByMembershipIdAsync(request.MembershipId.Value, cancellationToken);
            payments = payments.Where(p => p.BusinessId == _currentUserService.BusinessId.Value);
        }
        else
        {
            payments = await _paymentRepository.GetByBusinessIdAsync(_currentUserService.BusinessId.Value, cancellationToken);
        }

        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            MembershipId = p.MembershipId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            PaymentMethod = p.PaymentMethod,
            TransactionReference = p.TransactionReference,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        }).OrderByDescending(p => p.PaymentDate);
    }
}


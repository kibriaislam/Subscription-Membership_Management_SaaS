using Application.DTOs.Payment;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Payments.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IMembershipRepository membershipRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _membershipRepository = membershipRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var membership = await _membershipRepository.GetByIdAsync(request.CreateDto.MembershipId, cancellationToken);
        if (membership == null || membership.BusinessId != _currentUserService.BusinessId.Value)
        {
            throw new KeyNotFoundException("Membership not found");
        }

        var payment = new Payment
        {
            BusinessId = _currentUserService.BusinessId.Value,
            MembershipId = request.CreateDto.MembershipId,
            Amount = request.CreateDto.Amount,
            PaymentDate = request.CreateDto.PaymentDate ?? DateTime.UtcNow,
            PaymentMethod = request.CreateDto.PaymentMethod,
            TransactionReference = request.CreateDto.TransactionReference,
            Notes = request.CreateDto.Notes
        };

        await _paymentRepository.AddAsync(payment, cancellationToken);

        // Update membership paid amount
        var totalPaid = await _paymentRepository.GetTotalPaidByMembershipIdAsync(request.CreateDto.MembershipId, cancellationToken);
        membership.PaidAmount = totalPaid + request.CreateDto.Amount;
        membership.UpdatedAt = DateTime.UtcNow;

        await _membershipRepository.UpdateAsync(membership, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PaymentDto
        {
            Id = payment.Id,
            MembershipId = payment.MembershipId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            PaymentMethod = payment.PaymentMethod,
            TransactionReference = payment.TransactionReference,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }
}


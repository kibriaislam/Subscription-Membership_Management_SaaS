using Domain.Enums;

namespace Application.DTOs.Payment;

public class CreatePaymentDto
{
    public Guid MembershipId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? PaymentDate { get; set; }
}


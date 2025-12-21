using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Receipts;

public class ReceiptService : IReceiptService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<ReceiptService> _logger;

    public ReceiptService(IPaymentRepository paymentRepository, ILogger<ReceiptService> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task<byte[]> GenerateReceiptPdfAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        // MVP: Return empty bytes. In production, use a library like QuestPDF or iTextSharp
        _logger.LogInformation("Generating PDF receipt for Payment {PaymentId}", paymentId);
        
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found");
        }

        // Placeholder - would generate actual PDF here
        await Task.CompletedTask;
        return Array.Empty<byte>();
    }

    public async Task<string> GenerateReceiptHtmlAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found");
        }

        // Generate simple HTML receipt
        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Receipt</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .details {{ margin: 20px 0; }}
        .amount {{ font-size: 24px; font-weight: bold; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Payment Receipt</h1>
    </div>
    <div class=""details"">
        <p><strong>Payment ID:</strong> {payment.Id}</p>
        <p><strong>Date:</strong> {payment.PaymentDate:yyyy-MM-dd HH:mm}</p>
        <p><strong>Amount:</strong> {payment.Amount:C}</p>
        <p><strong>Method:</strong> {payment.PaymentMethod}</p>
        {(!string.IsNullOrEmpty(payment.TransactionReference) ? $"<p><strong>Reference:</strong> {payment.TransactionReference}</p>" : "")}
    </div>
</body>
</html>";

        return html;
    }

    public async Task<string> GenerateShareableLinkAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        // MVP: Return a placeholder link. In production, generate a secure, time-limited shareable link
        await Task.CompletedTask;
        return $"/api/receipts/{paymentId}";
    }
}


namespace Application.Interfaces;

public interface IReceiptService
{
    Task<byte[]> GenerateReceiptPdfAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<string> GenerateReceiptHtmlAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<string> GenerateShareableLinkAsync(Guid paymentId, CancellationToken cancellationToken = default);
}


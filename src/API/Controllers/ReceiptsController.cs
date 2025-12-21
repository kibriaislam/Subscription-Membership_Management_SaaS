using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    [HttpGet("{paymentId}/html")]
    public async Task<ActionResult<string>> GetReceiptHtml(Guid paymentId, CancellationToken cancellationToken)
    {
        var html = await _receiptService.GenerateReceiptHtmlAsync(paymentId, cancellationToken);
        return Content(html, "text/html");
    }

    [HttpGet("{paymentId}/pdf")]
    public async Task<ActionResult> GetReceiptPdf(Guid paymentId, CancellationToken cancellationToken)
    {
        var pdfBytes = await _receiptService.GenerateReceiptPdfAsync(paymentId, cancellationToken);
        return File(pdfBytes, "application/pdf", $"receipt-{paymentId}.pdf");
    }

    [HttpGet("{paymentId}/link")]
    public async Task<ActionResult<string>> GetShareableLink(Guid paymentId, CancellationToken cancellationToken)
    {
        var link = await _receiptService.GenerateShareableLinkAsync(paymentId, cancellationToken);
        return Ok(new { link });
    }
}


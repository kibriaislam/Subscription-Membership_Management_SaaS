using API.Models;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Receipt generation controller for creating and sharing payment receipts.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[Tags("Receipts")]
public class ReceiptsController : BaseController
{
    private readonly IReceiptService _receiptService;

    public ReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    /// <summary>
    /// Generates an HTML receipt for a payment.
    /// </summary>
    /// <remarks>
    /// This endpoint generates an HTML-formatted receipt for a specific payment.
    /// The receipt includes payment details such as amount, date, payment method, and transaction reference.
    /// The HTML can be displayed in a browser or converted to PDF using external tools.
    /// </remarks>
    /// <param name="paymentId">The unique identifier of the payment.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with HTML content on success,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the payment does not exist.
    /// </returns>
    [HttpGet("{paymentId}/html")]
    [Produces("text/html")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetReceiptHtml(Guid paymentId, CancellationToken cancellationToken)
    {
        var html = await _receiptService.GenerateReceiptHtmlAsync(paymentId, cancellationToken);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Generates a PDF receipt for a payment.
    /// </summary>
    /// <remarks>
    /// This endpoint generates a PDF-formatted receipt for a specific payment.
    /// The PDF receipt includes all payment details and can be downloaded or printed.
    /// Note: PDF generation is currently a placeholder - returns empty bytes in MVP.
    /// </remarks>
    /// <param name="paymentId">The unique identifier of the payment.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with PDF file on success,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the payment does not exist.
    /// </returns>
    [HttpGet("{paymentId}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetReceiptPdf(Guid paymentId, CancellationToken cancellationToken)
    {
        var pdfBytes = await _receiptService.GenerateReceiptPdfAsync(paymentId, cancellationToken);
        return File(pdfBytes, "application/pdf", $"receipt-{paymentId}.pdf");
    }

    /// <summary>
    /// Generates a shareable link for a payment receipt.
    /// </summary>
    /// <remarks>
    /// This endpoint generates a shareable link that can be used to access a payment receipt.
    /// The link can be shared with customers or used for email notifications.
    /// Note: In MVP, this returns a placeholder link. In production, this would generate a secure, time-limited shareable URL.
    /// </remarks>
    /// <param name="paymentId">The unique identifier of the payment.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with the shareable link on success,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the payment does not exist.
    /// </returns>
    [HttpGet("{paymentId}/link")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> GetShareableLink(Guid paymentId, CancellationToken cancellationToken)
    {
        var link = await _receiptService.GenerateShareableLinkAsync(paymentId, cancellationToken);
        return OkResponse<object>(new { link }, "Shareable link generated successfully");
    }
}

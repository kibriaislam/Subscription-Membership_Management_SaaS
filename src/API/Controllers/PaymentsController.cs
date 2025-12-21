using Application.DTOs.Payment;
using Application.Features.Payments.CreatePayment;
using Application.Features.Payments.GetPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Payment management controller for recording and tracking payments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves payments with optional filtering by membership.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of payments associated with the authenticated user's business.
    /// Optionally filter by a specific membership ID to retrieve all payments for that membership.
    /// Results are ordered by payment date (newest first).
    /// Supports partial payments - multiple payments can be recorded for a single membership.
    /// </remarks>
    /// <param name="membershipId">Optional membership ID to filter payments for a specific membership.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with a list of payments on success,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(
        [FromQuery] Guid? membershipId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentsQuery { MembershipId = membershipId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Records a new payment for a membership.
    /// </summary>
    /// <remarks>
    /// This endpoint records a payment for a membership. The payment amount is automatically added
    /// to the membership's paid amount. Partial payments are supported - multiple payments can be
    /// recorded until the total membership amount is paid.
    /// If no payment date is provided, the current date and time is used.
    /// </remarks>
    /// <param name="createDto">The payment creation data containing membership ID, amount, payment method, and optional details.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 201 Created response with the created payment details on success,
    /// or a 400 Bad Request response if validation fails or amount is invalid,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the membership does not exist.
    /// </returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPayments), new { id = result.Id }, result);
    }
}

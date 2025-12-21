using Application.DTOs.Payment;
using Application.Features.Payments.CreatePayment;
using Application.Features.Payments.GetPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(
        [FromQuery] Guid? membershipId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentsQuery { MembershipId = membershipId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPayments), new { id = result.Id }, result);
    }
}


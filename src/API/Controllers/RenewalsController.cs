using Application.DTOs.Membership;
using Application.Features.Renewals.GetExpiringMemberships;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RenewalsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RenewalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<MembershipDto>>> GetExpiringMemberships(
        [FromQuery] int days = 7,
        CancellationToken cancellationToken = default)
    {
        var query = new GetExpiringMembershipsQuery { Days = days };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}


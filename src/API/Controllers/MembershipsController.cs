using Application.DTOs.Membership;
using Application.Features.Memberships.CreateMembership;
using Application.Features.Memberships.GetMemberships;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembershipsController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembershipsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MembershipDto>>> GetMemberships(
        [FromQuery] Guid? memberId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMembershipsQuery { MemberId = memberId };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MembershipDto>> CreateMembership([FromBody] CreateMembershipDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreateMembershipCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMemberships), new { id = result.Id }, result);
    }
}


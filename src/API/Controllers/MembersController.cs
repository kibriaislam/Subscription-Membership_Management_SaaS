using Application.DTOs.Member;
using Application.Features.Member.CreateMember;
using Application.Features.Member.DeactivateMember;
using Application.Features.Member.GetMembers;
using Application.Features.Member.UpdateMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedMemberResponseDto>> GetMembers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMembersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MemberDto>> CreateMember([FromBody] CreateMemberDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreateMemberCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMembers), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<MemberDto>> UpdateMember(Guid id, [FromBody] UpdateMemberDto updateDto, CancellationToken cancellationToken)
    {
        var command = new UpdateMemberCommand { MemberId = id, UpdateDto = updateDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateMember(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateMemberCommand { MemberId = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}


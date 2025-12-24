using API.Models;
using Application.DTOs.Membership;
using Application.Features.Memberships.CreateMembership;
using Application.Features.Memberships.GetMemberships;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Membership management controller for assigning subscription plans to members.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Memberships")]
public class MembershipsController : BaseController
{
    private readonly IMediator _mediator;

    public MembershipsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves memberships with optional filtering by member.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of memberships associated with the authenticated user's business.
    /// Optionally filter by a specific member ID to retrieve all memberships for that member.
    /// Results are ordered by creation date (newest first).
    /// </remarks>
    /// <param name="memberId">Optional member ID to filter memberships for a specific member.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with a list of memberships on success,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MembershipDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MembershipDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MembershipDto>>>> GetMemberships(
        [FromQuery] Guid? memberId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMembershipsQuery { MemberId = memberId };
        var result = await _mediator.Send(query, cancellationToken);
        return OkResponse(result, "Memberships retrieved successfully");
    }

    /// <summary>
    /// Creates a new membership by assigning a subscription plan to a member.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new membership by assigning a subscription plan to a member.
    /// The expiry date is automatically calculated based on the plan's duration.
    /// Overlapping memberships for the same member are prevented.
    /// If no start date is provided, the current date is used.
    /// </remarks>
    /// <param name="createDto">The membership creation data containing member ID, plan ID, and optional start date.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 201 Created response with the created membership details on success,
    /// or a 400 Bad Request response if validation fails or overlapping membership exists,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the member or plan does not exist.
    /// </returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<MembershipDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<MembershipDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MembershipDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<MembershipDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MembershipDto>>> CreateMembership([FromBody] CreateMembershipDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreateMembershipCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedResponse(result, nameof(GetMemberships), new { id = result.Id }, "Membership created successfully");
    }
}

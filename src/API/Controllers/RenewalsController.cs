using API.Models;
using Application.DTOs.Membership;
using Application.Features.Renewals.GetExpiringMemberships;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Renewal management controller for tracking expiring memberships.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Renewals")]
public class RenewalsController : BaseController
{
    private readonly IMediator _mediator;

    public RenewalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves memberships that are expiring within a specified number of days.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of active memberships that are expiring within the specified number of days.
    /// Useful for sending renewal reminders and planning follow-up actions.
    /// Results are ordered by expiry date (earliest first).
    /// Default is 7 days if not specified.
    /// </remarks>
    /// <param name="days">The number of days to look ahead for expiring memberships (default: 7).</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with a list of expiring memberships on success,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpGet("expiring")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MembershipDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MembershipDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<MembershipDto>>>> GetExpiringMemberships(
        [FromQuery] int days = 7,
        CancellationToken cancellationToken = default)
    {
        var query = new GetExpiringMembershipsQuery { Days = days };
        var result = await _mediator.Send(query, cancellationToken);
        return OkResponse(result, "Expiring memberships retrieved successfully");
    }
}

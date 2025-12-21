using Application.DTOs.Dashboard;
using Application.Features.Dashboard.GetDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Dashboard controller for retrieving business statistics and analytics.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves comprehensive dashboard statistics for the authenticated user's business.
    /// </summary>
    /// <remarks>
    /// This endpoint returns key business metrics including:
    /// - Total number of members
    /// - Active and expired member counts
    /// - Renewals due today and within the next 7 days
    /// - Monthly collection summary (current month)
    /// - Total outstanding payments across all active memberships
    /// 
    /// All statistics are calculated in real-time based on the current business data.
    /// </remarks>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with dashboard statistics on success,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DashboardStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var query = new GetDashboardStatsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

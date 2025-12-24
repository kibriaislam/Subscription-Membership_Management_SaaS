using API.Models;
using Application.DTOs.Plan;
using Application.Features.Plans.CreatePlan;
using Application.Features.Plans.GetPlans;
using Application.Features.Plans.UpdatePlan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Subscription plan management controller for creating and managing subscription plans.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Subscription Plans")]
public class PlansController : BaseController
{
    private readonly IMediator _mediator;

    public PlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all subscription plans for the authenticated user's business.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a list of all subscription plans (both active and inactive) 
    /// associated with the authenticated user's business. Plans are ordered alphabetically by name.
    /// </remarks>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with a list of subscription plans on success,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubscriptionPlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SubscriptionPlanDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<SubscriptionPlanDto>>>> GetPlans(CancellationToken cancellationToken)
    {
        var query = new GetPlansQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return OkResponse(result, "Subscription plans retrieved successfully");
    }

    /// <summary>
    /// Creates a new subscription plan for the authenticated user's business.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new subscription plan with specified price and duration.
    /// The plan is automatically set as active upon creation. Price must be greater than or equal to 0,
    /// and duration must be specified in days (must be greater than 0).
    /// </remarks>
    /// <param name="createDto">The subscription plan creation data containing name, price, and duration.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 201 Created response with the created plan details on success,
    /// or a 400 Bad Request response if validation fails,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<SubscriptionPlanDto>>> CreatePlan([FromBody] CreatePlanDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreatePlanCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedResponse(result, nameof(GetPlans), new { id = result.Id }, "Subscription plan created successfully");
    }

    /// <summary>
    /// Updates an existing subscription plan.
    /// </summary>
    /// <remarks>
    /// This endpoint updates subscription plan details such as name, description, price, duration, and active status.
    /// The plan must belong to the authenticated user's business. Plans that are in use cannot be deleted,
    /// but can be deactivated by setting IsActive to false.
    /// </remarks>
    /// <param name="id">The unique identifier of the plan to update.</param>
    /// <param name="updateDto">The plan update data.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with the updated plan details on success,
    /// or a 400 Bad Request response if validation fails,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the plan does not exist or does not belong to the user's business.
    /// </returns>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<SubscriptionPlanDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<SubscriptionPlanDto>>> UpdatePlan(Guid id, [FromBody] UpdatePlanDto updateDto, CancellationToken cancellationToken)
    {
        var command = new UpdatePlanCommand { PlanId = id, UpdateDto = updateDto };
        var result = await _mediator.Send(command, cancellationToken);
        return OkResponse(result, "Subscription plan updated successfully");
    }
}


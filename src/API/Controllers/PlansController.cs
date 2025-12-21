using Application.DTOs.Plan;
using Application.Features.Plans.CreatePlan;
using Application.Features.Plans.GetPlans;
using Application.Features.Plans.UpdatePlan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubscriptionPlanDto>>> GetPlans(CancellationToken cancellationToken)
    {
        var query = new GetPlansQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionPlanDto>> CreatePlan([FromBody] CreatePlanDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreatePlanCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPlans), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubscriptionPlanDto>> UpdatePlan(Guid id, [FromBody] UpdatePlanDto updateDto, CancellationToken cancellationToken)
    {
        var command = new UpdatePlanCommand { PlanId = id, UpdateDto = updateDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}


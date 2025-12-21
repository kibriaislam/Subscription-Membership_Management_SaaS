using Application.DTOs.Business;
using Application.Features.Business.GetBusiness;
using Application.Features.Business.UpdateBusiness;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BusinessController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<BusinessDto>> GetBusiness(CancellationToken cancellationToken)
    {
        var query = new GetBusinessQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<BusinessDto>> UpdateBusiness([FromBody] UpdateBusinessDto updateDto, CancellationToken cancellationToken)
    {
        var command = new UpdateBusinessCommand { UpdateDto = updateDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}


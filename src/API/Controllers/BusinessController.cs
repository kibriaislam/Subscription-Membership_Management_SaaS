using Application.DTOs.Business;
using Application.Features.Business.GetBusiness;
using Application.Features.Business.UpdateBusiness;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Business profile management controller for retrieving and updating business information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Business")]
public class BusinessController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the current authenticated user's business profile.
    /// </summary>
    /// <remarks>
    /// This endpoint returns the complete business profile associated with the authenticated user.
    /// The business information is automatically filtered based on the JWT token's BusinessId claim.
    /// </remarks>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with the business profile on success,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the business profile does not exist.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(BusinessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusinessDto>> GetBusiness(CancellationToken cancellationToken)
    {
        var query = new GetBusinessQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Updates the current authenticated user's business profile.
    /// </summary>
    /// <remarks>
    /// This endpoint allows updating business information such as name, description, address, contact details, and currency.
    /// All fields are optional, but at least one field must be provided. The currency must be a valid 3-character ISO code (e.g., USD, EUR, GBP).
    /// </remarks>
    /// <param name="updateDto">The business profile update data.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with the updated business profile on success,
    /// or a 400 Bad Request response if validation fails,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the business profile does not exist.
    /// </returns>
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BusinessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BusinessDto>> UpdateBusiness([FromBody] UpdateBusinessDto updateDto, CancellationToken cancellationToken)
    {
        var command = new UpdateBusinessCommand { UpdateDto = updateDto };
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}


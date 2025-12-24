using API.Models;
using Application.DTOs.Member;
using Application.Features.Member.CreateMember;
using Application.Features.Member.DeactivateMember;
using Application.Features.Member.GetMembers;
using Application.Features.Member.UpdateMember;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Member management controller for CRUD operations on business members.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Members")]
public class MembersController : BaseController
{
    private readonly IMediator _mediator;

    public MembersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a paginated list of members with optional search functionality.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a paginated list of members associated with the authenticated user's business.
    /// Supports searching by first name, last name, email, or phone number.
    /// Results are ordered by last name, then first name.
    /// </remarks>
    /// <param name="pageNumber">The page number to retrieve (default: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10, max: 100).</param>
    /// <param name="searchTerm">Optional search term to filter members by name, email, or phone.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with a paged list of members on success,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedMemberResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedMemberResponseDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PagedMemberResponseDto>>> GetMembers(
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
        return OkResponse(result, "Members retrieved successfully");
    }

    /// <summary>
    /// Creates a new member for the authenticated user's business.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new member record associated with the authenticated user's business.
    /// The member is automatically set as active upon creation. Email address is optional but must be valid if provided.
    /// </remarks>
    /// <param name="createDto">The member creation data containing personal information.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 201 Created response with the created member details on success,
    /// or a 400 Bad Request response if validation fails,
    /// or a 401 Unauthorized response if the user is not authenticated.
    /// </returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<MemberDto>>> CreateMember([FromBody] CreateMemberDto createDto, CancellationToken cancellationToken)
    {
        var command = new CreateMemberCommand { CreateDto = createDto };
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedResponse(result, nameof(GetMembers), new { id = result.Id }, "Member created successfully");
    }

    /// <summary>
    /// Updates an existing member's information.
    /// </summary>
    /// <remarks>
    /// This endpoint updates member details such as name, contact information, and status.
    /// The member must belong to the authenticated user's business. All fields are optional.
    /// </remarks>
    /// <param name="id">The unique identifier of the member to update.</param>
    /// <param name="updateDto">The member update data.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 200 OK response with the updated member details on success,
    /// or a 400 Bad Request response if validation fails,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the member does not exist or does not belong to the user's business.
    /// </returns>
    [HttpPut("{id}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<MemberDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<MemberDto>>> UpdateMember(Guid id, [FromBody] UpdateMemberDto updateDto, CancellationToken cancellationToken)
    {
        var command = new UpdateMemberCommand { MemberId = id, UpdateDto = updateDto };
        var result = await _mediator.Send(command, cancellationToken);
        return OkResponse(result, "Member updated successfully");
    }

    /// <summary>
    /// Deactivates a member (soft delete).
    /// </summary>
    /// <remarks>
    /// This endpoint deactivates a member by setting their IsActive status to false.
    /// This is a soft delete operation - the member record is retained but marked as inactive.
    /// The member must belong to the authenticated user's business.
    /// </remarks>
    /// <param name="id">The unique identifier of the member to deactivate.</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>
    /// Returns a 204 No Content response on successful deactivation,
    /// or a 401 Unauthorized response if the user is not authenticated,
    /// or a 404 Not Found response if the member does not exist or does not belong to the user's business.
    /// </returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeactivateMember(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateMemberCommand { MemberId = id };
        await _mediator.Send(command, cancellationToken);
        return NoContentResponse("Member deactivated successfully");
    }
}


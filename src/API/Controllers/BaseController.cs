using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Base controller providing consistent response formatting methods for all API controllers.
/// </summary>
[ApiController]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Returns a successful response with data (200 OK).
    /// </summary>
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string message = "Operation completed successfully")
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status200OK));
    }

    /// <summary>
    /// Returns a successful response with data (200 OK).
    /// </summary>
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data)
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, "Operation completed successfully", StatusCodes.Status200OK));
    }

    /// <summary>
    /// Returns a created response with data (201 Created).
    /// </summary>
    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(T data, string actionName, object? routeValues = null, string message = "Resource created successfully")
    {
        return CreatedAtAction(actionName, routeValues, ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status201Created));
    }

    /// <summary>
    /// Returns a created response with data (201 Created).
    /// </summary>
    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(T data, string message = "Resource created successfully")
    {
        return StatusCode(StatusCodes.Status201Created, ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status201Created));
    }

    /// <summary>
    /// Returns a no content response (204 No Content).
    /// </summary>
    protected ActionResult<ApiResponse> NoContentResponse(string message = "Operation completed successfully")
    {
        return StatusCode(StatusCodes.Status204NoContent, ApiResponse.SuccessResponse(message, StatusCodes.Status204NoContent));
    }

    /// <summary>
    /// Returns a bad request response (400 Bad Request).
    /// </summary>
    protected ActionResult<ApiResponse<T>> BadRequestResponse<T>(string message, List<string>? errors = null)
    {
        return BadRequest(ApiResponse<T>.ErrorResponse(message, StatusCodes.Status400BadRequest, errors));
    }

    /// <summary>
    /// Returns a bad request response (400 Bad Request).
    /// </summary>
    protected ActionResult<ApiResponse> BadRequestResponse(string message, List<string>? errors = null)
    {
        return BadRequest(ApiResponse.ErrorResponse(message, StatusCodes.Status400BadRequest, errors));
    }

    /// <summary>
    /// Returns an unauthorized response (401 Unauthorized).
    /// </summary>
    protected ActionResult<ApiResponse<T>> UnauthorizedResponse<T>(string message = "Unauthorized access")
    {
        return Unauthorized(ApiResponse<T>.ErrorResponse(message, StatusCodes.Status401Unauthorized));
    }

    /// <summary>
    /// Returns an unauthorized response (401 Unauthorized).
    /// </summary>
    protected ActionResult<ApiResponse> UnauthorizedResponse(string message = "Unauthorized access")
    {
        return Unauthorized(ApiResponse.ErrorResponse(message, StatusCodes.Status401Unauthorized));
    }

    /// <summary>
    /// Returns a not found response (404 Not Found).
    /// </summary>
    protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message = "Resource not found")
    {
        return NotFound(ApiResponse<T>.ErrorResponse(message, StatusCodes.Status404NotFound));
    }

    /// <summary>
    /// Returns a not found response (404 Not Found).
    /// </summary>
    protected ActionResult<ApiResponse> NotFoundResponse(string message = "Resource not found")
    {
        return NotFound(ApiResponse.ErrorResponse(message, StatusCodes.Status404NotFound));
    }

    /// <summary>
    /// Returns a conflict response (409 Conflict).
    /// </summary>
    protected ActionResult<ApiResponse<T>> ConflictResponse<T>(string message, List<string>? errors = null)
    {
        return Conflict(ApiResponse<T>.ErrorResponse(message, StatusCodes.Status409Conflict, errors));
    }

    /// <summary>
    /// Returns a conflict response (409 Conflict).
    /// </summary>
    protected ActionResult<ApiResponse> ConflictResponse(string message, List<string>? errors = null)
    {
        return Conflict(ApiResponse.ErrorResponse(message, StatusCodes.Status409Conflict, errors));
    }

    /// <summary>
    /// Returns an internal server error response (500 Internal Server Error).
    /// </summary>
    protected ActionResult<ApiResponse<T>> InternalServerErrorResponse<T>(string message = "An error occurred while processing your request")
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<T>.ErrorResponse(message, StatusCodes.Status500InternalServerError));
    }

    /// <summary>
    /// Returns an internal server error response (500 Internal Server Error).
    /// </summary>
    protected ActionResult<ApiResponse> InternalServerErrorResponse(string message = "An error occurred while processing your request")
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.ErrorResponse(message, StatusCodes.Status500InternalServerError));
    }
}


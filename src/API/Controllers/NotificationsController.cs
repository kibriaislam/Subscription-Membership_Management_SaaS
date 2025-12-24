using API.Models;
using Application.DTOs.Notification;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Notifications controller for managing user notifications.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Notifications")]
public class NotificationsController : BaseController
{
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public NotificationsController(
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Gets all notifications for the authenticated user.
    /// </summary>
    /// <param name="unreadOnly">If true, returns only unread notifications</param>
    /// <param name="skip">Number of notifications to skip for pagination</param>
    /// <param name="take">Number of notifications to return (default: 50, max: 100)</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>List of notifications</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDto>>>> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return UnauthorizedResponse<IEnumerable<NotificationDto>>("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;
        take = Math.Min(take, 100); // Cap at 100

        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && 
                 !n.IsDeleted && 
                 (!unreadOnly || !n.IsRead),
            cancellationToken);

        var orderedNotifications = notifications
            .OrderByDescending(n => n.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityId = n.RelatedEntityId,
                Metadata = n.Metadata
            })
            .ToList();

        return OkResponse<IEnumerable<NotificationDto>>(orderedNotifications, "Notifications retrieved successfully");
    }

    /// <summary>
    /// Gets the count of unread notifications for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>Count of unread notifications</returns>
    [HttpGet("unread/count")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<object>>> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return UnauthorizedResponse<object>("User not authenticated");
        }

        var userId = _currentUserService.UserId.Value;
        var count = await _unitOfWork.Notifications.CountAsync(
            n => n.UserId == userId && !n.IsDeleted && !n.IsRead,
            cancellationToken);

        return OkResponse<object>(new { count }, "Unread notification count retrieved successfully");
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="id">The ID of the notification to mark as read</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/read")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return UnauthorizedResponse("User not authenticated");
        }

        try
        {
            await _notificationService.MarkAsReadAsync(id, _currentUserService.UserId.Value, cancellationToken);
            return NoContentResponse("Notification marked as read");
        }
        catch (InvalidOperationException)
        {
            return NotFoundResponse("Notification not found");
        }
        catch (UnauthorizedAccessException)
        {
            return UnauthorizedResponse("Unauthorized to access this notification");
        }
    }

    /// <summary>
    /// Marks all notifications as read for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>No content on success</returns>
    [HttpPost("read-all")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse>> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return UnauthorizedResponse("User not authenticated");
        }

        await _notificationService.MarkAllAsReadAsync(_currentUserService.UserId.Value, cancellationToken);
        return NoContentResponse("All notifications marked as read");
    }
}


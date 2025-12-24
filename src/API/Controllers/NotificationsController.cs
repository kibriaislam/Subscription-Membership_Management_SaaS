using Application.DTOs.Notification;
using Application.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Notifications controller for managing user notifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
[Tags("Notifications")]
public class NotificationsController : ControllerBase
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
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Unauthorized();
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

        return Ok(orderedNotifications);
    }

    /// <summary>
    /// Gets the count of unread notifications for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>Count of unread notifications</returns>
    [HttpGet("unread/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Unauthorized();
        }

        var userId = _currentUserService.UserId.Value;
        var count = await _unitOfWork.Notifications.CountAsync(
            n => n.UserId == userId && !n.IsDeleted && !n.IsRead,
            cancellationToken);

        return Ok(count);
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="id">The ID of the notification to mark as read</param>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            await _notificationService.MarkAsReadAsync(id, _currentUserService.UserId.Value, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Marks all notifications as read for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for request cancellation.</param>
    /// <returns>No content on success</returns>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        if (!_currentUserService.UserId.HasValue)
        {
            return Unauthorized();
        }

        await _notificationService.MarkAllAsReadAsync(_currentUserService.UserId.Value, cancellationToken);
        return NoContent();
    }
}


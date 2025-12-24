using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

/// <summary>
/// Service for processing expired memberships.
/// This service is designed to be called by Hangfire background jobs.
/// </summary>
public class MembershipExpiryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MembershipExpiryService> _logger;

    public MembershipExpiryService(
        IUnitOfWork unitOfWork,
        ILogger<MembershipExpiryService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Processes expired memberships by marking them as expired.
    /// This method is designed to be called by Hangfire recurring jobs.
    /// </summary>
    public async Task ProcessExpiredMembershipsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting membership expiry processing job");

            var now = DateTime.UtcNow;
            var expiredMemberships = await _unitOfWork.Memberships.FindAsync(
                m => m.Status == MembershipStatus.Active && 
                     m.ExpiryDate <= now && 
                     !m.IsDeleted,
                cancellationToken);

            var count = 0;
            foreach (var membership in expiredMemberships)
            {
                membership.Status = MembershipStatus.Expired;
                membership.UpdatedAt = now;
                await _unitOfWork.Memberships.UpdateAsync(membership, cancellationToken);
                count++;
            }

            if (count > 0)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully marked {Count} memberships as expired", count);
            }
            else
            {
                _logger.LogInformation("No expired memberships found to process");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired memberships");
            throw; // Re-throw to let Hangfire handle retry logic
        }
    }
}


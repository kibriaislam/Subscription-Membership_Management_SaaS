using Domain.Enums;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class MembershipExpiryJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MembershipExpiryJob> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run daily

    public MembershipExpiryJob(
        IServiceProvider serviceProvider,
        ILogger<MembershipExpiryJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredMembershipsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing expired memberships");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiredMembershipsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var now = DateTime.UtcNow;
        var expiredMemberships = await unitOfWork.Memberships.FindAsync(
            m => m.Status == MembershipStatus.Active && 
                 m.ExpiryDate <= now && 
                 !m.IsDeleted,
            cancellationToken);

        var count = 0;
        foreach (var membership in expiredMemberships)
        {
            membership.Status = MembershipStatus.Expired;
            membership.UpdatedAt = now;
            await unitOfWork.Memberships.UpdateAsync(membership, cancellationToken);
            count++;
        }

        if (count > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Marked {Count} memberships as expired", count);
        }
    }
}


using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MembershipRepository : Repository<Membership>, IMembershipRepository
{
    public MembershipRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Membership>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.SubscriptionPlan)
            .Where(m => m.BusinessId == businessId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Membership>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.SubscriptionPlan)
            .Where(m => m.MemberId == memberId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Membership>> GetActiveByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.SubscriptionPlan)
            .Where(m => m.BusinessId == businessId && 
                       m.Status == MembershipStatus.Active && 
                       m.ExpiryDate > now && 
                       !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Membership>> GetExpiredByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.SubscriptionPlan)
            .Where(m => m.BusinessId == businessId && 
                       (m.Status == MembershipStatus.Expired || m.ExpiryDate <= now) && 
                       !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Membership>> GetExpiringWithinDaysAsync(Guid businessId, int days, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var targetDate = now.AddDays(days);
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.SubscriptionPlan)
            .Where(m => m.BusinessId == businessId && 
                       m.Status == MembershipStatus.Active && 
                       m.ExpiryDate >= now && 
                       m.ExpiryDate <= targetDate && 
                       !m.IsDeleted)
            .OrderBy(m => m.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingMembershipAsync(
        Guid memberId, 
        DateTime startDate, 
        DateTime expiryDate, 
        Guid? excludeMembershipId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => 
            m.MemberId == memberId && 
            m.Status == MembershipStatus.Active && 
            !m.IsDeleted &&
            ((m.StartDate <= startDate && m.ExpiryDate >= startDate) ||
             (m.StartDate <= expiryDate && m.ExpiryDate >= expiryDate) ||
             (m.StartDate >= startDate && m.ExpiryDate <= expiryDate)));

        if (excludeMembershipId.HasValue)
        {
            query = query.Where(m => m.Id != excludeMembershipId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Membership>> GetByStatusAsync(Guid businessId, MembershipStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Member)
            .Include(m => m.SubscriptionPlan)
            .Where(m => m.BusinessId == businessId && m.Status == status && !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}


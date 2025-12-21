using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubscriptionPlanRepository : Repository<SubscriptionPlan>, ISubscriptionPlanRepository
{
    public SubscriptionPlanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<SubscriptionPlan>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.BusinessId == businessId && !p.IsDeleted)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPlanInUseAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await _context.Memberships
            .AnyAsync(m => m.SubscriptionPlanId == planId && !m.IsDeleted, cancellationToken);
    }
}


using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Payment>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Membership)
            .Where(p => p.BusinessId == businessId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.MembershipId == membershipId && !p.IsDeleted)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalPaidByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.MembershipId == membershipId && !p.IsDeleted)
            .SumAsync(p => p.Amount, cancellationToken);
    }
}


using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BusinessRepository : Repository<Business>, IBusinessRepository
{
    public BusinessRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Business?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.UserId == userId && !b.IsDeleted, cancellationToken);
    }
}


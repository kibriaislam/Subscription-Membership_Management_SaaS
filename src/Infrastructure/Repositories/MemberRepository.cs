using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MemberRepository : Repository<Member>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Member>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.BusinessId == businessId && !m.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Member> Items, int TotalCount)> GetPagedByBusinessIdAsync(
        Guid businessId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(m => m.BusinessId == businessId && !m.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(searchTerm) ||
                m.LastName.ToLower().Contains(searchTerm) ||
                (m.Email != null && m.Email.ToLower().Contains(searchTerm)) ||
                (m.Phone != null && m.Phone.Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.LastName)
            .ThenBy(m => m.FirstName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}


using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IMemberRepository : IRepository<Member>
{
    Task<IEnumerable<Member>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Member> Items, int TotalCount)> GetPagedByBusinessIdAsync(
        Guid businessId,
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
}


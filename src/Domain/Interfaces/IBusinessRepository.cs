using Domain.Entities;

namespace Domain.Interfaces;

public interface IBusinessRepository : IRepository<Business>
{
    Task<Business?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}


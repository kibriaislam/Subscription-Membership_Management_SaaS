using Domain.Entities;

namespace Domain.Interfaces;

public interface ISubscriptionPlanRepository : IRepository<SubscriptionPlan>
{
    Task<IEnumerable<SubscriptionPlan>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<bool> IsPlanInUseAsync(Guid planId, CancellationToken cancellationToken = default);
}


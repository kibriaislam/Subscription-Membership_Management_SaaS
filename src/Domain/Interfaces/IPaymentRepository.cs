using Domain.Entities;

namespace Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalPaidByMembershipIdAsync(Guid membershipId, CancellationToken cancellationToken = default);
}


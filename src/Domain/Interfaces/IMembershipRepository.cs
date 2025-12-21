using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces;

public interface IMembershipRepository : IRepository<Membership>
{
    Task<IEnumerable<Membership>> GetByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Membership>> GetByMemberIdAsync(Guid memberId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Membership>> GetActiveByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Membership>> GetExpiredByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Membership>> GetExpiringWithinDaysAsync(Guid businessId, int days, CancellationToken cancellationToken = default);
    Task<bool> HasOverlappingMembershipAsync(Guid memberId, DateTime startDate, DateTime expiryDate, Guid? excludeMembershipId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Membership>> GetByStatusAsync(Guid businessId, MembershipStatus status, CancellationToken cancellationToken = default);
}


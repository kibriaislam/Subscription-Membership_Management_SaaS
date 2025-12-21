using Application.DTOs.Dashboard;
using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Dashboard.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardStatsQueryHandler(
        IMemberRepository memberRepository,
        IMembershipRepository membershipRepository,
        IPaymentRepository paymentRepository,
        ICurrentUserService currentUserService)
    {
        _memberRepository = memberRepository;
        _membershipRepository = membershipRepository;
        _paymentRepository = paymentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var businessId = _currentUserService.BusinessId.Value;

        var allMembers = await _memberRepository.GetByBusinessIdAsync(businessId, cancellationToken);
        var activeMemberships = await _membershipRepository.GetActiveByBusinessIdAsync(businessId, cancellationToken);
        var expiredMemberships = await _membershipRepository.GetExpiredByBusinessIdAsync(businessId, cancellationToken);
        var renewalsDueToday = await _membershipRepository.GetExpiringWithinDaysAsync(businessId, 0, cancellationToken);
        var renewalsDueThisWeek = await _membershipRepository.GetExpiringWithinDaysAsync(businessId, 7, cancellationToken);
        var allPayments = await _paymentRepository.GetByBusinessIdAsync(businessId, cancellationToken);

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var monthlyPayments = allPayments.Where(p => p.PaymentDate >= startOfMonth);

        var totalOutstanding = activeMemberships.Sum(m => m.TotalAmount - m.PaidAmount);

        return new DashboardStatsDto
        {
            TotalMembers = allMembers.Count(),
            ActiveMembers = activeMemberships.Select(m => m.MemberId).Distinct().Count(),
            ExpiredMembers = expiredMemberships.Select(m => m.MemberId).Distinct().Count(),
            RenewalsDueToday = renewalsDueToday.Count(),
            RenewalsDueThisWeek = renewalsDueThisWeek.Count(),
            MonthlyCollection = monthlyPayments.Sum(p => p.Amount),
            TotalOutstanding = totalOutstanding
        };
    }
}


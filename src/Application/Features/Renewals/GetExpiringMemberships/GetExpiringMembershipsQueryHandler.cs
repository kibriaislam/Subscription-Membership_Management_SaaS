using Application.DTOs.Membership;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Renewals.GetExpiringMemberships;

public class GetExpiringMembershipsQueryHandler : IRequestHandler<GetExpiringMembershipsQuery, IEnumerable<MembershipDto>>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetExpiringMembershipsQueryHandler(
        IMembershipRepository membershipRepository,
        ICurrentUserService currentUserService)
    {
        _membershipRepository = membershipRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<MembershipDto>> Handle(GetExpiringMembershipsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var memberships = await _membershipRepository.GetExpiringWithinDaysAsync(
            _currentUserService.BusinessId.Value,
            request.Days,
            cancellationToken);

        return memberships.Select(m => new MembershipDto
        {
            Id = m.Id,
            MemberId = m.MemberId,
            MemberName = $"{m.Member?.FirstName} {m.Member?.LastName}",
            SubscriptionPlanId = m.SubscriptionPlanId,
            PlanName = m.SubscriptionPlan?.Name ?? string.Empty,
            StartDate = m.StartDate,
            ExpiryDate = m.ExpiryDate,
            Status = m.Status,
            TotalAmount = m.TotalAmount,
            PaidAmount = m.PaidAmount,
            Notes = m.Notes,
            CreatedAt = m.CreatedAt
        });
    }
}


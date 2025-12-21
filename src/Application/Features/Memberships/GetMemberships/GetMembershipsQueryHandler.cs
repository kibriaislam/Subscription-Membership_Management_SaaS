using Application.DTOs.Membership;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Memberships.GetMemberships;

public class GetMembershipsQueryHandler : IRequestHandler<GetMembershipsQuery, IEnumerable<MembershipDto>>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMembershipsQueryHandler(
        IMembershipRepository membershipRepository,
        ICurrentUserService currentUserService)
    {
        _membershipRepository = membershipRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<MembershipDto>> Handle(GetMembershipsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        IEnumerable<Domain.Entities.Membership> memberships;

        if (request.MemberId.HasValue)
        {
            memberships = await _membershipRepository.GetByMemberIdAsync(request.MemberId.Value, cancellationToken);
            memberships = memberships.Where(m => m.BusinessId == _currentUserService.BusinessId.Value);
        }
        else
        {
            memberships = await _membershipRepository.GetByBusinessIdAsync(_currentUserService.BusinessId.Value, cancellationToken);
        }

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


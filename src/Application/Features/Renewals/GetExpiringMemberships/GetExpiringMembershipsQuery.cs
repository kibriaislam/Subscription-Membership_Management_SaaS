using Application.DTOs.Membership;
using MediatR;

namespace Application.Features.Renewals.GetExpiringMemberships;

public class GetExpiringMembershipsQuery : IRequest<IEnumerable<MembershipDto>>
{
    public int Days { get; set; } = 7;
}


using Application.DTOs.Membership;
using MediatR;

namespace Application.Features.Memberships.GetMemberships;

public class GetMembershipsQuery : IRequest<IEnumerable<MembershipDto>>
{
    public Guid? MemberId { get; set; }
}


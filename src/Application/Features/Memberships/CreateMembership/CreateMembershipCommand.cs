using Application.DTOs.Membership;
using MediatR;

namespace Application.Features.Memberships.CreateMembership;

public class CreateMembershipCommand : IRequest<MembershipDto>
{
    public CreateMembershipDto CreateDto { get; set; } = null!;
}


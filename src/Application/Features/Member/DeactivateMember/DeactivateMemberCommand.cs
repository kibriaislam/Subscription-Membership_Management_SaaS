using MediatR;

namespace Application.Features.Member.DeactivateMember;

public class DeactivateMemberCommand : IRequest<Unit>
{
    public Guid MemberId { get; set; }
}


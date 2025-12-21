using Application.DTOs.Member;
using MediatR;

namespace Application.Features.Member.UpdateMember;

public class UpdateMemberCommand : IRequest<MemberDto>
{
    public Guid MemberId { get; set; }
    public UpdateMemberDto UpdateDto { get; set; } = null!;
}


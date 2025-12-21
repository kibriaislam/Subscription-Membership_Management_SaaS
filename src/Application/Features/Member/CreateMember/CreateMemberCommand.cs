using Application.DTOs.Member;
using MediatR;

namespace Application.Features.Member.CreateMember;

public class CreateMemberCommand : IRequest<MemberDto>
{
    public CreateMemberDto CreateDto { get; set; } = null!;
}


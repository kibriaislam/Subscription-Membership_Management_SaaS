using Application.DTOs.Member;
using MediatR;

namespace Application.Features.Member.GetMembers;

public class GetMembersQuery : IRequest<PagedMemberResponseDto>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}


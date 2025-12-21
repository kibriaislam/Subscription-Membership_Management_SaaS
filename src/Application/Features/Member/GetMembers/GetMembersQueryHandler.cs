using Application.DTOs.Member;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Member.GetMembers;

public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, PagedMemberResponseDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMembersQueryHandler(
        IMemberRepository memberRepository,
        ICurrentUserService currentUserService)
    {
        _memberRepository = memberRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedMemberResponseDto> Handle(GetMembersQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var (members, totalCount) = await _memberRepository.GetPagedByBusinessIdAsync(
            _currentUserService.BusinessId.Value,
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            cancellationToken);

        var memberDtos = members.Select(m => new MemberDto
        {
            Id = m.Id,
            FirstName = m.FirstName,
            LastName = m.LastName,
            Email = m.Email,
            Phone = m.Phone,
            Address = m.Address,
            DateOfBirth = m.DateOfBirth,
            IsActive = m.IsActive,
            Notes = m.Notes,
            CreatedAt = m.CreatedAt
        });

        return new PagedMemberResponseDto
        {
            Items = memberDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}


using Application.DTOs.Member;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Member.UpdateMember;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMemberCommandHandler(
        IMemberRepository memberRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        if (member == null || member.BusinessId != _currentUserService.BusinessId.Value)
        {
            throw new KeyNotFoundException("Member not found");
        }

        member.FirstName = request.UpdateDto.FirstName;
        member.LastName = request.UpdateDto.LastName;
        member.Email = request.UpdateDto.Email;
        member.Phone = request.UpdateDto.Phone;
        member.Address = request.UpdateDto.Address;
        member.DateOfBirth = request.UpdateDto.DateOfBirth;
        member.IsActive = request.UpdateDto.IsActive;
        member.Notes = request.UpdateDto.Notes;
        member.UpdatedAt = DateTime.UtcNow;

        await _memberRepository.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            Email = member.Email,
            Phone = member.Phone,
            Address = member.Address,
            DateOfBirth = member.DateOfBirth,
            IsActive = member.IsActive,
            Notes = member.Notes,
            CreatedAt = member.CreatedAt
        };
    }
}


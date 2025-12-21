using Application.DTOs.Member;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Member.CreateMember;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMemberCommandHandler(
        IMemberRepository memberRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var member = new Member
        {
            BusinessId = _currentUserService.BusinessId.Value,
            FirstName = request.CreateDto.FirstName,
            LastName = request.CreateDto.LastName,
            Email = request.CreateDto.Email,
            Phone = request.CreateDto.Phone,
            Address = request.CreateDto.Address,
            DateOfBirth = request.CreateDto.DateOfBirth,
            Notes = request.CreateDto.Notes,
            IsActive = true
        };

        await _memberRepository.AddAsync(member, cancellationToken);
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


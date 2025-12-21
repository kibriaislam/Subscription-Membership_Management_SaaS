using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Member.DeactivateMember;

public class DeactivateMemberCommandHandler : IRequestHandler<DeactivateMemberCommand, Unit>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateMemberCommandHandler(
        IMemberRepository memberRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeactivateMemberCommand request, CancellationToken cancellationToken)
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

        member.IsActive = false;
        member.UpdatedAt = DateTime.UtcNow;

        await _memberRepository.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}


using Application.DTOs.Membership;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Memberships.CreateMembership;

public class CreateMembershipCommandHandler : IRequestHandler<CreateMembershipCommand, MembershipDto>
{
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMembershipCommandHandler(
        IMembershipRepository membershipRepository,
        IMemberRepository memberRepository,
        ISubscriptionPlanRepository planRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _membershipRepository = membershipRepository;
        _memberRepository = memberRepository;
        _planRepository = planRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<MembershipDto> Handle(CreateMembershipCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var member = await _memberRepository.GetByIdAsync(request.CreateDto.MemberId, cancellationToken);
        if (member == null || member.BusinessId != _currentUserService.BusinessId.Value)
        {
            throw new KeyNotFoundException("Member not found");
        }

        var plan = await _planRepository.GetByIdAsync(request.CreateDto.SubscriptionPlanId, cancellationToken);
        if (plan == null || plan.BusinessId != _currentUserService.BusinessId.Value)
        {
            throw new KeyNotFoundException("Subscription plan not found");
        }

        var startDate = request.CreateDto.StartDate ?? DateTime.UtcNow;
        var expiryDate = startDate.AddDays(plan.DurationDays);

        // Check for overlapping memberships
        if (await _membershipRepository.HasOverlappingMembershipAsync(
            request.CreateDto.MemberId,
            startDate,
            expiryDate,
            null,
            cancellationToken))
        {
            throw new InvalidOperationException("Member already has an active membership in this period");
        }

        var membership = new Membership
        {
            BusinessId = _currentUserService.BusinessId.Value,
            MemberId = request.CreateDto.MemberId,
            SubscriptionPlanId = request.CreateDto.SubscriptionPlanId,
            StartDate = startDate,
            ExpiryDate = expiryDate,
            TotalAmount = plan.Price,
            PaidAmount = 0,
            Status = MembershipStatus.Active,
            Notes = request.CreateDto.Notes
        };

        await _membershipRepository.AddAsync(membership, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MembershipDto
        {
            Id = membership.Id,
            MemberId = membership.MemberId,
            MemberName = $"{member.FirstName} {member.LastName}",
            SubscriptionPlanId = membership.SubscriptionPlanId,
            PlanName = plan.Name,
            StartDate = membership.StartDate,
            ExpiryDate = membership.ExpiryDate,
            Status = membership.Status,
            TotalAmount = membership.TotalAmount,
            PaidAmount = membership.PaidAmount,
            Notes = membership.Notes,
            CreatedAt = membership.CreatedAt
        };
    }
}


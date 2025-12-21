using Application.DTOs.Business;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Business.GetBusiness;

public class GetBusinessQueryHandler : IRequestHandler<GetBusinessQuery, BusinessDto>
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetBusinessQueryHandler(
        IBusinessRepository businessRepository,
        ICurrentUserService currentUserService)
    {
        _businessRepository = businessRepository;
        _currentUserService = currentUserService;
    }

    public async Task<BusinessDto> Handle(GetBusinessQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.BusinessId.HasValue)
        {
            throw new UnauthorizedAccessException("Business context not found");
        }

        var business = await _businessRepository.GetByIdAsync(_currentUserService.BusinessId.Value, cancellationToken);
        if (business == null)
        {
            throw new KeyNotFoundException("Business not found");
        }

        return new BusinessDto
        {
            Id = business.Id,
            Name = business.Name,
            Description = business.Description,
            Address = business.Address,
            Phone = business.Phone,
            Email = business.Email,
            Currency = business.Currency,
            TaxId = business.TaxId,
            CreatedAt = business.CreatedAt
        };
    }
}


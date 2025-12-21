using Application.DTOs.Business;
using Application.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Business.UpdateBusiness;

public class UpdateBusinessCommandHandler : IRequestHandler<UpdateBusinessCommand, BusinessDto>
{
    private readonly IBusinessRepository _businessRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBusinessCommandHandler(
        IBusinessRepository businessRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _businessRepository = businessRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessDto> Handle(UpdateBusinessCommand request, CancellationToken cancellationToken)
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

        business.Name = request.UpdateDto.Name;
        business.Description = request.UpdateDto.Description;
        business.Address = request.UpdateDto.Address;
        business.Phone = request.UpdateDto.Phone;
        business.Email = request.UpdateDto.Email;
        business.Currency = request.UpdateDto.Currency;
        business.TaxId = request.UpdateDto.TaxId;
        business.UpdatedAt = DateTime.UtcNow;

        await _businessRepository.UpdateAsync(business, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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


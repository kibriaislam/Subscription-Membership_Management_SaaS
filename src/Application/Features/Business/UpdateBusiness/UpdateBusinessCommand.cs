using Application.DTOs.Business;
using MediatR;

namespace Application.Features.Business.UpdateBusiness;

public class UpdateBusinessCommand : IRequest<BusinessDto>
{
    public UpdateBusinessDto UpdateDto { get; set; } = null!;
}


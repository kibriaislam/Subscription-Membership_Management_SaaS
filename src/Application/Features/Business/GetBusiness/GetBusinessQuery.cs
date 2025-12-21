using Application.DTOs.Business;
using MediatR;

namespace Application.Features.Business.GetBusiness;

public class GetBusinessQuery : IRequest<BusinessDto>
{
}


using Application.DTOs.Dashboard;
using MediatR;

namespace Application.Features.Dashboard.GetDashboardStats;

public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
{
}


using Hangfire.Dashboard;
using Microsoft.Extensions.Hosting;

namespace API.Filters;

/// <summary>
/// Authorization filter for Hangfire Dashboard.
/// In production, this should be enhanced to check user roles and permissions.
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var environment = httpContext.RequestServices.GetRequiredService<IHostEnvironment>();

        // In development, allow access without authentication
        if (environment.IsDevelopment())
        {
            return true;
        }

        // In production, require authentication
        // You can enhance this to check for specific roles or permissions
        return httpContext.User?.Identity?.IsAuthenticated == true;
    }
}


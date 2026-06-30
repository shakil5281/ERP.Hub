using Hangfire.Dashboard;

namespace ERPHub.Security
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // For now, allow any authenticated user to view the Hangfire Dashboard.
            // In a strict production environment, we should verify the "Super Admin" role here.
            return httpContext.User.Identity?.IsAuthenticated == true;
        }
    }
}

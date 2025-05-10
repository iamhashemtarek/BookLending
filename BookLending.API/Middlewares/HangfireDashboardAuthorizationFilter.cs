﻿using Hangfire.Dashboard;

namespace BookLending.API.Middlewares
{
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity?.IsAuthenticated == true &&
                           httpContext.User.IsInRole("Admin");
        }
    }
}

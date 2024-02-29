using Serilog.Context;
using System.Security.Claims;

namespace BaselCoin.Middlewares
{
    public class SerilogEnrichingMiddleware
    {
        private readonly RequestDelegate _next;

        public SerilogEnrichingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var pageDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.RazorPages.PageActionDescriptor>();

                if (pageDescriptor != null)
                {
                    LogContext.PushProperty("Action", pageDescriptor.ViewEnginePath);
                    // pageDescriptor.ViewEnginePath will give you the Razor Page path, like "/Pages/YourPage.cshtml"
                }
            }

            var userId = context.User.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                LogContext.PushProperty("UserId", userId);
            }
            else
            {
                LogContext.PushProperty("UserId", "Unknown");
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}

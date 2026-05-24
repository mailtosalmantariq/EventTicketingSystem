using EventTickets.Api.Middleware;
using Microsoft.AspNetCore.Builder;

namespace EventTickets.Middleware.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }

}

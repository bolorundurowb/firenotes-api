using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace firenotes_api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
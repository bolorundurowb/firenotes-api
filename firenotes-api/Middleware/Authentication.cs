using JWT;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using firenotes_api.Configuration;
using Microsoft.AspNetCore.Builder;

namespace firenotes_api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Contains("notes"))
            {
                string token = context.Request.Headers["x-access-token"];
                if (token == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Sorry, a token is required to access this route.");
                    return;
                }
                
                try
                {
                    var json = Helpers.DecodeToken(token);
                    context.Items["id"] = json["id"];
                    await _next.Invoke(context);
                    return;
                }
                catch (TokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Sorry, your token is expired. Please login.");
                    return;
                }
                catch (SignatureVerificationException)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Sorry, this token has an invalid signature.");
                    return;
                }
                catch (ArgumentException)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Sorry, this token is corrupted.");
                    return;
                }
            }

            await _next.Invoke(context);
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
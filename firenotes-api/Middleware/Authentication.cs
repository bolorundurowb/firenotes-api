using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using firenotes_api.Configuration;
using JWT;
using JWT.Serializers;
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
                string secret = Config.Secret;
                try
                {
                    IJsonSerializer serializer = new JsonNetSerializer();
                    IDateTimeProvider provider = new UtcDateTimeProvider();
                    IJwtValidator validator = new JwtValidator(serializer, provider);
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                    var json = decoder.DecodeToObject<IDictionary<string, string>>(token, secret, verify: true);
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
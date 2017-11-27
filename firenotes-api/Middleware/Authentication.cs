using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace firenotes_api.Middleware
{
    public class AuthenticationMiddleware
    {
        public static void ValidateUser(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                string token = context.Request.Headers["x-access-token"];
                if (token == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Sorry, a token is required to access this route");
                }
                else
                {
                    await next.Invoke();
                }
            });
        }
    }
}
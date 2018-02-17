using System.IO;
using System.Text;
using AutoMapper;
using dotenv.net.DependencyInjection.Extensions;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Middleware;
using firenotes_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace firenotes_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            _environment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment _environment;
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            
            services.AddAutoMapper();
            
            services.AddMvc();
            
            // read the environment vars
            var envFile = _environment.IsDevelopment() ? ".env" : "test.env";
            bool throwOnError = !_environment.IsProduction();
            services.AddEnv(builder =>
            {
                builder
                    .AddEncoding(Encoding.Default)
                    .AddEnvFile(Path.GetFullPath(envFile))
                    .AddThrowOnError(throwOnError);
            });
            
            // add authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Config.Issuer,
                        ValidAudience = Config.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.Secret))
                    };
                });

            // register the services
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseCors(options => options
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowCredentials()
            );

            app.UseAuthenticationMiddleware();
            
            app.UseMvc();
        }
    }
}
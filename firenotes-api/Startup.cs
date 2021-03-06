﻿using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using dotenv.net.DependencyInjection.Microsoft;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace firenotes_api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            _environment = env;
        }

        private readonly IWebHostEnvironment _environment;
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers()
                .AddJsonOptions(x => { x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
            
            // read the environment vars
            var envFile = _environment.IsDevelopment() ? ".env" : "test.env";
            var throwOnError = _environment.IsDevelopment();
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
            );

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(opts => opts.MapControllers());
        }
    }
}
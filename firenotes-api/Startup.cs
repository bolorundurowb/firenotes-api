using System.IO;
using System.Text;
using AutoMapper;
using dotenv.net;
using dotenv.net.DependencyInjection.Extensions;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Middleware;
using firenotes_api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace firenotes_api
{
    public class Startup
    {
        public static string DatabaseName;
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .AllowAnyHeader();
            }));
            services.AddAutoMapper();
            services.AddMvc();
            
            // read in the environment vars
            services.AddEnv(builder =>
            {
                builder
                    .AddEncoding(Encoding.Default)
                    .AddEnvFile(Path.GetFullPath(".env"))
                    .AddThrowOnError(false);
            });

            // register the services
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            DatabaseName = "firenotes-prod-db";
            
            if (env.IsDevelopment())
            {
                DatabaseName = "firenotes-dev-db";
                app.UseDeveloperExceptionPage();
            }

            if (env.IsEnvironment("test"))
            {
                DatabaseName = "firenotes-test-db";
            }

            app.UseCors("CorsPolicy");

            app.UseAuthenticationMiddleware();
            
            app.UseMvc();
        }
    }
}
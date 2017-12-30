using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace firenotes_api.Tests.Integration
{
    public class BaseApiControllerTests
    {
        protected readonly HttpClient Client;
        protected static string Token { get; set; }

        protected BaseApiControllerTests()
        {
            var webBuilder = new WebHostBuilder()
                .UseEnvironment("test")
                .UseStartup<Startup>();
            var server = new TestServer(webBuilder);
            Client = server.CreateClient();
            
            Environment.SetEnvironmentVariable("MONGO_URL", "mongodb://localhost:27017/");
            Environment.SetEnvironmentVariable("SECRET", "test-secret");
            Environment.SetEnvironmentVariable("SERVICE_EMAIL", "test@email.org");
            Environment.SetEnvironmentVariable("MAILGUN_API_KEY", "UrZSSLzdVKwrTgOlVADV_w");
            Environment.SetEnvironmentVariable("MAILGUN_BASE_URI", "https://api.mailgun.net/v3/");
            Environment.SetEnvironmentVariable("MAILGUN_REQUEST_URI", "sandbox6413747fa8049658100b1b2b327f99e.mailgun.org");
            Environment.SetEnvironmentVariable("MAILGUN_SMTP_LOGIN", "postmaster@sandbox6413747fa8049658100b1b2b327f99e.mailgun.org");
        }
    }
}
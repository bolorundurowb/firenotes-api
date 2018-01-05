using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace firenotes_api.Tests.Integration
{
    public class BaseApiControllerTests
    {
        protected HttpClient Client;
        protected static string Token { get; set; }

        protected BaseApiControllerTests()
        {
            var webBuilder = new WebHostBuilder()
                .UseEnvironment("test")
                .UseStartup<Startup>();
            var server = new TestServer(webBuilder);
            Client = server.CreateClient();
        }
    }
}
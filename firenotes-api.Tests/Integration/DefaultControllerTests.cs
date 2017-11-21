using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace firenotes_api.Tests.Integration
{
    public class DefaultControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        
        public DefaultControllerTests()
        {
            _server = new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task RootResponse()
        {
            var response = await _client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Welcome to the Firenotes API. Start by making requests to the /api routes.");
        }

        [Fact]
        public async Task ApiRootResponse()
        {
            var response = await _client.GetAsync("/api/");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Welcome to the Firenotes API.");
        }
    }
}
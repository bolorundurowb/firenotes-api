using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [TestFixture]
    public class DefaultControllerTests : BaseApiControllerTests
    {
        [Test]
        public async Task RootResponse()
        {
            var response = await Client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Welcome to the Firenotes API. Start by making requests to the /api routes.");
        }

        [Test]
        public async Task ApiRootResponse()
        {
            var response = await Client.GetAsync("/api/");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Welcome to the Firenotes API.");
        }
    }
}
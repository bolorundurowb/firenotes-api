using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using firenotes_api.Configuration;
using firenotes_api.Models.View;
using MongoDB.Driver;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [SetUpFixture]
    public class SetupTests : BaseApiControllerTests
    {
        [OneTimeSetUp]
        public async Task SetUp()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/");
            mongoClient.DropDatabase(Config.DbName);

            await CreateDefaultAuthUser();
        }

        private async Task CreateDefaultAuthUser()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"default@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            var content = await response.Content.ReadAsJsonAsync<AuthViewModel>();

            Token = content.Token;
        }
    }
}
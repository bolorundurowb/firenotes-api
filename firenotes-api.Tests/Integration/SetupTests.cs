using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using dotenv.net;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
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
            mongoClient.DropDatabase("firenotes-test-db");
            
            string fullPath = Path.GetFullPath("./../../../../.env");
            DotEnv.Config(false, fullPath);

            await CreateDefaultAuthUser();
        }
        
        private async Task CreateDefaultAuthUser()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"default@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var content = JObject.Parse(responseString);

            Token = content["token"].ToString();
        }
    }
}
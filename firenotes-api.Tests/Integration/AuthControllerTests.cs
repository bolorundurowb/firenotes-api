using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using firenotes_api.Models.Binding;
using firenotes_api.Tests.Util;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [TestFixture]
    public class AuthControllerTests : BaseApiControllerTests
    {
        #region SignUp

        [Test]
        public async Task BadReqestIfTheSignUpEmailIsNull()
        {
            var payload = new LoginBindingModel();
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpPasswordIsNull()
        {
            var payload = new LoginBindingModel { Email = "name@email.com", Password = "" };
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpPasswordIsLessThanEightCharacters()
        {
            var payload = new LoginBindingModel { Email = "name@email.com", Password = "12345" };
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The password cannot be less than 8 characters.");
        }

        [Test, Order(100)]
        public async Task SuccessfulSignUp()
        {
            var payload = new LoginBindingModel { Email = "name@email.com", Password = "12345678" };
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["token"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["email"].ToString().Should().Be("name@email.com");
            jObject["firstName"].ToString().Should().BeEmpty();
            jObject["lastName"].ToString().Should().BeEmpty();
        }

        [Test, Order(101)]
        public async Task ConflictIfAUserWithTheEmailExists()
        {
            var payload = new LoginBindingModel { Email = "name@email.com", Password = "123456789" };
            var response = await Client.PostAsJsonAsync("/api/auth/signup", payload);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, a user with that email already exists.");
        }

        #endregion

        #region Login

        [Test]
        public async Task BadReqestIfTheEmailIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async Task BadReqestIfThePasswordIsNull()
        {
            var payload = new LoginBindingModel { Email = "name@email.com", Password = " " };
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async Task NotFoundIfTheUserDoesNotExist()
        {
            var payload = new LoginBindingModel { Email = "names@email.com", Password = "password" };
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A user with that email address doesn't exist.");
        }

        [Test, Order(102)]
        public async Task SuccessfulLoginWithExistingUser()
        {
            var payload = new LoginBindingModel { Email = "name@email.com", Password = "12345678" };
            var response = await Client.PostAsJsonAsync("/api/auth/login", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["token"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["email"].ToString().Should().Be("name@email.com");
            jObject["firstName"].ToString().Should().BeEmpty();
            jObject["lastName"].ToString().Should().BeEmpty();
        }

         #endregion
    }
}
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        public async Task BadReqestIfTheSignUpPayloadIsNull()
        {
            StringContent stringContent = new StringContent(
                "",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpEmailIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpPasswordIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async Task BadReqestIfTheSignUpPasswordIsLessThanEightCharacters()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The password cannot be less than 8 characters.");
        }

        [Test, Order(100)]
        public async Task SuccessfulSignUp()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
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
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345679\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, a user with that email already exists.");
        }

        #endregion

        #region Login

        [Test]
        public async Task BadReqestIfThePayloadIsNull()
        {
            StringContent stringContent = new StringContent(
                "",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }

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
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async Task NotFoundIfTheUserDoesNotExist()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"names@email.com\", \"password\": \"password\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A user with that email address doesn't exist.");
        }

        [Test, Order(102)]
        public async Task SuccessfulLoginWithExistingUser()
        {
            var stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/auth/login", stringContent);
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
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [TestFixture]
    public class AuthControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public AuthControllerTests()
        {
            var webBuilder = new WebHostBuilder()
                .UseEnvironment("test")
                .UseStartup<Startup>();
            _server = new TestServer(webBuilder);
            _client = _server.CreateClient();
        }

        #region SignUp

        [Test]
        public async void BadReqestIfTheSignUpPayloadIsNull()
        {
            StringContent stringContent = new StringContent(
                "",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }

        [Test]
        public async void BadReqestIfTheSignUpEmailIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async void BadReqestIfTheSignUpPasswordIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async void BadReqestIfTheSignUpPasswordIsLessThanEightCharacters()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The password cannot be less than 8 characters.");
        }

        [Test]
        public async void SuccessfulSignUp()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("token");
            responseString.Should().Contain("email");
            responseString.Should().Contain("firstName");
            responseString.Should().Contain("lastName");
        }

        [Test]
        public async void ConflictIfAUserWithTheEmailExists()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345679\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/signup", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, a user with that email already exists.");
        }

        #endregion

        #region Login

        [Test]
        public async void BadReqestIfThePayloadIsNull()
        {
            StringContent stringContent = new StringContent(
                "",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }

        [Test]
        public async void BadReqestIfTheEmailIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("An email address is required.");
        }

        [Test]
        public async void BadReqestIfThePasswordIsNull()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"  \" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A password is required.");
        }

        [Test]
        public async void NotFoundIfTheUserDoesNotExist()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"names@email.com\", \"password\": \"password\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A user with that email address doesn't exist.");
        }

        [Test]
        public async void SuccessfulLoginWithExistingUser()
        {
            var stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/auth/login", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("token");
            responseString.Should().Contain("email");
            responseString.Should().Contain("firstName");
            responseString.Should().Contain("lastName");
        }

         #endregion
    }
}
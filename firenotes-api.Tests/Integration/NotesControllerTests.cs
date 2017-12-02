using System;
using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace firenotes_api.Tests.Integration
{
    [Collection("API Tests")]
    public class NotesControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private string _token;
        
        public NotesControllerTests()
        {
            var webBuilder = new WebHostBuilder()
                .UseEnvironment("test")
                .UseStartup<Startup>();
            _server = new TestServer(webBuilder);
            _client = _server.CreateClient();

            LogAUserIn();
        }

        #region Creation

        [Fact]
        public async void BadReqestIfThePayloadIsNull()
        {
            var stringContent = new StringContent(
                "",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/notes", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }
        
        [Fact]
        public async void BadReqestIfThePayloadHasNoTitle()
        {
            var stringContent = new StringContent(
                "{ \"title\": \" \" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/notes", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A title is required.");
        }
        
        [Fact]
        public async void SuccessIfThePayloadIsValid()
        {
            var stringContent = new StringContent(
                "{ \"title\": \"Note\", \"details\": \"Note details\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/notes", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("title");
            responseString.Should().Contain("details");
            responseString.Should().Contain("created");
            responseString.Should().Contain("isFavorited");
            
        }

        #endregion

        #region HelperMethods

        private void LogAUserIn()
        {
            StringContent stringContent = new StringContent(
                "{ \"email\": \"name@email.com\", \"password\": \"12345678\" }",
                Encoding.UTF8,
                "application/json");
            var response = _client.PostAsync("/api/auth/login", stringContent).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var content = JObject.Parse(responseString);

            _client.DefaultRequestHeaders.TryAddWithoutValidation("x-access-token", content["token"].ToString());
        }

        #endregion
    }
}
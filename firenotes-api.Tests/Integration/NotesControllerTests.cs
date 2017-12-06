using System.Net;
using System.Net.Http;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [TestFixture]
    public class NotesControllerTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;
        private string _token;
        private string noteId;
        
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

        [Test]
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
        
        [Test]
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
        
        [Test]
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

        #region Retrieval

        [Test]
        public async void AllNotesCanBeRetrieved()
        {
            var response = await _client.GetAsync("/api/notes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            
            JArray array = JArray.Parse(responseString);
            noteId = array[0]["id"].ToString();
            array.Count.Should().Be(1);
        }

        [Test]
        public async void ASingleNoteCanBeRetrieved()
        {
            var response = await _client.GetAsync("/api/notes/" + noteId);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();

            responseString.Should().Contain("id");
            responseString.Should().Contain("title");
            responseString.Should().Contain("details");
            responseString.Should().Contain("tags");
            responseString.Should().Contain("created");
        }

        #endregion

        #region Update

        [Test]
        public async void NotFoundWhenNonExistentIdIsRequested()
        {
            var stringContent = new StringContent(
                string.Empty,
                Encoding.UTF8,
                "application/json");
            var response = await _client.PutAsync("/api/notes/xxxx", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, you either have no access to the note requested or it doesn't exist.");
        }
        
        [Test]
        public async void UpdatesNoteWithProperIdAndPayload()
        {
            var stringContent = new StringContent(
                "{ \"title\": \"Note To Be Updated\", \"details\": \"Note details\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/notes", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            var noteId = jObject["id"].ToString();
            stringContent = new StringContent(
                "{ \"title\": \"Note Updated\" }",
                Encoding.UTF8,
                "application/json");
            response = await _client.PutAsync("/api/notes/" + noteId, stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString = await response.Content.ReadAsStringAsync();
            jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().Be("Note Updated");
            jObject["details"].ToString().Should().Be("Note details");
        }

        #endregion

        #region Removal

        [Test]
        public async void RemoveNote()
        {
            var stringContent = new StringContent(
                "{ \"title\": \"Note To Be Deleted\", \"details\": \"Note details\" }",
                Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("/api/notes", stringContent);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            var noteId = jObject["id"].ToString();
            response = await _client.DeleteAsync("/api/notes" + noteId);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Note successfully removed.", responseString);
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
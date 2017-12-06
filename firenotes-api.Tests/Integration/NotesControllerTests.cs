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
    public class NotesControllerTests : BaseApiControllerTests
    {
        [SetUp]
        public void SetUp()
        {
            Client.DefaultRequestHeaders.TryAddWithoutValidation("x-access-token", Token);
        }

        [TearDown]
        public void TearDown()
        {
            Client.DefaultRequestHeaders.Remove("x-access-token");
        }

        #region Creation

        [Test]
        public async Task BadReqestIfThePayloadIsNull()
        {
            var stringContent = new StringContent(
                "",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/notes", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("The payload must not be null.");
        }
        
        [Test]
        public async Task BadReqestIfThePayloadHasNoTitle()
        {
            var stringContent = new StringContent(
                "{ \"title\": \" \" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/notes", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A title is required.");
        }
        
        [Test, Order(200)]
        public async Task SuccessIfThePayloadIsValid()
        {
            var stringContent = new StringContent(
                "{ \"title\": \"Note\", \"details\": \"Note details\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PostAsync("/api/notes", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Contain("title");
            responseString.Should().Contain("details");
            responseString.Should().Contain("created");
            responseString.Should().Contain("isFavorited");
        }

        #endregion

        #region Retrieval

        private string noteId;
        
        [Test, Order(201)]
        public async Task AllNotesCanBeRetrieved()
        {
            var response = await Client.GetAsync("/api/notes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            
            JArray array = JArray.Parse(responseString);
            noteId = array[0]["id"].ToString();
            array.Count.Should().Be(1);
        }

        [Test, Order(202)]
        public async Task ASingleNoteCanBeRetrieved()
        {
            var response = await Client.GetAsync("/api/notes/" + noteId);
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
        public async Task NotFoundWhenNonExistentIdIsRequested()
        {
            var stringContent = new StringContent(
                string.Empty,
                Encoding.UTF8,
                "application/json");
            var response = await Client.PutAsync("/api/notes/xxxx", stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, you either have no access to the note requested or it doesn't exist.");
        }
        
        [Test, Order(203)]
        public async Task UpdatesNoteWithProperIdAndPayload()
        {
            var stringContent = new StringContent(
                "{ \"title\": \"Note Updated\" }",
                Encoding.UTF8,
                "application/json");
            var response = await Client.PutAsync("/api/notes/" + noteId, stringContent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().Be("Note Updated");
            jObject["details"].ToString().Should().Be("Note details");
        }

        #endregion

        #region Removal

        [Test, Order(204)]
        public async Task RemoveNote()
        {
            var response = await Client.DeleteAsync("/api/notes/" + noteId);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Note successfully removed.", responseString);
        }

        #endregion

    }
}
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using firenotes_api.Models.Binding;
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
        public async Task BadReqestIfThePayloadHasNoTitle()
        {
            var note = new NoteBindingModel {Title = " "};
            var response = await Client.PostAsJsonAsync("/api/notes", note);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("A title is required.");
        }
        
        [Test, Order(200)]
        public async Task SuccessIfThePayloadIsValid()
        {
            var note = new NoteBindingModel {Title = "Note", Details = "Note details"};
            var response = await Client.PostAsJsonAsync("/api/notes", note);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["title"].ToString().Should().Be("Note");
            jObject["details"].ToString().Should().Be("Note details");
            jObject["tags"].ToObject<JArray>().Count.Should().Be(0);
            jObject["created"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["isFavorited"].ToObject<bool>().Should().BeFalse();
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
            var jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().Be(noteId);
            jObject["title"].ToString().Should().Be("Note");
            jObject["details"].ToString().Should().Be("Note details");
            jObject["tags"].ToObject<JArray>().Count.Should().Be(0);
            jObject["created"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["isFavorited"].ToObject<bool>().Should().BeFalse();
        }

        #endregion

        #region Update

        [Test]
        public async Task NotFoundWhenNonExistentIdIsRequested()
        {
            var note = new NoteBindingModel();
            var response = await Client.PutAsJsonAsync("/api/notes/xxxx", note);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Sorry, you either have no access to the note requested or it doesn't exist.");
        }
        
        [Test, Order(203)]
        public async Task UpdatesNoteWithProperIdAndPayload()
        {
            var note = new NoteBindingModel {Title = "Note Updated"};
            var response = await Client.PutAsJsonAsync("/api/notes/" + noteId, note);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseString);

            jObject["id"].ToString().Should().Be(noteId);
            jObject["title"].ToString().Should().Be("Note Updated");
            jObject["details"].ToString().Should().Be("Note details");
            jObject["tags"].ToObject<JArray>().Count.Should().Be(0);
            jObject["created"].ToString().Should().NotBeNullOrWhiteSpace();
            jObject["isFavorited"].ToObject<bool>().Should().BeFalse();
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
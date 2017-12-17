using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using firenotes_api.Models.Binding;
using firenotes_api.Models.View;
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
            var payload = new NoteBindingModel {Title = "Note", Details = "Note details"};
            var response = await Client.PostAsJsonAsync("/api/notes", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.Id.Should().NotBeNullOrWhiteSpace();
            note.Title.Should().Be("Note");
            note.Details.Should().Be("Note details");
            note.Tags.Count.Should().Be(0);
            note.Created.ToString().Should().NotBeNullOrWhiteSpace();
            note.IsFavorited.Should().BeFalse();
        }

        #endregion

        #region Retrieval

        private string noteId;
        
        [Test, Order(201)]
        public async Task AllNotesCanBeRetrieved()
        {
            var response = await Client.GetAsync("/api/notes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadAsJsonAsync<List<NoteViewModel>>();
            
            list.Count.Should().Be(1);
            noteId = list[0].Id;
        }

        [Test, Order(202)]
        public async Task ASingleNoteCanBeRetrieved()
        {
            var response = await Client.GetAsync("/api/notes/" + noteId);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.Id.Should().NotBeNullOrWhiteSpace();
            note.Title.Should().Be("Note");
            note.Details.Should().Be("Note details");
            note.Tags.Count.Should().Be(0);
            note.Created.ToString().Should().NotBeNullOrWhiteSpace();
            note.IsFavorited.Should().BeFalse();
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
            var payload = new NoteBindingModel {Title = "Note Updated", Tags = new List<string>{ "Tag1" }};
            var response = await Client.PutAsJsonAsync("/api/notes/" + noteId, payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.Id.Should().NotBeNullOrWhiteSpace();
            note.Title.Should().Be("Note Updated");
            note.Details.Should().Be("Note details");
            note.Tags.Count.Should().Be(1);
            note.Created.ToString().Should().NotBeNullOrWhiteSpace();
            note.IsFavorited.Should().BeFalse();
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
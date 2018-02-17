using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using firenotes_api.Models.Binding;
using firenotes_api.Models.View;
using FluentAssertions;
using NUnit.Framework;

namespace firenotes_api.Tests.Integration
{
    [TestFixture]
    public class NotesControllerTests : BaseApiControllerTests
    {
        private string _noteId;

        [SetUp]
        public void SetUp()
        {
            Client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {Token}");
        }

        [TearDown]
        public void TearDown()
        {
            Client.DefaultRequestHeaders.Remove("Authorization");
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

        [Test]
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

        [Test]
        public async Task AllNotesCanBeRetrieved()
        {
            var response = await Client.GetAsync("/api/notes");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var list = await response.Content.ReadAsJsonAsync<List<NoteViewModel>>();

            list.Count.Should().Be(2);
        }

        [Test, Order(202)]
        public async Task ASingleNoteCanBeRetrieved()
        {
            var payload = new NoteBindingModel {Title = "Title", Details = "Note details"};
            var response = await Client.PostAsJsonAsync("/api/notes", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            response = await Client.GetAsync("/api/notes/" + note.Id);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.Id.Should().NotBeNullOrWhiteSpace();
            note.Title.Should().Be("Title");
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

        [Test, Order(200)]
        public async Task UpdatesNoteWithProperIdAndPayload()
        {
            // add note to be updated
            var payload = new NoteBindingModel {Title = "Title", Details = "Note details"};
            var response = await Client.PostAsJsonAsync("/api/notes", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            _noteId = note.Id;

            payload = new NoteBindingModel {Title = "Title Updated", Tags = new List<string> {"Tag1"}};
            response = await Client.PutAsJsonAsync("/api/notes/" + note.Id, payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.Id.Should().NotBeNullOrWhiteSpace();
            note.Title.Should().Be("Title Updated");
            note.Details.Should().Be("Note details");
            note.Tags.Count.Should().Be(1);
            note.Created.ToString().Should().NotBeNullOrWhiteSpace();
            note.IsFavorited.Should().BeFalse();
        }

        #endregion

        #region Removal

        [Test]
        public async Task RemoveNote()
        {
            // add note to be removed
            var payload = new NoteBindingModel {Title = "Title", Details = "Note details"};
            var response = await Client.PostAsJsonAsync("/api/notes", payload);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            response = await Client.DeleteAsync("/api/notes/" + note.Id);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.Should().Be("Note successfully removed.", responseString);
        }

        #endregion

        #region (Un)Favoriting

        [Test, Order(201)]
        public async Task FavoriteNote()
        {
            var response = await Client.PostAsJsonAsync("/api/notes/" + _noteId + "/favorite", new object());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.IsFavorited.Should().BeTrue();
        }

        [Test, Order(202)]
        public async Task UnFavoriteNote()
        {
            var response = await Client.PostAsJsonAsync("/api/notes/" + _noteId + "/unfavorite", new object());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var note = await response.Content.ReadAsJsonAsync<NoteViewModel>();

            note.IsFavorited.Should().BeFalse();
        }

        #endregion
    }
}
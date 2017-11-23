using firenotes_api.Models.Data;
using FluentAssertions;
using Xunit;

namespace firenotes_api.Tests.Models
{
    public class NoteTests
    {
        [Fact]
        public void ShouldAutoGenerateId()
        {
            var note = new Note();
            note.Id.Should().NotBeNullOrWhiteSpace();
        }
    }
}
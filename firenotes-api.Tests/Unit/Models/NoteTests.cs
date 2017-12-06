using firenotes_api.Models.Data;
using FluentAssertions;
using NUnit.Framework;

namespace firenotes_api.Tests.Models
{
    [TestFixture]
    public class NoteTests
    {
        [Test]
        public void ShouldAutoGenerateId()
        {
            var note = new Note();
            note.Id.Should().NotBeNullOrWhiteSpace();
        }
    }
}
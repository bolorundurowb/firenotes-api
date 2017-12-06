using firenotes_api.Models.Data;
using FluentAssertions;
using NUnit.Framework;

namespace firenotes_api.Tests.Models
{
    [TestFixture]
    public class UserTests
    {
        [Test]
        public void ShouldAutoGenerateId()
        {
            var user = new User();
            user.Id.Should().NotBeNullOrWhiteSpace();
        }
    }
}
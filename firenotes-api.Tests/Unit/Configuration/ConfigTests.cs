using firenotes_api.Configuration;
using FluentAssertions;
using Xunit;

namespace firenotes_api.Tests.Unit.Configuration
{
    public class ConfigTests
    {
        [Fact(Skip = "I have to figure out a way to test this properly")]
        public void TheConfigPropertiesAreNotEmply()
        {
            Config.DbPath.Should().NotBeNullOrWhiteSpace();
            Config.Secret.Should().NotBeNullOrWhiteSpace();
            Config.DbPath.Should().Be("mongodb://localhost:27017");
        }
    }
}
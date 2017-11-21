using FluentAssertions;
using Xunit;

namespace firenotes_api.Tests
{
    public class BasicTests
    {
        [Fact]
        public void Test1()
        {
            2.Should().BeLessOrEqualTo(2);
        }
    }
}
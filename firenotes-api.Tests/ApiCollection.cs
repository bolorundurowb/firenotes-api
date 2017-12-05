using Xunit;

namespace firenotes_api.Tests
{
    [CollectionDefinition("API Tests")]
    public class ApiCollection: ICollectionFixture<TestFixture>
    {}
}
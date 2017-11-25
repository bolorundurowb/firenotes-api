using System;
using MongoDB.Driver;

namespace firenotes_api.Tests
{
    public class TestFixture: IDisposable
    {
        public TestFixture()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/");
            mongoClient.DropDatabase("firenotes-test-db");
        }
        
        public void Dispose()
        {
        }
    }
}
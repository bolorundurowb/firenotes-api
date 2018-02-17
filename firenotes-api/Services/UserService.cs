using System.Threading.Tasks;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using MongoDB.Driver;

namespace firenotes_api.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoDatabase _mongoDatabase;

        public UserService()
        {
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Config.DbName);
        }

        public async Task<User> GetUser(string id)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            return await usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            return await usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task Add(User user)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            await usersCollection.InsertOneAsync(user);
        }

        public async Task Update(string id, UserBindingModel user)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("_id", id);
            var updateBuilder = Builders<User>.Update;
            var update = updateBuilder.Set("IsArchived", false);

            if (!string.IsNullOrWhiteSpace(user.FirstName))
            {
                update = update.Set("FirstName", user.FirstName);
            }

            if (!string.IsNullOrWhiteSpace(user.LastName))
            {
                update = update.Set("LastName", user.LastName);
            }

            await usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetPassword(string email, string password)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("Email", email);

            var updateBuilder = Builders<User>.Update;
            var update = updateBuilder.Set("Password", password);

            await usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task Archive(string id)
        {
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("_id", id);
            var updateBuilder = Builders<User>.Update;
            var update = updateBuilder.Set("IsArchived", true);
            await usersCollection.UpdateOneAsync(filter, update);
        }
    }
}
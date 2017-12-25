using AutoMapper;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using firenotes_api.Configuration;
using firenotes_api.Models.Data;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IMongoDatabase _mongoDatabase;
        private IMapper _mapper;

        public UsersController(IMapper mapper)
        {
            _mapper = mapper;
            
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Startup.DatabaseName);
        }
        
        // POST api/users/:id/archive
        [Route("{id}/archive"), HttpPost]
        public async Task<IActionResult> ArchiveUser(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();

            if (callerId != id)
            {
                return BadRequest("You can only archive your own account.");
            }
            
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("_id", id);
            
            var updateBuilder = Builders<User>.Update;
            var update = updateBuilder.Set("IsArchived", true);
            
            await usersCollection.UpdateOneAsync(filter, update);
            
            return Ok();
        }
        
        // PUT api/users/:id
        [Route("{id}"), HttpPut]
        public async Task<IActionResult> Update(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();

            if (callerId != id)
            {
                return BadRequest("You can only update your own profile.");
            }
            
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("_id", id);
            
            var updateBuilder = Builders<User>.Update;
            var update = updateBuilder.Set("IsArchived", true);
            
            await usersCollection.UpdateOneAsync(filter, update);
            
            return Ok();
        }
    }
}
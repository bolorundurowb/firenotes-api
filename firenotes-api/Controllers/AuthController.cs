using System;
using System.Threading.Tasks;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private IMongoDatabase _mongoDatabase;

        public AuthController()
        {
            var dbPath = Environment.GetEnvironmentVariable("MONGO_URL");
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase("firenotesdb");
        }
        
        // POST api/auth/login
        [Route("login"), HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginBindingModel data)
        {
            if (string.IsNullOrWhiteSpace(data.Email))
            {
                return BadRequest("An email address is required.");
            }

            if (string.IsNullOrEmpty(data.Password))
            {
                return BadRequest("A password is required.");
            }
            
            var usersCollection = _mongoDatabase.GetCollection<User>("users");
            var user = await usersCollection.Find(x => x.Email == data.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("A user with that email address doesn't exist.");
            }

            if (!BCrypt.Net.BCrypt.Verify(data.Password, user.HashedPassword))
            {
                return Unauthorized();
            }

            return Ok(user);
        }
    }
}
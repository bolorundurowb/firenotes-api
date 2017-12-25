using AutoMapper;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using firenotes_api.Configuration;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using Microsoft.Extensions.Logging;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IMongoDatabase _mongoDatabase;
        private ILogger _logger;

        public UsersController(ILogger<AuthController> logger)
        {
            _logger = logger;
            
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
            var user = await usersCollection.Find(x => x.Id == id).FirstAsync();
            
            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("_id", id);
            
            var updateBuilder = Builders<User>.Update;
            var update = updateBuilder.Set("IsArchived", true);

            var email = EmailTemplates.GetArchivedAccountEmail();
            var result = await Email.SendAsync(user.Email, "Archived Account", email);
            if (result.Count == 0)
            {
                _logger.LogInformation("Forgot password email sent successfully.");
            }
            else
            {
                _logger.LogError("An error occurred when sending ");
            }
            
            await usersCollection.UpdateOneAsync(filter, update);
            
            return Ok();
        }
        
        // PUT api/users/:id
        [Route("{id}"), HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] UserBindingModel bm)
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
            var update = updateBuilder.Set("IsArchived", false);
            
            if (!string.IsNullOrWhiteSpace(bm.FirstName))
            {
                 update = update.Set("FirstName", bm.FirstName);
            }

            if (!string.IsNullOrWhiteSpace(bm.LastName))
            {
                update = update.Set("LastName", bm.LastName);
            }
            
            await usersCollection.UpdateOneAsync(filter, update);
            
            return Ok("Profile successfully updated.");
        }
    }
}
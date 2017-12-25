using AutoMapper;
using MongoDB.Driver;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using firenotes_api.Configuration;

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
        
        [Route("{id}"), HttpDelete]
        public async Task<IActionResult> ArchiveUser()
        {
            return Ok();
        }
    }
}
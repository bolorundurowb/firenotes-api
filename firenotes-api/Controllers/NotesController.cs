using System.Threading.Tasks;
using AutoMapper;
using firenotes_api.Configuration;
using firenotes_api.Models.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace firenotes_api.Controllers
{
    public class NotesController : Controller
    {
        private IMongoDatabase _mongoDatabase;
        private IMapper _mapper;
        
        public NotesController(IMapper mapper)
        {
            _mapper = mapper;
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Startup.DatabaseName);
        }
        
        // GET api/notes
        [Route(""), HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var notes = await notesCollection.Find(x => x.Id == "xxx").ToListAsync();
            return Ok(notes);
        }
    }
}
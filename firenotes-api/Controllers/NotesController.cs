using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using firenotes_api.Configuration;
using firenotes_api.Models.Data;
using firenotes_api.Models.View;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace firenotes_api.Controllers
{
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private IMongoDatabase _mongoDatabase;
        private IMapper _mapper;
        private string _callerId;
        
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
            var notes = await notesCollection.Find(x => x.Owner == _callerId).ToListAsync();
            return Ok(_mapper.Map<List<NoteViewModel>>(notes));
        }
        
        // GET api/notes/:id
        [Route("{id}"), HttpGet]
        public async Task<IActionResult> GetOne([FromRoute] string id)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var note = await notesCollection.Find(x => x.Id == id && x.Owner == _callerId).FirstOrDefaultAsync();

            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }
            
            return Ok(_mapper.Map<NoteViewModel>(note));
        }
    }
}
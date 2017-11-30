using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using firenotes_api.Configuration;
using firenotes_api.Models.Binding;
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
        
        public NotesController(IMapper mapper)
        {
            _mapper = mapper;
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Startup.DatabaseName);
        }
        
        // GET api/notes
        [Route(""), HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] NoteQueryModel query)
        {
            var callerId = HttpContext.Items["id"].ToString();
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var notes = await notesCollection.Find(x => x.Owner == callerId)
                .Limit(query.Limit)
                .ToListAsync();
            return Ok(_mapper.Map<List<NoteViewModel>>(notes));
        }
        
        // GET api/notes/:id
        [Route("{id}"), HttpGet]
        public async Task<IActionResult> GetOne(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();

            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }
            
            return Ok(_mapper.Map<NoteViewModel>(note));
        }

        // POST api/notes
        [Route(""), HttpPost]
        public async Task<IActionResult> Create([FromBody] NoteBindingModel data)
        {
            var callerId = HttpContext.Items["id"].ToString();

            if (data == null)
            {
                return BadRequest("The payload cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(data.Title))
            {
                return BadRequest("A title is required.");
            }

            var note = new Note
            {
                Owner = callerId,
                Title = data.Title,
                Details = data.Details,
                Tags = data.Tags,
                IsFavorited = false
            };
            
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            await notesCollection.InsertOneAsync(note);
            
            return Ok(_mapper.Map<NoteViewModel>(note));
        }
        
        // PUT api/notes/:id
        [Route("{id}"), HttpPut]
        public async Task<IActionResult> Update(string id, [FromBody] NoteBindingModel data)
        {
            var callerId = HttpContext.Items["id"].ToString();
            
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();
            
            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", callerId);

            var updateBuilder = Builders<Note>.Update;
            var update = updateBuilder.Set("Tags", data.Tags);

            if (!string.IsNullOrWhiteSpace(data.Title))
            {
                update = update.Set("Title", data.Title);
            }

            if (!string.IsNullOrWhiteSpace(data.Details))
            {
                update = update.Set("Details", data.Details);
            }
            
            await notesCollection.UpdateOneAsync(filter, update);
            note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();
            
            return Ok(_mapper.Map<NoteViewModel>(note));
        }
        
        // DELETE api/notes/:id
        [Route("{id}"), HttpDelete]
        public async Task<IActionResult> Remove(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();
            
            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", callerId);
            
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            await notesCollection.DeleteOneAsync(filter);

            return Ok("Note successfully removed.");
        }
        
        // POST api/notes/:id/favorite
        [Route("{id}/favorite"), HttpPost]
        public async Task<IActionResult> Favorite(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();
            
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();
            
            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", callerId);

            var updateBuilder = Builders<Note>.Update;
            var update = updateBuilder.Set("IsFavorited", true);
            
            await notesCollection.UpdateOneAsync(filter, update);
            note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();
            
            return Ok(_mapper.Map<NoteViewModel>(note));
        }
        
        // POST api/notes/:id/unfavorite
        [Route("{id}/unfavorite"), HttpPost]
        public async Task<IActionResult> UnFavorite(string id)
        {
            var callerId = HttpContext.Items["id"].ToString();
            
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();
            
            if (note == null)
            {
                return NotFound("Sorry, you either have no access to the note requested or it doesn't exist.");
            }

            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", callerId);

            var updateBuilder = Builders<Note>.Update;
            var update = updateBuilder.Set("IsFavorited", false);
            
            await notesCollection.UpdateOneAsync(filter, update);
            note = await notesCollection.Find(x => x.Id == id && x.Owner == callerId).FirstOrDefaultAsync();
            
            return Ok(_mapper.Map<NoteViewModel>(note));
        }
    }
}
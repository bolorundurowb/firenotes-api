using System.Collections.Generic;
using System.Threading.Tasks;
using firenotes_api.Configuration;
using firenotes_api.Interfaces;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;
using MongoDB.Driver;

namespace firenotes_api.Services
{
    public class NoteService : INoteService
    {
        private readonly IMongoDatabase _mongoDatabase;
        
        public NoteService()
        {
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Config.DbName);
        }
        
        public async Task<List<Note>> GetNotes(string owner, NoteQueryModel query)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("Owner", owner);
            
            if (query.Date.Year != 0001)
            {
                filter = filter 
                         & filterBuilder.Gte(x => x.Created, query.Date)
                         & filterBuilder.Lt(x => x.Created, query.Date.AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(query.Tag))
            {
                filter = filter
                         & filterBuilder.AnyEq(x => x.Tags, query.Tag);
            }
            
            return await notesCollection.Find(filter)
                .Limit(query.Limit)
                .Skip(query.Skip)
                .Sort("{Created: -1}")
                .ToListAsync();
        }

        public async Task<Note> GetNote(string id, string owner)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            return await notesCollection.Find(x => x.Id == id && x.Owner == owner).FirstOrDefaultAsync();
        }

        public async Task Add(Note note)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            await notesCollection.InsertOneAsync(note);
        }

        public async Task Update(string id, string owner, NoteBindingModel note)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", owner);
            var updateBuilder = Builders<Note>.Update;
            var update = updateBuilder.Set("Tags", note.Tags);

            if (!string.IsNullOrWhiteSpace(note.Title))
            {
                update = update.Set("Title", note.Title);
            }
            if (!string.IsNullOrWhiteSpace(note.Details))
            {
                update = update.Set("Details", note.Details);
            }
            
            await notesCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetFavorite(string id, string owner)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", owner);
            var updateBuilder = Builders<Note>.Update;
            var update = updateBuilder.Set("IsFavorited", true);
            await notesCollection.UpdateOneAsync(filter, update);
        }

        public async Task SetUnFavorite(string id, string owner)
        {
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", owner);
            var updateBuilder = Builders<Note>.Update;
            var update = updateBuilder.Set("IsFavorited", false);
            await notesCollection.UpdateOneAsync(filter, update);
        }

        public async Task Delete(string id, string owner)
        {
            var filterBuilder = Builders<Note>.Filter;
            var filter = filterBuilder.Eq("_id", id) & filterBuilder.Eq("Owner", owner);
            var notesCollection = _mongoDatabase.GetCollection<Note>("notes");
            await notesCollection.DeleteOneAsync(filter);
        }
    }
}
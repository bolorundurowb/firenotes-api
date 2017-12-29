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
        private IMongoDatabase _mongoDatabase;
        
        public NoteService()
        {
            var dbPath = Config.DbPath;
            var mongoClient = new MongoClient(dbPath);
            _mongoDatabase = mongoClient.GetDatabase(Startup.DatabaseName);
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

        public void Add(Note note)
        {
            throw new System.NotImplementedException();
        }

        public void Update(string id, string owner, Note note)
        {
            throw new System.NotImplementedException();
        }

        public void SetFavorite(string id, string owner)
        {
            throw new System.NotImplementedException();
        }

        public void SetUnFavorite(string id, string owner)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(string id, string owner)
        {
            throw new System.NotImplementedException();
        }
    }
}
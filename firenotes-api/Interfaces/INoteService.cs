using System.Collections.Generic;
using System.Threading.Tasks;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;

namespace firenotes_api.Interfaces
{
    public interface INoteService
    {
        Task<List<Note>> GetNotes(string owner, NoteQueryModel query);

        Task<Note> GetNote(string id, string owner);

        void Add(Note note);

        void Update(string id, string owner, Note note);

        void SetFavorite(string id, string owner);
        
        void SetUnFavorite(string id, string owner);

        void Delete(string id, string owner);
    }
}
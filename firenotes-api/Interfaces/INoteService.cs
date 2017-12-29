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

        Task Add(Note note);

        Task Update(string id, string owner, NoteBindingModel note);

        Task SetFavorite(string id, string owner);
        
        Task SetUnFavorite(string id, string owner);

        Task Delete(string id, string owner);
    }
}
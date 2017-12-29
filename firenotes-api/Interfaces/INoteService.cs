using System.Collections.Generic;
using firenotes_api.Models.Data;

namespace firenotes_api.Interfaces
{
    public interface INoteService
    {
        List<Note> GetNotes(string owner);

        Note GetNote(string id, string owner);

        void Add(Note note);

        void Update(string id, string owner, Note note);

        void SetFavorite(string id, string owner);
        
        void SetUnFavorite(string id, string owner);

        void Delete(string id, string owner);
    }
}
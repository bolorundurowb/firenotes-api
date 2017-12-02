using System.Collections.Generic;

namespace firenotes_api.Models.Binding
{
    public class NoteBindingModel
    {
        public string Title { get; set; }

        public string Details { get; set; }

        public List<string> Tags { get; set; }
    }
}
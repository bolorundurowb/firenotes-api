using System;
using System.Collections.Generic;

namespace firenotes_api.Models.View
{
    public class NoteViewModel
    {
        public string Id { get; set; }

        public string Owner { get; set; }

        public string Title { get; set; }

        public string Details { get; set; }

        public List<string> Tags { get; set; }

        public DateTime Created { get; set; }

        public bool IsFavorited { get; set; }

    }
}
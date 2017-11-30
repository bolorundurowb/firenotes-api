using System;

namespace firenotes_api.Models.Binding
{
    public class NoteQueryModel
    {
        public int Limit { get; set; }

        public string Tag { get; set; }

        public int Skip { get; set; }

        public DateTime Date { get; set; }
    }
}
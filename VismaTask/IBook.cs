using System;

namespace VismaTask
{
    public interface IBook
    {
            public string Name { get; set; }
            public string Author { get; set; }
            public string Category { get; set; }
            public string Language { get; set; }
            public DateTime PublicationDate { get; set; }
            public string ISBN { get; set; }
            public bool Taken { get; set; }
    }

}

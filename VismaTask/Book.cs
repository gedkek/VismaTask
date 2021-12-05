using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VismaTask
{
    public class Book : IBook
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Language { get; set; }
        public DateTime PublicationDate { get; set; }
        public string ISBN { get; set; }
        public bool Taken { get; set; }

        /// <summary>
        /// Converts class parameters to string object
        /// </summary>
        /// <returns>String representation of class parameters</returns>
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4:yyyy/MM/dd}, {5}, {6}", Name, Author, Category, Language, PublicationDate, ISBN, Taken);
        }
    }
}

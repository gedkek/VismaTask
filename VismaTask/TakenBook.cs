using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VismaTask
{
    class TakenBook : Book, IBook
    {
        public string Taker { get; set; }     
        public DateTime ReturnBy { get; set; }
        
        // TODO:
        // Make deserialization for child classes using Custom JsonConverter possibly.

        /// <summary>
        /// Creates a taken book object from given json element by using GetProperty method.
        /// </summary>
        /// <param name="element">Json element of TakenBook</param>
        public TakenBook(JsonElement element)
        {
            this.Name = element.GetProperty("Name").ToString();
            this.Author = element.GetProperty("Author").ToString();
            this.Category = element.GetProperty("Category").ToString();
            this.Language = element.GetProperty("Language").ToString();
            this.PublicationDate = element.GetProperty("PublicationDate").GetDateTime();
            this.ISBN = element.GetProperty("ISBN").ToString();
            this.Taken = element.GetProperty("Taken").GetBoolean();
            this.Taker = element.GetProperty(nameof(Taker)).ToString();
            this.ReturnBy = element.GetProperty(nameof(ReturnBy)).GetDateTime();
        }
        /// <summary>
        /// Constructor for building a taken book from given book and other parameters.
        /// </summary>
        /// <param name="book">Given parent book to copy over</param>
        /// <param name="taker">Given name of person taking the book</param>
        /// <param name="returnBy">Date of expected return by of the book</param>
        public TakenBook(Book book, string taker, DateTime returnBy)
        {
            this.Name = book.Name;
            this.Author = book.Author;
            this.Category = book.Category;
            this.Language = book.Language;
            this.PublicationDate = book.PublicationDate;
            this.ISBN = book.ISBN;
            this.Taken = book.Taken;
            this.Taker = taker;
            this.ReturnBy = returnBy;
        }
        /// <summary>
        /// Converts class parameters to string object
        /// </summary>
        /// <returns>String representation of class parameters</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", base.ToString(), Taker, ReturnBy);
        }
    }
}
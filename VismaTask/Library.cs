using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VismaTask
{
    public class Library
    {
        readonly string libraryJson = "Library.json";
        readonly string takenBookJson = "TakenBooks.json";
        List<Book> books = new List<Book>();
        List<TakenBook> takenBooks = new List<TakenBook>();
        
        public Library()
        {
            
        }
        /// <summary>
        /// Populates books and taken books lists.
        /// </summary>
        public void DoRead()
        {
            ReadLibrary();
            ReadTakenBooks();
        }
        
        // TODO:
        // Fix next 4 methods (ReadLibrary, ReadTakenBooks, UpdateLibraryJSON, UpdateTakenBookJSON): reduce to 2.
        // Some Covariace problem with Book, TakenBook, IBook.
        // Apparently needs another interface? but need to look into further..

        /// <summary>
        /// Populates library book list from json file, or creates new jason file.
        /// </summary>
        void ReadLibrary()
        {
            if(!File.Exists(libraryJson))
            {
                FileStream fileStream =  File.Create(libraryJson);
                fileStream.Close();
            }
            else
            {
                string jsonString = File.ReadAllText(libraryJson);
                if (jsonString != String.Empty)
                {
                    books = JsonSerializer.Deserialize<List<Book>>(jsonString);
                }
            }
        }
        /// <summary>
        /// Populates taken books list from json file, or creates new jason file.
        /// </summary>
        void ReadTakenBooks()
        {
            if(!File.Exists(takenBookJson))
            {
                FileStream fileStream =  File.Create(takenBookJson);
                fileStream.Close();
            }
            else
            {
                string jsonString = File.ReadAllText(takenBookJson);
                if (jsonString != String.Empty)
                {
                    List<JsonElement> elements = JsonSerializer.Deserialize<List<JsonElement>>(jsonString);
                    
                    foreach (JsonElement item in elements)
                    {
                        takenBooks.Add(new TakenBook(item));
                    }
                }
            }
        }
        /// <summary>
        /// Updates book list to its json file.
        /// </summary>
        public void UpdateLibraryJSON()
        {
            string booksJson = JsonSerializer.Serialize(books);
            File.WriteAllText(libraryJson, booksJson);
        }
        /// <summary>
        /// Updates taken book list to its json file.
        /// </summary>
        public void UpdateTakenBookJSON()
        {
            string booksJson = JsonSerializer.Serialize(takenBooks);
            File.WriteAllText(takenBookJson, booksJson);
        }        
        /// <summary>
        /// Adds a new book to the library list.
        /// </summary>
        /// <param name="book">A new book to add to the library</param>
        /// <returns>Message of success or error</returns>
        public string AddBook(Book newBook)
        {
            newBook.Taken = false;
            books.Add(newBook);
            return "Book added successfully.";
        }
        /// <summary>
        /// Takes specified book from the library.
        /// </summary>
        /// <param name="isbn">Target book isbn</param>
        /// <param name="name">Name of the person taking the book</param>
        /// <param name="period">For how many months the book wil be taken</param>
        /// <returns>Message of success or error</returns>
        public string TakeBook(string isbn, string name, int period)
        {
            if (period > 2)
            {
                return "Can not take a book for more than 2 months..";
            }
            int count = TakenBookCount(name);
            if (count > 2)
            {
                return "Can not have more than 3 books taken at a time..";
            }
            Book found = FindBook(isbn, false);
            if (found is null)
            {
                return "No free book found..";
            }
            found.Taken = true;
            TakenBook newTakenBook = new TakenBook(found, name, DateTime.Now.AddMonths(period));
            takenBooks.Add(newTakenBook);
            return "Book taken successfully.";
        }
        /// <summary>
        /// Returns a taken book back to the library by looking for it in both lists by isbn and takers name, 
        /// removing from taken book list and changing Taken property in books list.
        /// </summary>
        /// <param name="isbn">The isbn of the taken book</param>
        /// <param name="name">The name of the person who took the book</param>
        /// <returns>Message of success or error</returns>
        public string ReturnBook(string isbn, string name)
        {
            TakenBook found = FindTakenBook(isbn, true, name);
            Book book = FindBook(isbn, true);
            if (found is null)
            {
                return "No such taken book found.. Error?";
            }
            takenBooks.Remove(found);
            book.Taken = false;
            if (found.ReturnBy.CompareTo(DateTime.Now) == -1)
            {
                return "Book FINALLY returned....";
            }
            return "Book returned successfully.";
        }
        /// <summary>
        /// Returns book list.
        /// </summary>
        /// <returns>All book list</returns>
        public List<Book> ListBooks()
        {
            return books;
        }
        /// <summary>
        /// Deletes a book from the library book list if it is not taken by anyone.
        /// </summary>
        /// <param name="isbn">ISBN of target book</param>
        /// <returns>Message of success or error</returns>
        public string DeleteBook(string isbn)
        {
            Book foundBook = FindBook(isbn, false);
            if (foundBook is null)
            {
                return "No such book found, or is taken. Can delete only free books.";
            }
            books.Remove(foundBook);
            return "Book deleted successfully.";
        }
        // TODO:
        // Reduce FindBook and FindTakenBook to 1 method.
        // Same problem as with reading methods.

        /// <summary>
        /// Finds first free book by ISBN and given taken status.
        /// </summary>
        /// <param name="ISBN">Target book ISBN</param>
        /// <param name="takenStatus">true if looking for a taken book, false for free book</param>
        /// <returns>Target book or null, if book not found</returns>
        public Book FindBook(string ISBN, bool takenStatus)
        {
            foreach (Book book in books)
            {
                if (book.ISBN == ISBN && book.Taken == takenStatus)
                {
                    return book;
                }
            }
            return null;
        }
        /// <summary>
        /// Finds first taken book by ISBN and takers name.
        /// </summary>
        /// <param name="ISBN">Target book ISBN</param>
        /// <param name="takenStatus">true if looking for a taken book, false for free book</param>
        /// <param name="name">Name of the person who took the book</param>
        /// <returns>Target TakenBook or null, if book not found</returns>
        TakenBook FindTakenBook(string ISBN, bool takenStatus, string name)
        {
            foreach (TakenBook book in takenBooks)
            {
                if (book.ISBN == ISBN && book.Taken == takenStatus && name == book.Taker)
                {
                    return book;
                }
            }
            return null;
        }
        /// <summary>
        /// Counts, how many books the given name has taken.
        /// </summary>
        /// <param name="taker">Takers name to check</param>
        /// <returns>Amount of books taken by given name</returns>
        int TakenBookCount(string taker)
        {
            int count = 0;
            foreach (TakenBook book in takenBooks)
            {
                if (book.Taker == taker)
                {
                    count++;
                    if (count > 2)
                    {
                        break;
                    }
                }
            }
            return count;
        }
        // Returns amount of books in book array
        public int BookCount()
        {
            return books.Count;
        }
    }
}

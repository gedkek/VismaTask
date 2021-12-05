using System;
using System.Collections.Generic;
using Xunit;
using VismaTask;
using System.Text.Json;

namespace UnitTests
{
    public class UnitTest1
    {
        // Create instance of library
        Library library = new Library();
        // Creates 2 testing cases for Adding new book. 
        // Given parameters are: json text,  inputing search isbn for books
        public static IEnumerable<object[]> SetBooks => new List<object[]>
        {
            new object[]{ @"{
                ""Name"": ""MARQET"",
                ""Author"": ""Louella Fletcher"",
                ""Category"": ""Fiction"",
                ""Language"": ""English"",
                ""PublicationDate"": ""2017-01-03"",
                ""ISBN"": ""978-4-6752-0058-3""
            }", "978-4-6752-0058-3", "Tom", 3},
            new object[]{ @"{
                ""Name"": ""FLYBOYZ"",
                ""Author"": ""Adele Griffin"",
                ""Category"": ""Fiction"",
                ""Language"": ""English"",
                ""PublicationDate"": ""2017-01-03"",
                ""ISBN"": ""978-6-1843-4100-9""
            }", "978-6-1843-4100-9", "Tom", 2 },
            new object[]{ @"{
                ""Name"": ""FLYBOYZ"",
                ""Author"": ""Adele Griffin"",
                ""Category"": ""Fiction"",
                ""Language"": ""English"",
                ""PublicationDate"": ""2017-01-03"",
                ""ISBN"": ""978-6-1843-4100-9""
            }", "978-6-1843-4100-9", "Tom", 2 },
        };

        // Test for adding a new book to the library
        [Theory, MemberData(nameof(SetBooks))]
        public void AddBookTest(string json, string isbn, string name, int duration)
        {
            Book newBook = JsonSerializer.Deserialize<Book>(json);
            string returnString = library.AddBook(newBook);
            Assert.True( returnString == "Book added successfully.");
        }

        // Test for getting a book from the library, search for a free book
        [Theory, MemberData(nameof(SetBooks))]
        public void FindBookTest(string json, string isbn, string name, int duration)
        {
            // First creates some books and adds them to library
            AddSomeBooks(json, 5);
            // Then calls to find said books by isbn
            Book book = library.FindBook(isbn, false);
            Assert.True(book is not null);
        }
        void AddSomeBooks(string json, int count)
        {
            //Book newBook = JsonSerializer.Deserialize<Book>(json);
            for (int i = 0; i < count; i++)
            {
                Book newBook = JsonSerializer.Deserialize<Book>(json);
                library.AddBook(newBook);
            }
        }
        // TODO: Simplify this test maybe. Its too long and difficult perhaps..
        // Test for taking a book from le library
        [Theory, MemberData(nameof(SetBooks))]
        public void TakeBookTest(string json, string isbn, string name, int duration)
        {
            // First creates some books and adds them to library
            AddSomeBooks(json, 3);

            // Then call to take some books
            string returnString = library.TakeBook(isbn, name, duration); 

            // Checks for duration of taking
            if(duration > 2){
                Assert.True(returnString == "Can not take a book for more than 2 months..");
                return;
            }
            // Takes another book
            returnString = library.TakeBook(isbn, name, duration); 
            Assert.True(returnString == "Book taken successfully.");

            // Taskes few more books, but exeeds book limit (3 per name) and gets error message
            returnString = library.TakeBook(isbn, name, duration); 
            returnString = library.TakeBook(isbn, name, duration); 
            Assert.True(returnString == "Can not have more than 3 books taken at a time..");

            // Tries to take another book, but all are taken by someone else so gets no books left message
            returnString = library.TakeBook(isbn, "TestName1", duration); 
            Assert.True(returnString == "No free book found..");
        }
        // TODO: Figure out a way to test late return.
        // Test for returning a book
        [Theory, MemberData(nameof(SetBooks))]
        public void ReturnBookTest(string json, string isbn, string name, int duration)
        {
            // First creates some books and adds them to library
            AddSomeBooks(json, 5);
            // Then someone takes some books 
            string returnString = library.TakeBook(isbn, name, duration);
            if (returnString == "Can not take a book for more than 2 months..")
            {
                // Tries to return a book that they did not take!
                returnString = library.ReturnBook(isbn, name);
                // Fails
                Assert.True(returnString == "No such taken book found.. Error?");
                return;
            }
            // Proceeds to return a book that they did take
            returnString = library.ReturnBook(isbn, name);
            // Succeeds
            Assert.True(returnString == "Book returned successfully.");
        }

        // Tests for deleting books from library
        [Theory, MemberData(nameof(SetBooks))]
        public void DeleteBookTest(string json, string isbn, string name, int duration)
        {
            // First creates some books and adds them to library
            AddSomeBooks(json, 4);
            // Then deletes one..
            string returnString = library.DeleteBook(isbn);
            Assert.True(library.BookCount() == 3);
            Assert.True(returnString == "Book deleted successfully.");
            // Proceed to delete the rest.. one by one
            returnString = library.DeleteBook(isbn);
            returnString = library.DeleteBook(isbn);
            returnString = library.DeleteBook(isbn);
            // Tries to delete more, but no more books left
            returnString = library.DeleteBook(isbn);
            Assert.True(returnString == "No such book found, or is taken. Can delete only free books.");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VismaTask
{
    class Program
    {
        static Library library = new Library();
        static void Main(string[] args)
        {
            library.DoRead();
            Console.WriteLine("Hello World!");
            UIHeader();
            AwaitCommand();
        }

        static void UIHeader()
        {
            Console.WriteLine("To add a book, type \"Add \" and books json file path.");
            Console.WriteLine("To take a book, type \"Take \" ISBN of the book, takers name and for what period the book is taken (months).");
            Console.WriteLine("To return a book, type \"Return\" and books ISBN and name of the person who took it.");
            Console.WriteLine("To list all books, type \"List\" and chosen filters, if any (Author=? Category=? Language=? ISBN=? Name=? Taken=?), where ? is value. Example: List Author=Thomas_Thompson Language=Lithuanian Taken=false");
            Console.WriteLine("To delete a book, type \"Delete\" and books ISBN.");
        }
        static string AddBook(string path)
        {
            string jsonString = File.ReadAllText(path);
            Book newBook = JsonSerializer.Deserialize<Book>(jsonString);
            newBook.Taken = false;
            string returnString = library.AddBook(newBook);
            library.UpdateLibraryJSON();
            return returnString;
        }
        static string TakeBook(string ISBN, string name, int months)
        {
            
            string returnString = library.TakeBook(ISBN, name, months);
            library.UpdateLibraryJSON();
            library.UpdateTakenBookJSON();
            return returnString;
        }
        static string ReturnBook(string ISBN, string returnee)
        {
            string returnString =  library.ReturnBook(ISBN, returnee);
            library.UpdateTakenBookJSON();
            library.UpdateLibraryJSON();
            return returnString;
        }
        static string DeleteBook(string ISBN)
        {
            string returnString = library.DeleteBook(ISBN);
            library.UpdateTakenBookJSON();
            library.UpdateLibraryJSON();
            return returnString;
        }
        static string BuildListString(string[] parts)
        {
            List<Book> books = library.ListBooks();
            if (parts.Length > 1)
            {
                books = FilterList(books, parts);
            }
            string list = "";
            foreach (Book book in books)
            {
                list+=book.ToString()+"\n";
            }
            return list;
        }
        // TODO:
        // Remake the delegate method. Curently reusable with string and bool values only.
        static List<Book> FilterList(List<Book> books, string[] parts)
        {
            for (int i = 1; i < parts.Length; i++)
            {
                string[] props = parts[i].Split('=');
                books = books.FindAll(
                    delegate(Book book)
                    {
                        props[1] = props[1].Replace('_', ' ');
                        var value = book.GetType().GetProperty(props[0]).GetValue(book);
                        if (bool.TryParse(props[1], out bool propBool) && (value is bool boolean))
                        {
                            return boolean == propBool;
                        }
                        return (string)value == props[1];
                    }
                );
            }
            return books;
        }
        static void AwaitCommand()
        {
            string command;
            while (true)
            {
                command = Console.ReadLine();
                if (ProcessCommand(command) == 1)
                {
                    return;
                }
            }
        }
        static int ProcessCommand(string command)
        {
            string[] parts;
            parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return 0;
            }
            // TODO:
            // Add command string template validation
            switch (parts[0])
            {
                case "Add":
                    System.Console.WriteLine(AddBook(parts[1])); 
                    break;
                case "Take":
                    System.Console.WriteLine(TakeBook(parts[1], parts[2], Convert.ToInt32(parts[3])));
                    break;
                case "Return":
                    System.Console.WriteLine(ReturnBook(parts[1], parts[2])); 
                    break;
                case "List":
                    System.Console.WriteLine(BuildListString(parts));
                    break;
                case "Delete":
                    System.Console.WriteLine(DeleteBook(parts[1]));
                    break;
                case "Help":
                    UIHeader();
                    break;
                case "Exit":
                    return 1;
                default:
                    Console.WriteLine(String.Format("Command \"{0}\" not recognized. Type \"Help\" to see available commands.", command));
                    break;
            }
            return 0;
        }
    }
}

using System.Collections.Generic;
using System;
using Nancy;
using Library;

namespace Library
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] =_=> View["index.cshtml"];
      Get["/books"] =_=> View["books.cshtml", Book.AllCopyIdBookPairs()];
      Post["/books"] =_=> {
        Book newBook = new Book(Request.Form["book-title"], genreId: Request.Form["book-genre"]);
        newBook.Save();
        Book.AddCopy(newBook.GetId());
        return View["books.cshtml", Book.AllCopyIdBookPairs()];
      };
      Post["/books/{id}/addcopy"] =chocolate=> {
        Book.AddCopy(chocolate.id);
        return View["books.cshtml", Book.AllCopies()];
      };
      Get["/books/{id}"] =parameters=> {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Book foundBook = Book.Find(parameters.id);
        List<Author> allAuthors = Author.GetAll();
        model.Add("book", foundBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Post["/books/{id}/author"] =parameters=> {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Book foundBook = Book.Find(parameters.id);
        foundBook.AddAuthor(Author.Find(Request.Form["author-id"]));
        List<Author> allAuthors = Author.GetAll();
        model.Add("book", foundBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Post["/books/{id}/newauthor"] =parameters=> {
        Dictionary<string, object> model = new Dictionary<string, object>{};
        Author newAuthor = new Author(Request.Form["author-name"]);
        newAuthor.Save();
        Book foundBook = Book.Find(parameters.id);
        foundBook.AddAuthor(newAuthor);
        List<Author> allAuthors = Author.GetAll();
        model.Add("book", foundBook);
        model.Add("authors", allAuthors);
        return View["book.cshtml", model];
      };
      Get["/patrons"] =_=> View["patrons.cshtml", Patron.GetAll()];
      Post["/patrons"] =_=> {
        Patron newPatron = new Patron(Request.Form["patron-name"], Request.Form["patron-email"], Request.Form["patron-number"]);
        newPatron.Save();
        return View["patrons.cshtml", Patron.GetAll()];
      };
    }
  }
}

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
        foreach(var pair in Book.AllCopyIdBookPairs())
        {
          System.Console.WriteLine(pair.Key);
          System.Console.WriteLine(pair.Value);
        }
        return View["books.cshtml", Book.AllCopyIdBookPairs()];
      };
      Post["/books/{id}/addcopy"] =chocolate=> {
        Book.AddCopy(chocolate.id);
        return View["books.cshtml", Book.AllCopies()];
      };
      Get["/books/{id}"] =parameters=> {
        Book foundBook = Book.Find(parameters.id);
        return View["book.cshtml", foundBook];
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

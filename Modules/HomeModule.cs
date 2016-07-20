using System.Collections.Generic;
using System;
using Nancy;

namespace Library
{
  public class HomeModule : NancyModule
  {
    public HomeModule()
    {
      Get["/"] =_=> View["index.cshtml"];
      Get["/books"] =_=> View["books.cshtml", Book.AllCopies()];
      Post["/books"] =_=> {
        Book newBook = new Book(Request.Form["book-title"], genreId: Request.Form["book-genre"]);
        newBook.Save();
        Book.AddCopy(newBook.GetId());
        return View["books.cshtml", Book.AllCopies()];
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

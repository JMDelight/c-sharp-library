using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class BookTest : IDisposable
  {
    public BookTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Book.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Book firstBook = new Book("History of the Peloponessian War");
      Book secondBook = new Book("History of the Peloponessian War");

      //Assert
      Assert.Equal(firstBook, secondBook);
    }
    [Fact]
    public void Test_Save_SavesBookToDatabase()
    {
      //Arrange
      Book testBook = new Book("History");
      testBook.Save();

      //Act
      List<Book> result = Book.GetAll();
      List<Book> testList = new List<Book>{testBook};

      //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Save_AssignsIdToBookObject()
    {
      //Arrange
      Book testBook = new Book("History");
      testBook.Save();

      //Act
      Book savedBook = Book.GetAll()[0];

      int result = savedBook.GetId();
      int testId = testBook.GetId();

      //Assert
      Assert.Equal(testId, result);
    }
    public void Dispose()
    {
      Book.DeleteAll();
    }
  }
}

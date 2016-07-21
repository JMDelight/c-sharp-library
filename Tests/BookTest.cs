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
    [Fact]
    public void Test_Find_FindsBookInDatabase()
    {
      //Arrange
      Book testBook = new Book("History");
      Book testBook2 = new Book("Biology");
      testBook.Save();

      //Act
      Book foundBook = Book.Find(testBook.GetId());

      //Assert
      Assert.Equal(testBook, foundBook);
    }
    [Fact]
    public void Test_AddCopy_AddsCopyOfBookToDatabase()
    {
      //Arrange
      Book testBook = new Book("History");
      testBook.Save();

      //Act
      Book.AddCopy(testBook.GetId());
      List<int> copies = Book.GetCopies(testBook.GetId());

      //Assert
      Assert.Equal(1, copies.Count);
    }
    [Fact]
    public void Test_Update_UpdatesInfoInDatabaseToMatchInstance()
    {
      //Arrange
      Book testBook = new Book("History");
      Book testBook2 = new Book("Biology");
      testBook.Save();

      //Act
      Book.Update(testBook.GetId(), testBook2);
      Book foundBook = Book.Find(testBook.GetId());
      testBook2.SetId(testBook.GetId());
      //Assert
      Assert.Equal(testBook2, foundBook);
    }
    [Fact]
    public void Test_Delete_DeletesSpecifiedBookFromDatabaseAndNoOthers()
    {
      //Arrange
      Book testBook = new Book("History");
      Book testBook2 = new Book("Biology");
      testBook.Save();
      testBook2.Save();

      //Act
      Book.Delete(testBook.GetId());

      Assert.Equal(1, Book.GetAll().Count);
    }
    [Fact]
    public void Test_AddAuthorsGetAuthors_AddsAndRetrievesAuthorsForBookObject()
    {
      //Arrange
      Book testBook = new Book("History");
      Book testBook2 = new Book("Biology");
      testBook.Save();
      testBook2.Save();
      Author testAuthor = new Author("Bill");
      Author testAuthor2 = new Author("Bob");
      Author testAuthor3 = new Author("Frank");
      testAuthor.Save();
      testAuthor3.Save();
      testAuthor2.Save();
      List <Author> expectedAuthors = new List<Author> {testAuthor, testAuthor3};

      //Act
      testBook.AddAuthor(testAuthor);
      testBook.AddAuthor(testAuthor3);
      List<Author> result = testBook.GetAuthors();

      //Assert
      Assert.Equal(expectedAuthors, result);
    }
    public void Dispose()
    {
      Book.DeleteAll();
    }
  }
}

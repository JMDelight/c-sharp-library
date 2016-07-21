using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class AuthorTest : IDisposable
  {
    public AuthorTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Author.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Author firstAuthor = new Author("Bob");
      Author secondAuthor = new Author("Bob");

      //Assert
      Assert.Equal(firstAuthor, secondAuthor);
    }
    [Fact]
    public void Test_Save_SavesAuthorToDatabase()
    {
      //Arrange
      Author testAuthor = new Author("Bob");
      testAuthor.Save();

      //Act
      List<Author> result = Author.GetAll();
      List<Author> testList = new List<Author>{testAuthor};

      //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Save_AssignsIdToAuthorObject()
    {
      //Arrange
      Author testAuthor = new Author("Bob");
      testAuthor.Save();

      //Act
      Author savedAuthor = Author.GetAll()[0];

      int result = savedAuthor.GetId();
      int testId = testAuthor.GetId();

      //Assert
      Assert.Equal(testId, result);
    }
    [Fact]
    public void Test_Find_FindsAuthorInDatabase()
    {
      //Arrange
      Author testAuthor = new Author("Bob");
      Author testAuthor2 = new Author("Bill");
      testAuthor.Save();

      //Act
      Author foundAuthor = Author.Find(testAuthor.GetId());

      //Assert
      Assert.Equal(testAuthor, foundAuthor);
    }
    [Fact]
    public void Test_Update_UpdatesInfoInDatabaseToMatchInstance()
    {
      //Arrange
      Author testAuthor = new Author("Bob");
      Author testAuthor2 = new Author("Bill");
      testAuthor.Save();

      //Act
      Author.Update(testAuthor.GetId(), testAuthor2);
      Author foundAuthor = Author.Find(testAuthor.GetId());
      testAuthor2.SetId(testAuthor.GetId());
      //Assert
      Assert.Equal(testAuthor2, foundAuthor);
    }
    [Fact]
    public void Test_Delete_DeletesSpecifiedAuthorFromDatabaseAndNoOthers()
    {
      //Arrange
      Author testAuthor = new Author("Bob");
      Author testAuthor2 = new Author("Bill");
      testAuthor.Save();
      testAuthor2.Save();

      //Act
      Author.Delete(testAuthor.GetId());

      Assert.Equal(1, Author.GetAll().Count);
    }
    // [Fact]
    // public void Test_Checkout_CheckoutsSpecifiedBookCopyFromDatabase()
    // {
    //   //Arrange
    //   Author testAuthor = new Author("Bob");
    //   Book testBook = new Book("History");
    //   Book testBook2 = new Book("Biology");
    //
    //   testAuthor.Save();
    //   testBook.Save();
    //   testBook2.Save();
    //   Book.AddCopy(testBook.GetId());
    //
    //
    //   //Act
    //   testAuthor.Checkout(Book.GetCopies(testBook.GetId())[0]);
    //
    //   Assert.Equal(1, testAuthor.GetUnreturnedBooks().Count);
    // }
    public void Dispose()
    {
      Author.DeleteAll();
      Book.DeleteAll();
    }
  }
}

using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Library
{
  public class PatronTest : IDisposable
  {
    public PatronTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=library_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Patron.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
    public void Test_Equal_ReturnsTrueForSameName()
    {
      //Arrange, Act
      Patron firstPatron = new Patron("Bob", "Bob@bob.bob", 2);
      Patron secondPatron = new Patron("Bob", "Bob@bob.bob", 2);

      //Assert
      Assert.Equal(firstPatron, secondPatron);
    }
    [Fact]
    public void Test_Save_SavesPatronToDatabase()
    {
      //Arrange
      Patron testPatron = new Patron("Bob", "Bob@bob.bob", 2);
      testPatron.Save();

      //Act
      List<Patron> result = Patron.GetAll();
      List<Patron> testList = new List<Patron>{testPatron};

      //Assert
      Assert.Equal(testList, result);
    }
    [Fact]
    public void Test_Save_AssignsIdToPatronObject()
    {
      //Arrange
      Patron testPatron = new Patron("Bob", "Bob@bob.bob", 2);
      testPatron.Save();

      //Act
      Patron savedPatron = Patron.GetAll()[0];

      int result = savedPatron.GetId();
      int testId = testPatron.GetId();

      //Assert
      Assert.Equal(testId, result);
    }
    [Fact]
    public void Test_Find_FindsPatronInDatabase()
    {
      //Arrange
      Patron testPatron = new Patron("Bob", "Bob@bob.bob", 2);
      Patron testPatron2 = new Patron("Bill", "Bill@bob.bob", 3);
      testPatron.Save();

      //Act
      Patron foundPatron = Patron.Find(testPatron.GetId());

      //Assert
      Assert.Equal(testPatron, foundPatron);
    }
    public void Dispose()
    {
      Patron.DeleteAll();
    }
  }
}

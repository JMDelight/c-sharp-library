using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Book
  {
    private int _id;
    private string _title;
    private int _genreId;

    public Book(string title, int id = 0, int genreId = 1)
    {
      _title = title;
      _genreId = genreId;
      _id = id;
    }
    public override bool Equals(System.Object otherBook)
    {
        if (!(otherBook is Book))
        {
          return false;
        }
        else
        {
          Book newBook = (Book) otherBook;
          bool idEquality = this.GetId() == newBook.GetId();
          bool titleEquality = this.GetTitle() == newBook.GetTitle();
          bool genreIdEquality = this.GetGenreId() == newBook.GetGenreId();
          return (idEquality && titleEquality && genreIdEquality);
        }
    }
    public string GetTitle()
    {
      return _title;
    }
    public void SetTitle(string newTitle)
    {
      _title = newTitle;
    }

    public int GetGenreId()
    {
      return _genreId;
    }
    public void SetGenreId(int newGenreId)
    {
      _genreId = newGenreId;
    }

    public int GetId()
    {
      return _id;
    }

    public static List<Book> GetAll()
    {
      List<Book> allBooks = new List<Book> {};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int bookId = rdr.GetInt32(0);
        string bookTitle = rdr.GetString(1);
        int bookGenreId = rdr.GetInt32(2);
        Book newBook = new Book(bookTitle, bookId, bookGenreId);
        allBooks.Add(newBook);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allBooks;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO books (title, genre_id) OUTPUT INSERTED.id VALUES (@BookTitle, @GenreId);", conn);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@BookTitle";
      titleParameter.Value = this.GetTitle();

      SqlParameter genreIdParameter = new SqlParameter();
      genreIdParameter.ParameterName = "@GenreId";
      genreIdParameter.Value = this.GetGenreId();

      cmd.Parameters.Add(titleParameter);
      cmd.Parameters.Add(genreIdParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }
    public static Book Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM books WHERE id = @BookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = id.ToString();
      cmd.Parameters.Add(bookIdParameter);
      rdr = cmd.ExecuteReader();

      int foundBookId = 0;
      string foundBookTitle = null;
      int foundGenreId = 0;

      while(rdr.Read())
      {
        foundBookId = rdr.GetInt32(0);
        foundBookTitle = rdr.GetString(1);
        foundGenreId = rdr.GetInt32(2);
      }
      Book foundBook = new Book(foundBookTitle, foundBookId, foundGenreId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundBook;
    }
    public static void AddCopy(int bookId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO copies (book_id, checked_out) VALUES (@bookId, 0);", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = bookId;

      cmd.Parameters.Add(bookIdParameter);
      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public static List<int> GetCopies(int bookId)
    {
      List<int> copyIds = new List<int> {};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT id FROM copies WHERE book_id = @bookId;", conn);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = bookId;

      cmd.Parameters.Add(bookIdParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int foundBookId = rdr.GetInt32(0);
        copyIds.Add(foundBookId);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return copyIds;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM books;DELETE FROM copies", conn);
      cmd.ExecuteNonQuery();
    }
  }
}

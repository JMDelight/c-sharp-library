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
    public void SetId(int newId)
    {
      _id = newId;
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
    public void AddAuthor(Author bookAuthor)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO authors_books (author_id, book_id) VALUES (@authorId, @bookId);", conn);

      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@authorId";
      authorIdParameter.Value = bookAuthor.GetId();
      cmd.Parameters.Add(authorIdParameter);

      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = this.GetId();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public List<Author> GetAuthors()
    {
      List<Author> booksAuthors = new List<Author> {};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT authors.* FROM books JOIN authors_books ON (books.id = authors_books.book_id) JOIN authors ON (authors_books.author_id = authors.id) WHERE books.id = @bookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@bookId";
      bookIdParameter.Value = this.GetId().ToString();
      cmd.Parameters.Add(bookIdParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int foundAuthorId = rdr.GetInt32(0);
        string foundAuthorName = rdr.GetString(1);
        Author foundAuthor = new Author(foundAuthorName, foundAuthorId);
        booksAuthors.Add(foundAuthor);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return booksAuthors;
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
    public static List<int> AllCopies()
    {
      List<Book> allBooks = Book.GetAll();
      List<int> allCopyIds = new List<int> {};
      foreach(Book book in allBooks)
      {
         allCopyIds.AddRange(Book.GetCopies(book.GetId()));
      }
      return allCopyIds;
    }
    public static Book FindByCopyId(int queryId)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT books.* FROM copies JOIN books ON (copies.book_id = books.id) WHERE copies.id = @queryId;", conn);
      SqlParameter queryIdParameter = new SqlParameter();
      queryIdParameter.ParameterName = "@queryId";
      queryIdParameter.Value = queryId.ToString();
      cmd.Parameters.Add(queryIdParameter);
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
    public static Dictionary<string, object> AllCopyIdBookPairs()
    {
      Dictionary<string, object> copyIdBookPairs = new Dictionary<string, object>{};
      foreach(int copyId in Book.AllCopies())
      {
        string copyIdString = copyId.ToString();
        Book dictionaryBook = Book.FindByCopyId(copyId);
        copyIdBookPairs.Add(copyIdString, dictionaryBook);
      }
      return copyIdBookPairs;
    }
    public static void DeleteCopy(int QueryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM copies WHERE id = @copyId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@copyId";
      bookIdParameter.Value = QueryId.ToString();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }

    public static void Update(int queryId, Book updateBook)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE books SET title = @bookTitle, genre_id = @genreId WHERE id = @queryId;", conn);

      SqlParameter titleParameter = new SqlParameter();
      titleParameter.ParameterName = "@bookTitle";
      titleParameter.Value = updateBook.GetTitle();
      cmd.Parameters.Add(titleParameter);

      SqlParameter queryIdParameter = new SqlParameter();
      queryIdParameter.ParameterName = "@queryId";
      queryIdParameter.Value = queryId;
      cmd.Parameters.Add(queryIdParameter);

      SqlParameter genreIdParameter = new SqlParameter();
      genreIdParameter.ParameterName = "@genreId";
      genreIdParameter.Value = updateBook.GetGenreId();
      cmd.Parameters.Add(genreIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public static void Delete(int QueryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM books WHERE id = @BookId;", conn);
      SqlParameter bookIdParameter = new SqlParameter();
      bookIdParameter.ParameterName = "@BookId";
      bookIdParameter.Value = QueryId.ToString();
      cmd.Parameters.Add(bookIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
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

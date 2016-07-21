using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Author
  {
    private int _id;
    private string _authorName;
    private string _email;
    private int _phoneNumber;
    private bool _finesOutstanding;


    public Author(string authorName, int id = 0)
    {
      _authorName = authorName;
      _id = id;
    }
    public override bool Equals(System.Object otherAuthor)
    {
        if (!(otherAuthor is Author))
        {
          return false;
        }
        else
        {
          Author newAuthor = (Author) otherAuthor;
          bool idEquality = this.GetId() == newAuthor.GetId();
          bool authorNameEquality = this.GetAuthorName() == newAuthor.GetAuthorName();
          return (idEquality && authorNameEquality);
        }
    }
    public string GetAuthorName()
    {
      return _authorName;
    }
    public void SetAuthorName(string newAuthorName)
    {
      _authorName = newAuthorName;
    }
    public int GetId()
    {
      return _id;
    }
    public void SetId(int newId)
    {
      _id = newId;
    }

    public static List<Author> GetAll()
    {
      List<Author> allAuthors = new List<Author> {};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int authorId = rdr.GetInt32(0);
        string authorName = rdr.GetString(1);
        Author newAuthor = new Author(authorName, authorId);
        allAuthors.Add(newAuthor);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allAuthors;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO authors (name) OUTPUT INSERTED.id VALUES (@authorName);", conn);

      SqlParameter authorNameParameter = new SqlParameter();
      authorNameParameter.ParameterName = "@authorName";
      authorNameParameter.Value = this.GetAuthorName();

      cmd.Parameters.Add(authorNameParameter);

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
    public static Author Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM authors WHERE id = @authorId;", conn);
      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@authorId";
      authorIdParameter.Value = id.ToString();
      cmd.Parameters.Add(authorIdParameter);
      rdr = cmd.ExecuteReader();

      int foundAuthorId = 0;
      string foundAuthorName = null;

      while(rdr.Read())
      {
        foundAuthorId = rdr.GetInt32(0);
        foundAuthorName = rdr.GetString(1);
      }
      Author foundAuthor = new Author(foundAuthorName, foundAuthorId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundAuthor;
    }

    public static void Update(int queryId, Author updateAuthor)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE authors SET name = @authorName WHERE id = @queryId;", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@authorName";
      nameParameter.Value = updateAuthor.GetAuthorName();
      cmd.Parameters.Add(nameParameter);

      SqlParameter queryIdParameter = new SqlParameter();
      queryIdParameter.ParameterName = "@queryId";
      queryIdParameter.Value = queryId;
      cmd.Parameters.Add(queryIdParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    // public void Checkout(int copyId)
    // {
    //   SqlConnection conn = DB.Connection();
    //   conn.Open();
    //
    //   SqlCommand cmd = new SqlCommand("INSERT INTO checkouts (copy_id, author_id, due_date, return_date) VALUES(@copyId, @authorId, @dueDate, @returnDate); UPDATE copies SET checked_out = 1;", conn);
    //
    //   SqlParameter authorIdParameter = new SqlParameter();
    //   authorIdParameter.ParameterName = "@authorId";
    //   authorIdParameter.Value = this.GetId();
    //   cmd.Parameters.Add(authorIdParameter);
    //
    //   SqlParameter copyIdParameter = new SqlParameter();
    //   copyIdParameter.ParameterName = "@copyId";
    //   copyIdParameter.Value = copyId;
    //   cmd.Parameters.Add(copyIdParameter);
    //
    //   SqlParameter dueDateParameter = new SqlParameter();
    //   dueDateParameter.ParameterName = "@dueDate";
    //   dueDateParameter.Value = DateTime.Now.AddDays(30);
    //   cmd.Parameters.Add(dueDateParameter);
    //
    //   SqlParameter returnDateParameter = new SqlParameter();
    //   returnDateParameter.ParameterName = "@returnDate";
    //   returnDateParameter.Value = new DateTime(2000,1,1);
    //   cmd.Parameters.Add(returnDateParameter);
    //
    //   cmd.ExecuteNonQuery();
    //
    //   if (conn != null)
    //   {
    //     conn.Close();
    //   }
    // }
    // public List<int> GetUnreturnedBooks()
    // {
    //   List<int> copyIds = new List<int> {};
    //   SqlConnection conn = DB.Connection();
    //   SqlDataReader rdr = null;
    //   conn.Open();
    //
    //   SqlCommand cmd = new SqlCommand("SELECT copy_id FROM checkouts WHERE author_id = @authorId AND return_date =  '2000-01-01';" ,conn);
    //
    //   SqlParameter authorIdParameter = new SqlParameter();
    //   authorIdParameter.ParameterName = "@authorId";
    //   authorIdParameter.Value = this.GetId();
    //   cmd.Parameters.Add(authorIdParameter);
    //
    //   rdr = cmd.ExecuteReader();
    //   while(rdr.Read())
    //   {
    //     int foundCopyId = rdr.GetInt32(0);
    //     copyIds.Add(foundCopyId);
    //   }
    //   if(rdr != null)
    //   {
    //     rdr.Close();
    //   }
    //   if(conn != null)
    //   {
    //     conn.Close();
    //   }
    //   return copyIds;
    // }
    public static void Delete(int QueryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM authors WHERE id = @AuthorId;DELETE FROM authors_books WHERE author_id = @AuthorId;", conn);
      SqlParameter authorIdParameter = new SqlParameter();
      authorIdParameter.ParameterName = "@AuthorId";
      authorIdParameter.Value = QueryId.ToString();
      cmd.Parameters.Add(authorIdParameter);

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
      SqlCommand cmd = new SqlCommand("DELETE FROM authors; DELETE FROM authors_books;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}

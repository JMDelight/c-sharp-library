using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Library
{
  public class Patron
  {
    private int _id;
    private string _patronName;
    private string _email;
    private int _phoneNumber;
    private bool _finesOutstanding;


    public Patron(string patronName, string email, int phoneNumber, int id = 0, bool finesOutstanding = false)
    {
      _patronName = patronName;
      _email = email;
      _id = id;
      _phoneNumber = phoneNumber;
      _finesOutstanding = finesOutstanding;
    }
    public override bool Equals(System.Object otherPatron)
    {
        if (!(otherPatron is Patron))
        {
          return false;
        }
        else
        {
          Patron newPatron = (Patron) otherPatron;
          bool idEquality = this.GetId() == newPatron.GetId();
          bool patronNameEquality = this.GetPatronName() == newPatron.GetPatronName();
          bool emailEquality = this.GetEmail() == newPatron.GetEmail();
          bool phoneNumberEquality = this.GetPhoneNumber() == newPatron.GetPhoneNumber();
          bool finesEquality = this.GetFinesOutstanding() == newPatron.GetFinesOutstanding();
          return (idEquality && patronNameEquality && emailEquality && phoneNumberEquality && finesEquality);
        }
    }
    public string GetPatronName()
    {
      return _patronName;
    }
    public void SetPatronName(string newPatronName)
    {
      _patronName = newPatronName;
    }

    public string GetEmail()
    {
      return _email;
    }
    public void SetEmail(string newEmail)
    {
      _email = newEmail;
    }

    public int GetPhoneNumber()
    {
      return _phoneNumber;
    }
    public void SetPhoneNumber(int newPhoneNumber)
    {
      _phoneNumber = newPhoneNumber;
    }

    public bool GetFinesOutstanding()
    {
      return _finesOutstanding;
    }

    public int GetId()
    {
      return _id;
    }
    public void SetId(int newId)
    {
      _id = newId;
    }

    public static List<Patron> GetAll()
    {
      List<Patron> allPatrons = new List<Patron> {};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patrons;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int patronId = rdr.GetInt32(0);
        string patronName = rdr.GetString(1);
        string patronEmail = rdr.GetString(2);
        int patronPhoneNumber = rdr.GetInt32(3);
        bool patronFinesOutstanding = rdr.GetBoolean(4);
        Patron newPatron = new Patron(patronName, patronEmail, patronPhoneNumber, patronId, patronFinesOutstanding);
        allPatrons.Add(newPatron);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return allPatrons;
    }
    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO patrons (name, email, phone_number, fines_outstanding) OUTPUT INSERTED.id VALUES (@patronName, @email, @patronPhoneNumber, @finesOutstanding);", conn);

      SqlParameter patronNameParameter = new SqlParameter();
      patronNameParameter.ParameterName = "@patronName";
      patronNameParameter.Value = this.GetPatronName();

      SqlParameter emailParameter = new SqlParameter();
      emailParameter.ParameterName = "@email";
      emailParameter.Value = this.GetEmail();

      SqlParameter patronPhoneNumberParameter = new SqlParameter();
      patronPhoneNumberParameter.ParameterName = "@patronPhoneNumber";
      patronPhoneNumberParameter.Value = this.GetPhoneNumber();

      SqlParameter finesOutstandingParameter = new SqlParameter();
      finesOutstandingParameter.ParameterName = "@finesOutstanding";
      finesOutstandingParameter.Value = this.GetFinesOutstanding();

      cmd.Parameters.Add(patronNameParameter);
      cmd.Parameters.Add(emailParameter);
      cmd.Parameters.Add(patronPhoneNumberParameter);
      cmd.Parameters.Add(finesOutstandingParameter);

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
    public static Patron Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM patrons WHERE id = @patronId;", conn);
      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@patronId";
      patronIdParameter.Value = id.ToString();
      cmd.Parameters.Add(patronIdParameter);
      rdr = cmd.ExecuteReader();

      int foundPatronId = 0;
      string foundPatronName = null;
      string foundEmail = null;
      int foundPhoneNumber = 0;
      bool foundFinesOutstanding = false;


      while(rdr.Read())
      {
        foundPatronId = rdr.GetInt32(0);
        foundPatronName = rdr.GetString(1);
        foundEmail = rdr.GetString(2);
        foundPhoneNumber = rdr.GetInt32(3);
        foundFinesOutstanding = rdr.GetBoolean(4);
      }
      Patron foundPatron = new Patron(foundPatronName, foundEmail, foundPhoneNumber, foundPatronId, foundFinesOutstanding);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundPatron;
    }

    public static void Update(int queryId, Patron updatePatron)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("UPDATE patrons SET name = @patronName, email = @email, phone_number = @phoneNumber, fines_outstanding = @finesOutstanding  WHERE id = @queryId;", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@patronName";
      nameParameter.Value = updatePatron.GetPatronName();
      cmd.Parameters.Add(nameParameter);

      SqlParameter queryIdParameter = new SqlParameter();
      queryIdParameter.ParameterName = "@queryId";
      queryIdParameter.Value = queryId;
      cmd.Parameters.Add(queryIdParameter);

      SqlParameter emailParameter = new SqlParameter();
      emailParameter.ParameterName = "@email";
      emailParameter.Value = updatePatron.GetEmail();
      cmd.Parameters.Add(emailParameter);

      SqlParameter phoneNumberParameter = new SqlParameter();
      phoneNumberParameter.ParameterName = "@phoneNumber";
      phoneNumberParameter.Value = updatePatron.GetPhoneNumber();
      cmd.Parameters.Add(phoneNumberParameter);

      SqlParameter finesOutstandingParameter = new SqlParameter();
      finesOutstandingParameter.ParameterName = "@finesOutstanding";
      finesOutstandingParameter.Value = updatePatron.GetFinesOutstanding();
      cmd.Parameters.Add(finesOutstandingParameter);


      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public void Checkout(int copyId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO checkouts (copy_id, patron_id, due_date, return_date) VALUES(@copyId, @patronId, @dueDate, @returnDate); UPDATE copies SET checked_out = 1;", conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@patronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      SqlParameter copyIdParameter = new SqlParameter();
      copyIdParameter.ParameterName = "@copyId";
      copyIdParameter.Value = copyId;
      cmd.Parameters.Add(copyIdParameter);

      SqlParameter dueDateParameter = new SqlParameter();
      dueDateParameter.ParameterName = "@dueDate";
      dueDateParameter.Value = DateTime.Now.AddDays(30);
      cmd.Parameters.Add(dueDateParameter);

      SqlParameter returnDateParameter = new SqlParameter();
      returnDateParameter.ParameterName = "@returnDate";
      returnDateParameter.Value = new DateTime(2000,1,1);
      cmd.Parameters.Add(returnDateParameter);

      cmd.ExecuteNonQuery();

      if (conn != null)
      {
        conn.Close();
      }
    }
    public List<int> GetUnreturnedBooks()
    {
      List<int> copyIds = new List<int> {};
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT copy_id FROM checkouts WHERE patron_id = @patronId AND return_date =  '2000-01-01';" ,conn);

      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@patronId";
      patronIdParameter.Value = this.GetId();
      cmd.Parameters.Add(patronIdParameter);

      rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int foundCopyId = rdr.GetInt32(0);
        copyIds.Add(foundCopyId);
      }
      if(rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
      return copyIds;
    }
    public static void Delete(int QueryId)
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM patrons WHERE id = @PatronId;", conn);
      SqlParameter patronIdParameter = new SqlParameter();
      patronIdParameter.ParameterName = "@PatronId";
      patronIdParameter.Value = QueryId.ToString();
      cmd.Parameters.Add(patronIdParameter);

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
      SqlCommand cmd = new SqlCommand("DELETE FROM patrons;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}

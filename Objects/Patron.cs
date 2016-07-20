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
    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM patrons;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System.Text;
using ChangesControl.Models;

/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{
    public SqlDataAdapter da;
    public DataTable dt;
    SqlConnection con;
    SqlCommand cmd;
    public DBservices()
    {
        //
        // TODO: Add constructor logic here
        //
    }


    public void Connection()
    {
        try
        {
            con = connect("DBConnectionString"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }
    }

    //--------------------------------------------------------------------------------------------------
    // connection
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {
        // read the connection string from the configuration file
        string cStr = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
        //add to webconfig
        //  <connectionStrings>
        //< !--< add name = "destinationsDBConnectionString" connectionString = "Data Source=Media.ruppin.ac.il;
        //Initial Catalog = databaseName; User ID = igroup20; Password = igroup20_" />-->
        //      < add name = "destinationsDBConnectionString" connectionString = "Data Source=ALSQL\ALLIANCE;
        //Initial Catalog = Test; User ID = albi; Password = Al5342" />
        //    </ connectionStrings >
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }


    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommand(String CommandSTR, SqlConnection con)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = CommandSTR;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.Text; // the type of the command, can also be stored procedure

        return cmd;
    }
    //--------------------------------------------------------------------------------------------------
    // delete format
    //--------------------------------------------------------------------------------------------------

    public int delete(int id)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("DBConnectionString"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        String cStr = BuildDeleteCommand(id);      // helper method to build the insert string

        cmd = CreateCommand(cStr, con);             // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            return 0;
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }



    //--------------------------------------------------------------------------------------------------
    // insert format
    //--------------------------------------------------------------------------------------------------
    public int insert(string query)
    {

        try
        {
            Connection();
            int numEffected = 0;


            //cStr = BuildInsertCommand();      // helper method to build the insert string
            cmd = CreateCommand(query, con);             // create the command
            numEffected += cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }

        catch (Exception ex)
        {
            return 0;
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();

            }
        }

    }



    //--------------------------------------------------------------------
    // query format
    //--------------------------------------------------------------------
    public String BuildInsertChanges(Changes change)
    {
        String command;
        
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        command = $@"INSERT INTO ChangessS400 VALUES({change.Id},'{change.NameInitated}','{change.Programmer}',{change.ProgrammerNum},'{change.Date}','{change.Subject}','{change.Description}','{change.ApprovedIt}','{change.AprrovedInitated}','{change.FinanceApproveNecessary}','{change.ApprovedFinance}')";
        return command;
    }

    /// <summary>
    /// query for objects changed in one change
    /// </summary>
    internal string BuildInsertObjectsAffected(Objects obj,int Id)
    {
        String command = $@"INSERT INTO ChangesObject VALUES({Id},'{obj.Libary}','{obj.Object}','{obj.Comments}')";
        return command;
    }

    internal string BuildInsertObjectsAffected(Tests test, int Id)
    {
        String command = $@"INSERT INTO ChangesTest VALUES({Id},'{test.Description}','{test.ApprovedInitated}','{test.ApprovedProgrammer}')";
        return command;
    }

    /// <summary>
    /// import from db format
    /// </summary>
    internal List<User> importDataChangesPremission()
    {
        SqlConnection con;
        SqlCommand cmd;
        User user = new User();
        List<User> usersList = new List<User>();
        try
        {
            con = connect("DBConnectionString"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        try
        {
            String cStr = $@"SELECT*
                             FROM ChangesPremission"; ;
            cmd = CreateCommand(cStr, con);             // create the command
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    user.UserName = reader["UserName"].ToString();
                    user.FullName = reader["Name"].ToString();
                    user.PremissionEdit = Convert.ToBoolean(reader["Edit"]);
                    user.PremissionIT= Convert.ToBoolean(reader["It"]);
                    user.PremissionFinance= Convert.ToBoolean(reader["Finance"]);
                    usersList.Add(user);
                    user = new User();
                }
            }
            else
            {
                Console.WriteLine("No rows found.");
            }
            reader.Close();

        }

        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
        return usersList;
   
    }
    //--------------------------------------------------------------------
    // delete auery format
    //--------------------------------------------------------------------
    private String BuildDeleteCommand(int id)
    {
        String command;

        //StringBuilder sb = new StringBuilder();
        //// use a string builder to create the dynamic string
        //sb.AppendFormat("Values('{0}', '{1}')", movie.Name, movie.Actor);
        //String prefix = "INSERT INTO movie_2020 " + "(Name, Actor) ";
        //command = prefix + sb.ToString();

        command = "delete from movie_2020 where movieid = " + id.ToString();

        return command;
    }

    //datatable format
    public DataTable GetDataTable(string qry)
    {
        SqlConnection con = null;
        DataTable dataTable = new DataTable();
        try
        {
            con = connect("DBConnectionString");
            da = new SqlDataAdapter(qry, con);
            SqlCommandBuilder builder = new SqlCommandBuilder(da);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataTable = ds.Tables[0];
            cmd = CreateCommand(qry, con);             // create the command
         
        }

        catch (Exception ex)
        {
            // write errors to log file
            // try to handle the error
            throw ex;
        }

        finally
        {
            if (con != null)
            {
                con.Close();
            }
        }


        return dataTable;

    }

    public void update(DataTable dt)
    {
        da.Update(dt);
    }





}

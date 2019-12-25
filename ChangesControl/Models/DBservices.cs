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

    public DBservices()
    {
        //
        // TODO: Add constructor logic here
        //
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

        try
        {
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
    public String BuildInsertCommand(Changes change)
    {
        String command;
        
        StringBuilder sb = new StringBuilder();
        // use a string builder to create the dynamic string
        String prefix = "";

        sb.AppendFormat("Values({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8})",change.EmployeeNum, change.Name,DateTime.Parse(change.Date.ToString()).ToString("yyyy-MM-dd"),change.File,change.Libary,change.Subject,change.Description,change.AddedFile,0); //sb.AppendFormat("Values('{0}',{1},{2},'{3}')") destination.City, destination.LenLat, destination.LenLon, destination.Code
        prefix = "INSERT INTO ChangesS400 " + "(EmployeeNum,[Name],[Date],[File],[Libary],[Subject],[Description],AddFilePath,Approved) ";
            command = prefix + sb.ToString();

        return command;
    }


 

    /// <summary>
    /// import from db format
    /// </summary>
    internal void importData()
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

        try
        {
            String cStr = $@"SELECT *
                        FROM Airport_2020"; ;
            cmd = CreateCommand(cStr, con);             // create the command
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                   //reader
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

    public void update()
    {
        da.Update(dt);
    }





}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ChangesControl.Models
{
    public class Changes
    {
        public int ID { get; set; }
        public int EmployeeNum { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string File { get; set; }
        public string Libary { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string AddedFile { get; set; }
        public bool Approved { get; set; }

        DBservices dBservices = new DBservices();
        public Changes()
        {

        }

        public DataTable GetFromDB()
        {
            DataTable dataTable = new DataTable();
            string qry = $@"SELECT *
                          FROM ChangesS400";
            dataTable = dBservices.GetDataTable(qry);
            return dataTable;
        }

        internal void PostToDB(Changes change)
        {
            string qry = dBservices.BuildInsertCommand(change);
            dBservices.insert(qry);
        
        }
    }
}
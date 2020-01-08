﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ChangesControl.Models
{
    public class Changes
    {
        public int Id { get; set; }
        public string NameInitated { get; set; }
        public string Programmer { get; set; }
        public int ProgrammerNum { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public bool ApprovedIt { get; set; }
        public bool AprrovedInitated { get; set; }
        public bool FinanceApproveNecessary { get; set; }
        public bool ApprovedFinance { get; set; }
        public List<Objects> ObjectsList { get; set; }
        public List<Tests> TestsList { get; set; }
        DBservices dBservices = new DBservices();

        public Changes()
        {

        }

        public DataTable GetFromDB()
        {
            DataTable dataTable = new DataTable();
            string qry = $@"SELECT *
                          FROM ChangessS400
                          order by Id desc";
            dataTable = dBservices.GetDataTable(qry);
            return dataTable;
        }

        internal void PostToDB(Changes change)
        {
            string itemQry = "";
            try
            {
                List<string> qryList = new List<string>();
                //insert to master data 
                string qry = dBservices.BuildInsertChanges(change);
                qryList.Add(qry);
                for (int i = 0; i < change.ObjectsList.Count; i++)
                {
                    qry = dBservices.BuildInsertObjectsAffected(change.ObjectsList[i], Id);
                    qryList.Add(qry);
                }
                for (int i = 0; i < change.TestsList.Count; i++)
                {
                    qry = dBservices.BuildInsertObjectsAffected(change.TestsList[i], Id);
                    qryList.Add(qry);
                }
                foreach (var item in qryList)
                {
                    dBservices.insert(item);
                    itemQry = item;
                }
            }
            catch(Exception ex)
            {
                LogWaveClass.LogWave("שגיאת הכנסה לדטהבייס " +itemQry+" "+ ex.Message);
            }



        }
    }
}
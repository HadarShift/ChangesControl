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

        /// <summary>
        /// save the new change record
        /// </summary>
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
                    qry = dBservices.BuildInsertObjectsAffected(change.ObjectsList[i], change.Id);
                    qryList.Add(qry);
                }
                for (int i = 0; i < change.TestsList.Count; i++)
                {
                    qry = dBservices.BuildInsertObjectsAffected(change.TestsList[i], change.Id);
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

        /// <summary>
        /// update an existing record in db
        /// </summary>
        internal void UpdateChangeRecord()
        {
            DataTable dataTable = new DataTable();
            string qry = $@"SELECT *
                          FROM ChangessS400
                          WHERE Id={Id}";
            dataTable = dBservices.GetDataTable(qry);
            if (dataTable.Rows.Count != 0)
            {
                dataTable.Rows[0]["NameInitated"] = NameInitated;
                dataTable.Rows[0]["Programmer"] = Programmer;
                dataTable.Rows[0]["Date"] = Date;
                dataTable.Rows[0]["Subject"] = Subject;
                dataTable.Rows[0]["Description"] = Description;
                dataTable.Rows[0]["FinanceApproveNecessary"] = FinanceApproveNecessary;
                //all users need to sign again
                dataTable.Rows[0]["It"] = false;
                dataTable.Rows[0]["Initated"] = false;
                dataTable.Rows[0]["Finance"] = false;
                dBservices.update(dataTable);
            }

            //update list of objects was affected by this change
            qry = $@"SELECT *
                       FROM ChangesObject
                       WHERE Id={Id}";
            dataTable = dBservices.GetDataTable(qry);
            for (int i = 0; i < ObjectsList.Count; i++)
            {
                if (i < dataTable.Rows.Count)
                {
                    dataTable.Rows[i]["Libary"] = ObjectsList[i].Libary;
                    dataTable.Rows[i]["ObjectFile"] = ObjectsList[i].Object;
                    dataTable.Rows[i]["Comments"] = ObjectsList[i].Comments;
                }
                else
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["Id"] = Id;
                    dataRow["Libary"] = ObjectsList[i].Libary;
                    dataRow["ObjectFile"] = ObjectsList[i].Object;
                    dataRow["Comments"] = ObjectsList[i].Comments;
                    dataTable.Rows.Add(dataRow);

                }

            }
            dBservices.update(dataTable);


            //update list of tests was affected by this change
            qry = $@"SELECT *
                       FROM ChangesTest
                       WHERE Id={Id}";
            dataTable = dBservices.GetDataTable(qry);
            for (int i = 0; i < TestsList.Count; i++)
            {
                if(i< dataTable.Rows.Count)
                {
                    dataTable.Rows[i]["Id"] = Id;
                    dataTable.Rows[i]["Descriptionn"] = TestsList[i].Description;
                    dataTable.Rows[i]["AprrovedInitated"] = false;//initate needs to sign again no matter what
                    dataTable.Rows[i]["AprrovedProgrammer"] = TestsList[i].ApprovedProgrammer;
                }
                else
                {
                    DataRow dataRow = dataTable.NewRow();
                    dataRow["Id"] = Id;
                    dataRow["Descriptionn"] = TestsList[i].Description;
                    dataRow["AprrovedInitated"] = false;
                    dataRow["AprrovedProgrammer"] = TestsList[i].ApprovedProgrammer;
                    dataTable.Rows.Add(dataRow);
                }
      
            }
            dBservices.update(dataTable);

        }


        /// <summary>
        /// get the change details after click on edit/view
        /// </summary>
        internal void GetChangeDetails(string idChange)
        {
             Id = int.Parse(idChange);
            string qry = $@"SELECT *
                           FROM ChangessS400
                           WHERE Id={Id}";
            DataTable dataTable = new DataTable();
            dataTable=dBservices.GetDataTable(qry);
            if (dataTable.Rows.Count != 0)
            {
                NameInitated = dataTable.Rows[0]["NameInitated"].ToString();
                Programmer= dataTable.Rows[0]["Programmer"].ToString();
                ProgrammerNum=Convert.ToInt32( dataTable.Rows[0]["ProgrammerNum"]);
                Date = DateTime.Parse(dataTable.Rows[0]["Date"].ToString()).ToString("yyyy-MM-dd");
                Subject= dataTable.Rows[0]["Subject"].ToString();
                Description= dataTable.Rows[0]["Description"].ToString();
                FinanceApproveNecessary = bool.Parse(dataTable.Rows[0]["FinanceApproveNecessary"].ToString());

                //import list of objects was affected by this change
                qry = $@"SELECT *
                       FROM ChangesObject
                       WHERE Id={Id}";
                dataTable = dBservices.GetDataTable(qry);
                ObjectsList = new List<Objects>();
                Objects obj = new Objects();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    obj.Libary= dataTable.Rows[i]["Libary"].ToString();
                    obj.Object= dataTable.Rows[i]["ObjectFile"].ToString();
                    obj.Comments= dataTable.Rows[i]["Comments"].ToString();
                    ObjectsList.Add(obj);
                    obj = new Objects();
                }
                //import list of tests was affected by this change
                qry = $@"SELECT *
                       FROM ChangesTest
                       WHERE Id={Id}";
                dataTable = dBservices.GetDataTable(qry);
                TestsList = new List<Tests>();
                Tests testObj = new Tests();
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    testObj.Description = dataTable.Rows[i]["Descriptionn"].ToString();
                    testObj.ApprovedInitated = bool.Parse(dataTable.Rows[i]["AprrovedInitated"].ToString());
                    testObj.ApprovedProgrammer = bool.Parse(dataTable.Rows[i]["AprrovedProgrammer"].ToString());
                    TestsList.Add(testObj);
                    testObj = new Tests();
                }
            }

        }


        /// <summary>
        /// check if current user can approve the change
        /// </summary>
        internal bool CheckifCanApprove(string fullNameUser, string userName, string typeApprove, string idChange)
        {
            bool check = false;
            Id = int.Parse(idChange);
            try
            {
                string qry = "";
                switch (typeApprove)//which type of button was clicked
                {
                    case "Initated":
                        qry = $@"SELECT *
                             FROM ChangessS400
                             WHERE NameInitated='{fullNameUser}' and Id={Id}";
                        break;

                    case "It":
                        qry = $@"SELECT *
                            FROM ChangesPremission
                            WHERE UserName='{userName}' and It = 'TRUE'";
                        break;

                    case "Finance":
                        qry = $@"SELECT *
                            FROM ChangesPremission
                            WHERE UserName='{userName}' and Finance = 'TRUE'";
                        break;
                }
                DataTable dataTable = new DataTable();
                dataTable = dBservices.GetDataTable(qry);
                if (dataTable.Rows.Count >= 1)//can sign
                {
                    //update approve field to true
                    qry = $@"SELECT *
                           FROM ChangessS400
                           WHERE Id={Id}";
                    dataTable = dBservices.GetDataTable(qry);
                    dataTable.Rows[0][typeApprove] = true;
                    dBservices.update(dataTable);
                    check = true;

                }
                else
                    check = false;
                            
            }

            catch(Exception ex)
            {
                LogWaveClass.LogWave(ex.Message);
            }
            return check;

        }

    }
}
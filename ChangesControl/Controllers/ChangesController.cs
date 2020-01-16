using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ChangesControl.Models;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;


namespace ChangesControl.Controllers
{
    public class ChangesController : ApiController
    {
        User UserDetails = new User();
        Changes changes = new Changes();

        [HttpPost]
        [Route("api/Changes/User")]
        public User CheckUser(User UserDetails)
        {
            this.UserDetails = UserDetails;
            this.UserDetails.IsValidUser();
            return UserDetails;
        }

        /// <summary>
        /// check if user can sign on record change
        /// </summary>
        [HttpGet]
        [Route("api/Changes/User/{fullNameUser}/{UserName}/{TypeApprove}/{idChange}")]
        public bool CheckUser(string fullNameUser, string userName,string TypeApprove ,string idChange )
        {
            string[] id = idChange.Split('-');
            idChange = id[1];
            bool check =false;
            check=changes.CheckifCanApprove(fullNameUser,userName,TypeApprove,idChange);
            return check;
        }

        [HttpGet]
        [Route("api/Changes/PermissionsUsers")]
        public List<User> GetPermissionsUsers()
        {
            return UserDetails.GetPermissionsUsers();
        }


        // GET api/values
        public DataTable Get()
        {         
            DataTable dataTable = new DataTable();
            dataTable=changes.GetFromDB();
            return dataTable;
        }

        /// <summary>
        /// get the change details after click on edit/view
        /// </summary>
        [HttpGet]
        [Route("api/Changes/getChange/{idChange}")]
        public Changes GetChangeDetails(string idChange)
        {     
            Changes changeDetails= new Changes();
            changeDetails.GetChangeDetails(idChange);
            return changeDetails;
        }

        // POST api/values
        public string Post([FromBody]Changes change)
        {
            try
            {
                string UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                LogWaveClass.LogWave("user: " + UserName);
                changes.PostToDB(change);
                return "Success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
    
        }

        // PUT api/values/5
        public string Put([FromBody]Changes changeToUpdate)
        {
            try
            {
                changeToUpdate.UpdateChangeRecord();
                return "Success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

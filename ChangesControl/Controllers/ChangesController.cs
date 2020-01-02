﻿using System;
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
        [HttpPost]
        [Route("api/Changes/User")]
        public User CheckUser(User UserDetails)
        {
            UserDetails.IsValidUser();
            return UserDetails;
        }

        Changes changes = new Changes();        
        // GET api/values
        public DataTable Get()
        {
          
            DataTable dataTable = new DataTable();
            dataTable=changes.GetFromDB();
            return dataTable;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
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
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}

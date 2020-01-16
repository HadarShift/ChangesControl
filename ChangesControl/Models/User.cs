using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ChangesControl.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public bool IsValid { get; set; }
        public bool PremissionEdit { get; set; }
        public bool PremissionIT { get; set; }
        public bool PremissionFinance { get; set; }

        public List<string> ListUsersNames { get; set; }//list of all users full names
        public User()
        {

        }
        /// <summary>
        /// check user authentication  
        /// </summary>
        public void IsValidUser()
        {
            try
            {
                List<UserPrincipal> ListUserPrincipal = new List<UserPrincipal>();
                ListUsersNames = new List<string>();
                string Domain = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().ToString();

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, Domain))
                {
                    // validate the credentials
                    IsValid = pc.ValidateCredentials(UserName, Password);
                    var usr = UserPrincipal.FindByIdentity(pc, UserName);
                    if (usr != null)
                        FullName = usr.DisplayName;
                    Password = "";

                    //get all names from active directory
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(pc)))
                    {
                        ListUserPrincipal = searcher.FindAll().Select(u => (UserPrincipal)u).ToList();
                        ListUsersNames = ListUserPrincipal.Select(x => x.DisplayName).Distinct().ToList();
                    }

                }
            }
            catch(Exception ex)
            {
                LogWaveClass.LogWave("IsValidUser -" + ex.Message);
            }
     

        }

        /// <summary>
        /// get all Permissions of users 
        /// </summary>
        internal List<User> GetPermissionsUsers()
        {
            DBservices dBservices = new DBservices();
            return dBservices.importDataChangesPremission();

        }
    }
}
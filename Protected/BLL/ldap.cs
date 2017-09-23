using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace TheBox.Protected.BLL
{
    public class ldap
    {
        public string Name { get; set; }

        public ldap(string name)
        {
            Name = name;
        }
        public static List<ldap> getAllUsers()
        {
            using (DirectoryEntry de = new DirectoryEntry("LDAP://192.168.1.19"))
            {
                using (DirectorySearcher adSearch = new DirectorySearcher(de))
                {
                    adSearch.Filter = "(sAMAccountName=*)";
                    var adSearchResult = adSearch.FindAll();
                    List<ldap> result = new List<ldap>();
                    foreach (SearchResult r in adSearchResult)
                    {
                        result.Add(new ldap(r.Properties["cn"][0].ToString()));
                    }
                    return result;
                }
            }
        }

        public static string[] getListOfUsers()
        {
            using (DirectoryEntry de = new DirectoryEntry("LDAP://192.168.1.19"))
            {
                using (DirectorySearcher adSearch = new DirectorySearcher(de))
                {
                    adSearch.Filter = "(sAMAccountName=*)";
                    var adSearchResult = adSearch.FindAll();
                    string[] result = new string[adSearchResult.Count];
                    for (int i = 0; i < adSearchResult.Count; i++)
                    {
                        result[i] = adSearchResult[i].Properties["cn"][0].ToString();
                    }
                    return result;
                }
            }
        }

        public static string[] getListOfUsersTemp()
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.UserProfiles
                             select o.UserName).ToArray();
                return result;

            }
        }
    }
}
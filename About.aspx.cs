using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.DirectoryServices;
using System.Data;
using System.IO;
using TheBox.Protected.BLL;


namespace TheBox
{
    public partial class About : System.Web.UI.Page
    {
        public static int counter;

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetTeams(string name)
        {

            return "WOoOoOoOoW I got it from server side. Hello " + name;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var result = ldap.getAllUsers();
            //string path = "http://" + Request.Url.Authority + @"/Test/An experimental system for measuring the credibility of news content in Twitter.pdf";
            //string path1 = "http://" + Request.Url.Authority + @"/Default.aspx";
            //string path2 = @"file://///192.168.1.19/share/MRI/MRI/HCM brochure NF.PDF";
            //docPreview.Attributes["src"] = path2;
            //DirectoryInfo directoryInfo = new DirectoryInfo(@"\\192.168.1.19\share");

            //// Get the Excel Files

            //FileInfo[] fileinfo = directoryInfo.GetFiles();
            //Directory.Delete(@"\\192.168.1.19\share\temp");
            if (!Page.IsPostBack)
            {
                counter = 0;
            }
            //// get ldap info
            //string username = "Marwa";
            //string password = "~Aa04170584";
            //string ldapAddress = "LDAP://192.168.1.19";
            //DirectoryEntry de = new DirectoryEntry(ldapAddress, username, password);

            //DirectorySearcher ds = new DirectorySearcher(de);
            //ds.Filter = "(&(objectCategory=person)(objectClass=user)(!userAccountControl:1.2.840.113556.1.4.803:=2)(sn=marw*))";
            //ds.SearchScope = SearchScope.Subtree;
            //SearchResultCollection rs = ds.FindAll();


            //DataTable dt = new DataTable();
            //DataRow dr = null;
            //dt.Columns.Add(new DataColumn("username", typeof(string)));
            //dt.Columns.Add(new DataColumn("firstname", typeof(string)));
            //dt.Columns.Add(new DataColumn("lastname", typeof(string)));
            //dt.Columns.Add(new DataColumn("email", typeof(string)));
            //foreach (SearchResult r in rs)
            //{
            //    dr = dt.NewRow();

            //    dt.Rows.Add(dr);
            //    if (r.GetDirectoryEntry().Properties["samaccountname"].Value != null)
            //    {
            //        dr["username"] = r.GetDirectoryEntry().Properties["samaccountname"].Value.ToString();
            //    }
            //    if (r.GetDirectoryEntry().Properties["givenName"].Value != null)
            //        dr["firstname"] = r.GetDirectoryEntry().Properties["givenName"].Value.ToString();
            //    if (r.GetDirectoryEntry().Properties["sn"].Value != null)
            //        dr["lastname"] = r.GetDirectoryEntry().Properties["sn"].Value.ToString();
            //    if (r.GetDirectoryEntry().Properties["mail"].Value != null)
            //        dr["email"] = r.GetDirectoryEntry().Properties["mail"].Value.ToString();
            //}

            //GridView1.DataSource = dt;
            //GridView1.DataBind();
   
        }
    }
}

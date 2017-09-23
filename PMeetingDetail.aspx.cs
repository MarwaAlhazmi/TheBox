using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using TheBox.Protected.BLL;
using System.Web.Services;
using System.Web.Script.Services;

namespace TheBox
{
    public partial class PMettingDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Request["id"] != null)
            //{
            //   // get the meeting info based on the id
            //    int id = Convert.ToInt32(Request["id"]);
            //    MeetingClass.MeetingInfo m = MeetingClass.MeetingInfo.GetInfo(id);
            //    // fill data
            //    tbtitle2.Value = m.Title;
            //    tbdate2.Value = m.Date.ToShortDateString();
            //    tbSTime.Value = m.StartTime.ToShortTimeString();
            //    tbETime.Value = m.EndTime.ToShortTimeString();
            //    tbIndo2.Value = m.Desc;     
            //}
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string updateInfo(int id, string title, DateTime date, DateTime stime, DateTime etime, string desc)
        {
            //MeetingClass.MeetingInfo info = new MeetingClass.MeetingInfo(id, title, date, stime, etime, desc);
            //return info.Update().ToString();
            return "";
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void updateInfo()
        {
            //MeetingClass.MeetingInfo nn = new MeetingClass.MeetingInfo(1, "POS", DateTime.Now.Date, DateTime.Now, DateTime.Now, "this is a meeting");
            //return nn;
            
        }
    }
}
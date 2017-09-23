using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;
using System.Web.Services;
using System.Web.Script.Services;

namespace TheBox
{
    public partial class PCalender : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ddlType.DataSource = MeetingClass.getMeetingTypes();
            ddlType.DataTextField = "Type";
            ddlType.DataValueField = "CatID";
            ddlType.DataBind();
            if (Session["cal"] != null)
            {
                if (Convert.ToBoolean(Session["cal"]))
                cbCal.Checked = true;
            }
            if (Session["meet"] != null)
            {
                if (Convert.ToBoolean(Session["meet"]))
                cbMeet.Checked = true;
            }
            if (Session["app"] != null)
            {
                if (Convert.ToBoolean(Session["app"]))
                cbApp.Checked = true;
            }
            
        }
      
       
        [WebMethod]
        [ScriptMethod]
        //[System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
        public static string[] GetCompletionList(string prefixText, int count, string contextKey)
        {
            // Create array of users  
            string[] users = ldap.getListOfUsersTemp();
            
            // Return matching movies  
            return (from m in users where m.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase) select m).Take(count).ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveCal(DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string etitle, bool aday)
        {
            string username = HttpContext.Current.User.Identity.Name; 
            var result = MeetingClass.MeetingInfo.InsertCalender(sdate,edate, stime, etime, etitle, username, aday);
            return result.ToString();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveMeet(DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string etitle, string desc, int mtype, bool aday)
        {
            string username = HttpContext.Current.User.Identity.Name;
            var result = MeetingClass.MeetingInfo.InsertMeeting(sdate, edate, stime, etime, etitle, username, mtype, desc, aday);
            return result.ToString();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveApp(DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string etitle, string pinv, bool aday, string desc)
        {
            string username = HttpContext.Current.User.Identity.Name;
            var result = MeetingClass.MeetingInfo.InsertAppointment(sdate, edate, stime, etime, etitle, username, pinv, aday, desc);
            return result.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<Meeting> FilterEvents(bool Cal, bool App, bool Meet)
        {
            List<Meeting> result = new List<Meeting>();
            string username = HttpContext.Current.User.Identity.Name;
            if (Cal)
            {
                result.AddRange(MeetingClass.MeetingInfo.GetCalender(username));
            }
            if (App)
            {
                result.AddRange(MeetingClass.MeetingInfo.GetAppointments(username));
            }
            if (Meet)
            {
                result.AddRange(MeetingClass.MeetingInfo.GetMeetings(username));
            }

            string finalR = MeetingClass.MeetingInfo.formatEvents(result);
            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<Meeting> GetEvents()
        {
            string username = HttpContext.Current.User.Identity.Name;
            var result = MeetingClass.MeetingInfo.GetAllEvents(username);
            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static Protected.BLL.Event GetEvent(int id)
        { 
            Protected.BLL.Event e = MeetingClass.MeetingInfo.GetInfo(id);
            return e;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string Delete(int id)
        {
            string r = MeetingClass.MeetingInfo.Delete(id).ToString();
            return r;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateCalendar(int id,DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string etitle, bool aday)
        {
            DateTime start = new DateTime(sdate.Year, sdate.Month, sdate.Day, stime.Hour, stime.Minute, stime.Second);
            DateTime end = new DateTime(edate.Year, edate.Month, edate.Day, etime.Hour, etime.Minute, etime.Second);
            Protected.BLL.Event ev = new Protected.BLL.Event(id,etitle,start,end,aday,"Calendar");
            var result = MeetingClass.MeetingInfo.UpdateCalendar(ev);
            return result.ToString();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateMeeting(int id, DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string etitle, string desc, int mtype, bool aday)
        {
            DateTime start = new DateTime(sdate.Year, sdate.Month, sdate.Day, stime.Hour, stime.Minute, stime.Second);
            DateTime end = new DateTime(edate.Year, edate.Month, edate.Day, etime.Hour, etime.Minute, etime.Second);
            Protected.BLL.Event ev = new Protected.BLL.Event(id, etitle, start, end,mtype,"Meeting",desc, aday);
            string username = HttpContext.Current.User.Identity.Name;
            var result = Watcher.MeetingWatcher.UpdateInfo(ev, username);
            return result.ToString();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateAppointment(int id, DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string etitle, string pinv, bool aday, string desc)
        {
            DateTime start = new DateTime(sdate.Year, sdate.Month, sdate.Day, stime.Hour, stime.Minute, stime.Second);
            DateTime end = new DateTime(edate.Year, edate.Month, edate.Day, etime.Hour, etime.Minute, etime.Second);
            Protected.BLL.Event ev = new Protected.BLL.Event(id, etitle, start, end, aday, "Appointment", desc, pinv);
            var result = MeetingClass.MeetingInfo.UpdateMeeting(ev);
            return result.ToString();
        }
    }
}
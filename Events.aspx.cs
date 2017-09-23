using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using TheBox.Protected.BLL;
using System.Web.Script.Services;

namespace TheBox
{
    public partial class Events : System.Web.UI.Page
    {
        //public List<Meeting> filteredResult = new List<Meeting>();
        /* Only purpose is to write JSON string to browser so that FullCalendar can pick it up
        ---------------------------------------------------------------------------------------------------*/
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Meeting> filteredResult = new List<Meeting>();
            string username = HttpContext.Current.User.Identity.Name;

            if (Session["cal"] == null && Session["meet"] == null && Session["app"] == null)
            {
                filteredResult = MeetingClass.MeetingInfo.GetAllEvents(username);
            }
            else
            {
                bool cal = Convert.ToBoolean(Session["cal"]);
                bool meet = Convert.ToBoolean(Session["meet"]);
                bool app = Convert.ToBoolean(Session["app"]);
                if (cal)
                {
                    filteredResult.AddRange(MeetingClass.MeetingInfo.GetCalender(username));
                }
                if (app)
                {
                    filteredResult.AddRange(MeetingClass.MeetingInfo.GetAppointments(username));
                }
                if (meet)
                {
                    filteredResult.AddRange(MeetingClass.MeetingInfo.GetMeetings(username));
                }
            }
            string finalR = MeetingClass.MeetingInfo.formatEvents(filteredResult);
            HttpContext.Current.Response.Write(finalR);
           
            //if (filter == MeetingFilter.Appointment)
            //{
            //    var result = MeetingClass.MeetingInfo.GetAppointments(User.Identity.Name);
            //    HttpContext.Current.Response.Write(MeetingClass.MeetingInfo.formatEvents(result));
            //}
            //if (filter == MeetingFilter.Calendar)
            //{
            //    var result = MeetingClass.MeetingInfo.GetCalender(User.Identity.Name);
            //    HttpContext.Current.Response.Write(MeetingClass.MeetingInfo.formatEvents(result));
            //}
            //if (filter == MeetingFilter.Meeting)
            //{
            //    var result = MeetingClass.MeetingInfo.GetMeetings(User.Identity.Name);
            //    HttpContext.Current.Response.Write(MeetingClass.MeetingInfo.formatEvents(result));
            //}
            //if (filter == MeetingFilter.All)
            //{
            //    var result = MeetingClass.MeetingInfo.GetAllEvents(User.Identity.Name);
            //    HttpContext.Current.Response.Write(MeetingClass.MeetingInfo.formatEvents(result));
            //}          
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string FilterEvents(bool Cal, bool App, bool Meet)
        {
            HttpContext.Current.Session["cal"] = Cal;
            HttpContext.Current.Session["meet"] = Meet;
            HttpContext.Current.Session["app"] = App;
            //var filteredResult = new List<Meeting>();
            //string username = HttpContext.Current.User.Identity.Name;
            //if (Cal)
            //{
            //    filteredResult.AddRange(MeetingClass.MeetingInfo.GetCalender(username));
            //}
            //if (App)
            //{
            //    filteredResult.AddRange(MeetingClass.MeetingInfo.GetAppointments(username));
            //}
            //if (Meet)
            //{
            //    filteredResult.AddRange(MeetingClass.MeetingInfo.GetMeetings(username));
            //}

            //string finalR = MeetingClass.MeetingInfo.formatEvents(filteredResult);
            //HttpContext.Current.Response.Write(finalR);
            return "true";
        }

     
    }   
}
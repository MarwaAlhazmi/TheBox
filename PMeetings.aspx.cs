using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;
using System.Web.Services;
using System.Web.Script.Services;
using System.Text;

namespace TheBox
{
    public partial class PMeetings : System.Web.UI.Page
    {
        const string accordionheader = @"<div class=""accordion-group""><div style=""background-color: #f5f5f5"" class=""accordion-heading""><a class=""accordion-toggle"" data-toggle=""collapse"" data-parent=""#accordion2"" href=""#collapse";
        static int id = 1;
        const string table = @"<table><tr><td style=""width:700px"">{0}</td><td style=""width:100px"">{1}</td><td>{2}</td><td>{3}</td></tr></table>";
        const string span = @"<span class=""caret""></span>";
        const string accordionbody1 = @"</a></div><div id=""collapse";
        const string accordionbody2 = @"class=""accordion-body collapse""><div class=""accordion-inner"">";
        const string accordionbody3 = "</div></div></div>";
        private static string username;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                username = User.Identity.Name;
                // get the list of upcoming meetings
                var result = MeetingClass.getUpcomingMeetings(User.Identity.Name);
                BuildTags(result, accordion2);
                var month = MeetingClass.getMonthMeetings(User.Identity.Name);
                BuildTags(month, accordionMonth);

                // get a list of years and months and add tags
                BuildHistory();
            }

        }

        private void BuildHistory()
        {
         //   string year = "year";
         //   string month = "month";
         //   string str = @"<div class=""accordion-group""><div class=""accordion-heading"">";
         //   string str2 = @"<a class=""accordion-toggle"" data-toggle=""collapse"" data-parent=""#accordionHistory"" href=""#collapse" + year + @""">" + year + " </a>";
         //string str3 = @"</div><div id=""collapse"+year+@""" class=""accordion-body collapse""><div class=""accordion-inner"">";
         //string monthacc= @"<div class=""accordion"" id="""+year+@""">"; // startign the months accordion
         //string str33 = @"<div class=""accordion-group""><div class=""accordion-heading""><a class=""accordion-toggle"" data-toggle=""collapse"" data-parent=""#"+year+@""" href=""#"+year+month+@""" onclick="">";
         //string str4 = month + @"</a></div><div id="""+year + month+@""" class=""accordion-body collapse""><div class=""accordion-inner"">information of the inner item";
         //string endmonth = @"</div></div></div></div>";
         //   string endyear = "</div></div></div>";
            // get months and years
            var result = MeetingClass.getYearMonth();
            StringBuilder text = new StringBuilder();
            foreach (var r in result)
            {
                text.Append( @"<div class=""accordion-group""><div class=""accordion-heading"">"+
                 @"<a class=""accordion-toggle"" data-toggle=""collapse"" data-parent=""#accordionHistory"" href=""#collapse" + r.Key + @""">" + r.Key + " </a>" +
                 @"</div><div id=""collapse" + r.Key + @""" class=""accordion-body collapse""><div class=""accordion-inner"">" +
                 @"<div class=""accordion"" id=""" + r.Key + @"""><div class=""accordion-group"">");
                foreach (var t in r.Value)
                {
                    text.Append(@"<div class=""accordion-heading""><a class=""accordion-toggle"" data-toggle=""collapse"" data-parent=""#" + r.Key + @""" href=""#" + r.Key + t.ToString() + @""" onclick=""test(this)"">" +
                     t.ToString() + @"</a></div><div id=""" + r.Key + t.ToString() + @""" class=""accordion-body collapse""><div class=""accordion-inner"">information of the inner item" +
                     @"</div></div>");
                }
                text.Append("</div></div></div></div></div>");
            }
            accordionHistory.InnerHtml = text.ToString();
        }

        public static void BuildTags(List<Meeting> result, System.Web.UI.HtmlControls.HtmlGenericControl accordion)
        {
            if (result.Count() == 0)
            {
                accordion.InnerHtml += "<h4>No upcoming meetings</h4>";
            }
            else
            {
                foreach (var meet in result)
                {
                    accordion.InnerHtml += accordionheader + id.ToString() + @""">";// +meet.MeetingTitle + " On " + meet.StartDate.ToString("dd/MM/yyyy") + " | " + meet.StartDate.ToShortTimeString();
                    accordion.InnerHtml += string.Format(table, meet.MeetingTitle, meet.StartDate.ToShortTimeString(), meet.StartDate.ToString("MM/dd/yyyy"), span);
                    accordion.InnerHtml += accordionbody1;
                    accordion.InnerHtml += id.ToString() + @"""" + accordionbody2;
                    // get participants info
                    //var chairman = MeetingClass.Participant.getMeetingParticipant(meet.MeetingID, "Chairman");
                    var creator = MeetingClass.CParticipant.getMeetingParticipant(meet.MeetingID, "Creator");
                    // insert icons then line
                    accordion.InnerHtml += BuildAgenda(meet.MeetingID.ToString());
                    accordion.InnerHtml += BuildFiles(meet.MeetingID.ToString());
                    accordion.InnerHtml += BuildParticipants(meet.MeetingID.ToString());
                    accordion.InnerHtml += BuildDiscussion(meet.MeetingID.ToString());
                    //accordion.InnerHtml += "<hr />";
                    //accordion.InnerHtml += "<b>Chairman: </b>" + chairman.FirstName + " " + chairman.LastName + "<br />";
                    accordion.InnerHtml += "<b>Creator: </b>" + creator.FirstName + " " + creator.LastName + "<br />";
                    accordion.InnerHtml += "<b>Start Time: </b>" + meet.StartDate.ToShortTimeString() + "<br />";
                    accordion.InnerHtml += "<b>End Time: </b>" + meet.EndDate.ToShortTimeString() + "<br />";
                    accordion.InnerHtml += "<b>Description: </b>" + meet.MeetingDesc + "<br />";

                    //accordion2.InnerHtml += "<b>Chairman: </b><br />";
                    //accordion2.InnerHtml += "<b>Creator: </b><br />";
                    //accordion2.InnerHtml += "<b>Start Time: </b><br />";
                    //accordion2.InnerHtml += "<b>End Time: </b><br />";
                    //accordion2.InnerHtml += "<b>Description: </b><br />";

                    accordion.InnerHtml += accordionbody3;
                    id++;
                }
            }
        }

        private static string BuildDiscussion(string meetingID)
        {
            string tag1 = @"<a title=""Discussion"" class=""win-commandicon pull-right"" href=""";
            tag1 += "PMeetingSetUp.aspx?tab=discussion&id="+meetingID;
            string tag2 = @"""><span class=""win-commandicon icon-chat-2""></span></a>";

            return tag1 + tag2;
        }

        private static string BuildAgenda(string meetingID)
        {
            string tag1 = @"<a title=""Agenda"" class=""win-commandicon pull-right"" href=""";
            tag1 += "PMeetingSetUp.aspx?tab=agenda&id=" + meetingID;
            string tag2 = @"""><span class=""win-commandicon icon-clipboard""></span></a>";

            return tag1 + tag2;
        }
        private static string BuildParticipants(string meetingID)
        {

            string tag1 = @"<a title=""Participants"" class=""win-commandicon pull-right"" href=""";
            tag1 += "PMeetingSetUp.aspx?tab=participants&id=" + meetingID;
            string tag2 = @"""><span class=""win-commandicon icon-users""></span></a>";

            return tag1 + tag2;
        }
        private static string BuildFiles(string meetingID)
        {
            string tag1 = @"<a title=""Files"" class=""win-commandicon pull-right"" href=""";
            tag1 += "PMeetingSetUp.aspx?tab=files&id=" + meetingID;
            string tag2 = @"""><span class=""win-commandicon icon-copy""></span></a>";

            return tag1 + tag2;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string getInfo(string href)
        {
            // get the year and month
            int year = Convert.ToInt32(href.Substring(0, 4));
            int y;
            if (href.Length == 6)
            {
                y = 2;
            }
            else
            {
                y = 1;
            }
            int month = Convert.ToInt32(href.Substring(4,y));
            // get from db
            var result = MeetingClass.getHistoryMeetings(username, month, year);
            // send back a text
            System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl();
            BuildTags(result, div);
            return div.InnerHtml;
        }
    }
}
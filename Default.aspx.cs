using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;

namespace TheBox
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string username = User.Identity.Name;
            // notification
            int num = Notification.GetTotalNumberOfNotifications(username);
            string msg = "";
            if (num == 0)
            {
                msg = "No new updates available";
            }
            else
            {
                msg = "You have "+num+" new Updates";
            }
            NotDiv.InnerText = string.Format(msg,num);


            // meetings
            var mnum = MeetingClass.GetNumberOfUpcomingMeetings(username);
            string mstr = "";
            if (mnum == 0)
            {
                mstr = "No meetings to attend this month";
            }
            else
            {
                mstr = "You have "+mnum+" meetings to attend this month";
            }
            meetingDiv.InnerText = mstr;
        }
    }
}
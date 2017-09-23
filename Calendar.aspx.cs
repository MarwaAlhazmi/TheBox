using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;
using System.Web.UI.HtmlControls;
using System.Drawing;


namespace TheBox
{
    public partial class Calendar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void calTraining_DayRender(object sender, DayRenderEventArgs e)
        {
           // // first delete the controls and add thema manually
           // e.Cell.Controls.Clear();
           // Panel asppanel = new Panel();
           // asppanel.Width = Unit.Pixel(150);
           // asppanel.Height = Unit.Pixel(150);
           // asppanel.ScrollBars = ScrollBars.Auto;
           // HtmlAnchor day = new HtmlAnchor();
           // day.InnerText = e.Day.Date.Day.ToString();
           // day.ID = e.Day.Date.ToShortDateString();
           // day.Title=  "Click to set up a meeting";
           // day.Attributes.Add("onClick", "return myFunction(this)");
           // day.Attributes.Add("href", "#");
            
           // e.Cell.Controls.Add(day);
           // e.Cell.HorizontalAlign = HorizontalAlign.Center;
           // e.Cell.VerticalAlign = VerticalAlign.Middle;
           //// e.Cell.Height = Unit.Pixel(150);
           //// e.Cell.Width = Unit.Pixel(150);
           //// e.Cell.Style.Add("overflow", "auto");
           // // get the meetings then change the text and background color
           // var meeting = MeetingClass.getMeetingByDate(e.Day.Date, User.Identity.Name);
           // //e.Cell.Controls.Clear();
           // //control.Attributes.Add("onClick", "return myFunction()");


           // if (meeting.Count() > 0)
           // {
           //     e.Cell.Enabled = false;
           //     //e.Day.IsSelectable = false;
           //     e.Cell.HorizontalAlign = HorizontalAlign.Left;
           //     e.Cell.VerticalAlign = VerticalAlign.Top;
           //     e.Cell.BackColor = Color.Silver;
           //     //e.Cell.Style.Add("overflow", "auto");
           //     foreach (var meet in meeting)
           //     {
           //         string linkStr = @"<span style=""font-size:7pt; font-Weight:bold; background-color:Aqua""><br/><a href=""" + "PMeetingDetail.aspx?id=" + meet.MeetingID + @""">" + meet.MeetingTitle + " On: " + meet.StartTime.ToShortTimeString() + "</a></span>";
           //         //HtmlAnchor nn = new HtmlAnchor(); nn.Attributes.Add("href", "PMeetingDetail.aspx?id=" + meet.MeetingID);
           //         //nn.InnerText = meet.MeetingTitle + " On: " + meet.StartTime.ToShortTimeString();
           //         //asppanel.Controls.Add(new LiteralControl(linkStr));
           //         e.Cell.Controls.Add(new LiteralControl(linkStr));
           //     }

           //     //e.Cell.Controls.Add(asppanel);
           // }
            
            //if (e.Cell.Controls.Count > 0)
            //{
            //    var uu = e.Cell.Controls[0];
               
                
            //}
            //if (e.Day.Date == DateTime.Now.Date)
            //{
            //    e.Cell.Enabled = false;
            //    e.Day.IsSelectable = false;
            //    e.Cell.HorizontalAlign = HorizontalAlign.Left;
            //    e.Cell.VerticalAlign = VerticalAlign.Top;
            //    string linkStr = @"<span style=""font-size:7pt; font-Weight:bold; color:red""><br/><a href=""#"">" + "this is a test" + "</a></span>";
            //    e.Cell.Controls.Add(new LiteralControl(linkStr));
            //    string linkStr1 = @"<span style=""font-size:7pt; font-Weight:bold; color:red""><br/><a href=""#"">" + "this is a test two" + "</a></span>";
            //    e.Cell.Controls.Add(new LiteralControl(linkStr1));
            //}
            //var meetings = MeetingClass.getMeetings(User.Identity.Name);
            //foreach (var m in meetings)
            //{

            //}
        }


        protected void calMeetings_DateChanged(object sender, EventArgs e)
        {
            
        }
    }
}
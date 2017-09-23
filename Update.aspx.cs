using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Security;
using TheBox.Protected.BLL;
using System.Net.Mail;
using System.Net;
using System.Security.Principal;
using System.Web.Script.Services;

namespace TheBox
{
    public partial class Update : System.Web.UI.Page
    {
        enum UpdateType
        {
            New,
            Update,
            Delete
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //var t = Watcher.Out.GetNumberOfNewNotifications(User.Identity.Name);
            //test.InnerText = t.ToString();

            var y = Watcher.Out.GetAllNotification(User.Identity.Name);

            foreach (var i in y)
            {
                string html;
                if (!i.seen)
                {
                    html = "<div onclick=\"clickLink('" + i.NotURL + "', "+i.ID+")\" style=\"background-color:#C4C4C4; cursor: pointer; padding-left:5px; padding-top:15px; padding-bottom:15px\"><a onclick=\"clickLink('" + i.NotURL + "', " + i.ID + ")\" href=\"#\" title= \"" + i.Type + "\">" + i.NotText + "</a></div><br />";
                }
                else
                {
                    html = "<div onclick=\"clickLink('" + i.NotURL + "', " + i.ID + ")\" style=\"padding-left:5px; cursor: pointer; padding-top:15px\" onclick=\"location.href='" + i.NotURL + "'\"><a onclick=\"clickLink('" + i.NotURL + "', " + i.ID + ")\"  href=\"#\" title= \"" + i.Type + "\">" + i.NotText + "</a></div><br />";
                }
                updates.InnerHtml += html;
            }

        }


        [WebMethod]
        public static void follow(string username)
        {
            using (boxEntities box = new boxEntities())
            {

            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //var t = WindowsIdentity.GetCurrent();
            //cre.InnerText += t.Name;
            //Uri uri = new Uri("http://mrisystem.com.my");
            //ICredentials credentials = CredentialCache.DefaultNetworkCredentials;
            //NetworkCredential credential = credentials.GetCredential(uri, "Basic");
            //cre.InnerText += credential.Domain;
            //cre.InnerText += credential.UserName;
            //cre.InnerText += credential.Password;
            MailAddress FromMailAddress = new MailAddress("marwa@google.com");
            MailAddress ToMailAddress = new MailAddress("marwa@mrisystem.com.my");
            MailMessage MailMsg = new MailMessage();
            MailMsg.From = FromMailAddress;
            MailMsg.To.Add(ToMailAddress);
            MailMsg.Subject = "Testing email from MRI";

            MailMsg.Body = @"
        <html>
            <body>
                <b>Heading</b><br>
                <br>
                Text<br>
                Again Some Text<br>
            </body>
        </html>";

            MailMsg.IsBodyHtml = true;

            SmtpClient MailSender = new SmtpClient("mrisystem.com.my");
            //MailSender.Credentials = new NetworkCredential("marwa@mrisystem.com.my", "marwa740");
            //MailSender.SendCompleted += (s, a) =>
            //{
            //    MailSender.Dispose();
            //    MailMsg.Dispose();
            //};
            MailSender.Send(MailMsg);
           
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void UpdateNotification(int NotiID)
        {
            var result = Notification.UpdateNotification(NotiID);
        }
    }
}
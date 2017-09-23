using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Reflection;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using TheBox.Protected.BLL;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace TheBox
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string[] emails = { "alhazmi_m@hotmail.com", "marwa_aou@yahoo.com", "ahazmi.marwa@gmail.com"};
            //string admin;
            //SmtpClient client = CreateSmtpClient(out admin);
            //foreach (var t in emails)
            //{
            //    SendEmail(admin, t, t, "test", "Test Sending notification", client);
            //}

            //Response.Write("done sending");

        }
        private static bool SendEmail(string from, string to, string toName, string subject, string msg, SmtpClient client)
        {
            try
            {
                MailAddress FromMailAddress = new MailAddress(from);
                MailAddress ToMailAddress = new MailAddress(to, toName);
                MailMessage MailMsg = new MailMessage();
                MailMsg.From = FromMailAddress;
                MailMsg.To.Add(ToMailAddress);
                MailMsg.Subject = subject;

                MailMsg.Body = string.Format(@"
                    <html>
                        <body>
                            {0}
                        </body>
                    </html>", msg);
                MailMsg.IsBodyHtml = true;
                //client.SendCompleted += (s, e) =>
                //{
                //    client.Dispose();
                //    MailMsg.Dispose();
                //};
                client.Send(MailMsg);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static SmtpClient CreateSmtpClient(out string adminu)
        {
            SmtpClient MailSender;
            adminu = ConfigurationManager.AppSettings["AdminU"];
            var adminp = ConfigurationManager.AppSettings["AdminP"];
            MailSender = new SmtpClient("219.93.39.190", 567); //219.93.39.190 port 567 // inside "192.168.1.190",567
            MailSender.Credentials = new NetworkCredential(adminu, adminp);
            MailSender.SendCompleted  += (s, e) => {
                MailSender.Dispose();
                        };
            return MailSender;
        }

        static void MailSender_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ((SmtpClient)sender).Dispose();
        }

       

        public class CalendarData
        {

            //[DataMember(Order = 1)]

            public List<ArrayList> events { get; set; }

            // [DataMember(Order = 2)]

            public bool issort { get; set; }

            //[DataMember(Order = 3)]

            public string start { get; set; }

            //[DataMember(Order = 4)]

            public string end { get; set; }

            //[DataMember(Order = 5)]

            public string error { get; set; }

        }

        // Use a data contract as illustrated in the sample below to add composite types to service operations.



        public static class JSDateTime
        {

            public static string ToStr(DateTime dateTime)
            {

                return dateTime.ToString(@"MM\/dd\/yyyy HH:mm");

            }

        }



        public class Event
        {

            public int Id { get; set; }

            public string Subject { get; set; }

            public string StartDate { get; set; }

            public string EndDate { get; set; }

            public int IsAllDayEvent { get; set; }

            public int IsMoreThanOneDayEvent { get; set; }

            public int RecurringEvent { get; set; }

            public int Color { get; set; }

            public int IsEditable { get; set; }

            public string Location { get; set; }

            public string Attendents { get; set; }

        }



        public static ArrayList ConvertToArrayList<T>(T obj)
        {

            ArrayList ls = new ArrayList();

            PropertyInfo[] p = obj.GetType().GetProperties();

            foreach (var item in p)
            {

                ls.Add(item.GetValue(obj, null));

            }

            return ls;

        }

        public static string[] GetCompletionList(string prefixText, int count, string contextKey)
        {
            // Create array of users  
            string[] users = ldap.getListOfUsersTemp();

            // Return matching movies  
            return (from m in users where m.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase) select m).Take(count).ToArray();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            var t = Request.Form["TextBox1"];
            
        }
    }
}
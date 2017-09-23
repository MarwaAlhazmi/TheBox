using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Net.Mail;
using System.Configuration;
using System.Net;

namespace TheBox.Protected.BLL
{
    public enum NotificationType
    {
        Meeting_Parti_Insert,
        Meeting_Info_Update,
        Meeting_New_Discussion,
        Meeting_Parti_Disinvite,
        Agenda_New_Discussion,
        New_Reply,
        Agenda_New,
        Agenda_Update,
        Task_Assignment
    }

    public enum NotType
    {
        Comment,
        Follow,
        Upload,
        Task
    }
    public class Notification
    {
        public class NotificationForm
        {
            public string NotText;
            public string NotURL;
            public bool seen;
            public string Type;
            public int ID;
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
                client.SendAsync(MailMsg, null);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        // formatting msg subject
        public static string GetSubject(NotificationType type)
        {
            string t = type.ToString();
            switch (t)
            {
                case "Meeting_Parti_Insert":
                    return "Meeting Invitation";
                case "Meeting_Info_Update":
                    return "Meeting Update";
                case "Meeting_New_Discussion":
                case "Agenda_New_Discussion":
                    return "New Discussion";
                case "New_Reply":
                    return "New Reply";
                case "Meeting_Parti_Disinvite":
                    return "Disinvite from a Meeting";
                case "Agenda_New":
                    return "New Agenda";
                case "Agenda_Update":
                    return "Agenda Update";
                case "Task_Assignment":
                    return "A new Task Assignment";
                default:
                    return "Nothing!";
            }
        }
        // formatting msg body
        private static string FormatMeetingMsg(Meeting m, string note, NotificationType type)
        {
            string i = type.ToString();
            string msg ;
            switch (i)
            {
                case "Meeting_Parti_Insert":
                    msg = "<b>You Have been invited to a new meeting</b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + m.MeetingTitle + "<br>";
                    msg += "Date: " + m.StartDate.ToString("MM/dd/yyyy") + "<br>";
                    msg += "Time: " + m.StartDate.ToString("hh:mm:ss") + " - " + m.EndDate.ToString("hh:mm:ss") + "<br>";
                    msg += "All Day Event: " + (m.AllDay ? "yes" : "No") + "<br>";
                    msg += "Type: " + m.MeetingType.Type + "<br><br>";
                    msg += "<b>Notes:</b><br>";
                    msg += note;
                    return msg;

                case "Meeting_Info_Update":
                    msg = "<b>Update has been done on Meeting Information</b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + m.MeetingTitle + "<br>";
                    msg += "Date: " + m.StartDate.ToString("MM/dd/yyyy") + "<br>";
                    msg += "Time: " + m.StartDate.ToString("hh:mm:ss") + " - " + m.EndDate.ToString("hh:mm:ss") + "<br>";
                    msg += "All Day Event: " + (m.AllDay ? "yes" : "No") + "<br>";
                    msg += "Type: " + m.MeetingType.Type + "<br><br>";
                    return msg;
                case "Meeting_Parti_Disinvite":
                    msg = "<b>You Have been disinvited from the meeting: </b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + m.MeetingTitle + "<br>";
                    msg += "Date: " + m.StartDate.ToString("MM/dd/yyyy") + "<br>";
                    msg += "Time: " + m.StartDate.ToString("hh:mm:ss") + " - " + m.EndDate.ToString("hh:mm:ss") + "<br>";
                    return msg;
                case "Meeting_New_Discussion":
                   
                    msg = "<b>A new discussion is started for the meeting: </b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + m.MeetingTitle + "<br>";
                    msg += "Date: " + m.StartDate.ToString("MM/dd/yyyy") + "<br>";
                    msg += "Time: " + m.StartDate.ToString("hh:mm:ss") + " - " + m.EndDate.ToString("hh:mm:ss") + "<br><hr>";
                    msg += "<b>Discussion Title:</b>" + "{0}";
                    return msg;
                default:
                    return "";
            }
        }
        private static string FormatDiscussiongMsg(string dTitle, string note, NotificationType type)
        {
            string i = type.ToString();
            string msg;
            switch (i)
            {
                case "Meeting_New_Discussion":
                    // note is meeting title
                    msg = "<b>A new discussion is started for the meeting: </b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + note + "<br><hr>";
                    msg += "<b>Discussion Title: </b>" + dTitle;
                    return msg;
                case "Agenda_New_Discussion":
                    // note is agenda title
                    msg = "<b>A new discussion is started for the agenda: </b><br><hr>";
                    msg += "<b>"+note+"</b><br><hr>";
                    msg += "<b>Discussion Title: </b>" + dTitle;
                    return msg;
                case "New_Reply":
                    // note is user name
                    msg = "<b> "+ note +"A new reply has been posted for the discussion: </b><br><hr>";
                    msg += "<b>Discussion Title: </b>" + dTitle;
                    return msg;
                default:
                    return "";
            }
        }
        private static string FormatAgendaMsg(Agendum m, NotificationType type, string mTitle)
        {
            string t = type.ToString();
            string msg;
            switch (t)
            {
                case "Agenda_New":
                    // note is meeting title
                    msg = "<b>A new agendum has been added to the meeting: </b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + mTitle + "<br><hr>";
                    msg += "<b>Agendum Title: </b>" + m.AgendaTitle;
                    return msg;
                case "Agenda_Update":
                    // note is agenda title
                    msg = "<b>An update has been made to Meeting Agenda: </b><br><hr>";
                    msg += "<b>Meeting Title: </b>" + mTitle;
                    return msg;
                default:
                    return "";

            }
        }
        private static string FormatTaskMsg(AgendaClass.AgendaTaskClass m, NotificationType type, string mTitle)
        {
            string t = type.ToString();
            string msg;
            switch (t)
            {
                case "Task_Assignment":
                    // note is meeting title
                    msg = "<b>You have been Assigned a new task for the meeting:</b><br><hr>";
                    msg += "<b>Meeting Details</b><br>";
                    msg += "Title: " + mTitle + "<br><hr>";
                    msg += "<b>Task: </b>" + m.Title + "<br/>";
                    msg += "<b>Due Date: </b>" + m.DueDate.ToShortDateString() + "<br/>";
                    return msg;
               
                default:
                    return "";
            }
        }

        private static void SendEmail(Dictionary<string, string> emails, Meeting m, NotificationType type)
        {

            string adminu;
            SmtpClient MailSender;
            CreateSmtpClient(out adminu, out MailSender);
            foreach (var t in emails)
            {
                string msg = FormatMeetingMsg(m, t.Value, type);
                // if parti insert
                if (type == NotificationType.Meeting_Parti_Insert)
                { SendEmail(adminu, t.Key, t.Key, GetSubject(type), msg, MailSender); }
                else
                { SendEmail(adminu, t.Value, t.Key, GetSubject(type), msg, MailSender); }
            
                
            }
        }
        private static void SendEmail(Dictionary<string, string> emails, string msg, NotificationType type)
        {

            string adminu;
            SmtpClient MailSender;
            CreateSmtpClient(out adminu, out MailSender);
            foreach (var t in emails)
            {
                SendEmail(adminu, t.Value, t.Key, GetSubject(type), msg, MailSender);
            }
        }
     
        private static void CreateSmtpClient(out string adminu, out SmtpClient MailSender)
        {
            adminu = ConfigurationManager.AppSettings["AdminU"];
            var adminp = ConfigurationManager.AppSettings["AdminP"];
            MailSender = new SmtpClient("192.168.1.190", 567);//219.93.39.190 port 567 // inside "192.168.1.190",567
            MailSender.Credentials = new NetworkCredential(adminu, adminp);
            MailSender.SendCompleted += new SendCompletedEventHandler(MailSender_SendCompleted);
        }

        static void MailSender_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            ((SmtpClient)sender).Dispose();
        }
        public static void Not_DisinviteParti(int meetingid, string username, int user)
        {
            using (boxEntities box = new boxEntities())
            {
                // create notification
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Meeting_Parti_Disinvite.ToString();
                notH.MeetingID = meetingid;
                notH.NotDate = DateTime.Now;
                notH.NotCreator = user;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                NotInfo notI = new NotInfo();
                notI.UserID = Profile.getUserID(username);
                notI.NotHeaderID = notH.NotID;
                notI.Ack = false;
                notI.Seen = false;
                box.NotInfoes.AddObject(notI);

                box.SaveChanges();
                
                // send email
                // get user email
                string email = Profile.getUserEmail(username);
                string from;
                SmtpClient client;
                CreateSmtpClient(out from, out client);
                string msg = FormatMeetingMsg(box.Meetings.Where(a=>a.MeetingID == meetingid).FirstOrDefault(),"", NotificationType.Meeting_Parti_Disinvite);
                SendEmail(from, email, username, GetSubject(NotificationType.Meeting_Parti_Disinvite), msg, client); 
            }
        }
        public static void InsertMeetingParticipant(List<int> parties, int meetingid, int user)
        {
            
            using (boxEntities box = new boxEntities())
            {
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Meeting_Parti_Insert.ToString();
                notH.MeetingID = meetingid;
                notH.NotDate = DateTime.Now;
                notH.NotCreator = user;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();
                var m = (from o in box.Meetings.Include("MeetingType")
                         where o.MeetingID == meetingid
                         select o).FirstOrDefault();
                if (m == null)
                {
                    throw new Exception("Wrong Meeting ID.");
                }
                // loop to add info and follower
                foreach (var i in parties)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                    //notH.NotInfoes.Add(notI);
                }
                var emails = getEmails(parties, meetingid);
                SendEmail(emails, m, NotificationType.Meeting_Parti_Insert);
                box.SaveChanges();
               
            }
        }
        public static void UpdateMeetingInfo(int meetingid, int user)
        {
            using (boxEntities box = new boxEntities())
            {
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Meeting_Info_Update.ToString();
                notH.MeetingID = meetingid;
                notH.NotCreator = user;
                notH.NotDate = DateTime.Now;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                // get the list of UserIDs of participants
                var parties = (from o in box.MeetingParticipants
                               where o.MeetingID == meetingid
                               select o.UserID).ToList();
                // loop to add info and follower
                foreach (var i in parties)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                    //notH.NotInfoes.Add(notI);
                }
                box.SaveChanges();

                // get meeting 
                var m = (from o in box.Meetings
                         where o.MeetingID == meetingid
                         select o).FirstOrDefault();
                SendEmail(getEmails(parties), m, NotificationType.Meeting_Info_Update);
              
            }
        }
        public static void Not_NewMeetingDiscussion(int meetingid, int user, string dTitle)
        {
            using (boxEntities box = new boxEntities())
            {
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Meeting_New_Discussion.ToString();
                notH.MeetingID = meetingid;
                notH.NotDate = DateTime.Now;
                notH.NotCreator = user;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                // get meeting parties

                var parties = (from o in box.MeetingParticipants
                               where o.MeetingID == meetingid
                               select o.UserID).ToList();
                // loop to add info and follower
                foreach (var i in parties)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                    //notH.NotInfoes.Add(notI);
                }
                box.SaveChanges();
                // get meeting title
                var meetingTitle = (from o in box.Meetings
                                   where o.MeetingID == meetingid
                                   select o.MeetingTitle).FirstOrDefault();

                var r = getEmails(parties);
                var msg = FormatDiscussiongMsg(dTitle, meetingTitle, NotificationType.Meeting_New_Discussion);
                SendEmail(r, msg, NotificationType.Meeting_New_Discussion);
            }
        }
        public static void Not_NewAgendaDiscussion(string dTitle, int AgendaID, int user)
        {
            using (boxEntities box = new boxEntities())
            {
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Agenda_New_Discussion.ToString();
                notH.AgendaID = AgendaID ;
                notH.NotDate = DateTime.Now;
                notH.NotCreator = user;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                // get recipients email
                var re = (from o in box.AgendaPrivacies
                          where o.AgendaID == AgendaID && o.Agendum.Published == true && o.CanSee == true
                          select o.MeetingParticipant.UserID).ToList();
                foreach (var i in re)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                }
                box.SaveChanges();
                // get agenda title 
                var agendaT = (from o in box.Agenda
                              where o.AgendaID == AgendaID
                              select o.AgendaTitle).FirstOrDefault();
                 // get information for the email
                var parti = getEmails(re);
                var msg = FormatDiscussiongMsg(dTitle, agendaT, NotificationType.Agenda_New_Discussion);

                SendEmail(parti, msg, NotificationType.Agenda_New_Discussion);

            }
       
        }
        public static void Not_NewReply(int disID, string user, string dTitle)
        {
            using (boxEntities box = new boxEntities())
            {
                int uid = Profile.getUserID(user);
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.New_Reply.ToString();
                notH.NotDate = DateTime.Now;
                notH.DiscussionID = disID;
                notH.NotCreator = uid;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                // get ids
                var ids = (from o in box.Replies
                           where o.DiscussionID == disID
                           select o.UserID).Union((from o in box.Discussions where o.ID == disID select o.UserID)).ToList();
                ids.Remove(uid);
                foreach (var i in ids)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                }
                box.SaveChanges();

                // get data
                var parti = getEmails(ids);
                var msg = FormatDiscussiongMsg(dTitle, user, NotificationType.New_Reply);
                SendEmail(parti, msg, NotificationType.New_Reply);
            }
        }
        public static void Not_NewAgenda(Agendum agenda, string user)
        {
            using (boxEntities box = new boxEntities())
            {
                int uid = Profile.getUserID(user);
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Agenda_New.ToString();
                notH.NotDate = DateTime.Now;
                notH.NotCreator = uid;
                notH.AgendaID = agenda.AgendaID;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                // get parti
                // get recipients email
                var re = (from o in box.AgendaPrivacies
                          where o.AgendaID == agenda.AgendaID && o.Agendum.Published == true && o.CanSee == true
                          select o.MeetingParticipant.UserID).ToList();
                re.Remove(uid);
                foreach (var i in re)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                }
                box.SaveChanges();
                // get data
                var mTitle = agenda.Meeting.MeetingTitle;
                var emails = getEmails(re);
                var msg = FormatAgendaMsg(agenda, NotificationType.Agenda_New,mTitle);
                SendEmail(emails, msg, NotificationType.Agenda_New);

            }
        }
        public static void Not_UpdateAgenda(Agendum agenda, string user)
        {
            using (boxEntities box = new boxEntities())
            {
                int uid = Profile.getUserID(user);
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Agenda_Update.ToString();
                notH.NotDate = DateTime.Now;
                notH.AgendaID = agenda.AgendaID;
                notH.NotCreator = uid;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                // get parti
                // get recipients email
                var re = (from o in box.AgendaPrivacies
                          where o.AgendaID == agenda.AgendaID && o.Agendum.Published == true && o.CanSee == true
                          select o.MeetingParticipant.UserID).ToList();
                re.Remove(uid);
                foreach (var i in re)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                }
                box.SaveChanges();
                // get data
                var mTitle = agenda.Meeting.MeetingTitle;
                var emails = getEmails(re);
                var msg = FormatAgendaMsg(agenda, NotificationType.Agenda_Update, mTitle);
                SendEmail(emails, msg, NotificationType.Agenda_Update);

            }
        }
        public static void Not_AgendaTaskAssign(AgendaClass.AgendaTaskClass task)
        {
            using (boxEntities box = new boxEntities())
            {
                int uid = Profile.getUserID(task.Creator);
                NotificationHeader notH = new NotificationHeader();
                notH.NotType = NotificationType.Agenda_Update.ToString();
                notH.NotDate = DateTime.Now;
                notH.AgendaID = task.AgendaID;
                notH.NotCreator = uid;
                box.NotificationHeaders.AddObject(notH);
                box.SaveChanges();

                var parti = Profile.GetUserIDs(task.Usernames).ToList();
                foreach (var i in parti)
                {
                    NotInfo notI = new NotInfo();
                    notI.UserID = i;
                    notI.NotHeaderID = notH.NotID;
                    notI.Ack = false;
                    notI.Seen = false;
                    box.NotInfoes.AddObject(notI);
                }
                box.SaveChanges();
                // get data
                var emails = getEmails(parti);
                var mtitle = box.Agenda.Where(a => a.AgendaID == task.AgendaID).Select(a => a.Meeting.MeetingTitle).FirstOrDefault();
                var msg = FormatTaskMsg(task, NotificationType.Task_Assignment, mtitle);

                SendEmail(emails, msg, NotificationType.Task_Assignment);
            }
        }

        // Get Methods
        public static int GetTotalNumberOfNotifications(string username)
        {

            int userid = Profile.getUserID(username);
            using (TransactionScope ee = new TransactionScope())
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.NotInfoes
                              where o.UserID == userid && o.Ack == false
                              select o).ToList();
                int count = result.Count();
                foreach (var t in result)
                {
                    t.Ack = true;
                }
                box.SaveChanges();
                ee.Complete();
                return count;
            }
        }

        public static List<NotificationForm> GetNotifications(string username)
        {
            int userid = Profile.getUserID(username);
            using (boxEntities box = new boxEntities())
            {
                var noti = (from o in box.NotInfoes.Include("NotificationHeader").Include("UserProfile")
                            where o.UserID == userid
                            select o).OrderByDescending(a=>a.NotificationHeader.NotDate).ToList();
                List<NotificationForm> finalResult = new List<NotificationForm>();
                foreach (var t in noti)
                {
                    NotificationForm mform = new NotificationForm();
                    switch (t.NotificationHeader.NotType)
                    {
                        case "Meeting_Parti_Insert":
                            var mID = t.NotificationHeader.MeetingID;
                            var meeting = (from o in box.Meetings
                                           where o.MeetingID == mID
                                           select o).FirstOrDefault();
                            if (meeting != null)
                            {
                                mform.NotText = "You have been invited to a meeting by " + t.NotificationHeader.UserProfile.UserName + " On: " + meeting.StartDate.ToShortDateString();
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + meeting.MeetingID;
                                mform.seen = t.Seen;
                                mform.Type = "Meetings";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Meeting, Maybe Deleted?!");
                            //}

                            break;
                        case "Meeting_Info_Update":
                            var uID = t.NotificationHeader.MeetingID;
                            var umeeting = (from o in box.Meetings
                                            where o.MeetingID == uID
                                            select o).FirstOrDefault();
                            if (umeeting != null)
                            {
                                mform.NotText = "An update has been made the meeting: " + umeeting.MeetingTitle + " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + umeeting.MeetingID;
                                mform.seen = t.Seen;
                                mform.Type = "Meetings";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Meeting, Maybe Deleted?!");
                            //}
                            break;
                        case "Agenda_New":
                            var GID = t.NotificationHeader.AgendaID;
                            var agenda = (from o in box.Agenda.Include("Meeting")
                                          where o.AgendaID == GID
                                          select o).FirstOrDefault();
                            if (agenda != null)
                            {
                                mform.NotText = "A new Agenda has been added to the meeting: " +agenda.Meeting.MeetingTitle + " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + agenda.Meeting.MeetingID + "&tab=Agenda";
                                mform.seen = t.Seen;
                                mform.Type = "Agenda";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Agenda, Maybe deleted?!");
                            //}
                            break;
                        case "Agenda_Update":
                            var guID = t.NotificationHeader.AgendaID;
                            var agendaup = (from o in box.Agenda.Include("Meeting")
                                            where o.AgendaID == guID
                                          select o).FirstOrDefault();
                            if (agendaup != null)
                            {
                                mform.NotText = "A update has been done to the Agendum in the meeting : " + agendaup.Meeting.MeetingTitle + " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + agendaup.Meeting.MeetingID + "&tab=Agenda";
                                mform.seen = t.Seen;
                                mform.Type = "Agenda";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Agenda, Maybe deleted?!");
                            //}
                            break;
                        case "Task_Assignment":
                            var tID = t.NotificationHeader.AgendaID;
                            var task = (from o in box.Agenda.Include("Meeting")
                                            where o.AgendaID == tID
                                            select o).FirstOrDefault();
                            if (task != null)
                            {
                                mform.NotText = "You have been assigned a task for the Agendum: " + task.AgendaTitle+ " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + task.Meeting.MeetingID + "&tab=Agenda";
                                mform.seen = t.Seen;
                                mform.Type = "Agenda";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Agenda, Maybe deleted?!");
                            //}
                            break;
                        case "Meeting_New_Discussion":
                             var dID = t.NotificationHeader.DiscussionID;
                             var dis = (from o in box.Discussions.Include("Meeting")
                                            where o.ID == dID
                                            select o).FirstOrDefault();
                            if (dis != null)
                            {
                                mform.NotText = "A new discussion has been added to the meeting: " + dis.Meeting.MeetingTitle+ " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + dis.Meeting.MeetingID + "&tab=Discussion";
                                mform.seen = t.Seen;
                                mform.Type = "Discussions";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Discussion, Maybe deleted?!");
                            //}
                            break;
                        case "Agenda_New_Discussion":
                            var adID = t.NotificationHeader.DiscussionID;
                            var adis = (from o in box.Discussions.Include("Agendum")
                                       where o.ID == adID
                                       select o).FirstOrDefault();
                            if (adis != null)
                            {
                                mform.NotText = "A new discussion has been added to the Agenda: " + adis.Agendum.AgendaTitle + " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "PMeetingSetUp.aspx?id=" + adis.Agendum.MeetingID + "&tab=Agenda";
                                mform.seen = t.Seen;
                                mform.Type = "Discussions";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such Discussion, Maybe deleted?!");
                            //}
                            break;
                        case "New_Reply":
                            var rid = t.NotificationHeader.DiscussionID;
                            var rep = (from o in box.Discussions
                                        where o.ID == rid
                                        select o).FirstOrDefault();
                            if (rep != null)
                            {
                                mform.NotText = "A new reply has been added to the discussion: " + rep.Title + " On: " + t.NotificationHeader.NotDate;
                                mform.NotURL = "#";
                                mform.seen = t.Seen;
                                mform.Type = "Discussions";
                                mform.ID = t.NotInfoID;
                                finalResult.Add(mform);
                            }
                            //else
                            //{
                            //    throw new Exception("No such rpely, Maybe deleted?!");
                            //}
                            break;
                    }
                }
                return finalResult;
            }
        }

        public static bool UpdateNotification(int notID)
        {
            using (boxEntities box = new boxEntities())
            {
                var noti = (from o in box.NotInfoes
                            where o.NotInfoID == notID
                            select o).FirstOrDefault();
                if (noti != null)
                {
                    noti.Seen = true;
                    box.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }


            }
        }



        // old methods
        public void CreateNot(string creator, string fileDir, NotType type)
        {
            using (boxEntities box = new boxEntities())
            {
                int user;
                int file;
                getUser(creator, box, out user);
                //getFile(fileDir, box, out file);


                // creat noti header
                NotificationHeader notHeader = new NotificationHeader();


                notHeader.NotType = type.ToString();

                box.NotificationHeaders.AddObject(notHeader);

                // creat notifications
                // first get the followers for the file
                //var followers = new Follow().getFollowers(fileDir);
                // for each follower creat a notitfication
                //foreach (var f in followers)
                //{
                //    NotInfo not = new NotInfo();
                //    not.Ack = false;
                //    not.Seen = false;
                //    not.FollowerID = f.FollowerID;
                //    not.NotID = notHeader.NotID;
                //    notHeader.NotInfoes.Add(not);
                //}
                box.SaveChanges();
            }
        }

        public List<NotInfo> getNotifications(string username)
        {
            using (boxEntities box = new boxEntities())
            {
                int user;
                getUser(username, box, out user);

                // get all not ack notifications
                var notifications = (from o in box.NotInfoes
                                     where o.Ack == false && o.DateSeen == null && o.UserID == user
                                     select o).ToList();
                return notifications;
            }
        }

        //private static void getFile(string fileDir, boxEntities box, out int file)
        //{
        //    file = (from o in box.OFiles
        //            where o.Directory == fileDir
        //            select o.FileID).FirstOrDefault();
        //}

        private static void getUser(string username, boxEntities box, out int user)
        {
            user = (from o in box.UserProfiles
                    where o.UserName == username
                    select o.UserID).FirstOrDefault();
        }

        private static Dictionary<string, string> getEmails(List<int> ids, int meetingid)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.MeetingParticipants.Include("UserProfile")
                              where ids.Contains(o.UserID) && o.MeetingID == meetingid
                              select o).ToList().ToDictionary(a => a.UserProfile.Email, a => a.Note);
                return result;
            }
        }
        private static Dictionary<string, string> getEmails(List<int> ids)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.UserProfiles
                              where ids.Contains(o.UserID)
                              select o).ToList().ToDictionary(a => a.UserName, a => a.Email);
                return result;
            }
        }

    }
}
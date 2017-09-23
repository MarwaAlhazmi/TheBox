using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace TheBox.Protected.BLL
{
    public class Watcher
    {
        public class MeetingWatcher
        {
            public static bool InsertParticipants(object[] obj, int meetingID, string username)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    // insert
                    var list = MeetingClass.CParticipant.SaveParticipants(obj, meetingID);
                    // Notification
                    int id = Profile.getUserID(username);
                    Notification.InsertMeetingParticipant(list, meetingID, id);
                    ee.Complete();
                    return true;
                }
            }
            public static bool UpdateInfo(Event ev, string username)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    int meetingid = MeetingClass.MeetingInfo.UpdateMeeting(ev);
                    int userid = Profile.getUserID(username);
                    Notification.UpdateMeetingInfo(meetingid, userid);
                    ee.Complete();
                    return true;
                }
            }
            public static void DisinviteParti(int meetingID, string username, string creator)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    MeetingClass.CParticipant.DisinviteParti(meetingID, username);
                    int c = Profile.getUserID(creator);
                    Notification.Not_DisinviteParti(meetingID, username, c);
                    ee.Complete();
                }
            }


        }

        public class DiscussionWatcher
        {
            public static TheBox.Protected.BLL.DiscussionClass.MeetingDiscussion SaveMeetingDiscussion(string title, string text, int meetingid, string username)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    var result = DiscussionClass.MeetingDiscussion.SaveMeetingDiscussion(title, text, meetingid, username);
                    int u = Profile.getUserID(username);
                    Notification.Not_NewMeetingDiscussion(meetingid, u, result.Title);
                    ee.Complete();
                    return result;

                }
            }
            public static TheBox.Protected.BLL.DiscussionClass.MeetingDiscussion SaveAgendaDiscussion(string title, string text, int agendaID, string username)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    var result = DiscussionClass.AgendaDiscussion.SaveAgendaDiscussion(title, text, agendaID, username);
                    Notification.Not_NewAgendaDiscussion(result.Title, agendaID, Profile.getUserID(username));
                    ee.Complete();
                    return result;
                }

            }
            public static Reply SaveReply(int disID, string username, string title, string text)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    var result = DiscussionClass.MeetingDiscussion.SaveReply(disID, username, title, text);
                    Notification.Not_NewReply(disID, username, result.Discussion.Title);
                    ee.Complete();
                    return result;
                }
            }
        }

        public class AgendaWatcher
        {
            public static void SaveAgenda(string username, string count, string title, string desc, int meetingID, bool published, int? subagenda)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    var ag = AgendaClass.AgendaInfoClass.SaveAgenda(count, title, desc, meetingID, published, subagenda);
                    if (ag.Published)
                    {
                        // send notifications
                        Notification.Not_NewAgenda(ag, username);
                    }
                    ee.Complete();
                }
            }

            public static void UpdateAgenda(string username, int agendaID, string count, string title, string desc, bool published, int? subagenda)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    var g = AgendaClass.AgendaInfoClass.UpdateAgenda(agendaID, count, title, desc, published, subagenda);
                    if (g.Published)
                    {
                        Notification.Not_UpdateAgenda(g, username);
                    }
                    ee.Complete();
                }
            }

            public static TheBox.Protected.BLL.AgendaClass.AgendaTaskClass SaveAgendaTask(string username, int agendaID, string title, string desc, DateTime dueDate, string[] usernames)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    var result = AgendaClass.AgendaTaskClass.SaveAgendaTask(username, agendaID, title, desc, dueDate, usernames);
                    Notification.Not_AgendaTaskAssign(result);
                    ee.Complete();
                    return result;
                }
            }

        }
        public class Out
        {
            public static int GetNumberOfNewNotifications(string username)
            {
                return Notification.GetTotalNumberOfNotifications(username);
            }
            public static List<TheBox.Protected.BLL.Notification.NotificationForm> GetAllNotification(string username)
            {
                var result = Notification.GetNotifications(username);
                return result;
            }
        }


        public static void CreatCommentNotification(string creator, string fileDir)
        {
            Notification not = new Notification();
            not.CreateNot(creator, fileDir, NotType.Comment);
        }
    }
}
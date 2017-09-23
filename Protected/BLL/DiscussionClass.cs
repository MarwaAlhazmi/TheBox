using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheBox.Protected.BLL
{
    public class DiscussionClass
    {
        public class MeetingDiscussion
        {
            public struct DiscussionInfo
            {
                public DateTime date;
                public string title;
                public string text;
                public bool head;
                public string username;
            }
            public string Title;
            public int ID;
            public DateTime Date;
            public string UserName;
            public int Replies;

            public static List<MeetingDiscussion> GetListMeetingDiscussions(int meetingID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var dis = (from o in box.Discussions
                              where o.MeetingID == meetingID
                              select new MeetingDiscussion() { 
                              Title = o.Title,
                              ID = o.ID,
                              Date = o.Date,
                              UserName = o.UserProfile.UserName,
                              Replies = o.Replies.Count()
                              }).OrderByDescending(a=>a.Date).ToList();
                    return dis;
                }

            }

            public static MeetingDiscussion SaveMeetingDiscussion(string title, string text, int meetingid, string username)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);
                        Discussion dis = new Discussion();
                        dis.UserID = userid;
                        dis.Date = DateTime.Now;
                        dis.Title = title;
                        dis.Text = text;
                        dis.MeetingID = meetingid;

                        box.Discussions.AddObject(dis);
                        box.SaveChanges();
                        MeetingDiscussion m = new MeetingDiscussion() { 
                        Title = dis.Title,
                        ID = dis.ID,
                        Date = dis.Date,
                        UserName = dis.UserProfile.UserName,
                        Replies = dis.Replies.Count()
                        };
                        return m;
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }

            public static Discussion GetDiscussionHead(int disID)
            {
                using (boxEntities box = new boxEntities())
                {

                    var dis = (from o in box.Discussions.Include("UserProfile")
                               where o.ID == disID
                               select o).FirstOrDefault();
                    if (dis != null)
                    {
                        return dis;
                    }
                    else
                    {
                        throw new Exception("No such discussion");
                    }                    
                }
            }

            public static List<Reply> GetDiscussionReplies(int disID)
            {
                using (boxEntities box = new boxEntities())
                {

                    var dis = (from o in box.Replies.Include("UserProfile")
                               where o.DiscussionID == disID
                               select o).ToList();
                    return dis;
                }
            }

            public static Reply SaveReply(int disID, string username, string title, string text)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        var userid = Profile.getUserID(username);
                        Reply r = new Reply();
                        r.Date = DateTime.Now;
                        r.Title = title;
                        r.Text = text;
                        r.UserID = userid;
                        r.DiscussionID = disID;

                        box.Replies.AddObject(r);
                        
                        box.SaveChanges();
                        var re = (from o in box.Replies.Include("Discussion")
                                  where o.ReplyID == r.ReplyID
                                  select o).FirstOrDefault();
                        return re;
                    }
                }
                catch
                {
                    throw new Exception("Error saving the reply");
                }
            }
        }

        public class AgendaDiscussion : MeetingDiscussion
        {
            public static List<MeetingDiscussion> GetListAgendaDiscussions(int AgendaID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var dis = (from o in box.Discussions
                               where o.AgendaID == AgendaID
                               select new MeetingDiscussion()
                               {
                                   Title = o.Title,
                                   ID = o.ID,
                                   Date = o.Date,
                                   UserName = o.UserProfile.UserName,
                                   Replies = o.Replies.Count()
                               }).OrderByDescending(a => a.Date).ToList();
                    return dis;
                }

            }

            public static MeetingDiscussion SaveAgendaDiscussion(string title, string text, int agendaID, string username)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);
                        Discussion dis = new Discussion();
                        dis.UserID = userid;
                        dis.Date = DateTime.Now;
                        dis.Title = title;
                        dis.Text = text;
                        dis.AgendaID = agendaID;

                        box.Discussions.AddObject(dis);
                        box.SaveChanges();
                        MeetingDiscussion m = new MeetingDiscussion()
                        {
                            Title = dis.Title,
                            ID = dis.ID,
                            Date = dis.Date,
                            UserName = dis.UserProfile.UserName,
                            Replies = dis.Replies.Count()
                        };
                        return m;
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            new public static Discussion GetDiscussionHead(int disID)
            {
               return GetDiscussionHead(disID);
            }

            new public static List<Reply> GetDiscussionReplies(int disID)
            {
                return GetDiscussionReplies(disID);
            }

            new public static Reply SaveReply(int disID, string username, string title, string text)
            {
                return SaveReply(disID, username, title, text);
            }
        }



        public class ProjectDiscussion
        {
            public static List<MeetingDiscussion> GetListProjectDiscussions(int ProjectID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var dis = (from o in box.Discussions
                               where o.ProjectID == ProjectID
                               select new MeetingDiscussion()
                               {
                                   Title = o.Title,
                                   ID = o.ID,
                                   Date = o.Date,
                                   UserName = o.UserProfile.UserName,
                                   Replies = o.Replies.Count()
                               }).OrderByDescending(a => a.Date).ToList();
                    return dis;
                }
            }

            public static MeetingDiscussion SaveProjectDiscussion(string title, string text, int ProjectID, string username)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);
                        Discussion dis = new Discussion();
                        dis.UserID = userid;
                        dis.Date = DateTime.Now;
                        dis.Title = title;
                        dis.Text = text;
                        dis.ProjectID = ProjectID;

                        box.Discussions.AddObject(dis);
                        box.SaveChanges();
                        MeetingDiscussion m = new MeetingDiscussion()
                        {
                            Title = dis.Title,
                            ID = dis.ID,
                            Date = dis.Date,
                            UserName = dis.UserProfile.UserName,
                            Replies = dis.Replies.Count()
                        };
                        return m;
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            new public static Discussion GetDiscussionHead(int disID)
            {
                return MeetingDiscussion.GetDiscussionHead(disID);
            }

            new public static List<Reply> GetDiscussionReplies(int disID)
            {
                return MeetingDiscussion.GetDiscussionReplies(disID);
            }

            new public static Reply SaveReply(int disID, string username, string title, string text)
            {
                return MeetingDiscussion.SaveReply(disID, username, title, text);
            }
        }


        public class FileDiscussion
        {
            public static List<MeetingDiscussion> GetListFileDiscussions(string path)
            {
                using (boxEntities box = new boxEntities())
                {
                    var dis = (from o in box.Discussions
                               where o.ProjectFile.FullName == path
                               select new MeetingDiscussion()
                               {
                                   Title = o.Title,
                                   ID = o.ID,
                                   Date = o.Date,
                                   UserName = o.UserProfile.UserName,
                                   Replies = o.Replies.Count()
                               }).OrderByDescending(a => a.Date).ToList();
                    return dis;
                }
            }

            public static MeetingDiscussion SaveFileDiscussion(string title, string text, string path, string username)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        // get file id
                        var file = (from o in box.ProjectFiles
                                    where o.FullName == path
                                    select o.ID).FirstOrDefault();
                        int userid = Profile.getUserID(username);
                        Discussion dis = new Discussion();
                        dis.UserID = userid;
                        dis.Date = DateTime.Now;
                        dis.Title = title;
                        dis.Text = text;
                        dis.FileID = file;

                        box.Discussions.AddObject(dis);
                        box.SaveChanges();
                        MeetingDiscussion m = new MeetingDiscussion()
                        {
                            Title = dis.Title,
                            ID = dis.ID,
                            Date = dis.Date,
                            UserName = dis.UserProfile.UserName,
                            Replies = dis.Replies.Count()
                        };
                        return m;
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }

            new public static Discussion GetDiscussionHead(int disID)
            {
                return MeetingDiscussion.GetDiscussionHead(disID);
            }

            new public static List<Reply> GetDiscussionReplies(int disID)
            {
                return MeetingDiscussion.GetDiscussionReplies(disID);
            }

            new public static Reply SaveReply(int disID, string username, string title, string text)
            {
                return MeetingDiscussion.SaveReply(disID, username, title, text);
            }
        }
    }
}
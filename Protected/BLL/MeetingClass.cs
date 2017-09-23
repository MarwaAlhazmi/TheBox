using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using System.Drawing;
using System.Globalization;

namespace TheBox.Protected.BLL
{
    public class Event
    {
        public int Id;
        public string Title;
        public DateTime Start;
        public DateTime End;
        public bool Allday;
        public string Type;
        public int MType;
        public string Desc;
        public string Invited;

        public Event(int id, string title, DateTime start, DateTime end, bool allday, string type)
        {
            Id = id;
            Title = title;
            Start = start;
            End = end;
            Allday = allday;
            Type = type;
        }
        public Event(int id, string title, DateTime start, DateTime end, bool allday, string type, string desc, string invited)
        {
            Id = id;
            Title = title;
            Start = start;
            End = end;
            Allday = allday;
            Type = type;
            Desc = desc;
            Invited = invited;
        }
        public Event(int id, string title, DateTime start, DateTime end, int mtype, string type, string desc, bool allday)
        {
            Id = id;
            Title = title;
            Start = start;
            End = end;
            Allday = allday;
            Type = type;
            Desc = desc;
            MType = mtype;
        }
        public Event() { }
    }
    public enum MeetingRoles
    {
        Creator,
        Moderator,
        Chairman,
        Attendee
    }
    public enum MeetingReponse
    {
        Pending,
        Attending,
        Rejected,
        None
    }
    public enum MeetingFilter
    {
        All,
        Calendar,
        Meeting,
        Appointment
    }

    public class MeetingClass
    {
        public static List<MeetingType> getMeetingTypes()
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.MeetingTypes where o.CatID != 0 select o).ToList();
                return result;
            }
        }
        public class MeetingInfo
        {
            public int ID;
            public string Title;
            public string Desc;
            public DateTime StartDate;
            public DateTime EndDate;
            public string Creator;
            public string Chairman;



            public MeetingInfo(int id, string title, DateTime sdate, DateTime edate, string desc)
            {
                ID = id;
                Title = title;
                Desc = desc;
                StartDate = sdate;
                EndDate = edate;
            }

            internal static int InsertMeeting(DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string title, string username, int type, string desc, bool allday)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);

                        Meeting meet = new Meeting();
                        meet.StartDate = new DateTime(sdate.Year, sdate.Month, sdate.Day, stime.Hour, stime.Minute, stime.Second);
                        meet.EndDate = new DateTime(edate.Year, edate.Month, edate.Day, etime.Hour, etime.Minute, etime.Second);
                        meet.MeetingTitle = title;
                        meet.Cal = false;
                        meet.Mee = true;
                        meet.App = false;
                        meet.MeetingDesc = desc;
                        meet.Type = type;
                        meet.AllDay = allday;

                        MeetingParticipant part1 = new MeetingParticipant();
                        part1.UserID = userid;
                        part1.Role = MeetingRoles.Creator.ToString();
                        part1.MeetingID = meet.MeetingID;
                        part1.Response = MeetingReponse.Attending.ToString();
                        part1.Note = "";
                        meet.MeetingParticipants.Add(part1);

                        // send invitations here
                        box.Meetings.AddObject(meet);

                        box.SaveChanges();
                        return meet.MeetingID;
                    }
                }
                catch
                {
                    return 0;
                }
            }
            internal static bool InsertAppointment(DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string title, string username, string invitedUser, bool allday, string desc)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);
                        int inv = Profile.getUserID(invitedUser);

                        Meeting meet = new Meeting();
                        meet.StartDate = new DateTime(sdate.Year, sdate.Month, sdate.Day, stime.Hour, stime.Minute, stime.Second);
                        meet.EndDate = new DateTime(edate.Year, edate.Month, edate.Day, etime.Hour, etime.Minute, etime.Second);
                        meet.MeetingTitle = title;
                        meet.MeetingDesc = desc;
                        meet.Cal = false;
                        meet.Mee = false;
                        meet.AllDay = allday;
                        meet.App = true;
                        // additional 

                        meet.Type = 0;

                        MeetingParticipant part1 = new MeetingParticipant();
                        part1.UserID = userid;
                        part1.Role = MeetingRoles.Creator.ToString();
                        part1.MeetingID = meet.MeetingID;
                        part1.Response = MeetingReponse.Pending.ToString();
                        part1.Note = "";
                        meet.MeetingParticipants.Add(part1);


                        MeetingParticipant invited = new MeetingParticipant();
                        invited.UserID = inv;
                        invited.Role = MeetingRoles.Attendee.ToString();
                        invited.MeetingID = meet.MeetingID;
                        invited.Response = MeetingReponse.Pending.ToString();
                        invited.Note = "";
                        meet.MeetingParticipants.Add(invited);

                        box.Meetings.AddObject(meet);

                        box.SaveChanges();
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            internal static bool InsertCalender(DateTime sdate, DateTime edate, DateTime stime, DateTime etime, string title, string username, bool allday)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);

                        Meeting meet = new Meeting();
                        meet.StartDate = new DateTime(sdate.Year, sdate.Month, sdate.Day, stime.Hour, stime.Minute, stime.Second);
                        meet.EndDate = new DateTime(edate.Year, edate.Month, edate.Day, etime.Hour, etime.Minute, etime.Second);
                        meet.MeetingTitle = title;
                        meet.Cal = true;
                        meet.Mee = false;
                        meet.App = false;
                        meet.AllDay = allday;
                        // additional 
                        meet.MeetingDesc = "";
                        meet.Type = 0;

                        // add participant as creator
                        MeetingParticipant part = new MeetingParticipant();
                        part.UserID = userid;
                        part.Role = MeetingRoles.Creator.ToString();
                        part.MeetingID = meet.MeetingID;
                        part.Response = MeetingReponse.Pending.ToString();
                        part.Note = "";
                        meet.MeetingParticipants.Add(part);
                        box.Meetings.AddObject(meet);

                        box.SaveChanges();

                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            internal static string formatDate(DateTime date, DateTime time)
            {
                string ndate = "\"" + date.Date.ToShortDateString() + " " + time.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture) + "\"";
                return ndate;
            }
            public static List<Meeting> GetAllEvents(string username)
            {
                int userid = Profile.getUserID(username);
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingParticipants
                                  where o.UserID == userid
                                  select o.Meeting
                                  ).ToList();

                    // format the result
                    //return formatEvents(result);

                    return result;
                }
            }
            public static List<Meeting> GetMeetings(string username)
            {
                int userid = Profile.getUserID(username);
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingParticipants
                                  where o.UserID == userid && o.Meeting.Mee == true
                                  select o.Meeting).ToList();

                    // format the result
                    return result;
                }
            }
            public static List<Meeting> GetCalender(string username)
            {
                int userid = Profile.getUserID(username);
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingParticipants
                                  where o.UserID == userid && o.Meeting.Cal == true
                                  select o.Meeting).ToList();

                    // format the result
                    return result;
                }
            }
            public static List<Meeting> GetAppointments(string username)
            {
                int userid = Profile.getUserID(username);
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingParticipants
                                  where o.UserID == userid && o.Meeting.App == true
                                  select o.Meeting).ToList();

                    // format the result
                    return result;
                }
            }
            internal static string formatEvents(List<Meeting> meeting)
            {
                string str = "[";
                for (int m = 0; m < meeting.Count(); m++)
                {

                    str += "{\"id\":" + meeting[m].MeetingID + ",";
                    str += "\"title\":\"" + meeting[m].MeetingTitle + "\",";
                    str += "\"start\":" + formatDate(meeting[m].StartDate, meeting[m].StartDate) + ",";
                    str += "\"end\":" + formatDate(meeting[m].EndDate, meeting[m].EndDate) + ",";
                    str += "\"allDay\":" + meeting[m].AllDay.ToString().ToLower();
                    if (meeting[m].App)
                    {
                        str += ", \"backgroundColor\":\"#BF3D30\"";
                    }
                    else if (meeting[m].Mee)
                    {
                        //str += ", \"url\":\"PMeetingSetUp.aspx?id=" + meeting[m].MeetingID + "\"";
                        str += ", \"backgroundColor\":\"#BF9330\"";
                    }
                    if (m == meeting.Count() - 1)
                    {
                        str += "}";
                    }
                    else
                    {
                        str += "},";
                    }
                }
                str += "]";
                return str;
            }
            public static Event GetInfo(int id)
            {
                using (boxEntities box = new boxEntities())
                {
                    var m = (from o in box.Meetings.Include("MeetingParticipants").Include("MeetingType")
                             where o.MeetingID == id
                             select o).FirstOrDefault();
                    if (m != null)
                    {
                        string type = MeetingFilter.Calendar.ToString();
                        Event e = new Event(m.MeetingID, m.MeetingTitle, m.StartDate, m.EndDate, m.AllDay, type);

                        if (m.Mee)
                        {
                            type = MeetingFilter.Meeting.ToString();
                            // get type
                            var t = m.MeetingType.CatID;
                            e = new Event(m.MeetingID, m.MeetingTitle, m.StartDate, m.EndDate, t, type, m.MeetingDesc, m.AllDay);
                        }
                        if (m.App)
                        {
                            type = MeetingFilter.Appointment.ToString();
                            var inv = m.MeetingParticipants.Where(a => a.Role == "Attendee").FirstOrDefault();
                            if (inv != null)
                            {
                                e = new Event(m.MeetingID, m.MeetingTitle, m.StartDate, m.EndDate, m.AllDay, type, m.MeetingDesc, inv.UserProfile.UserName);
                            }
                        }

                        return e;
                    }
                    else
                    {
                        throw new Exception("Error fetching Meeting information | MeetingClass.MeetinInfo.GetInfo");
                    }
                }
            }
            public static Event GetMeeting(int id)
            {
                using (boxEntities box = new boxEntities())
                {
                    var m = (from o in box.Meetings.Include("MeetingParticipants").Include("MeetingType")
                             where o.MeetingID == id && o.Mee == true
                             select o).FirstOrDefault();
                    if (m != null)
                    {
                        string type = MeetingFilter.Meeting.ToString();
                        // get type
                        var t = m.MeetingType.CatID;
                        Event e = new Event(m.MeetingID, m.MeetingTitle, m.StartDate, m.EndDate, t, type, m.MeetingDesc, m.AllDay);

                        return e;
                    }
                    else
                    {
                        throw new Exception("Error fetching Meeting information | MeetingClass.MeetinInfo.GetInfo");
                    }
                }
            }
            internal bool Update()
            {
                using (boxEntities box = new boxEntities())
                {
                    int id = ID;
                    var result = (from o in box.Meetings
                                  where o.MeetingID == id
                                  select o).FirstOrDefault();
                    if (result != null)
                    {
                        result.MeetingTitle = Title;
                        result.MeetingDesc = Desc;
                        result.StartDate = StartDate;
                        result.EndDate = EndDate;

                        box.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public static string GetMeetingType(int meetingID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.Meetings
                                  where o.MeetingID == meetingID
                                  select o).FirstOrDefault();
                    if (result != null)
                    {
                        if (result.Mee)
                            return MeetingFilter.Meeting.ToString();
                        else if (result.App)
                            return MeetingFilter.Appointment.ToString();
                        else
                            return MeetingFilter.Calendar.ToString();
                    }
                    else
                    {
                        return "false";
                    }
                }
            }
            public static bool Delete(int id)
            {
                using (boxEntities box = new boxEntities())
                {
                    var evn = (from o in box.Meetings
                               where o.MeetingID == id
                               select o).FirstOrDefault();
                    if (evn != null)
                    {
                        if (evn.Mee)
                        {
                            // meeting
                            // Files
                            var file = (from o in box.MeetingFiles
                                        where o.MeetingID == evn.MeetingID
                                        select o).ToList();
                            foreach (var t in file)
                            {
                                box.MeetingFiles.DeleteObject(t);
                            }

                            // locations
                            var loc = (from o in box.MeetingLocations
                                       where o.MeetingID == evn.MeetingID
                                       select o).ToList();
                            foreach (var t in loc)
                            {
                                box.MeetingLocations.DeleteObject(t);
                            }

                            // participants
                            var part = (from o in box.MeetingParticipants
                                        where o.MeetingID == evn.MeetingID
                                        select o).ToList();
                            foreach (var t in part)
                            {
                                box.MeetingParticipants.DeleteObject(t);
                            }

                            //resources
                            var res = (from o in box.MeetingResources
                                       where o.MeetingID == evn.MeetingID
                                       select o).ToList();
                            foreach (var t in res)
                            {
                                box.MeetingResources.DeleteObject(t);
                            }

                            // agenda
                            var age = (from o in box.Agenda
                                       where o.MeetingID == evn.MeetingID
                                       select o).ToList();
                            foreach (var t in age)
                            {
                                box.Agenda.DeleteObject(t);
                            }

                            //tasks
                            var task = (from o in box.TaskAssigns
                                        where o.MeetingID == evn.MeetingID
                                        select o).ToList();
                            foreach (var t in task)
                            {
                                box.TaskAssigns.DeleteObject(t);
                            }

                        }
                        else if (evn.App)
                        {
                            // appointment
                            var par = (from o in box.MeetingParticipants
                                       where o.MeetingID == evn.MeetingID
                                       select o).ToList();
                            foreach (var t in par)
                            {
                                box.MeetingParticipants.DeleteObject(t);
                            }
                        }
                        else
                        {
                            // calendar
                            var par = (from o in box.MeetingParticipants
                                       where o.MeetingID == evn.MeetingID
                                       select o).ToList();
                            foreach (var t in par)
                            {
                                box.MeetingParticipants.DeleteObject(t);
                            }
                        }
                        box.Meetings.DeleteObject(evn);
                        box.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public static bool UpdateCalendar(Event ev)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.Meetings
                                  where o.MeetingID == ev.Id
                                  select o).FirstOrDefault();
                    if (result != null)
                    {
                        result.StartDate = ev.Start;
                        result.EndDate = ev.End;
                        result.MeetingTitle = ev.Title;
                        result.AllDay = ev.Allday;

                        box.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public static bool UpdateAppointment(Event ev)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.Meetings
                                  where o.MeetingID == ev.Id
                                  select o).FirstOrDefault();
                    if (result != null)
                    {
                        result.StartDate = ev.Start;
                        result.EndDate = ev.End;
                        result.MeetingTitle = ev.Title;
                        result.AllDay = ev.Allday;
                        result.MeetingDesc = ev.Desc;
                        // get invited and 
                        var part = (from i in box.MeetingParticipants
                                    where i.MeetingID == ev.Id && i.Role == MeetingRoles.Attendee.ToString()
                                    select i).FirstOrDefault();
                        if (part != null)
                        {
                            if (part.UserProfile.UserName != ev.Invited)
                            {
                                box.MeetingParticipants.DeleteObject(part);
                                int inv = Profile.getUserID(ev.Invited);
                                MeetingParticipant invited = new MeetingParticipant();
                                invited.UserID = inv;
                                invited.Role = MeetingRoles.Attendee.ToString();
                                invited.MeetingID = ev.Id;
                                invited.Response = MeetingReponse.Pending.ToString();
                                result.MeetingParticipants.Add(invited);
                            }
                        }
                        box.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public static int UpdateMeeting(Event ev)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.Meetings
                                  where o.MeetingID == ev.Id
                                  select o).FirstOrDefault();
                    if (result != null)
                    {
                        result.StartDate = ev.Start;
                        result.EndDate = ev.End;
                        result.MeetingTitle = ev.Title;
                        result.AllDay = ev.Allday;
                        result.MeetingDesc = ev.Desc;
                        result.Type = ev.MType;
                        box.SaveChanges();
                        return ev.Id;
                    }
                    else
                    {
                        throw new Exception("Error Updating meeting information");
                    }
                }
            }
 
        }

        public class CParticipant
        {
            public string UserName;
            public string Response;
            public int Number;
            public string Role;
            public string Note;

            public static bool isParticipant(int meetingid, string username)
            {
                using (boxEntities box = new boxEntities())
                {
                    var parti = (from o in box.MeetingParticipants
                                 where o.MeetingID == meetingid && o.UserProfile.UserName == username
                                 select o).FirstOrDefault();
                    if (parti != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            public static List<CParticipant> getMeetingParticipant(int meetingid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingParticipants.Include("UserProfiles")
                                  where o.Meeting.MeetingID == meetingid
                                  select new CParticipant()
                                  {
                                      UserName = o.UserProfile.UserName,
                                      Response = o.Response,
                                      Role = o.Role,
                                      Note = o.Note
                                  }).ToList();
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception("Error getting the user - MeetingClass_getMeetingParticipant");
                    }
                }
            }
            public static UserProfile getMeetingParticipant(int meetingid, string role)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingParticipants
                                  where o.Role == role && o.Meeting.MeetingID == meetingid
                                  select o.UserProfile).FirstOrDefault();
                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception("Error getting the user - MeetingClass_getMeetingParticipant");
                    }
                }

            }
            public static List<int> SaveParticipants(object[] obj, int meetingID)
            {
                using (boxEntities box = new boxEntities())
                {
                    List<int> Parties = new List<int>();
                    var meeting = (from o in box.Meetings where o.MeetingID == meetingID select o).FirstOrDefault();
                    if (meeting != null)
                    {
                        foreach (var i in obj)
                        {
                            var item = (Dictionary<string, object>)i;
                            MeetingParticipant parti = new MeetingParticipant();

                            // get user id
                            var userid = Profile.getUserID(item["UserName"].ToString());
                            parti.UserID = userid;
                            parti.Response = MeetingReponse.Pending.ToString();
                            parti.Role = item["Role"].ToString();
                            parti.Note = item["Note"].ToString();
                            parti.InvitationDate = DateTime.Now;
                            Parties.Add(userid);

                            meeting.MeetingParticipants.Add(parti);
                            box.SaveChanges();
                            // for each agenda add agenda privacy
                            // get agneda
                            var agenda = (from o in box.Agenda
                                          where o.MeetingID == meetingID
                                          select o.AgendaID).ToList();
                            foreach (var g in agenda)
                            {
                                AgendaPrivacy p = new AgendaPrivacy();
                                p.CanSee = false;
                                p.AgendaID = g;
                                p.PartiID = parti.MeetingParti;
                                box.AgendaPrivacies.AddObject(p);
                            }
                        }

                        box.SaveChanges();
                        return Parties;
                    }
                    else
                    {
                        throw new Exception("Error Saving Participants of the meeting MeetingClass.Participants.SaveParticipants");
                    }
                }
            }
            public static void DisinviteParti(int meetingID, string username)
            {
                using (boxEntities box = new boxEntities())
                {

                    var parti = (from o in box.MeetingParticipants
                                 where o.MeetingID == meetingID && o.UserProfile.UserName == username
                                 select o).FirstOrDefault();
                    if (parti != null)
                    {
                        box.MeetingParticipants.DeleteObject(parti);
                        box.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("User is not invited to the meeting");
                    }
                }
            }
        }
        public class CResource
        {
            public int RID;
            public string RName;
            public string LName;
            public int LID;
            // insert resource
            public static int InsertResource(string r, string desc, string username)
            {
                try
                {
                    if (Roles.ChkAdmin(username))
                    {
                        using (boxEntities box = new boxEntities())
                        {
                            Resource re = new Resource();
                            re.RName = r;
                            re.RDesc = desc;

                            box.Resources.AddObject(re);
                            box.SaveChanges();
                            return re.RID;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                    
                }
                catch
                {
                    return 0;
                }
            }
            public static bool InsertLocation(string name, string username)
            {
                if (Roles.ChkAdmin(username))
                {
                    using (boxEntities box = new boxEntities())
                    {
                        Location loc = new Location();
                        loc.LocationName = name;

                        box.Locations.AddObject(loc);
                        box.SaveChanges();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            public static List<CResource> GetMeetingResources(int meetingID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingResources
                                  where o.MeetingID == meetingID
                                  select new CResource() { 
                                  RID = o.ResourceID,
                                  RName = o.Resource.RName
                                  }).ToList();
                    return result;
                }
            }
            public static CResource GetMeetingLocation(int meetingID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.MeetingLocations
                                 where o.MeetingID == meetingID
                                 select new CResource() { 
                                 LName = o.Location.LocationName,
                                 LID = o.LocationID
                                 }).FirstOrDefault();
                    return result;
                }
            }
            public static bool SetMeetingLocation(int meetingID, int locationID)
            {
                using (boxEntities box = new boxEntities())
                {
                   
                        // check if there is a meeting location
                      
                        if (locationID == 0)
                        {
                            var meeting = (from o in box.MeetingLocations
                                           where o.MeetingID == meetingID
                                           select o).FirstOrDefault();
                            if (meeting != null)
                            {
                                
                                box.MeetingLocations.DeleteObject(meeting);
                                box.SaveChanges();
                                return true;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            var loc = (from o in box.MeetingLocations
                                       where o.MeetingID == meetingID
                                       select o).FirstOrDefault();

                            if (loc == null)
                            {
                                // create new 
                                MeetingLocation location = new MeetingLocation();
                                location.MeetingID = meetingID;
                                location.LocationID = locationID;

                                box.MeetingLocations.AddObject(location);
                                box.SaveChanges();
                            }
                            else
                            {
                                // update
                                if (loc.LocationID != locationID)
                                {
                                    box.MeetingLocations.DeleteObject(loc);
                                    MeetingLocation location = new MeetingLocation();
                                    location.MeetingID = meetingID;
                                    location.LocationID = locationID;
                                    box.MeetingLocations.AddObject(location);
                                    box.SaveChanges();
                                }

                            }
                            return true;
                        }
                }
            }
            public static List<Location> GetAvailableLocations(DateTime Sdate, DateTime Edate)
            {
                // TODO: Need Test
                using (boxEntities box = new boxEntities())
                {
                    var result = box.MeetingLocations.Where(a => Sdate == a.Meeting.StartDate ||
                                              (Sdate > a.Meeting.StartDate && Sdate < a.Meeting.EndDate) ||
                                              (Sdate < a.Meeting.StartDate && Edate > a.Meeting.StartDate) ||
                                              (Edate == a.Meeting.EndDate)).Select(a => a.LocationID).ToList();
                    var AvlLocations = (from o in box.Locations
                                        where !result.Contains(o.LocationID)
                                        select o).ToList();
                    return AvlLocations;
                }
            }
            public static List<Location> GetAvailableLocations(int meetingid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.Meetings
                                  where o.MeetingID == meetingid
                                  select o).FirstOrDefault();
                    if (result != null)
                    {
                        return GetAvailableLocations(result.StartDate, result.EndDate);
                    }
                    else
                    {
                        throw new Exception("Error in Meeting ID");
                    }
                }
            }
            public static bool SetMeetingResources(object[] ids, int meetingid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var oldList = (from o in box.MeetingResources
                                   where o.MeetingID == meetingid
                                   select o.ResourceID).ToList();
                    if (oldList.Count() == 0)
                    {
                        // insert new resources 
                        foreach (var t in ids)
                        {
                            MeetingResource re = new MeetingResource();
                            re.MeetingID = meetingid;
                            re.ResourceID = (int)t;
                            re.Quantity = 1;

                            box.MeetingResources.AddObject(re);
                        }
                        box.SaveChanges();
                        return true;
                    }
                    else
                    {
                        
                        List<int> newList = new List<int>();
                        foreach (var t in ids)
                        {
                            newList.Add(Convert.ToInt32(t));
                        }
                                                
                        var intersect = oldList.Intersect(newList);
                        // insert new
                        foreach (var t in newList)
                        {
                            if (!intersect.Contains(t))
                            {
                                // insert
                                MeetingResource re = new MeetingResource();
                                re.MeetingID = meetingid;
                                re.ResourceID = t;
                                re.Quantity = 1;

                                box.MeetingResources.AddObject(re);
                            }
                        }
                        // delete
                        foreach (var t in oldList)
                        {
                            if (!intersect.Contains(t))
                            {
                                var del = (from o in box.MeetingResources
                                           where o.ResourceID == t && o.MeetingID == meetingid
                                           select o).FirstOrDefault();
                                if (del != null)
                                {
                                    box.MeetingResources.DeleteObject(del);
                                }
                            }
                        }

                        box.SaveChanges();
                        return true;
                    }

                }
            }
            
            public static void getlocations(DateTime date)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = box.MeetingLocations.Include("Location").Where(a => a.StartDate == date).ToList();
                }
            }
            internal static List<Resource> getResources()
            {
                List<Resource> result = new List<Resource>();
                using (boxEntities box = new boxEntities())
                {
                    result = box.Resources.ToList();
                }
                return result;
            }
            public static List<Location> getLocations()
            {
                List<Location> loc = new List<Location>();
                using (boxEntities box = new boxEntities())
                {
                    loc = box.Locations.Select(a => a).ToList();
                }
                return loc;
            }
            public static bool CheckAvl(int loc, DateTime date, DateTime stime, DateTime etime)
            {
                using (boxEntities box = new boxEntities())
                {
                    // TODO: check correct
                    var result = box.MeetingLocations.Where(a => a.StartDate == date && a.LocationID == loc).ToList();
                    var tt = result.Where(a => stime == a.StartTime ||
                                              (stime > a.StartTime && stime < a.EndTime) ||
                                              (stime < a.StartTime && etime > a.StartTime) ||
                                              (etime == a.EndTime)).ToList();
                    if (tt.Count != 0)
                    {
                        return false;
                    }
                    else
                    { return true; }
                }
            }

        }







        public static int GetNumberOfUpcomingMeetings(string username)
        {
            var now = DateTime.Now;
    
            using (boxEntities box = new boxEntities())
            {

                var num = (from o in box.MeetingParticipants
                           where o.UserProfile.UserName == username && o.Meeting.StartDate >= now && o.Meeting.Mee == true
                           select o.Meeting).ToList();
                return num.Count();
            }
        }

        public static List<Meeting> getUpcomingMeetings(string username)
        {
            var user = Profile.getUserID(username);
            //List<int> meetlist = getMeetingsID(userid);
            // get a list of invitations for this user
            using (boxEntities box = new boxEntities())
            {
                box.ContextOptions.LazyLoadingEnabled = false;
                DateTime now = DateTime.Now;
                var result = (from o in box.MeetingParticipants.Include("UserProfile").Include("Meeting")
                              where o.UserID == user && o.Meeting.StartDate >= now && o.Meeting.Mee == true
                              select o.Meeting).OrderBy(a => a.StartDate).ToList();
                return result;


                /*
                List<Meeting> meet;
                box.ContextOptions.LazyLoadingEnabled = false;
                DateTime now = DateTime.Now;
                var met = (from o in box.Meetings.Include("UserProfile").Include("UserProfile1")
                           where ((o.CreatorID == userid || o.Chariman == userid) && o.StartDate >= now)
                           orderby o.StartDate
                           select o).ToList();

                var inv = (from o in box.Invitations
                           where o.UserID == userid && o.Meeting.StartDate >= now
                           select o.Meeting).ToList();

                meet = met.Concat(inv).Distinct().ToList();

                return meet;*/
            }
        }

        public static List<Meeting> getMonthMeetings(string username)
        {
            int userid = Profile.getUserID(username);
            //List<int> meetlist = getMeetingsID(userid);
            // get a list of invitations for this user
            using (boxEntities box = new boxEntities())
            {
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                box.ContextOptions.LazyLoadingEnabled = false;
                DateTime now = DateTime.Now;
                var result = (from o in box.MeetingParticipants.Include("UserProfile").Include("Meeting")
                              where o.UserID == userid && o.Meeting.StartDate.Year == year && o.Meeting.StartDate.Month == month && o.Meeting.Mee == true
                              select o.Meeting).OrderBy(a=>a.StartDate).ToList();
                return result;
            }
        }

        public static List<Meeting> getHistoryMeetings(string username, int month, int year)
        {
            int userid = Profile.getUserID(username);
            //List<int> meetlist = getMeetingsID(userid);
            // get a list of invitations for this user
            using (boxEntities box = new boxEntities())
            {
                box.ContextOptions.LazyLoadingEnabled = false;
                DateTime now = DateTime.Now;
                var result = (from o in box.MeetingParticipants.Include("UserProfile").Include("Meeting")
                              where o.UserID == userid && o.Meeting.StartDate.Year == year && o.Meeting.StartDate.Month == month && o.Meeting.Mee == true
                              select o.Meeting).ToList();
                return result;
            }
        }
        public static List<Meeting> getMeetingByDate(DateTime date, string username)
        {
            int userid = Profile.getUserID(username);
            //List<int> meetlist = getMeetingsID(userid);
            // get a list of invitations for this user
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.MeetingParticipants
                              where o.UserID == userid && o.Meeting.StartDate == date && o.Meeting.Mee == true
                              select o.Meeting).ToList();
                return result;
            }
        }

        private static List<int> getMeetingsID(int userid)
        {
            using (boxEntities box = new boxEntities())
            {
                var inv = (from o in box.Invitations
                           where o.UserID == userid && o.Meeting.StartDate >= DateTime.Now && o.Meeting.Mee == true
                           select o.MeetingID).ToList();
                return inv;
            }
        }

      


       

        public static Dictionary<int, List<int>> getYearMonth()
        {
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            using (boxEntities box = new boxEntities())
            {
                var years = box.Meetings.Select(a => a.StartDate.Year).Distinct();
                foreach (var y in years)
                {
                    var months = box.Meetings.Where(a => a.StartDate.Year == y && a.Mee == true).Select(a => a.StartDate.Month).Distinct().ToList();
                    result.Add(y, months);
                }
            }
            return result;
        }



    }


}
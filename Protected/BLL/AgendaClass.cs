using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TheBox.Protected.BLL
{
    public class AgendaClass
    {
        public int AgendaID;
        public string AgendaCount;
        public string Title;
        public string Description;
        public int MeetingID;
        public bool Published;
        public string Privacy;

        public class AgendaInfoClass
        {
            public string Count;
            public string Title;
            public string Desc;
            public string Status;
            public int ID;
            public string Published;
            public bool Creator;
            public int Parent;
            public enum AgendaStatus { Public, Private }

            public static Agendum SaveAgenda(string count, string title, string desc, int meetingID, bool published, int? subagenda)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        Agendum ag = new Agendum();
                        ag.AgendaCount = count;
                        ag.AgendaTitle = title;
                        ag.AgendaDesc = desc;
                        ag.MeetingID = meetingID;
                        ag.Published = published;
                        ag.Privacy = AgendaStatus.Public.ToString();
                        if (subagenda != 0)
                        {
                            ag.SubAgenda = subagenda;
                        }

                        box.Agenda.AddObject(ag);
                        box.SaveChanges();

                        // get meeting participants and save them in agenda privacy
                        var parti = (from o in box.MeetingParticipants
                                     where o.MeetingID == meetingID
                                     select o).ToList();
                        foreach (var p in parti)
                        {
                            AgendaPrivacy pri = new AgendaPrivacy();
                            pri.AgendaID = ag.AgendaID;
                            pri.PartiID = p.MeetingParti;
                            pri.CanSee = false;
                            if (p.Role == MeetingRoles.Creator.ToString())
                            {
                                pri.CanSee = true;
                            }
                            box.AgendaPrivacies.AddObject(pri);
                        }
                        box.SaveChanges();
                        return ag;
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            public static Agendum UpdateAgenda(int agendaID, string count, string title, string desc, bool published, int? subagenda)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        var ag = (from o in box.Agenda.Include("Meeting")
                                  where o.AgendaID == agendaID
                                  select o).FirstOrDefault();
                        if (ag != null)
                        {
                            ag.AgendaCount = count;
                            ag.AgendaTitle = title;
                            ag.AgendaDesc = desc;
                            if (published != false)
                            {
                                ag.Published = published;
                            }
                            
                            ag.Privacy = AgendaStatus.Public.ToString();
                            if (subagenda != 0)
                            {
                                ag.SubAgenda = subagenda;
                            }
                        }
                        box.SaveChanges();
                        return ag;
                        // get meeting participants and save them in agenda privacy
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
            public static List<AgendaInfoClass> GetMeetingAgenda(int meetingId, string username)
            {
                try
                {
                    using (boxEntities box = new boxEntities())
                    {
                        int userid = Profile.getUserID(username);

                        var role = (from o in box.MeetingParticipants
                                    where o.MeetingID == meetingId && o.UserID == userid
                                    select o).FirstOrDefault();
                        List<AgendaInfoClass> finalResult = new List<AgendaInfoClass>();
                        if (role.Role == MeetingRoles.Creator.ToString())
                        {
                            // return all agenda
                            var result = (from o in box.Agenda
                                          where o.MeetingID == meetingId
                                          select new AgendaInfoClass()
                                          {
                                              Count = o.AgendaCount,
                                              Title = o.AgendaTitle,
                                              Desc = o.AgendaDesc,
                                              Status = o.Privacy,
                                              ID = o.AgendaID,
                                              Published = (o.Published ? "Published" : "Not published"),
                                              Creator = true
                                          }).OrderBy(a => a.Count).ToList();
                            return result;
                        }
                        else
                        {
                            // check agenda privacy and published
                            var user = (from o in box.AgendaPrivacies
                                        where o.Agendum.MeetingID == meetingId && o.PartiID == role.MeetingParti && o.Agendum.Published == true && o.CanSee == true
                                        select o.Agendum).OrderBy(a => a.AgendaCount).ToList();
                            List<AgendaInfoClass> result = new List<AgendaInfoClass>();
                            foreach (var i in user)
                            {
                                AgendaInfoClass ii = new AgendaInfoClass();
                                ii.Count = i.AgendaCount;
                                ii.Title = i.AgendaTitle;
                                ii.Desc = i.AgendaDesc;
                                ii.Status = i.Privacy;
                                ii.ID = i.AgendaID;
                                ii.Published = i.Privacy;
                                ii.Creator = false;
                                result.Add(ii);
                            }

                            return result;
                        }


                    }
                }
                catch
                {
                    throw new Exception("Error fetching Meeting Agenda");
                }
            }

            public static AgendaInfoClass GetAgenda(int gid)
            {
                try
                {

                    using (boxEntities box = new boxEntities())
                    {
                        var agenda = (from o in box.Agenda
                                      where o.AgendaID == gid
                                      select new AgendaInfoClass()
                                      {
                                          ID = o.AgendaID,
                                          Title = o.AgendaTitle,
                                          Desc = o.AgendaDesc,
                                          Count = o.AgendaCount,
                                          Parent = (o.SubAgenda == null ? 0 : (int)o.SubAgenda)
                                      }).FirstOrDefault();
                        if (agenda != null)
                        {
                            return agenda;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                catch
                {
                    throw new Exception();
                }
            }
        }
        public class AgendaPrivacyClass
        {
            public string UserName;
            public bool CanSee;
            public string Role;
            public static bool CheckRole(string username, int meetingID, MeetingRoles role)
            {
                using (boxEntities box = new boxEntities())
                {
                    var user = Profile.getUserID(username);
                    var r = (from o in box.MeetingParticipants
                             where o.UserID == user && o.MeetingID == meetingID
                             select o).FirstOrDefault();
                    if (r != null)
                    {
                        if (r.Role == role.ToString())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // return false
                        return false;
                    }
                }
            }
            public static bool CheckRole(string username, MeetingRoles role, int agendaID)
            {
                using (boxEntities box = new boxEntities())
                {

                    // get meeting
                    var m = (from o in box.Agenda
                             where o.AgendaID == agendaID
                             select o.MeetingID).FirstOrDefault();
                    return CheckRole(username, m, role);
                }
            }
            public static bool CheckRole(int taskID, string username, MeetingRoles role)
            {
                using (boxEntities box = new boxEntities())
                {

                    // get meeting
                    var m = (from o in box.TaskAssigns
                             where o.TaskID == taskID
                             select o.AgendaID).FirstOrDefault();
                    int u = 0;
                    if (m != null)
                    {
                        u = (int)m;
                    }
                    return CheckRole(username, MeetingRoles.Creator, u);
                }
            }
            public static List<AgendaPrivacyClass> GetParticipants(int agendaID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.AgendaPrivacies.Include("MeetingParticipant").Include("UserProfile")
                                  where o.AgendaID == agendaID
                                  select new AgendaPrivacyClass()
                                  {
                                      UserName = o.MeetingParticipant.UserProfile.UserName,
                                      CanSee = o.CanSee,
                                      Role = o.MeetingParticipant.Role
                                  }).ToList();
                    return result;
                }
            }
            public static void UpdateAgendaPrivacy(int AgendaID, object[] users)
            {
                // get  list of names 
                List<string> usernames = new List<string>();
                Dictionary<string, bool> objects = new Dictionary<string, bool>();
                foreach (var i in users)
                {
                    Dictionary<string, object> item = (Dictionary<string, object>)i;
                    objects.Add(item["UserName"].ToString(), Convert.ToBoolean(item["CanSee"]));
                    usernames.Add(item["UserName"].ToString());
                }
                // get pbjects then update
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.AgendaPrivacies.Include("MeetingParticipant")
                                  where usernames.Contains(o.MeetingParticipant.UserProfile.UserName) && o.AgendaID == AgendaID
                                  select o).ToList();

                    foreach (var t in result)
                    {
                        t.CanSee = objects[t.MeetingParticipant.UserProfile.UserName];
                    }
                    box.SaveChanges();
                }
            }
            public static string[] ListParticipantUsername(int agendaID)
            {
                return GetParticipants(agendaID).Where(a => a.CanSee == true).Select(a => a.UserName).ToArray();
            }
            public static int[] ListParticipantIDs(int agendaID)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.AgendaPrivacies.Include("MeetingParticipant").Include("UserProfile")
                                  where o.AgendaID == agendaID && o.CanSee == true
                                  select o.MeetingParticipant.UserID).ToArray();
                    return result;
                }
            }
        }

        public class AgendaTaskClass : TaskClass
        {
            public int AgendaID;
         
            public static AgendaTaskClass SaveAgendaTask(string username, int agendaID, string title, string desc, DateTime dueDate, string[] usernames)
            {
                AgendaTaskClass rObject = new AgendaTaskClass();

                using (boxEntities box = new boxEntities())
                {
                    usernames = usernames.Distinct().ToArray();


                    if (AgendaClass.AgendaPrivacyClass.CheckRole(username, MeetingRoles.Creator, agendaID))
                    {
                        // create the header
                        TaskAssign task = new TaskAssign();
                        task.AgendaID = agendaID;
                        task.TaskTitle = title;
                        task.TaskDesc = desc;
                        task.DueDate = dueDate;
                        task.DateCreated = DateTime.Now;
                        task.Creator = Profile.getUserID(username);
                        box.TaskAssigns.AddObject(task);
                        box.SaveChanges();

                        // check if all
                        int[] users = { };
                        string[] na = { };
                        if (usernames.Contains("All"))
                        {
                            // get all users in meeting ids
                            users = AgendaPrivacyClass.ListParticipantIDs(agendaID);
                            na = AgendaPrivacyClass.ListParticipantUsername(agendaID);
                        }
                        else
                        {
                            users = Profile.GetUserIDs(usernames);
                            na = usernames;
                        }
                        foreach (var t in users)
                        {
                            TaskUser u = new TaskUser();
                            u.UserID = t;
                            u.TaskID = task.TaskID;
                            u.AssignedDate = DateTime.Now;
                            task.TaskUsers.Add(u);
                        }
                        box.SaveChanges();
                        rObject.TaskID = task.TaskID;
                        rObject.CreatedDate = task.DateCreated;
                        rObject.DueDate = task.DueDate;
                        rObject.Desc = task.TaskDesc;
                        rObject.Title = task.TaskTitle;
                        rObject.AgendaID = agendaID;
                        rObject.Usernames = na;
                        rObject.Creator = username;

                        return rObject;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }



                }

            }
            public static List<AgendaTaskClass> GetAgendaTasks(string username, int agendaID)
            {
                using (boxEntities box = new boxEntities())
                {
                    // user id 
                    int userid = Profile.GetUserID(username);
                    var result = new List<AgendaTaskClass>();
                    // check the user role if not creator then get the assigned to tasks only
                    if (AgendaPrivacyClass.CheckRole(username, MeetingRoles.Creator, agendaID))
                    {
                        result = (from o in box.TaskAssigns.Include("TaskUsers").Include("UserProfile")
                                  where o.AgendaID == agendaID
                                  select new AgendaTaskClass()
                                  {
                                      Title = o.TaskTitle,
                                      Desc = o.TaskDesc,
                                      TaskID = o.TaskID,
                                      DueDate = o.DueDate,
                                      CreatedDate = o.DateCreated,

                                  }).ToList();
                        foreach (var t in result)
                        {
                            t.Usernames = box.TaskUsers.Where(a => a.TaskID == t.TaskID).Select(a => a.UserProfile.UserName).ToArray();
                        }
                    }
                    else
                    {
                        result = (from o in box.TaskUsers
                                  where o.TaskAssign.AgendaID == agendaID && o.UserID == userid
                                  select new AgendaTaskClass()
                                  {
                                      Title = o.TaskAssign.TaskTitle,
                                      Desc = o.TaskAssign.TaskDesc,
                                      TaskID = o.TaskAssign.TaskID,
                                      DueDate = o.TaskAssign.DueDate,
                                      CreatedDate = o.TaskAssign.DateCreated,

                                  }).ToList();
                    }
                    return result;
                }
            }
            public static AgendaTaskClass UpdateAgendaTask(int taskID, string username, int agendaID, string title, string desc, DateTime dueDate, string[] usernames)
            {
                using (boxEntities box = new boxEntities())
                {
                    usernames = usernames.Distinct().ToArray();
                    // get meetin id 
                    var m = (from o in box.Agenda
                             where o.AgendaID == agendaID
                             select o.MeetingID).FirstOrDefault();

                    if (AgendaPrivacyClass.CheckRole(username, MeetingRoles.Creator, agendaID))
                    {
                        // get the header
                        var task = (from o in box.TaskAssigns.Include("TaskUsers")
                                    where o.TaskID == taskID
                                    select o).FirstOrDefault();
                        task.TaskTitle = title;
                        task.TaskDesc = desc;
                        task.DueDate = dueDate;
                        task.Creator = Profile.getUserID(username);
                        box.SaveChanges();

                        // get the users
                        int[] NewUsers = { };
                        if (usernames.Contains("All"))
                        {
                            // get all users in meeting ids
                            NewUsers = AgendaPrivacyClass.ListParticipantIDs(agendaID);
                        }
                        else
                        {
                            NewUsers = Profile.GetUserIDs(usernames);
                        }
                        // get old users
                        int[] OldUsers = task.TaskUsers.Select(a => a.UserProfile.UserID).ToArray();
                        int[] same = NewUsers.Intersect(OldUsers).ToArray();
                        // insert new 
                        foreach (var t in NewUsers)
                        {
                            if (!same.Contains(t))
                            {
                                TaskUser u = new TaskUser();
                                u.UserID = t;
                                u.TaskID = task.TaskID;
                                u.AssignedDate = DateTime.Now;
                                task.TaskUsers.Add(u);
                            }
                        }
                        // delete
                        foreach (var o in OldUsers)
                        {
                            if (!same.Contains(o))
                            {
                                var dtask = task.TaskUsers.Where(a => a.UserID == o).FirstOrDefault();
                                if (dtask != null)
                                {
                                    task.TaskUsers.Remove(dtask);
                                }
                            }
                        }
                        box.SaveChanges();
                        // return object
                        AgendaTaskClass tt = new AgendaTaskClass();
                        tt.Title = task.TaskTitle;
                        tt.Desc = task.TaskDesc;
                        tt.DueDate = task.DueDate;
                        tt.TaskID = task.TaskID;
                        tt.CreatedDate = task.DateCreated;
                        tt.Usernames = task.TaskUsers.Select(a => a.UserProfile.UserName).ToArray();

                        return tt;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }


                }

            }
           
          
        }



        public static List<AgendaClass> GetAgendaAjax(int meetingID, string username)
        {
            int userid = Profile.GetUserID(username);
            using (boxEntities box = new boxEntities())
            {
                var t = (from o in box.AgendaPrivacies
                         where o.PartiID == userid && o.Agendum.MeetingID == meetingID
                         select o.Agendum).ToList();
                List<AgendaClass> agenda = new List<AgendaClass>();
                foreach (var o in t)
                {
                    AgendaClass item = new AgendaClass();
                    item.AgendaCount = o.AgendaCount;
                    item.Title = o.AgendaTitle;
                    item.Description = o.AgendaDesc;
                    item.MeetingID = o.MeetingID;
                    item.Published = o.Published;
                    //item.Privacy = o.Privacy;
                    item.AgendaID = o.AgendaID;
                    agenda.Add(item);
                }
                return agenda;
            }
        }

        public static bool DeleteAgendum(int AgendumID)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = box.Agenda.Where(a => a.AgendaID == AgendumID).Select(a => a).FirstOrDefault();
                if (result != null)
                {
                    box.Agenda.DeleteObject(result);
                    box.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<Agendum> GetAgenda(int meetingID, string username)
        {
            int userid = Profile.GetUserID(username);
            using (boxEntities box = new boxEntities())
            {
                var t = (from o in box.AgendaPrivacies
                         where o.PartiID == userid && o.Agendum.MeetingID == meetingID
                         select o.Agendum).ToList();
                return t;
            }

        }


        // Another class
        public class AgendaPrivacyClassOld
        {
            public string Username;
            public string UserID;
            public bool CanSee;

            // set privacy
            // edit privacy
            // get privacy

            //public static List<AgendaPrivacy> GetAgendaPrivacy(int AgendumID)
            //{

            //}

            public static bool CanView(int userid, int meetingid)
            {
                using (boxEntities box = new boxEntities())
                {
                    string role1 = MeetingRoles.Creator.ToString();
                    string role2 = MeetingRoles.Moderator.ToString();
                    var result = (from o in box.MeetingParticipants
                                  where o.MeetingID == meetingid && o.MeetingParti == userid && (o.Role == role1 || o.Role == role2)
                                  select o).ToList();
                    if (result.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
        }
    }
}
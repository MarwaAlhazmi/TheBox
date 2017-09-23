using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Transactions;
using System.IO;

namespace TheBox.Protected.BLL
{
    public enum ProjectRoles
    {
        Admin,
        Editor,
        Moderator,
        Commentor,
        User
    }
    public class ProjectClass
    {
        private static string FormatPath(string path)
        {
            path = path.Replace("\\", "/");
            //path = path.Replace("//", "\\");
            return path;
        }
        public static string GetConfigString(string strKey, string strDefaultValue = "")
        {
            //strKey = "DataDir/" + strKey;
            if (ConfigurationManager.AppSettings[strKey] == null)
            {
                return strDefaultValue;
            }
            else
            {
                return Convert.ToString(ConfigurationManager.AppSettings[strKey]);
            }
        }
        private static string GetProjectRoot()
        {
            return GetConfigString("ProjectRoot", "Datadir");
        }
        private static string GetHTMLProjectRoot()
        {
            return GetConfigString("ProjectHTMLRoot", "HTMLProject");

        }
        private static string GetProjectRootURL()
        {
            return HttpContext.Current.Server.MapPath(GetConfigString("ProjectRoot", "Datadir"));
        }
        private static string GetHTMLProjectRootURL()
        {
            return HttpContext.Current.Server.MapPath(GetConfigString("ProjectHTMLRoot", "HTMLProject"));

        }
        public static List<ProjectRoles> GetProjectRoles()
        {
            var t = EnumUtil.GetValues<ProjectRoles>().ToList();
            return t;
        }
        public static Dictionary<string, string> GetProjectFiles(string username, string projectPath)
        {
            using (boxEntities box = new boxEntities())
            {
                projectPath = FormatPath(projectPath);
                var result = (from o in box.ProjectPrivacies
                              where o.ProjectUser.UserProfile.UserName == username && o.CanSee && o.ProjectFile.Project.RootPath == projectPath
                              select o).ToDictionary(a => a.ProjectUser.Project.RootPath, a => a.ProjectFile.Directory);
                return result;
            }
        }
        public static List<string> GetProjectFolderFiles(string username, string folderPath, int projectID)
        {
            using (boxEntities box = new boxEntities())
            {
                folderPath = FormatPath(folderPath);
                var result = (from o in box.ProjectPrivacies
                              where o.ProjectUser.UserProfile.UserName == username && o.CanSee && o.ProjectFile.ProjectID == projectID && o.ProjectFile.Directory == folderPath
                              select o.ProjectFile.FullName.ToLower()).ToList();
                return result;
            }
        }
        public static int GetProjectID(string projectPath)
        {
            using (boxEntities box = new boxEntities())
            {
                projectPath = FormatPath(projectPath);
                var id = (from o in box.Projects
                          where projectPath.StartsWith(o.RootPath)
                          select o.ProjectID).FirstOrDefault();
                return id;
            }
        }

        public static void CreateProject(string Title, string Username, Dictionary<string, string> Users)
        {
            if (Roles.ChkAdmin(Username))
            {
                using (TransactionScope ee = new TransactionScope())
                using (boxEntities box = new boxEntities())
                {
                    var id = Profile.getUserID(Username);
                    var path = GetProjectRoot() + "/" + Title;
                    var pathURL = GetProjectRootURL() + "\\" + Title;
                    var hpath = GetHTMLProjectRootURL() + "\\" + Title;
                    Project p = new Project();
                    p.DateCreated = DateTime.Now;
                    p.ProjectTitle = Title;
                    p.RootPath = path;

                    box.Projects.AddObject(p);

                    int adminID = Profile.GetUserID(Username);
                    ProjectUser admin = new ProjectUser();
                    admin.UserID = adminID;
                    admin.Role = ProjectRoles.Admin.ToString();
                    admin.ProjectID = p.ProjectID;
                    admin.Creator = true;
                    admin.AddedDate = DateTime.Now;
                    box.ProjectUsers.AddObject(admin);
                    box.SaveChanges();

                    // create the folder and html
                    if (!Directory.Exists(pathURL))
                    {
                        Directory.CreateDirectory(pathURL);
                    }
                    if (!Directory.Exists(hpath))
                    {
                        Directory.CreateDirectory(hpath);
                    }

                    InsertProjectUsers(Username, Users, p.ProjectID);


                    ee.Complete();
                }
            }
        }
        public static void InsertProjectUsers(string Username, Dictionary<string, string> Users, int projectid)
        {
            using (boxEntities box = new boxEntities())
            {
                var added = (from o in box.ProjectUsers
                             where o.ProjectID == projectid
                             select o.UserProfile.UserName).ToList();

                // save users as Project users and the creator in privacy table
                if (Users.Keys.Contains("All"))
                {
                    var role = Users["All"];
                    // get all users
                    var all = (from o in box.UserProfiles
                               select o).ToList();

                    foreach (var t in all)
                    {
                        if (!added.Contains(t.UserName))
                        {
                            if (Users.Keys.Contains(t.UserName))
                            {
                                ProjectUser u = new ProjectUser();
                                u.UserID = t.UserID;
                                u.Role = Users[t.UserName];
                                u.ProjectID = projectid;
                                u.AddedDate = DateTime.Now;
                                u.Creator = false;
                                box.ProjectUsers.AddObject(u);
                            }
                            else
                            {
                                if (t.UserName != Username)
                                {
                                    ProjectUser u = new ProjectUser();
                                    u.UserID = t.UserID;
                                    u.Role = role;
                                    u.ProjectID = projectid;
                                    u.AddedDate = DateTime.Now;
                                    u.Creator = false;
                                    box.ProjectUsers.AddObject(u);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var user in Users)
                    {
                        ProjectUser u = new ProjectUser();
                        u.UserID = Profile.GetUserID(user.Key);
                        u.Role = user.Value;
                        u.ProjectID = projectid;
                        u.AddedDate = DateTime.Now;
                        u.Creator = false;
                        box.ProjectUsers.AddObject(u);
                    }
                }
                box.SaveChanges();
            }

        }
        public static bool DeleteProject(string dir)
        {

            using (boxEntities box = new boxEntities())
            {
                var pro = (from o in box.Projects
                           where o.RootPath == dir
                           select o).FirstOrDefault();
                if (pro != null)
                {
                    box.Projects.DeleteObject(pro);
                    box.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public class ProjectUsersClass
        {
            public static List<KeyValuePair<string, string>> GetProjectUsers(int Projectid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var project = (from o in box.ProjectUsers
                                   where o.ProjectID == Projectid
                                   select o).ToDictionary(a => a.UserProfile.UserName, a => a.Role).ToList();
                    return project;
                }
            }
            public static List<string> ListProjectUsers(int Projectid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var project = (from o in box.ProjectUsers
                                   where o.ProjectID == Projectid && o.Creator == false
                                   select o.UserProfile.UserName).ToList();
                    return project;
                }
            }
            public static bool DeleteProjectUser(string username, int projectid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var user = (from o in box.ProjectUsers
                                where o.ProjectID == projectid && o.UserProfile.UserName == username
                                select o).FirstOrDefault();
                    if (user != null)
                    {
                        box.ProjectUsers.DeleteObject(user);
                        box.SaveChanges();
                        return true;
                    }
                    return false;

                }

            }
        }

        public class ProjectPrivacyClass
        {
            public string Filename;
            public IEnumerable<string> Usernames;
            public string UsernamesString;
            public string RolesString;
            public bool Public;
            public string By;
            public IEnumerable<string> PRoles;
            public string Privacy;

            private static void GetFileIDs(string[] filenames, string dir, int Projectid, string[] usernames, string username, bool pub)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, Projectid))
                {
                    if (filenames.Count() > 0 && usernames.Count() > 0)
                    {
                        using (boxEntities box = new boxEntities())
                        {
                            // seperate folders and files
                            List<string> Files = new List<string>();
                            List<string> Folders = new List<string>();
                            foreach (var t in filenames)
                            {
                                var r = t.Split('.');
                                if (string.IsNullOrEmpty(r[1]))
                                {
                                    Folders.Add(dir + "/" + t);
                                }
                                else
                                {
                                    Files.Add(dir + "/" + t);
                                }
                            }

                            var FileIDs = (from o in box.ProjectFiles
                                           where o.ProjectID == Projectid && (Files.Contains(o.FullName) || Folders.Contains(o.Directory))
                                           select o.ID).ToList();
                            // get all the privacy objects
                            var privacy = (from o in box.ProjectPrivacies.Include("ProjectUser")
                                           where FileIDs.Contains(o.ProjectFileID) && o.ProjectFile.ProjectID == Projectid
                                           select o).ToList();
                            foreach (var p in privacy)
                            {
                                if (pub)
                                {
                                    p.CanSee = true;
                                }
                                else
                                {
                                    if (usernames.Contains(p.ProjectUser.UserProfile.UserName))
                                    {
                                        p.CanSee = true;
                                    }
                                    else
                                    {
                                        p.CanSee = false;
                                    }
                                }
                            }

                            box.SaveChanges();
                        }
                    }
                }
            }


            public static void SetProjectPrivacyPublic(int projectID, string[] filenames, string dir, bool pub, string username)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, projectID))
                {
                    if (pub)
                    {
                        if (filenames.Count() > 0)
                        {
                            using (boxEntities box = new boxEntities())
                            {
                                // seperate folders and files
                                List<string> Files = new List<string>();
                                List<string> Folders = new List<string>();
                                foreach (var t in filenames)
                                {
                                    var r = t.Split('.');
                                    if (string.IsNullOrEmpty(r[1]))
                                    {
                                        Folders.Add(dir + "/" + t);
                                    }
                                    else
                                    {
                                        Files.Add(dir + "/" + t);
                                    }
                                }

                                var DBFiles = (from o in box.ProjectFiles
                                               where o.ProjectID == projectID && (Files.Contains(o.FullName) || Folders.Contains(o.Directory))
                                               select o).ToList();
                                var FileIDs = DBFiles.Select(a => a.ID).ToList();
                                foreach (var f in DBFiles)
                                {
                                    f.Privacy = "Public";
                                }
                                // get all the privacy objects
                                var privacy = (from o in box.ProjectPrivacies.Include("ProjectUser")
                                               where FileIDs.Contains(o.ProjectFileID) && o.ProjectFile.ProjectID == projectID
                                               select o).ToList();
                                foreach (var p in privacy)
                                {
                                    p.CanSee = true;
                                }

                                box.SaveChanges();
                            }
                        }
                    }
                }
            }
            public static void SetProjectPrivacyByUsers(int projectID, string[] filenames, string dir, string[] usernames, string username)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, projectID))
                {
                    if (filenames.Count() > 0)
                    {
                        using (boxEntities box = new boxEntities())
                        {
                            // seperate folders and files
                            List<string> Files = new List<string>();
                            List<string> Folders = new List<string>();
                            foreach (var t in filenames)
                            {
                                var r = t.Split('.');
                                if (string.IsNullOrEmpty(r[1]))
                                {
                                    Folders.Add(dir + "/" + t);
                                }
                                else
                                {
                                    Files.Add(dir + "/" + t);
                                }
                            }

                            var DBFiles = (from o in box.ProjectFiles
                                           where o.ProjectID == projectID && (Files.Contains(o.FullName) || Folders.Contains(o.Directory))
                                           select o).ToList();
                            var FileIDs = DBFiles.Select(a => a.ID).ToList();
                            foreach (var f in DBFiles)
                            {
                                f.Privacy = "Private.User";
                            }
                            // get all the privacy objects
                            var privacy = (from o in box.ProjectPrivacies.Include("ProjectUser")
                                           where FileIDs.Contains(o.ProjectFileID) && o.ProjectFile.ProjectID == projectID
                                           select o).ToList();
                            foreach (var p in privacy)
                            {
                               
                                    if (usernames.Contains(p.ProjectUser.UserProfile.UserName))
                                    {
                                        p.CanSee = true;
                                    }
                                    else
                                    {
                                        p.CanSee = false;
                                    }
                                
                            }

                            box.SaveChanges();
                        }
                    }
                }
            }
            public static void SetProjectPrivacyByRoles(int projectID, string[] filenames, string dir, string[] roles, string username)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, projectID))
                {
                    if (filenames.Count() > 0)
                    {
                        using (boxEntities box = new boxEntities())
                        {
                            // seperate folders and files
                            List<string> Files = new List<string>();
                            List<string> Folders = new List<string>();
                            foreach (var t in filenames)
                            {
                                var r = t.Split('.');
                                if (r.Length == 1)
                                {
                                    Folders.Add(dir + "/" + t);
                                }
                                else
                                {
                                    Files.Add(dir + "/" + t);
                                }
                            }

                            var DBFiles = (from o in box.ProjectFiles
                                           where o.ProjectID == projectID && (Files.Contains(o.FullName) || Folders.Contains(o.Directory))
                                           select o).ToList();
                            List<int> FileIDs = new List<int>();
                            foreach (var f in DBFiles)
                            {
                                f.Privacy = "Private.Role";
                                FileIDs.Add(f.ID);
                            }
                            // get all the privacy objects
                            var privacy = (from o in box.ProjectPrivacies.Include("ProjectUser")
                                           where FileIDs.Contains(o.ProjectFileID) && o.ProjectFile.ProjectID == projectID
                                           select o).ToList();
                            foreach (var p in privacy)
                            {
                                if (roles.Contains(p.ProjectUser.Role.ToLower()))
                                {
                                    p.CanSee = true;
                                }
                                else
                                {
                                    p.CanSee = false;
                                }
                            }

                            box.SaveChanges();
                        }
                    }
                }
            }
            public static ProjectPrivacyClass GetProjectFilePrivacy(int projectID, string dir, string username, string filename)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, projectID))
                {
                    dir = FormatPath(dir);
                    using (boxEntities box = new boxEntities())
                    {
                        // get the file
                        var result = (from o in box.ProjectFiles
                                      where o.ProjectID == projectID && o.Directory == dir && o.FileName == filename
                                      select new ProjectPrivacyClass()
                                      {
                                          Filename = o.FileName,
                                          Privacy = o.Privacy,
                                          Usernames = o.ProjectPrivacies.Where(a => a.CanSee == true).Select(a => a.ProjectUser.UserProfile.UserName),
                                          PRoles = o.ProjectPrivacies.Where(a=>a.CanSee).Select(a => a.ProjectUser.Role.ToLower()).Distinct(),
                                      }).FirstOrDefault();
                        if (result == null)
                        {
                            ProjectPrivacyClass pp = new ProjectPrivacyClass();
                            return pp;
                        }
                        else
                        {
                            result.Public = CheckPublic(result.Privacy);
                            result.By = GetBy(result.Privacy);
                            return result;
                        }
                    }
                }
                else
                {
                    throw new Exception("Not allowed to view privacy settings");
                }
            }

            private static bool CheckPublic(string txt)
            {
                var u = txt.Split('.');
                if (u[0] == "Public")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            private static string GetBy(string txt)
            {
                var u = txt.Split('.');
                if (u[0] == "Public")
                {
                    return "";
                }
                else
                {
                    return u[1];
                }
            }
            private static void FormatPrivacy(string txt, out string pub, out string by)
            {
                var y = txt.Split('.');
                pub = y[0];
                by = y[1];
            }

            // get who can see file
            public static int[] GetProjectFileUserIDs(int fileid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.ProjectPrivacies
                                  where o.ProjectFileID == fileid && o.CanSee == true
                                  select o.ProjectUser.UserID).ToArray();
                    return result;
                }
            }
            public static string[] GetProjectFileUsernames(int fileid)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.ProjectPrivacies
                                  where o.ProjectFileID == fileid && o.CanSee == true
                                  select o.ProjectUser.UserProfile.UserName).ToArray();
                    return result;
                }
            }
            public static string[] GetProjectFileUsernames(int projectID, string fullpath)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.ProjectPrivacies
                                  where o.ProjectFile.FullName == fullpath && o.ProjectFile.ProjectID == projectID && o.CanSee == true
                                  select o.ProjectUser.UserProfile.UserName).ToArray();
                    return result;
                }
            }
        }

        public class ProjectFileClass
        {
            public static void SaveProjectFile(int projectID, string dir, string fileName, string username)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Manage, projectID))
                {
                    using (TransactionScope sc = new TransactionScope())
                    using (boxEntities box = new boxEntities())
                    {
                        var id = Profile.getUserID(username);
                        var d = dir.Split('/');
                        ProjectFile file = new ProjectFile();
                        file.FileName = fileName;
                        file.ProjectID = projectID;
                        file.Directory = dir;
                        file.UpFolder = d[d.Length - 1];
                        file.DateAdded = DateTime.Now;
                        file.Privacy = "Private.Role";
                        file.FullName = dir + "/" + fileName;
                        file.Creator = box.ProjectUsers.Where(a => a.ProjectID == projectID && a.UserProfile.UserName == username).Select(a => a.ID).FirstOrDefault();

                        box.ProjectFiles.AddObject(file);
                        box.SaveChanges();
                        // save privacy settings
                        var users = (from o in box.ProjectUsers
                                     where o.ProjectID == projectID
                                     select o).ToList();
                        foreach (var u in users)
                        {
                            ProjectPrivacy pri = new ProjectPrivacy();
                            pri.ProjectFileID = file.ID;
                            pri.ProjectUserID = u.ID;
                            if (file.Creator == u.ID)
                            {
                                pri.CanSee = true;
                            }
                            else
                            {
                                if (u.Role == ProjectRoles.Admin.ToString())
                                {
                                    pri.CanSee = true;
                                }
                                else
                                {
                                    pri.CanSee = false;
                                }
                            }
                            box.ProjectPrivacies.AddObject(pri);
                        }
                        // last
                        box.SaveChanges();
                        sc.Complete();
                    }
                }
                else
                {
                    throw new Exception("You don't have the required previliges to upload files");
                }
            }
            public static int GetFileID(int ProjectID, string FilePath)
            {
                using (boxEntities box = new boxEntities())
                {
                    var result = (from o in box.ProjectFiles
                                  where o.FullName == FilePath && o.ProjectID == ProjectID
                                  select o.ID).FirstOrDefault();
                    if (result != 0)
                    {
                        return result;
                    }
                    else
                    {
                        throw new InvalidDataException("File information could not be found");
                    }
                }
            }
            public static void DeletProjectFile(int ProjectID, string FilePath)
            {
                using (boxEntities box = new boxEntities())
                {
                    var file = (from o in box.ProjectFiles
                                where o.ProjectID == ProjectID && o.FullName == FilePath
                                select o).FirstOrDefault();
                    if (file != null)
                    {
                        // get fileprivacies and delete them
                        var privacy = (from o in box.ProjectPrivacies
                                       where o.ProjectFileID == file.ID
                                       select o).ToList();

                        foreach (var p in privacy)
                        {
                            box.ProjectPrivacies.DeleteObject(p);
                        }

                        box.ProjectFiles.DeleteObject(file);
                        box.SaveChanges();
                    }
                }
            }

        }

        public class ProjectRolesClass
        {


            public static bool chkModerator(string username)
            {
                using (boxEntities box = new boxEntities())
                {
                    var user = (from o in box.ProjectUsers
                                where o.UserProfile.UserName == username
                                select o.Role).FirstOrDefault();
                    if (!string.IsNullOrEmpty(user))
                    {
                        if (user == ProjectRoles.Moderator.ToString())
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            public static bool chkCommentor(string username)
            {
                using (boxEntities box = new boxEntities())
                {
                    var user = (from o in box.ProjectUsers
                                where o.UserProfile.UserName == username
                                select o.Role).FirstOrDefault();
                    if (!string.IsNullOrEmpty(user))
                    {
                        if (user == ProjectRoles.Commentor.ToString())
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            public static bool chkEditor(string username)
            {
                using (boxEntities box = new boxEntities())
                {
                    var user = (from o in box.ProjectUsers
                                where o.UserProfile.UserName == username
                                select o.Role).FirstOrDefault();
                    if (!string.IsNullOrEmpty(user))
                    {
                        if (user == ProjectRoles.Editor.ToString())
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

        }

        public class ProjectTaskClass : TaskClass
        {
            public int ProjectFileID;

            public static List<ProjectTaskClass> GetProjectFileTasks(string username, string fileFullName, int projectID)
            {
                using (boxEntities box = new boxEntities())
                {
                    // user id 
                    int userid = Profile.GetUserID(username);
                    var result = new List<ProjectTaskClass>();
                    // check the user role if not creator then get the assigned to tasks only
                    if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Manage, projectID))
                    {
                        result = (from o in box.TaskAssigns.Include("TaskUsers").Include("UserProfile")
                                  where o.ProjectFile.ProjectID == projectID && o.ProjectFile.FullName == fileFullName
                                  select new ProjectTaskClass()
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
                                  where o.TaskAssign.ProjectFile.FullName == fileFullName && o.TaskAssign.ProjectFile.ProjectID == projectID && o.UserID == userid
                                  select new ProjectTaskClass()
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
            } // done

            public static ProjectTaskClass SaveProjectFileTask(string username, int projectID, string title, string desc, DateTime dueDate, string[] usernames, string fileFullname)
            {
                ProjectTaskClass rObject = new ProjectTaskClass();

                using (boxEntities box = new boxEntities())
                {
                    usernames = usernames.Distinct().ToArray();


                    if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Manage, projectID))
                    {
                        // get projectfile id
                        var fileid = (from o in box.ProjectFiles
                                      where o.ProjectID == projectID && o.FullName == fileFullname
                                      select o.ID).FirstOrDefault();
                        if (fileid != 0)
                        {


                            // create the header
                            TaskAssign task = new TaskAssign();
                            task.ProjecFileID = fileid;
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
                                users = ProjectPrivacyClass.GetProjectFileUserIDs(fileid);
                                na = ProjectPrivacyClass.GetProjectFileUsernames(fileid);
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
                            rObject.ProjectFileID = fileid;
                            rObject.Usernames = na;
                            rObject.Creator = username;

                            return rObject;
                        }
                        else
                        {
                            throw new FileNotFoundException();
                        }
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }



                }

            } // done

            public static TaskClass UpdateProjectFileTask(int taskID, string username, int projectID, string title, string desc, DateTime dueDate, string[] usernames)
            {
                using (boxEntities box = new boxEntities())
                {
                    usernames = usernames.Distinct().ToArray();
                    // get creator username
                    var creator = (from o in box.TaskAssigns
                                  where o.TaskID == taskID
                                  select o.UserProfile.UserName).FirstOrDefault();
                    

                    if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, projectID) || creator == username)
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
                            int t = task.ProjecFileID == null ? 0 : (int)task.ProjecFileID;
                            NewUsers = ProjectPrivacyClass.GetProjectFileUserIDs(t);
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
                        TaskClass tt = new TaskClass();
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

            } // done
        }
    }
}
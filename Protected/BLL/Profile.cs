using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;


namespace TheBox.Protected.BLL
{

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
    public class Profile
    {
        private string firstName { get; set; }

        public bool CreateUser(string fname, string lname, string uname, string email, string position)
        {
            try
            {
                using (boxEntities box = new boxEntities())
                {
                    UserProfile user = new UserProfile()
                    {
                        UserName = uname,
                        LastName = lname,
                        FirstName = fname,
                        Email = email,
                        Position = position
                    };
                    box.UserProfiles.AddObject(user);
                    box.SaveChanges();
                }
                return true;
            }
            catch
            {
                // delete the user from the aspnet
                Membership.DeleteUser(uname, true);
                return false;
            }

        }

        internal static UserProfile GetUser(string username)
        {
            UserProfile re;
            using (boxEntities box = new boxEntities())
            {
                re = (from o in box.UserProfiles
                      where o.UserName == username
                      select o).FirstOrDefault();

            }
            return re;
        }

        internal static int GetUserID(string username)
        {
            UserProfile re;
            using (boxEntities box = new boxEntities())
            {
                re = (from o in box.UserProfiles
                      where o.UserName == username
                      select o).FirstOrDefault();

            }
            return re.UserID;
        }
        internal static int[] GetUserIDs(string[] username)
        {
            int[] re;
            using (boxEntities box = new boxEntities())
            {
                re = (from o in box.UserProfiles
                      where username.Contains(o.UserName)
                      select o.UserID).ToArray();

            }
            return re;
        }

        internal static bool EditUser(string fname, string lname, string position, string email, string uname)
        {
            using (boxEntities box = new boxEntities())
            {
                var re = (from o in box.UserProfiles
                          where o.UserName == uname
                          select o).FirstOrDefault();
                if (re != null)
                {
                    re.FirstName = fname;
                    re.LastName = lname;
                    re.Email = email;
                    re.Position = position;
                    box.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        internal static List<UserProfile> GetAllUsers()
        {
            List<UserProfile> result;
            using (boxEntities box = new boxEntities())
            {
                result = (from o in box.UserProfiles
                          select o).ToList();
            }
            return result;
        }

        internal static bool DeleteUser(string username)
        {
            try
            {
                using (boxEntities box = new boxEntities())
                {
                    var user = (from o in box.UserProfiles
                                where o.UserName == username
                                select o).FirstOrDefault();
                    if (user != null)
                    {
                        box.UserProfiles.DeleteObject(user);
                        box.SaveChanges();
                    }
                }
                // delete from membership
                Membership.DeleteUser(username, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static UserProfile getUser(string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var user = (from o in box.UserProfiles
                            where o.UserName == username
                            select o).FirstOrDefault();
                if (user != null)
                {
                    return user;
                }
                else
                {
                    throw new Exception(username + " does not exist in the system");
                }
            }
        }
        public static int getUserID(string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var user = (from o in box.UserProfiles
                            where o.UserName == username
                            select o.UserID).FirstOrDefault();
                if (user != 0)
                {
                    return user;
                }
                else
                {
                    throw new Exception(username + " does not exist in the system");
                }


            }

        }

        internal static string getUserEmail(string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var user = (from o in box.UserProfiles
                            where o.UserName == username
                            select o.Email).FirstOrDefault();
                if (user != "" && user != null)
                {
                    return user;
                }
                else
                {
                    throw new Exception(username + " does not have email");
                }


            }
        }
    }
    public enum ProjectPrivilages
    {
        Create,
        Manage,
        Comment,
        ControlSetting,
    }
    public class Roles
    {
        internal static bool ChkAdmin(string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.UserProfiles
                              where o.UserName == username
                              select o.Position).FirstOrDefault();
                if (result != null || result != "")
                {
                    if (result == Positions.Admin.ToString())
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
                    return false;
                }
            }
        }
        internal static string GetProfileRole(string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var role = (from o in box.UserProfiles
                            where o.UserName == username
                            select o.Position).FirstOrDefault();
                return role;
            }
        }
        internal static string GetProjectRole(string username, int projectID)
        {
            using (boxEntities box = new boxEntities())
            {
                var role = (from o in box.ProjectUsers
                            where o.ProjectID == projectID && o.UserProfile.UserName == username
                            select o.Role).FirstOrDefault();
                return role;
            }
        }
        internal static string GetRole(string username, int projectid)
        {
            var UserRole = Roles.GetProjectRole(username, projectid);
            if (string.IsNullOrEmpty(UserRole))
            {
                UserRole = Roles.GetProfileRole(username);
            }
            return UserRole;         
        }
        public static bool UserIsAllowed_Project(string username, ProjectPrivilages p, int projectID)
        {
            var role = GetProjectRole(username, projectID);
            if (string.IsNullOrEmpty(role))
            {
                role = GetProfileRole(username);
            }

            var strP = p.ToString();
            switch (strP)
            {
                case "Create":
                    if (role == ProjectRoles.Admin.ToString())
                    { return true; }
                    else
                        return false;

                case "Manage":
                    if (role == ProjectRoles.Admin.ToString() || role == ProjectRoles.Editor.ToString() || role == ProjectRoles.Moderator.ToString())
                        return true;
                    else
                        return false;

                case "Comment":
                    if (role == ProjectRoles.Commentor.ToString())
                        return true;
                    else
                        return false;
                case "ControlSetting":
                    if (role == ProjectRoles.Admin.ToString() || role == ProjectRoles.Moderator.ToString())
                        return true;
                    else
                        return false;
            }


            return false;


        }
        public static bool UserIsAllowed_Project(string username, ProjectPrivilages p, int projectID, string role)
        {
            var strP = p.ToString();
            switch (strP)
            {
                case "Create":
                    if (role == ProjectRoles.Admin.ToString())
                    { return true; }
                    else
                        return false;

                case "Manage":
                    if (role == ProjectRoles.Admin.ToString() || role == ProjectRoles.Editor.ToString() || role == ProjectRoles.Moderator.ToString())
                        return true;
                    else
                        return false;

                case "Comment":
                    if (role == ProjectRoles.Commentor.ToString() || role == ProjectRoles.Admin.ToString() || role == ProjectRoles.Editor.ToString() || role == ProjectRoles.Moderator.ToString())
                        return true;
                    else
                        return false;
                case "ControlSetting":
                    if (role == ProjectRoles.Admin.ToString() || role == ProjectRoles.Moderator.ToString())
                        return true;
                    else
                        return false;
            }


            return false;


        }
    }
    public enum Positions
    {
        Admin,
        Staff
    }
}
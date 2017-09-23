using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheBox.Protected.BLL
{
    public class TaskClass
    {

        public string Title;
        public string Desc;
        public int TaskID;
        public DateTime DueDate;
        public DateTime CreatedDate;
        public string[] Usernames;
        public string Creator;
        public class TaskResponse
        {
            public string Desc;
            public string Response;
            public DateTime? ResponseDate;
            public string Username;
        }

        
        public static TaskClass GetTask(int TaskID)
        {
            using (boxEntities box = new boxEntities())
            {

                var result = (from o in box.TaskAssigns.Include("TaskUsers")
                              where o.TaskID == TaskID
                              select new TaskClass()
                              {
                                  Title = o.TaskTitle,
                                  Desc = o.TaskDesc,
                                  TaskID = o.TaskID,
                                  DueDate = o.DueDate
                              }).FirstOrDefault();
                if (result != null)
                {
                    result.Usernames = box.TaskUsers.Where(a => a.TaskID == result.TaskID).Select(a => a.UserProfile.UserName).ToArray();
                    return result;
                }
                else
                {
                    throw new Exception();
                }


            }
        }
        public static bool DeleteTask(int taskID, string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var task = (from o in box.TaskAssigns.Include("TaskUsers")
                            where o.TaskID == taskID
                            select o).FirstOrDefault();
                task.TaskUsers.Clear();
                box.TaskAssigns.DeleteObject(task);
                box.SaveChanges();
                return true;

            }
        }
        // Task Response // updae and save
        public static bool SaveTaskResponse(string username, int taskID, string desc, string response)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.TaskUsers
                              where o.UserProfile.UserName == username && o.TaskID == taskID
                              select o).FirstOrDefault();
                if (result != null)
                {
                    result.TDesc = desc;
                    result.Response = response;
                    result.ResponseDate = DateTime.Now;

                    box.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static List<TaskResponse> GetAllTaskResponse(int taskID)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.TaskUsers
                              where o.TaskID == taskID
                              select new TaskResponse()
                              {
                                  Response = o.Response,
                                  ResponseDate = o.ResponseDate,
                                  Username = o.UserProfile.UserName,
                                  Desc = o.TDesc
                              }).ToList();
                return result;
            }
        }
        public static TaskResponse GetUserTaskResponse(int taskID, string username)
        {
            using (boxEntities box = new boxEntities())
            {
                var result = (from o in box.TaskUsers
                              where o.TaskID == taskID && o.UserProfile.UserName == username
                              select new TaskResponse()
                              {
                                  Response = o.Response,
                                  ResponseDate = o.ResponseDate,
                                  Username = o.UserProfile.UserName,
                                  Desc = o.TDesc
                              }).FirstOrDefault();
                return result;

            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheBox.Protected.BLL
{
    public class CommentBll
    {
        public string Username { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; }
        public TimeSpan time { get; set; }

        //public void insertComment(string username, string fileDirectory, string comment)
        //{
        //    using (boxEntities box = new boxEntities())
        //    {
        //        var user = (from o in box.UserProfiles
        //                    where o.UserName == username
        //                    select o.UserID).FirstOrDefault();
        //        var file = (from o in box.OFiles
        //                    where o.Directory == fileDirectory
        //                    select o.FileID).FirstOrDefault();
        //        if (user != 0 && file != 0)
        //        {
        //            Comment c = new Comment();
        //            c.Text = comment;
        //            c.FileID = file;
        //            c.UserID = user;
        //            c.Date = DateTime.Now;

        //            box.Comments.AddObject(c);
        //            box.SaveChanges();

        //            // inform watcher
        //            Watcher.CreatCommentNotification(username, fileDirectory);
        //        }
        //    }
        //}

        //public List<CommentBll> getComments(string file)
        //{
        //    List<CommentBll> groupComments = new List<CommentBll>();
        //    using (boxEntities box = new boxEntities())
        //    {
        //        var f = (from o in box.OFiles
        //                 where o.Directory == file
        //                 select o.FileID).FirstOrDefault();
        //        if (f != 0)
        //        {
        //            var result = (from o in box.Comments
        //                         where o.FileID == f
        //                         select o).ToList();
                    
        //            foreach (var t in result)
        //            {
        //                CommentBll bll = new CommentBll();
        //                bll.date = t.Date;
        //                bll.Username = t.UserProfile.UserName;
        //                bll.text = t.Text;
        //                groupComments.Add(bll);
        //            }
        //        }
        //     }
        //    return groupComments;
        //}
    }
}
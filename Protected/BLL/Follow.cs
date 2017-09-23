using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheBox.Protected.BLL
{
    public class Follow
    {
        //public void insertFollow(string fileDir, string username)
        //{
        //    using (boxEntities box = new boxEntities())
        //    {
        //        // get user id
        //        int user;
        //        int file;
        //        getInfo(fileDir, username, box, out user, out file);

        //        // save into 

        //        if (user == 0 || file == 0)
        //        {
        //            // throw exception
        //        }
        //        else
        //        {
        //            Follower f = new Follower();
        //            f.FollowerID = user;
        //            f.FileID = file;

        //            box.Followers.AddObject(f);
        //            box.SaveChanges();
        //        }
        //    }

        //}

        //private static void getInfo(string fileDir, string username, boxEntities box, out int user, out int file)
        //{
        //    user = (from o in box.UserProfiles
        //            where o.UserName == username
        //            select o.UserID).FirstOrDefault();
        //    file = (from o in box.Files
        //            where o.Directory == fileDir
        //            select o.FileID).FirstOrDefault();
        //}


        //public void unfollow(string fileDir, string username)
        //{
        //    using (boxEntities box = new boxEntities())
        //    {
        //        // get user id
        //        int user;
        //        int file;
        //        getInfo(fileDir, username, box, out user, out file);

        //        // save into 

        //        if (user == 0 || file == 0)
        //        {
        //            // throw exception
        //        }
        //        else
        //        {
        //            var result = (from o in box.Followers
        //                         where o.FileID == file && o.FollowerID == user
        //                         select o).FirstOrDefault();
        //            if (result != null)
        //            {
        //                box.Followers.DeleteObject(result);
        //                box.SaveChanges();
        //            }
        //        }
        //    }
        //}


        //public List<Follower> getFollowers(string fileDir)
        //{
        //    // get the file
        //    using (boxEntities box = new boxEntities())
        //    {
        //        List<Follower> followers = new List<Follower>();
        //        var file = (from o in box.Files
        //                    where o.Directory == fileDir
        //                    select o.FileID).FirstOrDefault();
        //        if (file != 0)
        //        {
        //            followers = (from o in box.Followers
        //                             where o.FileID == file
        //                             select o).ToList();
        //        }
        //        return followers;
        //    }

        //}
    }
}
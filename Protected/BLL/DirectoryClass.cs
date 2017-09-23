using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;

namespace TheBox.Protected.BLL
{
    //public class DirectoryClass

    public class DirectoryFileClass 
        {
            public static void SaveFile( string dir, string[] fileNames, string username, bool locked, string privacy, string[] usernames)
            {
               
                    using (TransactionScope sc = new TransactionScope())
                    using (boxEntities box = new boxEntities())
                    {
                        
                            if (privacy == "Global")
                            {
                                dir = ProjectClass.GetConfigString("GlobalDir", "Global");
                            }
                            var id = Profile.getUserID(username);
                            var d = dir.Split('/');
                            foreach (var t in fileNames)
                            {
                            DFile file = new DFile();
                            file.Filename = t;
                            file.Directory = dir;
                            file.UpFolder = d[d.Length - 1];
                            file.DateAdded = DateTime.Now;
                            file.Privacy = privacy;
                            file.FullName = dir + "/" + t;
                            file.Locked = locked;
                            file.Creator = id;

                            box.DFiles.AddObject(file);


                            if (privacy == "Public")
                            {
                                foreach (var user in usernames)
                                {
                                    FilePrivacy fileP = new FilePrivacy();
                                    fileP.UserID = id;
                                    fileP.FileID = file.ID;
                                    fileP.CanSee = true;

                                    box.FilePrivacies.AddObject(fileP);
                                }

                                box.SaveChanges();
                            }
                            }
                        box.SaveChanges();

                        // save file in the 
                        sc.Complete();
                    }
            }

            public static void DeleteFile(string dir)
            {
                using (boxEntities box = new boxEntities())
                {//linq
                    var file = (from o in box.DFiles
                               where o.FullName == dir
                               select o).FirstOrDefault();
                    var pri = (from p in box.FilePrivacies
                               where p.FileID == file.ID
                               select p).ToList();
                    foreach (var item in pri)
                    {
                        box.FilePrivacies.DeleteObject(item);
                    }
                    box.DFiles.DeleteObject(file);
                    box.SaveChanges();
                
                }
            }

        }
    }

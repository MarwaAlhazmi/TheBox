using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace TheBox.Protected.BLL
{
    public class Dir
    {
        public static string createTempDir(string path)
        {
            string name = "temp";
            try
            {
                DirectoryInfo t = Directory.CreateDirectory(path +"\\"+ name);
                if (t != null)
                {
                    return t.FullName;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }

        }

        public static bool deleteTempDir(string path)
        {
            string name = "temp";
            try
            {
                Directory.Delete(path + "\\" + name, true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
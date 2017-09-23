using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;

namespace TheBox.Protected.BLL
{
    public struct fileData
    {
        public string fileName;
        public string fileSize;
        public string fileCreator;
    }
    public class BoxFile
    {
        public DirectoryInfo directory
        {
            get;
            set;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dir"></param>
        public BoxFile(DirectoryInfo dir)
        {
            directory = dir;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentDir"></param>
        /// <param name="currentNode"></param>
        public void GetDirectiries(DirectoryInfo currentDir, TreeNode currentNode)
        {

            //loop through each sub-directory in the current one

            foreach (DirectoryInfo dir in currentDir.GetDirectories())
            {

                //create node and add to the tree view

                TreeNode node = new TreeNode(dir.Name, dir.FullName);

                currentNode.ChildNodes.Add(node);

                //recursively call same method to go down the next level of the tree

                GetDirectiries(dir, node);
            }

            foreach (FileInfo File in currentDir.GetFiles())
            {

                //create node and add to the tree view

                TreeNode node = new TreeNode(File.Name, File.FullName);

                currentNode.ChildNodes.Add(node);
            }
        }

        public string createTempDir(string path)
        {
            string name = "temp";
            try
            {
                DirectoryInfo t = Directory.CreateDirectory(path + "\\" + name);
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

        public bool deleteTempDir(string path)
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

        public void getDirInfo(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
        }

        public fileData getFileInfo(string filePath)
        {
            FileInfo i = new FileInfo(filePath);
            fileData d = new fileData();
            d.fileName = i.Name;
            d.fileSize = (i.Length/1000000.00).ToString("0.000") + " MB";
            d.fileCreator = i.CreationTime.ToShortDateString();

            return d;
        }

        public bool createDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                if (Directory.CreateDirectory(path) != null)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
    }
}
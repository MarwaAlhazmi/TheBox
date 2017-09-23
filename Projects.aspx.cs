using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using TheBox.Protected.Tool;
using TheBox.Protected.BLL;
using System.Configuration;
using System.Web.Services;
using System.DirectoryServices;
using System.Web.Script.Services;

namespace TheBox
{
    public partial class Projects : System.Web.UI.Page
    {
        private static BoxFile box;
        private static string selected;
        private static string path;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                path = ConfigurationManager.AppSettings["ProjectPath"];
                BuildTree(path, TreeView1);
                //TreeView1.DataBind();
                // intilize the boxfile
            }
        }

        private void BuildTree(string ff, TreeView view)
        {
            box = new BoxFile(new DirectoryInfo(ff));
            //get root directory
            // TODO: change the directory
            DirectoryInfo rootDir = new DirectoryInfo(ff);
            TreeNode rootNode = new TreeNode(rootDir.Name, rootDir.FullName);

            view.Nodes.Add(rootNode);

            //begin recursively traversing the directory structure

            box.GetDirectiries(rootDir, rootNode);

        }





        protected void TreeView1_SelectedNodeChanged(object sender, EventArgs e)
        {
            if (TreeView1.SelectedNode.ChildNodes.Count > 0)
            {
                string t = TreeView1.SelectedNode.Value;
                tvFiles.Nodes.Clear();
                BuildTree(t, tvFiles);
                tvFiles.Nodes[0].Expand();
                //te.Expand();
                //tvFiles.Nodes.Add(te);
                //TreeView1.Nodes.Add(te);
                TabContainer1.ActiveTabIndex = 1;
            }
            else
            {
                getFile(TreeView1);
                //// solve if the folder has no files
                //WordXLS conv = new WordXLS();
                ////lblText.Text = "FileName :" + DocViewTreeView.SelectedNode.Text;
                //string strFilePath = path;
                //string target = box.createTempDir(path);
                //string source = Path.GetDirectoryName(TreeView1.SelectedNode.Value) + "\\";
                ////ConfigurationManager.AppSettings["ConvertLocation"].ToString().Trim();
                //string strFile = TreeView1.SelectedNode.Text.ToString();
                //string strExtension = TreeView1.SelectedNode.Text.ToString().Split
                //        ('.')[1].ToString().Trim().ToUpper();
                //string strUrl = TreeView1.SelectedNode.Value;

                //if (strExtension == "DOCX" || strExtension == "DOC")
                //{
                //    conv.WordToHTML(source + strFile, target + strFile.Split('.')[0] + ".html");
                //    if (conv.Status == WordXLS.StatusType.SUCCESS)
                //        docPreview.Attributes["src"] = strUrl + strFile.Split('.')[0] + ".html";
                //}
                //else if (strExtension == "XLS" || strExtension == "XLSX")
                //{
                //    conv.ExcelToHTML(source + strFile, strFilePath + strFile.Split('.')[0] + ".html");
                //    if (conv.Status == WordXLS.StatusType.SUCCESS)
                //        docPreview.Attributes["src"] = strUrl.Split('.')[0] + ".html";
                //}
                //else
                //{
                //    docPreview.Attributes["src"] = strUrl;
                //    getfileinfo(TreeView1.SelectedNode.Value);
                //}
                ////Response.Redirect(Server.MapPath("~/Test/test.txt"));
                ////viewer.Attributes["src"] = TreeView1.SelectedNode.Value;

                //// get the comments on this file
                //selected = TreeView1.SelectedNode.Value;
                //getComments(TreeView1.SelectedNode.Value);
            }

        }


        protected void tvFiles_SelectedNodeChanged(object sender, EventArgs e)
        {
           
            if (tvFiles.SelectedNode.ChildNodes.Count == 0)
            {
                getFile(tvFiles);
            //    // solve if the folder has no files
            //    WordXLS conv = new WordXLS();
            //    //lblText.Text = "FileName :" + DocViewTreeView.SelectedNode.Text;
            //    string strFilePath = Server.MapPath("~/Test/");
            //    //ConfigurationManager.AppSettings["ConvertLocation"].ToString().Trim();
            //    string strFile = tvFiles.SelectedNode.Text.ToString();
            //    string strExtension = tvFiles.SelectedNode.Text.ToString().Split
            //            ('.')[1].ToString().Trim().ToUpper();
            //    string strUrl = "http://" + Request.Url.Authority + "/Test/";

            //    if (strExtension == "DOCX" || strExtension == "DOC")
            //    {
            //        conv.WordToHTML(strFilePath + strFile, strFilePath + strFile.Split('.')[0] + ".html");
            //        if (conv.Status == WordXLS.StatusType.SUCCESS)
            //            docPreview.Attributes["src"] = strUrl + strFile.Split('.')[0] + ".html";
            //    }
            //    else if (strExtension == "XLS" || strExtension == "XLSX")
            //    {
            //        conv.ExcelToHTML(strFilePath + strFile, strFilePath + strFile.Split('.')[0] + ".html");
            //        if (conv.Status == WordXLS.StatusType.SUCCESS)
            //            docPreview.Attributes["src"] = strUrl + strFile.Split('.')[0] + ".html";
            //    }
            //    else
            //    {
            //        docPreview.Attributes["src"] = strUrl + tvFiles.SelectedNode.Text;

            //    }

            //    //Response.Redirect(Server.MapPath("~/Test/test.txt"));
            //    //viewer.Attributes["src"] = TreeView1.SelectedNode.Value;

            //    selected = tvFiles.SelectedNode.Value;
            //    // get the comments on this file
            //    getComments(tvFiles.SelectedNode.Value);

            }

        }

        private void getFile(TreeView tree)
        {
                // solve if the folder has no files
                WordXLS conv = new WordXLS();
                //lblText.Text = "FileName :" + DocViewTreeView.SelectedNode.Text;
                string strFilePath = Server.MapPath("~/Test/");
                //ConfigurationManager.AppSettings["ConvertLocation"].ToString().Trim();
                string strFile = tree.SelectedNode.Text.ToString();
                string strExtension = tree.SelectedNode.Text.ToString().Split
                        ('.')[1].ToString().Trim().ToUpper();
                string strUrl = "http://" + Request.Url.Authority + "/Test/";

                if (strExtension == "DOCX" || strExtension == "DOC")
                {
                    conv.WordToHTML(strFilePath + strFile, strFilePath + strFile.Split('.')[0] + ".html");
                    if (conv.Status == WordXLS.StatusType.SUCCESS)
                    {
                        docPreview.Attributes["src"] = strUrl + strFile.Split('.')[0] + ".html";
                        getfileinfo(tree.SelectedNode.Value);
                    }
                }
                else if (strExtension == "XLS" || strExtension == "XLSX")
                {
                    conv.ExcelToHTML(strFilePath + strFile, strFilePath + strFile.Split('.')[0] + ".html");
                    if (conv.Status == WordXLS.StatusType.SUCCESS)
                    {
                        docPreview.Attributes["src"] = strUrl + strFile.Split('.')[0] + ".html";
                        getfileinfo(tree.SelectedNode.Value);
                    }
                }
                else
                {
                    docPreview.Attributes["src"] = strUrl + tree.SelectedNode.Text;
                    getfileinfo(tree.SelectedNode.Value);

                }

                //Response.Redirect(Server.MapPath("~/Test/test.txt"));
                //viewer.Attributes["src"] = TreeView1.SelectedNode.Value;

                selected = tree.SelectedNode.Value;
                // get the comments on this file
                getComments(tree.SelectedNode.Value);

            
        }
        private void  getfileinfo(string file)
        {
            fileData data = box.getFileInfo(file);
            filename.InnerHtml = "<b>File name: </b>" + data.fileName;
            size.InnerHtml = "<b>Size: </b>" + data.fileSize;
            creator.InnerHtml = "<b>Craetion time: </b>" + data.fileCreator;
        }

        [WebMethod]
        protected void sendComment()
        {
            // get the name of the current user 
            // get the text of the comment
            string comment = tbComment.Value;
            string user = User.Identity.Name;
            //divComments.InnerHtml += "<p><b>"+user+": </b> "+comment+"</p><hr />";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string text = tbComment.Value;
            string user = User.Identity.Name;
            string dir = selected;
            if (!string.IsNullOrEmpty(dir))
            {
                CommentBll com = new Protected.BLL.CommentBll();
                //com.insertComment(user, dir, text);
                // update interface
                string markup = @"<div class=""row-fluid""><div class=""tile-listviewitem"" style=""background-color: rgb(238, 238, 238)""><div class=""span2"">" +
                                      @"<div class=""title""><a runat=""server"" id=""aName"" href=""#"">" + user + @"</a></div><div class=""subtitle"">" +
                                      @"<h6>" + DateTime.Now + @"</h6></div></div><div class=""span10""><div class=""detail""><p>" + text + "</p></div></div></div></div><hr/>";
                tilelistviewdemo.InnerHtml += markup;
                tbComment.Value = "";
                // tilelistviewdemo.InnerHtml += "<p><b>" + user + ": </b> " + text + "</p><hr />";
            }
            else
            {
                tbComment.Value = "";
                // show an error message
            }
        }


        private void getComments(string file)
        {
            //tilelistviewdemo.InnerHtml = "";
            //commentsContainer.Visible = true;
            //List<CommentBll> bll = new Protected.BLL.CommentBll().getComments(file);
            //foreach (var m in bll)
            //{

            //    string markup = @"<div class=""row-fluid""><div class=""tile-listviewitem"" style=""background-color: rgb(238, 238, 238)""><div class=""span2"">" +
            //                            @"<div class=""title""><a runat=""server"" id=""aName"" href=""#"">" + m.Username + @"</a></div><div class=""subtitle"">" +
            //                            @"<h6>" + m.date + @"</h6></div></div><div class=""span10""><div class=""detail""><p>" + m.text + "</p></div></div></div></div><hr/>";
            //    tilelistviewdemo.InnerHtml += markup;
            //}
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string createFolder(string name)
        {
            // get the selected node
            return "hello from server: " + name;
            //box.createDirectory(folderName);
            //return "";
        }

        private void getInfo(string file)
        {

        }
    }
}
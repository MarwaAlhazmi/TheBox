using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using TheBox.Protected.Tool;
using TheBox.Protected.BLL;
using System.Web.Services;
using System.Web.Script.Services;
using System.Transactions;
using HtmlAgilityPack;

namespace TheBox
{
    public partial class PProject : System.Web.UI.Page
    {
        public static string replies = @"<div class=""row-fluid replyBox""><div class=""replyContent"" style=""margin-bottom:10px"">
                             <div class=""span2""><b>{0} </b><br />{1}<br />{2}</div>
                             <div class=""span7""><div>{3}</div><hr />{4}</div></div></div>";
        public static string fileReplies = @"<div class=""row-fluid fileReplyBox""><div class=""replyContent"" style=""margin-bottom:10px"">
                             <div class=""span2""><b>{0} </b><br />{1}<br />{2}</div>
                             <div class=""span7""><div>{3}</div><hr />{4}</div></div></div>";
        private static string CreatorTaskTag = @"<div class=""btn-group pull-right""><button class=""btn dropdown-toggle"" style=""background-color: #F5F5F5; border-color:#F5F5F5"" data-toggle=""dropdown"">" +
                 @"<span class=""caret""></span></button><ul aria-labelledby=""dLabel"" role=""menu"" class=""dropdown-menu"">" +
                 @"<li><a id=""{0}"" onclick=""return EditTask(this)"" href=""#"">Edit</a></li><li><a id=""{1}"" onclick=""return DeleteTaskConfirm(this)"" href=""#"">Delete</a></li></ul></div>" +
                 @"<table><tr><td style=""width:100px"">Action:</td><td>{2}</td></tr><tr><td>Describtion</td><td>{3}</td></tr><tr><td>Date Created</td><td>{4}</td></tr>"
                 + @"<tr><td>Due Date</td><td>{5}</td></tr><tr><td>By Users</td><td>{6}</td></tr></table>";

        private static string CreatorTaskTag1 = @"<div class=""btn-group pull-right""><button class=""btn dropdown-toggle"" style=""background-color: #F5F5F5; border-color:#F5F5F5"" data-toggle=""dropdown"">" +
                 @"<span class=""caret""></span></button><ul aria-labelledby=""dLabel"" role=""menu"" class=""dropdown-menu"">" +
                 @"<li><a id=""{0}"" onclick=""return EditTask(this)"" href=""#"">Edit</a></li><li><a id=""{1}"" onclick=""return DeleteTaskConfirm(this)"" href=""#"">Delete</a></li></ul></div>" +
                                                @"<div class=""span1 pull-right"" style=""{8} width: 100px; color: White; text-align: center"">{9}</div><div class=""row-fluid""><div class=""span4""><strong>Action: </strong>"+
                                                @"<span>{2}</span></div><div class=""span3""><strong>Date Created:  </strong>{4}</div><div class=""span3"" {7}><strong>Due Date: </strong>{5}</div></div>"+
                                                @"<div class=""row-fluid""><div class=""span4""><strong>Describtion: </strong> {3}</div><div class=""span7""><strong>By Users: </strong>{6}</div></div>";

        private static string UserTaskTag = @"<table><tr><td style=""width:130px"">Action Required:</td><td>{0}</td></tr><tr><td>Describtion</td><td>{1}</td></tr><tr><td>Date Created</td><td>{2}</td></tr>"
                  + @"<tr><td>Due Date</td><td>{3}</td></tr></table>";

        private static string UserTaskTag1 = @"<div class=""row-fluid""><div class=""span4""><strong>Action: </strong>" +
                                                @"<span>{0}</span></div><div class=""span3""><strong>Date Created:  </strong>{2}</div><div class=""span3""{4}><strong>Due Date: </strong>{3}</div></div>" +
                                                @"<div class=""row-fluid""><div class=""span6""><strong>Describtion: </strong> {1}</div></div>";

        private static string UserResponse = @"<hr/><div><div><b>{0}</b><div class=""pull-right""><b> {1} </b>{2}</div></div><br /><div>{3}</div></div>";
        private static string UserUserResponse = @"<hr/><div><div><b>{0}</b><div class=""pull-right""><b> {1} </b>{2}</div></div><br /><div>{3}</div><a href=""#"" onclick=""return EditUserTaskResponse(this)"" class=""pull-right"" style=""font-size:smaller"" id=""a.{4}"">Edit</a></div>";

        private static string UserUserResponse1 = @"<hr /><div id=""divResponse{4}""><div><b>{0}</b> <i>{2} </i> {3} <div class=""pull-right""><span {5}><b>{1}</b></span></div></div><a href=""#"" onclick=""return EditUserTaskResponse(this)"" class=""pull-right"" style=""font-size:smaller"" id=""a.{4}"">Edit</a></div>";
        private static string UserResponse1 = @"<hr /><div id=""divResponse{5}""><div><b>{0}</b> <i> {2} </i> {3} <div class=""pull-right""><span {4}><b>{1}</b></span></div></div></div>";

        private static string ResponseTool = @"<hr /><div id=""divTool{3}"" class=""span10""><div class=""btn-group""><button class=""btn"" id=""btnTaskResponse{1}"" style=""width: 150px"" onclick=""return false "">{4}</button><button data-toggle=""dropdown"" " +
                                             @"class=""btn dropdown-toggle""><span class=""caret""></span></button><ul class=""dropdown-menu"" role=""menu"" aria-labelledby=""dLabel""><li><a id=""aCompleteAction.{3}"" href=""#"" onclick=""return btnTaskResponseClick(this)"">" +
                                             @"Complete</a></li><li><a href=""#"" id=""aCommentAction.{3}"" onclick=""return btnTaskResponseClick(this)"">Comment</a></li><li><a href=""#"" id=""aDeclineAction.{3}"" onclick=""return btnTaskResponseClick(this)"">Decline Task</a></li></div><br /><textarea id=""textarea{0}"" " +
                                             @"class=""input-block-level"" placeholder=""Insert a comment"" cols="""" rows="""" style=""width:100%; height: 90px"">{5}</textarea><div class=""pull-right""><input type=""button"" id=""btnResponse.{2}"" value=""Send"" class=""btn-primary"" onclick=""SaveTaskResponse(this)"" /></div></div>";

        private static string startAndEnd = @"<div class=""taskBox"" id=""div{0}"">{1}<div class=""row-fluid"">{2} {3} </div></div>";
        //-- path to GIF icon image files (absolute)
        private string _strImagePath;
        //-- hide any files matching pattern
        private string _strHideFilePattern;
        //-- hide any folders matching pattern
        private string _strHideFolderPattern;
        //-- force user to stay on paths matching pattern
        private string _strAllowedPathPattern;
        //-- flush rows to the browser as they are written
        private bool _blnFlushContent;
        private string _loadFile;

        private const string _strIconSize = "height=16 width=16";
        private const string _strRenameTag = "_2_";
        private const string _strCheckboxTag = "checked_";
        private const string _strActionTag = "action";
        private const string _strWebPathTag = "path";
        private const string _strColSortTag = "sort";

        private const string _strTargetFolderTag = "targetfolder";

        private Exception _FileOperationException;

        private static int ProjectID;
        private string UserRole;
        private static List<string> SaveFileTypes = new List<string>() {"DOC", "DOCX", "XLS" , "XLSX", "PPT", "PPTX", "TXT","XML", "CSS","HTML", "HTM"};
        /// <summary>
        /// tag used to seperate old filename from new filename for renamed files
        /// </summary>

        public string RenameTag
        {
            get { return _strRenameTag; }
        }
        /// <summary>
        /// tag used to indicate the action field in the form
        /// </summary>
        public string ActionTag
        {
            get { return _strActionTag; }
        }

        /// <summary>
        /// tag used to indicate file checkboxes in the form
        /// </summary>
        public string CheckboxTag
        {
            get { return _strCheckboxTag; }
        }

        /// <summary>
        /// tag used to indicate the target folder field in the form
        /// </summary>
        public string TargetFolderTag
        {
            get { return _strTargetFolderTag; }
        }

        /// <summary>
        /// returns the current web path being browsed
        /// </summary>
        public string CurrentWebPath
        {
            get { return WebPath(); }
        }

        /// <summary>
        /// returns the current script filename (.aspx)
        /// </summary>
        public string ScriptName
        {
            get { return Request.ServerVariables["script_name"]; }
        }
        public string loadFile
        {
            get { return _loadFile; }
        }
        /// <summary>
        /// This event fires when the page is being loaded
        /// </summary>
        private void Page_Load(System.Object sender, System.EventArgs e)
        {
            //-- get .config settings
            _strHideFolderPattern = GetConfigString("HideFolderPattern");
            _strHideFilePattern = GetConfigString("HideFilePattern");
            _strAllowedPathPattern = GetConfigString("AllowedPathPattern");
            _strImagePath = GetConfigString("ImagePath", "images/");
            _blnFlushContent = (!string.IsNullOrEmpty(GetConfigString("FlushContent")));

            // set the project id 
            string username = User.Identity.Name;
            UserRole = Roles.GetRole(username, ProjectID);
            ClientScriptManager script = Page.ClientScript;
            var root = GetConfigString("ProjectRoot", "Datadir");

            var url = WebPath();
            if (root == url)
            {
                // set to 0
                script.RegisterClientScriptBlock(this.GetType(), "test", "<script>SetProjectID(0); </script>");
            }
            else
            {
                // set to project id
                ProjectID = ProjectClass.GetProjectID(url);
                script.RegisterClientScriptBlock(this.GetType(), "test", "<script>SetProjectID(" + ProjectID + "); </script>");
            }

            if (!Page.IsPostBack)
            {
                var r = Profile.GetAllUsers().Select(a => a.UserName).ToList();
                r.Remove(User.Identity.Name);
                lbUsers.DataSource = r;
                lbUsers.DataBind();

                var t = ProjectClass.GetProjectRoles();
                lbRole.DataSource = t;
                lbRole.DataBind();

                // set the role
               
            }

            if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Manage, ProjectID))
            {
                divAddAgendaTask.Visible = true;
            }
            else
            {
                divAddAgendaTask.Visible = false;
            }

        }

        public void contentFrame_onLoadServer(object sender, EventArgs e)
        {
            if (!IsPostBack)
                iframe01.Attributes.Add("onLoad", "contentFrame_onLoadClient();");
        }
        /// <summary>
        /// Retrieve a value from the .config file appSettings
        /// </summary>
        private static string GetConfigString(string strKey, string strDefaultValue = "")
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

        /// <summary>
        /// performs the user action indicated in the hidden Action form field
        /// </summary>
        public void HandleAction()
        {
            if (Request.Form[_strActionTag] == null)
                return;

            string strAction = Request.Form[_strActionTag].ToLower();
            if (string.IsNullOrEmpty(strAction))
                return;

            switch (strAction)
            {
                case "newfolder":
                    MakeFolder(GetTargetPath());
                    break;
                case "upload":
                    SaveUploadedFile();
                    break;
                case "newproject":
                    MakeProject();
                    break;
                default:
                    ProcessCheckedFiles(strAction);
                    break;
            }
            if ((_FileOperationException != null))
            {
                WriteError(_FileOperationException);
            }
        }

        /// <summary>
        /// performs the specified action on all checked files
        /// </summary>
        private void ProcessCheckedFiles(string strAction)
        {
            int intLoc = 0;
            string strName = null;
            int intTagLength = _strCheckboxTag.Length;
            ArrayList FileList = new ArrayList();

            foreach (string strItem in Request.Form)
            {
                intLoc = strItem.IndexOf(_strCheckboxTag);
                if (intLoc > -1)
                {
                    _FileOperationException = null;
                    strName = strItem.Substring(intLoc + intTagLength);
                    FileList.Add(strName);
                    switch (strAction)
                    {
                        case "delete":
                            DeleteFileOrFolder(strName);
                            break;
                        case "move":
                            MakeFolder(GetTargetPath());
                            MoveFileOrFolder(strName);
                            break;
                        case "copy":
                            MakeFolder(GetTargetPath());
                            CopyFileOrFolder(strName);
                            break;
                        case "rename":
                            RenameFileOrFolder(strName);
                            break;
                    }
                }
            }

            //-- certain operations must work on all the selected files/folders at once.
            if (strAction == "zip")
            {
                ZipFileOrFolder(FileList);
            }
        }

        /// <summary>
        /// Saves the first HttpPostedFile (if there is one) to the current folder
        /// </summary>
        private void SaveUploadedFile()
        {

            if (Request.Files.Count > 0)
            {
                using (TransactionScope ee = new TransactionScope())
                {
                    ProjectID = GetProjectID();
                    HttpPostedFile pf = Request.Files[0];
                    if (pf.ContentLength > 0)
                    {
                        string strFilename = pf.FileName;
                        string strTargetFile = GetLocalPath(Path.GetFileName(strFilename));
                        //-- make sure we clear out any existing file before uploading
                        ProjectClass.ProjectFileClass.SaveProjectFile(ProjectID, WebPath(), strFilename, User.Identity.Name);
                        if (File.Exists(strTargetFile))
                        {
                            DeleteFileOrFolder(strFilename);
                        }
                        try
                        {
                            pf.SaveAs(strTargetFile);
                            if (CheckConvertableType(strFilename))
                            {
                                FileConverter f = new FileConverter();
                                f.Convert(strTargetFile);
                            }
                            ee.Complete();
                        }
                        catch (Exception ex)
                        {
                            _FileOperationException = ex;
                            if (File.Exists(strTargetFile))
                            {
                                DeleteFileOrFolder(strFilename);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// writes complete table to page, including header, body, and footer
        /// </summary>
        public void WriteTable()
        {
            var t = GetConfigString("ProjectRoot", "");
            var r = WebPath();
            int intRowsRendered = 0;
            //-- header table
            
            // Admin header
            ProjectSettingsDiv();
            // end admin header
            Response.Write("<TABLE class=\"Header\" width=\"100%\" border=0>");
            Response.Write("<TR class=\"pull-right\">");
            //Response.Write("<TD width=310>");
            //Response.Write("<IMG src=\"" + _strImagePath + "file/openfolder.gif\" " + _strIconSize + " align=absmiddle>&nbsp;");
            //Response.Write("<INPUT type=\"text\"  readonly name=\"path\" value=\"" + WebPath() + "\" size=35>");
            //Response.Write("<INPUT type=""submit"" value=""Go"">")

            Response.Write("<TD width=150>");
            Response.Write(UpUrl());
            Response.Write("<IMG src=\"" + _strImagePath + "icon/folderup.gif\" " + _strIconSize + " align=absmiddle>&nbsp;Move up a folder</A>");

            Response.Write("<TD  width=\"*\"><IMG src=\"" + _strImagePath + "icon/down.gif\" " + _strIconSize + " align=absmiddle>&nbsp;<a href=\"#bottom\" title=\"end key\">Scroll to Bottom</a>");
            //Response.Write("<TD width=\"*\" valign=\"top\" align=right>");
            //Response.Write("<IMG src=\"" + _strImagePath + "icon/search.gif\" " + _strIconSize + " align=absmiddle>");
            //Response.Write("&nbsp;<input type=\"text\" placeholder=\"Search\" name=\"searchtext\" size=35>");
            //Response.Write("&nbsp;<input type=\"submit\" class=\"pull-right btn-primary\" value=\"Search\" name=\"SearchButton\">");
            Response.Write("</TABLE>");
            Response.Write(Environment.NewLine);
            Flush();
            //Response.Write("<font size='3'>");
            //-- body table
            Response.Write("<TABLE cellspacing=0 border=0 width=\"100%\" STYLE=\"table-layout:fixed\">");
            Response.Write("<TR>");
            Response.Write("<TH width=20 align=right><INPUT name=\"all_files_checkbox\" onclick=\"javascript:checkall(this);\" type=checkbox>");
            Response.Write("<TH width=20 align=center>");
            Response.Write("<TH align=left>" + PageUrl("", "Name") + "File Name</a>");
            Response.Write("<TH width=80 align=right>" + PageUrl("", "Size") + "Size</a>");
            Response.Write("<TH width=30 align=left>");
            Response.Write("<TH width=45 align=right>" + PageUrl("", "Type") + "Type</a>");
            Response.Write("<TH width=150 align=right>" + PageUrl("", "Created") + "Created</a>");
            Response.Write("<TH width=150 align=right>" + PageUrl("", "Modified") + "Modified</a>");
            Response.Write("<TH width=45 align=right>" + PageUrl("", "Attr") + "Attr</a>");
            Response.Write(Environment.NewLine);
            Flush();

            //-- render body table rows for folders and files
            intRowsRendered = WriteRows();

            Response.Write("</TABLE>");
            Flush();

            //-- footer table
            if (intRowsRendered < 0)
                return;
            if (UserRole == ProjectRoles.Admin.ToString() || UserRole == ProjectRoles.Moderator.ToString() || UserRole == ProjectRoles.Editor.ToString())
            {
                Response.Write("<font size='3'>");
                Response.Write("<a name=\"bottom\"></a>");
                Response.Write("<TABLE class=\"Header\" width=\"100%\">");
                Response.Write("<TR>");

                Response.Write("<TD width=\"300\" valign=\"top\">");
                if (t != r)
                {
                    Response.Write("<IMG src=\"" + _strImagePath + "file/folder.gif\" " + _strIconSize + " align=absmiddle>");
                    Response.Write("&nbsp;<input type=\"text\" placeholder=\"Enter Folder Name\" name=\"" + _strTargetFolderTag + "\" size=35>");
                    Response.Write("<TD width=\"*\" valign=\"top\" rowspan=2> ");
                    Response.Write("<font size='3'>");
                    Response.Write("<TABLE>");
                    Response.Write("<TR>");
                    Response.Write("<TD width=140>");
                    Response.Write("<A href=\"javascript:newfolder();\">");
                    Response.Write("<IMG src=\"" + _strImagePath + "icon/newfolder.gif\" width=19 height=16 align=absmiddle>");
                    Response.Write("&nbsp;New folder</A>");

                    Response.Write("<TD width=140>");
                    Response.Write("<A href=\"javascript:confirmfiles('copy');\">");
                    Response.Write("<IMG src=\"" + _strImagePath + "icon/copy.gif\" " + _strIconSize + " align=absmiddle>&nbsp;Copy to folder</A>");
                    Response.Write("<TD width=140>");
                    Response.Write("<A href=\"javascript:confirmfiles('move');\">");
                    Response.Write("<IMG src=\"" + _strImagePath + "icon/move.gif\" " + _strIconSize + " align=absmiddle>&nbsp;Move to folder</A>");
                    Response.Write("<TD width=\"*\">");
                    Response.Write("<TR>");

                    Response.Write("<TD width=140>");
                    Response.Write("<A href=\"javascript:upload();\">");
                    Response.Write("<IMG src=\"" + _strImagePath + "icon/upload.gif\" width=18 height=16 align=absmiddle>&nbsp;Upload a file</A>");
                }
                Response.Write("<TD width=140>");
                Response.Write("<A href=\"javascript:confirmfiles('delete');\">");
                Response.Write("<IMG src=\"" + _strImagePath + "icon/delete.gif\" width=18 height=16 align=absmiddle>&nbsp;Delete</A>");
                Response.Write("<TD width=140>");
                Response.Write("<A href=\"javascript:confirmfiles('rename');\">");
                Response.Write("<IMG src=\"" + _strImagePath + "icon/rename.gif\" " + _strIconSize + " align=absmiddle>&nbsp;Rename</A>");
                Response.Write("<TD width=\"*\">");
                //Response.Write("<A href=\"javascript:confirmfiles('zip');\">");
                //Response.Write("<IMG src=\"" + _strImagePath + "file/compressed.gif\" " + _strIconSize + " align=absmiddle>");
                //Response.Write("&nbsp;Zip</A>");
                Response.Write("</TABLE>");

                Response.Write("<TR>");
                if (t != r)
                {
                    Response.Write("<TD width=\"*\" valign=\"top\">");//  <INPUT type=\"file\" name=\"fileupload\" enctype=\"multipart/form-data\" id=\"fileUpload\"  runat=\"server\" />
                    //Response.Write("<IMG src=\"" + _strImagePath + "file/generic.gif\" " + _strIconSize + " align=absmiddle>");
                    Response.Write("&nbsp;<INPUT type=\"file\" name=\"fileupload\" />");
                }

                Response.Write("<TD width=\"*\">");
                Response.Write("</TABLE>");
            }
            // Response.Write("<br/><input type=\"button\" runat=\"server\" class=\"pull-right btn-primary\"  value=\"Edit\" onclick=\"DoCopy(this.form.elements.textarea, 'iframe01')\"/>");
            // Response.Write("<IMG src=\"" + _strImagePath + "icon/edit.gif\" width=\"30px\" height=\"30px\" class=\"pull-right\">");
         
            Flush();
        }

        /// <summary>
        /// Gets all files and folders for the current path, and writes rows for each one
        /// </summary>
        private int WriteRows()
        {
            const string strPathError = "The path '{0}' {1} <a href='javascript:history.go(-1);'>Go back</a>";

            //-- make sure we're allowed to look at this web path
            if (!string.IsNullOrEmpty(_strAllowedPathPattern) && !Regex.IsMatch(WebPath(), _strAllowedPathPattern))
            {
                WriteErrorRow(string.Format(strPathError, WebPath(), "is not allowed because it does not match the pattern '" + Server.HtmlEncode(_strAllowedPathPattern) + "'."));
                return -1;
            }

            //-- make sure this directory exists on the server
            string strLocalPath = GetLocalPath();
            if (!Directory.Exists(strLocalPath))
            {
                WriteErrorRow(string.Format(strPathError, WebPath(), "does not exist."));
                return -1;
            }

            //-- make sure we can get the files and directories for this directory
            DirectoryInfo[] da = null;
            FileInfo[] fa = null;
            List<string> files = new List<string>();
            try
            {
                DirectoryInfo di = new DirectoryInfo(strLocalPath);
                da = di.GetDirectories();
                fa = di.GetFiles();
                int PID = GetProjectID();

                if (PID != 0)
                {
                    ProjectID = PID;
                    files = ProjectClass.GetProjectFolderFiles(User.Identity.Name, WebPath(), ProjectID);
                }
                else
                {
                    if (ProjectID != 0)
                    {
                        files = ProjectClass.GetProjectFolderFiles(User.Identity.Name, WebPath(), ProjectID);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorRow(ex);
                return -1;
            }

            //-- add all file/directory info to intermediate DataTable
            DataTable dt = GetFileInfoTable();
            dt.BeginLoadData();
            foreach (DirectoryInfo d in da)
            {
                AddRowToFileInfoTable(d, dt);
            }
            foreach (FileInfo f in fa)
            {
                string file = (WebPath() + "/" + f.Name).ToLower();
                //file = file.Replace("/", "\\");
                if (files.Contains(file))
                {
                    AddRowToFileInfoTable(f, dt);
                }
            }
            dt.EndLoadData();
            dt.AcceptChanges();

            if (dt.Rows.Count == 0)
            {
                WriteErrorRow("(no files)");
                return 0;
            }

            //-- sort and render intermediate DataView from our DataTable
            DataView dv = null;
            if (string.IsNullOrEmpty(SortColumn()))
            {
                dv = dt.DefaultView;
            }
            else
            {
                dv = new DataView(dt);
                if (SortColumn().StartsWith("-"))
                {
                    dv.Sort = "IsFolder, " + SortColumn().Substring(1) + " desc";
                }
                else
                {
                    dv.Sort = "IsFolder desc, " + SortColumn();
                }
            }

            int intRenderedRows = 0;
            foreach (DataRowView drv in dv)
            {
                if (WriteViewRow(drv))
                    intRenderedRows += 1;
            }

            return intRenderedRows;
        }

        /// <summary>
        /// returns intermediate DataTable of File/Directory info 
        /// to be used for sorting prior to display
        /// </summary>
        private DataTable GetFileInfoTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(System.String)));
            dt.Columns.Add(new DataColumn("IsFolder", typeof(System.Boolean)));
            dt.Columns.Add(new DataColumn("FileExtension", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Attr", typeof(System.String)));
            dt.Columns.Add(new DataColumn("Size", typeof(System.Int64)));
            dt.Columns.Add(new DataColumn("Modified", typeof(System.DateTime)));
            dt.Columns.Add(new DataColumn("Created", typeof(System.DateTime)));
            dt.Columns.Add(new DataColumn("Type", typeof(System.String)));
            return dt;
        }


        /// <summary>
        /// translates a FileSystemInfo entry to a DataRow in our intermediate DataTable
        /// </summary>
        private void AddRowToFileInfoTable(FileSystemInfo fi, DataTable dt)
        {
            DataRow dr = dt.NewRow();
            string Attr = AttribString(fi.Attributes);
            string typefolder = "Folder";
            string Type = Path.GetExtension(fi.Extension);

            dr["Name"] = fi.Name;
            dr["FileExtension"] = Path.GetExtension(fi.Name);
            dr["Attr"] = Attr;
            if (Attr.IndexOf("d") > -1)
            {
                dr["IsFolder"] = true;
                dr["Size"] = 0;
            }
            else
            {
                dr["IsFolder"] = false;
                dr["Size"] = new FileInfo(fi.FullName).Length;
            }
            dr["Modified"] = fi.LastWriteTime;
            dr["Created"] = fi.CreationTime;
            dr["Type"] = Type;
            if (Type.Length == 0)
            {
                dr["Type"] = typefolder;
            }
            else
            {
                dr["Type"] = fi.Extension;
            }

            dt.Rows.Add(dr);
        }


        /// <summary>
        /// Returns the specified sort column, if any, provided in the URL querystring
        /// </summary>
        private string SortColumn()
        {
            if (Request.QueryString[_strColSortTag] == null)
            {
                return "Name";
            }
            else
            {
                return Request.QueryString[_strColSortTag];
            }
        }

        /// <summary>
        /// Returns the current URL path we're browsing at the moment
        /// </summary>
        private string WebPath()
        {
            string strPath = Request[_strWebPathTag];
            if (strPath == null || string.IsNullOrEmpty(strPath))
            {
                strPath = GetConfigString("ProjectRoot", "Datadir");
            }
            return strPath;
        }

        private string HTMLWebPath()
        {
            string strPath = Request[_strWebPathTag];
            if (strPath == null || string.IsNullOrEmpty(strPath))
            {
                strPath = GetConfigString("ProjectHTMLRoot", "HTMLProject");
            }
            else
            {
                string strDefault = GetConfigString("ProjectRoot", "DataDir");
                string strConvert = GetConfigString("ProjectHTMLRoot", "HTMLProject");
                strPath = strPath.Replace(strDefault, strConvert);

            }
            return strPath;
        }
        /// <summary>
        /// Returns the URL for one level "up" from our current WebPath()
        /// </summary>
        private string UpUrl()
        {
            string strUp = Regex.Replace(WebPath(), "/[^/]+$", "");
            if (string.IsNullOrEmpty(strUp) | strUp == "/")
            {
                strUp = GetConfigString("ProjectRoot", "Datadir");
            }
            return PageUrl(strUp);
        }

        /// <summary>
        /// return partial URL to this page, optionally specifying a new target path
        /// </summary>
        private string PageUrl(string NewPath = "", string NewSortColumn = "")
        {

            bool blnSortProvided = (!string.IsNullOrEmpty(NewSortColumn));

            //-- if not provided, use the current values in the querystring
            if (string.IsNullOrEmpty(NewPath))
                NewPath = WebPath();
            if (string.IsNullOrEmpty(NewSortColumn))
                NewSortColumn = SortColumn();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<A href=\"");
            sb.Append(ScriptName);
            sb.Append("?");
            sb.Append(_strWebPathTag);
            sb.Append("=");
            sb.Append(NewPath);
            if (!string.IsNullOrEmpty(NewSortColumn))
            {
                sb.Append("&");
                sb.Append(_strColSortTag);
                sb.Append("=");
                if (blnSortProvided & (NewSortColumn.ToLower() == SortColumn().ToLower()))
                {
                    sb.Append("-");
                }
                sb.Append(NewSortColumn);
            }
            sb.Append("\">");

            return sb.ToString();
        }

        /// <summary>
        /// given file object, return formatted KB size in text
        /// </summary>
        private string FormatKB(long FileLength)
        {
            return string.Format("{0:N0}", (FileLength / 1024));
        }

        /// <summary>
        /// turn numeric attribute into standard "RHSDAC" text
        /// </summary>
        private string AttribString(System.IO.FileAttributes a)
        {
            StringBuilder sb = new StringBuilder();
            if ((a & FileAttributes.ReadOnly) > 0)
                sb.Append("r");
            if ((a & FileAttributes.Hidden) > 0)
                sb.Append("h");
            if ((a & FileAttributes.System) > 0)
                sb.Append("s");
            if ((a & FileAttributes.Directory) > 0)
                sb.Append("d");
            if ((a & FileAttributes.Archive) > 0)
                sb.Append("a");
            if ((a & FileAttributes.Compressed) > 0)
                sb.Append("c");
            return sb.ToString();
        }

        /// <summary>
        /// path.combine works great, but is filesystem-centric; we just convert the slashes
        /// </summary>
        private string WebPathCombine(string path1, string path2)
        {
            string strTemp = Path.Combine(path1, path2).Replace("\\", "/");
            if (strTemp.IndexOf("~/") > -1)
            {
                strTemp = strTemp.Replace("~/", Page.ResolveUrl("~/"));
            }
            return strTemp;
        }

        /// <summary>
        /// given filename, return URL to icon image for that filetype
        /// </summary>
        private string FileIconLookup(DataRowView drv)
        {

            if (IsDirectory(drv))
            {
                return WebPathCombine(_strImagePath, "file/folder.gif");
            }

            switch (Convert.ToString(drv["FileExtension"]))
            {
                case ".gif":
                case ".peg":
                case ".jpe":
                case ".jpg":
                case ".png":
                    return WebPathCombine(WebPath(), Convert.ToString(drv["Name"]));
                //Return "file_image.gif"
                case ".txt":
                    return WebPathCombine(_strImagePath, "file/text.gif");
                case ".htm":
                case ".xsl":
                case ".html":
                case ".config":
                    return WebPathCombine(_strImagePath, "file/html.gif");
                case ".mp3":
                case ".wav":
                case ".wma":
                case ".au":
                case ".mid":
                case ".ram":
                case ".rm":
                case ".snd":
                case ".asf":
                    return WebPathCombine(_strImagePath, "file/audio.gif");
                case ".zip":
                case "tar":
                case ".gz":
                case ".rar":
                case ".cab":
                case ".tgz":
                    return WebPathCombine(_strImagePath, "file/compressed.gif");
                case ".db":
                    return WebPathCombine(_strImagePath, "file/db.gif");
                case ".xml":
                    return WebPathCombine(_strImagePath, "file/xml.gif");
                case ".css":
                    return WebPathCombine(_strImagePath, "file/compressed.gif");
                case ".pdf":
                    return WebPathCombine(_strImagePath, "file/pdf.gif");
                case ".docx":
                case ".doc":
                    return WebPathCombine(_strImagePath, "file/word.gif");
                case ".asp":
                case ".wsh":
                case ".js":
                case ".vbs":
                case ".aspx":
                case ".cs":
                case ".vb":
                    return WebPathCombine(_strImagePath, "file/script.gif");
                default:
                    return WebPathCombine(_strImagePath, "file/generic.gif");
            }
        }

        /// <summary>
        /// writes a table row containing information about the file or folder
        /// </summary>
        /// 
        public bool WriteViewRow(DataRowView drv)
        {
            string strFileLink = null;
            bool blnFolder = IsDirectory(drv);
            string strFileName = Convert.ToString(drv["Name"]);
            string strFilePath = WebPathCombine(WebPath(), strFileName); ;
            string htmlName = "";
            if (!blnFolder)
            {

                if (CheckConvertableType(strFileName))
                {
                    htmlName = strFileName.Split('.')[0] + ".htm";
                    strFilePath = WebPathCombine(HTMLWebPath(), htmlName);
                }

                if (!string.IsNullOrEmpty(_strHideFilePattern) && Regex.IsMatch(strFileName, _strHideFilePattern, RegexOptions.IgnoreCase))
                {
                    return false;
                }
                //FileConverter i = new FileConverter();
                strFileLink = "<A href=\"" + strFilePath + "\" target = \"iframe01\" onclick=\"ShowFile(this)\">" + strFileName + "</A>";

            }
            else
            {
                if (!string.IsNullOrEmpty(_strHideFolderPattern) && Regex.IsMatch(strFileName, _strHideFolderPattern, RegexOptions.IgnoreCase))
                {
                    return false;
                }
                strFileLink = PageUrl(strFilePath) + strFileName + "</A>";
            }

            //CKEditor1.text = strFilePath

            Response.Write("<TR>");
            Response.Write("<TD align=right><INPUT name=\"");
            Response.Write(_strCheckboxTag);
            Response.Write(strFileName);
            Response.Write("\" type=checkbox onclick=\"chkFiles(this)\">");
            Response.Write("<TD align=center><IMG src=\"");
            Response.Write(FileIconLookup(drv));
            Response.Write("\" ");
            Response.Write(_strIconSize);
            Response.Write(">");
            Response.Write("<TD>");
            Response.Write(strFileLink);
            Response.Write("<TD align=right>");
            if (blnFolder)
            {
                Response.Write("<TD align=left>");
            }
            else
            {
                Response.Write(FormatKB(Convert.ToInt64(drv["Size"])));
                Response.Write("<TD align=left>kb");
            }
            Response.Write("<TD align=right>");
            Response.Write(Convert.ToString(drv["Type"]));
            Response.Write("<TD align=right>");
            Response.Write(Convert.ToString(drv["Created"]));
            Response.Write("<TD align=right>");
            Response.Write(Convert.ToString(drv["Modified"]));
            Response.Write("<TD align=right>");
            Response.Write(Convert.ToString(drv["Attr"]));
            Response.Write(Environment.NewLine);
            Flush();

            return true;
        }

        /// <summary>
        /// optionally dumps the current response buffer to the client as it is being rendered.
        /// This is faster, but it can cause problems with some HTTP filters 
        /// so it is off by default.
        /// </summary>
        private void Flush()
        {
            if (_blnFlushContent)
                Response.Flush();
        }

        /// <summary>
        /// maps the current web path to a server filesystem path
        /// </summary>
        private string GetLocalPath(string strFilename = "")
        {
            return Path.Combine(Server.MapPath(WebPath()), strFilename);
        }

        /// <summary>
        /// converts a filesystem path to a relative path based on our current 
        /// file browsing path, WITHOUT a leading slash
        /// </summary>
        private string MakeRelativePath(string strFilename)
        {
            string strRelativePath = strFilename.Replace(Server.MapPath(WebPath()), "");
            if (strRelativePath.StartsWith("\\"))
            {
                return strRelativePath.Substring(1);
            }
            else
            {
                return strRelativePath;
            }
        }

        /// <summary>
        /// maps the current web path, plus target folder, to a server filesystem path
        /// </summary>
        private string GetTargetPath(string strFilename = "")
        {
            return Path.Combine(Path.Combine(GetLocalPath(), Request.Form[_strTargetFolderTag]), strFilename);
        }

        /// <summary>
        /// returns True if the provided path is an existing directory
        /// </summary>
        private bool IsDirectory(string strFilepath)
        {
            return Directory.Exists(strFilepath);
        }

        /// <summary>
        /// Returns true if this DataRowView represents a directory/folder
        /// </summary>
        private bool IsDirectory(DataRowView drv)
        {
            return Convert.ToString(drv["attr"]).IndexOf("d") > -1;
        }

        /// <summary>
        /// deletes a file or folder
        /// </summary>
        private void DeleteFileOrFolder(string strName)
        {
            using (TransactionScope ee = new TransactionScope())
            {
                string strLocalPath = GetLocalPath(strName);
                string strHTMLPath = GetHTMLPath(strLocalPath);
                try
                {
                    RemoveReadOnly(strLocalPath);
                    RemoveReadOnly(strHTMLPath);
                    var filepath = WebPath() + "/" + strName;
                    if (IsDirectory(strLocalPath))
                    {
                        if (Directory.Exists(strLocalPath))
                            Directory.Delete(strLocalPath, true);
                        if (Directory.Exists(strHTMLPath))
                            Directory.Delete(strHTMLPath, true);
                        // delete project
                      
                        ProjectClass.DeleteProject(filepath);
                    }
                    else
                    {
                        
                        ProjectClass.ProjectFileClass.DeletProjectFile(ProjectID, filepath);
                        File.Delete(strLocalPath);
                        if (CheckConvertableType(strName))
                        {
                            if (File.Exists(strHTMLPath))
                            {
                                strHTMLPath = strHTMLPath.Split('.')[0] + ".htm";
                                File.Delete(strHTMLPath);
                            }
                        }
                        // delete file from DB
                    }
                }
                catch (Exception ex)
                {
                    _FileOperationException = ex;
                }
            }
        }

        /// <summary>
        /// moves a file from the current folder to the target folder
        /// </summary>
        private void MoveFileOrFolder(string strName)
        {
            string strLocalPath = GetLocalPath(strName);
            string strTargetPath = GetTargetPath(strName);

            string strNewTarget = GetHTMLPath(strTargetPath);
            string strNewSource = GetHTMLPath(strLocalPath);

            try
            {
                if (IsDirectory(strLocalPath))
                {
                    Directory.Move(strLocalPath, strTargetPath);
                    Directory.Move(strNewSource, strNewTarget);
                }
                else
                {
                    File.Move(strLocalPath, strTargetPath);
                    if (CheckConvertableType(strName))
                    {
                        strNewSource = strNewSource.Split('.')[0] + ".htm";
                        strNewTarget = strNewTarget.Split('.')[0] + ".htm";
                        File.Move(strNewSource, strNewTarget);
                    }
                }
            }
            catch (Exception ex)
            {
                _FileOperationException = ex;
            }
        }

        /// <summary>
        /// moves a file from the current folder to the target folder
        /// </summary>
        private void CopyFileOrFolder(string strName)
        {
            string strLocalPath = GetLocalPath(strName);
            string strTargetPath = GetTargetPath(strName);

            try
            {

                if (IsDirectory(strLocalPath))
                {
                    CopyFolder(strLocalPath, strTargetPath, true);

                }
                else
                {
                    CopyHTML(strName, strLocalPath, strTargetPath);
                    File.Copy(strLocalPath, strTargetPath, true);
                }
            }
            catch (Exception ex)
            {
                _FileOperationException = ex;
            }
        }

        private void CopyHTML(string strName, string strLocalPath, string strTargetPath)
        {
            if (CheckConvertableType(strName))
            {

                string strNewTarget = GetHTMLPath(strTargetPath);
                string strNewSource = GetHTMLPath(strLocalPath);
                strNewSource = strNewSource.Split('.')[0] + ".htm";
                strNewTarget = strNewTarget.Split('.')[0] + ".htm";
                string dir = Path.GetDirectoryName(strNewTarget);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Copy(strNewSource, strNewTarget, true);
            }
        }

        /// <summary>
        /// Compress all the selected files
        /// due to limitations of SharpZipLib, this must be done in one pass 
        /// (it cannot modify an existing zip file!)
        /// </summary>
        private void ZipFileOrFolder(ArrayList FileList)
        {
            string ZipTargetFile = null;

            if (FileList.Count == 1)
            {
                ZipTargetFile = GetLocalPath(Path.ChangeExtension(Convert.ToString(FileList[0]), ".zip"));
            }
            else
            {
                ZipTargetFile = GetLocalPath("ZipFile.zip");
            }

            FileStream zfs = default(FileStream);
            ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs = default(ICSharpCode.SharpZipLib.Zip.ZipOutputStream);
            try
            {
                if (File.Exists(ZipTargetFile))
                {
                    zfs = File.OpenWrite(ZipTargetFile);
                }
                else
                {
                    zfs = File.Create(ZipTargetFile);
                }

                zs = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(zfs);

                ExpandFileList(ref FileList);

                foreach (string strName in FileList)
                {
                    ICSharpCode.SharpZipLib.Zip.ZipEntry ze = default(ICSharpCode.SharpZipLib.Zip.ZipEntry);
                    //-- the ZipEntry requires a preceding slash if the file is a folder
                    if (strName.IndexOf("\\") > -1 & !strName.StartsWith("\\"))
                    {
                        ze = new ICSharpCode.SharpZipLib.Zip.ZipEntry("\\" + strName);
                    }
                    else
                    {
                        ze = new ICSharpCode.SharpZipLib.Zip.ZipEntry(strName);
                    }

                    ze.DateTime = DateTime.Now;
                    zs.PutNextEntry(ze);
                    FileStream fs = default(FileStream);
                    try
                    {
                        fs = File.OpenWrite(GetLocalPath(strName));
                        byte[] buffer = new byte[2049];
                        int len = fs.Read(buffer, 0, buffer.Length);
                        while (len > 0)
                        {
                            zs.Write(buffer, 0, len);
                            len = fs.Read(buffer, 0, buffer.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        _FileOperationException = ex;
                    }
                    finally
                    {
                        if ((fs != null))
                            fs.Close();
                        zs.CloseEntry();
                    }
                }
            }
            finally
            {
                if ((zs != null))
                    zs.Close();
                if ((zfs != null))
                    zfs.Close();
            }
        }

        /// <summary>
        /// renames a file; assumes filename is "(oldname)(renametag)(newname)"
        /// </summary>
        private void RenameFileOrFolder(string strName)
        {
            string strOldName = null;
            string strNewName = null;
            int intTagLoc = strName.IndexOf(_strRenameTag);
            if (intTagLoc == -1)
                return;

            strOldName = strName.Substring(0, intTagLoc);
            strNewName = strName.Substring(intTagLoc + _strRenameTag.Length);
            if (strOldName == strNewName)
                return;

            string strOldPath = GetLocalPath(strOldName);
            string strNewPath = GetLocalPath(strNewName);

            string strHTMLOldPath = GetHTMLPath(strOldPath);
            string strHTMLNewPath = GetHTMLPath(strNewPath);

            try
            {
                if (IsDirectory(strOldPath))
                {
                    Directory.Move(strOldPath, strNewPath);
                    Directory.Move(strHTMLOldPath, strHTMLNewPath);
                }
                else
                {
                    File.Move(strOldPath, strNewPath);
                    if (CheckConvertableType(strOldName))
                    {
                        strHTMLOldPath = strHTMLOldPath.Split('.')[0] + ".htm";
                        strHTMLNewPath = strHTMLNewPath.Split('.')[0] + ".htm";
                        File.Move(strHTMLOldPath, strHTMLNewPath);
                    }
                }
            }
            catch (Exception ex)
            {
                _FileOperationException = ex;
            }
        }
        private string GetHTMLPath(string path)
        {
            string strConvert = GetConfigString("ConvertFolder", "WordExcelPPointToHtml");
            string strDefault = GetConfigString("DefaultPath", "DataDir");
            path = path.Replace(strDefault, strConvert);
            string[] file = path.Split('.');
            if (file.Length > 1)
            {
                path = file[0] + ".htm";
            }
            return path;
        }

        /// <summary>
        /// creates a subfolder in the current folder
        /// </summary>
        private void MakeFolder(string strFilename)
        {
            string strLocalPath = GetLocalPath(strFilename);
            string strHTMLPath = GetHTMLPath(strLocalPath);

            try
            {
                if (!Directory.Exists(strLocalPath))
                {
                    Directory.CreateDirectory(strLocalPath);
                }
                if (!Directory.Exists(strHTMLPath))
                {
                    Directory.CreateDirectory(strHTMLPath);
                }
            }
            catch (Exception ex)
            {
                _FileOperationException = ex;
            }
        }

        private void MakeProject()
        {
            string username = User.Identity.Name;
            string title = Request.Form["tbProjectName"];

            var u = hiddenUsers.Value;
            var o = u.Split('|');


            // get users and roles
            Dictionary<string, string> users = new Dictionary<string, string>();
            foreach (var y in o)
            {
                if (!string.IsNullOrEmpty(y))
                {
                    var us = y.Split('.');
                    if (us.Count() == 2)
                    {
                        users.Add(us[0], us[1]);
                    }
                }
            }

            var edit = title.Split('|');
            if (edit.Count() > 1)
            {
                if (edit[0] == "EditUsers")
                {
                    // insert user
                    int pid = Convert.ToInt32(edit[1]);
                    ProjectClass.InsertProjectUsers(username, users, pid);
                }
            }
            else
            {
                ProjectClass.CreateProject(title, username, users);
            }
        }
        /// <summary>
        /// recursively copies a folder, and all subfolders and files, to a target path
        /// </summary>

        private void CopyFolder(string strSourceFolderPath, string strDestinationFolderPath, bool blnOverwrite)
        {
            //-- make sure target folder exists
            if (!Directory.Exists(strDestinationFolderPath))
            {
                Directory.CreateDirectory(strDestinationFolderPath);
            }

            //-- copy all of the files in this folder to the destination folder
            foreach (string strFilePath in Directory.GetFiles(strSourceFolderPath))
            {
                string strFileName = Path.GetFileName(strFilePath);
                //-- if exception, will be caught in calling proc
                File.Copy(strFilePath, Path.Combine(strDestinationFolderPath, strFileName), blnOverwrite);
                CopyHTML(strFileName, strFilePath, Path.Combine(strDestinationFolderPath, strFileName));
            }

            //-- copy all of the subfolders in this folder
            foreach (string strFolderPath in Directory.GetDirectories(strSourceFolderPath))
            {
                string strFolderName = Regex.Match(strFolderPath, "[^\\\\]+$").ToString();
                CopyFolder(strFolderPath, Path.Combine(strDestinationFolderPath, strFolderName), blnOverwrite);
            }
        }

        /// <summary>
        /// Given an ArrayList of file and folder names, ensure that the 
        /// ArrayList contains all subfolder file names
        /// </summary>

        private void ExpandFileList(ref ArrayList FileList)
        {
            string strLocalPath = null;
            ArrayList NewFileList = new ArrayList();

            for (int i = FileList.Count - 1; i >= 0; i += -1)
            {
                strLocalPath = GetLocalPath(Convert.ToString(FileList[i]));
                if (IsDirectory(strLocalPath))
                {
                    FileList.Remove(FileList[i]);
                    AddFilesFromFolder(strLocalPath, ref NewFileList);
                }
            }

            if (NewFileList.Count > 0)
            {
                FileList.AddRange(NewFileList);
            }
        }

        /// <summary>
        /// Adds all the files in the specified folder to the FileList, 
        /// </summary>
        private void AddFilesFromFolder(string strFolderName, ref ArrayList FileList)
        {
            if (!Directory.Exists(strFolderName))
                return;

            try
            {
                foreach (string strName in Directory.GetFiles(strFolderName))
                {
                    FileList.Add(MakeRelativePath(strName));
                }
            }
            catch (Exception ex)
            {
                //-- mostly to catch "access denied"
                _FileOperationException = ex;
            }

            try
            {
                foreach (string strName in Directory.GetDirectories(strFolderName))
                {
                    AddFilesFromFolder(strName, ref FileList);
                }
            }
            catch (Exception ex)
            {
                //-- mostly to catch "access denied"
                _FileOperationException = ex;
            }
        }

        /// <summary>
        /// recursively removes the read only tag from a file or folder, if it is present
        /// </summary>
        private void RemoveReadOnly(string strPath)
        {
            if (IsDirectory(strPath))
            {
                foreach (string strFile in Directory.GetFiles(strPath))
                {
                    RemoveReadOnly(strFile);
                }
                foreach (string strFolder in Directory.GetDirectories(strPath))
                {
                    RemoveReadOnly(strFolder);
                }
            }
            else
            {
                if (File.Exists(strPath))
                {
                    FileInfo fi = new FileInfo(strPath);
                    if ((fi.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        fi.Attributes = fi.Attributes ^ FileAttributes.ReadOnly;
                    }
                }
            }
        }

        /// <summary>
        /// returns the windows identity that ASP.NET is currently running under
        /// </summary>
        private string CurrentIdentity()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// adds additional helpful information to certain types of exceptions
        /// </summary>
        private string GetFriendlyErrorMessage(Exception ex)
        {
            string strMessage = ex.Message;
            if (ex is System.UnauthorizedAccessException)
            {
                strMessage += " The account '" + CurrentIdentity() + "' may not have permission to this file or folder.";
            }
            return strMessage;
        }

        /// <summary>
        /// Parse and display any exceptions encountered during a file operation
        /// </summary>
        private void WriteError(Exception ex)
        {
            WriteError(GetFriendlyErrorMessage(ex));
        }
        private void WriteError(string strText)
        {
            Response.Write("<DIV class=\"Error\">");
            Response.Write(strText);
            Response.Write("</DIV>");
        }
        private void WriteErrorRow(Exception ex)
        {
            WriteErrorRow(GetFriendlyErrorMessage(ex));
        }
        private void WriteErrorRow(string strText)
        {
            Response.Write("<TR><TD><TD><TD colspan=5><DIV class=\"Error\">");
            Response.Write(strText);
            Response.Write("</DIV>");
        }

        private bool CheckConvertableType(string FileName)
        {
            var ext = FileName.Split('.')[1].ToUpper();
            if (ext == "DOC" || ext == "DOCX" || ext == "XLS" || ext == "XLSX" || ext == "PPT" || ext == "PPTX")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetProjectID()
        {
            int PID = ProjectClass.GetProjectID(WebPath());
            return PID;
        }

        #region MyMethods

        #endregion

        #region Roles
        private void CreateProjectLink()
        {
            string username = User.Identity.Name;
            if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.ControlSetting, ProjectID, UserRole))
            {
                var t = GetConfigString("ProjectRoot", "");
                var r = WebPath();
                if (t == r)
                {
                    string button = "<input type=\"button\" value=\"New project\" class=\"btn\" onclick=\"return NewProject()\"/> ";
                    Response.Write(button);
                }
            }

        }
        private void CreateProjectDiscussion()
        {
            string username = User.Identity.Name;
            if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Comment, ProjectID, UserRole))
            {
                var t = GetConfigString("ProjectRoot", "");
                var r = WebPath();
                if (t != r)
                {
                    string button = "<input type=\"button\" value=\"Project Discussions\" class=\"btn\" onclick=\"return Discussions('Project')\"/> ";
                    Response.Write(button);
                }
            }

        }
        private void CreatePrivacyLink()
        {
            string username = User.Identity.Name;
            var t = GetConfigString("ProjectRoot", "");
            var r = WebPath();
            if (t != r)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Manage, ProjectID, UserRole))
                {
                    //string button = "<button class=\"btn\" onclick=\"return ProjectFilePrivacy()\"> Privacy Settings </button>";
                    string button = "<input class=\"btn\" type=\"button\" onclick=\"return ProjectFilePrivacy()\" value=\"Privacy Settings\" />";
                    Response.Write(button);
                }
            }

        }
        private void CreateProjectUsersLink()
        {
            string username = User.Identity.Name;
            var t = GetConfigString("ProjectRoot", "");
            var r = WebPath();
            if (t != r)
            {
                if (Roles.UserIsAllowed_Project(username, ProjectPrivilages.Manage, ProjectID, UserRole))
                {
                    string button = "<input type=\"button\" value=\"Project Users\" class=\"btn\" onclick=\"return EditProjectUsers()\"/> ";
                    Response.Write(button);
                }
            }

        }

        private void ProjectSettingsDiv()
        {
            Response.Write("<div class=\"row-fluid\"><div class=\"btn-toolbar pull-right\"><div class=\"btn-group\" >");

            CreateProjectLink();
            CreateProjectDiscussion();
            CreateProjectUsersLink();
            CreatePrivacyLink();
            Response.Write(" </div></div></div>");
        }

        #endregion


        #region WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetProjectUsers(int projectid)
        {
            var result = ProjectClass.ProjectUsersClass.GetProjectUsers(projectid);
            string final = "";
            foreach (var t in result)
            {
                string td = "<tr><td>" + t.Key + "</td><td>" + t.Value + "</td><td><a href=\"#\" onclick=\"DeleteUser(this, 'Edit')\"><span class=\"icon-x\"></span></a></td></tr>";
                final += td;
            }
            return final;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string DeleteProjectUser(int ProjectID, string Username)
        {
            var t = ProjectClass.ProjectUsersClass.DeleteProjectUser(Username, ProjectID);
            if (t)
                return "true";
            else
                return "false";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetProjectUsernames(int projectid)
        {
            var result = ProjectClass.ProjectUsersClass.ListProjectUsers(projectid).OrderBy(a => a).ToList();
            List<string> chk = new List<string>();
            string final = FormatUsers(result, chk);
            return final;

        }

        private static string FormatUsers(List<string> result, List<string> chk)
        {
            string final = "<table class=\"table table-striped\" style=\"font-size: smaller; width:100%\">{0}</table>";
            string tr = "<tr>{0}</tr>";
            string td1 = "<td><label class=\"checkbox inline\"><input type=\"checkbox\" value=\"{0}\" onclick=\"chkUsers(this)\" {1}><span class=\"metro-checkbox\">{0}</span></label></td>";
            string td2 = "<td><label class=\"checkbox inline\"><input type=\"checkbox\" value=\"{0}\" onclick=\"chkUsers(this)\" {1}><span class=\"metro-checkbox\">{0}</span></label></td>";
            string td3 = "<td><label class=\"checkbox inline\"><input type=\"checkbox\" value=\"{0}\" onclick=\"chkUsers(this)\" {1}><span class=\"metro-checkbox\">{0}</span></label></td>";
            string temp = "";
            string tempTR = "";
            for (int u = 0; u < result.Count() + 2; u = u + 3)
            {
                if (u < result.Count)
                {
                    if (chk.Contains(result[u]))
                    {
                        temp += string.Format(td1, result[u], "checked");
                    }
                    else
                    {
                        temp += string.Format(td1, result[u], "");
                    }

                    if (u + 1 < result.Count())
                    {
                        if (chk.Contains(result[u + 1]))
                        {
                            temp += string.Format(td2, result[u + 1], "checked");
                        }
                        else
                        {
                            temp += string.Format(td2, result[u + 1], "");
                        }

                        if (u + 2 < result.Count())
                        {
                            if (chk.Contains(result[u + 2]))
                            {
                                temp += string.Format(td3, result[u + 2], "checked");
                            }
                            else
                            {
                                temp += string.Format(td3, result[u + 2], "");
                            }

                        }
                        else
                        {
                            tempTR += string.Format(tr, temp);
                            break;
                        }
                    }
                    else
                    {
                        tempTR += string.Format(tr, temp);
                        break;
                    }
                    tempTR += string.Format(tr, temp);
                }
            }
            final = string.Format(final, tempTR);
            return final;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SetProjectPrivacyByUser(int ProjectID, string[] Files, string[] Users, string Dir)
        {
            string username = HttpContext.Current.User.Identity.Name;
            ProjectClass.ProjectPrivacyClass.SetProjectPrivacyByUsers(ProjectID, Files, Dir, Users, username);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SetProjectPrivacyPublic(int ProjectID, string[] Files, bool Public, string Dir)
        {
            string username = HttpContext.Current.User.Identity.Name;
            ProjectClass.ProjectPrivacyClass.SetProjectPrivacyPublic(ProjectID, Files, Dir, Public, username);
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SetProjectPrivacyByRole(int ProjectID, string[] Files, string[] Roles, string Dir)
        {
            string username = HttpContext.Current.User.Identity.Name;
            ProjectClass.ProjectPrivacyClass.SetProjectPrivacyByRoles(ProjectID, Files, Dir, Roles, username);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ProjectClass.ProjectPrivacyClass GetProjectFilePrivacy(int ProjectID, string Filename, string Dir)
        {
            string username = HttpContext.Current.User.Identity.Name;
            var result = ProjectClass.ProjectPrivacyClass.GetProjectFilePrivacy(ProjectID, Dir, username, Filename);
            result.UsernamesString = FormatUsers(ProjectClass.ProjectUsersClass.ListProjectUsers(ProjectID).OrderBy(a => a).ToList(), result.Usernames.ToList());

            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string test()
        {
            return "sucess";
        }
        # region Discussion
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetDiscussions(int ProjectID)
        {
            var result = DiscussionClass.ProjectDiscussion.GetListProjectDiscussions(ProjectID);
            string disScript = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowDiscussion(this); return false;"">{1}</a></p><span style=""font-size:small""><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p></span><hr />";
            string finalString = "";
            foreach (var t in result)
            {
                finalString += string.Format(disScript, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
            }
            return finalString;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveDiscussion(int ProjectID, string title, string text)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var t = DiscussionClass.ProjectDiscussion.SaveProjectDiscussion(title, text, ProjectID, user);
            string markup = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowDiscussion(this); return false;"">{1}</a></p><span style=""font-size:small""><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p></span><hr />";
            return string.Format(markup, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetDiscussion(int disId)
        {
            string mainthread = @"<div id=""MainThread"" class=""row-fluid mainBox""><div id=""MainThreadInfo"" class=""span2"" style=""margin-left: 25px; margin-top: 10px"">
                                <b>{0} </b><br /> {1} <br />{2}</div>
                                <div class=""span7"" style=""margin-right: 25px; margin-top: 10px; margin-bottom:10px"">
                                <div id=""MainThreadTitle"" class=""row-fluid"">{3}</div><hr />
                                <div id=""MainThreadText"" class=""row-fluid"">{4}
                                </div></div></div>";

            string replyContainer = @"<div id=""Replies"" class=""container-fluid"">{0}</div>";
            var t = DiscussionClass.ProjectDiscussion.GetDiscussionHead(disId);
            string head = string.Format(mainthread, t.UserProfile.UserName, t.Date.ToString("MM/dd/yyyy"), t.Date.ToString("hh:mm:ss tt"), t.Title, t.Text);
            // get replies
            var rep = DiscussionClass.ProjectDiscussion.GetDiscussionReplies(disId);
            string finalReplies = "";
            if (rep.Count != 0)
            {
                foreach (var r in rep)
                {
                    finalReplies += string.Format(replies, r.UserProfile.UserName, r.Date.ToString("MM/dd/yyyy"), r.Date.ToString("hh:mm:ss tt"), r.Title, r.Text);
                }

            }
            finalReplies = string.Format(replyContainer, finalReplies);
            string result = head + finalReplies;
            return result;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveReply(int disID, string title, string text)
        {
            string user = HttpContext.Current.User.Identity.Name;
            var r = Watcher.DiscussionWatcher.SaveReply(disID, user, title, text);
            string markup = string.Format(replies, user, r.Date.ToString("MM/dd/yyyy"), r.Date.ToString("hh:mm:ss tt"), r.Title, r.Text);
            return markup;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveFileReply(int disID, string title, string text)
        {
            string user = HttpContext.Current.User.Identity.Name;
            var r = Watcher.DiscussionWatcher.SaveReply(disID, user, title, text);
            string markup = string.Format(fileReplies, user, r.Date.ToString("MM/dd/yyyy"), r.Date.ToString("hh:mm:ss tt"), r.Title, r.Text);
            return markup;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetFileDiscussions(string Filename)
        {
            var path = GetFileDir(Filename);
            var result = DiscussionClass.FileDiscussion.GetListFileDiscussions(path);
            string disScript = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowFileDiscussion(this); return false;"">{1}</a></p><span style=""font-size:small""><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p></span><hr />";
            string finalString = "";
            foreach (var t in result)
            {
                finalString += string.Format(disScript, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
            }
            return finalString;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveFileDiscussion(string Filename, string title, string text)
        {
            var path = GetFileDir(Filename);
            var user = HttpContext.Current.User.Identity.Name;
            var t = DiscussionClass.FileDiscussion.SaveFileDiscussion(title, text, path, user);
            string markup = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowFileDiscussion(this); return false;"">{1}</a></p><span style=""font-size:small""><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p></span><hr />";
            return string.Format(markup, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
        }

        private static string GetFileDir(string path)
        {
            path = HttpContext.Current.Server.UrlDecode(path);
            var sp = path.Split('/').ToList();
            var folder = GetConfigString("ProjectRoot", "Datatdir");
            var startIndex = sp.IndexOf(folder);
            string FullPath = "";
            for (int y = startIndex; y < sp.Count(); y++)
            {
                if (y == sp.Count() - 1)
                {
                    FullPath += sp[y];
                }
                else
                {
                    FullPath += sp[y] + '/';
                }
            }
            FullPath = FullPath.Replace("%20", " ");
            return FullPath;
                 
        }
        private static string GetFileHTMLDir(string path)
        {
            var sp = path.Split('/').ToList();
            var folder = GetConfigString("ProjectRoot", "Datatdir");
            var html = GetConfigString("ProjectHTMLRoot", "HTMLProject");
            var startIndex = sp.IndexOf(folder) +1;
            string FullPath = html + "/";
            for (int y = startIndex; y < sp.Count(); y++)
            {
                if (y == sp.Count() - 1)
                {
                    FullPath += sp[y];
                }
                else
                {
                    FullPath += sp[y] + '/';
                }
            }
            FullPath = FullPath.Split('.')[0] + ".htm";
            FullPath = FullPath.Replace("%20", " ");
            return FullPath;
        }
        private static string GetSavePath(string path, out bool convertable)
        {
            var local = GetFileDir(path);
            var ex = local.Split('.')[1].ToUpper();
            var finalpath = "";
            if (ex == "DOC" || ex == "DOCX" || ex == "XLS" || ex == "XLSX" || ex == "PPT" || ex == "PPTX")
            {
                // get html path to save
               finalpath = GetFileHTMLDir(path);
               convertable = true;
            }
            else
            {
                // remove html tags from text
                finalpath = GetFileDir(path);
                convertable = false;
            }
            return finalpath;
        }
        public static string StripTags(string input)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(input ?? "");
            return doc.DocumentNode.InnerText;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void SaveEditedFile(string Filepath, string Filename, string File)
        {
            var ex = Filepath.Split('.')[1].ToUpper();
            if (SaveFileTypes.Contains(ex))
            {
                bool conv;
                var path = GetSavePath(HttpUtility.UrlDecode(Filepath), out conv);
                var fullpath = HttpContext.Current.Server.MapPath(path);
                // file
                var decode = Microsoft.JScript.GlobalObject.unescape(File);
                if (!conv)
                {
                    decode = StripTags(decode);
                }

                using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                {
                    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                    {
                        w.WriteLine(decode);
                    }
                }
            }
            else
            {
                throw new Exception("Unsupported File type");
            }
        }
        #endregion 


        #endregion

        #region Tasks
        // Task assginments WebMethods
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        //[System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
        public static string[] GetCompletionList(string prefixText, int count, string contextKey)
        {

            string file = HttpContext.Current.Request["FilePath"].ToString();
            file = GetFileDir(file);
            // Create array of users  
            var users = ProjectClass.ProjectPrivacyClass.GetProjectFileUsernames(ProjectID,file).ToList();
            string[] u = { };
            if (users.Count != 0)
            {
                users.Add("All");
                u = users.ToArray();
                  
            }
            return (from m in u where m.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase) select m).Take(count).ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveFileTask(int ProjectID, string Title, string Desc, DateTime DueDate, string[] usernames, string FilePath)
        {
            FilePath = GetFileDir(FilePath);
            string us = HttpContext.Current.User.Identity.Name;
            var t = ProjectClass.ProjectTaskClass.SaveProjectFileTask(us, ProjectID, Title, Desc, DueDate, usernames, FilePath);


            string user = "";
            for (int u = 0; u < t.Usernames.Count(); u++)
            {
                if (u == t.Usernames.Length - 1)
                {
                    user += t.Usernames[u];
                }
                else
                {
                    user += t.Usernames[u] + "; ";
                }
            }
            string final = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), user, "", @"border: 1px solid Red; background-color: #FF4D4D;", "Incomplete");
            //string  = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"), t.DueDate.ToString("MM/dd/yyyy"), user, "div" + t.TaskID);
            final= string.Format(startAndEnd, t.TaskID, final, "", "");
            return final;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetProjectFileTasks(string FileDirectory, int ProjectID)
        {
            FileDirectory = GetFileDir(FileDirectory);
            string username = HttpContext.Current.User.Identity.Name;
            var result = ProjectClass.ProjectTaskClass.GetProjectFileTasks(username, FileDirectory, ProjectID);
            var role = Roles.UserIsAllowed_Project(username, ProjectPrivilages.Create, ProjectID);

            StringBuilder final = new StringBuilder();
            foreach (var t in result)
            {
                
                string tasks = "";
                string responses = "";
                string tool = "";
                GetTaskResponses(username, role, final, ref tasks, ref responses, tool, t);
            }

            return final.ToString();
        }

        private static void GetTaskResponses(string username, bool role, StringBuilder final, ref string tasks, ref string responses, string tool, TaskClass t)
        {
            if (!role)
            {
                
                
                var res = TaskClass.GetUserTaskResponse(t.TaskID, username);
                if (t.DueDate <= DateTime.Now.AddDays(2) && res.Response != "Complete")
                {
                    tasks = string.Format(UserTaskTag1, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"),@"style=""color:Red""");
                }
                else
                {
                    tasks = string.Format(UserTaskTag1, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), "");
                }
                if (res.ResponseDate == null)
                {
                    // 
                    final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, string.Format(ResponseTool, t.TaskID, t.TaskID, t.TaskID, t.TaskID, "Choose Acttion", "")));
                }
                else
                {
                    DateTime y = (DateTime)res.ResponseDate;
                    string style = "";
                    GetCompleteStyle(res.Response, ref style);
                    responses = string.Format(UserUserResponse1, res.Username, res.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), res.Desc, t.TaskID, style);
                    //responses = string.Format(UserUserResponse1, res.Username, res.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), res.Desc, t.TaskID);
                    final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, tool));
                }
            }
            else
            {
                string user = "";
                for (int u = 0; u < t.Usernames.Count(); u++)
                {
                    if (u == t.Usernames.Length - 1)
                    {
                        user += t.Usernames[u];
                    }
                    else
                    {
                        user += t.Usernames[u] + "; ";
                    }
                }
                
                // get responses for the task
                var res = TaskClass.GetAllTaskResponse(t.TaskID);
                bool complete = true;
                foreach (var r in res)
                {
                    if (r.ResponseDate == null)
                    {
                        responses += "";
                        complete = false;
                    }
                    else
                    {
                        DateTime y = (DateTime)r.ResponseDate;
                        string style = "";
                        complete = GetCompleteStyle(r.Response, ref style);
                        
                        if (r.Username == username)
                        {
                            responses += string.Format(UserUserResponse1, r.Username, r.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), r.Desc, t.TaskID, style);
                            //responses += string.Format(UserUserResponse, r.Username, r.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), r.Desc, t.TaskID);
                            //final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, string.Format(ResponseTool, t.TaskID, t.TaskID, t.TaskID, t.TaskID, "Choose Acttion", "")));
                        }
                        else
                        {
                            responses += string.Format(UserResponse1, r.Username, r.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), r.Desc, style,t.TaskID);
                            //responses += string.Format(UserResponse, r.Username, r.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), r.Desc);
                        }
                    }
                }
                if (complete)
                {
                    tasks = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), user, "", @"border: 1px solid Green; background-color: #66C266;","Complete");
                }
                else
                {
                    if (t.DueDate <= DateTime.Now.AddDays(2))// && t.DueDate >= DateTime.Now
                    {
                        tasks = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), user, @"style=""color:Red""", @"border: 1px solid Red; background-color: #FF4D4D;", "Incomplete");
                    }
                    else
                    {
                        tasks = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), user, "", @"border: 1px solid Red; background-color: #FF4D4D;", "Incomplete");
                    }
                }
                //tasks = string.Format(CreatorTaskTag, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"), t.DueDate.ToString("MM/dd/yyyy"), user);
                //get user response
                var userRes = res.Where(a => a.Username == username).FirstOrDefault();
                if (userRes != null)
                {
                    if (userRes.ResponseDate == null)
                    {
                        final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, tool + string.Format(ResponseTool, t.TaskID, t.TaskID, t.TaskID, t.TaskID, "Choose Acttion", "")));
                    }
                    else
                    {
                        final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, tool));
                    }
                }
                else
                {
                    final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, tool));
                }
                //final.Append(string.Format(startAndEnd, t.TaskID, tasks, responses, tool + string.Format(ResponseTool, t.TaskID, t.TaskID, t.TaskID, t.TaskID, "Choose Acttion", "")));
            }
        }

        private static bool GetCompleteStyle( string response, ref string style)
        {
            switch (response)
            {
                case "Complete":

                    style = @"style=""color: Green""";
                    return true;

                case "Decline Task":
                    
                    style = @"style=""color: Red""";
                    return false;

                default:
                    
                    style = @"style=""color: Blue""";
                    return false;
            }
        }

       
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static TaskClass GetFileTask(int TaskID)
        {
            return AgendaClass.AgendaTaskClass.GetTask(TaskID);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string DeleteFileTask(int TaskID)
        {
            var username = HttpContext.Current.User.Identity.Name;
            if (TaskClass.DeleteTask(TaskID, username))
                return "true";
            else
                return "false";
        }

      

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateFileTask(int TaskID, int ProjectID, string Title, string Desc, DateTime DueDate, string[] usernames)
        {
            string us = HttpContext.Current.User.Identity.Name;
            var t = ProjectClass.ProjectTaskClass.UpdateProjectFileTask(TaskID, us, ProjectID, Title, Desc, DueDate, usernames);

            string user = "";
            for (int u = 0; u < t.Usernames.Count(); u++)
            {
                if (u == t.Usernames.Length - 1)
                {
                    user += t.Usernames[u];
                }
                else
                {
                    user += t.Usernames[u] + "; ";
                }
            }
            string username = HttpContext.Current.User.Identity.Name;
            var role = Roles.UserIsAllowed_Project(username, ProjectPrivilages.Create, ProjectID);
            StringBuilder final = new StringBuilder();
            string tasks = "";
            string responses = "";
            string tool = "";
            GetTaskResponses(username, role, final, ref tasks, ref responses, tool, t);

            return final.ToString();
            //string final = string.Format(CreatorTaskTag, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"), t.DueDate.ToString("MM/dd/yyyy"), user);
            //// get the responses

            //return final;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveTaskResponse(int TaskID, string Response, string Desc)
        {
            string username = HttpContext.Current.User.Identity.Name;
            var r = AgendaClass.AgendaTaskClass.SaveTaskResponse(username, TaskID, Desc, Response);
            if (r)
            {
                var res = AgendaClass.AgendaTaskClass.GetUserTaskResponse(TaskID, username);
                if (res.ResponseDate != null)
                {
                    DateTime y = (DateTime)res.ResponseDate;
                    string style = "";
                    GetCompleteStyle(res.Response, ref style);
                    return @"<div class=""row-fluid"">" + string.Format(UserUserResponse1, username, res.Response, y.ToString("MM/dd/yyyy"), res.Desc, TaskID, style) + "</div>";
                    //return string.Format(UserUserResponse1, username, res.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), res.Desc, TaskID) + "<br/>";

                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        [WebMethod]// delete
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetUserTaskResponse(int TaskID)
        {
            string username = HttpContext.Current.User.Identity.Name;
            var res = AgendaClass.AgendaTaskClass.GetUserTaskResponse(TaskID, username);
            if (res.ResponseDate != null)
            {
                DateTime y = (DateTime)res.ResponseDate;
                return string.Format(ResponseTool, TaskID, TaskID, TaskID, TaskID, res.Response, res.Desc);

            }
            else
            {
                return "";
            }
        }

        #endregion


    }
}
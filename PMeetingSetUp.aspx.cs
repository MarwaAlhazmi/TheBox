using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using TheBox.Protected.BLL;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI.HtmlControls;
using System.Text;

namespace TheBox
{
    public partial class PMeetingSetUp : System.Web.UI.Page
    {
        public static string replies = @"<div class=""row-fluid replyBox""><div class=""replyContent"" style=""margin-bottom:10px"">
                             <div class=""span2""><b>{0} </b><br />{1}<br />{2}</div>
                             <div class=""span7""><div>{3}</div><hr />{4}</div></div></div>";

        private static string CreatorTaskTag = @"<div class=""btn-group pull-right""><button class=""btn dropdown-toggle"" style=""background-color: #F5F5F5; border-color:#F5F5F5"" data-toggle=""dropdown"">" +
                         @"<span class=""caret""></span></button><ul aria-labelledby=""dLabel"" role=""menu"" class=""dropdown-menu"">" +
                         @"<li><a id=""{0}"" onclick=""return EditAgendaTask(this)"" href=""#"">Edit</a></li><li><a id=""{1}"" onclick=""return DeleteAgendaTaskConfirm(this)"" href=""#"">Delete</a></li></ul></div>" +
                         @"<table><tr><td style=""width:100px"">Action:</td><td>{2}</td></tr><tr><td>Describtion</td><td>{3}</td></tr><tr><td>Date Created</td><td>{4}</td></tr>"
                         + @"<tr><td>Due Date</td><td>{5}</td></tr><tr><td>By Users</td><td>{6}</td></tr></table>";

        private static string CreatorTaskTag1 = @"<div class=""btn-group pull-right""><button class=""btn dropdown-toggle"" style=""background-color: #F5F5F5; border-color:#F5F5F5"" data-toggle=""dropdown"">" +
            @"<span class=""caret""></span></button><ul aria-labelledby=""dLabel"" role=""menu"" class=""dropdown-menu"">" +
            @"<li><a id=""{0}"" onclick=""return EditAgendaTask(this)"" href=""#"">Edit</a></li><li><a id=""{1}"" onclick=""return DeleteAgendaTaskConfirm(this)"" href=""#"">Delete</a></li></ul></div>" +
                                           @"<div class=""span1 pull-right"" style=""{8} width: 100px; color: White; text-align: center"">{9}</div><div class=""row-fluid""><div class=""span4""><strong>Action: </strong>" +
                                           @"<span>{2}</span></div><div class=""span3""><strong>Date Created:  </strong>{4}</div><div class=""span3"" {7}><strong>Due Date: </strong>{5}</div></div>" +
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

        private static string ResponseTool = @"<hr/><div id=""divTool{3}"" class=""span10""><div class=""btn-group""><button class=""btn"" id=""btnTaskResponse{1}"" style=""width: 150px"" onclick=""return false "">{4}</button><button data-toggle=""dropdown"" " +
                                             @"class=""btn dropdown-toggle""><span class=""caret""></span></button><ul class=""dropdown-menu"" role=""menu"" aria-labelledby=""dLabel""><li><a id=""aCompleteAction.{3}"" href=""#"" onclick=""return btnTaskResponseClick(this)"">" +
                                             @"Complete</a></li><li><a href=""#"" id=""aCommentAction.{3}"" onclick=""return btnTaskResponseClick(this)"">Comment</a></li><li><a href=""#"" id=""aDeclineAction.{3}"" onclick=""return btnTaskResponseClick(this)"">Decline Task</a></li></div><br /><textarea id=""textarea{0}"" " +
                                             @"class=""input-block-level"" placeholder=""Insert a comment"" cols="""" rows="""" style=""width:100%; height: 90px"">{5}</textarea><div class=""pull-right""><input type=""button"" id=""btnResponse.{2}"" value=""Send"" class=""btn-primary"" onclick=""SaveTaskResponse(this)"" /></div></div>";

        private static string startAndEnd = @"<div class=""taskBox"" id=""div{0}"">{1}<div class=""row-fluid"">{2} {3} </div></div>";

       

        
        protected void Page_Load(object sender, EventArgs e)
        {
            
                if (!Page.IsPostBack)
                {
                    if (Request["id"] != null)
                    {
                        // get info
                        try
                        {
                            string username = User.Identity.Name;
                            int id = Convert.ToInt32(Request["id"]);
                            if (MeetingClass.CParticipant.isParticipant(id, username))
                            {
                                var evn = MeetingClass.MeetingInfo.GetMeeting(id);
                                tbID.Value = id.ToString();
                                tbtitle.Value = evn.Title;
                                tbSDate.Value = evn.Start.ToShortDateString();
                                tbEDate.Value = evn.End.ToShortDateString();
                                tbSTime.Value = evn.Start.ToShortTimeString();
                                tbETime.Value = evn.End.ToShortTimeString();
                                tbDesc.Value = evn.Desc;
                                ClientScriptManager script = Page.ClientScript;

                                // check if the user is allowed

                                script.RegisterClientScriptBlock(this.GetType(), "test", "<script>setMeetingID(" + id + "); </script>");

                                // fill locations 
                                var locations = MeetingClass.CResource.GetAvailableLocations(evn.Start, evn.End);
                                if (locations.Count() == 0)
                                {
                                    lbLocation.Items.Add("No Location available for this meeting's time and date");
                                }
                                else
                                {
                                    lbLocation.DataSource = locations;
                                    lbLocation.DataValueField = "LocationID";
                                    lbLocation.DataTextField = "LocationName";
                                    lbLocation.DataBind();
                                }

                                // get discussions
                                var result = DiscussionClass.MeetingDiscussion.GetListMeetingDiscussions(id);
                                string disScript = @"<p><a href=""#"" title=""{0}"" onclick=""ShowDiscussion(this); return false;"">{1}</a></p><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p><hr />";
                                foreach (var t in result)
                                {
                                    DiscussionList.InnerHtml += string.Format(disScript, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
                                }

                                // Privacy fro Agenda
                                if (AgendaClass.AgendaPrivacyClass.CheckRole(username, id, MeetingRoles.Creator))
                                {
                                    divAddAgendaTask.Visible = true;
                                }
                                else
                                {
                                    divAddAgendaTask.Visible = false;
                                }
                            }
                            else
                            {
                                Response.Redirect("~/Error.aspx");
                            }



                        }
                        catch
                        {

                        }

                        // get participant
                        //var parti = MeetingClass.Participant.getMeetingParticipant(id);
                        //int index = 1;
                        //foreach (var r in parti)
                        //{
                        //    HtmlTableRow row = new HtmlTableRow();
                        //    HtmlTableCell num = new HtmlTableCell();
                        //    num.InnerText = index.ToString();
                        //    row.Cells.Add(num);
                        //    HtmlTableCell user = new HtmlTableCell();
                        //    user.InnerText = r.UserName;
                        //    row.Cells.Add(user);
                        //    HtmlTableCell role = new HtmlTableCell();
                        //    role.InnerText = r.Role;
                        //    row.Cells.Add(role);
                        //    HtmlTableCell res = new HtmlTableCell();
                        //    res.InnerText = r.Response;
                        //    row.Cells.Add(res);
                        //    HtmlTableCell note = new HtmlTableCell();
                        //    note.InnerText = r.Note;
                        //    row.Cells.Add(note);
                        //    HtmlTableCell close = new HtmlTableCell();
                        //    close.InnerHtml = "<a href='#' onclick='return Delete(this)'><span class='icon-x'></span></a>";
                        //    row.Cells.Add(close);

                        //    // add row to table
                        //    parTable.Rows.Add(row);
                        //    index++;
                        //    btnInvite.Visible = true;
                        //}

                    }
                    else
                    {
                        if (!Roles.ChkAdmin(User.Identity.Name))
                        {
                            Response.Redirect("~/Error.aspx");
                        }
                    }
                    // fill resources
                    SetResources();
                    // fill MTypes
                    ddlMType.DataSource = MeetingClass.getMeetingTypes();
                    ddlMType.DataTextField = "Type";
                    ddlMType.DataValueField = "CatID";
                    ddlMType.DataBind();
                    // get all participants
                    string[] users = ldap.getListOfUsersTemp().OrderBy(a => a).ToArray();
                    lbPart.DataSource = users;
                    lbPart.DataBind();
                    //for (int o = 0; o < users.Count(); o++ )
                    //{

                    //    //string str = @"<label class=""checkbox""><input type=""checkbox"" id=""chk"" runat=""server"">";
                    //    //str += @"<span class=""metro-checkbox"">" + users[o] + "</span></label>";
                    //    //if (o < users.Count()/2)
                    //    //{
                    //    //    divAttendees1.InnerHtml += str;
                    //    //}
                    //    //else
                    //    //{
                    //    //    divAttendees2.InnerHtml += str;
                    //    //}
                    //}



                }

            
        }
        private void SetResources()
        {
            string i = @"<label class=""checkbox inline"">
                        <input type=""checkbox"" id=""{0}"" value=""{1}""><span class=""metro-checkbox"" style=""width:300px"">{2}</span>
                        </label></br>";
            var result = MeetingClass.CResource.getResources();
            foreach (var t in result)
            {
                divItems.InnerHtml += string.Format(i, "cb" + t.RID, t.RID, t.RName);
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string checkAvailability(int loc, DateTime date, DateTime stime, DateTime etime)
        {
            if (loc != 0 && date != null && stime != null && etime != null)
            {
                bool value = MeetingClass.CResource.CheckAvl(loc, date, stime, etime);
                if (value)
                    return "true";
                else
                    return "false";
            }
            else
                return "false";
        }

        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        //[System.Web.Services.WebMethodAttribute(), System.Web.Script.Services.ScriptMethodAttribute()]
        public static string[] GetCompletionList(string prefixText, int count, string contextKey)
        {

            int agenda = Convert.ToInt32(HttpContext.Current.Request["Agenda"]);
            // Create array of users  
            var users = AgendaClass.AgendaPrivacyClass.ListParticipantUsername(agenda).ToList();
            string[] u = { };
            if (users.Count != 0)
            {
                users.Add("All");
                u = users.ToArray();
                // Return matching movies  

            }
            return (from m in u where m.StartsWith(prefixText, StringComparison.CurrentCultureIgnoreCase) select m).Take(count).ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static bool CheckResource()
        {
            var username = HttpContext.Current.User.Identity.Name;
            return Roles.ChkAdmin(username);
        }

        #region Participnats
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<MeetingClass.CParticipant> GetParticipants(int id)
        {
            var result = MeetingClass.CParticipant.getMeetingParticipant(id);
            return result;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string DisinviteParticipant(int meetingid, string username)
        {
            string creator = HttpContext.Current.User.Identity.Name;
            Watcher.MeetingWatcher.DisinviteParti(meetingid, username, creator);
            return "true";
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveParticipants(object[] obj, int meetingID)
        {
            var username = HttpContext.Current.User.Identity.Name;
            var result = Watcher.MeetingWatcher.InsertParticipants(obj, meetingID, username);
            return result.ToString();
        }
        #endregion
        // Resources
        #region Resources
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string insertResource(string name, string desc)
        {
            var username = HttpContext.Current.User.Identity.Name;
            var result = MeetingClass.CResource.InsertResource(name, desc, username);
            return result.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void insertLocation(string name)
        {
            var username = HttpContext.Current.User.Identity.Name;
            MeetingClass.CResource.InsertLocation(name, username);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetLocation(int MeetingID)
        {
            var result = MeetingClass.CResource.GetMeetingLocation(MeetingID);
            if (result == null)
            {
                return "";
            }
            else
            {
                return result.LName;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<MeetingClass.CResource> GetResources(int MeetingID)
        {
            //            string tr = @"<label class=""checkbox inline"">
            //                        <input type=""checkbox"" value=""{0}""><span class=""metro-checkbox"" style=""width:300px"">{1}</span>
            //                        </label></br>"; 
            return MeetingClass.CResource.GetMeetingResources(MeetingID);
            //string finalResult = "";
            //foreach (var t in result)
            //{
            //    finalResult += string.Format(tr, t.RID.ToString(), t.RName);
            //}
            //return finalResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateLocation(int MeetingID, int LocationID)
        {
            try
            {
                MeetingClass.CResource.SetMeetingLocation(MeetingID, LocationID);
                return "true";
            }
            catch
            {
                return "false";
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateResources(object[] ids, int MeetingID)
        {

            if (MeetingClass.CResource.SetMeetingResources(ids, MeetingID))
                return "true";
            else
                return "false";
        }
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
        public static string CheckAvailableLocations(int MeetingID)
        {
            var locations = MeetingClass.CResource.GetAvailableLocations(MeetingID);
            string str = @"<option value=""{0}""> {1} </option>";
            string finalresult = "";
            foreach (var t in locations)
            {
                finalresult += string.Format(str, t.LocationID, t.LocationName);
            }
            finalresult += string.Format(str, "0", "Not Set");
            return finalresult;
        }
        #endregion

        #region Discussion
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetDiscussions(int meetingID)
        {
            var result = DiscussionClass.MeetingDiscussion.GetListMeetingDiscussions(meetingID);
            string disScript = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowDiscussion(this); return false;"">{1}</a></p><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p><hr />";
            string finalString = "";
            foreach (var t in result)
            {
                finalString += string.Format(disScript, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
            }
            return finalString;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveDiscussion(int meetingID, string title, string text)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var t= Watcher.DiscussionWatcher.SaveMeetingDiscussion(title, text, meetingID, user);
            string markup = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowDiscussion(this); return false;"">{1}</a></p><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p><hr />";
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
            var t = DiscussionClass.MeetingDiscussion.GetDiscussionHead(disId);
            string head = string.Format(mainthread, t.UserProfile.UserName, t.Date.ToString("MM/dd/yyyy"), t.Date.ToString("hh:mm:ss tt"), t.Title, t.Text);
            // get replies
            var rep = DiscussionClass.MeetingDiscussion.GetDiscussionReplies(disId);
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
        #endregion

        #region Agenda
        // Agenda WebMethods
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveAgenda(string count, string title, string desc, int meetingid, int subagenda, bool published)
        {
            string username = HttpContext.Current.User.Identity.Name;
            Watcher.AgendaWatcher.SaveAgenda(username,count, title, desc, meetingid, published, subagenda);
            return "true";
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string FetchAgenda(int meetingID)
        {
            string user = HttpContext.Current.User.Identity.Name;
            var result = AgendaClass.AgendaInfoClass.GetMeetingAgenda(meetingID, user);
            return FormtAgenda(result);
        }

        private static string FormtAgenda(List<AgendaClass.AgendaInfoClass> result)
        {
            // string text = "<tbody>{0}</tbody>";
            StringBuilder rows = new StringBuilder();
            foreach (var t in result)
            {
                string tr = @"<tr id=""" + t.ID + @""" onclick=""highlightRow(this)""><td>" + t.Count + "</td><td>" + t.Title + "</td><td>" + t.Desc + "</td><td>" + t.Published + "</td><td>{0}</td></tr>";
                string html = "";
                if (t.Creator)
                {

                    // add icons 
                    html = @"<a title=""Discussion"" href=""#"" class=""win-commandicon"" onclick=""return AgendaIconClick(" + t.ID + @", 'Discussion')""><span class=""win-commandicon icon-chat-2""></span></a><a title=""Files"" href=""#"" class=""win-commandicon"" onclick=""return AgendaIconClick(" + t.ID + @", 'File')""><span class=""win-commandicon  icon-file-3""></span></a><a title=""Privacy Settings"" href=""#""" +
                               @" class=""win-commandicon"" onclick=""return AgendaIconClick(" + t.ID + @", 'Privacy')""><span class=""win-commandicon icon-locked""></span></a><a href=""#"" class=""win-commandicon"" title=""Tasks"" onclick=""return AgendaIconClick(" + t.ID + @", 'Task')""><span class=""win-commandicon  icon-clipboard""></span></a><a href=""#"" class=""win-commandicon"" title=""Edit"" onclick=""return AgendaIconClick(" + t.ID + @", 'Edit')""><span class=""win-commandicon  icon-pencil""></span></a><a href=""#"" class=""win-commandicon"" title=""Delete"" onclick=""return AgendaIconClick(" + t.ID + @", 'Delete')""><span class=""win-commandicon  icon-trash""></span></a>";

                }
                else
                {
                    // add icons
                    html = @"<a title=""Discussion"" href=""#"" class=""win-commandicon"" onclick=""return AgendaIconClick(" + t.ID + @", 'Discussion')""><span class=""win-commandicon icon-chat-2""></span></a><a title=""Files"" href=""#"" class=""win-commandicon"" onclick=""return AgendaIconClick(" + t.ID + @", 'File')""><span class=""win-commandicon  icon-file-3""></span></a>" +
                        @"<a href=""#"" class=""win-commandicon"" title=""Tasks"" onclick=""return AgendaIconClick(" + t.ID + @", 'Task')""><span class=""win-commandicon  icon-clipboard""></span></a>";
                }
                rows.Append(string.Format(tr, html));
            }

            return rows.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string CheckRole(int meetingID)
        {
            string username = HttpContext.Current.User.Identity.Name;
            bool role = AgendaClass.AgendaPrivacyClass.CheckRole(username, meetingID, MeetingRoles.Creator);
            return role.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static AgendaClass.AgendaInfoClass GetAgenda(int gid)
        {
            var result = AgendaClass.AgendaInfoClass.GetAgenda(gid);
            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateAgenda(int agendaID, string count, string title, string desc, int subagenda, bool published)
        {
            string username = HttpContext.Current.User.Identity.Name;
            Watcher.AgendaWatcher.UpdateAgenda(username, agendaID, count, title, desc, published, subagenda);
            return "true";
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetAgendaPrivacy(int AgendaID)
        {
            return formatAgendaPrivacy(AgendaClass.AgendaPrivacyClass.GetParticipants(AgendaID));
        }
        private static string formatAgendaPrivacy(List<AgendaClass.AgendaPrivacyClass> result)
        {
            string finalResult = "";
            foreach (var t in result)
            {
                string i = "<tr><td>" + t.UserName + "</td><td>" + t.Role + "</td><td>";
                i += @"<label class=""checkbox inline""><input type=""checkbox"" onclick=""ChangePrivacy(this)"" {0}><span style=""width: 100px"" class=""metro-checkbox""></span></label></td></tr>";
                if (t.CanSee)
                {
                    finalResult += string.Format(i, @"checked=""checked""");
                }
                else
                {
                    finalResult += i;
                }
            }
            return finalResult;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static void UpdateAgendaPrivacy(object[] obj, int agendaID)
        {
            AgendaClass.AgendaPrivacyClass.UpdateAgendaPrivacy(agendaID, obj);
        }
        /// <summary>
        /// Agenda tasks start
        /// </summary>
        /// <param name="AgendaID"></param>
        /// <param name="Title"></param>
        /// <param name="Desc"></param>
        /// <param name="DueDate"></param>
        /// <param name="usernames"></param>
        /// <returns></returns>
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveAgendaTask(int AgendaID, string Title, string Desc, DateTime DueDate, string[] usernames)
        {
            string us = HttpContext.Current.User.Identity.Name;
            var t = AgendaClass.AgendaTaskClass.SaveAgendaTask(us, AgendaID, Title, Desc, DueDate, usernames);
            

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
            //string final = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"), t.DueDate.ToString("MM/dd/yyyy"), user, "div" + t.TaskID);
            string final = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), user, "", @"border: 1px solid Red; background-color: #FF4D4D;", "Incomplete");
            final = string.Format(startAndEnd, t.TaskID, final, "", "");
            return final;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetAgendaTasks(int AgendaID)
        {
            string username = HttpContext.Current.User.Identity.Name;
            var result = AgendaClass.AgendaTaskClass.GetAgendaTasks(username, AgendaID);
            var role = AgendaClass.AgendaPrivacyClass.CheckRole(username, MeetingRoles.Creator, AgendaID);
            if (!role)
            {
                role = AgendaClass.AgendaPrivacyClass.CheckRole(username, MeetingRoles.Moderator, AgendaID);
            }
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
                    tasks = string.Format(UserTaskTag1, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), @"style=""color:Red""");
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
                            responses += string.Format(UserResponse1, r.Username, r.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), r.Desc, style, t.TaskID);
                            //responses += string.Format(UserResponse, r.Username, r.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), r.Desc);
                        }
                    }
                }
                if (complete)
                {
                    tasks = string.Format(CreatorTaskTag1, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy"), t.DueDate.ToString("MM/dd/yyyy"), user, "", @"border: 1px solid Green; background-color: #66C266;", "Complete");
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

        private static bool GetCompleteStyle(string response, ref string style)
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
        public static string DeleteAgendaTask(int TaskID)
        {
            var username = HttpContext.Current.User.Identity.Name;
            if (AgendaClass.AgendaTaskClass.DeleteTask(TaskID, username))
                return "true";
            else
                return "false";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static TaskClass GetAgendaTask(int TaskID)
        {
            return AgendaClass.AgendaTaskClass.GetTask(TaskID);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string UpdateAgendaTask(int TaskID, int AgendaID, string Title, string Desc, DateTime DueDate, string[] usernames)
        {
            string us = HttpContext.Current.User.Identity.Name;
            var t = AgendaClass.AgendaTaskClass.UpdateAgendaTask(TaskID, us, AgendaID, Title, Desc, DueDate, usernames);

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
            var role = AgendaClass.AgendaPrivacyClass.CheckRole(username, MeetingRoles.Creator, AgendaID);
            if (!role)
            {
                role = AgendaClass.AgendaPrivacyClass.CheckRole(username, MeetingRoles.Moderator, AgendaID);
            }
            StringBuilder final = new StringBuilder();
            string tasks = "";
            string responses = "";
            string tool = "";
            GetTaskResponses(username, role, final, ref tasks, ref responses, tool, t);

            return final.ToString();
            //string txt = @"<div class=""btn-group pull-right""><button class=""btn dropdown-toggle"" style=""background-color: #F5F5F5; border-color:#F5F5F5"" data-toggle=""dropdown"">" +
            //             @"<span class=""caret""></span></button><ul aria-labelledby=""dLabel"" role=""menu"" class=""dropdown-menu"">" +
            //             @"<li><a id=""{0}"" onclick=""return EditAgendaTask(this)"" href=""#"">Edit</a></li><li><a id=""{1}"" onclick=""return DeleteAgendaTaskConfirm(this)"" href=""#"">Delete</a></li></ul></div>" +
            //             @"<table><tr><td style=""width:100px"">Action:</td><td>{2}</td></tr><tr><td>Describtion</td><td>{3}</td></tr><tr><td>Date Created</td><td>{4}</td></tr>"
            //             + @"<tr><td>Due Date</td><td>{5}</td></tr><tr><td>By Users</td><td>{6}</td></tr></table>";

            //string final = string.Format(txt, "Edit." + t.TaskID, "Delete." + t.TaskID, t.Title, t.Desc, t.CreatedDate.ToString("MM/dd/yyyy hh:mm:ss tt"), t.DueDate.ToString("MM/dd/yyyy"), user);
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
                    //DateTime y = (DateTime)res.ResponseDate;
                    //return string.Format(UserUserResponse, username, res.Response, y.ToString("MM/dd/yyyy hh:mm:ss tt"), res.Desc, TaskID) +"<br/>";
                    DateTime y = (DateTime)res.ResponseDate;
                    string style = "";
                    GetCompleteStyle(res.Response, ref style);
                    return @"<div class=""row-fluid"">" + string.Format(UserUserResponse1, username, res.Response, y.ToString("MM/dd/yyyy"), res.Desc, TaskID, style) + "</div>";
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
                return string.Format(ResponseTool, TaskID, TaskID, TaskID, TaskID,res.Response, res.Desc) ;
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region Agenda Discussion

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetAgendaDiscussions(int AgendaID)
        {
            var result = DiscussionClass.AgendaDiscussion.GetListAgendaDiscussions(AgendaID);
            string disScript = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowAgendaDiscussion(this); return false;"">{1}</a></p><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p><hr />";
            string finalString = "";
            foreach (var t in result)
            {
                finalString += string.Format(disScript, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
            }
            return finalString;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string SaveAgendaDiscussion(int AgendaID, string title, string text)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var t = Watcher.DiscussionWatcher.SaveAgendaDiscussion(title, text, AgendaID, user);
            string markup = @"<p><a href=""#"" title= ""{0}"" onclick=""ShowAgendaDiscussion(this); return false;"">{1}</a></p><p class=""pull-right""><b> Created by:</b> {2}<b> On:</b> {3}<b> Replys: </b>{4}</p><hr />";
            return string.Format(markup, t.ID, t.Title, t.UserName, t.Date.ToString("MM/dd/yyyy | hh:mm:ss"), t.Replies.ToString());
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string GetAgendaDiscussion(int disId)
        {
            string mainthread = @"<div id=""AgendaMainThread"" class=""row-fluid mainBox""><div id=""AgendaMainThreadInfo"" class=""span2"" style=""margin-left: 25px; margin-top: 10px"">
                                <b>{0} </b><br /> {1} <br />{2} PM</div>
                                <div class=""span7"" style=""margin-right: 25px; margin-top: 10px; margin-bottom:10px"">
                                <div id=""AgendaMainThreadTitle"" class=""row-fluid"">{3}</div><hr />
                                <div id=""AgendaMainThreadText"" class=""row-fluid"">{4}
                                </div></div></div>";

            string replyContainer = @"<div id=""AgendaReplies"" class=""container-fluid"">{0}</div>";
            var t = DiscussionClass.MeetingDiscussion.GetDiscussionHead(disId);
            string head = string.Format(mainthread, t.UserProfile.UserName, t.Date.ToString("MM/dd/yyyy"), t.Date.ToString("hh:mm:ss tt"), t.Title, t.Text);
            // get replies
            var rep = DiscussionClass.MeetingDiscussion.GetDiscussionReplies(disId);
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

        #endregion

    }
}
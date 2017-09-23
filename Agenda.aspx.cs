using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Web.Script.Services;


namespace TheBox
{
    public partial class AgendaP : System.Web.UI.Page
    {
        private static string Username;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Username = User.Identity.Name;
                // fill data
                var tt = AgendaClass.GetAgenda(3, "MAlhazmi");
                foreach (var t in tt)
                {
                    HtmlTableCell cell1 = new HtmlTableCell();
                    HtmlTableCell cell2 = new HtmlTableCell();
                    HtmlTableCell cell3 = new HtmlTableCell();
                    HtmlTableCell cell4 = new HtmlTableCell();
                    HtmlTableCell cell5 = new HtmlTableCell();
                    HtmlTableRow row = new HtmlTableRow();

                    string html = @"<a id=""discussion"" href=""Discussion.aspx?aid=" + t.AgendaID + @""" class=""win-commandicon""><span class=""win-commandicon icon-chat-2""></span></a><a id=""file"" href=""File.aspx?aid=" + t.AgendaID + @""" class=""win-commandicon""><span class=""win-commandicon  icon-file-3""></span></a><a id=""privacy"" title="""" href=""#"" class=""win-commandicon""><span class=""win-commandicon icon-locked""></span></a><a id=""edit"" href=""#"" class=""win-commandicon""><span class=""win-commandicon  icon-pencil""></span></a><a id=""delete"" href=""#"" class=""win-commandicon""><span class=""win-commandicon  icon-trash""></span></a>";
                    cell1.InnerText = t.AgendaCount.ToString();
                    cell2.InnerText = t.AgendaTitle;
                    cell3.InnerText = t.AgendaDesc;
                    cell4.InnerHtml = html;

                    row.Cells.Add(cell1);
                    row.Cells.Add(cell2);
                    row.Cells.Add(cell3);
                    row.Cells.Add(cell5);
                    row.Cells.Add(cell4);
                    tblAgenda.Rows.Add(row);
                }
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<AgendaClass> getAgendaInfo(int meetingID)
        {
            var result = AgendaClass.GetAgendaAjax(meetingID, Username);
            return result;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string DeleteAgendum(int AgendumID)
        {
            var result = AgendaClass.DeleteAgendum(AgendumID);
            return result.ToString();
        }


    }
}
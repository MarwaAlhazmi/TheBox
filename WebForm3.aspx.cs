using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using AjaxControlToolkit;

namespace TheBox
{
    public partial class WebForm3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var y = Protected.BLL.ldap.getListOfUsers();
            foreach (var t in y)
            {
                Response.Write(t);
                Response.Write("<br/>");
            }
        }

     

     

    }
}
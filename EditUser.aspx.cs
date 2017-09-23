using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;

namespace TheBox.Account
{
    public partial class EditUser : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddlPosition.DataSource = EnumUtil.GetValues<Positions>();
                ddlPosition.DataBind();
                
                // get the user
                string username = Request["username"];
                UserProfile user = Profile.GetUser(username);
                if (user != null)
                {
                    tbfname.Text = user.FirstName;
                    tblname.Text = user.LastName;
                    tbuname.Text = user.UserName;
                    ddlPosition.SelectedItem.Text = user.Position;
                    tbemail.Text = user.Email;
                }
                else
                {
                    // TODO: show error invoking user information
                }
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
           bool re = Profile.EditUser(tbfname.Text, tblname.Text, ddlPosition.SelectedItem.Text, tbemail.Text, tbuname.Text);
           if (re)
           {
               // TODO: user saved msg
           }
           else
           {
               // TODO: show error msg
           }

        }

        

        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;

namespace TheBox.Account
{
    public partial class ManageUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // TODO: access restricted to admin users

            // get all users
            GetUsers();
        }

        private void GetUsers()
        {
            var users = Profile.GetAllUsers();
            gvUsers.DataSource = users;
            gvUsers.DataBind();
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            int index = e.NewEditIndex;
            string username = gvUsers.Rows[index].Cells[0].Text;
            Response.Redirect("EditUser.aspx?username=" + username);
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // first Show confirmation dialog
            int index = e.RowIndex;
            string username = gvUsers.Rows[index].Cells[0].Text;
            bool result = Profile.DeleteUser(username);
            if (result)
            {
                GetUsers();
                // TODO: show success message.
            }
            else
            {
                // TODO: show error message
            }


            /*
             * <asp:TemplateField><ItemTemplate><a href="#myModal" role="button" class="btn" data-toggle="modal">Launch demo modal</a></ItemTemplate></asp:TemplateField>
             */
        }
    }
}
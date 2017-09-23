using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using TheBox.Protected.BLL;

namespace TheBox.Account
{
    public partial class Register : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterUser.ContinueDestinationPageUrl = Request.QueryString["ReturnUrl"];
        }

        protected void RegisterUser_CreatedUser(object sender, EventArgs e)
        {
            // get the data
            string uname = "", fname = "", lname = "", email = "", position = "";
            TextBox tbf = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("tbFirst");
            if (tbf != null)
            {
                fname = tbf.Text;
            }
            TextBox tbl = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("tbLast");
            if (tbl != null)
            {
                lname = tbl.Text;
            }
            TextBox tbu = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("UserName");
            if (tbu != null)
            {
                uname = tbu.Text;
            }
            TextBox tbe = (TextBox)RegisterUserWizardStep.ContentTemplateContainer.FindControl("Email");
            if (tbe != null)
            {
                email = tbe.Text;
            }
            DropDownList ddlp = (DropDownList)RegisterUserWizardStep.ContentTemplateContainer.FindControl("ddlPosition");
            if (ddlp != null)
            {
                position = ddlp.SelectedItem.Text;
            }
            // save
           
            Profile p = new Profile();
            if (!p.CreateUser(fname, lname, uname, email, position))
            {
                string error = "an Error occured, please try again!";
                // TODO: 
                // show error msg
            }
            
          
           
            FormsAuthentication.SetAuthCookie(RegisterUser.UserName, false /* createPersistentCookie */);

            string continueUrl = RegisterUser.ContinueDestinationPageUrl;
            if (String.IsNullOrEmpty(continueUrl))
            {
                continueUrl = "~/";
            }
            Response.Redirect(continueUrl);
        }

    }
}

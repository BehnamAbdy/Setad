using System;

public partial class User_Message : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!System.Web.HttpContext.Current.User.IsInRole("Administrator") &&
                !System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.imgMessage.Attributes.Add("disabled", "disabled");
            }
        }
    }
}
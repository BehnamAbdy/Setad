using System;
using System.Web.Security;

public partial class User_NewUser : System.Web.UI.Page
{
    private void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            try
            {
                SupplySystem.User user = new SupplySystem.User
                {
                    UserName = this.txtUserName.Text,
                    PassWord = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtPassword.Text, "SHA1"),
                    FirstName = this.txtName.Text,
                    LastName = this.txtFamily.Text,
                    Gender = (byte)this.drpGender.SelectedIndex,
                    Phone = this.txtPhone.Text
                };
                user.UsersInRoles.Add(new SupplySystem.UsersInRole { RoleID = Public.ToShort(this.drpRoles.SelectedValue), SubmitDate = DateTime.Now });
                SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
                db.Users.InsertOnSubmit(user);
                db.SubmitChanges();
                Response.Redirect("~/Default.aspx");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX_Users"))
                {
                    this.lblMessage.Text = "نام کاربری تکراری میباشد!";
                }
            }
        }
    }
}
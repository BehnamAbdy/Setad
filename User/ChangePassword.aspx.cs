using System;
using System.Linq;
using System.Web;

public partial class User_ChangePassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.txtUserName.Text = HttpContext.Current.User.Identity.Name;
                this.txtUserName.Enabled = false;
            }
            else
            {
                this.txtPassword.Text = "***";
                this.txtPassword.Enabled = false;
            }

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
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            SupplySystem.User user = null;
            if (HttpContext.Current.User.IsInRole("Administrator"))
            {
                user = db.Users.FirstOrDefault<SupplySystem.User>(u => u.UserName.Equals(this.txtUserName.Text));
                if (user != null)
                {
                    user.PassWord = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtNewPassword.Text, "SHA1");
                    db.SubmitChanges();
                    this.lblMessage.Text = "ویرایش گذرواژه انجام گردید";
                }
                else
                {
                    this.lblMessage.Text = "نام کاربری نادرست میباشد";
                }
            }
            else if (HttpContext.Current.User.IsInRole("AreaManager"))
            {
                bool isUserNameValid = false;
                if (HttpContext.Current.User.Identity.Name == this.txtUserName.Text) // he wants to change his own password
                {
                    user = db.Users.FirstOrDefault<SupplySystem.User>(u => u.UserName.Equals(this.txtUserName.Text));
                    if (user != null)
                    {
                        user.PassWord = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtNewPassword.Text, "SHA1");
                        db.SubmitChanges();
                        this.lblMessage.Text = "ویرایش گذرواژه انجام گردید";
                    }
                    isUserNameValid = true;
                }
                else
                {
                    user = (from u in db.Users
                            join ur in db.UsersInRoles on u.UserID equals ur.UserID
                            where u.UserName.Equals(this.txtUserName.Text) &&
                                     ur.AreaCode.GetValueOrDefault() == Public.ToInt(HttpContext.Current.User.Identity.Name)
                            select u).SingleOrDefault<SupplySystem.User>();

                    if (user != null)
                    {
                        user.PassWord = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtNewPassword.Text, "SHA1");
                        db.SubmitChanges();
                        isUserNameValid = true;
                    }
                }

                if (!isUserNameValid)
                {
                    this.lblMessage.Text = "نام کاربری نادرست میباشد";
                }
                else
                {
                    this.lblMessage.Text = "ویرایش گذرواژه انجام گردید";
                }
            }
            else if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                user = db.Users.FirstOrDefault<SupplySystem.User>(u => u.UserName.Equals(this.txtUserName.Text) && u.PassWord.Equals(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtPassword.Text, "SHA1")));
                if (user != null)
                {
                    user.PassWord = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtNewPassword.Text, "SHA1");
                    db.SubmitChanges();
                    this.lblMessage.Text = "ویرایش گذرواژه انجام گردید";
                }
                else
                {
                    this.lblMessage.Text = "نام کاربری نادرست میباشد";
                }
            }

            //this.txtUserName.Text = null;
            this.txtPassword.Text = null;
            this.txtNewPassword.Text = null;
            this.txtRePassword.Text = null;
        }
    }
}
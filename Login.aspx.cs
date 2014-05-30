using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data.Linq;
using System.Web.Security;

public partial class Login : System.Web.UI.Page
{
    protected void btnLogin_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<SupplySystem.User>(u => u.UsersInRoles);
            dlo.LoadWith<SupplySystem.UsersInRole>(ur => ur.Role);
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            db.LoadOptions = dlo;
            SupplySystem.User user = db.Users.FirstOrDefault<SupplySystem.User>(u => u.UserName == this.txtUserName.Text && u.PassWord == FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtPassword.Text, "SHA1"));
            if (user != null) // Credentials are valid
            {
                user.LastLoginDate = DateTime.Now;
                db.SubmitChanges();
                this.Session["User"] = user;
                string roles = null;
                var rolesList = user.UsersInRoles.Select(ur => new { ur.Role.RoleName });
                foreach (var item in rolesList)
                {
                    roles += string.Concat(item.RoleName, ",");
                }
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(2, this.txtUserName.Text, DateTime.Now, DateTime.Now.AddMinutes(30), false, roles.Remove(roles.Length - 1, 1));
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket)));
                FormsAuthentication.GetRedirectUrl(this.txtUserName.Text, false);
                int msgCount = (from msg in db.Messages
                                join umg in db.MessageUsers on msg.MessageID equals umg.MessageID
                                where umg.UserInRoleID == user.UsersInRoles[0].UserInRoleID && !umg.Read
                                select new { msg.MessageID }).Count();
                Response.Redirect(msgCount == 0 ? "~/Default.aspx" : "~/user/Message.aspx");
            }
            else
            {
                this.lblMessage.Text = "نام کاربری یا گذرواژه نادرست میباشد";
            }
        }
    }
}
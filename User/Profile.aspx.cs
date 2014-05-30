using System;
using System.Linq;
using System.Web;

public partial class User_Profile : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                SupplySystem.User user = db.Users.First<SupplySystem.User>(u => u.UserName.Equals(HttpContext.Current.User.Identity.Name));
                this.txtName.Text = user.FirstName;
                this.txtFamily.Text = user.LastName;
                this.drpGender.SelectedIndex = user.Gender.GetValueOrDefault();
                this.txtPhone.Text = user.Phone;
            }
            else
            {
                Server.Transfer("~/Error.aspx");
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            SupplySystem.User user = db.Users.First<SupplySystem.User>(u => u.UserName.Equals(HttpContext.Current.User.Identity.Name));
            user.FirstName = this.txtName.Text;
            user.LastName = this.txtFamily.Text;
            user.Gender = (byte)this.drpGender.SelectedIndex;
            user.Phone = this.txtPhone.Text;
            db.SubmitChanges();
            Response.Redirect("~/Default.aspx");
        }
    }
}

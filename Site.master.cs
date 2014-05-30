using System;
using System.Web;

public partial class Site : System.Web.UI.MasterPage
{
    private string memberName;
    private string memberRole;

    protected string MemberName
    {
        get { return memberName; }
    }

    protected string MemberRole
    {
        get { return memberRole; }
    }

    private void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && HttpContext.Current.User.Identity.IsAuthenticated)
        {
            SupplySystem.User user = this.Session["User"] as SupplySystem.User;
            if (user == null)
            {
                this.imgLogout_Clicked(sender, e);
            }

            this.memberName = string.Format("{0} {1}", user.FirstName, user.LastName);
            if (HttpContext.Current.User.IsInRole("AreaManager"))
            {
                this.memberRole = "مدیر منطقه";
            }
            else if (HttpContext.Current.User.IsInRole("Administrator"))
            {
                this.memberRole = "مدیر سیستم";
            }
            else if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.memberRole = "مدیر آموزشگاه";
            }
            else if (HttpContext.Current.User.IsInRole("Guest"))
            {
                this.memberRole = "بازدید کننده";
            }
        }
    }

    protected void imgLogout_Clicked(object sender, System.EventArgs e)
    {
        this.Session.Clear();
        System.Web.Security.FormsAuthentication.SignOut();
        Response.Redirect("~/Default.aspx");
    }
}

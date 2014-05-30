using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class User_Inbox : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int messageId = 0;
            if (Request.QueryString["mode"] != null && int.TryParse(Request.QueryString["mId"], out messageId))
            {
                int userInRoleId = ((SupplySystem.User)this.Session["User"]).UsersInRoles.First<SupplySystem.UsersInRole>().UserInRoleID;
                switch (Request.QueryString["mode"])
                {
                    case "r": // read mode
                        db.MessageUsers.First<SupplySystem.MessageUser>(mu => mu.MessageID == messageId && mu.UserInRoleID == userInRoleId).Read = true;
                        db.SubmitChanges();
                        Response.Clear();
                        Response.Write("1");
                        Response.End();
                        break;

                    case "d": // delete mode
                        db.MessageUsers.First<SupplySystem.MessageUser>(mu => mu.MessageID == messageId && mu.UserInRoleID == userInRoleId).Deleted = true;
                        db.SubmitChanges();
                        Response.Clear();
                        Response.Write("1");
                        Response.End();
                        break;
                }
            }
        }
    }

    protected void ObjectDataSource1_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["userInRoleId"] = ((SupplySystem.User)this.Session["User"]).UsersInRoles.First<SupplySystem.UsersInRole>().UserInRoleID;
    }

    protected string MessageIconUrl(bool read)
    {
        return read ? "../App_Themes/Default/images/email-1.png" : "../App_Themes/Default/images/email-0.png";
    }
}
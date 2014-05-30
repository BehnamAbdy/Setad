using System.Linq;

public partial class User_Outbox : System.Web.UI.Page
{
    protected void ObjectDataSource1_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["userInRoleId"] = ((SupplySystem.User)this.Session["User"]).UsersInRoles.First<SupplySystem.UsersInRole>().UserInRoleID;
    }
}
using System;
using System.Linq;

public partial class Lists_UsersList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            var areaManagers = from u in db.Users
                               join ur in db.UsersInRoles on u.UserID equals ur.UserID into grp
                               from g in grp.DefaultIfEmpty()
                               where g.RoleID == (short)Public.Role.AreaManager && g.AreaCode == null
                               select new { u.UserID, u.FirstName, u.LastName, u.UserName };
            this.grdUsers.DataSource = areaManagers.ToList();
            this.grdUsers.DataBind();
        }
    }
}

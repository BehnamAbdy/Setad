using System;
using System.Linq;

public partial class Lists_AreasList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            var areas = from ar in db.Areas
                        join ur in db.UsersInRoles on ar.AreaCode equals ur.AreaCode
                        join u in db.Users on ur.UserID equals u.UserID
                        orderby ar.AreaCode
                        select
                        new
                        {
                            ar.AreaCode,
                            ar.AreaName,
                            ur.UserInRoleID,
                            User = string.Concat(u.FirstName, " ", u.LastName)
                        };
            this.grdAreas.DataSource = areas.ToList();
            this.grdAreas.DataBind();
        }
    }
}

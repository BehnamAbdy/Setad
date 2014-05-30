using System;
using System.Linq;
using System.Web;

public partial class Lists_SchoolsList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            var schools = from sch in db.Schools
                          join slv in db.SchoolLevels on sch.SchoolID equals slv.SchoolID
                          join lv in db.Levels on slv.LevelID equals lv.LevelID
                          join ar in db.Areas on sch.AreaCode equals ar.AreaCode
                          join ct in db.Cities on sch.CityID equals ct.CityID into cty
                          from c in cty.DefaultIfEmpty()
                          where slv.LockOutDate == null
                          orderby sch.SchoolCode
                          select
                          new
                          {
                              sch.SchoolCode,
                              sch.SchoolName,
                              sch.Phone,
                              c.Name,
                              lv.LevelName,
                              ar.AreaName,
                              ar.AreaCode,
                          };
            if (System.Web.HttpContext.Current.User.IsInRole("Administrator") && Request.QueryString["ac"] != null)
            {
                schools = from q in schools
                          where q.AreaCode == Public.ToInt(Request.QueryString["ac"])
                          select q;
            }
            else if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
            {
                schools = from q in schools
                          where q.AreaCode == (from u in db.Users join ur in db.UsersInRoles on u.UserID equals ur.UserID where u.UserName.Equals(HttpContext.Current.User.Identity.Name) select ur.AreaCode).SingleOrDefault()
                          select q;
            }
            else if (System.Web.HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                schools = from q in schools
                          where q.SchoolCode == Public.ToInt(HttpContext.Current.User.Identity.Name)
                          select q;
            }
            this.grdSchools.DataSource = schools.ToList();
            this.grdSchools.DataBind();
        }
    }
}

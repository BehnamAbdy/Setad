using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Report_EpmloyeesArchive : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (HttpContext.Current.User.IsInRole("AreaManager"))
            {
                this.btnAreaSearch.Enabled = false;
                this.txtAreaCode.Enabled = false;
                var area = from u in db.Users
                           join ur in db.UsersInRoles on u.UserID equals ur.UserID
                           join ar in db.Areas on ur.AreaCode equals ar.AreaCode
                           where u.UserName == System.Web.HttpContext.Current.User.Identity.Name
                           select new { ar.AreaCode, ar.AreaName };
                foreach (var item in area)
                {
                    this.txtAreaCode.Text = item.AreaCode.ToString();
                    this.txtAreaName.Text = item.AreaName;
                }
            }
            else if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.btnAreaSearch.Enabled = false;
                this.txtAreaCode.Enabled = false;
                this.btnSchoolCodeSearch.Enabled = false;
                this.txtSchoolCode.Enabled = false;
                var schAre = from ar in db.Areas
                             join s in db.Schools on ar.AreaCode equals s.AreaCode
                             where s.SchoolCode == Public.ToInt(HttpContext.Current.User.Identity.Name)
                             select
                             new
                             {
                                 ar.AreaCode,
                                 ar.AreaName,
                                 s.SchoolCode,
                                 s.SchoolName
                             };

                foreach (var item in schAre)
                {
                    this.txtAreaCode.Text = item.AreaCode.ToString();
                    this.txtAreaName.Text = item.AreaName;
                    this.txtSchoolCode.Text = item.SchoolCode.ToString();
                    this.txtSchoolName.Text = item.SchoolName;
                }
            }

            //if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            //{
            //    this.btnSearch.Enabled = false;
            //}
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            this.lstArchive.DataSourceID = "ObjectDataSource1";
            this.ObjectDataSource1.Select();
            this.lstArchive.DataBind();
        }
    }

    protected void ObjectDataSource1_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["dateFrom"] = this.txtDateFrom.GeorgianDate;
        e.InputParameters["dateTo"] = this.txtDateTo.GeorgianDate;
        e.InputParameters["areaCode"] = Public.ToInt(this.txtAreaCode.Text);
        e.InputParameters["schoolCode"] = Public.ToInt(this.txtSchoolCode.Text);
        e.InputParameters["employeeType"] = this.drpEmployeeType.SelectedValue;
    }
}

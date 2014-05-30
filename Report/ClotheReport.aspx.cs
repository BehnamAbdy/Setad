using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Report_ClotheReport : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpCycle.DataSource = db.Cycles;
            this.drpCycle.DataBind();
            this.drpCycle.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
            this.drpCycle.SelectedValue = Public.ActiveCycle.CycleID.ToString();
            this.drpCycle.Enabled = System.Web.HttpContext.Current.User.IsInRole("Administrator");
            this.drpCycle_SelectedIndexChanged(this, e);

            if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
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

            //if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            //{
            //    this.btnSearch.Enabled = false;
            //}
        }
    }

    protected void drpCycle_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.drpCycle.SelectedIndex > 0)
        {
            this.drpStuffs.Items.Clear();
            LoadClothes(Public.ToInt(this.drpCycle.SelectedValue));
        }
        else
        {
            this.drpStuffs.Items.Clear();
            this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
        }
        this.lstClothe.Items.Clear();
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        int schoolCode = 0;
        if (int.TryParse(this.txtSchoolCode.Text, out schoolCode))
        {
            var sch = from s in db.Schools
                      join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                      join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                      join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                      where s.SchoolCode == schoolCode && s.AreaCode == Public.ToInt(this.txtAreaCode.Text) &&
                            !sl.SubLevelName.Equals("Employees") && slv.LockOutDate == null
                      select new
                      {
                          s.SchoolName,
                          ssl.SchoolSubLevelID,
                          sl.SubLevelName
                      };

            this.drpSubLevel.Items.Clear();
            foreach (var item in sch)
            {
                this.drpSubLevel.Items.Add(new ListItem(item.SubLevelName, item.SchoolSubLevelID.ToString()));
                this.txtSchoolName.Text = item.SchoolName;
            }

            if (this.drpSubLevel.Items.Count > 0)
            {
                this.drpSubLevel.Items.Insert(0, new ListItem("همه موارد", "0"));
            }
            else
            {
                this.txtSchoolName.Text = null;
            }
        }
        else
        {
            this.txtSchoolCode.Text = null;
            this.txtSchoolName.Text = null;
            this.drpSubLevel.Items.Clear();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            this.lstClothe.DataSourceID = "ObjectDataSource1";
            this.ObjectDataSource1.Select();
            this.lstClothe.DataBind();
            this.lnkSum_Click(sender, e);
        }
    }

    protected void ObjectDataSource1_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["cycleClotheId"] = Public.ToInt(this.drpStuffs.SelectedValue);
        e.InputParameters["areaCode"] = Public.ToInt(this.txtAreaCode.Text);
        e.InputParameters["schoolCode"] = Public.ToInt(this.txtSchoolCode.Text);
        e.InputParameters["subLevelId"] = Public.ToInt(this.drpSubLevel.SelectedValue);
    }

    protected void lnkSum_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            var query = from s in db.Schools
                        join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                        join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                        join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                        join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                        join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                        join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                        join stf in db.REP_Stuffs on cc.StuffID equals stf.StuffID
                        join a in db.Areas on s.AreaCode equals a.AreaCode
                        where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                                   slv.LockOutDate == null && cc.CycleClotheID == Public.ToInt(this.drpStuffs.SelectedValue)
                        group ssl by new { s.SchoolCode, s.SchoolName, AreaCode = a.AreaCode, a.AreaName, ssl.SchoolSubLevelID, sl.SubLevelName, st.ClotheCount, stf.StuffName, cc.CycleClotheID } into grp
                        select new
                        {
                            grp.Key.SchoolCode,
                            grp.Key.AreaCode,
                            grp.Key.SchoolSubLevelID,
                            ClotheCount = grp.Count()
                        };

            if (!string.IsNullOrEmpty(this.txtAreaCode.Text))
            {
                query = from q in query
                        where q.AreaCode == Public.ToInt(this.txtAreaCode.Text)
                        select q;
            }

            if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
            {
                query = from q in query
                        where q.SchoolCode == Public.ToInt(this.txtSchoolCode.Text)
                        select q;

                if (this.drpSubLevel.SelectedIndex > 0)
                {
                    query = from q in query
                            where q.SchoolSubLevelID == Public.ToShort(this.drpSubLevel.SelectedValue)
                            select q;
                }
            }

            try
            {
                this.lblTotal.Text = query.Sum(q => q.ClotheCount == null ? 0 : q.ClotheCount).ToString();
            }
            catch
            {
                this.lblTotal.Text = "0";
            }
        }
    }

    private void LoadClothes(int cycleId)
    {
        this.drpStuffs.DataSource = from st in db.REP_Stuffs
                                    join cc in db.CycleClothes on st.StuffID equals cc.StuffID
                                    where cc.CycleID == cycleId
                                    select new
                                    {
                                        cc.CycleClotheID,
                                        st.StuffName
                                    };
        this.drpStuffs.DataBind();
        this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
    }
}

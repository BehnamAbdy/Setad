using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Report_SchoolClotheRep : System.Web.UI.Page
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

    protected void drpCycle_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.drpCycle.SelectedIndex > 0)
        {
            this.drpStuffs.Items.Clear();

            int cycleId = Public.ToInt(this.drpCycle.SelectedValue);
            LoadClothes(cycleId);
        }
        else
        {
            this.drpStuffs.Items.Clear();
            this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
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
    }

    protected void lnkSum_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            var query = from s in db.Schools
                        join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                        join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                        join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                        join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                        join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                        join stf in db.REP_Stuffs on cc.StuffID equals stf.StuffID
                        join a in db.Areas on s.AreaCode equals a.AreaCode
                        where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                                 slv.LockOutDate == null && cc.CycleClotheID == Public.ToInt(this.drpStuffs.SelectedValue)
                        group ssl by new { s.SchoolCode, s.SchoolName, a.AreaCode, a.AreaName, st.ClotheCount, stf.StuffName, cc.CycleClotheID } into grp
                        select new
                        {
                            grp.Key.SchoolCode,
                            grp.Key.AreaCode,
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
            }

            try
            {
                this.lblTotal.Text = query.Sum(q => q.ClotheCount == null ? 0 : q.ClotheCount).ToString();
            }
            catch
            {
                this.lblTotal.Text = null;
            }
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            var query = from s in db.Schools
                        join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                        join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                        join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                        join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                        join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                        join stf in db.REP_Stuffs on cc.StuffID equals stf.StuffID
                        join a in db.Areas on s.AreaCode equals a.AreaCode
                        join lv in db.Levels on slv.LevelID equals lv.LevelID
                        where a.ProvinceID == 71 && cc.CycleID == Public.ToInt(this.drpCycle.SelectedValue) &&
                              slv.LockOutDate == null && cc.CycleClotheID == Public.ToInt(this.drpStuffs.SelectedValue)
                        group ssl by new { s.SchoolCode, s.SchoolName, lv.LevelName, a.AreaCode, a.AreaName, st.ClotheCount, stf.StuffName, cc.CycleClotheID } into grp
                        orderby grp.Key.SchoolCode
                        select new
                        {
                            grp.Key.SchoolCode,
                            grp.Key.SchoolName,
                            grp.Key.LevelName,
                            grp.Key.AreaCode,
                            grp.Key.AreaName,
                            grp.Key.StuffName,
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
            }

            var result = from q in query
                         group q by new { q.SchoolCode, q.SchoolName, q.AreaName, q.StuffName } into grp
                         select new
                         {
                             grp.Key.SchoolCode,
                             grp.Key.SchoolName,
                             grp.Key.AreaName,
                             grp.Key.StuffName,
                             ClotheCount = grp.Sum(s => s.ClotheCount)
                         };

            DataTable dtObj = new DataTable();
            dtObj.Columns.Add(new DataColumn("SchoolCode", typeof(int)));
            dtObj.Columns.Add(new DataColumn("SchoolName", typeof(string)));
            dtObj.Columns.Add(new DataColumn("AreaName", typeof(string)));
            dtObj.Columns.Add(new DataColumn("GoodName", typeof(string)));
            dtObj.Columns.Add(new DataColumn("ClotheCount", typeof(short)));
            dtObj.Columns.Add(new DataColumn("LevelName", typeof(string)));

            foreach (var item in query)
            {
                DataRow row = dtObj.NewRow();
                row[0] = item.SchoolCode;
                row[1] = item.SchoolName;
                row[2] = item.AreaName;
                row[3] = item.StuffName;
                row[4] = item.ClotheCount;
                row[5] = item.LevelName;
                dtObj.Rows.Add(row);
            }

            dtObj.TableName = "dt";
            Stimulsoft.Report.StiReport report = new Stimulsoft.Report.StiReport();
            report.RegData(dtObj);
            report.Load(HttpContext.Current.Server.MapPath("~/App_Data/Report/mrt/sch_clothe.mrt"));
            report.Render();
            Public.ExportInfo(3, report);
            report.Dispose();
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
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Report_SchoolPaperityRep : System.Web.UI.Page
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
            this.drpMonths.Items.Clear();
            this.drpStuffs.Items.Clear();

            int cycleId = Public.ToInt(this.drpCycle.SelectedValue);
            Public.LoadCycleMonths(this.drpMonths, cycleId);
            LoadPaperity(cycleId);
        }
        else
        {
            this.drpMonths.Items.Clear();
            this.drpStuffs.Items.Clear();
            this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
        }
        this.lstPaperity.Items.Clear();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            this.lstPaperity.DataSourceID = "ObjectDataSource1";
            this.ObjectDataSource1.Select();
            this.lstPaperity.DataBind();
            this.lnkSum_Click(sender, e);
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            int cycleId = Public.ToInt(this.drpCycle.SelectedValue);
            int cyclePaperityId = Public.ToInt(this.drpStuffs.SelectedValue);
            var query = from sp in db.SchoolPaperities
                        join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                        join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                        join s in db.Schools on slv.SchoolID equals s.SchoolID
                        join lv in db.Levels on slv.LevelID equals lv.LevelID
                        join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                        join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                        join st in db.REP_Stuffs on cp.StuffID equals st.StuffID
                        join ar in db.Areas on s.AreaCode equals ar.AreaCode
                        where cp.CycleID == cycleId && slv.LockOutDate == null &&
                                 cp.CyclePaperityID == cyclePaperityId
                        group ssl by new
                        {
                            s.SchoolID,
                            s.SchoolCode,
                            s.SchoolName,
                            lv.LevelName,
                            ar.AreaCode,
                            ar.AreaName,
                            st.StuffName,
                            sp.CyclePaperityID,
                            cl.SolarYear,
                            cl.SolarMonth,
                            cl.SolarDay
                        } into grp
                        orderby grp.Key.SchoolCode, grp.Key.SolarYear, grp.Key.SolarMonth, grp.Key.SolarDay
                        select
                        new
                        {
                            grp.Key.SchoolID,
                            grp.Key.SchoolCode,
                            grp.Key.SchoolName,
                            grp.Key.LevelName,
                            grp.Key.AreaCode,
                            grp.Key.AreaName,
                            grp.Key.StuffName,
                            grp.Key.CyclePaperityID,
                            grp.Key.SolarYear,
                            grp.Key.SolarMonth,
                            grp.Key.SolarDay,
                            RationDate = string.Concat(grp.Key.SolarYear, "/", grp.Key.SolarMonth),
                            Ration = (from sp2 in db.SchoolPaperities
                                      join cl2 in db.Calendars on sp2.CalendarID equals cl2.CalendarID
                                      join ssl2 in db.SchoolSubLevels on sp2.SchoolSubLevelID equals ssl2.SchoolSubLevelID
                                      join slv2 in db.SchoolLevels on ssl2.SchoolLevelID equals slv2.SchoolLevelID
                                      join cp2 in db.CyclePaperities on sp2.CyclePaperityID equals cp2.CyclePaperityID
                                      where slv2.SchoolID == grp.Key.SchoolID && slv2.LockOutDate == null &&
                                               cp2.CyclePaperityID == cyclePaperityId && cl2.SolarYear == grp.Key.SolarYear &&
                                               cl2.SolarMonth == grp.Key.SolarMonth && cl2.SolarDay == grp.Key.SolarDay
                                      select
                                      new
                                      {
                                          sp2.PaperityCount
                                      }).Sum(s => s.PaperityCount)
                        };

            if (this.drpMonths.SelectedIndex > 0)
            {
                string[] monthVals = this.drpMonths.SelectedValue.Split('|');
                short solarYear = Public.ToShort(monthVals[0]);
                byte solarMonth = Public.ToByte(monthVals[1]);
                query = from q in query
                        where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                        select q;
            }

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

            DataTable dtObj = new DataTable();
            db.Connection.Open();
            dtObj.Load(db.GetCommand(query).ExecuteReader());
            db.Connection.Close();
            dtObj.TableName = "dt";
            Stimulsoft.Report.StiReport report = new Stimulsoft.Report.StiReport();
            report.RegData(dtObj);
            report.Load(HttpContext.Current.Server.MapPath("~/App_Data/Report/mrt/sch_ration.mrt"));
            report.Render();
            Public.ExportInfo(3, report);
            report.Dispose();
        }
    }

    protected void ObjectDataSource1_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        short solarYear = 0;
        byte solarMonth = 0;
        if (this.drpMonths.SelectedIndex > 0)
        {
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            solarYear = Public.ToShort(monthVals[0]);
            solarMonth = Public.ToByte(monthVals[1]);
        }

        e.InputParameters["solarYear"] = solarYear;
        e.InputParameters["solarMonth"] = solarMonth;
        e.InputParameters["cyclePaperityId"] = Public.ToShort(this.drpStuffs.SelectedValue);
        e.InputParameters["areaCode"] = Public.ToInt(this.txtAreaCode.Text);
        e.InputParameters["schoolCode"] = Public.ToInt(this.txtSchoolCode.Text);
    }

    protected void lnkSum_Click(object sender, EventArgs e)
    {
        var query = from sp in db.SchoolPaperities
                    join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                    join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             cp.CyclePaperityID == Public.ToInt(this.drpStuffs.SelectedValue) && slv.LockOutDate == null
                    select new
                    {
                        s.SchoolCode,
                        s.AreaCode,
                        ssl.SchoolSubLevelID,
                        sp.PaperityCount,
                        cl.SolarYear,
                        cl.SolarMonth
                    };

        if (this.drpMonths.SelectedIndex > 0)
        {
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

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
            this.lblTotal.Text = query.Sum(q => q.PaperityCount == null ? 0 : q.PaperityCount).ToString();
        }
        catch
        {
            this.lblTotal.Text = null;
        }
    }

    private void LoadPaperity(int cycleId)
    {
        this.drpStuffs.DataSource = from cp in db.CyclePaperities
                                    join st in db.REP_Stuffs on cp.StuffID equals st.StuffID
                                    where cp.CycleID == Public.ToInt(this.drpCycle.SelectedValue)
                                    select new { st.StuffName, cp.CyclePaperityID };
        this.drpStuffs.DataBind();
        this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
    }
}
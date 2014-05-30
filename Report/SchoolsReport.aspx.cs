using System;
using System.Linq;
using System.Data;

public partial class Report_SchoolsReport : System.Web.UI.Page
{
    protected string visibility = System.Web.HttpContext.Current.User.IsInRole("Administrator") ? "visible" : "hidden";
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int schoolCode = 0;
            if (int.TryParse(Request.QueryString["id"], out schoolCode) && System.Web.HttpContext.Current.User.IsInRole("Administrator"))// delete mode
            {
                SupplySystem.School sch = db.Schools.First<SupplySystem.School>(s => s.SchoolCode == schoolCode);
                db.Users.DeleteOnSubmit(db.Users.First<SupplySystem.User>(u => u.UserName == sch.SchoolCode.ToString()));
                db.Schools.DeleteOnSubmit(sch);
                db.SubmitChanges();
                Response.Clear();
                Response.Write("1");
                Response.End();
            }

            this.drpSchoolLevel.DataSource = db.Levels;
            this.drpSchoolLevel.DataBind();
            this.drpSchoolLevel.Items.Insert(0, "-- انتخاب کنید --");
            this.drpSchoolKind.DataSource = db.SchoolKinds;
            this.drpSchoolKind.DataBind();
            this.drpSchoolKind.Items.Insert(0, "-- انتخاب کنید --");

            if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
            {
                this.dvUtility.Visible = false;
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
                this.txtAreaCode.Enabled = false;
                this.btnArea.Enabled = false;
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSearch.Enabled = false;
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.lstSchools.DataSourceID = "ObjectDataSource1";
        this.ObjectDataSource1.Select();
        this.lstSchools.DataBind();
        this.lnkSum_Click(sender, e);
    }

    protected void lnkSum_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            var query = from s in db.Schools
                        join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                        join lv in db.Levels on slv.LevelID equals lv.LevelID
                        join a in db.Areas on s.AreaCode equals a.AreaCode
                        where slv.LockOutDate == null
                        select
                        new
                        {
                            s.SchoolCode,
                            s.SchoolName,
                            s.Gender,
                            s.SchoolKindID,
                            s.EmployeesCount_Changable,
                            s.EmployeesCount_Fixed,
                            lv.LevelID,
                            a.AreaCode,
                            GirlsCount = GetGirlsCount(s),
                            BoysCount = GetBoysCount(s)
                        };

            if (!string.IsNullOrEmpty(this.txtSchoolCode_From.Text) && string.IsNullOrEmpty(this.txtSchoolCode_To.Text))
            {
                query = from q in query
                        where q.SchoolCode == Public.ToInt(this.txtSchoolCode_From.Text)
                        select q;
            }
            else if (!string.IsNullOrEmpty(this.txtSchoolCode_From.Text) && !string.IsNullOrEmpty(this.txtSchoolCode_To.Text))
            {
                query = from q in query
                        where q.SchoolCode >= Public.ToInt(this.txtSchoolCode_From.Text) && q.SchoolCode <= Public.ToInt(this.txtSchoolCode_To.Text)
                        select q;
            }

            if (!string.IsNullOrEmpty(this.txtSchoolName.Text))
            {
                query = from q in query
                        where q.SchoolName.Contains(this.txtSchoolName.Text)
                        select q;
            }

            if (this.drpSchoolLevel.SelectedIndex > 0)
            {
                query = from q in query
                        where q.LevelID == Public.ToShort(this.drpSchoolLevel.SelectedValue)
                        select q;
            }

            if (this.drpSchoolKind.SelectedIndex > 0)
            {
                query = from q in query
                        where q.SchoolKindID == Public.ToShort(this.drpSchoolKind.SelectedValue)
                        select q;
            }

            if (this.drpGender.SelectedIndex < 3)
            {
                query = from q in query
                        where q.Gender == this.drpGender.SelectedIndex
                        select q;
            }

            if (!string.IsNullOrEmpty(this.txtAreaCode.Text))
            {
                query = from q in query
                        where q.AreaCode == Public.ToInt(this.txtAreaCode.Text)
                        select q;
            }

            var result = query.ToList();
            int boysCount = result.Sum(q => q.BoysCount);
            int girlsCount = result.Sum(q => q.GirlsCount);
            int? emplFixCount = result.Sum(q => q.EmployeesCount_Fixed);
            int? emplChgCount = result.Sum(q => q.EmployeesCount_Changable);

            this.td_bc.InnerHtml = boysCount.ToString();
            this.td_gc.InnerHtml = girlsCount.ToString();
            this.td_bgc.InnerHtml = (boysCount + girlsCount).ToString();
            this.td_fc.InnerHtml = emplFixCount.ToString();
            this.td_cc.InnerHtml = emplChgCount.ToString();
            this.td_fcc.InnerHtml = (emplFixCount + emplChgCount).ToString();
            this.td_sum.InnerHtml = (boysCount + girlsCount + emplFixCount + emplChgCount).ToString();
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            var query = from s in db.Schools
                        join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                        join lv in db.Levels on slv.LevelID equals lv.LevelID
                        join a in db.Areas on s.AreaCode equals a.AreaCode
                        where slv.LockOutDate == null
                        orderby a.AreaCode, s.SchoolCode
                        select
                        new
                        {
                            s.SchoolID,
                            s.SchoolCode,
                            s.SchoolName,
                            s.Gender,
                            s.SchoolKindID,
                            s.EmployeesCount_Changable,
                            s.EmployeesCount_Fixed,
                            lv.LevelID,
                            lv.LevelName,
                            a.AreaName,
                            a.AreaCode,
                            GirlsCount = GetGirlsCount(s),
                            BoysCount = GetBoysCount(s)
                        };

            if (!string.IsNullOrEmpty(this.txtSchoolCode_From.Text) && string.IsNullOrEmpty(this.txtSchoolCode_To.Text))
            {
                query = from q in query
                        where q.SchoolCode == Public.ToInt(this.txtSchoolCode_From.Text)
                        select q;
            }
            else if (!string.IsNullOrEmpty(this.txtSchoolCode_From.Text) && !string.IsNullOrEmpty(this.txtSchoolCode_To.Text))
            {
                query = from q in query
                        where q.SchoolCode >= Public.ToInt(this.txtSchoolCode_From.Text) && q.SchoolCode <= Public.ToInt(this.txtSchoolCode_To.Text)
                        select q;
            }

            if (!string.IsNullOrEmpty(this.txtSchoolName.Text))
            {
                query = from q in query
                        where q.SchoolName.Contains(this.txtSchoolName.Text)
                        select q;
            }

            if (this.drpSchoolLevel.SelectedIndex > 0)
            {
                query = from q in query
                        where q.LevelID == Public.ToShort(this.drpSchoolLevel.SelectedValue)
                        select q;
            }

            if (this.drpSchoolKind.SelectedIndex > 0)
            {
                query = from q in query
                        where q.SchoolKindID == Public.ToShort(this.drpSchoolKind.SelectedValue)
                        select q;
            }

            if (this.drpGender.SelectedIndex < 3)
            {
                query = from q in query
                        where q.Gender == this.drpGender.SelectedIndex
                        select q;
            }

            if (!string.IsNullOrEmpty(this.txtAreaCode.Text))
            {
                query = from q in query
                        where q.AreaCode == Public.ToInt(this.txtAreaCode.Text)
                        select q;
            }

            DataTable dtObj = new DataTable();
            dtObj.Columns.Add(new DataColumn("SchoolCode", typeof(int)));
            dtObj.Columns.Add(new DataColumn("SchoolName", typeof(string)));
            dtObj.Columns.Add(new DataColumn("LevelName", typeof(string)));
            dtObj.Columns.Add(new DataColumn("AreaName", typeof(string)));
            dtObj.Columns.Add(new DataColumn("BoysCount", typeof(short)));
            dtObj.Columns.Add(new DataColumn("GirlsCount", typeof(short)));
            dtObj.Columns.Add(new DataColumn("StudentsCount", typeof(short)));
            dtObj.Columns.Add(new DataColumn("EmployeesCount_Fixed", typeof(byte)));
            dtObj.Columns.Add(new DataColumn("EmployeesCount_Changable", typeof(byte)));
            dtObj.Columns.Add(new DataColumn("Sum", typeof(short)));

            foreach (var item in query)
            {
                DataRow row = dtObj.NewRow();
                row[0] = item.SchoolCode;
                row[1] = item.SchoolName;
                row[2] = item.LevelName;
                row[3] = item.AreaName;
                row[4] = item.BoysCount;
                row[5] = item.GirlsCount;
                row[6] = item.BoysCount + item.GirlsCount;
                row[7] = item.EmployeesCount_Fixed;
                row[8] = item.EmployeesCount_Changable;
                row[9] = item.BoysCount + item.GirlsCount + item.EmployeesCount_Fixed + item.EmployeesCount_Changable;
                dtObj.Rows.Add(row);
            }

            dtObj.TableName = "dt";
            Stimulsoft.Report.StiReport report = new Stimulsoft.Report.StiReport();
            report.RegData(dtObj);
            report.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Report/mrt/sch.mrt"));
            report.Render();
            Public.ExportInfo(3, report);
            report.Dispose();
        }
    }

    protected void btnChangeCode_Click(object sender, EventArgs e)
    {
        SupplySystem.School school = db.Schools.FirstOrDefault<SupplySystem.School>(s => s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text));
        if (school != null)
        {
            school.SchoolCode = Public.ToInt(this.txtSchoolNewCode.Text);
            db.Users.First<SupplySystem.User>(u => u.UserName == this.txtSchoolCode.Text.Trim()).UserName = this.txtSchoolNewCode.Text.Trim();
            db.SubmitChanges();
            this.txtSchoolCode.Text = null;
            this.txtSchoolNewCode.Text = null;
            this.lblMessage.Text = "ویرایش  کد آموزشگاه انجام گردبد";
        }
        else
        {
            this.lblMessage.Text = "آموزشگاهی با این کد یافت نشد";
        }
    }

    private short GetGirlsCount(SupplySystem.School sch)
    {
        short count = 0;
        foreach (SupplySystem.SchoolSubLevel ssl in sch.SchoolLevels.First<SupplySystem.SchoolLevel>(slv => slv.LockOutDate == null).SchoolSubLevels)
        {
            count += ssl.GirlsCount.GetValueOrDefault();
        }
        return count;
    }

    private short GetBoysCount(SupplySystem.School sch)
    {
        short count = 0;
        foreach (SupplySystem.SchoolSubLevel ssl in sch.SchoolLevels.First<SupplySystem.SchoolLevel>(slv => slv.LockOutDate == null).SchoolSubLevels)
        {
            count += ssl.BoysCount.GetValueOrDefault();
        }
        return count;
    }

    protected void ObjectDataSource1_Selecting(object sender, System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["schoolCode_From"] = Public.ToInt(this.txtSchoolCode_From.Text);
        e.InputParameters["schoolCode_To"] = Public.ToInt(this.txtSchoolCode_To.Text);
        e.InputParameters["schoolName"] = this.txtSchoolName.Text.Trim();
        e.InputParameters["levelId"] = Public.ToShort(this.drpSchoolLevel.SelectedValue);
        e.InputParameters["schoolKindId"] = Public.ToShort(this.drpSchoolKind.SelectedValue);
        e.InputParameters["gender"] = Convert.ToByte(this.drpGender.SelectedIndex);
        e.InputParameters["areaCode"] = Public.ToInt(this.txtAreaCode.Text);
    }
}

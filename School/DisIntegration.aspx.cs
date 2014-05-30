using System;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI.WebControls;

public partial class School_DisIntegration : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        bool isCodeValid = false;
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text) && !string.IsNullOrEmpty(this.txtAreaCode.Text))
        {
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         join a in db.Areas on s.AreaCode equals a.AreaCode
                         where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) &&
                               s.AreaCode == Public.ToInt(this.txtAreaCode.Text)
                         select new { s.SchoolID, s.SchoolName, s.Gender, lv.LevelName };

            foreach (var sch in school)
            {
                this.hdnSchoolId.Value = sch.SchoolID.ToString();
                this.txtSchoolName.Text = sch.SchoolName;
                this.txtGender.Text = Public.GetGender(sch.Gender);
                this.txtLevel.Text = sch.LevelName;
                isCodeValid = true;

                var subLevels = from slv in db.SchoolLevels
                                join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                                where slv.SchoolID == sch.SchoolID && slv.LockOutDate == null
                                select
                                new
                                {
                                    sl.SubLevelName,
                                    ssl.SubLevelID,
                                    ssl.BoysCount,
                                    ssl.GirlsCount
                                };

                var disIntegs = from dt in db.Integrations
                                where dt.SchoolID == sch.SchoolID &&
                                      dt.CycleID == Public.ActiveCycle.CycleID &&
                                      dt.IntegrationMode == (byte)Public.IntegrationMode.DisIntegration
                                select new { dt.IntegrationID, dt.SchoolID, dt.IntegrationMode };
                foreach (var item in disIntegs)
                {
                    this.hdnDisIntegID.Value = item.IntegrationID.ToString();
                }

                if (!string.IsNullOrEmpty(this.hdnDisIntegID.Value))
                {
                    var disIntgSch = from dts in db.IntegratedSchools
                                     join s in db.Schools on dts.SchoolID equals s.SchoolID
                                     join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                                     join lv in db.Levels on slv.LevelID equals lv.LevelID
                                     join ar in db.Areas on s.AreaCode equals ar.AreaCode
                                     where dts.IntegrationID == Public.ToInt(this.hdnDisIntegID.Value) && slv.LockOutDate == null
                                     select new { s.SchoolID, s.SchoolName, lv.LevelName, ar.AreaName, slv.SubmitDate };
                    this.grdSchools.DataSource = disIntgSch.ToList();
                    this.grdSchools.DataBind();
                }
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnAdd.Enabled = false;
            }
        }

        if (!isCodeValid)
        {
            this.hdnSchoolId.Value = null;
            this.hdnDisIntegID.Value = null;
            this.txtSchoolName.Text = null;
            this.txtGender.Text = null;
            this.txtLevel.Text = null;
            this.grdSchools.Items.Clear();
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int schoolId = 0;
        if (int.TryParse(this.hdnSchoolId.Value, out schoolId) && string.IsNullOrEmpty(this.hdnDisIntegID.Value))
        {
            SupplySystem.Integration itg = new SupplySystem.Integration { SchoolID = schoolId, CycleID = Public.ActiveCycle.CycleID, IntegrationMode = (byte)Public.IntegrationMode.DisIntegration, SubmitDate = DateTime.Now };
            db.Integrations.InsertOnSubmit(itg);
            db.SubmitChanges();
            Response.Redirect(string.Format("~/School/School.aspx?dtg={0}", TamperProofString.QueryStringEncode(itg.IntegrationID.ToString())));
        }
        else if (!string.IsNullOrEmpty(this.hdnDisIntegID.Value))
        {
            Response.Redirect(string.Format("~/School/School.aspx?dtg={0}", TamperProofString.QueryStringEncode(this.hdnDisIntegID.Value)));
        }
    }
}
using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections;

public partial class School_EditLevel : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpSchoolLevel.DataSource = db.Levels;
            this.drpSchoolLevel.DataBind();
            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSaveLevel.Enabled = false;
            }
        }
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        bool isCodeValid = false;
        this.btnSaveLevel.Enabled = false;
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text) && !string.IsNullOrEmpty(this.txtAreaCode.Text))
        {
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         join a in db.Areas on s.AreaCode equals a.AreaCode
                         where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) &&
                               s.AreaCode == Public.ToInt(this.txtAreaCode.Text)
                         select new { s.SchoolID, s.SchoolName, s.Gender, lv.LevelID };

            foreach (var sch in school)
            {
                this.hdnSchoolId.Value = sch.SchoolID.ToString();
                this.txtSchoolName.Text = sch.SchoolName;
                this.drpGender.SelectedIndex = sch.Gender;
                this.drpSchoolLevel.SelectedValue = sch.LevelID.ToString();
                this.ViewState["LevelID"] = sch.LevelID;
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

                ArrayList list = new ArrayList();
                foreach (var item in subLevels)
                {
                    if (!item.SubLevelName.Equals("Employees"))
                    {
                        list.Add(item);
                    }
                }
                this.grdSubLevels.DataSource = list;
                this.grdSubLevels.DataBind();
                this.drpSchoolLevel.Enabled = true;
            }
        }

        if (!isCodeValid)
        {
            this.hdnSchoolId.Value = null;
            this.txtSchoolName.Text = null;
            this.txtAreaName.Text = null;
            this.drpGender.SelectedIndex = 0;
            this.drpSchoolLevel.SelectedIndex = 0;
            this.drpSchoolLevel.Enabled = false;
        }
    }

    protected void drpSchoolLevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.hdnSchoolId.Value))
        {
            short levelId = Public.ToShort(this.drpSchoolLevel.SelectedValue);
            ArrayList subLevelsList = new ArrayList();
            if (this.drpSchoolLevel.SelectedValue == this.ViewState["LevelID"].ToString()) //Current school's level is selected again
            {
                var subLevels = from slv in db.SchoolLevels
                                join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                                where slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) &&
                                      slv.LevelID == levelId && slv.LockOutDate == null
                                select
                                new
                                {
                                    sl.SubLevelName,
                                    ssl.SubLevelID,
                                    ssl.BoysCount,
                                    ssl.GirlsCount
                                };

                foreach (var item in subLevels)
                {
                    if (!item.SubLevelName.Equals("Employees"))
                    {
                        subLevelsList.Add(item);
                    }
                }
                this.btnSaveLevel.Enabled = false;
            }
            else
            {
                var subLevels = from sl in db.SubLevels
                                where sl.LevelID == levelId
                                select
                                new
                                {
                                    sl.SubLevelName,
                                    sl.SubLevelID,
                                    BoysCount = string.Empty,
                                    GirlsCount = string.Empty
                                };

                foreach (var item in subLevels)
                {
                    if (!item.SubLevelName.Equals("Employees"))
                    {
                        subLevelsList.Add(item);
                    }
                    else
                    {
                        this.ViewState["EmplSLID"] = item.SubLevelID;
                    }
                }
                this.btnSaveLevel.Enabled = true;
            }

            this.grdSubLevels.DataSource = subLevelsList;
            this.grdSubLevels.DataBind();
        }
    }

    protected void btnSaveLevel_Click(object sender, EventArgs e)
    {
        int schoolId = 0;
        if (int.TryParse(this.hdnSchoolId.Value, out schoolId))
        {
            db.SchoolLevels.Single<SupplySystem.SchoolLevel>(lv => lv.SchoolID == schoolId && lv.LockOutDate == null).LockOutDate = DateTime.Now;
            SupplySystem.SchoolLevel slv = new SupplySystem.SchoolLevel { LevelID = Public.ToShort(this.drpSchoolLevel.SelectedValue), SchoolID = schoolId, SubmitDate = DateTime.Now };
            for (int i = 0; i < this.grdSubLevels.Rows.Count; i++)
            {
                slv.SchoolSubLevels.Add(new SupplySystem.SchoolSubLevel
                {
                    SubLevelID = Public.ToShort(this.grdSubLevels.DataKeys[i].Value)
                  ,
                    BoysCount = Public.ToShort(((TextBox)this.grdSubLevels.Rows[i].FindControl("txtBoysCount")).Text)
                  ,
                    GirlsCount = Public.ToShort(((TextBox)this.grdSubLevels.Rows[i].FindControl("txtGirlsCount")).Text)
                });
            }
            slv.SchoolSubLevels.Add(new SupplySystem.SchoolSubLevel { SubLevelID = Public.ToShort(this.ViewState["EmplSLID"]) });
            db.SchoolLevels.InsertOnSubmit(slv);
            db.SubmitChanges();
            this.hdnSchoolId.Value = null;
            this.drpSchoolLevel.Enabled = false;
            this.btnSaveLevel.Enabled = false;
            this.ViewState.Clear();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
        }
    }

    protected bool ActivateByGender(string gender)
    {
        bool activation = true;
        switch (this.drpGender.SelectedIndex)
        {
            case 0:
                if (gender == "F")
                {
                    activation = false;
                }
                break;

            case 1:
                if (gender == "M")
                {
                    activation = false;
                }
                break;
        }
        return activation;
    }
}
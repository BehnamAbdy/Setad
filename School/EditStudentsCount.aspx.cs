using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Web.UI.WebControls;

public partial class School_EditStudentsCount : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.txtSchoolCode.Text = HttpContext.Current.User.Identity.Name;
                this.txtSchoolCode.Enabled = false;
                this.btnSchoolCodeSearch.Enabled = false;
                txtSchoolCode_TextChanged(sender, e);
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        this.hdnSchoolId.Value = null;
        this.txtSchoolName.Text = null;
        this.txtLevel.Text = null;
        this.txtGender.Text = null;
        this.grdSubLevels.DataSource = null;
        int schoolCode = 0;

        if (int.TryParse(this.txtSchoolCode.Text, out schoolCode))
        {
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         where s.SchoolCode == schoolCode && slv.LockOutDate == null
                         select new { s.SchoolID, s.SchoolCode, s.SchoolName, s.Gender, lv.LevelName, s.AreaCode };

            if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
            {
                var area = from u in db.Users join ur in db.UsersInRoles on u.UserID equals ur.UserID where u.UserName.Equals(HttpContext.Current.User.Identity.Name) select new { ur.AreaCode };
                foreach (var item in area)
                {
                    school = from q in school
                             where q.AreaCode == item.AreaCode
                             select q;
                }
            }
            else if (System.Web.HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                school = from q in school
                         where q.SchoolCode == Public.ToInt(HttpContext.Current.User.Identity.Name)
                         select q;
            }

            foreach (var sch in school)
            {
                this.hdnSchoolId.Value = sch.SchoolID.ToString();
                this.txtSchoolName.Text = sch.SchoolName;
                this.txtLevel.Text = sch.LevelName;
                this.txtGender.Text = Public.GetGender(sch.Gender);
                this.grdSubLevels.DataSource = from slv in db.SchoolLevels
                                               join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                               join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                                               where slv.SchoolID == sch.SchoolID && slv.LockOutDate == null &&
                                                        !sl.SubLevelName.Equals("Employees")
                                               select new
                                               {
                                                   Gender = sch.Gender,
                                                   sl.SubLevelName,
                                                   ssl.SubLevelID,
                                                   ssl.BoysCount,
                                                   ssl.GirlsCount
                                               };
            }
        }
        this.grdSubLevels.DataBind();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.hdnSchoolId.Value))
        {
            IQueryable<SupplySystem.SchoolSubLevel> subLevels = from slv in db.SchoolLevels
                                                                join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                                                join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                                                                where slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) && slv.LockOutDate == null &&
                                                                         !sl.SubLevelName.Equals("Employees")
                                                                select ssl;
            byte index = 0;
            TextBox txtBoys = null;
            TextBox txtGirls = null;
            byte boysCount = 0;
            byte girlsCount = 0;
            foreach (SupplySystem.SchoolSubLevel item in subLevels)
            {
                txtBoys = (TextBox)this.grdSubLevels.Rows[index].FindControl("txtBoysCount");
                txtGirls = (TextBox)this.grdSubLevels.Rows[index].FindControl("txtGirlsCount");
                boysCount = Public.ToByte(txtBoys.Text);
                girlsCount = Public.ToByte(txtGirls.Text);

                if (txtBoys.Enabled && boysCount != item.BoysCount.GetValueOrDefault())
                {
                    db.SchoolSubLevelsArchives.InsertOnSubmit(new SupplySystem.SchoolSubLevelsArchive
                        {
                            SchoolSubLevelID = item.SchoolSubLevelID,
                            Gender = "M",
                            FormerCount = item.BoysCount.GetValueOrDefault(),
                            NextCount = boysCount,
                            SubmitDate = DateTime.Now
                        });
                    item.BoysCount = boysCount;
                }

                if (txtGirls.Enabled && girlsCount != item.GirlsCount.GetValueOrDefault())
                {
                    db.SchoolSubLevelsArchives.InsertOnSubmit(new SupplySystem.SchoolSubLevelsArchive
                     {
                         SchoolSubLevelID = item.SchoolSubLevelID,
                         Gender = "F",
                         FormerCount = item.GirlsCount.GetValueOrDefault(),
                         NextCount = girlsCount,
                         SubmitDate = DateTime.Now
                     });
                    item.GirlsCount = girlsCount;
                }
                index++;
            }

            try
            {
                db.SubmitChanges();
                this.hdnSchoolId.Value = null;
                this.lblMessage.Text = Public.SUCCESSMESSAGE;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate key"))
                {
                    this.lblMessage.Text = "براي هر پايه تنها روزانه يكبار ويرايش آمار پذيرفته ميشود";
                }
            }
        }
    }

    protected bool ActivateByGender(byte gender, string mode)
    {
        bool activation = true;
        switch (gender)
        {
            case 0:
                if (mode == "F")
                {
                    activation = false;
                }
                break;

            case 1:
                if (mode == "M")
                {
                    activation = false;
                }
                break;
        }
        return activation;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class School_EditGender : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        bool isCodeValid = false;
        this.btnSave.Enabled = false;
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
        {
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text)
                         select new
                         {
                             s.SchoolID,
                             s.SchoolCode,
                             s.AreaCode,
                             s.SchoolName,
                             s.Gender,
                             lv.LevelName
                         };

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
                this.ViewState["Gender"] = sch.Gender;
                this.txtGender.Text = Public.GetGender(sch.Gender);
                this.btnSave.Enabled = true;
                isCodeValid = true;
                var subLevels = from slv in db.SchoolLevels
                                join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                                where slv.SchoolID == sch.SchoolID && slv.LockOutDate == null && !sl.SubLevelName.Equals("Employees")
                                select
                                new
                                {
                                    sch.Gender,
                                    sl.SubLevelName,
                                    ssl.SubLevelID,
                                    ssl.BoysCount,
                                    ssl.GirlsCount
                                };
                this.grdSubLevels.DataSource = subLevels.ToList();
                this.grdSubLevels.DataBind();
            }
        }

        if (!isCodeValid)
        {
            this.hdnSchoolId.Value = null;
            this.txtSchoolName.Text = null;
            this.txtGender.Text = null;
            this.drpGender.SelectedIndex = 0;
        }
    }

    protected void drpGender_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.hdnSchoolId.Value))
        {
            var subLevels = from s in db.Schools
                            join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                            join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                            join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                            where slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) && slv.LockOutDate == null && !sl.SubLevelName.Equals("Employees")
                            select
                            new
                            {
                                s.Gender,
                                sl.SubLevelName,
                                ssl.SubLevelID,
                                ssl.BoysCount,
                                ssl.GirlsCount
                            };
            this.grdSubLevels.DataSource = subLevels.ToList();
            this.grdSubLevels.DataBind();

            TextBox txt = null;
            RequiredFieldValidator rfv = null;
            switch (this.drpGender.SelectedIndex)
            {
                case 1: // Male
                    foreach (GridViewRow row in this.grdSubLevels.Rows)
                    {
                        txt = (TextBox)row.FindControl("txtBoysCount");
                        rfv = (RequiredFieldValidator)row.FindControl("rfvBoys");
                        txt.Enabled = true;
                        rfv.Enabled = true;
                        txt = (TextBox)row.FindControl("txtGirlsCount");
                        rfv = (RequiredFieldValidator)row.FindControl("rfvGirls");
                        txt.Text = null;
                        txt.Enabled = false;
                        rfv.Enabled = false;
                    }
                    break;

                case 2: // Female
                    foreach (GridViewRow row in this.grdSubLevels.Rows)
                    {
                        txt = (TextBox)row.FindControl("txtBoysCount");
                        rfv = (RequiredFieldValidator)row.FindControl("rfvBoys");
                        txt.Text = null;
                        txt.Enabled = false;
                        rfv.Enabled = false;
                        txt = (TextBox)row.FindControl("txtGirlsCount");
                        rfv = (RequiredFieldValidator)row.FindControl("rfvGirls");
                        txt.Enabled = true;
                        rfv.Enabled = true;
                    }
                    break;

                case 3: // Male & Female
                    foreach (GridViewRow row in this.grdSubLevels.Rows)
                    {
                        txt = (TextBox)row.FindControl("txtBoysCount");
                        rfv = (RequiredFieldValidator)row.FindControl("rfvBoys");
                        txt.Enabled = true;
                        rfv.Enabled = true;
                        txt = (TextBox)row.FindControl("txtGirlsCount");
                        rfv = (RequiredFieldValidator)row.FindControl("rfvGirls");
                        txt.Enabled = true;
                        rfv.Enabled = true;
                    }
                    break;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid)
        {
            byte selectedGender = (byte)(Public.ToByte(this.drpGender.SelectedValue) - 1);
            if (!string.IsNullOrEmpty(this.hdnSchoolId.Value) && this.ViewState["Gender"].ToString() != selectedGender.ToString())
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
                    switch (this.drpGender.SelectedIndex)
                    {
                        case 1: // Boys                           
                            // Setting the previous gender to zero
                            db.SchoolSubLevelsArchives.InsertOnSubmit(new SupplySystem.SchoolSubLevelsArchive
                            {
                                SchoolSubLevelID = item.SchoolSubLevelID,
                                Gender = "F",
                                FormerCount = item.GirlsCount.GetValueOrDefault(),
                                NextCount = 0,
                                SubmitDate = DateTime.Now
                            });
                            item.GirlsCount = null;

                            // Archiving new Counts
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
                            break;

                        case 2: // Girls                            
                            // Setting the previous gender to zero
                            db.SchoolSubLevelsArchives.InsertOnSubmit(new SupplySystem.SchoolSubLevelsArchive
                            {
                                SchoolSubLevelID = item.SchoolSubLevelID,
                                Gender = "M",
                                FormerCount = item.BoysCount.GetValueOrDefault(),
                                NextCount = 0,
                                SubmitDate = DateTime.Now
                            });
                            item.BoysCount = null;

                            // Archiving new Counts
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
                            break;

                        case 3: // Both                           
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

                            // Archiving new Counts
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
                            break;
                    }
                    index++;
                }

                try
                {
                    db.Schools.First<SupplySystem.School>(s => s.SchoolID == Public.ToInt(this.hdnSchoolId.Value)).Gender = selectedGender;
                    db.SubmitChanges();
                    this.hdnSchoolId.Value = null;
                    this.ViewState["Gender"] = null;
                    this.btnSave.Enabled = false;
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
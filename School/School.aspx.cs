using System;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;

public partial class School_School : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpSchoolLevel.DataSource = db.Levels;
            this.drpSchoolLevel.DataBind();
            this.drpSchoolLevel.SelectedIndex = 1;
            this.drpSchoolKind.DataSource = db.SchoolKinds;
            this.drpSchoolKind.DataBind();
            this.drpSchoolKind.SelectedIndex = 7;
            this.txtProvinceCode.Text = ConfigurationManager.AppSettings["DefaultProvinceId"].ToString();
            this.txtProvinceName.Text = ConfigurationManager.AppSettings["DefaultProvinceName"].ToString();

            if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.txtSchoolCode.Text = HttpContext.Current.User.Identity.Name;
                this.txtSchoolCode.ReadOnly = true;
                this.btnSchoolCodeSearch.Enabled = false;
                this.btnAreaSearch.Enabled = false;
                this.txtSchoolCode_TextChanged(sender, e);
            }
            else
            {
                if (HttpContext.Current.User.IsInRole("AreaManager"))
                {
                    var area = from u in db.Users
                               join ur in db.UsersInRoles on u.UserID equals ur.UserID
                               join ar in db.Areas on ur.AreaCode equals ar.AreaCode
                               where u.UserName.Equals(HttpContext.Current.User.Identity.Name)
                               select new { ur.AreaCode, ar.AreaName };

                    foreach (var item in area)
                    {
                        this.txtAreaCode.Text = item.AreaCode.Value.ToString();
                        this.txtAreaName.Text = item.AreaName;
                    }
                }
                LoadSublevels();
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
        {
            db = new SupplySystem.SupplySystem(Public.ConnectionString);
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<SupplySystem.School>(s => s.SchoolLevels);
            dlo.LoadWith<SupplySystem.SchoolLevel>(slv => slv.SchoolSubLevels);
            db.LoadOptions = dlo;
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         join k in db.SchoolKinds on s.SchoolKindID equals k.SchoolKindID
                         join a in db.Areas on s.AreaCode equals a.AreaCode
                         join c in db.Cities on s.CityID equals c.CityID into cty
                         from ct in cty.DefaultIfEmpty()
                         where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) && slv.LockOutDate == null
                         select
                         new
                         {
                             s.SchoolID,
                             s.SchoolCode,
                             s.SchoolName,
                             s.Phone,
                             s.SchoolKindID,
                             s.Village,
                             s.Street,
                             s.Line,
                             s.Pelak,
                             s.PostalCode,
                             s.Gender,
                             s.EmployeesCount_Fixed,
                             s.EmployeesCount_Changable,
                             s.ManagerID,
                             s.AssistanceID,
                             s.AreaCode,
                             s.CityID,
                             CityName = ct.Name,
                             lv.LevelID,
                             lv.LevelName,
                             a.AreaName,
                             k.SchoolKindName
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

            this.hdnSchoolID.Value = null;
            foreach (var sch in school)
            {
                this.hdnSchoolID.Value = sch.SchoolID.ToString();
                this.txtSchoolName.Text = sch.SchoolName;
                this.drpSchoolLevel.SelectedValue = sch.LevelID.ToString();
                this.drpSchoolLevel.Enabled = false;
                this.drpSchoolKind.SelectedValue = sch.SchoolKindID.ToString();
                this.drpGender.SelectedIndex = sch.Gender;
                this.drpGender.Enabled = false;
                this.txtAreaName.Text = sch.AreaName;
                this.txtAreaCode.Text = sch.AreaCode.ToString();
                this.txtCityCode.Text = sch.CityID.ToString();
                this.txtCityName.Text = sch.CityName;
                this.txtVillage.Text = sch.Village;
                this.txtStreet.Text = sch.Street;
                this.txtAlly.Text = sch.Line;
                this.txtPelak.Text = sch.Pelak;
                this.txtPostCode.Text = sch.PostalCode;
                this.txtPhone.Text = sch.Phone;
                this.txtFixed.Text = sch.EmployeesCount_Fixed.ToString();
                this.txtChangable.Text = sch.EmployeesCount_Changable.ToString();
                this.txtManagerName.Text = sch.ManagerID.ToString();
                this.txtAssistanceName.Text = sch.AssistanceID.ToString();

                var subLevels = from slv in db.SchoolLevels
                                join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                                where slv.SchoolID == sch.SchoolID && slv.LockOutDate == null && !sl.SubLevelName.Equals("Employees")
                                select
                                new
                                {
                                    sl.SubLevelName,
                                    ssl.SubLevelID,
                                    ssl.BoysCount,
                                    ssl.GirlsCount
                                };
                this.grdSubLevels.DataSource = subLevels.ToList();
                this.grdSubLevels.DataBind();
                this.btnSave.Visible = false;
                this.btnEdit.Visible = !System.Web.HttpContext.Current.User.IsInRole("Guest");
            }

            if (string.IsNullOrEmpty(this.hdnSchoolID.Value)) // SchoolCode is not valid
            {
                this.txtSchoolName.Focus();
                this.drpSchoolLevel.Enabled = true;
                this.drpGender.Enabled = true;
                CleanCotrols();
            }
        }
    }

    protected void drpSchoolLevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadSublevels();
    }

    protected void drpGender_SelectedIndexChanged(object sender, EventArgs e)
    {
        TextBox txt = null;
        RequiredFieldValidator rfv = null;
        switch (this.drpGender.SelectedIndex)
        {
            case 0: // Male
                foreach (GridViewRow row in this.grdSubLevels.Rows)
                {
                    txt = (TextBox)row.FindControl("txtBoysCount");
                    rfv = (RequiredFieldValidator)row.FindControl("rfvBoys");
                    txt.Text = null;
                    txt.Enabled = true;
                    rfv.Enabled = true;
                    txt = (TextBox)row.FindControl("txtGirlsCount");
                    rfv = (RequiredFieldValidator)row.FindControl("rfvGirls");
                    txt.Text = null;
                    txt.Enabled = false;
                    rfv.Enabled = false;
                }
                break;

            case 1: // Female
                foreach (GridViewRow row in this.grdSubLevels.Rows)
                {
                    txt = (TextBox)row.FindControl("txtBoysCount");
                    rfv = (RequiredFieldValidator)row.FindControl("rfvBoys");
                    txt.Text = null;
                    txt.Enabled = false;
                    rfv.Enabled = false;
                    txt = (TextBox)row.FindControl("txtGirlsCount");
                    rfv = (RequiredFieldValidator)row.FindControl("rfvGirls");
                    txt.Text = null;
                    txt.Enabled = true;
                    rfv.Enabled = true;
                }
                break;

            case 2: // Male & Female
                foreach (GridViewRow row in this.grdSubLevels.Rows)
                {
                    txt = (TextBox)row.FindControl("txtBoysCount");
                    rfv = (RequiredFieldValidator)row.FindControl("rfvBoys");
                    txt.Text = null;
                    txt.Enabled = true;
                    rfv.Enabled = true;
                    txt = (TextBox)row.FindControl("txtGirlsCount");
                    rfv = (RequiredFieldValidator)row.FindControl("rfvGirls");
                    txt.Text = null;
                    txt.Enabled = true;
                    rfv.Enabled = true;
                }
                break;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid)
        {
            int schCode = Public.ToInt(this.txtSchoolCode.Text);
            int areaCode = Public.ToInt(this.txtAreaCode.Text);

            if (schCode > areaCode + 1000 || schCode <= areaCode)
            {
                this.lblMessage.Text = string.Format("کد آموزشگاه باید بین {0} و {1} باشد", areaCode + 1000, areaCode);
                return;
            }

            SupplySystem.School school = new SupplySystem.School();
            school.SchoolCode = schCode;
            school.SchoolName = this.txtSchoolName.Text.Trim();
            school.SchoolKindID = Public.ToByte(this.drpSchoolKind.SelectedValue);
            school.Gender = (byte)this.drpGender.SelectedIndex;
            school.AreaCode = Public.ToInt(this.txtAreaCode.Text);
            school.CityID = Public.ToShort(this.txtCityCode.Text);
            school.Village = this.txtVillage.Text.Trim();
            school.Street = this.txtStreet.Text.Trim();
            school.Line = this.txtAlly.Text.Trim();
            school.Pelak = this.txtPelak.Text.Trim();
            school.PostalCode = this.txtPostCode.Text.Trim();
            school.Phone = this.txtPhone.Text.Trim();
            school.EmployeesCount_Fixed = Public.ToByte(this.txtFixed.Text);
            school.EmployeesCount_Changable = Public.ToByte(this.txtChangable.Text);
            school.ManagerID = this.txtManagerName.Text.Trim();
            school.AssistanceID = this.txtAssistanceName.Text.Trim();

            SupplySystem.SchoolLevel slv = new SupplySystem.SchoolLevel { LevelID = Public.ToShort(this.drpSchoolLevel.SelectedValue), SubmitDate = DateTime.Now };
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
            school.SchoolLevels.Add(slv);

            SupplySystem.User user = new SupplySystem.User
            {
                UserName = this.txtSchoolCode.Text.Trim(),
                PassWord = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile((Public.ToInt(this.txtSchoolCode.Text.Trim()) * 2).ToString(), "SHA1"),
                FirstName = this.txtManagerName.Text.Trim(),
                LastName = string.Empty,
                Gender = (byte)this.drpGender.SelectedIndex,
                Phone = this.txtPhone.Text.Trim()
            };
            user.UsersInRoles.Add(new SupplySystem.UsersInRole { RoleID = (short)Public.Role.SchoolManager, School = school, SubmitDate = DateTime.Now });
            db.Schools.InsertOnSubmit(school);

            int disIntegId = 0;
            if (Request.QueryString["dtg"] != null && int.TryParse(TamperProofString.QueryStringDecode(Request.QueryString["dtg"]), out disIntegId))
            {
                SupplySystem.Integration dtg = db.Integrations.First<SupplySystem.Integration>(tg => tg.IntegrationID == disIntegId && tg.IntegrationMode == (byte)Public.IntegrationMode.DisIntegration);
                dtg.IntegratedSchools.Add(new SupplySystem.IntegratedSchool { School = school });
                db.SubmitChanges();
                Response.Redirect("~/School/DisIntegration.aspx");
            }

            db.SubmitChanges();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid && !string.IsNullOrEmpty(this.hdnSchoolID.Value))
        {
            SupplySystem.School school = db.Schools.First<SupplySystem.School>(s => s.SchoolID == Public.ToInt(this.hdnSchoolID.Value));
            school.SchoolName = this.txtSchoolName.Text.Trim();
            school.SchoolKindID = Public.ToByte(this.drpSchoolKind.SelectedValue);
            school.Gender = (byte)this.drpGender.SelectedIndex;
            school.AreaCode = Public.ToInt(this.txtAreaCode.Text);
            school.CityID = Public.ToShort(this.txtCityCode.Text);
            school.Village = this.txtVillage.Text.Trim();
            school.Street = this.txtStreet.Text.Trim();
            school.Line = this.txtAlly.Text.Trim();
            school.Pelak = this.txtPelak.Text.Trim();
            school.PostalCode = this.txtPostCode.Text.Trim();
            school.Phone = this.txtPhone.Text.Trim();
            school.EmployeesCount_Fixed = Public.ToByte(this.txtFixed.Text);
            school.EmployeesCount_Changable = Public.ToByte(this.txtChangable.Text);
            school.ManagerID = this.txtManagerName.Text.Trim();
            school.AssistanceID = this.txtAssistanceName.Text.Trim();
            db.SubmitChanges();
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

    private void LoadSublevels()
    {
        var subLevels = from sl in db.SubLevels
                        where sl.LevelID == Public.ToShort(this.drpSchoolLevel.SelectedValue)
                        select
                        new
                        {
                            sl.SubLevelID,
                            sl.SubLevelName
                        };

        ArrayList list = new ArrayList();
        foreach (var item in subLevels)
        {
            if (!item.SubLevelName.Equals("Employees"))
            {
                list.Add(new { item.SubLevelID, item.SubLevelName, BoysCount = string.Empty, GirlsCount = string.Empty });
            }
            else
            {
                this.ViewState["EmplSLID"] = item.SubLevelID;
            }
        }
        this.grdSubLevels.DataSource = list;
        this.grdSubLevels.DataBind();     
    }

    private void CleanCotrols()
    {
        this.ViewState["EmplSLID"] = null;
        this.hdnSchoolID.Value = null;
        this.txtSchoolName.Text = null;
        this.drpSchoolLevel.SelectedIndex = 1;
        this.drpSchoolKind.SelectedIndex = 7;
        this.drpGender.SelectedIndex = 2;
        this.txtAreaName.Text = null;
        this.txtAreaCode.Text = null;
        this.txtCityCode.Text = null;
        this.txtCityName.Text = null;
        this.txtVillage.Text = null;
        this.txtStreet.Text = null;
        this.txtAlly.Text = null;
        this.txtPelak.Text = null;
        this.txtPostCode.Text = null;
        this.txtPhone.Text = null;
        this.txtFixed.Text = null;
        this.txtChangable.Text = null;
        this.txtManagerName.Text = null;
        this.txtAssistanceName.Text = null;
        this.btnSave.Visible = true;
        this.btnEdit.Visible = false;
        LoadSublevels();
    }
}

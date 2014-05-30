using System;
using System.Linq;
using System.Web;

public partial class School_EditEmployeesCount : System.Web.UI.Page
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
        int schoolCode = 0;
        if (int.TryParse(this.txtSchoolCode.Text, out schoolCode))
        {
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         where s.SchoolCode == schoolCode && slv.LockOutDate == null
                         select new { s.SchoolID, s.SchoolCode, s.SchoolName, s.AreaCode, s.Gender, lv.LevelName, s.EmployeesCount_Changable, s.EmployeesCount_Fixed };

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
                this.txtCurrentChangable.Text = sch.EmployeesCount_Changable.GetValueOrDefault().ToString();
                this.txtCurrentFixed.Text = sch.EmployeesCount_Fixed.GetValueOrDefault().ToString();
                this.txtChangable.Text = sch.EmployeesCount_Changable.GetValueOrDefault().ToString();
                this.txtFixed.Text = sch.EmployeesCount_Fixed.GetValueOrDefault().ToString();
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        SupplySystem.School school = db.Schools.FirstOrDefault<SupplySystem.School>(s => s.SchoolID == Public.ToInt(this.hdnSchoolId.Value));
        if (school != null)
        {
            byte fixedCount = Public.ToByte(this.txtFixed.Text);
            byte changableCount = Public.ToByte(this.txtChangable.Text);
            byte currentFixedCount = Public.ToByte(this.txtCurrentFixed.Text);
            byte currentChangableCount = Public.ToByte(this.txtCurrentChangable.Text);

            if (fixedCount != currentFixedCount)
            {
                school.EmployeesCount_Fixed = fixedCount;
                school.SchoolEmployeesArchives.Add(new SupplySystem.SchoolEmployeesArchive
                {
                    FormerCount = currentFixedCount,
                    NextCount = fixedCount,
                    EmployeeType = "F",
                    SubmitDate = DateTime.Now,
                    Comment = this.txtFixedComment.Text
                });
            }

            if (changableCount != currentChangableCount)
            {
                school.EmployeesCount_Changable = changableCount;
                school.SchoolEmployeesArchives.Add(new SupplySystem.SchoolEmployeesArchive
                {
                    FormerCount = currentChangableCount,
                    NextCount = changableCount,
                    EmployeeType = "C",
                    SubmitDate = DateTime.Now,
                    Comment = this.txtChangableComment.Text
                });
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
                    this.lblMessage.Text = "براي هر گروه پرسنل روزانه يكبار ويرايش آمار پذيرفته ميشود";
                }
            }
        }
    }
}

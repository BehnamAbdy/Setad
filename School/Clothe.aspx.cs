using System;
using System.Linq;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

public partial class School_Clothe : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.lblCycle.Text = Public.ActiveCycle.CycleName;
            LoadClothes();
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
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
        {
            var school = from s in db.Schools
                         join ar in db.Areas on s.AreaCode equals ar.AreaCode
                         where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text)
                         select
                         new
                         {
                             s.SchoolID,
                             s.SchoolCode,
                             s.SchoolName,
                             ar.AreaName,
                             ar.AreaCode,
                         };

            if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
            {
                school = from q in school
                         where q.AreaCode == (from u in db.Users join ur in db.UsersInRoles on u.UserID equals ur.UserID where u.UserName.Equals(HttpContext.Current.User.Identity.Name) select ur.AreaCode).SingleOrDefault()
                         select q;
            }
            else if (System.Web.HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                school = from q in school
                         where q.SchoolCode == Public.ToInt(HttpContext.Current.User.Identity.Name)
                         select q;
            }

            this.hdnSchId.Value = null;
            this.drpSublevel.Items.Clear();
            foreach (var item in school)
            {
                this.hdnSchId.Value = item.SchoolID.ToString();
                this.txtSchoolName.Text = item.SchoolName;
                this.txtCity.Text = item.AreaName;
            }

            if (string.IsNullOrEmpty(this.hdnSchId.Value))
            {
                this.grdStudentClothes.DataSource = null;
                this.grdStudentClothes.DataBind();
                this.hdnSchId.Value = null;
                this.txtSchoolName.Text = null;
                this.txtCity.Text = null;
                ResetGoodsLists();
                return;
            }

            var subLevels = from slv in db.SchoolLevels
                            join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                            join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                            where slv.SchoolID == Public.ToInt(this.hdnSchId.Value) && !sl.SubLevelName.Equals("Employees") && slv.LockOutDate == null
                            select
                            new
                            {
                                ssl.SchoolSubLevelID,
                                sl.SubLevelName,
                                StudentCount = ssl.BoysCount.GetValueOrDefault() + ssl.GirlsCount.GetValueOrDefault()
                            };

            foreach (var item in subLevels)
            {
                this.drpSublevel.Items.Add(new ListItem(item.SubLevelName, string.Concat(item.SchoolSubLevelID, "|", item.StudentCount)));
            }

            FillGrids();
        }
        else
        {
            this.grdStudentClothes.DataSource = null;
            this.grdStudentClothes.DataBind();
            this.drpSublevel.Items.Clear();
            this.hdnSchId.Value = null;
            this.txtSchoolName.Text = null;
            this.txtCity.Text = null;
            ResetGoodsLists();
        }
    }

    protected void drpMonths_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillGrids();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && !string.IsNullOrEmpty(this.hdnSchId.Value))
        {
            int schoolSublevelId = Public.ToInt(this.drpSublevel.SelectedValue.Split('|')[0]);
            for (int i = 1; i < this.drpStuff_1.Items.Count; i++)
            {
                db.SchoolClothes.DeleteAllOnSubmit<SupplySystem.SchoolClothe>(from sc in db.SchoolClothes join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID where sc.SchoolSubLevelID == schoolSublevelId && sc.CycleClotheID == int.Parse(this.drpStuff_1.Items[i].Value) select sc);
            }

            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                db.SubmitChanges();
                int cycleClotheId1 = Public.ToInt(this.drpStuff_1.SelectedValue);
                int cycleClotheId2 = Public.ToInt(this.drpStuff_2.SelectedValue);
                int cycleClotheId3 = Public.ToInt(this.drpStuff_3.SelectedValue);
                int cycleClotheId4 = Public.ToInt(this.drpStuff_4.SelectedValue);
                int cycleClotheId5 = Public.ToInt(this.drpStuff_5.SelectedValue);

                List<SupplySystem.SchoolClothe> clothesList = new List<SupplySystem.SchoolClothe>();
                TextBox txtStudentName = null;
                CheckBox chkGood_1 = null;
                CheckBox chkGood_2 = null;
                CheckBox chkGood_3 = null;
                CheckBox chkGood_4 = null;
                CheckBox chkGood_5 = null;
                for (int i = 0; i < this.grdStudentClothes.Rows.Count; i++)
                {
                    txtStudentName = (TextBox)this.grdStudentClothes.Rows[i].FindControl("txtStudentName");
                    chkGood_1 = (CheckBox)this.grdStudentClothes.Rows[i].FindControl("chkGood_1");
                    chkGood_2 = (CheckBox)this.grdStudentClothes.Rows[i].FindControl("chkGood_2");
                    chkGood_3 = (CheckBox)this.grdStudentClothes.Rows[i].FindControl("chkGood_3");
                    chkGood_4 = (CheckBox)this.grdStudentClothes.Rows[i].FindControl("chkGood_4");
                    chkGood_5 = (CheckBox)this.grdStudentClothes.Rows[i].FindControl("chkGood_5");

                    if (!string.IsNullOrEmpty(txtStudentName.Text))
                    {
                        if (this.drpStuff_1.SelectedIndex > 0 && chkGood_1.Checked)
                        {
                            SupplySystem.SchoolClothe schoolClothe = clothesList.SingleOrDefault<SupplySystem.SchoolClothe>(sc => sc.CycleClotheID == cycleClotheId1);
                            if (schoolClothe == null)
                            {
                                schoolClothe = new SupplySystem.SchoolClothe { CycleClotheID = Public.ToShort(this.drpStuff_1.SelectedValue), SchoolSubLevelID = schoolSublevelId, SubmitDate = DateTime.Now };
                                clothesList.Add(schoolClothe);
                            }
                            schoolClothe.StudentClothes.Add(new SupplySystem.StudentClothe { StudentName = txtStudentName.Text, ClotheCount = 1 });
                        }

                        if (this.drpStuff_2.SelectedIndex > 0 && chkGood_2.Checked)
                        {
                            SupplySystem.SchoolClothe schoolClothe = clothesList.SingleOrDefault<SupplySystem.SchoolClothe>(sc => sc.CycleClotheID == cycleClotheId2);
                            if (schoolClothe == null)
                            {
                                schoolClothe = new SupplySystem.SchoolClothe { CycleClotheID = Public.ToShort(this.drpStuff_2.SelectedValue), SchoolSubLevelID = schoolSublevelId, SubmitDate = DateTime.Now };
                                clothesList.Add(schoolClothe);
                            }
                            schoolClothe.StudentClothes.Add(new SupplySystem.StudentClothe { StudentName = txtStudentName.Text, ClotheCount = 1 });
                        }

                        if (this.drpStuff_3.SelectedIndex > 0 && chkGood_3.Checked)
                        {
                            SupplySystem.SchoolClothe schoolClothe = clothesList.SingleOrDefault<SupplySystem.SchoolClothe>(sc => sc.CycleClotheID == cycleClotheId3);
                            if (schoolClothe == null)
                            {
                                schoolClothe = new SupplySystem.SchoolClothe { CycleClotheID = Public.ToShort(this.drpStuff_3.SelectedValue), SchoolSubLevelID = schoolSublevelId, SubmitDate = DateTime.Now };
                                clothesList.Add(schoolClothe);
                            }
                            schoolClothe.StudentClothes.Add(new SupplySystem.StudentClothe { StudentName = txtStudentName.Text, ClotheCount = 1 });
                        }

                        if (this.drpStuff_4.SelectedIndex > 0 && chkGood_4.Checked)
                        {
                            SupplySystem.SchoolClothe schoolClothe = clothesList.SingleOrDefault<SupplySystem.SchoolClothe>(sc => sc.CycleClotheID == cycleClotheId4);
                            if (schoolClothe == null)
                            {
                                schoolClothe = new SupplySystem.SchoolClothe { CycleClotheID = Public.ToShort(this.drpStuff_4.SelectedValue), SchoolSubLevelID = schoolSublevelId, SubmitDate = DateTime.Now };
                                clothesList.Add(schoolClothe);
                            }
                            schoolClothe.StudentClothes.Add(new SupplySystem.StudentClothe { StudentName = txtStudentName.Text, ClotheCount = 1 });
                        }

                        if (this.drpStuff_5.SelectedIndex > 0 && chkGood_5.Checked)
                        {
                            SupplySystem.SchoolClothe schoolClothe = clothesList.SingleOrDefault<SupplySystem.SchoolClothe>(sc => sc.CycleClotheID == cycleClotheId5);
                            if (schoolClothe == null)
                            {
                                schoolClothe = new SupplySystem.SchoolClothe { CycleClotheID = Public.ToShort(this.drpStuff_5.SelectedValue), SchoolSubLevelID = schoolSublevelId, SubmitDate = DateTime.Now };
                                clothesList.Add(schoolClothe);
                            }
                            schoolClothe.StudentClothes.Add(new SupplySystem.StudentClothe { StudentName = txtStudentName.Text, ClotheCount = 1 });
                        }
                    }
                }

                if (clothesList.Count > 0)
                {
                    db.SchoolClothes.InsertAllOnSubmit(clothesList);
                    db.SubmitChanges();
                    scope.Complete();
                    this.lblMessage.Text = Public.SUCCESSMESSAGE;
                }
            }
        }
    }

    private void LoadClothes()
    {
        var goodsList = GetClothes();
        this.drpStuff_1.DataSource = goodsList;
        this.drpStuff_1.DataBind();
        this.drpStuff_1.Items.Insert(0, "- انتخاب کنید -");
        this.drpStuff_2.DataSource = goodsList;
        this.drpStuff_2.DataBind();
        this.drpStuff_2.Items.Insert(0, "- انتخاب کنید -");
        this.drpStuff_3.DataSource = goodsList;
        this.drpStuff_3.DataBind();
        this.drpStuff_3.Items.Insert(0, "- انتخاب کنید -");
        this.drpStuff_4.DataSource = goodsList;
        this.drpStuff_4.DataBind();
        this.drpStuff_4.Items.Insert(0, "- انتخاب کنید -");
        this.drpStuff_5.DataSource = goodsList;
        this.drpStuff_5.DataBind();
        this.drpStuff_5.Items.Insert(0, "- انتخاب کنید -");
    }

    private void FillGrids()
    {
        short studentsCount = Public.ToShort(this.drpSublevel.SelectedValue.Split('|')[1]);
        ArrayList list = new ArrayList(studentsCount);
        for (short i = 0 + 1; i <= studentsCount; i++)
        {
            list.Add(new { RowNumber = i });
        }
        this.grdStudentClothes.DataSource = list;
        this.grdStudentClothes.DataBind();

        List<int> clotheIds = new List<int>();
        for (byte i = 1; i < this.drpStuff_1.Items.Count; i++)
        {
            clotheIds.Add(int.Parse(this.drpStuff_1.Items[i].Value));
        }

        DataLoadOptions dlo = new DataLoadOptions();
        dlo.LoadWith<SupplySystem.SchoolClothe>(sc => sc.StudentClothes);
        db.LoadOptions = dlo;
        List<SupplySystem.SchoolClothe> clothesList = db.SchoolClothes.Where<SupplySystem.SchoolClothe>(sc => sc.SchoolSubLevelID == Public.ToInt(this.drpSublevel.SelectedValue.Split('|')[0]) &&
                                                                                                                                                             clotheIds.Contains(sc.CycleClotheID)).Take<SupplySystem.SchoolClothe>(5).ToList<SupplySystem.SchoolClothe>();

        ResetGoodsLists();
        for (byte i = 0; i < clothesList.Count; i++)
        {
            switch (i)
            {
                case 0:
                    this.drpStuff_1.SelectedValue = clothesList[i].CycleClotheID.ToString();
                    this.drpStuff_1.Enabled = false;
                    break;

                case 1:
                    this.drpStuff_2.SelectedValue = clothesList[i].CycleClotheID.ToString();
                    this.drpStuff_2.Enabled = false;
                    break;

                case 2:
                    this.drpStuff_3.SelectedValue = clothesList[i].CycleClotheID.ToString();
                    this.drpStuff_3.Enabled = false;
                    break;

                case 3:
                    this.drpStuff_4.SelectedValue = clothesList[i].CycleClotheID.ToString();
                    this.drpStuff_4.Enabled = false;
                    break;

                case 4:
                    this.drpStuff_5.SelectedValue = clothesList[i].CycleClotheID.ToString();
                    this.drpStuff_5.Enabled = false;
                    break;
            }
            SetClothes(clothesList[i].StudentClothes.ToList<SupplySystem.StudentClothe>(), i);
        }
    }

    private void ResetGoodsLists()
    {
        this.drpStuff_1.SelectedIndex = 0;
        this.drpStuff_2.SelectedIndex = 0;
        this.drpStuff_3.SelectedIndex = 0;
        this.drpStuff_4.SelectedIndex = 0;
        this.drpStuff_5.SelectedIndex = 0;
        this.drpStuff_1.Enabled = true;
        this.drpStuff_2.Enabled = true;
        this.drpStuff_3.Enabled = true;
        this.drpStuff_4.Enabled = true;
        this.drpStuff_5.Enabled = true;
    }

    private ArrayList GetClothes()
    {
        ArrayList arrayList = HttpContext.Current.Cache["Cloth"] as ArrayList;

        if (arrayList == null)
        {
            arrayList = new ArrayList();
            var foods = from cl in db.CycleClothes
                        join st in db.REP_Stuffs on cl.StuffID equals st.StuffID
                        where cl.CycleID == Public.ActiveCycle.CycleID && cl.Available
                        select new { cl.CycleClotheID, st.StuffName };

            foreach (var stuff in foods)
            {
                arrayList.Add(new { stuff.CycleClotheID, stuff.StuffName });
            }
            HttpContext.Current.Cache.Insert("Cloth", arrayList, null, DateTime.MaxValue, TimeSpan.FromMinutes(6));
        }
        return arrayList;
    }

    private void SetClothes(List<SupplySystem.StudentClothe> clothesList, byte index)
    {
        TextBox txt = null;
        byte i = 0;
        while (i < clothesList.Count)
        {
            foreach (GridViewRow row in this.grdStudentClothes.Rows)
            {
                txt = (TextBox)row.FindControl("txtStudentName");
                if (string.IsNullOrEmpty(txt.Text) && txt.Text != clothesList[i].StudentName)
                {
                    txt.Text = clothesList[i].StudentName;
                    ((CheckBox)row.FindControl(string.Format("chkGood_{0}", index + 1))).Checked = true;
                    break;
                }
                else if (!string.IsNullOrEmpty(txt.Text) && txt.Text == clothesList[i].StudentName)
                {
                    ((CheckBox)row.FindControl(string.Format("chkGood_{0}", index + 1))).Checked = true;
                    break;
                }
            }
            i++;
        }
    }
}

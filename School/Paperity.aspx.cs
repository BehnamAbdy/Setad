using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;

public partial class School_Paperity : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        ConstructGridViewTemplates();
        if (!IsPostBack)
        {
            this.lblCycle.Text = Public.ActiveCycle.CycleName;
            Public.LoadCycleMonths(this.drpMonths);
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
        if (this.drpMonths.Items.Count > 0)
        {
            this.drpMonths.SelectedIndex = 0;
        }
        ClearGrid();
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
        {
            var school = from s in db.Schools
                         join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                         join lv in db.Levels on slv.LevelID equals lv.LevelID
                         join ar in db.Areas on s.AreaCode equals ar.AreaCode
                         where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) && slv.LockOutDate == null
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

            this.hdnSchoolId.Value = null;
            foreach (var item in school)
            {
                this.hdnSchoolId.Value = item.SchoolID.ToString();
                this.txtSchoolName.Text = item.SchoolName;
                this.txtCity.Text = item.AreaName;
            }

            if (string.IsNullOrEmpty(this.hdnSchoolId.Value))
            {
                this.txtSchoolName.Text = null;
                this.txtCity.Text = null;
                return;
            }
            this.drpMonths.Focus();
        }
        else
        {
            this.hdnSchoolId.Value = null;
            this.txtSchoolName.Text = null;
            this.txtCity.Text = null;
            if (this.drpMonths.Items.Count > 0)
            {
                this.drpMonths.SelectedIndex = 0;
            }
        }
    }

    protected void drpMonths_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClearGrid();
        if (!string.IsNullOrEmpty(this.hdnSchoolId.Value) && this.drpMonths.SelectedIndex > 0)
        {
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            var paperityList = (from sp in db.SchoolPaperities
                                join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                                join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                                join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                                join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                                where slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) &&
                                         cl.SolarYear == short.Parse(monthVals[0]) && cl.SolarMonth == byte.Parse(monthVals[1]) &&
                                         cl.SolarDay == 1 && slv.LockOutDate == null
                                select new
                                {
                                    sp.SchoolSubLevelID,
                                    sp.CyclePaperityID,
                                    sp.PaperityCount
                                }).ToList();

            int horizontalSum = 0;
            int[] verticalSums = new int[this.grdPaperities.HeaderRow.Cells.Count - 1];
            int cellsCount = this.grdPaperities.HeaderRow.Cells.Count - 1;
            DropDownList drpGoods = ((DropDownList)this.grdPaperities.Rows[0].Cells[0].Controls[0]);
            byte grdRowIndex = 0;

            for (byte i = 1; i < drpGoods.Items.Count; i++) // Iterate through rows
            {
                bool rowFoodSelected = false;
                for (byte j = 1; j < cellsCount; j++) // Iterate through cells
                {
                    var paperity = paperityList.Where(sp => sp.SchoolSubLevelID == Public.ToInt(((HiddenField)this.grdPaperities.HeaderRow.Cells[j].Controls[1]).Value) &&
                                                                           sp.CyclePaperityID == short.Parse(drpGoods.Items[i].Value));
                    foreach (var sp in paperity)
                    {
                        ((TextBox)this.grdPaperities.Rows[grdRowIndex].Cells[j].Controls[0]).Text = sp.PaperityCount.ToString();
                        if (!rowFoodSelected)
                        {
                            ((DropDownList)this.grdPaperities.Rows[grdRowIndex].Cells[0].Controls[0]).SelectedValue = drpGoods.Items[i].Value;
                            rowFoodSelected = true;
                        }
                        horizontalSum += sp.PaperityCount;
                        verticalSums[j - 1] += sp.PaperityCount;
                    }
                }

                if (rowFoodSelected)
                {
                    verticalSums[cellsCount - 1] += horizontalSum;
                    ((TextBox)this.grdPaperities.Rows[grdRowIndex].Cells[cellsCount].Controls[0]).Text = horizontalSum.ToString();
                    grdRowIndex++;
                }
                horizontalSum = 0;
            }

            for (byte i = 0; i < cellsCount; i++)
            {
                ((TextBox)this.grdPaperities.FooterRow.Cells[i + 1].Controls[0]).Text = verticalSums[i].ToString();
            }
        }
    }

    protected void grdPaperities_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ((DropDownList)e.Row.Cells[0].Controls[0]).Attributes.Add("onchange", string.Format("checkGoods(this, {0})", e.Row.RowIndex + 1));
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && this.drpMonths.SelectedIndex > 0)
        {
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            int cellsCount = this.grdPaperities.HeaderRow.Cells.Count - 1;
            List<SupplySystem.SchoolPaperity> grdPaperityList = GridPaperities(cellsCount);
            List<SupplySystem.SchoolPaperity> dbPaperityList = (from sp in db.SchoolPaperities
                                                                join sl in db.SchoolSubLevels on sp.SchoolSubLevelID equals sl.SchoolSubLevelID
                                                                join slv in db.SchoolLevels on sl.SchoolLevelID equals slv.SchoolLevelID
                                                                join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                                                                where cl.SolarYear == short.Parse(monthVals[0]) && cl.SolarMonth == byte.Parse(monthVals[1]) &&
                                                                cl.SolarDay == 1 && slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) && slv.LockOutDate == null
                                                                select sp).ToList<SupplySystem.SchoolPaperity>();
            if (dbPaperityList.Count > 0)
            {
                var changes = from dbp in dbPaperityList
                              join grp in grdPaperityList on new { dbp.CyclePaperityID, dbp.SchoolSubLevelID } equals
                                                              new { grp.CyclePaperityID, grp.SchoolSubLevelID }
                              where dbp.PaperityCount != grp.PaperityCount
                              select new { dbp.CyclePaperityID, dbp.SchoolSubLevelID, NewPaperityCount = grp.PaperityCount };

                foreach (var item in changes)
                {
                    dbPaperityList.SingleOrDefault<SupplySystem.SchoolPaperity>(sp => sp.SchoolSubLevelID == item.SchoolSubLevelID && sp.CyclePaperityID == item.CyclePaperityID).PaperityCount = item.NewPaperityCount;
                }

                foreach (SupplySystem.SchoolPaperity item in grdPaperityList)
                {
                    if (!dbPaperityList.Any<SupplySystem.SchoolPaperity>(sp => sp.SchoolSubLevelID == item.SchoolSubLevelID && sp.CyclePaperityID == item.CyclePaperityID))
                    {
                        db.SchoolPaperities.InsertOnSubmit(item);
                    }
                }
                db.SubmitChanges();
                this.lblMessage.Text = Public.SUCCESSMESSAGE;
            }
            else
            {
                db.SchoolPaperities.InsertAllOnSubmit<SupplySystem.SchoolPaperity>(grdPaperityList.Where<SupplySystem.SchoolPaperity>(sp => sp.PaperityCount > 0));
                db.SubmitChanges();
                this.lblMessage.Text = Public.SUCCESSMESSAGE;
            }
        }
    }

    private void ConstructGridViewTemplates()
    {
        var subLevels = from s in db.Schools
                        join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                        join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                        join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                        where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) && !sl.SubLevelName.Equals("Employees") && slv.LockOutDate == null
                        select
                        new
                        {
                            ssl.SchoolSubLevelID,
                            sl.SubLevelName,
                            StudentCount = ssl.BoysCount + ssl.GirlsCount
                        };

        TemplateField goodsColumn = new TemplateField();
        goodsColumn.HeaderTemplate = new GridViewTemplate(ListItemType.Header, "نام لوازم التحریر", "Sum", null);
        goodsColumn.ItemTemplate = new GridViewTemplate(ListItemType.SelectedItem, null, "Sum", null);
        this.grdPaperities.Columns.Add(goodsColumn);
        byte index = 0;

        foreach (var item in subLevels)
        {
            TemplateField column = new TemplateField();
            column.HeaderTemplate = new GridViewTemplate(ListItemType.Header, item.SubLevelName, index.ToString(), item.SchoolSubLevelID.ToString());
            column.ItemTemplate = new GridViewTemplate(ListItemType.Item, item.SubLevelName, index.ToString(), null);
            column.FooterTemplate = new GridViewTemplate(ListItemType.Footer, "footer", index.ToString(), null);
            this.grdPaperities.Columns.Add(column);
            index++;
        }

        TemplateField sumColumn = new TemplateField();
        sumColumn.HeaderTemplate = new GridViewTemplate(ListItemType.Header, "جمع", "Sum", null);
        sumColumn.ItemTemplate = new GridViewTemplate(ListItemType.Item, null, "Sum", null);
        sumColumn.FooterTemplate = new GridViewTemplate(ListItemType.Footer, null, "", null);
        this.grdPaperities.Columns.Add(sumColumn);
        this.grdPaperities.DataSource = index == 0 ? null : new byte[GetPaperteries().Count - 1];
        this.grdPaperities.DataBind();
    }

    protected ArrayList GetPaperteries()
    {
        ArrayList arrayList = HttpContext.Current.Cache["Papertery"] as ArrayList;
        if (arrayList == null)
        {
            arrayList = new ArrayList();
            var paperities = from cp in db.CyclePaperities
                             join st in db.REP_Stuffs on cp.StuffID equals st.StuffID
                             where cp.CycleID == Public.ActiveCycle.CycleID && cp.Available
                             select new { cp.CyclePaperityID, st.StuffName };
            arrayList.Add(new { CyclePaperityID = 0, StuffName = "- انتخاب کنید -" });

            foreach (var stuff in paperities)
            {
                arrayList.Add(new { stuff.CyclePaperityID, stuff.StuffName });
            }

            HttpContext.Current.Cache.Insert("Papertery", arrayList, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
        }
        return arrayList;
    }

    private List<SupplySystem.SchoolPaperity> GridPaperities(int cellsCount)
    {
        string[] monthVals = this.drpMonths.SelectedValue.Split('|');
        int calendarId = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == short.Parse(monthVals[0]) &&
                                                                                                   cl.SolarMonth == byte.Parse(monthVals[1]) &&
                                                                                                   cl.SolarDay == 1).CalendarID;
        List<SupplySystem.SchoolPaperity> paperityList = new List<SupplySystem.SchoolPaperity>();
        foreach (GridViewRow row in this.grdPaperities.Rows)
        {
            DropDownList drp = (DropDownList)row.Cells[0].Controls[0];
            if (drp.SelectedIndex > 0)
            {
                for (int i = 1; i < cellsCount; i++)
                {
                    string txt = ((TextBox)row.Cells[i].Controls[0]).Text;
                    if (!string.IsNullOrEmpty(txt))
                    {
                        paperityList.Add(new SupplySystem.SchoolPaperity
                        {
                            SchoolSubLevelID = Public.ToInt(((HiddenField)this.grdPaperities.HeaderRow.Cells[i].Controls[1]).Value),
                            CyclePaperityID = Public.ToShort(drp.SelectedValue),
                            CalendarID = calendarId,
                            PaperityCount = short.Parse(txt),
                            SubmitDate = DateTime.Now
                        });
                    }
                }
            }
        }
        return paperityList;
    }

    private void ClearGrid()
    {
        if (this.grdPaperities.Rows.Count > 0)
        {
            int cellsCount = this.grdPaperities.Rows[1].Cells.Count;
            foreach (GridViewRow row in this.grdPaperities.Rows)
            {
                ((DropDownList)row.Cells[0].Controls[0]).SelectedIndex = 0;
                for (int i = 1; i < cellsCount; i++)
                {
                    ((TextBox)row.Cells[i].Controls[0]).Text = null;
                }
            }

            for (int i = 1; i < cellsCount; i++)
            {
                ((TextBox)this.grdPaperities.FooterRow.Cells[i].Controls[0]).Text = null;
            }
        }
    }
}

internal class GridViewTemplate : ITemplate
{
    private ListItemType templateType;
    private string columnName = null;
    private string ctrlId = null;
    private string schoolSubLevelId = null;
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    public GridViewTemplate(ListItemType type, string columnName, string ctrlId, string schoolSubLevelId)
    {
        this.templateType = type;
        this.columnName = columnName;
        this.ctrlId = ctrlId;
        this.schoolSubLevelId = schoolSubLevelId;
    }

    protected ArrayList GetPaperteries()
    {
        ArrayList arrayList = HttpContext.Current.Cache["Papertery"] as ArrayList;
        if (arrayList == null)
        {
            arrayList = new ArrayList();
            var paperities = from cp in db.CyclePaperities
                             join st in db.REP_Stuffs on cp.StuffID equals st.StuffID
                             where cp.CycleID == Public.ActiveCycle.CycleID && cp.Available
                             select new { cp.CyclePaperityID, st.StuffName };
            arrayList.Add(new { CyclePaperityID = 0, StuffName = "- انتخاب کنید -" });

            foreach (var stuff in paperities)
            {
                arrayList.Add(new { stuff.CyclePaperityID, stuff.StuffName });
            }

            HttpContext.Current.Cache.Insert("Papertery", arrayList, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
        }
        return arrayList;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        TextBox txt = null;
        switch (templateType)
        {
            case ListItemType.Header:
                Literal lc = new Literal();
                lc.Text = string.Format("<B>{0}</B>", columnName);
                HiddenField hdn = new HiddenField();
                hdn.ID = string.Format("hdn_{0}", ctrlId);
                hdn.Value = schoolSubLevelId;
                container.Controls.Add(lc);
                if (ctrlId != "Sum")
                {
                    container.Controls.Add(hdn);
                }
                break;

            case ListItemType.SelectedItem:
                DropDownList drp = new DropDownList();
                drp.ID = string.Format("drpPapertery_{0}", ctrlId);
                drp.CssClass = "dropdown";
                drp.DataValueField = "CyclePaperityID";
                drp.DataTextField = "StuffName";
                drp.DataSource = GetPaperteries();
                drp.DataBind();
                drp.Items.Insert(0, new ListItem("- انتخاب کنید -", "0"));
                container.Controls.Add(drp);
                break;

            case ListItemType.Item:
                txt = new TextBox();
                txt.ID = string.Format("txt_{0}", ctrlId);
                txt.Attributes.Add("onchange", "javascript:getGridSum()");
                txt.Attributes.Add("onkeypress", "javascript:return isNumberKey(event)");
                txt.CssClass = "textbox";
                txt.Width = 70;
                txt.MaxLength = 4;
                if (ctrlId == "Sum")
                {
                    txt.Enabled = false;
                }
                container.Controls.Add(txt);
                break;

            case ListItemType.Footer:
                txt = new TextBox();
                txt.Enabled = false;
                txt.ID = string.Format("txtFooter_{0}", ctrlId);
                txt.CssClass = "textbox";
                txt.Width = 70;
                container.Controls.Add(txt);
                break;
        }
    }
}


using System;
using System.Linq;
using System.Web.Script.Serialization;
using System.Collections;

public partial class Cycles : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            int cycleId = 0;
            if (int.TryParse(Request.QueryString["cId"], out cycleId))
            {
                var cycle = from c in db.Cycles
                            where c.CycleID == cycleId
                            select
                            new
                            {
                                c.CycleID,
                                c.CycleName,
                                c.StartDate,
                                c.EndDate,
                                IsActive = c.IsActive ? 1 : 0
                            };

                foreach (var item in cycle)
                {
                    var cyl = new
                        {
                            item.CycleID,
                            item.CycleName,
                            StartDate = Public.ToPersianDate(item.StartDate),
                            EndDate = Public.ToPersianDate(item.EndDate),
                            item.IsActive
                        };
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Response.Clear();
                    Response.Write(serializer.Serialize(cyl));
                    Response.End();
                }
            }
            FillGrid();
            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            string[] startDate = this.txtStartDate.PersianDate.Split('/');
            string[] endDate = this.txtEndDate.PersianDate.Split('/');

            if (Public.ToInt(startDate[0]) < Public.ToInt(endDate[0])) // Both dates are not in the same year
            {
                if (Public.MonthsInStartYear(Public.ToInt(startDate[1]), Public.ToInt(startDate[0]), Public.ToInt(endDate[1]), Public.ToInt(endDate[0])).Count > 11)
                {
                    this.lblMessage.Text = "محدوده دوره نباید بیشتر از یازده ماه باشد!";
                    return;
                }
            }

            if (this.drpActivation.SelectedIndex == 1)
            {
                foreach (SupplySystem.Cycle item in db.Cycles)
                {
                    item.IsActive = false;
                }
            }

            SupplySystem.Cycle cycle = null;
            int cycleId = 0;
            if (string.IsNullOrEmpty(this.hdnCycleID.Value)) // Add mode
            {
                cycle = new SupplySystem.Cycle();
                db.Cycles.InsertOnSubmit(cycle);
            }
            else if (int.TryParse(this.hdnCycleID.Value, out cycleId))
            {
                cycle = db.Cycles.First<SupplySystem.Cycle>(c => c.CycleID == cycleId);
            }

            cycle.CycleName = this.txtCycleName.Text;
            cycle.StartDate = this.txtStartDate.GeorgianDate.GetValueOrDefault();
            cycle.EndDate = this.txtEndDate.GeorgianDate.GetValueOrDefault();
            cycle.SubmitDate = DateTime.Now;
            cycle.IsActive = this.drpActivation.SelectedIndex == 1 ? true : false;
            db.SubmitChanges();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            FillGrid();
            ClearControls();
            if (cycle.IsActive)
            {
                System.Web.HttpContext.Current.Cache.Insert("ActiveCycle", cycle, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdCycles.SelectedItems.Count == 1)
        {
            SupplySystem.Cycle cycle = db.Cycles.First<SupplySystem.Cycle>(c => c.CycleID == Public.ToInt(this.grdCycles.SelectedKeys[0]));
            this.hdnCycleID.Value = cycle.CycleID.ToString();
            this.txtCycleName.Text = cycle.CycleName;
            this.drpActivation.SelectedIndex = cycle.IsActive ? 1 : 0;
            this.txtStartDate.SetDate(cycle.StartDate);
            this.txtEndDate.SetDate(cycle.EndDate);
        }
        else
        {
            ClearControls();
        }
    }

    private void FillGrid()
    {
        var cycle = from c in db.Cycles
                    select
                    new
                    {
                        c.CycleID,
                        c.CycleName,
                        c.StartDate,
                        c.EndDate,
                        c.IsActive
                    };

        ArrayList list = new ArrayList();
        foreach (var item in cycle)
        {
            list.Add(new
            {
                item.CycleID,
                item.CycleName,
                StartDate = Public.ToPersianDate(item.StartDate),
                EndDate = Public.ToPersianDate(item.EndDate),
                IsActive = item.IsActive ? "بلی" : "خیر"
            });
        }

        this.grdCycles.DataSource = list;
        this.grdCycles.DataBind();
    }

    private void ClearControls()
    {
        this.hdnCycleID.Value = null;
        this.drpActivation.SelectedIndex = 0;
        this.txtCycleName.Text = null;
        this.txtStartDate.Text = null;
        this.txtEndDate.Text = null;
    }
}
using System;
using System.Linq;
using System.Web.UI.WebControls;

public partial class BaseInfo_CycleStuffs : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["mode"] != null)
            {
                string title = null;
                switch (Request.QueryString["mode"])
                {
                    case "1":
                        title = "خوراکی های روزانه دوره";
                        this.pnlDaily.Visible = true;
                        break;

                    case "2":
                        title = "پوشاک دوره";
                        break;

                    case "3":
                        title = "نوشت افزار دوره";
                        break;
                }
                this.Page.Title = title;
                this.spnTitle.InnerHtml = title;
                this.drpCycles.DataSource = db.Cycles;
                this.drpCycles.DataBind();
                this.drpCycles.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
                FillGrid();
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void drpCycles_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid)
        {
            if (this.ViewState["CycleStuffID"] == null) // Add mode
            {
                switch (Request.QueryString["mode"])
                {
                    case "1":
                        db.CycleFoods.InsertOnSubmit(new SupplySystem.CycleFood
                        {
                            CycleID = Public.ToInt(this.drpCycles.SelectedValue)
                                                   ,
                            StuffID = Public.ToInt(this.hdnStuffId.Value)
                                                   ,
                            Available = this.chkAvailable.Checked
                                                   ,
                            IsDaily = this.chkIsDaily.Checked
                        });
                        break;

                    case "2":
                        db.CycleClothes.InsertOnSubmit(new SupplySystem.CycleClothe
                        {
                            CycleID = Public.ToInt(this.drpCycles.SelectedValue)
                                                   ,
                            StuffID = Public.ToInt(this.hdnStuffId.Value)
                                                   ,
                            Available = this.chkAvailable.Checked
                        });
                        break;

                    case "3":
                        db.CyclePaperities.InsertOnSubmit(new SupplySystem.CyclePaperity
                        {
                            CycleID = Public.ToInt(this.drpCycles.SelectedValue)
                                                    ,
                            StuffID = Public.ToInt(this.hdnStuffId.Value)
                                                    ,
                            Available = this.chkAvailable.Checked
                        });
                        break;
                }
            }
            else // Edit mode
            {
                switch (Request.QueryString["mode"])
                {
                    case "1":
                        SupplySystem.CycleFood cycleFood = db.CycleFoods.First<SupplySystem.CycleFood>(cf => cf.CycleFoodID == Public.ToInt(this.ViewState["CycleStuffID"]));
                        cycleFood.StuffID = Public.ToInt(this.hdnStuffId.Value);
                        cycleFood.Available = this.chkAvailable.Checked;
                        cycleFood.IsDaily = this.chkIsDaily.Checked;
                        break;

                    case "2":
                        SupplySystem.CycleClothe cycleClothe = db.CycleClothes.First<SupplySystem.CycleClothe>(cc => cc.CycleClotheID == Public.ToInt(this.ViewState["CycleStuffID"]));

                        cycleClothe.StuffID = Public.ToInt(this.hdnStuffId.Value);
                        cycleClothe.Available = this.chkAvailable.Checked;
                        break;

                    case "3":
                        SupplySystem.CyclePaperity cyclePaperity = db.CyclePaperities.First<SupplySystem.CyclePaperity>(cc => cc.CyclePaperityID == Public.ToInt(this.ViewState["CycleStuffID"]));
                        cyclePaperity.StuffID = Public.ToInt(this.hdnStuffId.Value);
                        cyclePaperity.Available = this.chkAvailable.Checked;
                        break;
                }
            }

            try
            {
                db.SubmitChanges();
                this.ViewState["CycleStuffID"] = null;
                this.hdnStuffId.Value = null;
                this.chkAvailable.Checked = false;
                FillGrid();
                this.lblMessage.Text = Public.SUCCESSMESSAGE;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicate key"))
                {
                    this.lblMessage.Text = "این کالا قبلا در این دوره ثبت شده است";
                }
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        switch (Request.QueryString["mode"])
        {
            case "1":
                SupplySystem.CycleFood cycleFood = db.CycleFoods.First<SupplySystem.CycleFood>(cf => cf.CycleID == Public.ToInt(this.drpCycles.SelectedValue) &&
                                                                                                                                            cf.StuffID == Public.ToInt(((ImageButton)sender).CommandArgument));
                this.ViewState["CycleStuffID"] = cycleFood.CycleFoodID;
                this.hdnStuffId.Value = cycleFood.StuffID.ToString();
                this.txtStuffName.Text = cycleFood.REP_Stuff.StuffName;
                this.chkAvailable.Checked = cycleFood.Available;
                this.chkIsDaily.Checked = cycleFood.IsDaily;
                break;

            case "2":
                SupplySystem.CycleClothe cycleClothe = db.CycleClothes.First<SupplySystem.CycleClothe>(cc => cc.CycleID == Public.ToInt(this.drpCycles.SelectedValue) &&
                                                                                                                                            cc.StuffID == Public.ToInt(((ImageButton)sender).CommandArgument));

                this.ViewState["CycleStuffID"] = cycleClothe.CycleClotheID;
                this.hdnStuffId.Value = cycleClothe.StuffID.ToString();
                this.txtStuffName.Text = cycleClothe.REP_Stuff.StuffName;
                this.chkAvailable.Checked = cycleClothe.Available;
                break;

            case "3":
                SupplySystem.CyclePaperity cyclePaperity = db.CyclePaperities.First<SupplySystem.CyclePaperity>(cc => cc.CycleID == Public.ToInt(this.drpCycles.SelectedValue) &&
                                                                                                                                                      cc.StuffID == Public.ToInt(((ImageButton)sender).CommandArgument));
                this.ViewState["CycleStuffID"] = cyclePaperity.CyclePaperityID;
                this.hdnStuffId.Value = cyclePaperity.StuffID.ToString();
                this.txtStuffName.Text = cyclePaperity.REP_Stuff.StuffName;
                this.chkAvailable.Checked = cyclePaperity.Available;
                break;
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        //if (this.grdStuffs.SelectedItems.Count == 1 && this.drpCycles.SelectedIndex > 0)
        //{
        //    if (FeedingSystemDAL.CycleDailyFood.DeleteCycleDailyFood(PublicUtility.ToInt(this.drpCycles.SelectedValue), PublicUtility.ToInt(this.grdStuffs.SelectedKeys[0])) > 0)
        //    {
        //        FillGrid();
        //        PublicUtility.Alert(this, PublicUtility.DELETEMESSAGE);
        //    }
        //}
    }

    private void FillGrid()
    {
        if (this.drpCycles.SelectedIndex > 0)
        {
            switch (Request.QueryString["mode"])
            {
                case "1":
                    this.grdStuffs.DataSource = (from cdf in db.CycleFoods
                                                 join st in db.REP_Stuffs on cdf.StuffID equals st.StuffID
                                                 where cdf.CycleID == Public.ToInt(this.drpCycles.SelectedValue) && st.StuffTypeID == 1
                                                 select new { st.StuffID, st.StuffName, Available = cdf.Available ? "بلی" : "خیر", IsDaily = cdf.IsDaily ? "بلی" : "خیر" }).ToList();
                    break;

                case "2":
                    this.grdStuffs.DataSource = (from cc in db.CycleClothes
                                                 join st in db.REP_Stuffs on cc.StuffID equals st.StuffID
                                                 where cc.CycleID == Public.ToInt(this.drpCycles.SelectedValue) && st.StuffTypeID == 2
                                                 select new { st.StuffID, st.StuffName, Available = cc.Available ? "بلی" : "خیر" }).ToList();
                    break;

                case "3":
                    this.grdStuffs.DataSource = (from cp in db.CyclePaperities
                                                 join st in db.REP_Stuffs on cp.StuffID equals st.StuffID
                                                 where cp.CycleID == Public.ToInt(this.drpCycles.SelectedValue) && st.StuffTypeID == 3
                                                 select new { st.StuffID, st.StuffName, Available = cp.Available ? "بلی" : "خیر" }).ToList();
                    break;
            }
            this.grdStuffs.DataBind();
        }
        else
        {
            this.grdStuffs.Items.Clear();
        }
    }
}
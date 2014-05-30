using System;
using System.Linq;
using System.Web.UI;

public partial class BaseInfo_SubLevels : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpLevels.DataSource = db.Levels;
            this.drpLevels.DataBind();
            this.grdSubLevels.DataSource = db.SubLevels.Where<SupplySystem.SubLevel>(sl => sl.LevelID == Public.ToShort(this.drpLevels.SelectedValue) && !sl.SubLevelName.Equals("Employees")).ToList<SupplySystem.SubLevel>();
            this.grdSubLevels.DataBind();

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnAdd.Enabled = false;
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdSubLevels.SelectedItems.Count == 1)
        {
            db = new SupplySystem.SupplySystem(Public.ConnectionString);
            var slv = from sl in db.SubLevels
                      where sl.SubLevelID == Public.ToShort(this.grdSubLevels.SelectedKeys[0])
                      select new { sl.SubLevelID, sl.LevelID, sl.SubLevelName };
            foreach (var item in slv)
            {
                this.drpLevels.SelectedValue = item.LevelID.ToString();
                this.ViewState["SLID"] = item.SubLevelID.ToString();
                this.txtSubLevelName.Text = item.SubLevelName;
            }
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (this.ViewState["SLID"] == null) // Add mode
            {
                db.SubLevels.InsertOnSubmit(new SupplySystem.SubLevel { LevelID = Public.ToShort(this.drpLevels.SelectedValue), SubLevelName = this.txtSubLevelName.Text });
            }
            else
            {
                SupplySystem.SubLevel slv = db.SubLevels.First<SupplySystem.SubLevel>(sl => sl.SubLevelID == Public.ToShort(this.ViewState["SLID"]));
                slv.LevelID = Public.ToShort(this.drpLevels.SelectedValue);
                slv.SubLevelName = this.txtSubLevelName.Text;
            }

            db.SubmitChanges();
            this.grdSubLevels.DataSource = db.SubLevels.Where<SupplySystem.SubLevel>(sl => sl.LevelID == Public.ToShort(this.drpLevels.SelectedValue) && !sl.SubLevelName.Equals("Employees")).ToList<SupplySystem.SubLevel>();
            this.grdSubLevels.DataBind();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            ClearControls();
        }
    }

    protected void drpLevels_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.grdSubLevels.DataSource = db.SubLevels.Where<SupplySystem.SubLevel>(sl => sl.LevelID == Public.ToShort(this.drpLevels.SelectedValue) && !sl.SubLevelName.Equals("Employees")).ToList<SupplySystem.SubLevel>();
        this.grdSubLevels.DataBind();
    }

    private void ClearControls()
    {
        this.ViewState["SLID"] = null;
        this.drpLevels.SelectedIndex = 0;
        this.txtSubLevelName.Text = null;
    }
}


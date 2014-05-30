using System;
using System.Linq;
using System.Web.UI;

public partial class BaseInfo_Levels : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.grdLevels.DataSource = db.Levels.ToList<SupplySystem.Level>();
            this.grdLevels.DataBind();

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void txtCode_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.txtCode.Text))
        {
            SupplySystem.Level level = db.Levels.FirstOrDefault<SupplySystem.Level>(lv => lv.LevelID == Public.ToShort(this.txtCode.Text));
            if (level != null)
            {
                this.txtLevelName.Text = level.LevelName;
                this.ViewState["Mode"] = "edit";
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            if (this.ViewState["Mode"] == null) // Add mode
            {
                SupplySystem.Level level = new SupplySystem.Level { LevelID = Public.ToShort(this.txtCode.Text), LevelName = this.txtLevelName.Text };
                level.SubLevels.Add(new SupplySystem.SubLevel { SubLevelName = "Employees" });
                db.Levels.InsertOnSubmit(level);
            }
            else if (this.ViewState["Mode"].ToString() == "edit") // Edit mode
            {
                db.Levels.First<SupplySystem.Level>(sk => sk.LevelID == Public.ToShort(this.txtCode.Text)).LevelName = this.txtLevelName.Text;
            }

            db.SubmitChanges();
            this.grdLevels.DataSource = db.Levels.ToList<SupplySystem.Level>();
            this.grdLevels.DataBind();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            ClearControls();
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdLevels.SelectedItems.Count == 1)
        {
            SupplySystem.Level lv = db.Levels.First<SupplySystem.Level>(sk => sk.LevelID == Public.ToShort(this.grdLevels.SelectedKeys[0]));
            this.ViewState["Mode"] = "edit";
            this.txtCode.Text = lv.LevelID.ToString();
            this.txtLevelName.Text = lv.LevelName;
        }
        else
        {
            ClearControls();
        }
    }

    private void ClearControls()
    {
        this.ViewState["Mode"] = null;
        this.txtCode.Text = null;
        this.txtLevelName.Text = null;
    }
}

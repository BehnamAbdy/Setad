using System;
using System.Linq;

public partial class Repository_Units : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.grdUnits.DataSource = db.REP_Units.ToList<SupplySystem.REP_Unit>();
            this.grdUnits.DataBind();

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
            if (string.IsNullOrEmpty(this.hdnUnitId.Value))
            {
                byte unitId = 0;
                try
                {
                    unitId = (byte)(db.REP_Units.Max(u => u.UnitID) + 1);
                }
                catch
                {
                    unitId = 1;
                }
                db.REP_Units.InsertOnSubmit(new SupplySystem.REP_Unit { UnitID = unitId, UnitName = this.txtUnitName.Text.Trim() });
            }
            else
            {
                ((SupplySystem.REP_Unit)db.REP_Units.Single<SupplySystem.REP_Unit>(u => u.UnitID == Public.ToByte(this.hdnUnitId.Value))).UnitName = this.txtUnitName.Text.Trim();
            }

            try
            {
                db.SubmitChanges();
                this.lblMessage.Text = Public.SUCCESSMESSAGE;
                SetControls(null);
                this.grdUnits.DataSource = db.REP_Units.ToList<SupplySystem.REP_Unit>();
                this.grdUnits.DataBind();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE KEY"))
                {
                    this.lblMessage.Text = "نام واحد تکراری میباشد";
                }
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdUnits.SelectedItems.Count == 1)
        {
            SetControls(db.REP_Units.First<SupplySystem.REP_Unit>(u => u.UnitID == Public.ToByte(this.grdUnits.SelectedKeys[0])));
        }
    }

    private void SetControls(SupplySystem.REP_Unit unit)
    {
        if (unit != null)
        {
            this.txtUnitName.Text = unit.UnitName;
            this.hdnUnitId.Value = unit.UnitID.ToString();
        }
        else
        {
            this.hdnUnitId.Value = null;
            this.txtUnitName.Text = null;
        }
    }
}

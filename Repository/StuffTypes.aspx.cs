using System;
using System.Linq;

public partial class Repository_StuffTypes : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.grdStuffTypes.DataSource = db.REP_StuffTypes.ToList<SupplySystem.REP_StuffType>();
            this.grdStuffTypes.DataBind();

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdnStuffTypeId.Value))
        {
            byte stuffTypeId = 0;
            try
            {
                stuffTypeId = (byte)(db.REP_StuffTypes.Max(rp => rp.StuffTypeID) + 1);
            }
            catch
            {
                stuffTypeId = 1;
            }

            db.REP_StuffTypes.InsertOnSubmit(new SupplySystem.REP_StuffType
            {
                StuffTypeID = stuffTypeId,
                TypeName = this.txtTypeName.Text.Trim()
            });
        }
        else
        {
            ((SupplySystem.REP_StuffType)db.REP_StuffTypes.Single<SupplySystem.REP_StuffType>(st => st.StuffTypeID == Public.ToByte(this.hdnStuffTypeId.Value))).TypeName = this.txtTypeName.Text;
        }

        try
        {
            db.SubmitChanges();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            SetControls(null);
            this.grdStuffTypes.DataSource = db.REP_StuffTypes.ToList<SupplySystem.REP_StuffType>();
            this.grdStuffTypes.DataBind();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("UNIQUE KEY"))
            {
                this.lblMessage.Text = "دسنه کالا تکراری میباشد";
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdStuffTypes.SelectedItems.Count == 1)
        {
            SetControls(db.REP_StuffTypes.First<SupplySystem.REP_StuffType>(st => st.StuffTypeID == Public.ToByte(this.grdStuffTypes.SelectedKeys[0])));
        }
    }

    private void SetControls(SupplySystem.REP_StuffType stuffType)
    {
        if (stuffType != null)
        {
            this.txtTypeName.Text = stuffType.TypeName;
            this.hdnStuffTypeId.Value = stuffType.StuffTypeID.ToString();
        }
        else
        {
            this.hdnStuffTypeId.Value = null;
            this.txtTypeName.Text = null;
        }
    }
}

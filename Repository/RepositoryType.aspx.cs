using System;
using System.Linq;

public partial class Repository_RepositoryType : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.grdRepositoryTypes.DataSource = db.REP_RepositoryTypes.ToList<SupplySystem.REP_RepositoryType>();
            this.grdRepositoryTypes.DataBind();
        }

        if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
        {
            this.btnSave.Enabled = false;
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdnRepositoryTypeId.Value))
        {
            byte repositoryTypeId = 0;
            try
            {
                repositoryTypeId = (byte)(db.REP_RepositoryTypes.Max(rp => rp.RepositoryTypeID) + 1);
            }
            catch
            {
                repositoryTypeId = 1;
            }

            db.REP_RepositoryTypes.InsertOnSubmit(new SupplySystem.REP_RepositoryType
            {
                RepositoryTypeID = repositoryTypeId,
                TypeName = this.txtTypeName.Text.Trim()
            });
        }
        else
        {
            ((SupplySystem.REP_RepositoryType)db.REP_RepositoryTypes.Single<SupplySystem.REP_RepositoryType>(rt => rt.RepositoryTypeID == Public.ToShort(this.hdnRepositoryTypeId.Value))).TypeName = this.txtTypeName.Text;
        }

        try
        {
            db.SubmitChanges();
            this.grdRepositoryTypes.DataSource = db.REP_RepositoryTypes.ToList<SupplySystem.REP_RepositoryType>();
            this.grdRepositoryTypes.DataBind();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            SetControls(null);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("UNIQUE KEY"))
            {
                this.lblMessage.Text = "نوع انبار تکراری میباشد";
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdRepositoryTypes.SelectedItems.Count == 1)
        {
            SupplySystem.REP_RepositoryType repType = db.REP_RepositoryTypes.First<SupplySystem.REP_RepositoryType>(rt => rt.RepositoryTypeID == Public.ToByte(this.grdRepositoryTypes.SelectedKeys[0]));
            this.hdnRepositoryTypeId.Value = repType.RepositoryTypeID.ToString();
            this.txtTypeName.Text = repType.TypeName;
        }
        else
        {
            SetControls(null);
        }
    }

    private void SetControls(SupplySystem.REP_RepositoryType rpType)
    {
        if (rpType != null)
        {
            this.txtTypeName.Text = rpType.TypeName;
            this.hdnRepositoryTypeId.Value = rpType.RepositoryTypeID.ToString();
        }
        else
        {
            this.hdnRepositoryTypeId.Value = null;
            this.txtTypeName.Text = null;
        }
    }
}

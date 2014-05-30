using System;
using System.Linq;
using System.Web.UI;

public partial class BaseInfo_SchoolKinds : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.grdSchoolKinds.DataSource = db.SchoolKinds.ToList<SupplySystem.SchoolKind>();
            this.grdSchoolKinds.DataBind();

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.hdnKindId.Value))
        {
            byte kindId = 0;
            try
            {
                kindId = (byte)(db.SchoolKinds.Max(sk => sk.SchoolKindID) + 1);
            }
            catch
            {
                kindId = 1;
            }

            db.SchoolKinds.InsertOnSubmit(new SupplySystem.SchoolKind { SchoolKindID = kindId, SchoolKindName = this.txtSchoolKindName.Text.Trim() });

        }
        else
        {
            ((SupplySystem.SchoolKind)db.SchoolKinds.Single<SupplySystem.SchoolKind>(sk => sk.SchoolKindID == Public.ToByte(this.hdnKindId.Value))).SchoolKindName = this.txtSchoolKindName.Text.Trim();
        }

        try
        {
            db.SubmitChanges();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            SetControls(null);
            this.grdSchoolKinds.DataSource = db.SchoolKinds.ToList<SupplySystem.SchoolKind>();
            this.grdSchoolKinds.DataBind();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("UNIQUE KEY"))
            {
                this.lblMessage.Text = "نام نوع آموزشگاه تکراری میباشد";
            }
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdSchoolKinds.SelectedItems.Count == 1)
        {
            SetControls(db.SchoolKinds.First<SupplySystem.SchoolKind>(sk => sk.SchoolKindID == Public.ToShort(this.grdSchoolKinds.SelectedKeys[0])));
        }
    }

    private void SetControls(SupplySystem.SchoolKind schKind)
    {
        if (schKind != null)
        {
            this.hdnKindId.Value = schKind.SchoolKindID.ToString();
            this.txtSchoolKindName.Text = schKind.SchoolKindName;
        }
        else
        {
            this.hdnKindId.Value = null;
            this.txtSchoolKindName.Text = null;
        }
    }
}

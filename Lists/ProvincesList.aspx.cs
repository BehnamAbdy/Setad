using System;

public partial class Lists_ProvincesList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            this.grdProvinces.DataSource = db.Provinces;
            this.grdProvinces.DataBind();
        }
    }
}

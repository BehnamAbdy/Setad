using System;
using System.Linq;

public partial class Lists_CitiesList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            short provinceId = 0;
            if (short.TryParse(Request.QueryString["pId"], out provinceId))
            {
                SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
                this.grdCities.DataSource = db.Cities.Where<SupplySystem.City>(c => c.ProvinceID == provinceId).OrderBy(ct => ct.Name).ToList<SupplySystem.City>();
                this.grdCities.DataBind();
            }
            else
            {
                Server.Transfer("~/Error.aspx");
            }
        }
    }
}

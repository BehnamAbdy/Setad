using System;
using System.Linq;
using System.Web.UI;
using System.Configuration;

public partial class BaseInfo_Cities : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.txtProvinceCode.Text = ConfigurationManager.AppSettings["DefaultProvinceId"].ToString();
            this.txtProvinceName.Text = ConfigurationManager.AppSettings["DefaultProvinceName"].ToString();
            this.grdCities.DataSource = db.Cities.Where<SupplySystem.City>(ct => ct.ProvinceID == Public.ToByte(this.txtProvinceCode.Text)).OrderBy(ct => ct.Name).ToList<SupplySystem.City>();
            this.grdCities.DataBind();

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

            if (string.IsNullOrEmpty(this.hdnCityCode.Value)) // Add mode
            {
                db.Cities.InsertOnSubmit(new SupplySystem.City { ProvinceID = Public.ToByte(this.txtProvinceCode.Text), Name = this.txtCityName.Text });
            }
            else // Edit mode
            {
                db.Cities.Single<SupplySystem.City>(c => c.CityID == Public.ToShort(this.hdnCityCode.Value) && c.ProvinceID == Public.ToByte(this.txtProvinceCode.Text)).Name = this.txtCityName.Text;
            }
            db.SubmitChanges();
            this.grdCities.DataSource = db.Cities.Where<SupplySystem.City>(ct => ct.ProvinceID == Public.ToByte(this.txtProvinceCode.Text)).OrderBy(ct => ct.Name).ToList<SupplySystem.City>();
            this.grdCities.DataBind();
            this.hdnCityCode.Value = null;
            this.txtCityName.Text = null;
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdCities.SelectedItems.Count == 1)
        {
            var city = from ct in db.Cities
                       where ct.CityID == Public.ToInt(this.grdCities.SelectedKeys[0])
                       select
                       new
                       {
                           ct.CityID,
                           ct.Name
                       };

            foreach (var item in city)
            {
                this.txtCityName.Text = item.Name;
                this.hdnCityCode.Value = item.CityID.ToString();
            }
        }
    }
}

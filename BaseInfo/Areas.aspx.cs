using System;
using System.Linq;
using System.Data.Linq;
using System.Web.UI;
using System.Configuration;

public partial class BaseInfo_Areas : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.grdAreas.DataSource = db.Areas.Where<SupplySystem.Area>(ar => ar.ProvinceID == Public.ToByte(ConfigurationManager.AppSettings["DefaultProvinceId"])).Select(ar => new { ar.AreaCode, ar.AreaName }).OrderBy(ar => ar.AreaCode).ToList();
            this.grdAreas.DataBind();
            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnAdd.Enabled = false;
            }
        }
    }

    protected void txtAreaCode_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.txtAreaCode.Text))
        {
            var area = from ar in db.Areas
                       join ur in db.UsersInRoles on ar.AreaCode equals ur.AreaCode
                       join u in db.Users on ur.UserID equals u.UserID
                       where ar.AreaCode == Public.ToInt(this.txtAreaCode.Text)
                       select
                       new
                       {
                           ar.AreaCode,
                           ar.AreaName,
                           u.UserID,
                           User = string.Concat(u.FirstName, " ", u.LastName)
                       };

            foreach (var item in area)
            {
                this.txtAreaCode.Text = item.AreaCode.ToString();
                this.txtAreaName.Text = item.AreaName;
                this.hdnUserID.Value = item.UserID.ToString();
                this.txtChiefName.Text = item.User;
                this.ViewState["Mode"] = "edit";
            }
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            SupplySystem.Area area = null;

            if (this.ViewState["Mode"] == null) // Add mode
            {
                if (string.IsNullOrEmpty(this.hdnUserID.Value))
                {
                    this.lblMessage.Text = "مدیر منطقه را انتخاب کنید";
                    return;
                }

                DataLoadOptions dlo = new DataLoadOptions();
                dlo.LoadWith<SupplySystem.User>(u => u.UsersInRoles);
                db.LoadOptions = dlo;

                area = new SupplySystem.Area
                {
                    AreaCode = Public.ToInt(this.txtAreaCode.Text),
                    AreaName = this.txtAreaName.Text,
                    ProvinceID = Public.ToByte(ConfigurationManager.AppSettings["DefaultProvinceId"])
                };

                SupplySystem.User user = db.Users.First<SupplySystem.User>(u => u.UserID == Public.ToInt(this.hdnUserID.Value));
                user.UsersInRoles.First<SupplySystem.UsersInRole>().AreaCode = area.AreaCode;
                db.Areas.InsertOnSubmit(area);
            }
            else if (this.ViewState["Mode"].ToString() == "edit") // Edit mode
            {
                area = db.Areas.First<SupplySystem.Area>(ar => ar.AreaCode == Public.ToInt(this.txtAreaCode.Text));
                area.AreaName = this.txtAreaName.Text;
                area.UsersInRoles.First<SupplySystem.UsersInRole>().AreaCode = area.AreaCode;
            }

            db.SubmitChanges();
            this.grdAreas.DataSource = db.Areas.Where<SupplySystem.Area>(ar => ar.ProvinceID == Public.ToByte(ConfigurationManager.AppSettings["DefaultProvinceId"])).Select(ar => new { ar.AreaCode, ar.AreaName }).OrderBy(ar => ar.AreaCode).ToList();
            this.grdAreas.DataBind();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            ClearControls();
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        if (this.grdAreas.SelectedItems.Count == 1)
        {
            var area = from ar in db.Areas
                       join ur in db.UsersInRoles on ar.AreaCode equals ur.AreaCode
                       join u in db.Users on ur.UserID equals u.UserID
                       where ar.AreaCode == Public.ToInt(this.grdAreas.SelectedKeys[0])
                       select
                       new
                       {
                           ar.AreaCode,
                           ar.AreaName,
                           u.UserID,
                           User = string.Concat(u.FirstName, " ", u.LastName)
                       };

            foreach (var item in area)
            {
                this.txtAreaCode.Text = item.AreaCode.ToString();
                this.txtAreaName.Text = item.AreaName;
                this.hdnUserID.Value = item.UserID.ToString();
                this.txtChiefName.Text = item.User;
                this.ViewState["Mode"] = "edit";
            }
        }
    }

    private void ClearControls()
    {
        this.txtAreaCode.Text = null;
        this.txtAreaName.Text = null;
        this.txtChiefName.Text = null;
        this.hdnUserID.Value = null;
        this.ViewState["Mode"] = null;
    }
}

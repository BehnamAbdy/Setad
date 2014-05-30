using System;
using System.Linq;
using System.Collections.Generic;

public partial class Repository_Stuffs : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpStuffTypes.DataSource = db.REP_StuffTypes;
            this.drpStuffTypes.DataBind();
            List<SupplySystem.REP_Unit> units = db.REP_Units.ToList<SupplySystem.REP_Unit>();
            this.drpFirstUnit.DataSource = units;
            this.drpFirstUnit.DataBind();
            this.drpFirstUnit.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---  انتخاب کنید  ---", "0"));
            this.drpSecondUnit.DataSource = units;
            this.drpSecondUnit.DataBind();
            this.drpSecondUnit.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---  انتخاب کنید  ---", "0"));
            this.drpThirdUnit.DataSource = units;
            this.drpThirdUnit.DataBind();
            this.drpThirdUnit.Items.Insert(0, new System.Web.UI.WebControls.ListItem("---  انتخاب کنید  ---", "0"));
            this.grdStuffs.DataSource = db.REP_Stuffs.Where(st => st.StuffTypeID == Public.ToShort(this.drpStuffTypes.SelectedValue)).Select(st => new { st.StuffID, st.StuffName }).ToList();
            this.grdStuffs.DataBind();

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
            if (string.IsNullOrEmpty(this.hdnStuffId.Value))
            {
                SupplySystem.REP_Stuff stuff = new SupplySystem.REP_Stuff
                {
                    StuffName = this.txtGoodName.Text.Trim(),
                    StuffTypeID = Public.ToByte(this.drpStuffTypes.SelectedValue),
                    TechnicalInfo = this.txtTechnicalInfo.Text,
                    PrimaryUnitID = Public.ToByte(this.drpFirstUnit.SelectedValue),
                    SecondaryUnitID = Public.ToByte(this.drpSecondUnit.SelectedValue),
                    ThirdUnitID = Public.ToByte(this.drpThirdUnit.SelectedValue),
                    FirstConversionCoefficient = Public.ToByte(this.txtFirstConversionCoefficient.Text),
                    SecondConversionCoefficient = Public.ToByte(this.txtSecondConversionCoefficient.Text),
                    ProducerCompany = this.txtProducerCompany.Text,
                    ProducerCountry = this.txtProducerCountry.Text,
                    Capacity = this.txtCapacity.Text,
                    Color = this.txtColor.Text,
                    Size = this.txtSize.Text,
                    SubmitDate = DateTime.Now.Date
                };

                db.REP_Stuffs.InsertOnSubmit(stuff);
            }
            else
            {
                SupplySystem.REP_Stuff stuff = db.REP_Stuffs.SingleOrDefault(st => st.StuffID == Public.ToInt(this.hdnStuffId.Value));
                if (stuff != null)
                {
                    stuff.StuffName = this.txtGoodName.Text.Trim();
                    stuff.StuffTypeID = Public.ToByte(this.drpStuffTypes.SelectedValue);
                    stuff.TechnicalInfo = this.txtTechnicalInfo.Text;
                    stuff.PrimaryUnitID = Public.ToByte(this.drpFirstUnit.SelectedValue);
                    stuff.SecondaryUnitID = Public.ToByte(this.drpSecondUnit.SelectedValue);
                    stuff.ThirdUnitID = Public.ToByte(this.drpThirdUnit.SelectedValue);
                    stuff.FirstConversionCoefficient = Public.ToByte(this.txtFirstConversionCoefficient.Text);
                    stuff.SecondConversionCoefficient = Public.ToByte(this.txtSecondConversionCoefficient.Text);
                    stuff.ProducerCompany = this.txtProducerCompany.Text;
                    stuff.ProducerCountry = this.txtProducerCountry.Text;
                    stuff.Capacity = this.txtCapacity.Text;
                    stuff.Color = this.txtColor.Text;
                    stuff.Size = this.txtSize.Text;
                }
            }

            db.SubmitChanges();
            this.grdStuffs.DataSource = db.REP_Stuffs.Where(st => st.StuffTypeID == Public.ToByte(this.drpStuffTypes.SelectedValue)).Select(st => new { st.StuffID, st.StuffName }).ToList();
            this.grdStuffs.DataBind();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
            SetControls(null);
        }
    }

    protected void btnEdit_Click(object sender, EventArgs e)
    {
        SetControls(db.REP_Stuffs.SingleOrDefault(st => st.StuffID == Public.ToInt(((System.Web.UI.WebControls.ImageButton)sender).CommandArgument)));
    }

    protected void drpStuffTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        this.grdStuffs.DataSource = db.REP_Stuffs.Where(st => st.StuffTypeID == Public.ToByte(this.drpStuffTypes.SelectedValue)).Select(st => new { st.StuffID, st.StuffName }).ToList();
        this.grdStuffs.DataBind();
    }

    private void SetControls(SupplySystem.REP_Stuff stuff)
    {
        if (stuff != null)
        {
            this.hdnStuffId.Value = stuff.StuffID.ToString();
            this.txtGoodName.Text = stuff.StuffName;
            this.drpStuffTypes.SelectedValue = stuff.StuffTypeID.ToString();
            this.txtTechnicalInfo.Text = stuff.TechnicalInfo;
            this.drpFirstUnit.SelectedValue = stuff.PrimaryUnitID.ToString();
            this.drpSecondUnit.SelectedValue = stuff.SecondaryUnitID.ToString();
            this.drpThirdUnit.SelectedValue = stuff.ThirdUnitID.ToString();
            this.txtFirstConversionCoefficient.Text = stuff.FirstConversionCoefficient.ToString();
            this.txtSecondConversionCoefficient.Text = stuff.SecondConversionCoefficient.ToString();
            this.txtProducerCompany.Text = stuff.ProducerCompany;
            this.txtProducerCountry.Text = stuff.ProducerCountry;
            this.txtCapacity.Text = stuff.Capacity;
            this.txtColor.Text = stuff.Color;
            this.txtSize.Text = stuff.Size;
        }
        else
        {
            this.hdnStuffId.Value = null;
            this.txtGoodName.Text = null;
            this.drpStuffTypes.SelectedIndex = 0;
            this.txtTechnicalInfo.Text = null;
            this.drpFirstUnit.SelectedIndex = 0;
            this.drpSecondUnit.SelectedIndex = 0;
            this.drpThirdUnit.SelectedIndex = 0;
            this.txtFirstConversionCoefficient.Text = null;
            this.txtSecondConversionCoefficient.Text = null;
            this.txtProducerCompany.Text = null;
            this.txtProducerCountry.Text = null;
            this.txtCapacity.Text = null;
            this.txtColor.Text = null;
            this.txtSize.Text = null;
        }
    }
}


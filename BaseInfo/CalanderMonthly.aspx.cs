using System;
using System.Linq;
using System.Collections.Generic;

public partial class BaseInfo_CalanderMonthly : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["year"] != null)
            {
                this.lblYear.Text = Request.QueryString["year"];
                ArrangeDays();
            }
            else
            {
                Server.Transfer("~/Error.aspx");
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClearDays();
        ArrangeDays();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(hdnDay.Value))
        {
            string[] day = this.hdnDay.Value.Split('_');
            SupplySystem.Calendar calendar = db.Calendars.Single<SupplySystem.Calendar>(c => c.SolarYear == Public.ToShort(Request.QueryString["year"]) &&
                                                                                                                                  c.SolarMonth == Public.ToByte(this.drpMonth.SelectedValue) &&
                                                                                                                                  c.SolarDay == Public.ToByte(day[1]));
            calendar.IsHoliday = day[0] == "1" ? false : true;
            db.SubmitChanges();
            ClearDays();
            ArrangeDays();
        }
    }

    private void ArrangeDays()
    {
        List<SupplySystem.Calendar> daysList = db.Calendars.Where<SupplySystem.Calendar>(c => c.SolarYear == Public.ToShort(Request.QueryString["year"]) &&
                                                                                                                                       c.SolarMonth == Public.ToByte(this.drpMonth.SelectedValue)).ToList<SupplySystem.Calendar>();

        for (int i = 0; i < daysList.Count; i++)
        {
            ArrangeDay(i + daysList[0].DayOfWeek, (i + 1).ToString(), daysList[i].IsHoliday);
        }
    }

    private void ArrangeDay(int blockNo, string dayOfMonth, bool isHolliday)
    {
        switch (blockNo)
        {
            case 1:
                this.d1.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d1.Style["background-color"] = "#f2f200";
                }
                break;

            case 2:
                this.d2.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d2.Style["background-color"] = "#f2f200";
                }
                break;

            case 3:
                this.d3.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d3.Style["background-color"] = "#f2f200";
                }
                break;

            case 4:
                this.d4.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d4.Style["background-color"] = "#f2f200";
                }
                break;

            case 5:
                this.d5.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d5.Style["background-color"] = "#f2f200";
                }
                break;

            case 6:
                this.d6.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d6.Style["background-color"] = "#f2f200";
                }
                break;

            case 7:
                this.d7.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d7.Style["background-color"] = "#f2f200";
                }
                break;

            case 8:
                this.d8.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d8.Style["background-color"] = "#f2f200";
                }
                break;

            case 9:
                this.d9.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d9.Style["background-color"] = "#f2f200";
                }
                break;

            case 10:
                this.d10.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d10.Style["background-color"] = "#f2f200";
                }
                break;

            case 11:
                this.d11.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d11.Style["background-color"] = "#f2f200";
                }
                break;

            case 12:
                this.d12.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d12.Style["background-color"] = "#f2f200";
                }
                break;

            case 13:
                this.d13.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d13.Style["background-color"] = "#f2f200";
                }
                break;

            case 14:
                this.d14.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d14.Style["background-color"] = "#f2f200";
                }
                break;

            case 15:
                this.d15.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d15.Style["background-color"] = "#f2f200";
                }
                break;

            case 16:
                this.d16.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d16.Style["background-color"] = "#f2f200";
                }
                break;

            case 17:
                this.d17.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d17.Style["background-color"] = "#f2f200";
                }
                break;

            case 18:
                this.d18.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d18.Style["background-color"] = "#f2f200";
                }
                break;

            case 19:
                this.d19.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d19.Style["background-color"] = "#f2f200";
                }
                break;

            case 20:
                this.d20.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d20.Style["background-color"] = "#f2f200";
                }
                break;

            case 21:
                this.d21.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d21.Style["background-color"] = "#f2f200";
                }
                break;

            case 22:
                this.d22.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d22.Style["background-color"] = "#f2f200";
                }
                break;

            case 23:
                this.d23.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d23.Style["background-color"] = "#f2f200";
                }
                break;

            case 24:
                this.d24.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d24.Style["background-color"] = "#f2f200";
                }
                break;

            case 25:
                this.d25.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d25.Style["background-color"] = "#f2f200";
                }
                break;

            case 26:
                this.d26.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d26.Style["background-color"] = "#f2f200";
                }
                break;

            case 27:
                this.d27.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d27.Style["background-color"] = "#f2f200";
                }
                break;

            case 28:
                this.d28.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d28.Style["background-color"] = "#f2f200";
                }
                break;

            case 29:
                this.d29.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d29.Style["background-color"] = "#f2f200";
                }
                break;

            case 30:
                this.d30.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d30.Style["background-color"] = "#f2f200";
                }
                break;

            case 31:
                this.d31.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d31.Style["background-color"] = "#f2f200";
                }
                break;

            case 32:
                this.d32.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d32.Style["background-color"] = "#f2f200";
                }
                break;

            case 33:
                this.d33.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d33.Style["background-color"] = "#f2f200";
                }
                break;

            case 34:
                this.d34.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d34.Style["background-color"] = "#f2f200";
                }
                break;

            case 35:
                this.d35.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d35.Style["background-color"] = "#f2f200";
                }
                break;

            case 36:
                this.d36.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d36.Style["background-color"] = "#f2f200";
                }
                break;

            case 37:
                this.d37.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d37.Style["background-color"] = "#f2f200";
                }
                break;

            case 38:
                this.d38.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d38.Style["background-color"] = "#f2f200";
                }
                break;

            case 39:
                this.d39.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d39.Style["background-color"] = "#f2f200";
                }
                break;

            case 40:
                this.d40.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d40.Style["background-color"] = "#f2f200";
                }
                break;

            case 41:
                this.d41.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d41.Style["background-color"] = "#f2f200";
                }
                break;

            case 42:
                this.d42.InnerHtml = dayOfMonth;
                if (isHolliday)
                {
                    this.d42.Style["background-color"] = "#f2f200";
                }
                break;
        }
    }

    private void ClearDays()
    {
        this.d1.InnerHtml = null;
        this.d1.Style["background-color"] = "#f8f8f8";
        this.d2.InnerHtml = null;
        this.d2.Style["background-color"] = "#f8f8f8";
        this.d3.InnerHtml = null;
        this.d3.Style["background-color"] = "#f8f8f8";
        this.d4.InnerHtml = null;
        this.d4.Style["background-color"] = "#f8f8f8";
        this.d5.InnerHtml = null;
        this.d5.Style["background-color"] = "#f8f8f8";
        this.d6.InnerHtml = null;
        this.d6.Style["background-color"] = "#f8f8f8";
        this.d7.InnerHtml = null;
        this.d7.Style["background-color"] = "#f8f8f8";
        this.d8.InnerHtml = null;
        this.d8.Style["background-color"] = "#f8f8f8";
        this.d9.InnerHtml = null;
        this.d9.Style["background-color"] = "#f8f8f8";
        this.d10.InnerHtml = null;
        this.d10.Style["background-color"] = "#f8f8f8";
        this.d11.InnerHtml = null;
        this.d11.Style["background-color"] = "#f8f8f8";
        this.d12.InnerHtml = null;
        this.d12.Style["background-color"] = "#f8f8f8";
        this.d13.InnerHtml = null;
        this.d13.Style["background-color"] = "#f8f8f8";
        this.d14.InnerHtml = null;
        this.d14.Style["background-color"] = "#f8f8f8";
        this.d15.InnerHtml = null;
        this.d15.Style["background-color"] = "#f8f8f8";
        this.d16.InnerHtml = null;
        this.d16.Style["background-color"] = "#f8f8f8";
        this.d17.InnerHtml = null;
        this.d17.Style["background-color"] = "#f8f8f8";
        this.d18.InnerHtml = null;
        this.d18.Style["background-color"] = "#f8f8f8";
        this.d19.InnerHtml = null;
        this.d19.Style["background-color"] = "#f8f8f8";
        this.d20.InnerHtml = null;
        this.d20.Style["background-color"] = "#f8f8f8";
        this.d21.InnerHtml = null;
        this.d21.Style["background-color"] = "#f8f8f8";
        this.d22.InnerHtml = null;
        this.d22.Style["background-color"] = "#f8f8f8";
        this.d23.InnerHtml = null;
        this.d23.Style["background-color"] = "#f8f8f8";
        this.d24.InnerHtml = null;
        this.d24.Style["background-color"] = "#f8f8f8";
        this.d25.InnerHtml = null;
        this.d25.Style["background-color"] = "#f8f8f8";
        this.d26.InnerHtml = null;
        this.d26.Style["background-color"] = "#f8f8f8";
        this.d27.InnerHtml = null;
        this.d27.Style["background-color"] = "#f8f8f8";
        this.d28.InnerHtml = null;
        this.d28.Style["background-color"] = "#f8f8f8";
        this.d29.InnerHtml = null;
        this.d29.Style["background-color"] = "#f8f8f8";
        this.d30.InnerHtml = null;
        this.d30.Style["background-color"] = "#f8f8f8";
        this.d31.InnerHtml = null;
        this.d31.Style["background-color"] = "#f8f8f8";
        this.d32.InnerHtml = null;
        this.d32.Style["background-color"] = "#f8f8f8";
        this.d33.InnerHtml = null;
        this.d33.Style["background-color"] = "#f8f8f8";
        this.d34.InnerHtml = null;
        this.d34.Style["background-color"] = "#f8f8f8";
        this.d35.InnerHtml = null;
        this.d35.Style["background-color"] = "#f8f8f8";
        this.d36.InnerHtml = null;
        this.d36.Style["background-color"] = "#f8f8f8";
        this.d37.InnerHtml = null;
        this.d37.Style["background-color"] = "#f8f8f8";
        this.d38.InnerHtml = null;
        this.d8.Style["background-color"] = "#f8f8f8";
        this.d39.InnerHtml = null;
        this.d39.Style["background-color"] = "#f8f8f8";
        this.d40.InnerHtml = null;
        this.d40.Style["background-color"] = "#f8f8f8";
        this.d41.InnerHtml = null;
        this.d41.Style["background-color"] = "#f8f8f8";
        this.d42.InnerHtml = null;
        this.d42.Style["background-color"] = "#f8f8f8";
    }
}


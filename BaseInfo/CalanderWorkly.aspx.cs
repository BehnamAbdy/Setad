using System;
using System.Linq;
using System.Globalization;

public partial class BaseInfo_CalanderWorkly : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.drpYear.DataSource = from c in db.Calendars
                                      group c by new { c.SolarYear } into years
                                      orderby years.Key.SolarYear descending
                                      select new { years.Key.SolarYear };
            this.drpYear.DataBind();

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnAdd.Enabled = false;
            }
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.txtYear.Text) && Public.ToShort(this.txtYear.Text) > 1300)
        {
            int year = Public.ToInt(this.txtYear.Text);
            int daysOfYear = new PersianCalendar().IsLeapYear(year) ? 366 : 365;
            DateTime date = Persia.Calendar.ConvertToGregorian(year, 1, 1, Persia.DateType.Persian);
            byte dayOfWeek = (byte)(date.DayOfWeek + 2) == (byte)8 ? (byte)1 : (byte)(date.DayOfWeek + 2);
            int[] persianDate = null;
            int i;
            for (i = 1; i <= daysOfYear; i++)
            {
                persianDate = Persia.Calendar.ConvertToPersian(date).ArrayType;
                db.Calendars.InsertOnSubmit(new SupplySystem.Calendar
                {
                    GeorgianDate = date
                    ,
                    SolarYear = (short)year
                    ,
                    SolarMonth = (byte)persianDate[1]
                    ,
                    SolarDay = (byte)persianDate[2]
                    ,
                    DayOfWeek = dayOfWeek
                    ,
                    IsHoliday = IsHoliday(dayOfWeek)
                });
                dayOfWeek = dayOfWeek == 7 ? (byte)1 : ++dayOfWeek;
                date = date.AddDays(1);
            }

            if (i == 365 || i == 366)
            {
                db.SubmitChanges();
                this.lblMessage.Text = Public.SUCCESSMESSAGE;
                this.drpYear.DataSource = from c in db.Calendars
                                          group c by new { c.SolarYear } into years
                                          orderby years.Key.SolarYear descending
                                          select new { years.Key.SolarYear };
                this.drpYear.DataBind();
                ClearControls();
            }
            //this.lblMessage.Text = "سال تکراری می باشد";
        }
    }

    private bool IsHoliday(byte dayOfWeek)
    {
        bool result = false;

        switch (dayOfWeek)
        {
            case 1:
                result = this.chkSaturday.Checked;
                break;

            case 2:
                result = this.chkSunday.Checked;
                break;

            case 3:
                result = this.chkMonday.Checked;
                break;

            case 4:
                result = this.chkTuseDay.Checked;
                break;

            case 5:
                result = this.chkWedneseday.Checked;
                break;

            case 6:
                result = this.chkThurseday.Checked;
                break;

            case 7:
                result = this.chkFriday.Checked;
                break;
        }
        return result;
    }

    private void ClearControls()
    {
        this.txtYear.Text = null;
        this.chkSaturday.Checked = false;
        this.chkSunday.Checked = false;
        this.chkMonday.Checked = false;
        this.chkTuseDay.Checked = false;
        this.chkWedneseday.Checked = false;
        this.chkThurseday.Checked = false;
        this.chkFriday.Checked = false;
    }
}

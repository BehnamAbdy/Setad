using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;
using System.Text.RegularExpressions;


public partial class UC_Date : System.Web.UI.UserControl
{
    public string Text
    {
        get { return txtDate.Text.Trim(); }
        set { this.txtDate.Text = value; }
    }

    public string CssClass
    {
        get
        {
            return txtDate.CssClass;
        }
        set
        {
            txtDate.CssClass = value;
        }
    }

    public bool Enabled
    {
        get
        {
            return this.txtDate.Enabled;
        }
        set
        {
            this.txtDate.Enabled = value;
        }
    }

    public Unit Width
    {
        get { return txtDate.Width; }
        set { txtDate.Width = value; }
    }

    public void Clear()
    {
        txtDate.Text = string.Format("____{0}__{0}__", Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator);
    }

    public bool HasDate
    {
        get
        {
            Regex regex = new Regex(@"\d{4}(/|_)\d{2}(/|_)\d{2}");
            return regex.IsMatch(this.txtDate.Text);
        }
    }

    public string PersianDate
    {
        get { return txtDate.Text; }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.txtDate.Text = string.Format("{0}/{1}/{2}", value.Substring(0, 4), value.Substring(4, 2), value.Substring(6, 2));
            }
            else
            {
                this.txtDate.Text = string.Empty;
            }
        }
    }

    public DateTime? GeorgianDate
    {
        get
        {
            if (HasDate)
            {
                short year = Convert.ToInt16(this.txtDate.Text.Substring(0, 4));
                short month = Convert.ToInt16(this.txtDate.Text.Substring(5, 2));
                short day = Convert.ToInt16(this.txtDate.Text.Substring(8, 2));
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, 0, 0, 0, 0).Date;
            }
            else
            {
                return null;
            }
        }
    }

    public DateTime? GeorgianDateTime
    {
        get
        {
            if (HasDate)
            {
                short year = Convert.ToInt16(this.txtDate.Text.Substring(0, 4));
                short month = Convert.ToInt16(this.txtDate.Text.Substring(5, 2));
                short day = Convert.ToInt16(this.txtDate.Text.Substring(8, 2));
                PersianCalendar pc = new PersianCalendar();
                return pc.ToDateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            }
            else
            {
                return null;
            }
        }
        set
        {
            this.txtDate.Text = Public.ToPersianDate(value);
        }
    }
    
    public void SetDate(DateTime? georgianDate)
    {
        if (georgianDate == null)
        {
            this.txtDate.Text = null;
        }
        else
        {
            this.txtDate.Text = Public.ToPersianDate(georgianDate);
        }
    }
}





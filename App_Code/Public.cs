using System;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Globalization;
using Stimulsoft.Report;

public static class Public
{
    public const string SUCCESSMESSAGE = "ثبت اطلاعات انجام گردید";
    public const string DELETEMESSAGE = "حذف اطلاعات انجام گردید";
    public const string CODE_NOTFOUND = "کد مورد نظر یافت نشد";
    public enum Flag : byte { Save, Send, ManagerConfirm, AdminConfirm };
    public enum IntegrationMode : byte { Integration = 1, DisIntegration = 2 };
    public enum Role : byte { Administrator = 1, AreaManager = 2, SchoolManager = 3, Repository = 4, Guest = 5 };
    public static string ConnectionString
    {
        get
        {
            string connectionString = HttpContext.Current.Cache["ConnStr"] as string;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = ConfigurationManager.ConnectionStrings["SupplySystemCS"].ConnectionString;
                HttpContext.Current.Cache.Insert("ConnStr", connectionString, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
            }
            return connectionString;
        }
    }

    #region Convertion Methods

    public static short ToShort(object input)
    {
        short result = 0;
        short.TryParse(input.ToString(), out result);
        return result;
    }

    public static int ToInt(object input)
    {
        int result = 0;
        int.TryParse(input.ToString(), out result);
        return result;
    }

    public static long ToLong(object input)
    {
        long result = 0;
        long.TryParse(input.ToString(), out result);
        return result;
    }

    public static decimal ToDecimal(object input)
    {
        decimal result = 0;
        decimal.TryParse(input.ToString(), out result);
        return result;
    }

    public static byte ToByte(object input)
    {
        byte result = 0;
        byte.TryParse(input.ToString(), out result);
        return result;
    }

    public static bool ToBool(object input)
    {
        bool result = false;
        bool.TryParse(input.ToString(), out result);
        return result;
    }

    public static string ToHex(object input)
    {
        return string.Format("{0:X}", input);
    }

    public static string ToPersianDate(object date)
    {
        string result = null;
        if (date != null)
        {
            DateTime dt = (DateTime)date;
            PersianCalendar objPersianCalendar = new PersianCalendar();
            int year = objPersianCalendar.GetYear(dt);
            int month = objPersianCalendar.GetMonth(dt);
            int day = objPersianCalendar.GetDayOfMonth(dt);
            int hour = objPersianCalendar.GetHour(dt);
            int min = objPersianCalendar.GetMinute(dt);
            int sec = objPersianCalendar.GetSecond(dt);
            result = string.Concat(year.ToString().PadLeft(4, '0'), DateTimeFormatInfo.CurrentInfo.DateSeparator, month.ToString().PadLeft(2, '0'), DateTimeFormatInfo.CurrentInfo.DateSeparator, day.ToString().PadLeft(2, '0'));
        }
        return result;
    }

    public static string ToPersianDateTime(object date)
    {
        string result = null;
        if (date != null)
        {
            DateTime dt = (DateTime)date;
            PersianCalendar objPersianCalendar = new PersianCalendar();
            int year = objPersianCalendar.GetYear(dt);
            int month = objPersianCalendar.GetMonth(dt);
            int day = objPersianCalendar.GetDayOfMonth(dt);
            int hour = objPersianCalendar.GetHour(dt);
            int minute = objPersianCalendar.GetMinute(dt);
            result = string.Format("{0}/{1}/{2}  {3}:{4}", year, month, day, hour, minute);
        }
        return result;
    }

    public static bool IsDate(string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            return false;
        }

        date = date.Replace("/", string.Empty);
        date = date.Replace("-", string.Empty);
        date = date.Replace("\0", string.Empty);
        date = date.Replace("_", string.Empty);
        if (date.Length == 8)
        {
            return true;
        }
        {
            return false;
        }
    }

    #endregion

    #region Public Methods

    public static string GetGender(byte gender)
    {
        string result = null;
        switch (gender)
        {
            case 0:
                result = "پسرانه";
                break;

            case 1:
                result = "دخترانه";
                break;

            case 2:
                result = "مختلط";
                break;
        }
        return result;
    }

    public static void ExportInfo(byte exportType, StiReport report)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            string fileName = "rep";
            switch (exportType)
            {
                case 0:// "pdf"
                    report.ExportDocument(StiExportFormat.Pdf, HttpContext.Current.Server.MapPath(string.Format("~/App_Data/Report/Tmp/{0}.Pdf", fileName)));
                    report.ExportDocument(StiExportFormat.Pdf, ms);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.pdf", fileName));
                    HttpContext.Current.Response.ContentType = "application/pdf";
                    break;
                case 1:// "jpeg"
                    report.ExportDocument(StiExportFormat.ImageJpeg, HttpContext.Current.Server.MapPath(string.Format("~/App_Data/Report/Tmp/{0}.jpeg", fileName)));
                    report.ExportDocument(StiExportFormat.ImageJpeg, ms);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.jpeg", fileName));
                    HttpContext.Current.Response.ContentType = "application/ImageJpeg";
                    break;
                case 2: // "html":
                    report.ExportDocument(StiExportFormat.Html, HttpContext.Current.Server.MapPath(string.Format("~/App_Data/Report/Tmp/{0}.html", fileName)));
                    report.ExportDocument(StiExportFormat.Html, ms);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.html", fileName));
                    HttpContext.Current.Response.ContentType = "application/html";
                    break;
                case 3: // "xls":
                    report.ExportDocument(StiExportFormat.Excel, HttpContext.Current.Server.MapPath(string.Format("~/App_Data/Report/Tmp/{0}.xls", fileName)));
                    report.ExportDocument(StiExportFormat.Excel, ms);
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", fileName));
                    HttpContext.Current.Response.ContentType = "application/xls";
                    break;
            }
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
        }
    }

    #endregion

    #region CycleMonths

    public static SupplySystem.Cycle ActiveCycle
    {
        get
        {
            SupplySystem.Cycle cycle = HttpContext.Current.Cache["ActiveCycle"] as SupplySystem.Cycle;
            if (cycle == null)
            {
                SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
                cycle = db.Cycles.FirstOrDefault<SupplySystem.Cycle>(c => c.IsActive);
                HttpContext.Current.Cache.Insert("ActiveCycle", cycle, null, DateTime.MaxValue, TimeSpan.FromMinutes(10));
            }

            return cycle;
        }
    }

    public static void LoadCycleMonths(DropDownList drp)
    {
        SupplySystem.Cycle cycle = ActiveCycle;
        if (cycle != null)
        {
            int[] startDate = Persia.Calendar.ConvertToPersian(cycle.StartDate).ArrayType;
            int[] endDate = Persia.Calendar.ConvertToPersian(cycle.EndDate).ArrayType;

            if (startDate[0] == endDate[0]) // Both dates are in the same year
            {
                for (int i = startDate[1]; i <= endDate[1]; i++)
                {
                    drp.Items.Add(new ListItem(string.Format("{0} {1}", MonthName(i), startDate[0]), string.Concat(startDate[0], "|", i)));
                }
            }
            else
            {
                ArrayList months = MonthsInStartYear(startDate[1], startDate[0], endDate[1], endDate[0]);
                string[] month = null;

                for (int i = 1; i <= months.Count; i++)
                {
                    month = months[i - 1].ToString().Split(',');
                    drp.Items.Add(new ListItem(string.Format("{0} {1}", month[1], month[2]), month[0]));
                }
            }

            drp.DataBind();
            drp.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
        }
    }

    public static void LoadCycleMonths(DropDownList drp, int cycleId)
    {
        SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
        SupplySystem.Cycle cycle = db.Cycles.FirstOrDefault<SupplySystem.Cycle>(c => c.CycleID == cycleId);
        if (cycle != null)
        {
            int[] startDate = Persia.Calendar.ConvertToPersian(cycle.StartDate).ArrayType;
            int[] endDate = Persia.Calendar.ConvertToPersian(cycle.EndDate).ArrayType;

            if (startDate[0] == endDate[0]) // Both dates are in the same year
            {
                for (int i = startDate[1]; i <= endDate[1]; i++)
                {
                    drp.Items.Add(new ListItem(string.Format("{0} {1}", MonthName(i), startDate[0]), string.Concat(startDate[0], "|", i)));
                }
            }
            else
            {
                ArrayList months = MonthsInStartYear(startDate[1], startDate[0], endDate[1], endDate[0]);
                string[] month = null;

                for (int i = 1; i <= months.Count; i++)
                {
                    month = months[i - 1].ToString().Split(',');
                    drp.Items.Add(new ListItem(string.Format("{0} {1}", month[1], month[2]), month[0]));
                }
            }

            drp.DataBind();
            drp.Items.Insert(0, new ListItem("-- همه ماهها --", "0"));
        }
    }

    public static ArrayList MonthsInStartYear(int startMonth, int startYear, int endMonth, int endYear)
    {
        ArrayList months = new ArrayList();
        int index = 1;

        while (startMonth <= 12)
        {
            months.Add(string.Concat(startYear, "|", startMonth, ",", MonthName(startMonth), ",", startYear));
            startMonth++;
            index++;
        }

        for (int j = 1; j <= endMonth; j++)
        {
            months.Add(string.Concat(endYear, "|", j, ",", MonthName(j), ",", endYear));
            index++;
        }
        return months;
    }

    private static string MonthName(int month)
    {
        string monthName = null;

        switch (month)
        {
            case 1:
                monthName = "فروردین";
                break;

            case 2:
                monthName = "اردیبهشت";
                break;

            case 3:
                monthName = "خرداد";
                break;

            case 4:
                monthName = "تیر";
                break;

            case 5:
                monthName = "مرداد";
                break;

            case 6:
                monthName = "شهریور";
                break;

            case 7:
                monthName = "مهر";
                break;

            case 8:
                monthName = "آبان";
                break;

            case 9:
                monthName = "آذر";
                break;

            case 10:
                monthName = "دی";
                break;

            case 11:
                monthName = "بهمن";
                break;

            case 12:
                monthName = "اسفند";
                break;
        }
        return monthName;
    }

    #endregion
}

public static class TamperProofString
{
    private static string TamperProofStringEncode(string value, string key)
    {
        System.Security.Cryptography.MACTripleDES mac3des = new System.Security.Cryptography.MACTripleDES();
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
        return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value)) + System.Convert.ToChar("-") + System.Convert.ToBase64String(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
    }

    private static string TamperProofStringDecode(string value, string key)
    {
        String dataValue = "";
        String calcHash = "";
        String storedHash = "";

        System.Security.Cryptography.MACTripleDES mac3des = new System.Security.Cryptography.MACTripleDES();
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        mac3des.Key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));

        try
        {
            dataValue = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(value.Split(System.Convert.ToChar("-"))[0]));
            storedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(value.Split(System.Convert.ToChar("-"))[1]));
            calcHash = System.Text.Encoding.UTF8.GetString(mac3des.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dataValue)));

            if (storedHash != calcHash)
            {
                throw new ArgumentException("Hash value does not match");
            }
        }
        catch (System.Exception)
        {
            throw new ArgumentException("Invalid TamperProofString");
        }
        return dataValue;
    }

    public static string QueryStringEncode(string value)
    {
        return System.Web.HttpUtility.UrlEncode(TamperProofStringEncode(value, System.Configuration.ConfigurationManager.AppSettings["TamperProofKey"]));
    }

    public static string QueryStringDecode(string value)
    {
        return TamperProofStringDecode(value, System.Configuration.ConfigurationManager.AppSettings["TamperProofKey"]);
    }
}


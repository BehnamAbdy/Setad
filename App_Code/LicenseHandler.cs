using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public class LicenseHandler : IHttpHandler
{
    #region IHttpHandler Members

    public bool IsReusable
    {
        get { return true; }
    }

    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.QueryString["fp"] != null)
        {
            context.Response.Write(IsFingerPrintValid(context.Request.QueryString["fp"]) == true ? "1" : "0");
        }
    }

    #endregion

    #region DAL

    public bool IsFingerPrintValid(string fingerPrint)
    {
        SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["LicenseConnectionString"].ConnectionString);
        SqlCommand command = new SqlCommand("Licenses_Insert", connection);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@FingerPrint", SqlDbType.Char, 39)).Value = fingerPrint;
        command.Parameters.Add(new SqlParameter("@Result", SqlDbType.Bit)).Direction = ParameterDirection.Output;

        try
        {
            connection.Open();
            command.ExecuteNonQuery();
            return (bool)command.Parameters["@Result"].Value;
        }
        catch
        {
            return false;
        }
        finally
        {
            connection.Close();
        }
    }

    #endregion
}


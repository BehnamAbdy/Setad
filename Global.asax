<%@ Application Language="C#" %>
<script RunAt="server">

    private void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

        if (authCookie == null)
        {
            return;
        }

        FormsAuthenticationTicket authTicket = null;
        try
        {
            authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        }
        catch
        {
            return;
        }

        if (authTicket == null)
        {
            return;
        }

        System.Security.Principal.GenericIdentity identity = new System.Security.Principal.GenericIdentity(authTicket.Name, "Forms");
        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(identity, authTicket.UserData.Split(','));
    }

    //private void Application_Error(object sender, EventArgs e)
    //{
    //    System.Globalization.PersianCalendar objPersianCalendar = new System.Globalization.PersianCalendar();
    //    int year = objPersianCalendar.GetYear(DateTime.Now);
    //    int month = objPersianCalendar.GetMonth(DateTime.Now);
    //    int day = objPersianCalendar.GetDayOfMonth(DateTime.Now);
    //    int hour = objPersianCalendar.GetHour(DateTime.Now);
    //    int min = objPersianCalendar.GetMinute(DateTime.Now);
    //    System.IO.StreamWriter objStreamWriter = System.IO.File.AppendText(Server.MapPath("~/log.txt"));
    //    objStreamWriter.WriteLine(string.Concat("Log Date : ", string.Concat(year.ToString(), "/", month.ToString(), "/", day.ToString(), " ", hour.ToString(), ":", min.ToString())));
    //    Exception ex = Server.GetLastError();
    //    string log = string.Format("Exception Type : {0}\n Message : {1} \n Source : {2} \n InnerException : {3} \n StackTrace : {4} \n TargetSite : {5}", ex.GetType(), ex.Message, ex.Source, ex.InnerException, ex.StackTrace, ex.TargetSite);
    //    objStreamWriter.WriteLine(log);
    //    objStreamWriter.WriteLine("--------------------------------------------");
    //    objStreamWriter.Close();
    //}
    
</script>

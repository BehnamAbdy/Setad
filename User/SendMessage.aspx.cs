using System;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web;

public partial class User_SendMessage : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

            if (Request.QueryString["trv"] != null)
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string treeJSON = null;
                switch (Request.QueryString["trv"])
                {
                    case "0": // managers mode
                        if (HttpContext.Current.User.IsInRole("AreaManager")) // Just sees the administrator
                        {
                            treeJSON = serializer.Serialize(new { key = "2539", title = "مدیر سیستم", icon = "manager.png" });
                            Response.Clear();
                            Response.Write(treeJSON);
                            Response.End();
                        }
                        else if (HttpContext.Current.User.IsInRole("SchoolManager")) // Just sees hisown areamanager
                        {
                            var areaManager = from ar in db.Areas
                                              join ur in db.UsersInRoles on ar.AreaCode equals ur.AreaCode
                                              where ar.AreaCode == (from s in db.Schools where s.SchoolCode.ToString().Equals(HttpContext.Current.User.Identity.Name) select s.AreaCode).FirstOrDefault()
                                              select
                                              new
                                              {
                                                  key = ur.UserInRoleID,
                                                  title = ar.AreaName
                                              };
                            foreach (var item in areaManager)
                            {
                                treeJSON = serializer.Serialize(new { key = item.key, title = string.Format("مدیر منطقه {0}", item.title), icon = "manager.png" });
                            }
                            Response.Clear();
                            Response.Write(treeJSON);
                            Response.End();
                        }

                        var areaManagers = from ar in db.Areas
                                           join ur in db.UsersInRoles on ar.AreaCode equals ur.AreaCode
                                           orderby ar.AreaName
                                           select
                                           new
                                           {
                                               ar.AreaCode,
                                               key = ur.UserInRoleID,
                                               title = ar.AreaName,
                                               icon = "manager.png"
                                           };

                        if (HttpContext.Current.User.IsInRole("SchoolManager")) // Just sees hisown areamanager
                        {
                            areaManagers = from q in areaManagers
                                           where q.AreaCode == (from s in db.Schools where s.AreaCode.ToString().Equals(HttpContext.Current.User.Identity.Name) select s.AreaCode).SingleOrDefault()
                                           select q;
                        }
                        treeJSON = serializer.Serialize(from q in areaManagers select new { q.key, q.title, q.icon });
                        break;

                    case "1": // School managers mode
                        if (System.Web.HttpContext.Current.User.IsInRole("SchoolManager"))
                        {
                            Response.Clear();
                            Response.Write(null);
                            Response.End();
                        }
                        var schoolManagers = from ar in db.Areas
                                             join ur in db.UsersInRoles on ar.AreaCode equals ur.AreaCode
                                             orderby ar.AreaName
                                             select
                                             new
                                             {
                                                 key = ar.AreaCode,
                                                 title = ar.AreaName,
                                                 icon = "area.png",
                                                 children = (from s in db.Schools
                                                             join ur2 in db.UsersInRoles on s.SchoolID equals ur2.SchoolID
                                                             where s.AreaCode == ar.AreaCode
                                                             orderby s.SchoolName
                                                             select new { key = ur2.UserInRoleID, title = s.SchoolName, icon = "school.png" })
                                             };

                        if (HttpContext.Current.User.IsInRole("AreaManager"))
                        {
                            schoolManagers = from q in schoolManagers
                                             where q.key == (from u in db.Users join ur in db.UsersInRoles on u.UserID equals ur.UserID where u.UserName.Equals(HttpContext.Current.User.Identity.Name) select ur.AreaCode).SingleOrDefault()
                                             select q;
                        }
                        treeJSON = serializer.Serialize(schoolManagers);
                        break;
                }
                Response.Clear();
                Response.Write(treeJSON);
                Response.End();
            }
            else if (Request.HttpMethod == "POST" && Request.Params["sbj"] != null && Request.Params["bdy"] != null && Request.Params["ids"] != null)
            {
                if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
                {
                    return;
                }
                string[] userInRoles = Request.Params["ids"].Split(',');
                if (userInRoles.Length > 0)
                {
                    SupplySystem.Message message = new SupplySystem.Message();
                    message.Subject = Request.Params["sbj"].Trim();
                    message.Body = Request.Params["bdy"];
                    message.UserInRoleID = ((SupplySystem.User)this.Session["User"]).UsersInRoles.First<SupplySystem.UsersInRole>().UserInRoleID;
                    message.SubmitDate = DateTime.Now;
                    foreach (string item in userInRoles)
                    {
                        message.MessageUsers.Add(new SupplySystem.MessageUser { UserInRoleID = int.Parse(item) });
                    }
                    db.Messages.InsertOnSubmit(message);
                    db.SubmitChanges();
                    Response.Clear();
                    Response.Write('1');
                    Response.End();
                }
            }
        }
    }
}
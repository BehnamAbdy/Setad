using System;
using System.Linq;

public partial class Lists_StuffsList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            int repositoryId = 0;
            int stuffTypeId = 0;

            if (int.TryParse(Request.QueryString["rId"], out repositoryId))
            {
                this.grdStuffs.DataSource = (from st in db.REP_Stuffs
                                             join repGd in db.REP_RepositoryStuffs on st.StuffID equals repGd.StuffID
                                             where repGd.RepositoryID == repositoryId
                                             select new { st.StuffID, st.StuffName }).ToList();
            }
            else if (int.TryParse(Request.QueryString["st"], out stuffTypeId))
            {
                this.grdStuffs.DataSource = (from st in db.REP_Stuffs
                                             where st.StuffTypeID == stuffTypeId
                                             select new { st.StuffID, st.StuffName }).ToList();
            }
            else
            {
                this.grdStuffs.DataSource = db.REP_Stuffs.Select(st => new { st.StuffID, st.StuffName }).ToList();
            }

            this.grdStuffs.DataBind();
        }
    }
}

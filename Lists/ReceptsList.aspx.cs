using System;
using System.Linq;

public partial class Lists_ReceptsList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        byte type = 0;
        if (!IsPostBack && byte.TryParse(Request.QueryString["type"], out type))
        {
            SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
            var reporitories = from rct in db.REP_Recepts
                               join ar in db.Areas on rct.AreaCode equals ar.AreaCode
                               join rep in db.REP_Repositories on rct.RepositoryID equals rep.RepositoryID
                               where rct.ReceptType == type
                               select new
                               {
                                   rct.ReceptID,
                                   rct.ReceptCode,
                                   rct.ReceptDate,
                                   rep.RepositoryName,
                                   ar.AreaName
                               };
            this.grdRecepts.DataSource = reporitories.ToList();
            this.grdRecepts.DataBind();
        }
    }
}

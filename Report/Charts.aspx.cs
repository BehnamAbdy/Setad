using System;
using System.Linq;
using System.Web.UI.WebControls;

public partial class Report_Charts : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            short cycleStuffId = 0;
            if (Request.QueryString["mode"] != null && short.TryParse(Request.QueryString["cs"], out cycleStuffId))
            {
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                string jsonData = null;

                short solarYear = Public.ToShort(Request.QueryString["y"]);
                byte solarMonth = Public.ToByte(Request.QueryString["m"]);

                switch (Request.QueryString["mode"])
                {
                    case "1":
                        var ration = from sf in db.SchoolFoods
                                     join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                     join ssl in db.SchoolSubLevels on sf.SchoolSubLevelID equals ssl.SchoolSubLevelID
                                     join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                                     join s in db.Schools on slv.SchoolID equals s.SchoolID
                                     where slv.LockOutDate == null && sf.CycleFoodID == cycleStuffId
                                     select new
                                     {
                                         cl.SolarYear,
                                         cl.SolarMonth,
                                         s.AreaCode,
                                         sf.FoodCount
                                     };

                        if (solarYear > 0 && solarMonth > 0)
                        {
                            ration = from q in ration
                                     where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                                     select q;
                        }

                        var areaRation = from ar in db.Areas
                                         select new
                                         {
                                             ar.AreaCode,
                                             ar.AreaName,
                                             Count = (from r in ration
                                                      where r.AreaCode == ar.AreaCode
                                                      select new
                                                      {
                                                          r.FoodCount
                                                      }).Sum(fc => (int?)fc.FoodCount)
                                         };
                        areaRation = from q in areaRation where q.Count > 0 orderby q.AreaName select q;
                        jsonData = serializer.Serialize(areaRation);
                        break;

                    case "2":
                        var paperity = from sp in db.SchoolPaperities
                                       join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                                       join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                                       join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                                       join s in db.Schools on slv.SchoolID equals s.SchoolID
                                       where slv.LockOutDate == null && sp.CyclePaperityID == cycleStuffId
                                       select new
                                       {
                                           cl.SolarYear,
                                           cl.SolarMonth,
                                           s.AreaCode,
                                           sp.PaperityCount
                                       };

                        if (solarYear > 0 && solarMonth > 0)
                        {
                            paperity = from q in paperity
                                       where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                                       select q;
                        }

                        var paperityRation = from ar in db.Areas
                                             select new
                                             {
                                                 ar.AreaCode,
                                                 ar.AreaName,
                                                 Count = (from r in paperity
                                                          where r.AreaCode == ar.AreaCode
                                                          select new
                                                          {
                                                              r.PaperityCount
                                                          }).Sum(pc => (int?)pc.PaperityCount)
                                             };
                        paperityRation = from q in paperityRation where q.Count > 0 orderby q.AreaName select q;
                        jsonData = serializer.Serialize(paperityRation);
                        break;

                    case "3":
                        var clothe = from ar in db.Areas
                                     select new
                                     {
                                         ar.AreaCode,
                                         ar.AreaName,
                                         Count = (from s in db.Schools
                                                  join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                                                  join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                                                  join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                                                  join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                                                  where slv.LockOutDate == null && sc.CycleClotheID == cycleStuffId && s.AreaCode == ar.AreaCode
                                                  select new
                                                  {
                                                      st.ClotheCount
                                                  }).Sum(st => (int?)st.ClotheCount)
                                     };
                        clothe = from q in clothe where q.Count > 0 orderby q.AreaName select q;
                        jsonData = serializer.Serialize(clothe);
                        break;
                }

                Response.Clear();
                Response.Write(jsonData);
                Response.End();
            }

            if (Request.QueryString["mode"] == "3")
            {
                this.drpMonths.Enabled = false;
            }
            this.drpCycle.DataSource = db.Cycles;
            this.drpCycle.DataBind();
            this.drpCycle.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
            this.drpCycle.SelectedValue = Public.ActiveCycle.CycleID.ToString();
            this.drpCycle_SelectedIndexChanged(this, e);
        }
    }

    protected void drpCycle_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.drpCycle.SelectedIndex > 0)
        {
            this.drpMonths.Items.Clear();
            this.drpStuffs.Items.Clear();

            int cycleId = Public.ToInt(this.drpCycle.SelectedValue);
            Public.LoadCycleMonths(this.drpMonths, cycleId);
            LoadStuffs(cycleId);
        }
        else
        {
            this.drpMonths.Items.Clear();
            this.drpStuffs.Items.Clear();
            this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
        }
    }

    private void LoadStuffs(int cycleId)
    {
        switch (Request.QueryString["mode"])
        {
            case "1":
                this.drpStuffs.DataSource = from st in db.REP_Stuffs
                                            join cg in db.CycleFoods on st.StuffID equals cg.StuffID
                                            where cg.CycleID == cycleId
                                            select new { CycleStuffID = cg.CycleFoodID, st.StuffName };
                break;

            case "2":
                this.drpStuffs.DataSource = from cp in db.CyclePaperities
                                            join st in db.REP_Stuffs on cp.StuffID equals st.StuffID
                                            where cp.CycleID == Public.ToInt(this.drpCycle.SelectedValue)
                                            select new { CycleStuffID = cp.CyclePaperityID, st.StuffName };
                break;

            case "3":
                this.drpStuffs.DataSource = from st in db.REP_Stuffs
                                            join cc in db.CycleClothes on st.StuffID equals cc.StuffID
                                            where cc.CycleID == cycleId
                                            select new { CycleStuffID = cc.CycleClotheID, st.StuffName };
                break;
        }

        this.drpStuffs.DataBind();
        this.drpStuffs.Items.Insert(0, new ListItem("-- انتخاب کنید --", "0"));
    }
}
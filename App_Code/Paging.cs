using System.Linq;
using System;

public class Paging
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);

    #region SchoolsReport.aspx

    public int GetSchoolsCount(int schoolCode_From, int schoolCode_To, string schoolName, short levelId, short schoolKindId, byte gender, int areaCode)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join lv in db.Levels on slv.LevelID equals lv.LevelID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where slv.LockOutDate == null
                    select
                    new
                    {
                        s.SchoolCode,
                        s.SchoolName,
                        s.Gender,
                        s.SchoolKindID,
                        s.EmployeesCount_Changable,
                        s.EmployeesCount_Fixed,
                        lv.LevelID,
                        a.AreaCode,
                    };

        if (schoolCode_From > 0 && schoolCode_To == 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode_From
                    select q;
        }
        else if (schoolCode_From > 0 && schoolCode_To > 0)
        {
            query = from q in query
                    where q.SchoolCode >= schoolCode_From && q.SchoolCode <= schoolCode_To
                    select q;
        }

        if (!string.IsNullOrEmpty(schoolName))
        {
            query = from q in query
                    where q.SchoolName.Contains(schoolName)
                    select q;
        }

        if (levelId > 0)
        {
            query = from q in query
                    where q.LevelID == levelId
                    select q;
        }

        if (schoolKindId > 0)
        {
            query = from q in query
                    where q.SchoolKindID == schoolKindId
                    select q;
        }

        if (gender < 3)
        {
            query = from q in query
                    where q.Gender == gender
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        return query.Count();
    }

    public IQueryable LoadSchools(int maximumRows, int startRowIndex, int schoolCode_From, int schoolCode_To, string schoolName, short levelId, short schoolKindId, byte gender, int areaCode)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join lv in db.Levels on slv.LevelID equals lv.LevelID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where slv.LockOutDate == null
                    orderby a.AreaCode, s.SchoolCode
                    select
                    new
                    {
                        s.SchoolCode,
                        s.SchoolName,
                        s.Gender,
                        s.SchoolKindID,
                        s.EmployeesCount_Changable,
                        s.EmployeesCount_Fixed,
                        lv.LevelID,
                        lv.LevelName,
                        a.AreaName,
                        a.AreaCode,
                        GirlsCount = GetGirlsCount(s),
                        BoysCount = GetBoysCount(s)
                    };

        if (schoolCode_From > 0 && schoolCode_To == 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode_From
                    select q;
        }
        else if (schoolCode_From > 0 && schoolCode_To > 0)
        {
            query = from q in query
                    where q.SchoolCode >= schoolCode_From && q.SchoolCode <= schoolCode_To
                    select q;
        }

        if (!string.IsNullOrEmpty(schoolName))
        {
            query = from q in query
                    where q.SchoolName.Contains(schoolName)
                    select q;
        }

        if (levelId > 0)
        {
            query = from q in query
                    where q.LevelID == levelId
                    select q;
        }

        if (schoolKindId > 0)
        {
            query = from q in query
                    where q.SchoolKindID == schoolKindId
                    select q;
        }

        if (gender < 3)
        {
            query = from q in query
                    where q.Gender == gender
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    private short GetGirlsCount(SupplySystem.School sch)
    {
        short count = 0;
        foreach (SupplySystem.SchoolSubLevel ssl in sch.SchoolLevels.First<SupplySystem.SchoolLevel>(slv => slv.LockOutDate == null).SchoolSubLevels)
        {
            count += ssl.GirlsCount.GetValueOrDefault();
        }
        return count;
    }

    private short GetBoysCount(SupplySystem.School sch)
    {
        short count = 0;
        foreach (SupplySystem.SchoolSubLevel ssl in sch.SchoolLevels.First<SupplySystem.SchoolLevel>(slv => slv.LockOutDate == null).SchoolSubLevels)
        {
            count += ssl.BoysCount.GetValueOrDefault();
        }
        return count;
    }

    #endregion

    #region SchoolRationRep.aspx

    public int GetSchoolRationsCount(short solarYear, byte solarMonth, short cycleFoodId, int areaCode, int schoolCode)
    {
        var query = from sf in db.SchoolFoods
                    join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                    join ssl in db.SchoolSubLevels on sf.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join cf in db.CycleFoods on sf.CycleFoodID equals cf.CycleFoodID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             sf.CycleFoodID == cycleFoodId && slv.LockOutDate == null
                    group ssl by new
                    {
                        s.SchoolCode,
                        a.AreaCode,
                        cl.SolarYear,
                        cl.SolarMonth,
                        cl.SolarDay
                    } into grp
                    select new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.AreaCode,
                        grp.Key.SolarYear,
                        grp.Key.SolarMonth,
                        grp.Key.SolarDay
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        return query.Count();
    }

    public IQueryable LoadSchoolRations(int maximumRows, int startRowIndex, short solarYear, byte solarMonth, short cycleFoodId, int areaCode, int schoolCode)
    {
        var query = from sf in db.SchoolFoods
                    join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                    join ssl in db.SchoolSubLevels on sf.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join lv in db.Levels on slv.LevelID equals lv.LevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join cf in db.CycleFoods on sf.CycleFoodID equals cf.CycleFoodID
                    join g in db.REP_Stuffs on cf.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    orderby a.AreaCode, s.SchoolCode, sf.CalendarID
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             sf.CycleFoodID == cycleFoodId && slv.LockOutDate == null
                    group ssl by new
                    {
                        s.SchoolID,
                        s.SchoolCode,
                        s.SchoolName,
                        lv.LevelName,
                        a.AreaCode,
                        a.AreaName,
                        g.StuffName,
                        cl.SolarYear,
                        cl.SolarMonth,
                        cl.SolarDay
                    } into grp
                    orderby grp.Key.SchoolCode, grp.Key.SolarYear, grp.Key.SolarMonth, grp.Key.SolarDay
                    select new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.SchoolName,
                        grp.Key.AreaCode,
                        grp.Key.AreaName,
                        grp.Key.LevelName,
                        grp.Key.StuffName,
                        grp.Key.SolarYear,
                        grp.Key.SolarMonth,
                        grp.Key.SolarDay,
                        FoodCount = (from sf2 in db.SchoolFoods
                                     join cl2 in db.Calendars on sf2.CalendarID equals cl2.CalendarID
                                     join ssl2 in db.SchoolSubLevels on sf2.SchoolSubLevelID equals ssl2.SchoolSubLevelID
                                     join slv2 in db.SchoolLevels on ssl2.SchoolLevelID equals slv2.SchoolLevelID
                                     where slv2.SchoolID == grp.Key.SchoolID && slv2.LockOutDate == null &&
                                              sf2.CycleFoodID == cycleFoodId && cl2.SolarYear == grp.Key.SolarYear &&
                                              cl2.SolarMonth == grp.Key.SolarMonth && cl2.SolarDay == grp.Key.SolarDay
                                     select new
                                     {
                                         sf2.FoodCount
                                     }).Sum(fc => fc.FoodCount)
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region SublevelRationRep.aspx

    public int GetSubLevelRationsCount(short solarYear, byte solarMonth, short cycleFoodId, int areaCode, int schoolCode, int subLevelId)
    {
        var query = from sf in db.SchoolFoods
                    join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                    join ssl in db.SchoolSubLevels on sf.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             sf.CycleFoodID == cycleFoodId && slv.LockOutDate == null
                    select new
                    {
                        s.SchoolCode,
                        s.AreaCode,
                        ssl.SchoolSubLevelID,
                        cl.SolarYear,
                        cl.SolarMonth
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        return query.Count();
    }

    public IQueryable LoadSubLevelRations(int maximumRows, int startRowIndex, short solarYear, byte solarMonth, short cycleFoodId, int areaCode, int schoolCode, int subLevelId)
    {
        var query = from sf in db.SchoolFoods
                    join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                    join ssl in db.SchoolSubLevels on sf.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                    join cf in db.CycleFoods on sf.CycleFoodID equals cf.CycleFoodID
                    join g in db.REP_Stuffs on cf.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             sf.CycleFoodID == cycleFoodId && slv.LockOutDate == null
                    orderby a.AreaCode, s.SchoolCode, sf.CalendarID
                    select new
                    {
                        s.SchoolCode,
                        s.SchoolName,
                        s.AreaCode,
                        a.AreaName,
                        ssl.SchoolSubLevelID,
                        sl.SubLevelName,
                        g.StuffName,
                        sf.FoodCount,
                        cl.SolarYear,
                        cl.SolarMonth,
                        cl.SolarDay
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region SchoolPaperityRep.aspx

    public int GetSchoolPaperityCount(short solarYear, byte solarMonth, short cyclePaperityId, int areaCode, int schoolCode)
    {
        var query = from sp in db.SchoolPaperities
                    join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                    join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                    join ar in db.Areas on s.AreaCode equals ar.AreaCode
                    where slv.LockOutDate == null && cp.CyclePaperityID == cyclePaperityId
                    group ssl by new
                    {
                        s.SchoolID,
                        s.SchoolCode,
                        ar.AreaCode,
                        sp.CyclePaperityID,
                        cl.SolarYear,
                        cl.SolarMonth,
                        cl.SolarDay
                    } into grp
                    select
                    new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.AreaCode,
                        grp.Key.CyclePaperityID,
                        grp.Key.SolarYear,
                        grp.Key.SolarMonth,
                        grp.Key.SolarDay
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        return query.Count();
    }

    public IQueryable LoadSchoolPaperityRations(int maximumRows, int startRowIndex, short solarYear, byte solarMonth, short cyclePaperityId, int areaCode, int schoolCode)
    {
        var query = from sp in db.SchoolPaperities
                    join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join lv in db.Levels on slv.LevelID equals lv.LevelID
                    join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                    join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                    join g in db.REP_Stuffs on cp.StuffID equals g.StuffID
                    join ar in db.Areas on s.AreaCode equals ar.AreaCode
                    where slv.LockOutDate == null && cp.CyclePaperityID == cyclePaperityId
                    group ssl by new
                    {
                        s.SchoolID,
                        s.SchoolCode,
                        s.SchoolName,
                        lv.LevelName,
                        ar.AreaCode,
                        ar.AreaName,
                        g.StuffName,
                        sp.CyclePaperityID,
                        cl.SolarYear,
                        cl.SolarMonth,
                        cl.SolarDay
                    } into grp
                    orderby grp.Key.SchoolCode, grp.Key.SolarYear, grp.Key.SolarMonth, grp.Key.SolarDay
                    select
                    new
                    {
                        grp.Key.SchoolID,
                        grp.Key.SchoolCode,
                        grp.Key.SchoolName,
                        grp.Key.LevelName,
                        grp.Key.AreaCode,
                        grp.Key.AreaName,
                        grp.Key.StuffName,
                        grp.Key.CyclePaperityID,
                        grp.Key.SolarYear,
                        grp.Key.SolarMonth,
                        PaperityCount = (from sp2 in db.SchoolPaperities
                                         join cl2 in db.Calendars on sp2.CalendarID equals cl2.CalendarID
                                         join ssl2 in db.SchoolSubLevels on sp2.SchoolSubLevelID equals ssl2.SchoolSubLevelID
                                         join slv2 in db.SchoolLevels on ssl2.SchoolLevelID equals slv2.SchoolLevelID
                                         join cp2 in db.CyclePaperities on sp2.CyclePaperityID equals cp2.CyclePaperityID
                                         where slv2.SchoolID == grp.Key.SchoolID && slv2.LockOutDate == null &&
                                                  cp2.CyclePaperityID == cyclePaperityId && cl2.SolarYear == grp.Key.SolarYear &&
                                                  cl2.SolarMonth == grp.Key.SolarMonth && cl2.SolarDay == grp.Key.SolarDay
                                         select
                                         new
                                         {
                                             sp2.PaperityCount
                                         }).Sum(s => s.PaperityCount)
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region PaperityReport.aspx

    public int GetSubLevelPaperitiesCount(short solarYear, byte solarMonth, short cyclePaperityId, int areaCode, int schoolCode, int subLevelId)
    {
        var query = from sp in db.SchoolPaperities
                    join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                    join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             slv.LockOutDate == null && cp.CyclePaperityID == cyclePaperityId
                    select new
                    {
                        s.SchoolCode,
                        s.AreaCode,
                        ssl.SchoolSubLevelID,
                        cl.SolarYear,
                        cl.SolarMonth
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        return query.Count();
    }

    public IQueryable LoadSubLevelPaperities(int maximumRows, int startRowIndex, short solarYear, byte solarMonth, short cyclePaperityId, int areaCode, int schoolCode, int subLevelId)
    {
        var query = from sp in db.SchoolPaperities
                    join ssl in db.SchoolSubLevels on sp.SchoolSubLevelID equals ssl.SchoolSubLevelID
                    join slv in db.SchoolLevels on ssl.SchoolLevelID equals slv.SchoolLevelID
                    join s in db.Schools on slv.SchoolID equals s.SchoolID
                    join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                    join cl in db.Calendars on sp.CalendarID equals cl.CalendarID
                    join cp in db.CyclePaperities on sp.CyclePaperityID equals cp.CyclePaperityID
                    join g in db.REP_Stuffs on cp.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             slv.LockOutDate == null && cp.CyclePaperityID == cyclePaperityId
                    orderby s.SchoolCode, cl.CalendarID, sl.SubLevelID
                    select new
                    {
                        s.SchoolCode,
                        s.SchoolName,
                        s.AreaCode,
                        a.AreaName,
                        ssl.SchoolSubLevelID,
                        sl.SubLevelName,
                        g.StuffName,
                        sp.PaperityCount,
                        cl.SolarYear,
                        cl.SolarMonth
                    };

        if (solarYear > 0 && solarMonth > 0)
        {
            query = from q in query
                    where q.SolarYear == solarYear && q.SolarMonth == solarMonth
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region SchoolClotheRep.aspx

    public int GetSchoolClothesCount(int cycleClotheId, int areaCode, int schoolCode)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                    join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                    join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                    join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                    join g in db.REP_Stuffs on cc.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                               slv.LockOutDate == null && cc.CycleClotheID == cycleClotheId
                    group ssl by new { s.SchoolCode, s.SchoolName, a.AreaCode, a.AreaName, st.ClotheCount, g.StuffName, cc.CycleClotheID } into grp
                    select new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.AreaCode,
                        ClotheCount = grp.Count()
                    };

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        return query.Count();
    }

    public IQueryable LoadSchoolClothes(int maximumRows, int startRowIndex, int cycleClotheId, int areaCode, int schoolCode)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                    join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                    join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                    join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                    join g in db.REP_Stuffs on cc.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    join lv in db.Levels on slv.LevelID equals lv.LevelID
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                               slv.LockOutDate == null && cc.CycleClotheID == cycleClotheId
                    group ssl by new { s.SchoolCode, s.SchoolName, lv.LevelName, a.AreaCode, a.AreaName, st.ClotheCount, g.StuffName, cc.CycleClotheID } into grp
                    orderby grp.Key.SchoolCode
                    select new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.SchoolName,
                        grp.Key.LevelName,
                        grp.Key.AreaCode,
                        grp.Key.AreaName,
                        grp.Key.StuffName,
                        ClotheCount = grp.Count()
                    };

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region ClotheReport.aspx

    public int GetSubLevelClothesCount(int cycleClotheId, int areaCode, int schoolCode, int subLevelId)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                    join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                    join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                    join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                    join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                    join g in db.REP_Stuffs on cc.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             slv.LockOutDate == null && cc.CycleClotheID == cycleClotheId
                    group ssl by new { s.SchoolCode, s.SchoolName, AreaCode = a.AreaCode, a.AreaName, ssl.SchoolSubLevelID, sl.SubLevelName, st.ClotheCount, g.StuffName, cc.CycleClotheID } into grp
                    select new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.AreaCode,
                        grp.Key.SchoolSubLevelID,
                        ClotheCount = grp.Count()
                    };

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        return query.Count();
    }

    public IQueryable LoadSubLevelClothes(int maximumRows, int startRowIndex, int cycleClotheId, int areaCode, int schoolCode, int subLevelId)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                    join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                    join sc in db.SchoolClothes on ssl.SchoolSubLevelID equals sc.SchoolSubLevelID
                    join st in db.StudentClothes on sc.SchoolClotheID equals st.SchoolClotheID
                    join cc in db.CycleClothes on sc.CycleClotheID equals cc.CycleClotheID
                    join g in db.REP_Stuffs on cc.StuffID equals g.StuffID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             slv.LockOutDate == null && cc.CycleClotheID == cycleClotheId
                    group ssl by new { s.SchoolCode, s.SchoolName, AreaCode = a.AreaCode, a.AreaName, ssl.SchoolSubLevelID, sl.SubLevelName, st.ClotheCount, g.StuffName, cc.CycleClotheID } into grp
                    orderby grp.Key.SchoolCode
                    select new
                    {
                        grp.Key.SchoolCode,
                        grp.Key.SchoolName,
                        grp.Key.AreaCode,
                        grp.Key.AreaName,
                        grp.Key.SchoolSubLevelID,
                        grp.Key.SubLevelName,
                        grp.Key.StuffName,
                        ClotheCount = grp.Count()
                    };

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region Inbox.aspx

    public int GetInboxCount(int userInRoleId)
    {
        var query = from msg in db.Messages
                    join umg in db.MessageUsers on msg.MessageID equals umg.MessageID
                    where umg.UserInRoleID == userInRoleId && !umg.Deleted
                    select new
                    {
                        msg.MessageID
                    };

        return query.Count();
    }

    public IQueryable LoadInbox(int maximumRows, int startRowIndex, int userInRoleId)
    {
        var query = from u in db.Users
                    join ur in db.UsersInRoles on u.UserID equals ur.UserID
                    join msg in db.Messages on ur.UserInRoleID equals msg.UserInRoleID
                    join umg in db.MessageUsers on msg.MessageID equals umg.MessageID
                    join sch in db.Schools on ur.SchoolID equals sch.SchoolID into ljsch
                    from js in ljsch.DefaultIfEmpty()
                    join ar in db.Areas on ur.AreaCode equals ar.AreaCode into ljar
                    from ja in ljar.DefaultIfEmpty()
                    where umg.UserInRoleID == userInRoleId && !umg.Deleted
                    orderby msg.SubmitDate
                    select new
                    {
                        msg.MessageID,
                        msg.Subject,
                        msg.Body,
                        msg.SubmitDate,
                        umg.Read,
                        Sender = GetSender(ur.RoleID, ja.AreaName, js.SchoolName)
                    };

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    private string GetSender(short roleId, string area, string school)
    {
        if ((Public.Role)roleId == Public.Role.Administrator)
        {
            return "مدیر سیستم";
        }
        return string.IsNullOrEmpty(school) ? string.Format("مدیر منطقه {0}", area) : string.Format("آموزشگاه {0}", school);
    }

    #endregion

    #region Outbox.aspx

    public int GetOutboxCount(int userInRoleId)
    {
        var query = from msg in db.Messages
                    where msg.UserInRoleID == userInRoleId
                    select new
                    {
                        msg.MessageID
                    };

        return query.Count();
    }

    public IQueryable LoadOutbox(int maximumRows, int startRowIndex, int userInRoleId)
    {
        var query = from msg in db.Messages
                    where msg.UserInRoleID == userInRoleId
                    orderby msg.SubmitDate
                    select new
                    {
                        msg.Subject,
                        msg.Body,
                        msg.SubmitDate
                    };

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region SubLevelsArchive.aspx

    public int GetSubLevelsArchiveCount(DateTime? dateFrom, DateTime? dateTo, int areaCode, int schoolCode, int subLevelId, string gender)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                    join arch in db.SchoolSubLevelsArchives on ssl.SchoolSubLevelID equals arch.SchoolSubLevelID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             slv.LockOutDate == null
                    select new
                    {
                        s.SchoolCode,
                        a.AreaCode,
                        ssl.SchoolSubLevelID,
                        arch.SubmitDate,
                        arch.Gender
                    };

        if (dateFrom != null && dateTo == null)
        {
            query = from q in query
                    where q.SubmitDate == dateFrom
                    select q;
        }
        else if (dateFrom != null && dateTo != null)
        {
            query = from q in query
                    where q.SubmitDate >= dateFrom && q.SubmitDate <= dateTo
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        if (!gender.Equals("B"))
        {
            query = from q in query
                    where q.Gender == gender
                    select q;
        }

        return query.Count();
    }

    public IQueryable LoadSubLevelsArchives(int maximumRows, int startRowIndex, DateTime? dateFrom, DateTime? dateTo, int areaCode, int schoolCode, int subLevelId, string gender)
    {
        var query = from s in db.Schools
                    join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                    join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                    join arch in db.SchoolSubLevelsArchives on ssl.SchoolSubLevelID equals arch.SchoolSubLevelID
                    join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"]) &&
                             slv.LockOutDate == null
                    orderby a.AreaCode, s.SchoolCode
                    select new
                    {
                        s.SchoolCode,
                        s.SchoolName,
                        a.AreaCode,
                        a.AreaName,
                        ssl.SchoolSubLevelID,
                        sl.SubLevelName,
                        arch.SubmitDate,
                        arch.Gender,
                        arch.FormerCount,
                        arch.NextCount
                    };

        if (dateFrom != null && dateTo == null)
        {
            query = from q in query
                    where q.SubmitDate == dateFrom
                    select q;
        }
        else if (dateFrom != null && dateTo != null)
        {
            query = from q in query
                    where q.SubmitDate >= dateFrom && q.SubmitDate <= dateTo
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;

            if (subLevelId > 0)
            {
                query = from q in query
                        where q.SchoolSubLevelID == subLevelId
                        select q;
            }
        }

        if (!gender.Equals("B"))
        {
            query = from q in query
                    where q.Gender == gender
                    select q;
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion

    #region EmployeesArchive.aspx

    public int GetEmployeesArchiveCount(DateTime? dateFrom, DateTime? dateTo, int areaCode, int schoolCode, string employeeType)
    {
        var query = from s in db.Schools
                    join arch in db.SchoolEmployeesArchives on s.SchoolID equals arch.SchoolID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"])
                    select new
                    {
                        s.SchoolCode,
                        a.AreaCode,
                        arch.SubmitDate,
                        arch.EmployeeType
                    };

        if (dateFrom != null && dateTo == null)
        {
            query = from q in query
                    where q.SubmitDate == dateFrom
                    select q;
        }
        else if (dateFrom != null && dateTo != null)
        {
            query = from q in query
                    where q.SubmitDate >= dateFrom && q.SubmitDate <= dateTo
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        if (!employeeType.Equals("B"))
        {
            query = from q in query
                    where q.EmployeeType == employeeType
                    select q;
        }

        return query.Count();
    }

    public IQueryable LoadEmployeesArchives(int maximumRows, int startRowIndex, DateTime? dateFrom, DateTime? dateTo, int areaCode, int schoolCode, string employeeType)
    {
        var query = from s in db.Schools
                    join arch in db.SchoolEmployeesArchives on s.SchoolID equals arch.SchoolID
                    join a in db.Areas on s.AreaCode equals a.AreaCode
                    where a.ProvinceID == int.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultProvinceId"])
                    orderby a.AreaCode, s.SchoolCode
                    select new
                    {
                        s.SchoolCode,
                        s.SchoolName,
                        a.AreaCode,
                        a.AreaName,
                        arch.SubmitDate,
                        arch.EmployeeType,
                        arch.FormerCount,
                        arch.NextCount
                    };

        if (dateFrom != null && dateTo == null)
        {
            query = from q in query
                    where q.SubmitDate == dateFrom
                    select q;
        }
        else if (dateFrom != null && dateTo != null)
        {
            query = from q in query
                    where q.SubmitDate >= dateFrom && q.SubmitDate <= dateTo
                    select q;
        }

        if (areaCode > 0)
        {
            query = from q in query
                    where q.AreaCode == areaCode
                    select q;
        }

        if (schoolCode > 0)
        {
            query = from q in query
                    where q.SchoolCode == schoolCode
                    select q;
        }

        if (!employeeType.Equals("B"))
        {
            query = from q in query
                    where q.EmployeeType == employeeType
                    select q;
        }

        return query.Skip(startRowIndex).Take(maximumRows);
    }

    #endregion
}



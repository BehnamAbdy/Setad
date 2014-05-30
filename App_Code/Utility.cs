using System.Linq;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

/// <summary>
/// Summary description for Utility
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class Utility : System.Web.Services.WebService
{
    SupplySystem.SupplySystem db = null;

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string GetProductsJson(string prefix)
    //{
    //    List<Product> products = new List<Product>();
    //    if (prefix.Trim().Equals(string.Empty, StringComparison.OrdinalIgnoreCase))
    //    {
    //        products = ProductFacade.GetAllProducts();
    //    }
    //    else
    //    {
    //        products = ProductFacade.GetProducts(prefix);
    //    }
    //    //yourobject is your actula object (may be collection) you want to serialize to json
    //    DataContractJsonSerializer serializer = new DataContractJsonSerializer(products.GetType());
    //    //create a memory stream
    //    MemoryStream ms = new MemoryStream();
    //    //serialize the object to memory stream
    //    serializer.WriteObject(ms, products);
    //    //convert the serizlized object to string
    //    string jsonString = Encoding.Default.GetString(ms.ToArray());
    //    //close the memory stream
    //    ms.Close();
    //    return jsonString;
    //}

    [WebMethod]
    public string GetSchool(string sCode, string aCode)
    {
        db = new SupplySystem.SupplySystem(Public.ConnectionString);
        var school = db.Schools.Where(s => s.SchoolCode == Public.ToInt(sCode) && s.AreaCode == Public.ToInt(aCode)).Select(s => new { s.SchoolName });
        foreach (var item in school)
        {
            return item.SchoolName;
        }
        return null;
    }

    [WebMethod]
    public string GetArea(string code)
    {
        db = new SupplySystem.SupplySystem(Public.ConnectionString);
        var area = from ar in db.Areas
                   where ar.AreaCode == Public.ToInt(code)
                   select new { ar.AreaName };

        foreach (var item in area)
        {
            return item.AreaName;
        }
        return null;
    }

    [WebMethod]
    public string GetProvince(string code)
    {
        db = new SupplySystem.SupplySystem(Public.ConnectionString);
        var province = from p in db.Provinces
                       where p.ProvinceID == Public.ToByte(code)
                       select new { p.Name };

        foreach (var item in province)
        {
            return item.Name;
        }
        return null;
    }

    [WebMethod]
    public string GetCity(string pCode, string cCode)
    {
        db = new SupplySystem.SupplySystem(Public.ConnectionString);
        var city = from c in db.Cities
                   where c.ProvinceID == Public.ToByte(pCode) && c.CityID == Public.ToShort(cCode)
                   select new { c.Name };

        foreach (var item in city)
        {
            return item.Name;
        }
        return null;
    }

    [WebMethod]
    public string GetGood(string id, string mode)
    {
        db = new SupplySystem.SupplySystem(Public.ConnectionString);
        SupplySystem.REP_Stuff stuff = db.REP_Stuffs.SingleOrDefault<SupplySystem.REP_Stuff>(st => st.StuffID == Public.ToInt(id) && st.StuffTypeID == Public.ToByte(mode));
        if (stuff != null)
        {
            return string.Concat(stuff.StuffName, "|", stuff.StuffID);
        }
        else
        {
            return null;
        }
    }
}

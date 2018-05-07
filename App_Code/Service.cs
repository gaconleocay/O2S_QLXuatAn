using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Services;

/// <summary>
/// Summary description for Service
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class Service : System.Web.Services.WebService {

    public Service () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld() {
        return "Hello World";
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string[] GetCustomers(string prefix)
    {
        List<string> customers = new List<string>();
        //using (SqlConnection conn = new SqlConnection())
        //{
        //    conn.ConnectionString = ConfigurationManager
        //            .ConnectionStrings["constr"].ConnectionString;
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        cmd.CommandText = "select ContactName, CustomerId from Customers where " +
        //        "ContactName like @SearchText + '%'";
        //        cmd.Parameters.AddWithValue("@SearchText", prefix);
        //        cmd.Connection = conn;
        //        conn.Open();
        //        using (SqlDataReader sdr = cmd.ExecuteReader())
        //        {
        //            while (sdr.Read())
        //            {
        //                customers.Add(string.Format("{0}-{1}", sdr["ContactName"], sdr["CustomerId"]));
        //            }
        //        }
        //        conn.Close();
        //    }
        //    return customers.ToArray();
        //}
        customers.Add("CUong");
        customers.Add("Thoa");
        customers.Add("Linh");
        customers.Add("Tom");
        return customers.ToArray();

    }
    
}

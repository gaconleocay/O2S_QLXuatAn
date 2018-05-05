<%@ WebHandler Language="C#" Class="Search_CS" %>

using System;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Data;

public class Search_CS : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        string prefixText = context.Request.QueryString["q"];
        //using (SqlConnection conn = new SqlConnection())
        //{
        //    conn.ConnectionString = ConfigurationManager
        //            .ConnectionStrings["constr"].ConnectionString;
        //    using (SqlCommand cmd = new SqlCommand())
        //    {
        //        cmd.CommandText = "select * from tblCustomer where " +
        //        "CustomerName like N'%" + prefixText + "%'";
        //        //cmd.Parameters.AddWithValue("@SearchText", prefixText);
        //        cmd.Connection = conn;
        //        StringBuilder sb = new StringBuilder(); 
        //        conn.Open();
        //        using (SqlDataReader sdr = cmd.ExecuteReader())
        //        {
        //            while (sdr.Read())
        //            {
        //                sb.Append(sdr["CustomerName"])
        //                    .Append(Environment.NewLine);
        //            }
        //        }
        //        conn.Close();
        //        context.Response.Write(sb.ToString()); 
        //    }
        //}

        //DataTable dt = StaticPool.mdb.FillData("select * from tblCustomer where CustomerName like N'%" + prefixText + "%'");
        //if (dt != null && dt.Rows.Count > 0)
        //{
        //    string st = "";
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        st = dr["CustomerName"].ToString() + Environment.NewLine;
                     
        //    }

        //    context.Response.Write(st); 
        //} 

        string st = "cuong" + Environment.NewLine +
            "Linh";
        context.Response.Write(st);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}
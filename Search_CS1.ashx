﻿<%@ WebHandler Language="C#" Class="Search_CS" %>

using System;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

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

        string st = "cuong";
        context.Response.Write(st);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }
}
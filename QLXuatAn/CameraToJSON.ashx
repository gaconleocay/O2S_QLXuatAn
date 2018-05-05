<%@ WebHandler Language="C#" Class="CameraToJSON" %>

using System;
using System.Web;
using System.Data;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using System.Web.UI.WebControls;

public class CameraToJSON : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        try
        {
            context.Response.ContentType = "application/json";
            List<ListItem> list = new List<ListItem>();
            string commandString = "";
            if (context.Request.Params["PCID"] != null)
                commandString = "select * from tblCamera where PCID = '" + context.Request.Params["PCID"] + "'";
            else
                commandString = "select * from tblCamera";
            DataTable dt = StaticPool.mdb.FillData(commandString);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ListItem item = new ListItem();
                    item.Value = row["CameraID"].ToString();
                    item.Text = row["CameraName"].ToString() + " (" + row["HttpURL"].ToString() + ")";
                    list.Add(item);
                }
            }
            context.Response.Write(new JavaScriptSerializer().Serialize(list).ToString());
        }
        catch (Exception ex)
        {
            context.Response.Write(ex.Message);
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}
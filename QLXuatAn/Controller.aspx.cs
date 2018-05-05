using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class QLXuatAn_Controller : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                // Check xem nguoi dung nay co quyen truy cap chuc nang nay khong
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Device_Controller", "Selects", "Parking"))
                {
                    string result = "";
                    div_alert.Visible = false;
                    rpt_Controller.DataSource = StaticPool.mdb.FillData("select * from dbo.tblController order by PCID", ref result);
                    rpt_Controller.DataBind();
                    if (result != "")
                    {
                        div_alert.Visible = true;
                        id_alert.InnerText = result;
                    }
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    public string GetComputerName(string PCID)
    {
        try
        {
            DataTable dt = StaticPool.mdb.FillData("select * from dbo.tblPC where PCID = '" + PCID + "'");
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["ComputerName"].ToString() + " (" + dt.Rows[0]["IPAddress"].ToString() + ")";
            else
                return "";
        }
        catch
        {
            return "";
        }
    }

    public string GetStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Ngừng kích hoạt</span>";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Device_Controller", "Deletes", "Parking")==false)       
                return "Bạn không có quyền thực hiện chức năng này!";

            string _controllername = "";
            DataTable temp = StaticPool.mdb.FillData("select ControllerName from tblController where ControllerID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _controllername = temp.Rows[0]["ControllerName"].ToString();
            }
            if (StaticPool.mdb.ExecuteCommand("delete from tblController where ControllerID = '" + id + "'"))
            {              
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Device_Controller", _controllername, "Xóa", "id=" + id);
                return "true";
            }

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }
}
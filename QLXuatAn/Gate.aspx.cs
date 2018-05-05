using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class QLXuatAn_Gate : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Device_Gate", "Selects", "Parking"))
                {
                    rpt_Gate.DataSource = StaticPool.mdb.FillData("select * from tblGate order by SortOrder");
                    rpt_Gate.DataBind();
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
            if (StaticPool.CheckPermission(userid, "Parking_Device_Gate", "Deletes", "Parking"))
            {
                DataTable temp = StaticPool.mdb.FillData("select top 1 GateID from tblPC where GateID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "Cổng đang sử dụng, không xóa được";
                }

                string _gatename = "";
                temp = StaticPool.mdb.FillData("select GateName from tblGate where GateID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    _gatename = temp.Rows[0]["GateName"].ToString();
                }

                if (StaticPool.mdb.ExecuteCommand("delete from tblGate where GateID = '" + id + "'"))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Device_Gate", _gatename, "Xóa", "");
                    return "true";
                }
            }
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }
}
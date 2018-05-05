using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class QLXuatAn_PC : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Device_PC", "Selects", "Parking"))
                {
                    rpt_PC.DataSource = StaticPool.mdb.FillData("select * from tblPC order by GateID, SortOrder");
                    rpt_PC.DataBind();
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

    public string GetGateName(string gateid)
    {
        DataTable dtGate = StaticPool.mdb.FillData("select * from tblGate where GateID = '" + gateid + "'");
        if (dtGate != null && dtGate.Rows.Count > 0)
        {
            return dtGate.Rows[0]["GateName"].ToString();
        }
        return "";
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
            if (StaticPool.CheckPermission(userid, "Parking_Device_PC", "Deletes", "Parking"))
            {
                DataTable temp = StaticPool.mdb.FillData("select PCID from tblCamera where PCID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "PC đang sử dụng, không xóa được";
                }
                temp = StaticPool.mdb.FillData("select PCID from tblController where PCID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "PC đang sử dụng, không xóa được";
                }
                temp = StaticPool.mdb.FillData("select PCID from tblLane where PCID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "PC đang sử dụng, không xóa được";
                }
                temp = StaticPool.mdb.FillData("select PCID from tblLED where PCID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "PC đang sử dụng, không xóa được";
                }

                if (temp == null)
                    return "Failed";

                string _pcname = "";
                temp = StaticPool.mdb.FillData("select ComputerName from tblPC where PCID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    _pcname = temp.Rows[0]["ComputerName"].ToString();
                }
               // return _pcname;
                if (StaticPool.mdb.ExecuteCommand("delete from tblPC where PCID = '" + id + "'"))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Device_PC", _pcname, "Xóa", "id=" + id);
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
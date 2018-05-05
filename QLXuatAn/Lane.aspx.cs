using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using Futech.Tools;
using Futech.Helpers;

public partial class QLXuatAn_Lane : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Device_Lane", "Selects", "Parking"))
                {
                    string result = "";
                    div_alert.Visible = false;
                    rpt_Lane.DataSource = StaticPool.mdb.FillData("select * from dbo.tblLane order by PCID, SortOrder", ref result);
                    rpt_Lane.DataBind();
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

    public string GetLaneTypeName(string lanetype)
    {
        for (int i = 0; i < SystemUI.GetLaneTypes().Length; i++)
        {
            if (i == int.Parse(lanetype))
                return SystemUI.GetLaneTypes()[i];
        }
        return "";
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
            if (StaticPool.CheckPermission(userid, "Parking_Device_Lane", "Deletes", "Parking")==false)
                return "Bạn không có quyền thực hiện chức năng này!";
            DataTable temp = StaticPool.mdbevent.FillData("select top 1 Id from tblCardEvent where LaneIDIn='" + id + "' or LaneIDOut='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Làn đang dùng, không xóa được";
            }
            temp = StaticPool.mdbevent.FillData("select top 1 Id from tblLoopEvent where LaneIDIn='" + id + "' or LaneIDOut='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Làn đang dùng, không xóa được";
            }
            if (temp == null)
                return "Failed";
            string _lanename = "";
            temp = StaticPool.mdb.FillData("select LaneName from tblLane where LaneID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _lanename = temp.Rows[0]["LaneName"].ToString();
            }
            if (StaticPool.mdb.ExecuteCommand("delete from tblLane where LaneID = '" + id + "'"))
            {
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Device_Lane", _lanename, "Xóa", "id=" + id);
                CacheLayer.Clear(StaticCached.c_tblLane);
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
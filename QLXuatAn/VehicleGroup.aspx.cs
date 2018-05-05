using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using Futech.Tools;

public partial class QLXuatAn_VehicleGroup : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_List_VehicleGroup", "Selects", "Parking"))
                {
                    rpt_VehicleGroup.DataSource = StaticPool.mdb.FillData("select * from tblVehicleGroup");
                    rpt_VehicleGroup.DataBind();
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

    public string GetVehicleTypeName(string vehicletype)
    {
        for (int i = 0; i < SystemUI.GetVehicleTypes().Length; i++)
        {
            if (i == int.Parse(vehicletype))
                return SystemUI.GetVehicleTypes()[i];
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
            if (StaticPool.CheckPermission(userid, "Parking_List_CustomerGroup", "Deletes", "Parking")==false)
                return "Bạn không có quyền thực hiện chức năng này!";

            DataTable temp = StaticPool.mdb.FillData("select top 1 VehicleGroupID from tblCardGroup where VehicleGroupID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Nhóm xe đang sử dụng, không xóa được";
            }

            if (temp == null)
                return "Failed";

            string _vehiclegroupname = "";
            temp = StaticPool.mdb.FillData("select VehicleGroupName from tblVehicleGroup where VehicleGroupID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _vehiclegroupname = temp.Rows[0]["VehicleGroupName"].ToString();
            }
            if(StaticPool.mdb.ExecuteCommand("delete from tblVehicleGroup where VehicleGroupID = '" + id + "'"))
            {
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_List_VehicleGroup", _vehiclegroupname, "Xóa", "id=" + id);
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
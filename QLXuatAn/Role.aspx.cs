using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class QLXuatAn_Role : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_System_Role", "Selects", "Parking"))
                {
                    rpt_Role.DataSource = StaticPool.mdb.FillData("select * from tblRole where AppCode = 'Parking' order by SortOrder");
                    rpt_Role.DataBind();
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Message.aspx?Message=" + ex.Message);
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
            if (StaticPool.CheckPermission(userid, "Parking_System_Role", "Deletes", "Parking"))
            {
                DataTable dtRole = StaticPool.mdb.FillData("select * from tblRole where RoleID = '" + id + "' and IsSystem = 1");
                if (dtRole != null && dtRole.Rows.Count > 0)
                    return "Bạn không thể xóa được vai trò của hệ thống!";

                DataTable temp = StaticPool.mdb.FillData("select RoleID from tblUserJoinRole where RoleID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "Vai trò đang được sử dụng, không xóa được";
                }

                string _rolename = "";
                temp = StaticPool.mdb.FillData("select RoleName from tblRole where RoleID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    _rolename = temp.Rows[0]["RoleName"].ToString();
                }

                if (StaticPool.mdb.ExecuteCommand("delete from tblRole where RoleID = '" + id + "'"))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_System_Role", _rolename, "Xóa", "id=" + id);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class accesscontrol_User : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "AccessControl_System_User", "Selects", "AccessControl"))
                {
                    rpt_User.DataSource = StaticPool.mdb.FillData("select * from tblUser order by SortOrder");
                    rpt_User.DataBind();
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
            return "<span class='label label-sm label-warning'>Đã khóa</span>";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + id + "' and IsSystem = 1");
            if (dtUser != null && dtUser.Rows.Count > 0)
                return "Bạn không thể xóa được người dùng của hệ thống!";

            if (StaticPool.CheckPermission(userid, "AccessControl_System_User", "Deletes", "AccessControl"))
                return StaticPool.mdb.ExecuteCommand("delete from tblUser where UserID = '" + id + "'").ToString().ToLower();
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
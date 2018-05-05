using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using Futech.Helpers;

public partial class QLXuatAn_User : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_System_User", "Selects", "Parking"))
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

            if (StaticPool.CheckPermission(userid, "Parking_System_User", "Deletes", "Parking")==false)
                return "Bạn không có quyền thực hiện chức năng này!";

            DataTable temp = StaticPool.mdb.FillData("select top 1 UserID from tblActiveCard where UserID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Người dùng đang được sử dụng, không xóa được";
            }

            temp = StaticPool.mdb.FillData("select top 1 UserID from tblCardProcess where UserID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Người dùng đang được sử dụng, không xóa được";
            }

            temp = StaticPool.mdbevent.FillData("select top 1 Id from tblCardEvent where UserIDIn ='" + id + "' or UserIDOut='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Người dùng đang được sử dụng, không xóa được";
            }

            temp = StaticPool.mdbevent.FillData("select top 1 Id from tblLoopEvent where UserIDIn ='" + id + "' or UserIDOut='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Người dùng đang được sử dụng, không xóa được";
            }

            

            if (temp == null)
                return "Failed";

            string _username = "";
            temp = StaticPool.mdb.FillData("select UserName from tblUser where UserID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _username = temp.Rows[0]["UserName"].ToString();
            }

            if (StaticPool.mdb.ExecuteCommand("delete from tblUser where UserID = '" + id + "'"))
            {
                StaticPool.mdb.ExecuteCommand("delete from tblUserJoinRole where UserID='" + id + "'");
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_System_User", _username, "Xóa", "id=" + id);
                CacheLayer.Clear(StaticCached.c_tblUser);
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
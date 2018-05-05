using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using Futech.Helpers;

public partial class QLXuatAn_CardType : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_List_CardGroup", "Selects", "Parking"))
                {
                    rpt_CardGroup.DataSource = StaticPool.mdb.FillData("select * from tblCardGroup order by SortOrder");
                    rpt_CardGroup.DataBind();
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
            if (StaticPool.CheckPermission(userid, "Parking_List_CardGroup", "Deletes", "Parking")==false)
                return "Bạn không có quyền thực hiện chức năng này!";
            DataTable temp = StaticPool.mdb.FillData("select top 1 CardID from tblCard where CardGroupID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Nhóm thẻ đang dùng, không xóa được";
            }
            temp = StaticPool.mdbevent.FillData("select top 1 Id from tblCardEvent where CardGroupID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Nhóm thẻ đang dùng, không xóa được";
            }
           

            if (temp == null)
                return "Failed";

            string _cardgroupname="";
            temp = StaticPool.mdb.FillData("select CardGroupName from tblCardGroup where CardGroupID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _cardgroupname = temp.Rows[0]["CardGroupName"].ToString();
            }

            if (StaticPool.mdb.ExecuteCommand("delete from tblCardGroup where CardGroupID = '" + id + "'"))
            {
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_List_CardGroup", _cardgroupname, "Xóa", "id=" + id);
                CacheLayer.Clear(StaticCached.c_tblCardGroup);
                return "true";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }

    public string formatMoney(string money)
    {
        if (double.Parse(money) == 0)
            return "";
        else
            return String.Format("{0:000,0}", Convert.ToDouble(money));
    }

}
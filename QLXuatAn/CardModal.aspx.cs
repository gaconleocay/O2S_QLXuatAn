using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class QLXuatAn_CardModal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ViewState["CustomerID"] = "";
                div_alert.Visible = false;
                id_alert.InnerText = "";

                if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Card", "Updates", "Parking"))
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();

                if (Request.QueryString["CustomerID"] != null)
                {
                    ViewState["CustomerID"] = Request.QueryString["CustomerID"].ToString();
                    rpt_Card.DataSource = StaticPool.mdb.FillData("select ca.CardNo, ca.CardNumber, g.CardGroupName, ca.ExpireDate, ca.IsLock, ca.CardID, ca.Plate1 from tblCard ca inner join tblCardgroup g on ca.CardGroupID = CONVERT(varchar(255), g.CardGroupID) where ca.CustomerID = '" + ViewState["CustomerID"].ToString() + "' order by ca.SortOrder");
                    rpt_Card.DataBind();
                }
            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }
        }
    }

    //public string GetCardGroup(string CardGroupID)
    //{
    //    DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
    //    if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
    //        return dtCardGroup.Rows[0]["CardGroupName"].ToString();
    //    else
    //        return "";
    //}

    public string GetExpireDate(string ExpireDate)
    {
        if (ExpireDate == "")
            return "";
        else
        {
            if (DateTime.Parse(ExpireDate) < DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")))
                return "<span class='label label-sm label-danger'>" + DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy") + "</span>";
            else if (DateTime.Parse(ExpireDate) == DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")))
                return "<span class='label label-sm label-warning'>" + DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy") + "</span>";
            else
                return DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy");
        }
    }

    public string GetCardStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Đã khóa thẻ</span>";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Card", "Deletes", "Parking"))
            {
                string _cardnumber = "";
                string _cardgroupid = "";
                DataTable temp = StaticPool.mdb.FillData("select CardNumber, CardGroupID from tblCard where CardID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    _cardnumber = temp.Rows[0]["CardNumber"].ToString();
                    _cardgroupid = temp.Rows[0]["CardGroupID"].ToString();
                }

                temp = StaticPool.mdbevent.FillData("select top 1 CardNumber from tblCardEvent where CardNumber='" + _cardnumber + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "Thẻ đang sử dụng, không xóa được";
                }

                temp = StaticPool.mdb.FillData("select top 1 CardNumber from tblActiveCard where CardNumber='" + _cardnumber + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "Thẻ đang sử dụng, không xóa được";
                }

                if (StaticPool.mdb.ExecuteCommand("update tblCard set IsDelete=1 where CardID = '" + id + "'"))
                {

                    DeleteInvalidCardInLogCardCustomer(id);
                    //delete card
                    StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID) values('" +
                      DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                      _cardnumber + "', '" +
                      "DELETE" + "', '" +
                      _cardgroupid + "', '" +
                      userid +
                      "')");
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Card", _cardnumber, "Xóa", "id=" + id);
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

    private static void DeleteInvalidCardInLogCardCustomer(string id)
    {
        string getCard = string.Format("select * from tblCard where CardID = '{0}'", id);
        DataTable tb = StaticPool.mdb.FillData(getCard);

        if (tb != null && tb.Rows.Count > 0)
        {
            string command = string.Format("delete from tblLogCardCustomer where CardNumber = '{0}'", tb.Rows[0]["CardNumber"].ToString());
            StaticPool.mdb.ExecuteCommand(command);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class accesscontrol_CardModal : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;
                id_alert.InnerText = "";

                if (Request.QueryString["CustomerID"] != null)
                {
                    ViewState["CustomerID"] = Request.QueryString["CustomerID"].ToString();
                    rpt_Card.DataSource = StaticPool.mdb.FillData("select * from tblCard where CustomerID = '" + ViewState["CustomerID"].ToString() + "' order by SortOrder");
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

    public string GetCardGroup(string CardGroupID)
    {
        DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            return dtCardGroup.Rows[0]["CardGroupName"].ToString();
        else
            return "";
    }

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
    public static string Delete(string id)
    {
        try
        {
            return StaticPool.mdb.ExecuteCommand("delete from tblCard where CardID = '" + id + "'").ToString().ToLower();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
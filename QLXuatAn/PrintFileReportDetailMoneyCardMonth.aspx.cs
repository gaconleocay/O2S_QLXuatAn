using Futech.Helpers;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportDetailMoneyCardMonth : System.Web.UI.Page
{
    int findex, lindex;

    string KeyWord = "", CardGroupID = "", CustomerID = "", CustomerGroupID = "", UserID = "";
    string FromDate = "", ToDate = "";
    DataTable dtCus = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        BindDataList();
    }

    public string GetCardGroup(string CardGroupID)
    {
        if (string.IsNullOrWhiteSpace(CardGroupID))
        {
            return "";
        }

        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
        {
            var rRow = dtCardGroup.Select(string.Format("CardGroupID = '{0}'", CardGroupID));
            if (rRow.Any())
            {
                return rRow[0]["CardGroupName"].ToString();
            }
            return "";
        }
        else
        {
            return "";
        }
    }

    public string GetUserName(string userid)
    {
        if (string.IsNullOrWhiteSpace(userid))
        {
            return "";
        }

        DataTable dt = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        var gName = "";
        if (dt != null)
        {
            var rRow = dt.Select(string.Format("UserID = '{0}'", userid));
            if (rRow.Any())
            {
                gName = rRow[0]["UserName"].ToString();
            }
        }
        return gName;
    }

    public string GetCustomerName(string customerid)
    {
        var gName = "";
        if (dtCus != null)
        {
            var rRow = dtCus.Select(string.Format("CustomerID = '{0}'", customerid));
            if (rRow.Any())
            {
                gName = rRow[0]["CustomerName"].ToString();
            }
        }
        return gName;
    }

    public string GetDateTime(string dtime)
    {
        if (dtime != "")
        {
            return DateTime.Parse(dtime).ToString("dd/MM/yyyy HH:mm:ss");
        }
        return "";
    }

    private void BindDataList()
    {
        try
        {
            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
            }

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"].ToString();
            }

            if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
            {
                CustomerID = Request.QueryString["CustomerID"].ToString();
            }

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
            }

            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
            }

            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"].ToString() != "")
            {
                ToDate = Request.QueryString["ToDate"].ToString();
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
            }

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd HH:mm:00"); //FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2);
            string _todate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd HH:mm:59"); //ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + "23:59:59";
            DataTable dtcard = ReportService.GetReportDetailMoneyCardMonthFile(KeyWord, _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, UserID);

            if (dtcard != null && dtcard.Rows.Count > 0)
            {
                long _totalmoneys = 0;
                foreach (DataRow dr in dtcard.Rows)
                {
                    _totalmoneys = _totalmoneys + long.Parse(dr["FeeLevel"].ToString());
                }

                dtcard.Rows.Add(null, null, "TỔNG SỐ", null, null, null, null, null, null, _totalmoneys, null);

                var listCusId = "";
                foreach (DataRow item in dtcard.Rows)
                {
                    listCusId += item["CustomerID"].ToString() + ",";
                }
                dtCus = ReportService.GetCustomerBylistId(listCusId);
            }

            

            if (dtcard == null)
                return;

            if (dtcard != null && dtcard.Rows.Count > 0)
            {
                //Bind resulted PageSource into the Repeater
                rpt_Card.DataSource = dtcard;
                rpt_Card.DataBind();
            }

        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}
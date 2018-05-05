using Futech.Helpers;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportDetailMoneyCardDay : System.Web.UI.Page
{
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
    bool IsVehicleIn = true;
    string LaneID = "", UserID = "";

    DataTable dtCus = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        //cardgroup
        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup == null)
        {
            dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            if (dtCardGroup!=null && dtCardGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
        }

        //lane
        DataTable dtlane = CacheLayer.Get<DataTable>(StaticCached.c_tblLane);
        if (dtlane == null)
        {
            dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane order by SortOrder");
            if (dtlane!=null && dtlane.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblLane, dtlane, StaticCached.TimeCache);
        }

        //user
        DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        if (dtuser == null)
        {
            dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
            if (dtuser!=null && dtuser.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
        }

        BindDataList();
    }
    public string GetCardGroup(string CardGroupID)
    {
        if (string.IsNullOrWhiteSpace(CardGroupID))
        {
            return "";
        }

        if (CardGroupID == "LOOP_D")
            return "Vòng từ-Xe lượt";
        else if (CardGroupID == "LOOP_M")
            return "Vòng từ-Xe tháng";
        //DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        var gName = "";
        if (dtCardGroup != null)
        {
            var rRow = dtCardGroup.Select(string.Format("CardGroupID = '{0}'", CardGroupID));
            if (rRow.Any())
            {
                gName = rRow[0]["CardGroupName"].ToString();
            }
        }
        return gName;
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

    //public string GetCustomerName(string customerid)
    //{
    //    var gName = "";
    //    if (dtCus != null)
    //    {
    //        var rRow = dtCus.Select(string.Format("CustomerID = '{0}'", customerid));
    //        if (rRow.Any())
    //        {
    //            gName = rRow[0]["CustomerName"].ToString();
    //        }
    //    }
    //    return gName;
    //}

    public string GetDateTime(string dtime)
    {
        if (dtime != "")
        {
            return DateTime.Parse(dtime).ToString("dd/MM/yyyy HH:mm:ss");
        }
        return "";
    }

    public string GetLane(string laneid)
    {
        if (string.IsNullOrWhiteSpace(laneid))
        {
            return "";
        }

        DataTable dt = CacheLayer.Get<DataTable>(StaticCached.c_tblLane);
        var gName = "";
        if (dt != null)
        {
            var rRow = dt.Select(string.Format("LaneID = '{0}'", laneid));
            if (rRow.Any())
            {
                gName = rRow[0]["LaneName"].ToString();
            }
        }
        return gName;
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
            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
            }
            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"].ToString() != "")
            {
                ToDate = Request.QueryString["ToDate"].ToString();
            }

            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"].ToString() != "")
            {
                LaneID = Request.QueryString["LaneID"].ToString();
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
            }

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd HH:mm:00"); //FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2) + " " + FromDate.Substring(11, 5);
            string _todate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd HH:mm:59"); //ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + ToDate.Substring(11, 5) + ":59";

            long _totalmoneys = 0;
            DataTable dt = ReportService.GetReportDetailMoneyCardDayFile(KeyWord, UserID, _fromdate, _todate, CardGroupID, LaneID);

            //DataTable dtevent = null;
            //string st = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string _dtimein = dr["DatetimeIn"].ToString();
                    string _dtimeout = dr["DatetimeOut"].ToString();

                    dr["TotalTimes"] = StaticPool.GetPeriodTime(DateTime.Parse(_dtimein), DateTime.Parse(_dtimeout)).ToString();

                    string _moneys = dr["Moneys"].ToString();
                    _totalmoneys = _totalmoneys + long.Parse(_moneys);

                    dr["LaneIDIn"] = GetLane(dr["LaneIDIn"].ToString());
                    dr["LaneIDOut"] = GetLane(dr["LaneIDOut"].ToString());
                    dr["CardGroupID"] = GetCardGroup(dr["CardGroupID"].ToString());
                    dr["UserIDIn"] = GetUserName(dr["UserIDIn"].ToString());
                    dr["UserIDOut"] = GetUserName(dr["UserIDOut"].ToString());
                }

                //var listCusId = "";
                //foreach (DataRow item in dt.Rows)
                //{
                //    listCusId += item["CustomerID"].ToString() + ",";
                //}
                //dtCus = ReportService.GetCustomerBylistId(listCusId);
            }

            dt.Rows.Add(0, "#", "TỔNG SỐ", "", null, null, "", "", "", "", "", "", _totalmoneys, "");

            if (dt != null && dt.Rows.Count > 0)
            {
                rpt_Card.DataSource = dt;
                rpt_Card.DataBind();
            }

        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}
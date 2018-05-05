using Futech.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportTotalMoneyByLane : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", LaneID = "";
    string FromDate = "", ToDate = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        //lane
        DataTable dtlane = CacheLayer.Get<DataTable>(StaticCached.c_tblLane);
        if (dtlane == null)
        {
            dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane WITH (NOLOCK) order by SortOrder");
            if (dtlane!=null && dtlane.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblLane, dtlane, StaticCached.TimeCache);
        }

        BindDataList();
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
            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"].ToString() != "")
            {
                LaneID = Request.QueryString["LaneID"].ToString();
            }


            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
            }

            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"].ToString() != "")
            {
                ToDate = Request.QueryString["ToDate"].ToString();
            }



            DataTable dt = new DataTable();
            dt.Columns.Add("LaneName", typeof(string));
            dt.Columns.Add("Moneys", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2) + " " + FromDate.Substring(11, 5);
            string _todate = ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + ToDate.Substring(11, 5) + ":59";

            DataTable dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane WITH (NOLOCK) where LaneID LIKE '%" + LaneID + "%' order by SortOrder");

            long _totalmoneys = 0;
            if (dtlane != null && dtlane.Rows.Count > 0)
            {

                DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
                if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
                {
                    foreach (DataRow dr in dtlane.Rows)
                    {
                        long _moneys = 0;

                        DataTable temp =
                            ReportService.GetReportTotalMoneyByLaneUnion(dr["LaneID"].ToString(), _fromdate, _todate);
                        if (temp != null && temp.Rows.Count > 0)
                        {
                            if (temp.Rows[0][0].ToString() != "")
                            {
                                _moneys = long.Parse(temp.Rows[0][0].ToString());
                                _totalmoneys = _totalmoneys + _moneys;
                            }
                        }
                        dt.Rows.Add(GetLane(dr["LaneID"].ToString()),
                            string.Format(new System.Globalization.CultureInfo("en-US"), "{0:0,0}", _moneys));
                    }
                }
                else
                {
                    
                    var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
                    if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
                    {
                        foreach (DataRow dr in dtlane.Rows)
                        {
                            long _moneys = 0;
                            // truong hop bang loop co du lieu lay union ca 2 bang
                            DataTable temp =
                                ReportService.GetReportTotalMoneyByLaneAndLoop(dr["LaneID"].ToString(), _fromdate, _todate);
                            if (temp != null && temp.Rows.Count > 0)
                            {
                                if (temp.Rows[0][0].ToString() != "")
                                {
                                    _moneys = long.Parse(temp.Rows[0][0].ToString());
                                    _totalmoneys = _totalmoneys + _moneys;
                                }
                            }
                            dt.Rows.Add(GetLane(dr["LaneID"].ToString()), string.Format(new System.Globalization.CultureInfo("en-US"), "{0:0,0}", _moneys));
                        }
                    }
                    else // truong hop bang loop khong co du lieu, thi lau moi bang tblCardEvent thoi
                    {
                        foreach (DataRow dr in dtlane.Rows)
                        {
                            long _moneys = 0;

                            DataTable temp =
                                ReportService.GetReportTotalMoneyByLane(dr["LaneID"].ToString(), _fromdate, _todate);
                            if (temp != null && temp.Rows.Count > 0)
                            {
                                if (temp.Rows[0][0].ToString() != "")
                                {
                                    _moneys = long.Parse(temp.Rows[0][0].ToString());
                                    _totalmoneys = _totalmoneys + _moneys;
                                }
                            }

                            dt.Rows.Add(GetLane(dr["LaneID"].ToString()),
                                string.Format(new System.Globalization.CultureInfo("en-US"), "{0:0,0}", _moneys));

                        }
                    }
                }

            }


            dt.Rows.Add("TỔNG SỐ", string.Format(new System.Globalization.CultureInfo("en-US"), "{0:0,0}", _totalmoneys));

            if (dt == null)
                return;


            if (dt != null && dt.Rows.Count > 0)
            {
                //Bind resulted PageSource into the Repeater
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
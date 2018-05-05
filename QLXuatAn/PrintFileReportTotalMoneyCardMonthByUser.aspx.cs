using Futech.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportTotalMoneyCardMonthByUser : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", UserID = "";
    string FromDate = "", ToDate = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        //user
        DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        if (dtuser == null)
        {
            dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser WITH (NOLOCK) where IsLock=0 order by SortOrder");
            if (dtuser!=null && dtuser.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
        }

        BindDataList();
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

    private void BindDataList()
    {
        try
        {
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
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
            dt.Columns.Add("UserName", typeof(string));
            dt.Columns.Add("Moneys", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2) + " " + FromDate.Substring(11, 5);
            string _todate = ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + ToDate.Substring(11, 5) + ":59";

            DataTable dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser WITH (NOLOCK) where IsLock=0 and UserID LIKE '%" + UserID + "%' order by SortOrder");

            long _totalmoneys = 0;
            if (dtuser != null && dtuser.Rows.Count > 0)
            {

                foreach (DataRow dr in dtuser.Rows)
                {

                    long _moneys = 0;
                    DataTable temp = StaticPool.mdb.FillData("select sum(FeeLevel) from tblActiveCard WITH (NOLOCK) where IsDelete=0" +
                        " and Date>='" + _fromdate +
                        "' and Date<='" + _todate +
                        "' and UserID='" + dr["UserID"].ToString() + "'");
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        if (temp.Rows[0][0].ToString() != "")
                        {
                            _moneys = long.Parse(temp.Rows[0][0].ToString());
                            _totalmoneys = _totalmoneys + _moneys;
                        }
                    }

                    dt.Rows.Add(dr["UserName"].ToString(), string.Format(new System.Globalization.CultureInfo("en-US"),"{0:0,0}", _moneys));

                }

            }


            dt.Rows.Add("TỔNG SỐ", string.Format(new System.Globalization.CultureInfo("en-US"),"{0:0,0}", _totalmoneys));

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
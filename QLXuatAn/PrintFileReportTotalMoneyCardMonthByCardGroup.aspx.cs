using Futech.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportTotalMoneyCardMonthByCardGroup : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        //cardgroup
        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup == null)
        {
            dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup WITH (NOLOCK) order by SortOrder");
            if (dtCardGroup!=null && dtCardGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
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

    private void BindDataList()
    {
        try
        {
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



            DataTable dt = new DataTable();
            dt.Columns.Add("CardGroupName", typeof(string));
            dt.Columns.Add("Moneys", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2) + " " + FromDate.Substring(11, 5);
            string _todate = ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + ToDate.Substring(11, 5) + ":59";

            DataTable dtcardgroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup WITH (NOLOCK) where CardType=0 and CardGroupID LIKE '%" + CardGroupID + "%' order by SortOrder");

            long _totalmoneys = 0;
            if (dtcardgroup != null && dtcardgroup.Rows.Count > 0)
            {

                foreach (DataRow dr in dtcardgroup.Rows)
                {

                    long _moneys = 0;
                    DataTable temp = StaticPool.mdb.FillData("select sum(FeeLevel) from tblActiveCard WITH (NOLOCK) where IsDelete=0" +
                        " and Date>='" + _fromdate +
                        "' and Date<='" + _todate +
                        "' and CardGroupID='" + dr["CardGroupID"].ToString() + "'");
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        if (temp.Rows[0][0].ToString() != "")
                        {
                            _moneys = long.Parse(temp.Rows[0][0].ToString());
                            _totalmoneys = _totalmoneys + _moneys;
                        }
                    }

                    dt.Rows.Add(GetCardGroup(dr["CardGroupID"].ToString()), string.Format(new System.Globalization.CultureInfo("en-US"),"{0:0,0}", _moneys));

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
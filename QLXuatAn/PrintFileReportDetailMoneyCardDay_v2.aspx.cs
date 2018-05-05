using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportDetailMoneyCardDay_v2 : System.Web.UI.Page
{
    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
    bool IsVehicleIn = true;
    string LaneID = "", UserID = "";
    static int pageIndex = 1;

    private int ps = StaticPool.pageLevelPrint * StaticPool.pagesize;

    //int pageLevel = StaticPool.pageLevelPrint;
    DataTable dtCus = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"]);
            }

            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"] != "")
            {
                KeyWord = Request.QueryString["KeyWord"];
            }

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"] != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"];
            }
            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"] != "")
            {
                FromDate = Request.QueryString["FromDate"];
            }
            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"] != "")
            {
                ToDate = Request.QueryString["ToDate"];
            }

            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"] != "")
            {
                LaneID = Request.QueryString["LaneID"];
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"] != "")
            {
                UserID = Request.QueryString["UserID"];
            }

            // PRINTCONTROLLER: filter on box Printer --------------------------------------------------------------
            var chkFilter = "0";
            int bRow = 0;
            int eRow = 0;
            ViewState["pageSize"] = ps;
            if (Request.QueryString["chkFilter"] != null && Request.QueryString["chkFilter"] != "")
            {
                chkFilter = Request.QueryString["chkFilter"];
            }
            if (Request.QueryString["bRow"] != null && Request.QueryString["bRow"] != "")
            {
                bRow = Convert.ToInt32(Request.QueryString["bRow"]);
            }
            if (Request.QueryString["eRow"] != null && Request.QueryString["eRow"] != "")
            {
                eRow = Convert.ToInt32(Request.QueryString["eRow"]);
            }


            //=======================================================================================

            string _fromdate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd HH:mm:00");
            string _todate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd HH:mm:59");

            long _totalmoneys = 0;
            var totalCount = 0;
            var dt = new DataTable();
            if (chkFilter == "0")
            {
                dt = ReportService.GetReportDetailMoneyCardDay_2(KeyWord, UserID, _fromdate, _todate,
                    CardGroupID,
                    LaneID, pageIndex, ps, ref totalCount, ref _totalmoneys);
            }
            else
            {
                //Khong lay nhieu hơn ps(500 ban ghi)
                if ((eRow - bRow) > ps)
                {
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "Lỗi", "alert('Số bản ghi phải nhỏ hơn hoặc bằng " + ps + "');", true);
                    //Response.Write("<script>alert('Số bản ghi phải nhỏ hơn hoặc bằng " + ps + "');</script>");
                }
                else
                {
                    // PRINTCONTROLLER: 
                    dt = ReportService.GetReportDetailMoneyCardDay_CheckRecord(KeyWord, UserID, _fromdate, _todate, CardGroupID,
                        LaneID, bRow, eRow, ref totalCount, ref _totalmoneys);
                }

            }


            //DataTable dtevent = null;
            //string st = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string _dtimein = dr["DatetimeIn"].ToString();
                    string _dtimeout = dr["DatetimeOut"].ToString();

                    dr["TotalTimes"] = StaticPool.GetPeriodTime(DateTime.Parse(_dtimein), DateTime.Parse(_dtimeout));

                    //string _moneys = dr["Moneys"].ToString();
                    //_totalmoneys = _totalmoneys + long.Parse(_moneys);

                    //dr["LaneIDIn"] = StaticPool.GetLane(dr["LaneIDIn"].ToString());
                    //dr["LaneIDOut"] = StaticPool.GetLane(dr["LaneIDOut"].ToString());
                    //dr["CardGroupID"] = StaticPool.GetCardGroup(dr["CardGroupID"].ToString());
                    //dr["UserIDIn"] = StaticPool.GetUserName(dr["UserIDIn"].ToString());
                    //dr["UserIDOut"] = StaticPool.GetUserName(dr["UserIDOut"].ToString());
                }

                //var listCusId = "";
                //foreach (DataRow item in dt.Rows)
                //{
                //    listCusId += item["CustomerID"].ToString() + ",";
                //}
                //dtCus = ReportService.GetCustomerBylistId(listCusId);
            }

            // PRINTCONTROLLER:  ---------------------------------------------------------------------------
            var pageCount = totalCount / ps;
            if (totalCount % ps != 0)
            {
                pageCount = pageCount + 1;
            }
            //hiển thị tổng tiền ở cuối trang
            if (pageIndex == pageCount)
                dt.Rows.Add(0, "#", "TỔNG SỐ", "", null, null, "", "", "", "", "", "", _totalmoneys, "");
            //-----------------------------------------------------------------------------------------------

            if (dt != null && dt.Rows.Count > 0)
            {
                // PRINTCONTROLLER:  -----------------
                PrinterControl.PageIndex = pageIndex;
                PrinterControl.PageNext = ps;
                PrinterControl.TotalItem = totalCount;
                //------------------------------------
                rpt_Card.DataSource = dt;
                rpt_Card.DataBind();
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
        }
    }


}
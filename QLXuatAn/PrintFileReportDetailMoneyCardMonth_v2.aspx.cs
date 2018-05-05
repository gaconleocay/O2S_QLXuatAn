using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileReportDetailMoneyCardMonth_v2 : System.Web.UI.Page
{
    string KeyWord = "", CardGroupID = "", CustomerID = "", CustomerGroupID = "", UserID = "";
    string FromDate = "", ToDate = "";
    DataTable dtCus = null;
    private int ps = StaticPool.pageLevelPrint * StaticPool.pagesize;
    static int pageIndex = 1;


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
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"]);
            }
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

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd HH:mm:00"); //FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2);
            string _todate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd HH:mm:59"); //ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + "23:59:59";

            long _totalmoneys = 0;
            var totalCount = 0;
            var dtcard = new DataTable();
            if (chkFilter == "0")
            {
                dtcard = ReportService.GetReportDetailMoneyCardMonth(KeyWord, _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, UserID, pageIndex, ps, ref totalCount, ref _totalmoneys);
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
                    dtcard = ReportService.GetReportDetailMoneyCardMonth_CheckRow(KeyWord, _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, UserID, pageIndex, ps, bRow, eRow, ref totalCount, ref _totalmoneys);
                }

            }
            if (dtcard != null && dtcard.Rows.Count > 0)
            {
                var listCusId = "";
                foreach (DataRow item in dtcard.Rows)
                {
                    listCusId += item["CustomerID"].ToString() + ",";
                }
                dtCus = ReportService.GetCustomerBylistId(listCusId);
            }


            // PRINTCONTROLLER:  ---------------------------------------------------------------------------
            var pageCount = totalCount / ps;
            if (totalCount % ps != 0)
            {
                pageCount = pageCount + 1;
            }
            //hiển thị tổng tiền ở cuối trang
            if (pageIndex == pageCount)
                dtcard.Rows.Add(null, null, "TỔNG SỐ", null, null, null, null, null, null, _totalmoneys, null);


            if (dtcard != null && dtcard.Rows.Count > 0)
            {
                // PRINTCONTROLLER:  -----------------
                PrinterControl.PageIndex = pageIndex;
                PrinterControl.PageNext = ps;
                PrinterControl.TotalItem = totalCount;
                //------------------------------------

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
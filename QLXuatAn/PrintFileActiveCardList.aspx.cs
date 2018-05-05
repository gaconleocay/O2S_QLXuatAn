using Futech.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_PrintFileActiveCardList : System.Web.UI.Page
{
    int findex, lindex;

    string KeyWord = "", CardGroupID = "", CustomerID = "", CustomerGroupID = "";
    string FromDate = "", ToDate = "";
    int pageIndex = 1;

    private int ps = StaticPool.pageLevelPrint10 * StaticPool.pagesize;
    protected void Page_Load(object sender, EventArgs e)
    {
        BindDataList();
    }

    public string GetCardGroup(string CardGroupID)
    {
        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup == null)
        {
            dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            if (dtCardGroup!=null && dtCardGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
        }
        //DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            return dtCardGroup.Rows[0]["CardGroupName"].ToString();
        else
            return "";
    }

    public string GetUserName(string userid)
    {
        DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        if (dtuser == null)
        {
            dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
            if (dtuser!=null && dtuser.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
        }
        //DataTable dt = StaticPool.mdb.FillData("select UserName from tblUser where UserID='" + userid + "'");
        if (dtuser != null && dtuser.Rows.Count > 0)
            return dtuser.Rows[0]["UserName"].ToString();
        return "";
    }

    //public string GetCustomerName(string customerid)
    //{
    //    DataTable dt = StaticPool.mdb.FillData("select CustomerName from tblCustomer where CustomerID='" + customerid + "'");
    //    if (dt != null && dt.Rows.Count > 0)
    //        return dt.Rows[0]["CustomerName"].ToString();

    //    return "";
    //}

    private void BindDataList()
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


            DataTable dtCard = null;
            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(FromDate).ToString("yyyy/MM/dd"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2);
            string _todate = Convert.ToDateTime(ToDate).AddDays(1).ToString("yyyy/MM/dd"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + "23:59:59";
            // fix tạm max 500 bản ghi
            var totalCount = 0;
            if (chkFilter == "0")
            {
                dtCard = ReportService.GetActiveCardList_2Print(KeyWord, "", _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, pageIndex, ps, ref totalCount);
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
                    dtCard = ReportService.GetActiveCardList_2PrintCheckRow(KeyWord, "", _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, bRow, eRow, ref totalCount);
                }

            }
            

            //DataTable dtCard = null;
            ////ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            //string _fromdate = FromDate.Substring(6, 4) + "/" + FromDate.Substring(3, 2) + "/" + FromDate.Substring(0, 2);
            //string _todate = ToDate.Substring(6, 4) + "/" + ToDate.Substring(3, 2) + "/" + ToDate.Substring(0, 2) + " " + "23:59:59";
            //dtCard = StaticPool.mdb.FillData("select ID,[Date],[CardNumber],[Plate],[OldExpireDate],[Days],[NewExpireDate],[CardGroupID],[CustomerGroupID],[CustomerID],[UserID],[FeeLevel] from tblActiveCard where IsDelete=0 and" +
            //    " Date>='" + _fromdate +
            //    "' and Date<='" + _todate +
            //    "' and CardGroupID LIKE '%" + CardGroupID +
            //    "%' and CustomerID LIKE '%" + CustomerID +
            //    "%' and CustomerGroupID LIKE '%" + CustomerGroupID +
            //    "%' and (Plate LIKE N'%" + KeyWord +
            //    "%' or CardNumber LIKE '%" + KeyWord +
            //    "%') order by Date desc");


            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                // PRINTCONTROLLER:  -----------------
                PrinterControl.PageIndex = pageIndex;
                PrinterControl.PageNext = ps;
                PrinterControl.TotalItem = totalCount;
                //------------------------------------
                rpt_Card.DataSource = dtCard;
                rpt_Card.DataBind();
            }

        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}
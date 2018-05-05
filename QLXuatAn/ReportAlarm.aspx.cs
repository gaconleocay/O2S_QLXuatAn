using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Services;
using Futech.Tools;
using Futech.Helpers;

public partial class QLXuatAn_ReportAlarm : System.Web.UI.Page
{
    public long _money2 = 0;

    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
    string AlarmCode = "";
    string LaneID = "", UserID = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {


            if (Request.Cookies["UserID"] != null)
                ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
            else
                ViewState["UserID"] = "";

            if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Report_Report1", "Selects", "Parking") == false)
            {
                Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
            }

            div_alert.Visible = false;

            dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

            //cardgroup


            //lane
            DataTable dtlane = CacheLayer.Get<DataTable>(StaticCached.c_tblLane);
            if (dtlane == null)
            {
                dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane order by SortOrder");
                if (dtlane!=null && dtlane.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblLane, dtlane, StaticCached.TimeCache);
            }
            if (dtlane != null && dtlane.Rows.Count > 0)
            {
                cbLane.Items.Add(new ListItem("<< Tất cả các làn >>", ""));
                foreach (DataRow dr in dtlane.Rows)
                {
                    cbLane.Items.Add(new ListItem(dr["LaneName"].ToString(), dr["LaneID"].ToString()));
                }
            }

            //user
            DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
            if (dtuser == null)
            {
                dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
                if (dtuser!=null && dtuser.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
            }
            if (dtuser != null && dtuser.Rows.Count > 0)
            {
                cbUser.Items.Add(new ListItem("<< Tất cả người dùng >>", ""));
                foreach (DataRow dr in dtuser.Rows)
                {
                    cbUser.Items.Add(new ListItem(dr["UserName"].ToString(), dr["UserID"].ToString()));
                }
            }

            //alarmcode
            cbAlarmCode.Items.Add(new ListItem("<<Tất cả>>",""));
            for (int i = 0; i < SystemUI.ListAlarmCode.Length; i++)
            {
                string[] _code = SystemUI.ListAlarmCode[i].Split(':');
                if (_code != null && _code.Length == 2)
                {
                    cbAlarmCode.Items.Add(new ListItem(_code[1], _code[0]));
                }
            }


            BindDataList();

        }
    }

    public string GetCardGroup(string CardGroupID)
    {
        if (CardGroupID == "LOOP_D")
            return "Vòng từ-Xe lượt";
        else if (CardGroupID == "LOOP_M")
            return "Vòng từ-Xe tháng";
        DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            return dtCardGroup.Rows[0]["CardGroupName"].ToString();
        else
            return "";
    }

    public string GetUserName(string userid)
    {
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
        DataTable dt = StaticPool.mdb.FillData("select CustomerName from tblCustomer where CustomerID='" + customerid + "'");
        if (dt != null && dt.Rows.Count > 0)
            return dt.Rows[0]["CustomerName"].ToString();

        return "";
    }

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

    public string GetAlarmName(string alarmcode)
    {
        try
        {
            for (int i = 0; i < SystemUI.ListAlarmCode.Length; i++)
            {
                string[] _code = SystemUI.ListAlarmCode[i].Split(':');
                if (_code != null && _code.Length == 2)
                {
                    if (_code[0] == alarmcode)
                        return _code[1];
                }
            }
        }
        catch
        { }
        return "";
    }


    private void BindDataList()
    {
        try
        {
            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }

            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
                txtKeyWord.Value = KeyWord;
            }

           


            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
                dtpFromDate.Value = FromDate;
            }

            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"].ToString() != "")
            {
                ToDate = Request.QueryString["ToDate"].ToString();
                dtpToDate.Value = ToDate;
            }

           
            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"].ToString() != "")
            {
                LaneID = Request.QueryString["LaneID"].ToString();
                cbLane.Value = LaneID;
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                cbUser.Value = UserID;
            }

            if (Request.QueryString["AlarmCode"] != null && Request.QueryString["AlarmCode"].ToString() != "")
            {
                AlarmCode = Request.QueryString["AlarmCode"].ToString();
                cbAlarmCode.Value = AlarmCode;
            }

            //DataTable dt = new DataTable();
            //dt.Columns.Add("Date", typeof(string));
            //dt.Columns.Add("CardNumber", typeof(string));
            //dt.Columns.Add("Plate", typeof(string));           
            //dt.Columns.Add("PicIn1", typeof(string));
            //dt.Columns.Add("PicIn2", typeof(string));
            //dt.Columns.Add("AlarmCode", typeof(string));
            //dt.Columns.Add("LaneID", typeof(string));
            //dt.Columns.Add("UserID", typeof(string));
         
            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";
            var totalCount = 0;
            DataTable dt = ReportService.GetReportAlarm(KeyWord, _fromdate, _todate, LaneID, UserID, AlarmCode, pageIndex, StaticPool.pagesize, ref totalCount);

            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        string _picdir1 = dr["PicDir"].ToString();
            //        dr["PicIn2"] = _picdir1.Replace("PLATEIN.JPG", "OVERVIEWIN.JPG").Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
            //        dr["PicIn1"] = _picdir1;
            //    }
            //}

            //DataTable dtevent = null;
            //string st = "";

            //st = "select * from tblAlarm where" +
            //    " Date>='" + _fromdate +
            //    "' and Date<='" + _todate +
            //    "' and LaneID LIKE '%" + LaneID +
            //    "%' and UserID LIKE '%" + UserID +
            //    "%' and AlarmCode LIKE '%" + AlarmCode +
            //    "%' and (CardNumber LIKE N'%" + KeyWord +
            //    "%' or Plate LIKE N'%" + KeyWord +
            //    "%') order by Date desc";



            //dtevent = StaticPool.mdbevent.FillData(st);

            //if (dtevent != null && dtevent.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dtevent.Rows)
            //    {
            //        string _picdir1 = dr["PicDir"].ToString();
            //        string _picdir2 = _picdir1.Replace("PLATEIN.JPG", "OVERVIEWIN.JPG").Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
            //        dt.Rows.Add(dr["Date"].ToString(),
            //            dr["CardNumber"].ToString(),
            //            dr["Plate"].ToString(),
            //            _picdir1,
            //            _picdir2,
            //            dr["AlarmCode"].ToString(),
            //            dr["LaneID"].ToString(),
            //            dr["UserID"].ToString());
            //    }
            //}

            //if (dt == null)
            //    return;

            //// Phan trang
            //pgsource.DataSource = dt.DefaultView;
            ////Set PageDataSource paging 
            //pgsource.AllowPaging = true;
            ////Set number of items to be displayed in the Repeater using drop down list
            //pgsource.PageSize = 20;
            ////Get Current Page Index
            //pgsource.CurrentPageIndex = CurrentPage;
            ////Store it Total pages value in View state
            //ViewState["totpage"] = pgsource.PageCount;
            ////Below line is used to show page number based on selection like "Page 1 of 20"
            //lblpage.Text = (CurrentPage + 1) + " / " + pgsource.PageCount;
            ////Enabled true Link button previous when current page is not equal first page 
            ////Enabled false Link button previous when current page is first page
            //lnkPrevious.Enabled = !pgsource.IsFirstPage;
            ////Enabled true Link button Next when current page is not equal last page 
            ////Enabled false Link button Next when current page is last page
            //lnkNext.Enabled = !pgsource.IsLastPage;
            ////Enabled true Link button First when current page is not equal first page 
            ////Enabled false Link button First when current page is first page
            //lnkFirst.Enabled = !pgsource.IsFirstPage;
            ////Enabled true Link button Last when current page is not equal last page 
            ////Enabled false Link button Last when current page is last page
            //lnkLast.Enabled = !pgsource.IsLastPage;

            ////Create Paging with help of DataList control "RepeaterPaging"
            //doPaging();
            ////RepeaterPaging.ItemStyle.HorizontalAlign = HorizontalAlign.Center;


            if (dt != null && dt.Rows.Count > 0)
            {
                id_reportin.InnerText = "Số bản ghi (" + totalCount + ")";
                StaticPool.HNGpaging(dt, totalCount, StaticPool.pagesize, pager, rpt_Card);
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

    private int CurrentPage
    {
        get
        {   //Check view state is null if null then return current index value as "0" else return specific page viewstate value
            if (ViewState["CurrentPage"] == null)
            {
                return 0;
            }
            else
            {
                return ((int)ViewState["CurrentPage"]);
            }
        }
        set
        {
            //Set View statevalue when page is changed through Paging "RepeaterPaging" DataList
            ViewState["CurrentPage"] = value;
        }
    }

    public string formatMoney(string money)
    {
        if (double.Parse(money) == 0)
            return "";
        else
            return String.Format("{0:000,0}", Convert.ToDouble(money));
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            string _id = "";
            if (StaticPool.CheckPermission(userid, "Parking_Report_Report1", "Deletes", "Parking"))
            {
                if (id.Contains("_CARD") == true)
                {
                    _id = id.Replace("_CARD", "");
                    string _cardnumber = StaticPool.GetCardNumberByID(_id);


                    if (StaticPool.mdbevent.ExecuteCommand("update tblCardEvent set EventCode='2', IsDelete=1 where Id='" + _id + "'"))
                    {
                        StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Parking_Report1", _cardnumber, "Xóa", "Id=" + _id);
                        return "true";
                    }

                }
                else if (id.Contains("_LOOP") == true)
                {
                    _id = id.Replace("_LOOP", "");
                    string _plate = StaticPool.GetPlateByID(_id);

                    if (StaticPool.mdbevent.ExecuteCommand("update tblLoopEvent set EventCode='2', IsDelete=1 where Id='" + _id + "'"))
                    {
                        StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Parking_Report1", _plate, "Xóa", "Id=" + _id);
                        return "true";
                    }
                }

                return "Failed";

            }
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    //export all page
    protected void Excel_Click(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
               // txtKeyWord.Value = KeyWord;
            }




            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
                //dtpFromDate.Value = FromDate;
            }

            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"].ToString() != "")
            {
                ToDate = Request.QueryString["ToDate"].ToString();
                //dtpToDate.Value = ToDate;
            }


            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"].ToString() != "")
            {
                LaneID = Request.QueryString["LaneID"].ToString();
                //cbLane.Value = LaneID;
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                //cbUser.Value = UserID;
            }

            if (Request.QueryString["AlarmCode"] != null && Request.QueryString["AlarmCode"].ToString() != "")
            {
                AlarmCode = Request.QueryString["AlarmCode"].ToString();
                //cbAlarmCode.Value = AlarmCode;
            }
            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("Thời gian", typeof(string));
            dt.Columns.Add("Mã thẻ", typeof(string));
            dt.Columns.Add("Biển số", typeof(string));
            //dt.Columns.Add("PicIn1", typeof(string));
            //dt.Columns.Add("PicIn2", typeof(string));
            dt.Columns.Add("Cảnh báo", typeof(string));
            dt.Columns.Add("Diễn giải", typeof(string));
            dt.Columns.Add("Làn", typeof(string));
            dt.Columns.Add("Người dùng", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            DataTable dtevent = ReportService.GetReportAlarmExcel(KeyWord, _fromdate, _todate, LaneID, UserID, AlarmCode, pageIndex, StaticPool.pagesize);
            //DataTable dtevent = null;
            //string st = "";

            //st = "select * from tblAlarm where" +
            //    " Date>='" + _fromdate +
            //    "' and Date<='" + _todate +
            //    "' and LaneID LIKE '%" + LaneID +
            //    "%' and UserID LIKE '%" + UserID +
            //    "%' and AlarmCode LIKE '%" + AlarmCode +
            //    "%' and (CardNumber LIKE N'%" + KeyWord +
            //    "%' or Plate LIKE N'%" + KeyWord +
            //    "%') order by Date desc";



            //dtevent = StaticPool.mdbevent.FillData(st);

            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    //string _picdir1 = dr["PicDir"].ToString();
                    //string _picdir2 = _picdir1.Replace("PLATEIN.JPG", "OVERVIEWIN.JPG").Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
                    dr["Cảnh báo"] = GetAlarmName(dr["Cảnh báo"].ToString());
                    dr["Làn"] = GetLane(dr["Làn"].ToString());
                    dr["Người dùng"] = GetUserName(dr["Người dùng"].ToString());
                    //dt.Rows.Add(
                    //    dt.Rows.Count+1,
                    //    this.GetDateTime(dr["Date"].ToString()),
                    //    dr["CardNumber"].ToString(),
                    //    dr["Plate"].ToString(),
                    //    //_picdir1,
                    //    //_picdir2,
                    //    this.GetAlarmName(dr["AlarmCode"].ToString()),
                    //    this.GetLane(dr["LaneID"].ToString()),
                    //    this.GetUserName(dr["UserID"].ToString()));
                }
            }

            string _title1 = "Báo cáo sự kiện cảnh báo";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dtevent, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportAlarm.aspx", false);
            }
            //GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
            //GridView gv = new GridView();
            //gv.DataSource = dtevent;
            //gv.DataBind();

            //Table tb = new Table();
            //TableRow tr1 = new TableRow();
            //TableCell cell1 = new TableCell();
            //cell1.Controls.Add(gvheader);
            //tr1.Cells.Add(cell1);

            //TableCell cell3 = new TableCell();
            //cell3.Controls.Add(gv);

            //TableCell cell2 = new TableCell();
            //TableRow tr2 = new TableRow();
            //tr2.Cells.Add(cell2);

            //TableRow tr3 = new TableRow();
            //tr3.Cells.Add(cell3);

            //tb.Rows.Add(tr1);
            //tb.Rows.Add(tr2);
            //tb.Rows.Add(tr3);

            //gv.GridLines = GridLines.None;
            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=Report" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls");
            //Response.Charset = "";
            //Response.ContentType = "application/excel";
            //System.IO.StringWriter sw = new System.IO.StringWriter();
            //HtmlTextWriter htm = new HtmlTextWriter(sw);
            //Response.ContentEncoding = System.Text.Encoding.UTF8;
            ////StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(htm);
            //htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");

            //tb.RenderControl(htm);

            //Response.Write(sw.ToString());

            //HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
            //HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            //HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }

    }

    //--#1
    public void BindDataToExcel(DataTable _dt, string _title1, string _title2, string uId)
    {
        var dtHeader = StaticPool.getHeaderExcel(_title1, _title2, StaticPool.GetUserName(uId));
        // Gọi lại hàm để tạo file excel
        var stream = StaticPool.CreateExcelFile(new MemoryStream(), _dt, dtHeader);
        // Tạo buffer memory strean để hứng file excel
        var buffer = stream as MemoryStream;
        // Đây là content Type dành cho file excel, còn rất nhiều content-type khác nhưng cái này mình thấy okay nhất
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        // Dòng này rất quan trọng, vì chạy trên firefox hay IE thì dòng này sẽ hiện Save As dialog cho người dùng chọn thư mục để lưu
        // File name của Excel này là ExcelDemo
        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={1}-{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmm"), _title1.Replace(" ", "-")));
        // Lưu file excel của chúng ta như 1 mảng byte để trả về response
        Response.BinaryWrite(buffer.ToArray());
        // Send tất cả ouput bytes về phía clients
        Response.Flush();
        Response.End();
    }
}
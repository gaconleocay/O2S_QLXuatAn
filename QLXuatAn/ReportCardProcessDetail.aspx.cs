using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Services;
using Futech.Helpers;

public partial class QLXuatAn_ReportCardProcess : System.Web.UI.Page
{
    public long _money2 = 0;

    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
    string Actions = "";
    string UserID = "";

    DataTable dtCus = null;
    DataTable dtcustomergroup = null;
    DataTable dtCardSub = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {


            if (Request.Cookies["UserID"] != null)
                ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
            else
                ViewState["UserID"] = "";

            if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Report_Report7", "Selects", "Parking") == false)
            {
                Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
            }

            div_alert.Visible = false;

            dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

            //cardgroup
            DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
            if (dtCardGroup == null)
            {
                dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
                if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
            }
            if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            {
                cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                foreach (DataRow dr in dtCardGroup.Rows)
                {
                    cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                }

                if (StaticPool.SystemUsingLoop() == true)
                {
                    cbCardGroup.Items.Add(new ListItem("Vòng từ-Xe lượt (Loop)", "LOOP_D"));
                    cbCardGroup.Items.Add(new ListItem("Vòng từ-Xe tháng (Loop)", "LOOP_M"));
                }
            }

            //user
            DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
            if (dtuser == null)
            {
                dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
                if (dtuser != null && dtuser.Rows.Count > 0)
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

            //Customergroup
            dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
            if (dtcustomergroup == null)
            {
                dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
                if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtcustomergroup, StaticCached.TimeCache);
            }
            //DataTable dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID, CustomerGroupName from tblCustomerGroup order by SortOrder");
            if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
            {
                cbCustomerGroup.Items.Add(new ListItem("<<Tất cả nhóm KH>>", ""));
                foreach (DataRow dr in dtcustomergroup.Rows)
                {
                    cbCustomerGroup.Items.Add(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
                }
            }



            BindDataList();

        }
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

    public string GetCustomerName(string customerid, ref string address)
    {
        var gName = "";
        if (dtCus != null)
        {
            var rRow = dtCus.Select(string.Format("CustomerID = '{0}'", customerid));
            if (rRow.Any())
            {
                gName = rRow[0]["CustomerName"].ToString();
                address = rRow[0]["Address"].ToString();
            }
        }
        return gName;
    }

    public string GetPlateByCardNumber(string cardNumber)
    {
        var gName = "";
        if (dtCardSub != null)
        {
            var rRow = dtCardSub.Select(string.Format("CardNumber = '{0}'", cardNumber));
            if (rRow.Any())
            {
                gName = rRow[0]["Plate"].ToString();
            }
        }
        return gName;
    }
    public string GetCardNoByCardNumber(string cardNumber)
    {
        var gName = "";
        if (dtCardSub != null)
        {
            var rRow = dtCardSub.Select(string.Format("CardNumber = '{0}'", cardNumber));
            if (rRow.Any())
            {
                gName = rRow[0]["CardNo"].ToString();
            }
        }
        return gName;
    }
    public string GetCustomerGroupName(string cusGroupId)
    {
        var gName = "";
        if (dtcustomergroup != null)
        {
            var rRow = dtcustomergroup.Select(string.Format("CustomerGroupID = '{0}'", cusGroupId));
            if (rRow.Any())
            {
                gName = rRow[0]["CustomerGroupName"].ToString();
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

    public string GetLane(string laneid)
    {
        DataTable dt = StaticPool.mdb.FillData("select LaneName from tblLane where LaneID='" + laneid + "'");
        if (dt != null && dt.Rows.Count > 0)
            return dt.Rows[0]["LaneName"].ToString();
        return "";
    }

    public string GetAction(string actions)
    {
        switch (actions)
        {
            case "ADD":
                return "Thêm thẻ";
            case "DELETE":
                return "Xóa thẻ";
            case "CHANGE":
                return "Đổi thẻ";
            case "RELEASE":
                return "Phát thẻ";
            case "RETURN":
                return "Trả thẻ";
            case "LOCK":
                return "Khóa thẻ";
            case "UNLOCK":
                return "Mở thẻ";
            default:
                return "";

        }
    }

    public string GetActionLabel(string actions)
    {
        switch (actions)
        {
            case "ADD":
                return HttpContext.Current.Server.HtmlEncode("<span class='label label-success'>Thêm thẻ</span>"); 
            case "DELETE":
                return HttpContext.Current.Server.HtmlEncode("<span class='label label-danger'>Xóa thẻ</span>");
            case "CHANGE":
                return HttpContext.Current.Server.HtmlEncode("<span class='label label-info'>Đổi thẻ</span>");
            case "RELEASE":
                return HttpContext.Current.Server.HtmlEncode("<span class='label label-purple'>Phát thẻ</span>");
            case "RETURN":
                return HttpContext.Current.Server.HtmlEncode("<span class='label'>Trả thẻ</span>");
            case "LOCK":
                return HttpContext.Current.Server.HtmlEncode("<span class='label label-light'>Khóa thẻ</span>");
            case "UNLOCK":
                return HttpContext.Current.Server.HtmlEncode("<span class='label label-grey'>Mở thẻ</span>");
            default:
                return "";
        }
    }
    //public string GetCardInfo(string cardnumber, ref string plate)
    //{
    //    try
    //    {
    //        DataTable dt = StaticPool.mdb.FillData("select CardNo, Plate1, Plate2, Plate3 from tblCard where IsDelete=0 and CardNumber='" + cardnumber + "'");
    //        if (dt != null && dt.Rows.Count > 0)
    //        {
    //            DataRowView drv = dt.DefaultView[0];
    //            plate = drv["Plate1"].ToString();
    //            if (drv["Plate2"].ToString() != "")
    //                plate = plate + "_" + drv["Plate2"].ToString();
    //            if (drv["Plate3"].ToString() != "")
    //                plate = plate + "_" + drv["Plate3"].ToString();
    //            return drv["CardNo"].ToString();
    //        }
    //    }
    //    catch
    //    { 
    //    }
    //    return "";
    //}

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

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"].ToString();
                cbCardGroup.Value = CardGroupID;
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

            if (Request.QueryString["Actions"] != null && Request.QueryString["Actions"].ToString() != "")
            {
                Actions = Request.QueryString["Actions"].ToString();
                cbAction.Value = Actions;
            }

            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                cbUser.Value = UserID;
            }
            //Nhóm Khách hàng
            string CustomerGroupID = "";
            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = CustomerGroupID;
            }
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Date", typeof(string));
            //dt.Columns.Add("CardNo", typeof(string));
            //dt.Columns.Add("CardNumber", typeof(string));          
            //dt.Columns.Add("CardGroupID", typeof(string));
            //dt.Columns.Add("Actions", typeof(string));
            //dt.Columns.Add("Description", typeof(string));
            //dt.Columns.Add("Address", typeof(string));
            //dt.Columns.Add("Plate", typeof(string));
            //dt.Columns.Add("UserID", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            var totalCount = 0;
            DataTable dt = ReportService.GetReportCardProcessDetail(KeyWord, CustomerGroupID, _fromdate, _todate, CardGroupID, Actions, UserID, pageIndex, StaticPool.pagesize, ref totalCount);


            if (dt != null && dt.Rows.Count > 0)
            {
                var listCardNumber = "";
                foreach (DataRow item in dt.Rows)
                {
                    listCardNumber += item["CardNumber"] + ",";
                }
                dtCardSub = ReportService.GetCardByCardNumberForCardProcess(listCardNumber);

                id_reportin.InnerText = "Số bản ghi (" + totalCount + ")";
                //Bind resulted PageSource into the Repeater
                StaticPool.HNGpaging(dt, totalCount, StaticPool.pagesize, pager, rpt_Card);
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
                //txtKeyWord.Value = KeyWord;
            }

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"].ToString();
                //cbCardGroup.Value = CardGroupID;
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

            if (Request.QueryString["Actions"] != null && Request.QueryString["Actions"].ToString() != "")
            {
                Actions = Request.QueryString["Actions"].ToString();
                // cbAction.Value = Actions;
            }

            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                //cbUser.Value = UserID;
            }
            //Nhóm Khách hàng
            string CustomerGroupID = "";
            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();

            }
            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("Thời gian", typeof(string));
            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("Mã thẻ", typeof(string));
            dt.Columns.Add("Nhóm thẻ", typeof(string));
            dt.Columns.Add("Hành vi", typeof(string));
            dt.Columns.Add("Chủ thẻ", typeof(string));
            dt.Columns.Add("Địa chỉ", typeof(string));
            dt.Columns.Add("Biển số", typeof(string));
            dt.Columns.Add("NV thực hiện", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            DataTable dtevent = ReportService.GetReportCardProcessDetailExcel(KeyWord, CustomerGroupID, _fromdate, _todate, CardGroupID, Actions, UserID, pageIndex, StaticPool.pagesize);

            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
                if (dtcustomergroup == null)
                {
                    dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
                    if (dtcustomergroup.Rows.Count > 0)
                        CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtcustomergroup, StaticCached.TimeCache);
                }
                foreach (DataRow item in dtevent.Rows)
                {
                    //if (!string.IsNullOrWhiteSpace(item["Thời gian"].ToString()))
                    //{
                    //    item["Thời gian"] = Convert.ToDateTime(item["Thời gian"]).ToString("dd/MM/yyyy");
                    //}
                    if (!string.IsNullOrWhiteSpace(item["CustomerGroupID"].ToString()))
                    {
                        item["Nhóm KH"] = GetCustomerGroupName(item["CustomerGroupID"].ToString());
                        item["CustomerGroupID"] = "";
                    }

                }
            }
            //DataTable dtevent = null;
            //dtevent = StaticPool.mdb.FillData("select * from tblCardProcess where" +
            //    " Date>='" + _fromdate +
            //    "' and Date<='" + _todate +
            //    "' and CardGroupID LIKE '%" + CardGroupID +
            //    "%' and Actions LIKE '%" + Actions +
            //    "%' and UserID LIKE '%" + UserID +
            //    "%' and CardNumber LIKE '%" + KeyWord +
            //    "%' order by Date desc");

            //var listCusId = "";

            //foreach (DataRow item in dtevent.Rows)
            //{
            //    listCusId += item["CustomerID"].ToString() + ",";
            //}

            //dtCus = ReportService.GetCustomerBylistId(listCusId);

            //if (dtevent != null && dtevent.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dtevent.Rows)
            //    {
            //        string _actions = dr["Actions"].ToString();
            //        string _des = dr["Description"].ToString();
            //        string _address = "";
            //        string _plate = "";
            //        string _cardNo = "";

            //        _cardNo = GetCardInfo(dr["CardNumber"].ToString(), ref _plate);

            //        if (_actions == "RELEASE" || _actions == "RETURN")
            //            _des = GetCustomerName(dr["CustomerID"].ToString(), ref _address);

            //        dt.Rows.Add(
            //            dt.Rows.Count+1,
            //            this.GetDateTime(dr["Date"].ToString()),
            //            _cardNo,
            //            dr["CardNumber"].ToString(),
            //            this.GetCardGroup(dr["CardGroupID"].ToString()),
            //            this.GetAction(dr["Actions"].ToString()),
            //            _des,
            //            _address,
            //            _plate,
            //            this.GetUserName(dr["UserID"].ToString()));
            //    }
            //}

            string _title1 = "Báo cáo chi tiết xử lý thẻ";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dtevent, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportCardProcessDetail.aspx", false);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Services;

public partial class QLXuatAn_ReportCustomerDetail2 : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "", CompartmentID = "";
    string FromDate = "", ToDate = "";
    bool IsFilterByTimeRegister = false;
    string LaneID = "", UserID = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";

                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Report_Report9", "Selects", "Parking") == false)
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }

                DataTable ddlCompartment = StaticPool.mdb.FillData("select * from tblCompartment order by SortOrder");
                if (ddlCompartment != null && ddlCompartment.Rows.Count > 0)
                {
                    cbCompartment.Items.Add(new ListItem("<< Tất cả căn hộ >>", ""));
                    foreach (DataRow dr in ddlCompartment.Rows)
                    {
                        cbCompartment.Items.Add(new ListItem(dr["CompartmentName"].ToString(), dr["CompartmentID"].ToString()));
                    }
                }

                DataTable dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
                if (dtuser != null && dtuser.Rows.Count > 0)
                {
                    cbUserSelect.Items.Add(new ListItem("<< Tất cả người dùng >>", ""));
                    foreach (DataRow dr in dtuser.Rows)
                    {
                        cbUserSelect.Items.Add(new ListItem(dr["UserName"].ToString(), dr["UserID"].ToString()));
                    }
                }

                div_alert.Visible = false;

                dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
                dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

                BindDataList();
            }
            catch (Exception ex)
            {
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }

        }
    }

    public string GetUserName(string userid)
    {
        DataTable dt = StaticPool.mdb.FillData("select UserName from tblUser where UserID='" + userid + "'");
        if (dt != null && dt.Rows.Count > 0)
            return dt.Rows[0]["UserName"].ToString();
        return "";
    }

    public string GetCustomerName(string customerid)
    {
        DataTable dt = StaticPool.mdb.FillData("select CustomerName from tblCustomer where CustomerID='" + customerid + "'");
        if (dt != null && dt.Rows.Count > 0)
            return dt.Rows[0]["CustomerName"].ToString();

        return "";
    }

    public string ConvertName(string name)
    {
        var nameconvert = "";

        if (name.Equals("Create"))
        {
            nameconvert = "Thêm mới";
        }

        if (name.Equals("Update"))
        {
            nameconvert = "Cập nhật";
        }

        return nameconvert;
    }

    public string GetDateTime(string dtime)
    {
        if (dtime != "")
        {
            return DateTime.Parse(dtime).ToString("dd/MM/yyyy");
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

            if (Request.QueryString["IsFilterByTimeRegister"] != null && Request.QueryString["IsFilterByTimeRegister"].ToString() != "")
            {
                IsFilterByTimeRegister = bool.Parse(Request.QueryString["IsFilterByTimeRegister"].ToString());
                chFilterByTimeRegister.Checked = IsFilterByTimeRegister;
                chFilterByTimeRelease.Checked = !chFilterByTimeRegister.Checked;
            }

            if (Request.QueryString["CompartmentId"] != null && Request.QueryString["CompartmentId"].ToString() != "")
            {
                CompartmentID = Request.QueryString["CompartmentId"].ToString();
                cbCompartment.Value = CompartmentID;
            }

            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                cbUserSelect.Value = UserID;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("LogID", typeof(string));

            dt.Columns.Add("CompartmentName", typeof(string));

            dt.Columns.Add("CustomerCode", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("CustomerGroup", typeof(string));

            dt.Columns.Add("CardNumber", typeof(string));
            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("CardGroup", typeof(string));
            dt.Columns.Add("Plate", typeof(string));

            dt.Columns.Add("DateRegister", typeof(string));
            dt.Columns.Add("DateRelease", typeof(string));
            dt.Columns.Add("DateCanceled", typeof(string));

            dt.Columns.Add("UserID", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            //card
            DataTable dtevent = null;
            string st = "";
            if (IsFilterByTimeRegister == true)
            {
                st = "select * from tblLogCardCustomer where (" +
                    " DateRegisted>='" + _fromdate +
                    "' and DateRegisted<='" + _todate +
                    "') and (CardNumber LIKE '%" + KeyWord +
                    "%' or CustomerCode LIKE '%" + KeyWord +
                    "' or CustomerName LIKE N'%" + KeyWord +
                    "') and (UserID LIKE '%" + UserID +
                    "') and (CompartmentID LIKE '%" + CompartmentID +
                    "%') order by DateChanged desc";
            }
            else
            {
                st = "select * from tblLogCardCustomer where (" +
                    " DateReleased>='" + _fromdate +
                    "' and DateReleased<='" + _todate +
                    "') and (CardNumber LIKE '%" + KeyWord +
                    "%' or CustomerCode LIKE '%" + KeyWord +
                    "%' or CustomerName LIKE N'%" + KeyWord +
                    "') and (UserID LIKE '%" + UserID +
                    "%') and (CompartmentID LIKE '%" + CompartmentID +
                    "%') order by DateChanged desc";
            }

            dtevent = StaticPool.mdb.FillData(st);
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    string LogId = dr["ID"].ToString();

                    string _compartmentName = "";

                    string _customerCode = dr["CustomerCode"].ToString();
                    string _customerName = dr["CustomerName"].ToString();
                    string _customerGroup = "";

                    string _cardNumber = dr["CardNumber"].ToString();
                    string _cardNo = dr["CardNo"].ToString();
                    string _cardGroup = "";
                    string _plate = dr["Plate"].ToString();

                    string _dateRegister = dr["DateRegisted"].ToString();
                    string _dateRelease = dr["DateReleased"].ToString();
                    string _dateCancele = dr["DateCanceled"].ToString();

                    string _user = StaticPool.GetUserName(dr["UserId"].ToString());

                    GetInfoFromCustomer(dr["CompartmentID"].ToString(), dr["CustomerGroupID"].ToString(), dr["CardGroupID"].ToString(), ref _compartmentName, ref _customerGroup, ref _cardGroup);

                    dt.Rows.Add(LogId, _compartmentName, _customerCode, _customerName, _customerGroup, _cardNumber, _cardNo, _cardGroup, _plate, _dateRegister, _dateRelease, _dateCancele, _user);
                }
            }

            if (dt == null)
                return;

            // Phan trang
            pgsource.DataSource = dt.DefaultView;
            //Set PageDataSource paging 
            pgsource.AllowPaging = true;
            //Set number of items to be displayed in the Repeater using drop down list
            pgsource.PageSize = 20;
            //Get Current Page Index
            pgsource.CurrentPageIndex = CurrentPage;
            //Store it Total pages value in View state
            ViewState["totpage"] = pgsource.PageCount;
            //Below line is used to show page number based on selection like "Page 1 of 20"
            lblpage.Text = (CurrentPage + 1) + " / " + pgsource.PageCount;
            //Enabled true Link button previous when current page is not equal first page 
            //Enabled false Link button previous when current page is first page
            lnkPrevious.Enabled = !pgsource.IsFirstPage;
            //Enabled true Link button Next when current page is not equal last page 
            //Enabled false Link button Next when current page is last page
            lnkNext.Enabled = !pgsource.IsLastPage;
            //Enabled true Link button First when current page is not equal first page 
            //Enabled false Link button First when current page is first page
            lnkFirst.Enabled = !pgsource.IsFirstPage;
            //Enabled true Link button Last when current page is not equal last page 
            //Enabled false Link button Last when current page is last page
            lnkLast.Enabled = !pgsource.IsLastPage;

            //Create Paging with help of DataList control "RepeaterPaging"
            doPaging();
            //RepeaterPaging.ItemStyle.HorizontalAlign = HorizontalAlign.Center;


            if (dt != null && dt.Rows.Count > 0)
            {
                id_cardlist.InnerText = "Số bản ghi (" + dt.Rows.Count + ")";
                //Bind resulted PageSource into the Repeater
                rpt_ReportCustomerDetail2.DataSource = pgsource;
                rpt_ReportCustomerDetail2.DataBind();
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

    private void doPaging()
    {
        DataTable dt = new DataTable("Paging");
        //Add two column into the DataTable "dt" 
        //First Column store page index default it start from "0"
        //Second Column store page index default it start from "1"
        dt.Columns.Add("PageIndex");
        dt.Columns.Add("PageText");

        //Assign First Index starts from which number in paging data list
        findex = CurrentPage - 3;

        //Set Last index value if current page less than 5 then last index added "5" values to the Current page else it set "10" for last page number
        if (CurrentPage > 3)
        {
            lindex = CurrentPage + 3;
        }
        else
        {
            lindex = 6;
        }

        //Check last page is greater than total page then reduced it to total no. of page is last index
        if (lindex > Convert.ToInt32(ViewState["totpage"]))
        {
            lindex = Convert.ToInt32(ViewState["totpage"]);
            findex = lindex - 6;
        }

        if (findex < 0)
        {
            findex = 0;
        }

        //Now creating page number based on above first and last page index
        for (int i = findex; i < lindex; i++)
        {
            DataRow dr = dt.NewRow();
            dr[0] = i;
            dr[1] = i + 1;
            dt.Rows.Add(dr);
        }

        //Finally bind it page numbers in to the Paging DataList "RepeaterPaging"
        RepeaterPaging.DataSource = dt;
        RepeaterPaging.DataBind();
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        //If user click First Link button assign current index as Zero "0" then refresh "Repeater1" Data.
        CurrentPage = 0;
        BindDataList();
    }
    protected void lnkLast_Click(object sender, EventArgs e)
    {
        //If user click Last Link button assign current index as totalpage then refresh "Repeater1" Data.
        CurrentPage = (Convert.ToInt32(ViewState["totpage"]) - 1);
        BindDataList();
    }
    protected void lnkPrevious_Click(object sender, EventArgs e)
    {
        //If user click Previous Link button assign current index as -1 it reduce existing page index.
        CurrentPage -= 1;
        //refresh "Repeater1" Data
        BindDataList();
    }
    protected void lnkNext_Click(object sender, EventArgs e)
    {
        //If user click Next Link button assign current index as +1 it add one value to existing page index.
        CurrentPage += 1;

        //refresh "Repeater1" Data
        BindDataList();
    }
    protected void RepeaterPaging_ItemCommand(object source, DataListCommandEventArgs e)
    {
        if (e.CommandName.Equals("newpage"))
        {
            //Assign CurrentPage number when user click on the page number in the Paging "RepeaterPaging" DataList
            CurrentPage = Convert.ToInt32(e.CommandArgument.ToString());
            //Refresh "Repeater1" control Data once user change page
            BindDataList();
        }
    }
    protected void RepeaterPaging_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        //Enabled False for current selected Page index
        LinkButton lnkPage = (LinkButton)e.Item.FindControl("Pagingbtn");
        if (lnkPage.CommandArgument.ToString() == CurrentPage.ToString())
        {
            lnkPage.Enabled = false;
            lnkPage.BackColor = System.Drawing.Color.FromName("#FFCC01");
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

            if (Request.QueryString["IsFilterByTimeRegister"] != null && Request.QueryString["IsFilterByTimeRegister"].ToString() != "")
            {
                IsFilterByTimeRegister = bool.Parse(Request.QueryString["IsFilterByTimeRegister"].ToString());
                chFilterByTimeRegister.Checked = IsFilterByTimeRegister;
                chFilterByTimeRelease.Checked = !chFilterByTimeRegister.Checked;
            }

            if (Request.QueryString["CompartmentId"] != null && Request.QueryString["CompartmentId"].ToString() != "")
            {
                CompartmentID = Request.QueryString["CompartmentId"].ToString();
                cbCompartment.Value = CompartmentID;
            }

            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                cbUserSelect.Value = UserID;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("Căn hộ", typeof(string));

            dt.Columns.Add("Mã khách hàng", typeof(string));
            dt.Columns.Add("Tên khách hàng", typeof(string));
            dt.Columns.Add("Nhóm khách hàng", typeof(string));

            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("Mã thẻ", typeof(string));
            dt.Columns.Add("Nhóm thẻ", typeof(string));
            dt.Columns.Add("Biển số", typeof(string));

            dt.Columns.Add("Ngày đăng ký", typeof(string));
            dt.Columns.Add("Ngày phát", typeof(string));
            dt.Columns.Add("Ngày khóa", typeof(string));

            dt.Columns.Add("Người thực hiện", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            //card
            DataTable dtevent = null;
            string st = "";
            if (IsFilterByTimeRegister == true)
            {
                st = "select * from tblLogCardCustomer where (" +
                    " DateRegisted>='" + _fromdate +
                    "' and DateRegisted<='" + _todate +
                    "') and (CardNumber LIKE '%" + KeyWord +
                    "%' or CustomerCode LIKE '%" + KeyWord +
                    "' or CustomerName LIKE N'%" + KeyWord +
                    "') and (UserID LIKE '%" + UserID +
                    "') and (CompartmentID LIKE '%" + CompartmentID +
                    "%') order by DateChanged desc";
            }
            else
            {
                st = "select * from tblLogCardCustomer where (" +
                    " DateReleased>='" + _fromdate +
                    "' and DateReleased<='" + _todate +
                    "') and (CardNumber LIKE '%" + KeyWord +
                    "%' or CustomerCode LIKE '%" + KeyWord +
                    "%' or CustomerName LIKE N'%" + KeyWord +
                    "') and (UserID LIKE '%" + UserID +
                    "%') and (CompartmentID LIKE '%" + CompartmentID +
                    "%') order by DateChanged desc";
            }

            dtevent = StaticPool.mdb.FillData(st);
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    string _compartmentName = "";

                    string _customerCode = dr["CustomerCode"].ToString();
                    string _customerName = dr["CustomerName"].ToString();
                    string _customerGroup = "";

                    string _cardNumber = dr["CardNumber"].ToString();
                    string _cardNo = dr["CardNo"].ToString();
                    string _cardGroup = "";
                    string _plate = dr["Plate"].ToString();

                    string _dateRegister = dr["DateRegisted"].ToString();
                    string _dateRelease = dr["DateReleased"].ToString();
                    string _dateCancele = dr["DateCanceled"].ToString();

                    string _user = StaticPool.GetUserName(dr["UserId"].ToString());

                    GetInfoFromCustomer(dr["CompartmentID"].ToString(), dr["CustomerGroupID"].ToString(), dr["CardGroupID"].ToString(), ref _compartmentName, ref _customerGroup, ref _cardGroup);

                    dt.Rows.Add(_compartmentName, _customerCode, _customerName, _customerGroup,_cardNo, _cardNumber, _cardGroup, _plate, _dateRegister, _dateRelease, _dateCancele, _user);
                }
            }

            if (dt == null)
                return;

            string _title1 = "Báo cáo chi tiết sử dụng thẻ";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dt, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportCustomerDetail2.aspx", false);
            }
            //GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
            //GridView gv = new GridView();
            //gv.DataSource = dt;
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
            //string style = @"<style> TD { mso-number-format:\@; } </style>";
            //Response.Write(style);

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
    private void GetInfoFromCustomer(string compartmentid, string customergroupid, string cardgroupid, ref string compartmentname, ref string customergroupname, ref string cardgroupname)
    {
        compartmentname = GetCompartment(compartmentid);

        customergroupname = GetCustomerGroup(customergroupid);

        cardgroupname = GetCardGroup(cardgroupid);
    }

    public string GetCustomerGroup(string groupid)
    {
        DataTable dtCustomerGroup = StaticPool.mdb.FillData("select CustomerGroupName from tblCustomerGroup where CustomerGroupID = '" + groupid + "'");
        if (dtCustomerGroup != null && dtCustomerGroup.Rows.Count > 0)
            return dtCustomerGroup.Rows[0]["CustomerGroupName"].ToString();
        else
            return "";
    }

    public string GetCompartment(string compartmentid)
    {
        DataTable dtCompartment = StaticPool.mdb.FillData("select CompartmentName from tblCompartment where CompartmentID = '" + compartmentid + "'");
        if (dtCompartment != null && dtCompartment.Rows.Count > 0)
            return dtCompartment.Rows[0]["CompartmentName"].ToString();
        else
            return "";
    }

    public string GetCardGroup(string cardgroupid)
    {
        DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName from tblCardGroup where CardGroupID = '" + cardgroupid + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            return dtCardGroup.Rows[0]["CardGroupName"].ToString();
        else
            return "";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Report_Report9", "Deletes", "Parking") == false)
                return "Bạn không có quyền thực hiện chức năng này!";

            DataTable temp = StaticPool.mdb.FillData("select * from tblLogCardCustomer where ID=" + id);

            if (temp != null & temp.Rows.Count > 0)
            {
               // var report = getOldRecordLog(temp.Rows[0]["CustomerID"].ToString(), temp.Rows[0]["CardNumber"].ToString(), temp.Rows[0]["CompartmentID"].ToString(), temp.Rows[0]["DateChanged"].ToString(), temp.Rows[0]["DateRegisted"].ToString(), temp.Rows[0]["DateReleased"].ToString());
                //if (report.Equals("OK"))
                //{
                //    if (StaticPool.mdb.ExecuteCommand("delete from tblLogCardCustomer where ID = " + id + ""))
                //    {
                //        StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Report", "Parking_Report_Report9", temp.Rows[0]["ID"].ToString(), "Xóa", "id=" + id);
                //        return "true";
                //    }
                //}
                //else
                //{
                //    return report;
                //}
                if (StaticPool.mdb.ExecuteCommand("delete from tblLogCardCustomer where ID = " + id + ""))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Report", "Parking_Report_Report9", temp.Rows[0]["ID"].ToString(), "Xóa", "id=" + id);
                    return "true";
                }
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }

    private static string getOldRecordLog(string customerid, string cardnumber, string compartment, string datechange, string dateregiste, string daterelease)
    {
        string getCurrentObj = string.Format("select * from tblCard where CardNumber='{0}' and CustomerID = '{1}' and DateRegister='{2}' and DateRelease='{3}'", cardnumber, customerid, dateregiste, daterelease);

        DataTable CurrentObj = StaticPool.mdb.FillData(getCurrentObj);

        if (CurrentObj != null && CurrentObj.Rows.Count > 0)
        {
            return "Không thể xóa log của bản ghi hiện tại.";
        }
        else
        {
            bool Success = false;
            string reportmessage = "";

            string command = string.Format("select top 1 DateRegisted, DateReleased, DateCanceled, CardIsLock from tblLogCardCustomer where CardNumber='{0}' and CustomerID = '{1}' and CompartmentID = '{2}' and DateChanged < '{3}'", cardnumber, customerid, compartment, datechange);

            DataTable objOldRecord = StaticPool.mdb.FillData(command);

            if (objOldRecord != null && objOldRecord.Rows.Count > 0)
            {
                string updatecommand = string.Format("update tblCard set DateRegister = '{0}', DateRelease = '{1}', IsLock = {2}, DateCancel = {3}, CustomerID='{4}' where CardNumber = '{5}'", objOldRecord.Rows[0]["DateRegisted"].ToString(), objOldRecord.Rows[0]["DateReleased"].ToString(), bool.Parse(objOldRecord.Rows[0]["CardIsLock"].ToString()) ? 1 : 0, !string.IsNullOrWhiteSpace(objOldRecord.Rows[0]["DateCanceled"].ToString()) ? "'" + objOldRecord.Rows[0]["DateCanceled"].ToString() + "'" : "null", customerid, cardnumber);

                Success = StaticPool.mdb.ExecuteCommand(updatecommand, ref reportmessage);
                if (Success)
                {
                    return "OK";
                }
                else
                {
                    return reportmessage;
                }
            }

            return "OK";
        }
    }
}
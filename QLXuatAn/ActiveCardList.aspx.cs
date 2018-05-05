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
using OfficeOpenXml;

public partial class QLXuatAn_ActiveCardList : System.Web.UI.Page
{
    int findex, lindex;

    string KeyWord = "", CardGroupID = "", CustomerID = "", CustomerGroupID = "";
    string FromDate = "", ToDate = "", CaNo = "";
    DataTable dtCus = null;
    DataTable dtcustomergroup = null;
    DataTable dtcardMap = null;
    DataTable dtCusMap = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.Cookies["UserID"] != null)
                ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
            else
                ViewState["UserID"] = "";

            div_alert.Visible = false;

            dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

            //cardgroup --set cached
            DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
            if (dtCardGroup == null)
            {
                dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
                if (dtCardGroup!=null && dtCardGroup.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
            }
            if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            {
                cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                foreach (DataRow dr in dtCardGroup.Rows)
                {
                    cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                }
            }

            dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
            if (dtcustomergroup == null)
            {
                dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
                if (dtcustomergroup!=null && dtcustomergroup.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtcustomergroup, StaticCached.TimeCache);
            }
            if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
            {
                cbCustomerGroup.Items.Add(new ListItem("<<Tất cả nhóm KH>>", ""));
                foreach (DataRow dr in dtcustomergroup.Rows)
                {
                    cbCustomerGroup.Items.Add(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
                }
            }

            //customer
            // Khách hàng

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();

            }

            //DataTable dtCustomer = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomer);
            //if (dtCustomer == null)
            //{
            //    dtCustomer = StaticPool.mdb.FillData("select CustomerName, CustomerID, CustomerGroupID from tblCustomer order by SortOrder");
            //    if (dtCustomer.Rows.Count > 0)
            //        CacheLayer.Add(StaticCached.c_tblCustomer, dtCustomer, StaticCached.TimeCache);
            //}

            //if (dtCustomer != null && dtCustomer.Rows.Count > 0)
            //{
            //    var nroW = dtCustomer.Select(string.Format("CustomerGroupID = '{0}'", CustomerGroupID));
            //    cbCustomer.Items.Add(new ListItem("<< Tất cả khách hàng >>", ""));
            //    if (nroW.Any())
            //    {
            //        foreach (DataRow dr in nroW)
            //        {
            //            cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
            //        }
            //    }

            //    //if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            //    //{

            //    //}
            //    //else
            //    //{
            //    //    cbCustomer.Items.Add(new ListItem("<< Tất cả khách hàng >>", ""));
            //    //    if (dtCustomer != null && dtCustomer.Rows.Count > 0)
            //    //    {
            //    //        foreach (DataRow dr in dtCustomer.Rows)
            //    //        {
            //    //            cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
            //    //        }
            //    //    }
            //    //}
            //}

            DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
            if (dtuser == null)
            {
                dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
                if (dtuser.Rows.Count > 0)
                    CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
            }

            BindDataList();
        }
    }

    public string GetCardGroup(string CardGroupID)
    {
        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup == null)
        {
            dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            if (dtCardGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
        }
        //DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
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
        DataTable dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        if (dtuser == null)
        {
            dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
            if (dtuser.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
        }
        var gName = "";
        if (dtuser != null)
        {
            var rRow = dtuser.Select(string.Format("UserID = '{0}'", userid));
            if (rRow.Length > 0)
                gName = rRow[0]["UserName"].ToString();
        }
        return gName;
    }

    public string GetCardNo(string cardNumber)
    {
        var _cardNo = "";
        if (dtcardMap != null && dtcardMap.Rows.Count > 0)
        {
            var rRow = dtcardMap.Select(string.Format("CardNumber = '{0}'", cardNumber));
            if (rRow.Length > 0)
                _cardNo = rRow[0]["CardNo"].ToString();
        }
        return _cardNo;
    }

    public string GetCusName(string cusIs, ref string address)
    {
        var _cusName = "";
        if (dtCusMap != null && dtCusMap.Rows.Count > 0)
        {
            var rRow = dtCusMap.Select(string.Format("CustomerID = '{0}'", cusIs));
            if (rRow.Length > 0)
            {
                _cusName = rRow[0]["CustomerName"].ToString();
                address = rRow[0]["Address"].ToString();
            }
                
        }
        return _cusName;
    }

    public string GetCustomerGroupName(string gId)
    {
        var gName = "";
        if (dtcustomergroup != null)
        {
            var rRow = dtcustomergroup.Select(string.Format("CustomerGroupID = '{0}'", gId));
            if (rRow.Any())
            {
                gName = rRow[0]["CustomerGroupName"].ToString();
            }
        }
        return gName;
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Active_Card", "Deletes", "Parking"))
            {
                DataTable dt = StaticPool.mdb.FillData("select CardNumber, NewExpireDate, OldExpireDate from tblActiveCard where Id='" + id + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    string _cardnumber = dt.Rows[0]["CardNumber"].ToString();
                    string _newexpiredate = dt.Rows[0]["NewExpireDate"].ToString();
                    string _oldexpiredate = DateTime.Parse(dt.Rows[0]["OldExpireDate"].ToString()).ToString("yyyy/MM/dd");

                    DataTable dtcard = StaticPool.mdb.FillData("select CardNumber, ExpireDate from tblCard where IsDelete=0 and CardNumber='" + _cardnumber + "'");
                    if (dtcard != null && dtcard.Rows.Count > 0)
                    {
                        if (DateTime.Parse(dtcard.Rows[0]["ExpireDate"].ToString()).ToString("yyyy/MM/dd") != DateTime.Parse(_newexpiredate).ToString("yyyy/MM/dd"))
                        {
                            return "Thẻ đã được gia hạn lần nữa, không xóa được";
                        }
                        else
                        {
                            if (StaticPool.mdb.ExecuteCommand("update tblActiveCard set IsDelete=1 where ID = '" + id + "'"))
                            {
                                return StaticPool.mdb.ExecuteCommand("update tblCard set ExpireDate='" + _oldexpiredate + "' where CardNumber='" + _cardnumber + "'").ToString().ToLower();
                            }
                        }

                    }


                }

            }
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return "Error";
    }


    private void BindDataList()
    {
        try
        {
            var pageIndex = 1;
            if (Request.QueryString["Page"] != null && Request.QueryString["Page"].ToString() != "")
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }
            ViewState["Page"] = pageIndex.ToString();
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

            if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
            {
                CustomerID = Request.QueryString["CustomerID"].ToString();
                //cbCustomer.Value = CustomerID;
            }

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = CustomerGroupID;
            }

            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
                dtpFromDate.Value = FromDate;
            }
            if (Request.QueryString["cusName"] != null && Request.QueryString["cusName"].ToString() != "")
            {
                ViewState["cusName"] = Request.QueryString["cusName"].ToString();
            }
            if (Request.QueryString["ToDate"] != null && Request.QueryString["ToDate"].ToString() != "")
            {
                ToDate = Request.QueryString["ToDate"].ToString();
                dtpToDate.Value = ToDate;
            }
            if (Request.QueryString["CaNoSort"] != null && Request.QueryString["CaNoSort"].ToString() != "")
            {
                ViewState["CaNoSort"] = Request.QueryString["CaNoSort"].ToString();
                CaNo = ViewState["CaNoSort"].ToString();
            }
            else
            {
                ViewState["CaNoSort"] = "asc";
                CaNo = "";
            }

            DataTable dtCard = null;

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + "23:59:59";
            var total = 0;
            dtCard = ReportService.GetActiveCardList_2(KeyWord, CaNo, _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, pageIndex, StaticPool.pagesize, ref total);

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                var cardActiveList = "";
                var CustomerList = "";
                foreach (DataRow item in dtCard.Rows)
                {
                    cardActiveList += item["CardNumber"] + ",";
                    CustomerList += item["CustomerID"] + ",";
                }
                dtcardMap = ReportService.GetCardByCardNumber(cardActiveList);
                dtCusMap = ReportService.GetCardByCustomerId(CustomerList);

                foreach (DataRow item in dtCard.Rows)
                {
                    item["CardNo"] = GetCardNo(item["CardNumber"].ToString());
                    var _address = "";
                    item["CustomerName"] = GetCusName(item["CustomerID"].ToString(), ref _address);
                    item["CustomerAddress"] = _address;
                }

            }






            //dtCard = StaticPool.mdb.FillData("select * from tblActiveCard where IsDelete=0 and" +
            //    " Date>='" + _fromdate +
            //    "' and Date<='" + _todate +
            //    "' and CardGroupID LIKE '%" + CardGroupID +
            //    "%' and CustomerID LIKE '%" + CustomerID +
            //    "%' and CustomerGroupID LIKE '%" + CustomerGroupID +
            //    "%' and (Plate LIKE N'%" + KeyWord +
            //    "%' or CardNumber LIKE '%" + KeyWord +
            //    "%') order by Date desc");


            //// Phan trang
            //pgsource.DataSource = dtCard.DefaultView;
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


            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                id_cardlist.InnerText = "Danh sách thẻ (" + total + ")";
                StaticPool.HNGpaging(dtCard, total, StaticPool.pagesize, pager, rpt_Card);
                //Bind resulted PageSource into the Repeater
                rpt_Card.DataSource = dtCard;
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

    protected void Excel_Click(object sender, EventArgs e)
    {
        try
        {
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

            if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
            {
                CustomerID = Request.QueryString["CustomerID"].ToString();
                //cbCustomer.Value = CustomerID;
            }

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = CustomerGroupID;
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

            if (Request.QueryString["cusName"] != null && Request.QueryString["cusName"].ToString() != "")
            {
                ViewState["cusName"] = Request.QueryString["cusName"].ToString();
            }

            if (Request.QueryString["CaNoSort"] != null && Request.QueryString["CaNoSort"].ToString() != "")
            {
                CaNo = ViewState["CaNoSort"].ToString();
            }
            else
            {
                CaNo = "";
            }

            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + "23:59:59";

            var dtCard = ReportService.GetActiveCardList_2Excel(KeyWord, CaNo, _fromdate, _todate, CardGroupID, CustomerID, CustomerGroupID, pageIndex, StaticPool.pagesize);

            if (dtCard.Rows.Count > 0)
            {
                dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
                if (dtcustomergroup == null)
                {
                    dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
                    if (dtcustomergroup.Rows.Count > 0)
                        CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtcustomergroup, StaticCached.TimeCache);
                }
                foreach (DataRow item in dtCard.Rows)
                {
                    item["Nhóm thẻ"] = GetCardGroup(item["Nhóm thẻ"].ToString());
                    item["Người dùng"] = GetUserName(item["Người dùng"].ToString());
                    item["Nhóm KH"] = GetCustomerGroupName(item["Nhóm KH"].ToString());
                }
            }
            //dtCard = StaticPool.mdb.FillData("select * from tblActiveCard where IsDelete=0 and" +
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
                var _title1 = "Danh sách thẻ đã gia hạn";
                var _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;// "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
                try
                {
                    BindDataToExcel(dtCard, _title1, _title2, ViewState["UserID"].ToString());
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                    Response.Redirect("~/QLXuatAn/ActiveCardList.aspx", false);
                }
                //GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
                //GridView gv = new GridView();

                //gv.DataSource = dtCard;
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

                ////TableRow tr4 = new TableRow();
                ////TableCell cell4 = new TableCell();
                ////cell4.Controls.Add(StaticPool.CreateFooterTable());
                ////tr4.Cells.Add(cell4);


                //tb.Rows.Add(tr1);
                //tb.Rows.Add(tr2);
                //tb.Rows.Add(tr3);
                ////tb.Rows.Add(tr4);

                //gv.GridLines = GridLines.None;
                //Response.ClearContent();
                //Response.Buffer = true;
                //Response.AddHeader("content-disposition", "attachment; filename=Report_" + _title1.Replace(" ", "_").Trim() + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls");
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

        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    //--#1
    public void BindDataToExcel(DataTable _dt, string _title1, string _title2, string uId)
    {
        //Clear the response
        //Response.ClearHeaders();
        //Response.ClearContent();
        //Response.Clear();
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

    public static Stream CreateExcelFile(Stream stream = null, DataTable dt = null, DataTable dtHeader = null, string title1 = "")
    {
        using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
        {
            // Tạo author cho file Excel
            excelPackage.Workbook.Properties.Author = "Futech";
            // Tạo title cho file Excel
            excelPackage.Workbook.Properties.Title = "Báo cáo " + title1;
            // thêm tí comments vào làm màu 
            excelPackage.Workbook.Properties.Comments = "Báo cáo " + title1;
            // Add Sheet vào file Excel
            excelPackage.Workbook.Worksheets.Add(title1);
            // Lấy Sheet bạn vừa mới tạo ra để thao tác 
            var workSheet = excelPackage.Workbook.Worksheets[1];
            // Đổ data vào Excel file
            //workSheet.Cells[1, 1].LoadFromDataTable(dt, true, TableStyles.Dark9);
            StaticPool.BindingFormatForExcel(workSheet, dt, dtHeader);
            excelPackage.Save();
            return excelPackage.Stream;
        }
    }
}
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

public partial class QLXuatAn_ReportCardExpire : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
   
    string LaneID = "", UserID = "";
    string CustomerID = "";

    DataTable dtCus = null;
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

                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Report_Report6", "Selects", "Parking") == false)
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }

                div_alert.Visible = false;

                dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
                dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

                DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup where CardType=0 order by SortOrder");
                if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                {
                    cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                    foreach (DataRow dr in dtCardGroup.Rows)
                    {
                        cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                    }

                    //if (StaticPool.SystemUsingLoop() == true)
                    //{
                    //    cbCardGroup.Items.Add(new ListItem("Vòng từ-Xe lượt (Loop)", "LOOP_D"));
                    //    cbCardGroup.Items.Add(new ListItem("Vòng từ-Xe tháng (Loop)", "LOOP_M"));
                    //}
                }

                //lane
                //DataTable dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane order by SortOrder");
                //if (dtlane != null && dtlane.Rows.Count > 0)
                //{
                //    cbLane.Items.Add(new ListItem("<< Tất cả các làn >>", ""));
                //    foreach (DataRow dr in dtlane.Rows)
                //    {
                //        cbLane.Items.Add(new ListItem(dr["LaneName"].ToString(), dr["LaneID"].ToString()));
                //    }
                //}

                //user
                //DataTable dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
                //if (dtuser != null && dtuser.Rows.Count > 0)
                //{
                //    cbUser.Items.Add(new ListItem("<< Tất cả người dùng >>", ""));
                //    foreach (DataRow dr in dtuser.Rows)
                //    {
                //        cbUser.Items.Add(new ListItem(dr["UserName"].ToString(), dr["UserID"].ToString()));
                //    }
                //}

                // Khách hàng
                //DataTable dtCustomer = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomer);
                //if (dtCustomer == null)
                //{
                //    dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer order by SortOrder");
                //    if (dtCustomer.Rows.Count > 0)
                //        CacheLayer.Add(StaticCached.c_tblCustomer, dtCustomer, StaticCached.TimeCache);
                //}
                //if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                //{
                //    cbCustomer.Items.Add(new ListItem("<< Tất cả khách hàng >>", ""));
                //    foreach (DataRow dr in dtCustomer.Rows)
                //    {
                //        cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
                //    }
                //}

                BindDataList();
            }
            catch (Exception ex)
            {
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }

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

    public string GetDateTime(string dtime)
    {
        if (dtime != "")
        {
            return DateTime.Parse(dtime).ToString("dd/MM/yyyy");
        }
        return "";
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
                    string _oldexpiredate = dt.Rows[0]["OldExpireDate"].ToString();

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

            if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
            {
                CustomerID = Request.QueryString["CustomerID"].ToString();
                //cbCustomer.SelectedValue = CustomerID;
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


            //DataTable dt = new DataTable();
            //dt.Columns.Add("CardNo", typeof(string));
            //dt.Columns.Add("CardNumber", typeof(string));
            //dt.Columns.Add("Plate", typeof(string));
            //dt.Columns.Add("ExpireDate", typeof(string));
            //dt.Columns.Add("CardGroupID", typeof(string));
            //dt.Columns.Add("CustomerID", typeof(string));


            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            var totalCount = 0;
            DataTable dt = ReportService.GetReportCardExpired(KeyWord, _fromdate, _todate, CardGroupID, CustomerID, pageIndex, StaticPool.pagesize, ref totalCount);

            foreach (DataRow dr in dt.Rows)
            {
                string _plate = dr["Plate1"].ToString();
                if (dr["Plate2"].ToString() != "")
                    _plate = _plate + ";" + dr["Plate2"].ToString();
                if (dr["Plate3"].ToString() != "")
                    _plate = _plate + dr["Plate3"].ToString();
                dr["Plate"] = _plate;
            }
           

            ////card
            //DataTable dtevent = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and" +
            //    " ExpireDate>='" + _fromdate +
            //    "' and ExpireDate<='" + _todate +
            //    "' and CardGroupID LIKE '%" + CardGroupID +
            //    "' and CustomerID LIKE '%" + CustomerID +
            //    "%' and (CardNo LIKE '%" + KeyWord +
            //    "%' or CardNumber LIKE '%" + KeyWord +
            //    "%' or Plate1 LIKE '%" + KeyWord +
            //    "%' or Plate2 LIKE '%" + KeyWord +
            //    "%' or Plate3 LIKE '%" + KeyWord +
            //    "%')" +
            //    " order by ExpireDate");

            //if (dtevent != null && dtevent.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dtevent.Rows)
            //    {
            //        string _plate = dr["Plate1"].ToString();
            //        if (dr["Plate2"].ToString() != "")
            //            _plate = _plate + ";" + dr["Plate2"].ToString();
            //        if (dr["Plate3"].ToString() != "")
            //            _plate = _plate + dr["Plate3"].ToString();
            //        dt.Rows.Add(dr["CardNo"].ToString(),
            //            dr["CardNumber"].ToString(),
            //            _plate,
            //            dr["ExpireDate"].ToString(),
            //            dr["CardGroupID"].ToString(),
            //            dr["CustomerID"].ToString());

            //    }
            //}



            if (dt == null)
                return;

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
                id_cardlist.InnerText = "Số bản ghi (" + totalCount + ")";
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

    //private void doPaging()
    //{
    //    DataTable dt = new DataTable("Paging");
    //    //Add two column into the DataTable "dt" 
    //    //First Column store page index default it start from "0"
    //    //Second Column store page index default it start from "1"
    //    dt.Columns.Add("PageIndex");
    //    dt.Columns.Add("PageText");

    //    //Assign First Index starts from which number in paging data list
    //    findex = CurrentPage - 3;

    //    //Set Last index value if current page less than 5 then last index added "5" values to the Current page else it set "10" for last page number
    //    if (CurrentPage > 3)
    //    {
    //        lindex = CurrentPage + 3;
    //    }
    //    else
    //    {
    //        lindex = 6;
    //    }

    //    //Check last page is greater than total page then reduced it to total no. of page is last index
    //    if (lindex > Convert.ToInt32(ViewState["totpage"]))
    //    {
    //        lindex = Convert.ToInt32(ViewState["totpage"]);
    //        findex = lindex - 6;
    //    }

    //    if (findex < 0)
    //    {
    //        findex = 0;
    //    }

    //    //Now creating page number based on above first and last page index
    //    for (int i = findex; i < lindex; i++)
    //    {
    //        DataRow dr = dt.NewRow();
    //        dr[0] = i;
    //        dr[1] = i + 1;
    //        dt.Rows.Add(dr);
    //    }

    //    //Finally bind it page numbers in to the Paging DataList "RepeaterPaging"
    //    RepeaterPaging.DataSource = dt;
    //    RepeaterPaging.DataBind();
    //}

    //protected void lnkFirst_Click(object sender, EventArgs e)
    //{
    //    //If user click First Link button assign current index as Zero "0" then refresh "Repeater1" Data.
    //    CurrentPage = 0;
    //    BindDataList();
    //}
    //protected void lnkLast_Click(object sender, EventArgs e)
    //{
    //    //If user click Last Link button assign current index as totalpage then refresh "Repeater1" Data.
    //    CurrentPage = (Convert.ToInt32(ViewState["totpage"]) - 1);
    //    BindDataList();
    //}
    //protected void lnkPrevious_Click(object sender, EventArgs e)
    //{
    //    //If user click Previous Link button assign current index as -1 it reduce existing page index.
    //    CurrentPage -= 1;
    //    //refresh "Repeater1" Data
    //    BindDataList();
    //}
    //protected void lnkNext_Click(object sender, EventArgs e)
    //{
    //    //If user click Next Link button assign current index as +1 it add one value to existing page index.
    //    CurrentPage += 1;

    //    //refresh "Repeater1" Data
    //    BindDataList();
    //}
    //protected void RepeaterPaging_ItemCommand(object source, DataListCommandEventArgs e)
    //{
    //    if (e.CommandName.Equals("newpage"))
    //    {
    //        //Assign CurrentPage number when user click on the page number in the Paging "RepeaterPaging" DataList
    //        CurrentPage = Convert.ToInt32(e.CommandArgument.ToString());
    //        //Refresh "Repeater1" control Data once user change page
    //        BindDataList();
    //    }
    //}
    //protected void RepeaterPaging_ItemDataBound(object sender, DataListItemEventArgs e)
    //{
    //    //Enabled False for current selected Page index
    //    LinkButton lnkPage = (LinkButton)e.Item.FindControl("Pagingbtn");
    //    if (lnkPage.CommandArgument.ToString() == CurrentPage.ToString())
    //    {
    //        lnkPage.Enabled = false;
    //        lnkPage.BackColor = System.Drawing.Color.FromName("#FFCC01");
    //    }
    //}

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

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"].ToString();
                cbCardGroup.Value = CardGroupID;
            }

            if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
            {
                CustomerID = Request.QueryString["CustomerID"].ToString();
               //cbCustomer.SelectedValue = CustomerID;
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
            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("Mã thẻ", typeof(string));
            dt.Columns.Add("Biển số", typeof(string));
            dt.Columns.Add("Ngày hết hạn", typeof(string));
            dt.Columns.Add("Nhóm thẻ", typeof(string));
            dt.Columns.Add("Khách hàng", typeof(string));


            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); //dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            DataTable dtevent = ReportService.GetReportCardExpiredExcel(KeyWord, _fromdate, _todate, CardGroupID, CustomerID, pageIndex, StaticPool.pagesize);
            //card
            //DataTable dtevent = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and" +
            //    " ExpireDate>='" + _fromdate +
            //    "' and ExpireDate<='" + _todate +
            //    "' and CardGroupID LIKE '%" + CardGroupID +
            //    "' and CustomerID LIKE '%" + CustomerID +
            //    "%' and (CardNo LIKE '%" + KeyWord +
            //    "%' or CardNumber LIKE '%" + KeyWord +
            //    "%' or Plate1 LIKE '%" + KeyWord +
            //    "%' or Plate2 LIKE '%" + KeyWord +
            //    "%' or Plate3 LIKE '%" + KeyWord +
            //    "%')" +
            //    " order by ExpireDate");

            //if (dtevent != null && dtevent.Rows.Count > 0)
            //{
            //    var listCusId = "";
            //    foreach (DataRow item in dtevent.Rows)
            //    {
            //        listCusId += item["CustomerID"].ToString() + ",";
            //    }
            //    dtCus = ReportService.GetCustomerBylistId(listCusId);

            //    foreach (DataRow dr in dtevent.Rows)
            //    {
            //        string _plate = dr["Plate1"].ToString();
            //        if (dr["Plate2"].ToString() != "")
            //            _plate = _plate + ";" + dr["Plate2"].ToString();
            //        if (dr["Plate3"].ToString() != "")
            //            _plate = _plate + dr["Plate3"].ToString();
            //        dt.Rows.Add(
            //            dt.Rows.Count+1,
            //            dr["CardNo"].ToString(),
            //            dr["CardNumber"].ToString(),
            //            _plate,
            //            this.GetDateTime(dr["ExpireDate"].ToString()),
            //            this.GetCardGroup(dr["CardGroupID"].ToString()),
            //            this.GetCustomerName(dr["CustomerID"].ToString()));

            //    }
            //}

            string _title1 = "Thời hạn thẻ tháng";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dtevent, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportCardExpire.aspx", false);
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
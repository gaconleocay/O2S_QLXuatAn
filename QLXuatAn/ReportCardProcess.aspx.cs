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

    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
    string Actions = "";
    string UserID = "";

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
            DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            {
                cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                foreach (DataRow dr in dtCardGroup.Rows)
                {
                    cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                }


            }

            //action


            //user

            // Khách hàng
            //DataTable dtCustomer = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomer);
            //if (dtCustomer == null)
            //{
            //    dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer order by SortOrder");
            //    if (dtCustomer.Rows.Count > 0)
            //        CacheLayer.Add(StaticCached.c_tblCustomer, dtCustomer, StaticCached.TimeCache);
            //}


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

    //public string GetCustomerName(string customerid)
    //{
    //    if (string.IsNullOrWhiteSpace(customerid))
    //    {
    //        return "";
    //    }

    //    DataTable dt = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomer);
    //    var gName = "";
    //    if (dt != null)
    //    {
    //        var rRow = dt.Select(string.Format("CustomerID = '{0}'", customerid));
    //        if (rRow.Any())
    //        {
    //            gName = rRow[0]["CustomerName"].ToString();
    //        }
    //    }
    //    return gName;
    //}

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


    private void BindDataList()
    {
        try
        {

          

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

          

          

            DataTable dt = new DataTable();
            dt.Columns.Add("CardGroupID", typeof(string));
            dt.Columns.Add("ADD", typeof(string));
            dt.Columns.Add("RELEASE", typeof(string));
            dt.Columns.Add("CHANGE", typeof(string));
            dt.Columns.Add("RETURN", typeof(string));
            dt.Columns.Add("LOCK", typeof(string));
            dt.Columns.Add("UNLOCK", typeof(string));
            dt.Columns.Add("DELETE", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            DataTable dtevent = null;
            DataTable dtcardgroup = StaticPool.mdb.FillData("select CardGroupID from tblCardGroup where Inactive=0" +
                " and CardGroupID LIKE '%" + CardGroupID +
                "%' order by SortOrder");
            if (dtcardgroup != null && dtcardgroup.Rows.Count > 0)
            {
                foreach (DataRow dr in dtcardgroup.Rows)
                {
                    int _add = 0;
                    int _release = 0;
                    int _change = 0;
                    int _return = 0;
                    int _lock = 0;
                    int _unlock = 0;
                    int _delete = 0;

                    DataTable temp = StaticPool.mdb.FillData("select * from tblCardProcess where" +
                        " Date>='" + _fromdate +
                        "' and Date<='" + _todate +
                        "' and CardGroupID='" + dr["CardGroupID"].ToString() +
                        "'");
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        foreach (DataRow dtr in temp.Rows)
                        {
                            if (dtr["Actions"].ToString() == "ADD")
                                _add++;
                            else if (dtr["Actions"].ToString() == "RELEASE")
                                _release++;
                            else if (dtr["Actions"].ToString() == "CHANGE")
                                _change++;
                            else if (dtr["Actions"].ToString() == "RETURN")
                                _return++;
                            else if (dtr["Actions"].ToString() == "LOCK")
                                _lock++;
                            else if (dtr["Actions"].ToString() == "UNLOCK")
                                _unlock++;
                            else if (dtr["Actions"].ToString() == "DELETE")
                                _delete++;
                        }
                    }

                    dt.Rows.Add(dr["CardGroupID"].ToString(), _add, _release, _change, _return, _lock, _unlock, _delete);
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
                id_reportin.InnerText = "Số bản ghi (" + dt.Rows.Count + ")";
                //Bind resulted PageSource into the Repeater
                rpt_Card.DataSource = pgsource;
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





            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("Nhóm thẻ", typeof(string));
            dt.Columns.Add("Thêm", typeof(string));
            dt.Columns.Add("Phát", typeof(string));
            dt.Columns.Add("Đổi", typeof(string));
            dt.Columns.Add("Trả", typeof(string));
            dt.Columns.Add("Khóa", typeof(string));
            dt.Columns.Add("Mở", typeof(string));
            dt.Columns.Add("Xóa", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            //DataTable dtevent = null;
            DataTable dtcardgroup = StaticPool.mdb.FillData("select CardGroupID from tblCardGroup where Inactive=0" +
                " and CardGroupID LIKE '%" + CardGroupID +
                "%' order by SortOrder");
            if (dtcardgroup != null && dtcardgroup.Rows.Count > 0)
            {
                foreach (DataRow dr in dtcardgroup.Rows)
                {
                    int _add = 0;
                    int _release = 0;
                    int _change = 0;
                    int _return = 0;
                    int _lock = 0;
                    int _unlock = 0;
                    int _delete = 0;

                    DataTable temp = StaticPool.mdb.FillData("select * from tblCardProcess where" +
                        " Date>='" + _fromdate +
                        "' and Date<='" + _todate +
                        "' and CardGroupID='" + dr["CardGroupID"].ToString() +
                        "'");
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        foreach (DataRow dtr in temp.Rows)
                        {
                            if (dtr["Actions"].ToString() == "ADD")
                                _add++;
                            else if (dtr["Actions"].ToString() == "RELEASE")
                                _release++;
                            else if (dtr["Actions"].ToString() == "CHANGE")
                                _change++;
                            else if (dtr["Actions"].ToString() == "RETURN")
                                _return++;
                            else if (dtr["Actions"].ToString() == "LOCK")
                                _lock++;
                            else if (dtr["Actions"].ToString() == "UNLOCK")
                                _unlock++;
                            else if (dtr["Actions"].ToString() == "DELETE")
                                _delete++;
                        }
                    }

                    dt.Rows.Add(dt.Rows.Count + 1, this.GetCardGroup(dr["CardGroupID"].ToString()), _add, _release, _change, _return, _lock, _unlock, _delete);
                }
            }

            string _title1 = "Báo cáo tổng hợp xử lý thẻ";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dt, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportCardProcess.aspx", false);
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
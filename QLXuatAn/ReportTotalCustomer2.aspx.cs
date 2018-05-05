using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Text;

public partial class QLXuatAn_ReportTotalCustomer2 : System.Web.UI.Page
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

                div_alert.Visible = false;

                dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
                dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

                DataTable ddlCompartment = StaticPool.mdb.FillData("select * from tblCompartment");
                if (ddlCompartment != null && ddlCompartment.Rows.Count > 0)
                {
                    cbCompartment.Items.Add(new ListItem("<< Tất cả căn hộ >>", ""));
                    foreach (DataRow dr in ddlCompartment.Rows)
                    {
                        cbCompartment.Items.Add(new ListItem(dr["CompartmentName"].ToString(), dr["CompartmentID"].ToString()));
                    }
                }


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

    public string GetDateTime(string dtime)
    {
        if (dtime != "")
        {
            return DateTime.Parse(dtime).ToString("dd/MM/yyyy");
        }
        return "";
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

            DataTable dt = new DataTable();
            dt.Columns.Add("CompartmentName", typeof(string));
            dt.Columns.Add("CountRegistedBicycle", typeof(string));
            dt.Columns.Add("CountLockBicycle", typeof(string));
            dt.Columns.Add("CountUseBicycle", typeof(string));
            dt.Columns.Add("CountRegistedMotorcycle", typeof(string));
            dt.Columns.Add("CountLockMotorcycle", typeof(string));
            dt.Columns.Add("CountUseMotorcycle", typeof(string));
            dt.Columns.Add("CountRegistedCar", typeof(string));
            dt.Columns.Add("CountLockCar", typeof(string));
            dt.Columns.Add("CountUseCar", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            //card
            DataTable dtevent = null;
            string countRegistedBicycle = "";
            string countLockBicycle = "";
            string countUseBicycle = "";
            string countRegistedMotorcycle = "";
            string countLockMotorcycle = "";
            string countUseMotorcycle = "";
            string countRegistedCar = "";
            string countLockCar = "";
            string countUseCar = "";

            if (IsFilterByTimeRegister == true)
            {
                //Bicycle
                GetCommandCountByDateRegister(_fromdate, _todate, ref countRegistedBicycle, ref countLockBicycle, ref countUseBicycle, ref countRegistedMotorcycle, ref countLockMotorcycle, ref countUseMotorcycle, ref countRegistedCar, ref countLockCar, ref countUseCar);


            }
            else
            {
                GetCommandCountByDateRelease(_fromdate, _todate, ref countRegistedBicycle, ref countLockBicycle, ref countUseBicycle, ref countRegistedMotorcycle, ref countLockMotorcycle, ref countUseMotorcycle, ref countRegistedCar, ref countLockCar, ref countUseCar);
            }

            string command = string.Format(" select distinct cm.CompartmentName, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} from tblCompartment cm where cm.CompartmentID LIKE '%{9}%'", countRegistedBicycle, countLockBicycle, countUseBicycle, countRegistedMotorcycle, countLockMotorcycle, countUseMotorcycle, countRegistedCar, countLockCar, countUseCar, CompartmentID);

            dtevent = StaticPool.mdb.FillData(command);
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    string CompartmentName = dr["CompartmentName"].ToString();

                    string CountRegistedBicycle = dr["CountRegistedBicycle"].ToString();
                    string CountLockBicycle = dr["CountLockBicycle"].ToString();
                    string CountUseBicycle = Convert.ToString(int.Parse(dr["CountRegistedBicycle"].ToString()) - int.Parse(dr["CountLockBicycle"].ToString()));

                    string CountRegistedMotorcycle = dr["CountRegistedMotorcycle"].ToString();
                    string CountLockMotorcycle = dr["CountLockMotorcycle"].ToString();
                    string CountUseMotorcycle = Convert.ToString(int.Parse(dr["CountRegistedMotorcycle"].ToString()) - int.Parse(dr["CountLockMotorcycle"].ToString()));

                    string CountRegistedCar = dr["CountRegistedCar"].ToString();
                    string CountLockCar = dr["CountLockCar"].ToString();
                    string CountUseCar = Convert.ToString(int.Parse(dr["CountRegistedCar"].ToString()) - int.Parse(dr["CountLockCar"].ToString()));

                    dt.Rows.Add(CompartmentName, CountRegistedBicycle, CountLockBicycle, CountUseBicycle, CountRegistedMotorcycle, CountLockMotorcycle, CountUseMotorcycle, CountRegistedCar, CountLockCar, CountUseCar);
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

            DataTable dt = new DataTable();
            dt.Columns.Add("Căn hộ", typeof(string));
            dt.Columns.Add("Ôtô đăng ký", typeof(string));
            dt.Columns.Add("Ôtô hủy", typeof(string));
            dt.Columns.Add("Ôtô sử dụng", typeof(string));
            dt.Columns.Add("Xe máy đăng ký", typeof(string));
            dt.Columns.Add("Xe máy hủy", typeof(string));
            dt.Columns.Add("Xe máy sử dụng", typeof(string));
            dt.Columns.Add("Xe đạp đăng ký", typeof(string));
            dt.Columns.Add("Xe đạp hủy", typeof(string));
            dt.Columns.Add("Xe đạp sử dụng", typeof(string));

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            //card
            DataTable dtevent = null;
            string countRegistedBicycle = "";
            string countLockBicycle = "";
            string countUseBicycle = "";
            string countRegistedMotorcycle = "";
            string countLockMotorcycle = "";
            string countUseMotorcycle = "";
            string countRegistedCar = "";
            string countLockCar = "";
            string countUseCar = "";

            if (IsFilterByTimeRegister == true)
            {
                //Bicycle
                GetCommandCountByDateRegister(_fromdate, _todate, ref countRegistedBicycle, ref countLockBicycle, ref countUseBicycle, ref countRegistedMotorcycle, ref countLockMotorcycle, ref countUseMotorcycle, ref countRegistedCar, ref countLockCar, ref countUseCar);


            }
            else
            {
                GetCommandCountByDateRelease(_fromdate, _todate, ref countRegistedBicycle, ref countLockBicycle, ref countUseBicycle, ref countRegistedMotorcycle, ref countLockMotorcycle, ref countUseMotorcycle, ref countRegistedCar, ref countLockCar, ref countUseCar);
            }

            string command = string.Format(" select distinct cm.CompartmentName, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8} from tblCompartment cm where cm.CompartmentID LIKE '%{9}%'", countRegistedBicycle, countLockBicycle, countUseBicycle, countRegistedMotorcycle, countLockMotorcycle, countUseMotorcycle, countRegistedCar, countLockCar, countUseCar, CompartmentID);

            dtevent = StaticPool.mdb.FillData(command);
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    string CompartmentName = dr["CompartmentName"].ToString();

                    string CountRegistedBicycle = dr["CountRegistedBicycle"].ToString();
                    string CountLockBicycle = dr["CountLockBicycle"].ToString();
                    string CountUseBicycle = Convert.ToString(int.Parse(dr["CountRegistedBicycle"].ToString()) - int.Parse(dr["CountLockBicycle"].ToString()));

                    string CountRegistedMotorcycle = dr["CountRegistedMotorcycle"].ToString();
                    string CountLockMotorcycle = dr["CountLockMotorcycle"].ToString();
                    string CountUseMotorcycle = Convert.ToString(int.Parse(dr["CountRegistedMotorcycle"].ToString()) - int.Parse(dr["CountLockMotorcycle"].ToString()));

                    string CountRegistedCar = dr["CountRegistedCar"].ToString();
                    string CountLockCar = dr["CountLockCar"].ToString();
                    string CountUseCar = Convert.ToString(int.Parse(dr["CountRegistedCar"].ToString()) - int.Parse(dr["CountLockCar"].ToString()));

                    dt.Rows.Add(CompartmentName, CountRegistedCar, CountLockCar, CountUseCar, CountRegistedMotorcycle, CountLockMotorcycle, CountUseMotorcycle, CountRegistedBicycle, CountLockBicycle, CountUseBicycle);
                }
            }

            if (dt == null)
                return;

            string _title1 = "Báo cáo tổ hợp sử dụng thẻ theo căn hộ";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dt, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportTotalCustomer2.aspx", false);
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

    private void GetInfoFromCustomer(string customerId, ref string compartmentname, ref string customername, ref string customergroupname, ref string customercode)
    {
        DataTable dtCustomer = StaticPool.mdb.FillData("select CustomerCode, CustomerName, CompartmentId, CustomerGroupID from tblCustomer where CustomerID = '" + customerId + "'");
        customercode = dtCustomer.Rows[0]["CustomerCode"].ToString();

        customername = dtCustomer.Rows[0]["CustomerName"].ToString();

        compartmentname = GetCompartment(dtCustomer.Rows[0]["CompartmentId"].ToString());

        customergroupname = GetCustomerGroup(dtCustomer.Rows[0]["CustomerGroupID"].ToString());
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

    public void GetCommandCountByDateRegister(string _fromdate, string _todate, ref string registerbicycle, ref string lockbicycle, ref string usebicycle, ref string registermotor, ref string lockmotor, ref string usemotor, ref string registercar, ref string lockcar, ref string usecar)
    {
        //Bicycle
        registerbicycle = GenerateRegisterVehicleByRegisterDay("2", _fromdate, _todate, "CountRegistedBicycle");

        lockbicycle = GenerateLockVehicleByRegisterDay("2", _fromdate, _todate, "CountLockBicycle"); 

        usebicycle = "0";

        //Motor
        registermotor = GenerateRegisterVehicleByRegisterDay("1", _fromdate, _todate, "CountRegistedMotorcycle"); 

        lockmotor = GenerateLockVehicleByRegisterDay("1", _fromdate, _todate, "CountLockMotorcycle"); 

        usemotor = "0";

        //Car
        registercar = GenerateRegisterVehicleByRegisterDay("0", _fromdate, _todate, "CountRegistedCar"); 

        lockcar = GenerateLockVehicleByRegisterDay("0", _fromdate, _todate, "CountLockCar"); 

        usecar = "0";
    }

    public void GetCommandCountByDateRelease(string _fromdate, string _todate, ref string registerbicycle, ref string lockbicycle, ref string usebicycle, ref string registermotor, ref string lockmotor, ref string usemotor, ref string registercar, ref string lockcar, ref string usecar)
    {
        //Bicycle
        registerbicycle = GenerateRegisterVehicleByReleaseDay("2", _fromdate, _todate, "CountRegistedBicycle");

        lockbicycle = GenerateLockVehicleByReleaseDay("2", _fromdate, _todate, "CountLockBicycle");

        usebicycle = "0";

        //Motor
        registermotor = GenerateRegisterVehicleByReleaseDay("1", _fromdate, _todate, "CountRegistedMotorcycle");

        lockmotor = GenerateLockVehicleByReleaseDay("1", _fromdate, _todate, "CountLockMotorcycle");

        usemotor = "0";

        //Car
        registercar = GenerateRegisterVehicleByReleaseDay("0", _fromdate, _todate, "CountRegistedCar");

        lockcar = GenerateLockVehicleByReleaseDay("0", _fromdate, _todate, "CountLockCar");

        usecar = "0";
    }

    public string GenerateRegisterVehicleByRegisterDay(string vehicletype, string _fromdate, string _todate, string name)
    {
        var type = "";
        if (vehicletype.Equals("0"))
        {
            type = " AND (vg.VehicleType = '0')";
        }
        else if(vehicletype.Equals("1"))
        {
            type = " AND (vg.VehicleType = '1')";
        }
        else if (vehicletype.Equals("2") || vehicletype.Equals("3"))
        {
            type = " AND (vg.VehicleType = '2' OR vg.VehicleType = '3')";
        }

        string format = string.Format("(SELECT COUNT(*) FROM( SELECT lcc.CardNumber, lcc.CardIsLock, lcc.CustomerID, ROW_NUMBER() OVER( PARTITION BY lcc.CardNumber ORDER BY lcc.DateChanged DESC) AS RN FROM tblLogCardCustomer lcc INNER JOIN tblCard cd on lcc.CardNumber = cd.CardNumber INNER JOIN tblCardGroup cg on lcc.CardGroupID = CONVERT(varchar(50), cg.CardGroupID) INNER JOIN tblVehicleGroup vg on cg.VehicleGroupID = CONVERT(varchar(50), vg.VehicleGroupID) WHERE cd.IsDelete = 0 AND lcc.CompartmentID = cm.CompartmentID {0} AND lcc.DateRegisted >= '{1}' AND lcc.DateRegisted <= '{2}') AS a WHERE RN = 1 AND CustomerID <> '') AS {3}", type, _fromdate, _todate, name);

        return format;
    }

    public string GenerateLockVehicleByRegisterDay(string vehicletype, string _fromdate, string _todate, string name)
    {
        var type = "";
        if (vehicletype.Equals("0"))
        {
            type = "AND (vg.VehicleType = '0')";
        }
        else if (vehicletype.Equals("1"))
        {
            type = "AND (vg.VehicleType = '1')";
        }
        else if (vehicletype.Equals("2") || vehicletype.Equals("3"))
        {
            type = "AND (vg.VehicleType = '2' OR vg.VehicleType = '3')";
        }

        string format = string.Format("(SELECT COUNT(*) FROM( SELECT lcc.CardNumber, lcc.CardIsLock, lcc.CustomerID, ROW_NUMBER() OVER( PARTITION BY lcc.CardNumber ORDER BY lcc.DateChanged DESC) AS RN FROM tblLogCardCustomer lcc INNER JOIN tblCard cd on lcc.CardNumber = cd.CardNumber INNER JOIN tblCardGroup cg on lcc.CardGroupID = CONVERT(varchar(50), cg.CardGroupID) INNER JOIN tblVehicleGroup vg on cg.VehicleGroupID = CONVERT(varchar(50), vg.VehicleGroupID) WHERE cd.IsDelete = 0 AND lcc.CompartmentID = cm.CompartmentID {0} AND lcc.DateReleased >= '{1}' AND lcc.DateReleased <= '{2}') AS a WHERE RN = 1 AND CustomerID <> '' AND  AND CardIsLock = 1) AS {3}", type, _fromdate, _todate, name);

        return format;
    }

    public string GenerateRegisterVehicleByReleaseDay(string vehicletype, string _fromdate, string _todate, string name)
    {
        var type = "";
        if (vehicletype.Equals("0"))
        {
            type = "AND (vg.VehicleType = '0')";
        }
        else if (vehicletype.Equals("1"))
        {
            type = "AND (vg.VehicleType = '1')";
        }
        else if (vehicletype.Equals("2") || vehicletype.Equals("3"))
        {
            type = "AND (vg.VehicleType = '2' OR vg.VehicleType = '3')";
        }

        string format = string.Format("(SELECT COUNT(*) FROM( SELECT lcc.CardNumber, lcc.CardIsLock, lcc.CustomerID, ROW_NUMBER() OVER( PARTITION BY lcc.CardNumber ORDER BY lcc.DateChanged DESC) AS RN FROM tblLogCardCustomer lcc INNER JOIN tblCard cd on lcc.CardNumber = cd.CardNumber INNER JOIN tblCardGroup cg on lcc.CardGroupID = CONVERT(varchar(50), cg.CardGroupID) INNER JOIN tblVehicleGroup vg on cg.VehicleGroupID = CONVERT(varchar(50), vg.VehicleGroupID) WHERE cd.IsDelete = 0 AND lcc.CompartmentID = cm.CompartmentID {0} AND lcc.DateReleased >= '{1}' AND lcc.DateReleased <= '{2}') AS a WHERE RN = 1 AND CustomerID <> '') AS {3}", type, _fromdate, _todate, name);

        return format;
    }

    public string GenerateLockVehicleByReleaseDay(string vehicletype, string _fromdate, string _todate, string name)
    {
        var type = "";
        if (vehicletype.Equals("0"))
        {
            type = "AND (vg.VehicleType = '0')";
        }
        else if (vehicletype.Equals("1"))
        {
            type = "AND (vg.VehicleType = '1')";
        }
        else if (vehicletype.Equals("2") || vehicletype.Equals("3"))
        {
            type = "AND (vg.VehicleType = '2' OR vg.VehicleType = '3')";
        }

        string format = string.Format("(SELECT COUNT(*) FROM( SELECT lcc.CardNumber, lcc.CardIsLock, lcc.CustomerID, ROW_NUMBER() OVER( PARTITION BY lcc.CardNumber ORDER BY lcc.DateChanged DESC) AS RN FROM tblLogCardCustomer lcc INNER JOIN tblCard cd on lcc.CardNumber = cd.CardNumber INNER JOIN tblCardGroup cg on lcc.CardGroupID = CONVERT(varchar(50), cg.CardGroupID) INNER JOIN tblVehicleGroup vg on cg.VehicleGroupID = CONVERT(varchar(50), vg.VehicleGroupID) WHERE cd.IsDelete = 0 AND lcc.CompartmentID = cm.CompartmentID {0} AND lcc.DateReleased >= '{1}' AND lcc.DateReleased <= '{2}') AS a WHERE RN = 1 AND CustomerID <> '' AND CardIsLock = 1) AS {3}", type, _fromdate, _todate, name);

        return format;
    }
}
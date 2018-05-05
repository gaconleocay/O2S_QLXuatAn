using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using System.Web.Services;

public partial class QLXuatAn_Compartment : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();

    private string codeTable = "CustomerGroup";
    int findex, lindex;
    string KeyWord = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                // Check xem nguoi dung nay co quyen truy cap chuc nang nay khong
                if (Request.Cookies["UserID"] != null)
                {
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                }
                else 
                {
                    ViewState["UserID"] = "";
                
                }
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_List_Compartment", "Selects", "Parking"))
                {
                    //rpt_CustomerGroup.DataSource = StaticPool.mdb.FillData("select * from tblCustomerGroup");
                    //rpt_CustomerGroup.DataBind();
                    //DisplayCompartment();
                    BindDataList();
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
                //BindDataList();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
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

            DataTable dtCompartment = new DataTable();

            dtCompartment.Columns.Add("CompartmentID", typeof(string));
            dtCompartment.Columns.Add("CompartmentName", typeof(string));

            DataTable dt = StaticPool.mdb.FillData(string.Format("select * from tblCompartment where CompartmentName LIKE '%{0}%' order by SortOrder asc", KeyWord));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dtCompartment.Rows.Add(
                                   dr["CompartmentID"].ToString(),
                                   dr["CompartmentName"].ToString()
                                   );
                }
            }


            if (dtCompartment == null)
                return;

            // Phan trang
            pgsource.DataSource = dtCompartment.DefaultView;
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

            //if (dtCustomer != null && dtCustomer.Rows.Count <= pgsource.PageSize)
            //     frm1.Visible = false;
            // else
            //     frm1.Visible = true;

            if (dtCompartment != null && dtCompartment.Rows.Count > 0)
            {
                //Bind resulted PageSource into the Repeater
                rpt_Compartment.DataSource = pgsource;
                rpt_Compartment.DataBind();
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

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_List_Compartment", "Deletes", "Parking") == false)
                return "Bạn không có quyền thực hiện chức năng này!";

            DataTable temp = StaticPool.mdb.FillData("select * from tblCompartment where CompartmentID='" + id + "'");

            DataTable objExist = StaticPool.mdb.FillData("select top 1 CompartmentId from tblCustomer where CompartmentId='" + id + "'");
            if (objExist !=null && objExist.Rows.Count > 0)
            {
                return "Căn đã được gắn cho khách hàng. Không thể xóa";
            }

            objExist = StaticPool.mdb.FillData("select top 1 CompartmentID from tblLogCardCustomer where CompartmentID='" + id + "'");
            if (objExist != null && objExist.Rows.Count > 0)
            {
                return "Căn đã có trong báo cáo. Không thể xóa";
            }

            if (StaticPool.mdb.ExecuteCommand("delete from tblCompartment where CompartmentID = '" + id + "'"))
            {
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_List_Compartment", temp.Rows[0]["CompartmentName"].ToString(), "Xóa", "id=" + id);
                return "true";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
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

            DataTable dtCompartment = new DataTable();

            dtCompartment.Columns.Add("CompartmentID", typeof(string));
            dtCompartment.Columns.Add("CompartmentName", typeof(string));

            DataTable dt = StaticPool.mdb.FillData(string.Format("select * from tblCompartment where CompartmentName LIKE '%{0}%' order by SortOrder asc", KeyWord));

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dtCompartment.Rows.Add(
                                   dr["CompartmentID"].ToString(),
                                   dr["CompartmentName"].ToString()
                                   );
                }
            }


            if (dtCompartment == null)
                return;

            string _title1 = "Danh sách căn hộ";
            string _title2 = "";// "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;

            GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
            GridView gv = new GridView();

            gv.DataSource = dtCompartment;
            gv.DataBind();

            Table tb = new Table();
            TableRow tr1 = new TableRow();
            TableCell cell1 = new TableCell();
            cell1.Controls.Add(gvheader);
            tr1.Cells.Add(cell1);

            TableCell cell3 = new TableCell();
            cell3.Controls.Add(gv);

            TableCell cell2 = new TableCell();
            TableRow tr2 = new TableRow();
            tr2.Cells.Add(cell2);

            TableRow tr3 = new TableRow();
            tr3.Cells.Add(cell3);

            //TableRow tr4 = new TableRow();
            //TableCell cell4 = new TableCell();
            //cell4.Controls.Add(StaticPool.CreateFooterTable());
            //tr4.Cells.Add(cell4);


            tb.Rows.Add(tr1);
            tb.Rows.Add(tr2);
            tb.Rows.Add(tr3);
            //tb.Rows.Add(tr4);

            gv.GridLines = GridLines.None;
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/excel";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htm = new HtmlTextWriter(sw);
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            //StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(htm);
            htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            string style = @"<style> TD { mso-number-format:\@; } </style>";
            Response.Write(style);
            tb.RenderControl(htm);

            Response.Write(sw.ToString());

            HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
            HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }

    }
}
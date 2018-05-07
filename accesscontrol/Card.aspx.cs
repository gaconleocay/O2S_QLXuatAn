using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class accesscontrol_Card : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "", VehicleGroupID = "", CustomerID = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                // Check xem nguoi dung nay co quyen truy cap chuc nang nay khong
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "AccessControl_Card", "Selects", "AccessControl"))
                {
                    div_alert.Visible = false;
                    id_alert.InnerText = "";

                    // Nhom the
                    DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup");
                    if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                    {
                        cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                        foreach (DataRow dr in dtCardGroup.Rows)
                        {
                            cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                        }
                    }

                    // Khách hàng
                    DataTable dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer order by SortOrder");
                    if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                    {
                        cbCustomer.Items.Add(new ListItem("<< Tất cả khách hàng >>", ""));
                        foreach (DataRow dr in dtCustomer.Rows)
                        {
                            cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
                        }
                    }

                    //rpt_Card.DataSource = StaticPool.mdb.FillData("select * from tblCard order by SortOrder");
                    //rpt_Card.DataBind();
                    BindDataList();
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }
        }
    }

    public string GetCardGroup(string CardGroupID)
    {
        DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            return dtCardGroup.Rows[0]["CardGroupName"].ToString();
        else
            return "";
    }

    public string GetExpireDate(string ExpireDate)
    {
        if (ExpireDate == "")
            return "";
        else
        {
            if (DateTime.Parse(ExpireDate) < DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")))
                return "<span class='label label-sm label-danger'>" + DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy") + "</span>";
            else if (DateTime.Parse(ExpireDate) == DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")))
                return "<span class='label label-sm label-warning'>" + DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy") + "</span>";
            else
                return DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy");
        }
    }

    public string GetCardStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Đã khóa thẻ</span>";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "AccessControl_Card", "Deletes", "AccessControl"))
                return StaticPool.mdb.ExecuteCommand("delete from tblCard where CardID = '" + id + "'").ToString().ToLower();
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private void BindDataList()
    {
        try
        {
            DataTable dtCard = null;

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
                cbCustomer.Value = CustomerID;
            }

            dtCard = StaticPool.mdb.FillData("select * from tblCard where (CardNo LIKE N'%" + KeyWord + "%' or CardNumber LIKE N'%" + KeyWord + "%') and CardGroupID LIKE '%" + CardGroupID + "%' and CustomerID LIKE '%" + CustomerID + "%' order by SortOrder desc");

            // Phan trang
            pgsource.DataSource = dtCard.DefaultView;
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

            if (dtCard != null && dtCard.Rows.Count <= pgsource.PageSize)
                frm1.Visible = false;
            else
                frm1.Visible = true;

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                id_cardlist.InnerText = "Danh sách thẻ (" + dtCard.Rows.Count + ")";
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

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using System.Xml;

public partial class accesscontrol_Customer : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CustomerGroupID = "";

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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "AccessControl_Customer", "Selects", "AccessControl"))
                {
                    DisplayCustomerGroup();

                    //rpt_Customer.DataSource = StaticPool.mdb.FillData("select * from tblCustomer order by SortOrder");
                    //rpt_Customer.DataBind();
                    BindDataList();
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Message.aspx?Message=" + ex.Message);
            }
        }
    }

    private void CreateXmlDocumentFromDataTable(DataTable dt, string parentID, XmlNode parentNode, ref XmlDocument XDoc, ref DataTable itemTable)
    {
        try
        {
            DataRow[] drs;
            if (parentID != "")
                drs = dt.Select("ParentID = '" + parentID.ToString() + "'");
            else
                drs = dt.Select("ParentID is null or ParentID = ''");

            foreach (DataRow row in drs)
            {
                CreateXmlDocumentFromDataTable(dt, row["CustomerGroupID"].ToString(), InsertNode(row, parentNode, ref XDoc, ref itemTable), ref XDoc, ref itemTable);
            }
        }
        catch (Exception ex)
        {

        }
    }

    public XmlNode InsertNode(DataRow Row, XmlNode parentNode, ref XmlDocument XDoc, ref DataTable itemTable)
    {
        try
        {
            XmlElement NewNode = XDoc.CreateElement("_" + Row["CustomerGroupID"].ToString());
            NewNode.SetAttribute("CustomerGroupID", Row["CustomerGroupID"].ToString());
            NewNode.SetAttribute("CustomerGroupName", Row["CustomerGroupName"].ToString());
            NewNode.SetAttribute("ParentID", Row["ParentID"].ToString());
            NewNode.SetAttribute("Description", Row["Description"].ToString());
            NewNode.SetAttribute("Inactive", Row["Inactive"].ToString());

            itemTable.Rows.Add(new string[] { Row["CustomerGroupID"].ToString(), Row["CustomerGroupName"].ToString(), Row["ParentID"].ToString(), Row["Description"].ToString(), Row["Inactive"].ToString() });
            if (parentNode == null)
                XDoc.AppendChild(NewNode);  // root node
            else
                parentNode.AppendChild(NewNode);
        }
        catch (Exception ex)
        {

        }
        return XDoc.SelectSingleNode(String.Format("//*[@CustomerGroupID=\"{0}\"]", Row["CustomerGroupID"].ToString()));
    }

    public void DisplayCustomerGroup()
    {
        DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup order by CustomerGroupID");
        // create an XmlDocument (with an XML declaration)
        XmlDocument XDoc = new XmlDocument();
        XmlDeclaration XDec = XDoc.CreateXmlDeclaration("1.0", null, null);
        XDoc.AppendChild(XDec);

        DataTable itemTable = new DataTable("CustomerGroup");
        itemTable.Columns.Add("CustomerGroupID");
        itemTable.Columns.Add("CustomerGroupName");
        itemTable.Columns.Add("ParentID");
        itemTable.Columns.Add("Description");
        itemTable.Columns.Add("Inactive");

        CreateXmlDocumentFromDataTable(dtCustomerGroup, "", null, ref XDoc, ref itemTable);

        // we cannot bind the TreeView directly to an XmlDocument
        // so we must create an XmlDataSource and assign the XML text
        XmlDataSource XDdataSource = new XmlDataSource();
        XDdataSource.ID = DateTime.Now.Ticks.ToString();  // unique ID is required
        XDdataSource.Data = XDoc.OuterXml;

        if (itemTable != null && itemTable.Rows.Count > 0)
        {
            cbCustomerGroup.Items.Add(new ListItem("<< Tất cả nhóm khách hàng >>", ""));
            foreach (DataRow dr in itemTable.Rows)
            {
                cbCustomerGroup.Items.Add(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
            }
        }
        //cbCustomerGroup.DataSource = itemTable;
        //cbCustomerGroup.DataTextField = "CustomerGroupName";
        //cbCustomerGroup.DataValueField = "CustomerGroupID";
        //cbCustomerGroup.DataBind();
    }

    public string GetCustomerGroup(string CustomerGroupID)
    {
        DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup where CustomerGroupID = '" + CustomerGroupID + "'");
        if (dtCustomerGroup != null && dtCustomerGroup.Rows.Count > 0)
            return dtCustomerGroup.Rows[0]["CustomerGroupName"].ToString();
        else
            return "";
    }

    public string GetStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Ngừng kích hoạt</span>";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "AccessControl_Customer", "Deletes", "AccessControl"))
                return StaticPool.mdb.ExecuteCommand("delete from tblCustomer where CustomerID = '" + id + "'").ToString().ToLower();
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
            DataTable dtCustomer = null;

            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
                txtKeyWord.Value = KeyWord;
            }

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = Request.QueryString["CustomerGroupID"].ToString();
            }

            dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer where (CustomerName LIKE N'%" + KeyWord + "%' or CustomerCode LIKE '%" + KeyWord + "%' or Mobile LIKE N'%" + KeyWord + "%') and CustomerGroupID LIKE N'%" + CustomerGroupID + "%' order by SortOrder desc");


            // Phan trang
            pgsource.DataSource = dtCustomer.DefaultView;
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

            if (dtCustomer != null && dtCustomer.Rows.Count <= pgsource.PageSize)
                frm1.Visible = false;
            else
                frm1.Visible = true;

            if (dtCustomer != null && dtCustomer.Rows.Count > 0)
            {
                id_customerlist.InnerText = "Danh sách khách hàng (" + dtCustomer.Rows.Count + ")";
                //Bind resulted PageSource into the Repeater
                rpt_Customer.DataSource = pgsource;
                rpt_Customer.DataBind();
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
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Xml;
using Futech.Helpers;

public partial class QLXuatAn_Customer : System.Web.UI.Page
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Customer", "Selects", "Parking"))
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
        DataTable dtCustomerGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
        if (dtCustomerGroup == null)
        {
            dtCustomerGroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
            if (dtCustomerGroup!=null && dtCustomerGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtCustomerGroup, StaticCached.TimeCache);
        }
        //DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup order by SortOrder");
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

    //public string GetCustomerGroup(string CustomerGroupID)
    //{
    //    DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup where CustomerGroupID = '" + CustomerGroupID + "'");
    //    if (dtCustomerGroup != null && dtCustomerGroup.Rows.Count > 0)
    //        return dtCustomerGroup.Rows[0]["CustomerGroupName"].ToString();
    //    else
    //        return "";
    //}

    //public string GetCompartment(string CompartmentID)
    //{
    //    DataTable dtCompartment = StaticPool.mdb.FillData("select * from tblCompartment where CompartmentID = '" + CompartmentID + "'");
    //    if (dtCompartment != null && dtCompartment.Rows.Count > 0)
    //        return dtCompartment.Rows[0]["CompartmentName"].ToString();
    //    else
    //        return "";
    //}

    public string GetStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Ngừng kích hoạt</span>";
    }

    //public string GetCard(string customerid)
    //{
    //    DataTable dtcard = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and CustomerID='" + customerid + "'");
    //    if (dtcard != null && dtcard.Rows.Count > 0)
    //    {
    //        string card = "";
    //        foreach (DataRow dr in dtcard.Rows)
    //        {
    //            if (card == "")
    //                card = dr["CardNumber"].ToString();
    //            else
    //                card = card + ";" + dr["CardNumber"].ToString();

    //            //string _plate = dr["Plate1"].ToString();
    //            //if (dr["Plate2"].ToString() != "")
    //            //    _plate = _plate + "_" + dr["Plate2"].ToString();
    //            //if (dr["Plate3"].ToString() != "")
    //            //    _plate = _plate + "_" + dr["Plate3"].ToString();

    //            //if (plate == "")
    //            //    plate = _plate;
    //            //else
    //            //    plate = plate + ";" + _plate;



    //        }
    //        return card;
    //    }
    //    return "";
    //}

    //public string GetPlate(string customerid)
    //{
    //    DataTable dtcard = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and CustomerID='" + customerid + "'");
    //    if (dtcard != null && dtcard.Rows.Count > 0)
    //    {
    //        string plate = "";
    //        foreach (DataRow dr in dtcard.Rows)
    //        {
    //            //if (card == "")
    //            //    card = dr["CardNumber"].ToString();
    //            //else
    //            //    card = card + ";" + dr["CardNumber"].ToString();

    //            string _plate = dr["Plate1"].ToString();
    //            if (dr["Plate2"].ToString() != "")
    //                _plate = _plate + "_" + dr["Plate2"].ToString();
    //            if (dr["Plate3"].ToString() != "")
    //                _plate = _plate + "_" + dr["Plate3"].ToString();

    //            if (plate == "")
    //                plate = _plate;
    //            else
    //                plate = plate + ";" + _plate;



    //        }
    //        return plate;
    //    }
    //    return "";
    //}

    //public string GetCardNo(string customerid)
    //{
    //    DataTable dtcard = StaticPool.mdb.FillData("select CardNo from tblCard where IsDelete=0 and CustomerID='" + customerid + "'");
    //    if (dtcard != null && dtcard.Rows.Count > 0)
    //    {
    //        string cardno = "";
    //        foreach (DataRow dr in dtcard.Rows)
    //        {
    //            if (cardno == "")
    //                cardno = dr["CardNo"].ToString();
    //            else
    //                cardno = cardno + ";" + dr["CardNo"].ToString();




    //        }
    //        return cardno;
    //    }
    //    return "";
    //}

    public void GetCardAndPlate(string customerid, ref string card, ref string plate)
    {
        DataTable dtcard = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and CustomerID='" + customerid + "'");
        if (dtcard != null && dtcard.Rows.Count > 0)
        {
            foreach (DataRow dr in dtcard.Rows)
            {
                if (card == "")
                    card = dr["CardNumber"].ToString();
                else
                    card = card + ";" + dr["CardNumber"].ToString();

                string _plate = dr["Plate1"].ToString();
                if (dr["Plate2"].ToString() != "")
                    _plate = _plate + "_" + dr["Plate2"].ToString();
                if (dr["Plate3"].ToString() != "")
                    _plate = _plate + "_" + dr["Plate3"].ToString();

                if (plate == "")
                    plate = _plate;
                else
                    plate = plate + ";" + _plate;



            }
        }
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Customer", "Deletes", "Parking") == false)
                return "Bạn không có quyền thực hiện chức năng này!";

            DataTable temp = StaticPool.mdb.FillData("select top 1 CustomerID, CardNumber from tblCard where CustomerID='" + id + "' and IsDelete=0");
            if (temp != null && temp.Rows.Count > 0)
            {
                return string.Format("Khách hàng đang thuộc vào 1 thẻ ({0}) , không xóa được", temp.Rows[0]["CardNumber"].ToString());
            }

            if (temp == null)
                return "Failed";

            string _customername = "";
            temp = StaticPool.mdb.FillData("select CustomerName from tblCustomer where CustomerID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _customername = temp.Rows[0]["CustomerName"].ToString();
            }

            if (StaticPool.mdb.ExecuteCommand("delete from tblCustomer where CustomerID = '" + id + "'"))
            {
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Customer", _customername, "Xóa", "id=" + id);
                return "true";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
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

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = Request.QueryString["CustomerGroupID"].ToString();
            }

            var totalCount = 0;
            DataTable dtCustomer = ReportService.GetCustomerList_2(KeyWord, CustomerGroupID, pageIndex, StaticPool.pagesize, ref totalCount);


            //dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer where (CustomerName LIKE N'%" + KeyWord + "%' or CustomerCode LIKE '%" + KeyWord + "%' or Mobile LIKE N'%" + KeyWord + "%') and CustomerGroupID LIKE N'%" + CustomerGroupID + "%' order by SortOrder desc");

            if (dtCustomer == null)
                return;

            var strListId = "";
            foreach (DataRow item in dtCustomer.Rows)
            {
                strListId += item["CustomerID"] + ",";
            }
            var dtCard = ReportService.GetCardByCustomer(strListId);
            foreach (DataRow iCus in dtCustomer.Rows)
            {
                var cardNo = "";
                var cardNumber = "";
                var plate = "";
                foreach (DataRow iCard in dtCard.Rows)
                {
                    if (iCard["CustomerID"].ToString().ToLower() == iCus["CustomerID"].ToString())
                    {
                        // cardNo
                        if (cardNo == "")
                            cardNo = iCard["CardNo"].ToString();
                        else
                            cardNo = cardNo + ";" + iCard["CardNo"].ToString();


                        // cardNo
                        if (cardNumber == "")
                            cardNumber = iCard["CardNumber"].ToString();
                        else
                            cardNumber = cardNumber + ";" + iCard["CardNumber"].ToString();

                        //plate
                        var _plate = iCard["Plate1"].ToString();
                        if (iCard["Plate2"].ToString() != "")
                            _plate = _plate + "_" + iCard["Plate2"].ToString();
                        if (iCard["Plate3"].ToString() != "")
                            _plate = _plate + "_" + iCard["Plate3"].ToString();

                        if (plate == "")
                            plate = _plate;
                        else
                            plate = plate + ";" + _plate;
                    }

                }
               
                iCus["CardNo"] = cardNo;
                iCus["CardNumber"] = cardNumber;
                iCus["Plate"] = plate;
               
            }
           // dtCustomer.Select

            // // Phan trang
            // pgsource.DataSource = dtCustomer.DefaultView;
            // //Set PageDataSource paging 
            // pgsource.AllowPaging = true;
            // //Set number of items to be displayed in the Repeater using drop down list
            // pgsource.PageSize = 20;
            // //Get Current Page Index
            // pgsource.CurrentPageIndex = CurrentPage;
            // //Store it Total pages value in View state
            // ViewState["totpage"] = pgsource.PageCount;
            // //Below line is used to show page number based on selection like "Page 1 of 20"
            // lblpage.Text = (CurrentPage + 1) + " / " + pgsource.PageCount;
            // //Enabled true Link button previous when current page is not equal first page 
            // //Enabled false Link button previous when current page is first page
            // lnkPrevious.Enabled = !pgsource.IsFirstPage;
            // //Enabled true Link button Next when current page is not equal last page 
            // //Enabled false Link button Next when current page is last page
            // lnkNext.Enabled = !pgsource.IsLastPage;
            // //Enabled true Link button First when current page is not equal first page 
            // //Enabled false Link button First when current page is first page
            // lnkFirst.Enabled = !pgsource.IsFirstPage;
            // //Enabled true Link button Last when current page is not equal last page 
            // //Enabled false Link button Last when current page is last page
            // lnkLast.Enabled = !pgsource.IsLastPage;

            // //Create Paging with help of DataList control "RepeaterPaging"
            // doPaging();
            // //RepeaterPaging.ItemStyle.HorizontalAlign = HorizontalAlign.Center;

            ////if (dtCustomer != null && dtCustomer.Rows.Count <= pgsource.PageSize)
            ////     frm1.Visible = false;
            //// else
            ////     frm1.Visible = true;

            if (dtCustomer != null && dtCustomer.Rows.Count > 0)
            {
                id_customerlist.InnerText = "Danh sách khách hàng (" + totalCount + ")";
                //By HNG paging
                StaticPool.HNGpaging(dtCustomer, totalCount, StaticPool.pagesize, pager, rpt_Customer);
                rpt_Customer.DataSource = dtCustomer;
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

            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }



            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("Mã KH", typeof(string));
            dt.Columns.Add("Tên KH", typeof(string));
            dt.Columns.Add("Nhóm KH", typeof(string));
            dt.Columns.Add("Địa chỉ", typeof(string));
            dt.Columns.Add("Điện thoại", typeof(string));
            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("Mã thẻ", typeof(string));
            dt.Columns.Add("Biển số", typeof(string));

            dtCustomer = ReportService.GetCustomerListExcel(KeyWord, CustomerGroupID, pageIndex, StaticPool.pagesize); 
            //StaticPool.mdb.FillData("select * from tblCustomer where (CustomerName LIKE N'%" + KeyWord + "%' or CustomerCode LIKE '%" + KeyWord + "%' or Mobile LIKE N'%" + KeyWord + "%') and CustomerGroupID LIKE N'%" + CustomerGroupID + "%' order by SortOrder desc");

            var strListId = "";
            foreach (DataRow item in dtCustomer.Rows)
            {
                strListId += item["Mã KH"] + ",";
            }
            var dtCard = ReportService.GetCardByCustomer(strListId);
            foreach (DataRow iCus in dtCustomer.Rows)
            {
                var cardNo = "";
                var cardNumber = "";
                var plate = "";
                foreach (DataRow iCard in dtCard.Rows)
                {
                    if (iCard["CustomerID"].ToString() == iCus["Mã KH"].ToString())
                    {
                        // cardNo
                        if (cardNo == "")
                            cardNo = iCard["CardNo"].ToString();
                        else
                            cardNo = cardNo + ";" + iCard["CardNo"].ToString();


                        // cardNo
                        if (cardNumber == "")
                            cardNumber = iCard["CardNumber"].ToString();
                        else
                            cardNumber = cardNumber + ";" + iCard["CardNumber"].ToString();

                    }

                }

                iCus["CardNo"] = cardNo;
                iCus["Mã thẻ"] = cardNumber;
                iCus["Biển số"] = plate;

            }

            //foreach (DataRow dr in dtCustomer.Rows)
            //{
            //    string _cardNos = "";
            //    string _cardnumbers = "";
            //    string _plates = "";
            //    DataTable dtcard = StaticPool.mdb.FillData("select CardNumber, Plate1, Plate2, Plate3, CardNo from tblCard where CustomerID='" + dr["CustomerID"] + "'");
            //    if (dtcard != null && dtcard.Rows.Count > 0)
            //    {
            //        foreach (DataRow drcard in dtcard.Rows)
            //        {
            //            if (_cardNos == "")
            //            {
            //                _cardNos = drcard["CardNo"].ToString();
            //            }
            //            else
            //            {
            //                _cardNos = _cardNos + ";" + drcard["CardNo"].ToString();
            //            }

            //            if (_cardnumbers == "")
            //                _cardnumbers = drcard["CardNumber"].ToString();
            //            else
            //                _cardnumbers = _cardnumbers + ";" + drcard["CardNumber"].ToString();

            //            if (_plates == "")
            //            {
            //                _plates = drcard["Plate1"].ToString() +
            //                    (drcard["Plate2"].ToString() == "" ? "" : ("_" + drcard["Plate2"].ToString())) +
            //                    (drcard["Plate3"].ToString() == "" ? "" : ("_" + drcard["Plate3"].ToString()));
            //            }
            //            else
            //            {
            //                _plates = _plates + ";" + drcard["Plate1"].ToString() +
            //                   (drcard["Plate2"].ToString() == "" ? "" : ("_" + drcard["Plate2"].ToString())) +
            //                   (drcard["Plate3"].ToString() == "" ? "" : ("_" + drcard["Plate3"].ToString()));
            //            }
            //        }
            //    }

            //    dt.Rows.Add(
            //        dt.Rows.Count + 1,
            //        dr["CustomerCode"].ToString(),
            //        dr["CustomerName"].ToString(),
            //        GetCustomerGroup(dr["CustomerGroupID"].ToString()),
            //        dr["Address"].ToString(),
            //        dr["Mobile"].ToString(),
            //        _cardNos,
            //        _cardnumbers,
            //        _plates);

            //}



            string _title1 = "Danh sách khách hàng";
            string _title2 = "";// "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
            try
            {
                BindDataToExcel(dtCustomer, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/Customer.aspx", false);
            }
            //GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
            //GridView gv = new GridView();

            //gv.DataSource = dtCustomer;
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
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Futech.Helpers;

public partial class QLXuatAn_Card : Page
{
    int findex, lindex;

    string KeyWord = "", CardGroupID = "", VehicleGroupID = "", CustomerID = "";
    string CardState = "";
    string CustomerGroupID = "", CaNo = "";
    DataTable dtCusMap = null;
    DataTable dtCardSub = null;
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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Card", "Selects", "Parking"))
                {
                    div_alert.Visible = false;
                    id_alert.InnerText = "";

                    // Nhom the
                    DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
                    if (dtCardGroup == null)
                    {
                        dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
                        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                            CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
                    }
                    //DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup");
                    if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                    {
                        cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                        foreach (DataRow dr in dtCardGroup.Rows)
                        {
                            cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                        }
                    }

                    //// Khách hàng
                    //DataTable dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer order by SortOrder");
                    //if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                    //{
                    //    cbCustomer.Items.Add(new ListItem("<< Tất cả khách hàng >>", ""));
                    //    foreach (DataRow dr in dtCustomer.Rows)
                    //    {
                    //        cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
                    //    }
                    //}

                    //Customergroup 
                    DataTable dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
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
    public string GetCardGroupByName(string GroupName)
    {
        DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup == null)
        {
            dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            if (dtCardGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
        }
        //DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
        {
            var dtr = dtCardGroup.Select(string.Format("CardGroupName = '{0}'", GroupName));
            if (dtr.Length > 0)
            {
                return dtr[0]["CardGroupID"].ToString();
            }
        }
        return "";
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
    public string GetImportDate(string importdate)
    {
        if (importdate == "")
            return "";
        else
            return DateTime.Parse(importdate).ToString("dd/MM/yyyy");
    }
    public string GetCardStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Đã khóa thẻ</span>";
    }
    public string GetCustomeIDbyName(string CardNumber)
    {
        if (!string.IsNullOrWhiteSpace(CardNumber))
        {
            var dt = StaticPool.mdb.FillData("select CustomerID from tblCard where CardNumber='" + CardNumber + "' AND IsDelete=0");
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["CustomerID"].ToString();
            }
        }
        return "";
    }
    public string GetCustomerName(string customerid)
    {
        if (customerid != "" && customerid != "-1")
        {
            DataTable dt = StaticPool.mdb.FillData("select CustomerName from tblCustomer where CustomerID='" + customerid + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["CustomerName"].ToString();
            }
        }
        return "";
    }
    public string GetCustomerAddress(string customerid)
    {
        if (customerid != "" && customerid != "-1")
        {
            DataTable dt = StaticPool.mdb.FillData("select Address from tblCustomer where CustomerID='" + customerid + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Address"].ToString();
            }
        }
        return "";
    }
    public string GetCustomerGroupByName(string groupName)
    {
        DataTable dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
        if (dtcustomergroup == null)
        {
            dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
            if (dtcustomergroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtcustomergroup, StaticCached.TimeCache);
        }
        if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
        {
            var dtr = dtcustomergroup.Select(string.Format("CustomerGroupName = '{0}'", groupName));
            if (dtr.Length > 0)
            {
                return dtr[0]["CustomerGroupID"].ToString();
            }
        }

        return "";
    }
    //public string GetCustomerGroup(string customerid)
    //{
    //    if (customerid != "" && customerid != "-1")
    //    {
    //        DataTable dt = StaticPool.mdb.FillData("select CustomerGroupID from tblCustomer where CustomerID='" + customerid + "'");
    //        if (dt != null && dt.Rows.Count > 0)
    //        {
    //            DataTable temp = StaticPool.mdb.FillData("select CustomerGroupName from tblCustomerGroup where CustomerGroupID='" + dt.Rows[0]["CustomerGroupID"].ToString() + "'");
    //            if (temp != null && temp.Rows.Count > 0)
    //                return temp.Rows[0]["CustomerGroupName"].ToString();
    //            //return dt.Rows[0]["Address"].ToString();
    //        }
    //    }
    //    return "";
    //}
    //ham lay ra ket qua tim kiem khach hang realtime, Autocomplete
    [WebMethod]
    public static List<ListItem> getCustomerByAutocomplete(string name)
    {
        var list = new List<ListItem>();
        //var firstItem = new ListItem
        //{
        //    Value = "",
        //    Text = "Xóa khách hàng"
        //};
        //list.Add(firstItem);;
        string commandstring = "";
        if (!string.IsNullOrWhiteSpace(name))
            commandstring = "select top 10 customername, customerid,CustomerCode from tblcustomer where customername like N'%" + name + "%' OR CustomerCode LIKE N'%" + name + "%' order by customername ";
        else
            commandstring = "select top 10 customername, customerid,CustomerCode from tblcustomer order by customername";

        DataTable dt = StaticPool.mdb.FillData(commandstring);
        if (dt.Rows.Count != 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                ListItem item = new ListItem();
                item.Value = dr["customerid"].ToString();
                if (!string.IsNullOrWhiteSpace(dr["customername"].ToString()))
                    item.Text = dr["customername"].ToString().Trim();
                if (!string.IsNullOrWhiteSpace(dr["CustomerCode"].ToString()))
                {
                    item.Text = dr["CustomerCode"] + @" / " + item.Text;
                }
                list.Add(item);
            }
        }

        return list; //new JavaScriptSerializer().Serialize(list).ToString();
    }
    [WebMethod]
    public static List<ListItem> getCardByAutocomplete(string name)
    {
        List<ListItem> list = new List<ListItem>();

        DataTable dt = ReportService.GetCardToSearchComplete(name);
        if (dt != null && dt.Rows.Count != 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                ListItem item = new ListItem();
                item.Value = dr["itemvalue"].ToString();
                item.Text = dr["itemtext"].ToString();
                list.Add(item);
            }
        }

        return list; //new JavaScriptSerializer().Serialize(list).ToString();
    }
    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Card", "Deletes", "Parking"))
            {
                string _cardnumber = "";
                string _cardgroupid = "";
                DataTable temp = StaticPool.mdb.FillData("select CardNumber, CardGroupID from tblCard where CardID='" + id + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    _cardnumber = temp.Rows[0]["CardNumber"].ToString();
                    _cardgroupid = temp.Rows[0]["CardGroupID"].ToString();
                }

                temp = StaticPool.mdbevent.FillData("select top 1 CardNumber from tblCardEvent where CardNumber='" + _cardnumber + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "Thẻ đang sử dụng, không xóa được";
                }

                temp = StaticPool.mdb.FillData("select top 1 CardNumber from tbActiveCard where CardNumber='" + _cardnumber + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    return "Thẻ đang sử dụng, không xóa được";
                }

                if (StaticPool.mdb.ExecuteCommand("update tblCard set IsDelete=1 where CardID = '" + id + "'"))
                {
                    DeleteInvalidCardInLogCardCustomer(id);

                    //delete card
                    StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID) values('" +
                      DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                      _cardnumber + "', '" +
                      "DELETE" + "', '" +
                      _cardgroupid + "', '" +
                      userid +
                      "')");
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Card", _cardnumber, "Xóa", "id=" + id);
                    return "true";
                }
            }
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }
    public string GetCusName(string cusIs)
    {
        var _cusName = "";
        if (dtCusMap != null && dtCusMap.Rows.Count > 0)
        {
            var rRow = dtCusMap.Select(string.Format("CustomerID = '{0}'", cusIs));
            if (rRow.Length > 0)
                _cusName = rRow[0]["CustomerName"].ToString();
        }
        return _cusName;
    }
    public string GetCusAddress(string cusIs)
    {
        var _cusName = "";
        if (dtCusMap != null && dtCusMap.Rows.Count > 0)
        {
            var rRow = dtCusMap.Select(string.Format("CustomerID = '{0}'", cusIs));
            if (rRow.Length > 0)
                _cusName = rRow[0]["Address"].ToString();
        }
        return _cusName;
    }
    public string GetListCardSub(string cardNumber)
    {
        var _list = "";
        if (dtCardSub != null && dtCardSub.Rows.Count > 0)
        {
            var rRow = dtCardSub.Select(string.Format("MainCard = '{0}'", cardNumber));
            if (rRow.Length > 0)
            {
                _list += "<ul class='ulListSubCard'>";
                foreach (DataRow item in rRow)
                {
                    if (!string.IsNullOrWhiteSpace(item["CardNo"].ToString()))
                    {
                        _list += "<li><i class='fa fa-angle-right'></i> ";
                        _list += item["CardNo"].ToString();
                        _list += "</li>";
                    }
                }
                _list += "</ul>";
            }
        }
        return Server.HtmlEncode(_list);
    }
    public string GetCustomerGroup(string customerid)
    {
        if (customerid != "" && customerid != "-1")
        {
            if (dtCusMap != null && dtCusMap.Rows.Count > 0)
            {
                var dt = dtCusMap.Select(string.Format("CustomerID = '{0}'", customerid));
                if (dt.Length > 0)
                {
                    var dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup + "_2");
                    if (dtcustomergroup == null)
                    {
                        dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupName,CustomerGroupID from tblCustomerGroup WITH (NOLOCK)");
                        if (dtcustomergroup.Rows.Count > 0)
                            CacheLayer.Add(StaticCached.c_tblCustomerGroup + "_2", dtcustomergroup, StaticCached.TimeCache);
                    }
                    if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
                    {
                        var dtr = dtcustomergroup.Select(string.Format("CustomerGroupID = '{0}'", dt[0]["CustomerGroupID"].ToString()));
                        if (dtr.Length > 0)
                        {
                            return dtr[0]["CustomerGroupName"].ToString();
                        }
                    }
                    //return dt.Rows[0]["Address"].ToString();
                }

            }
        }
        return "";
    }
    private void BindDataList()
    {
        try
        {
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

            if (Request.QueryString["CardState"] != null && Request.QueryString["CardState"].ToString() != "")
            {
                CardState = Request.QueryString["CardState"].ToString();
                cbCardState.Value = CardState;
            }

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = CustomerGroupID;
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
            var totalCount = 0;
            DataTable dtCard = ReportService.GetCardList_2(KeyWord, CaNo, CardGroupID, CustomerID, CardState, CustomerGroupID, pageIndex, StaticPool.pagesize, ref totalCount);

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                var listCusId = "";
                var listCardNumber = "";
                foreach (DataRow item in dtCard.Rows)
                {
                    listCusId += item["CustomerID"] + ",";
                    listCardNumber += item["CardNumber"] + ",";
                }

                dtCusMap = ReportService.GetCardByCustomerId(listCusId);
                dtCardSub = ReportService.GetCardSubByCardNumber(listCardNumber);

                foreach (DataRow item in dtCard.Rows)
                {
                    item["CustomerName"] = GetCusName(item["CustomerID"].ToString());
                    item["Address"] = GetCusAddress(item["CustomerID"].ToString());
                }
            }



            // dtCard.Columns.Add("CardID", typeof(string));
            // dtCard.Columns.Add("CardNo", typeof(string));
            // dtCard.Columns.Add("CardNumber", typeof(string));
            // dtCard.Columns.Add("CardGroupID", typeof(string));
            // dtCard.Columns.Add("Plate1", typeof(string));
            // dtCard.Columns.Add("ExpireDate", typeof(string));
            // dtCard.Columns.Add("IsLock", typeof(string));
            // dtCard.Columns.Add("CustomerName", typeof(string));
            // dtCard.Columns.Add("Address", typeof(string));
            // dtCard.Columns.Add("CustomerGroupName", typeof(string));
            // dtCard.Columns.Add("ImportDate", typeof(string));

            //DataTable  dt = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and (CardNo LIKE N'%" + KeyWord +
            //     "%' or CardNumber LIKE N'%" + KeyWord + 
            //     "%' or Plate1 LIKE N'%"+KeyWord+
            //     "%' or Plate2 LIKE N'%" + KeyWord +
            //     "%' or Plate3 LIKE N'%" + KeyWord +
            //     "%') and CardGroupID LIKE '%" + CardGroupID + 
            //     "%' and CustomerID LIKE '%" + CustomerID +
            //     "%' and IsLock LIKE '%"+CardState+
            //     "%' order by SortOrder desc");
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {

            //        DataTable temp = StaticPool.mdb.FillData("select CustomerID, CustomerGroupID, CustomerName from tblCustomer where CustomerID='" + dr["CustomerID"].ToString() + "'");
            //        if (temp != null && temp.Rows.Count > 0)
            //        {
            //            if (CustomerGroupID != "")
            //            {
            //                if (temp.Rows[0]["CustomerGroupID"].ToString() != CustomerGroupID)
            //                    continue;
            //            }

            //        }
            //        else if (temp == null || (temp != null && temp.Rows.Count == 0))
            //        {
            //            if (CustomerGroupID != "")
            //                continue;
            //        }

            //        string _customername = "";
            //        string _customeraddress = "";
            //        string _customergroupname = "";

            //        GetCustomerNameAndAddress(dr["CustomerID"].ToString(), ref _customername, ref _customeraddress, ref _customergroupname);

            //        dtCard.Rows.Add(
            //            dr["CardID"].ToString(),
            //            dr["CardNo"].ToString(),
            //            dr["CardNumber"].ToString(),
            //            dr["CardGroupID"].ToString(),
            //            dr["Plate1"].ToString(),
            //            dr["ExpireDate"].ToString(),
            //            dr["IsLock"].ToString(),
            //            _customername,
            //            _customeraddress,
            //            _customergroupname,
            //            dr["ImportDate"].ToString()
            //            );
            //    }
            //}


            // if (dtCard == null)
            //     return;
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

            ////if (dtCard != null && dtCard.Rows.Count <= pgsource.PageSize)
            ////    frm1.Visible = false;
            ////else
            ////    frm1.Visible = true;

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                id_cardlist.InnerText = "Danh sách thẻ (" + totalCount + ")";
                //By HNG paging
                StaticPool.HNGpaging(dtCard, totalCount, StaticPool.pagesize, pager, rpt_Card);
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
            lnkPage.BackColor = Color.FromName("#FFCC01");
        }
    }
    private void GetCustomerNameAndAddress(string customerid, ref string customername, ref string address, ref string customergroupname)
    {
        if (customerid != "" && customerid != "-1")
        {
            DataTable dt = StaticPool.mdb.FillData("select CustomerName, Address, CustomerGroupID from tblCustomer where CustomerID='" + customerid + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                customername = dt.Rows[0]["CustomerName"].ToString();
                address = dt.Rows[0]["Address"].ToString();
                DataTable temp = StaticPool.mdb.FillData("select CustomerGroupName from tblCustomerGroup where CustomerGroupID='" + dt.Rows[0]["CustomerGroupID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                    customergroupname = temp.Rows[0]["CustomerGroupName"].ToString();
            }
        }
    }
    //export all page
    protected void Excel_Click(object sender, EventArgs e)
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
            //cbCustomer.Value = CustomerID;
        }

        if (Request.QueryString["CardState"] != null && Request.QueryString["CardState"].ToString() != "")
        {
            CardState = Request.QueryString["CardState"].ToString();
            cbCardState.Value = CardState;
        }

        if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
        {
            CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
            cbCustomerGroup.Value = CustomerGroupID;
        }
        var pageIndex = 1;
        if (Request.QueryString["Page"] != null)
        {
            pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
        }
        if (Request.QueryString["CaNoSort"] != null && Request.QueryString["CaNoSort"].ToString() != "")
        {
            CaNo = ViewState["CaNoSort"].ToString();
        }
        else
        {
            CaNo = "";
        }
        dtCard = ReportService.GetCardListExcel(KeyWord, CaNo, CardGroupID, CustomerID, CardState, CustomerGroupID, pageIndex, StaticPool.pagesize);

        string _title1 = "Danh sách thẻ";
        string _title2 = "";// "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
        try
        {
            BindDataToExcel(dtCard, _title1, _title2, ViewState["UserID"].ToString());
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
        }

    }
    //--#1
    public void BindDataToExcel(DataTable _dt, string _title1, string _title2, string uId)
    {
        Response.Clear();
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
    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        if (FileUpload1 == null || string.IsNullOrWhiteSpace(FileUpload1.PostedFile.FileName))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Có lỗi sảy ra", "alert('bạn cần chọn file')", true);
            Response.Redirect("~/QLXuatAn/Card.aspx", false);
            return;
        }
        string _title1 = "Danh sách thẻ";
        string _title2 = "";
        var dtHeader = StaticPool.getHeaderExcel(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
        string myTempFile = Path.Combine(Path.GetTempPath(), FileUpload1.PostedFile.FileName);
        FileUpload1.SaveAs(myTempFile);
        var txtError = "";
        if (File.Exists(myTempFile))
        {
            var dt = StaticPool.ReadFromExcelfile(myTempFile, dtHeader, ref txtError);
            if (!string.IsNullOrWhiteSpace(txtError))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Có lỗi sảy ra", string.Format("alert('{0}')", txtError), true);
                Response.Redirect("~/QLXuatAn/Card.aspx", false);
                return;
            }

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(item["Mã thẻ"].ToString()) &&
                            string.IsNullOrWhiteSpace(item["CardNo"].ToString()))
                        {
                            continue;
                        }
                        var cusId = "";
                        cusId = GetCustomeIDbyName(item["Mã thẻ"].ToString());
                        // update the
                        var u1 = ReportService.UpdateCardFromExcel(item["CardNo"].ToString(), item["Mã thẻ"].ToString(), GetCardGroupByName(item["Nhóm thẻ"].ToString()), item["Ngày hết hạn"].ToString(), cusId, item["Ngày nhập thẻ"].ToString(), item["Biển số"].ToString(), item["Trạng thái"].ToString(), item["Loại xe"].ToString(), Request.Cookies["UserID"].Value.ToString(), cusId);
                        if (u1)
                        {
                            //update khach hang
                            if (item["Mã KH"] != DBNull.Value &&
                                !string.IsNullOrWhiteSpace(item["Mã KH"].ToString()))
                            {
                                ReportService.UpdateCustomerFromExcel(cusId,
                                    item["Tên khách hàng"].ToString(), item["Mã KH"].ToString(), item["Địa chỉ"].ToString(),
                                    GetCustomerGroupByName(item["Nhóm KH"].ToString()), item["Mã thẻ"].ToString(), GetCardGroupByName(item["Nhóm thẻ"].ToString()), Request.Cookies["UserID"].Value);
                            }
                            StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Card", item["Mã thẻ"].ToString(), "Thêm", "");
                        }

                    }


                }



                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Thông báo", "alert('Cập nhập thành công');", true);
                Response.Redirect("~/QLXuatAn/Card.aspx", false);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}');", ex.Message), true);
            }
        }
    }
    //    try
    //    {

    //        DataTable dtCard = null;

    //        if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
    //        {
    //            KeyWord = Request.QueryString["KeyWord"].ToString();
    //            txtKeyWord.Value = KeyWord;
    //        }

    //        if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
    //        {
    //            CardGroupID = Request.QueryString["CardGroupID"].ToString();
    //            cbCardGroup.Value = CardGroupID;
    //        }

    //        if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
    //        {
    //            CustomerID = Request.QueryString["CustomerID"].ToString();
    //            //cbCustomer.Value = CustomerID;
    //        }

    //        if (Request.QueryString["CardState"] != null && Request.QueryString["CardState"].ToString() != "")
    //        {
    //            CardState = Request.QueryString["CardState"].ToString();
    //            cbCardState.Value = CardState;
    //        }

    //        if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
    //        {
    //            CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
    //            cbCustomerGroup.Value = CustomerGroupID;
    //        }
    //        var pageIndex = 1;
    //        if (Request.QueryString["Page"] != null)
    //        {
    //            pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
    //        }
    //        if (Request.QueryString["CaNoSort"] != null && Request.QueryString["CaNoSort"].ToString() != "")
    //        {
    //            CaNo = ViewState["CaNoSort"].ToString();
    //        }
    //        else
    //        {
    //            CaNo = "";
    //        }
    //        dtCard = ReportService.GetCardListExcel(KeyWord, CaNo, CardGroupID, CustomerID, CardState, CustomerGroupID, pageIndex, StaticPool.pagesize);

    //        //if (dtCard != null && dtCard.Rows.Count > 0)
    //        //{
    //        //    foreach (DataRow item in dtCard.Rows)
    //        //    {
    //        //        if (!string.IsNullOrWhiteSpace(item["Ngày hết hạn"].ToString()))
    //        //        {
    //        //            item["Ngày hết hạn"] = Convert.ToDateTime(item["Ngày hết hạn"].ToString()).ToString("dd/MM/yyyy");
    //        //        }
    //        //        if (!string.IsNullOrWhiteSpace(item["Ngày nhập thẻ"].ToString()))
    //        //        {
    //        //            item["Ngày nhập thẻ"] = Convert.ToDateTime(item["Ngày nhập thẻ"].ToString()).ToString("dd/MM/yyyy");
    //        //        }
    //        //    }
    //        //}

    //        //dtCard = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and (CardNo LIKE N'%" + KeyWord +
    //        //    "%' or CardNumber LIKE N'%" + KeyWord +
    //        //    "%' or Plate1 LIKE N'%" + KeyWord +
    //        //    "%' or Plate2 LIKE N'%" + KeyWord +
    //        //    "%' or Plate3 LIKE N'%" + KeyWord +
    //        //    "%') and CardGroupID LIKE '%" + CardGroupID +
    //        //    "%' and CustomerID LIKE '%" + CustomerID +
    //        //    "%' and IsLock LIKE '%" + CardState +
    //        //    "%' order by SortOrder desc");



    //        //DataTable dt = new DataTable();
    //        //dt.Columns.Add("STT", typeof(string));
    //        //dt.Columns.Add("CardNo", typeof(string));
    //        //dt.Columns.Add("Mã thẻ", typeof(string));
    //        //dt.Columns.Add("Nhóm thẻ", typeof(string));
    //        //dt.Columns.Add("Biển số", typeof(string));
    //        //dt.Columns.Add("Ngày hết hạn", typeof(string));
    //        //dt.Columns.Add("Trạng thái", typeof(string));
    //        //dt.Columns.Add("Khách hàng", typeof(string));
    //        //dt.Columns.Add("Địa chỉ", typeof(string));
    //        //dt.Columns.Add("Nhóm KH", typeof(string));
    //        //dt.Columns.Add("Ngày nhập thẻ", typeof(string));

    //        //foreach (DataRow dr in dtCard.Rows)
    //        //{
    //        //    DataTable temp = StaticPool.mdb.FillData("select CustomerID, CustomerGroupID, CustomerName from tblCustomer where CustomerID='" + dr["CustomerID"].ToString() + "'");
    //        //    if (temp != null && temp.Rows.Count > 0)
    //        //    {
    //        //        if (CustomerGroupID != "")
    //        //        {
    //        //            if (temp.Rows[0]["CustomerGroupID"].ToString() != CustomerGroupID)
    //        //                continue;
    //        //        }

    //        //    }
    //        //    else if (temp == null || (temp != null && temp.Rows.Count == 0))
    //        //    {
    //        //        if (CustomerGroupID != "")
    //        //            continue;
    //        //    }

    //        //    string _plate = dr["Plate1"].ToString() +
    //        //        (dr["Plate2"].ToString() == "" ? "" : ("_" + dr["Plate2"].ToString())) +
    //        //        (dr["Plate3"].ToString() == "" ? "" : ("_" + dr["Plate3"].ToString()));

    //        //    string _customername = "";
    //        //    string _address = "";
    //        //    string _customergroupname = "";
    //        //    GetCustomerNameAndAddress(dr["CustomerID"].ToString(), ref _customername, ref _address, ref _customergroupname);

    //        //    dt.Rows.Add(
    //        //        dt.Rows.Count + 1,
    //        //        dr["CardNo"].ToString(),
    //        //        dr["CardNumber"].ToString(),
    //        //        GetCardGroup(dr["CardGroupID"].ToString()),
    //        //        _plate,
    //        //        DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy"),
    //        //        (bool.Parse(dr["IsLock"].ToString()) == true ? "Khóa" : "Hoạt động"),
    //        //        _customername,
    //        //        _address,
    //        //        _customergroupname,
    //        //        GetImportDate(dr["ImportDate"].ToString())
    //        //        );

    //        //}

    //        string _title1 = "Danh sách thẻ";
    //        string _title2 = "";// "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;

    //        GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
    //        GridView gv = new GridView();

    //        gv.DataSource = dtCard;
    //        gv.DataBind();

    //        Table tb = new Table();
    //        TableRow tr1 = new TableRow();
    //        TableCell cell1 = new TableCell();
    //        cell1.Controls.Add(gvheader);
    //        tr1.Cells.Add(cell1);

    //        TableCell cell3 = new TableCell();
    //        cell3.Controls.Add(gv);

    //        TableCell cell2 = new TableCell();
    //        TableRow tr2 = new TableRow();
    //        tr2.Cells.Add(cell2);

    //        TableRow tr3 = new TableRow();
    //        tr3.Cells.Add(cell3);

    //        //TableRow tr4 = new TableRow();
    //        //TableCell cell4 = new TableCell();
    //        //cell4.Controls.Add(StaticPool.CreateFooterTable());
    //        //tr4.Cells.Add(cell4);


    //        tb.Rows.Add(tr1);
    //        tb.Rows.Add(tr2);
    //        tb.Rows.Add(tr3);
    //        //tb.Rows.Add(tr4);

    //        gv.GridLines = GridLines.None;
    //        Response.ClearContent();
    //        Response.Buffer = true;
    //        Response.AddHeader("content-disposition", "attachment; filename=Report" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls");
    //        Response.Charset = "";
    //        Response.ContentType = "application/excel";
    //        StringWriter sw = new StringWriter();
    //        HtmlTextWriter htm = new HtmlTextWriter(sw);
    //        Response.ContentEncoding = Encoding.UTF8;
    //        //StringWriter sw = new StringWriter();
    //        HtmlTextWriter htw = new HtmlTextWriter(htm);
    //        htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
    //        string style = @"<style> TD { mso-number-format:\@; } </style>";
    //        Response.Write(style);
    //        tb.RenderControl(htm);

    //        Response.Write(sw.ToString());

    //        HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
    //        HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
    //        HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
    //    }
    //    catch (Exception ex)
    //    {
    //        Response.Write(ex.Message);
    //    }

    //}
    private static void DeleteInvalidCardInLogCardCustomer(string id)
    {
        string getCard = string.Format("select * from tblCard where CardID = '{0}'", id);
        DataTable tb = StaticPool.mdb.FillData(getCard);

        if (tb != null && tb.Rows.Count > 0)
        {
            string command = string.Format("delete from tblLogCardCustomer where CardNumber = '{0}'", tb.Rows[0]["CardNumber"].ToString());
            StaticPool.mdb.ExecuteCommand(command);
        }
    }




}
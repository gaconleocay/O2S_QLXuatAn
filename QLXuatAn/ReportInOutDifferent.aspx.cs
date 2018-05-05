using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;

public partial class QLXuatAn_Report3 : System.Web.UI.Page
{
    PagedDataSource pgsource = new PagedDataSource();
    int findex, lindex;

    string KeyWord = "", CardGroupID = "";
    string FromDate = "", ToDate = "";
    bool IsFilterByTimeIn = false;
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

                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Report_Report2", "Selects", "Parking") == false)
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

                    if (StaticPool.SystemUsingLoop() == true)
                    {
                        cbCardGroup.Items.Add(new ListItem("Vòng từ-Xe lượt (Loop)", "LOOP_D"));
                        cbCardGroup.Items.Add(new ListItem("Vòng từ-Xe tháng (Loop)", "LOOP_M"));
                    }
                }

                //lane
                DataTable dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane order by SortOrder");
                if (dtlane != null && dtlane.Rows.Count > 0)
                {
                    cbLane.Items.Add(new ListItem("<< Tất cả các làn >>", ""));
                    foreach (DataRow dr in dtlane.Rows)
                    {
                        cbLane.Items.Add(new ListItem(dr["LaneName"].ToString(), dr["LaneID"].ToString()));
                    }
                }

                //user
                DataTable dtuser = StaticPool.mdb.FillData("select UserName, UserID from tblUser where IsLock=0 order by SortOrder");
                if (dtuser != null && dtuser.Rows.Count > 0)
                {
                    cbUser.Items.Add(new ListItem("<< Tất cả người dùng >>", ""));
                    foreach (DataRow dr in dtuser.Rows)
                    {
                        cbUser.Items.Add(new ListItem(dr["UserName"].ToString(), dr["UserID"].ToString()));
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

    public string GetCardGroup(string CardGroupID)
    {
        if (CardGroupID == "LOOP_D")
            return "Vòng từ-Xe lượt";
        else if (CardGroupID == "LOOP_M")
            return "Vòng từ-Xe tháng";
        DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName from tblCardGroup where CardGroupID = '" + CardGroupID + "'");
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            return dtCardGroup.Rows[0]["CardGroupName"].ToString();
        else
            return "";
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

            if (Request.QueryString["IsFilterByTimeIn"] != null && Request.QueryString["IsFilterByTimeIn"].ToString() != "")
            {
                IsFilterByTimeIn = bool.Parse(Request.QueryString["IsFilterByTimeIn"].ToString());
                chFilterByTimeIn.Checked = IsFilterByTimeIn;
                chFilterByTimeOut.Checked = !chFilterByTimeIn.Checked;
            }

            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"].ToString() != "")
            {
                LaneID = Request.QueryString["LaneID"].ToString();
                cbLane.Value = LaneID;
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                cbUser.Value = UserID;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("CardNumber", typeof(string));
            dt.Columns.Add("PlateIn", typeof(string));
            dt.Columns.Add("PlateOut", typeof(string));
            dt.Columns.Add("DateTimeIn", typeof(string));
            dt.Columns.Add("DateTimeOut", typeof(string));
            dt.Columns.Add("PicIn1", typeof(string));
            dt.Columns.Add("PicIn2", typeof(string));
            dt.Columns.Add("PicOut1", typeof(string));
            dt.Columns.Add("PicOut2", typeof(string));
            dt.Columns.Add("CardGroupID", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("LaneIDIn", typeof(string));
            dt.Columns.Add("LaneIDOut", typeof(string));
            dt.Columns.Add("UserIDIn", typeof(string));
            dt.Columns.Add("UserIDOut", typeof(string));
            dt.Columns.Add("Moneys", typeof(string));
            dt.Columns.Add("ID", typeof(string));
            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            //card
            DataTable dtevent = null;
            string st = "";
            if (IsFilterByTimeIn == true)
            {
                st = "select * from tblCardEvent where IsDelete=0 and EventCode='2' and" +
                    " DatetimeIn>='" + _fromdate +
                    "' and DatetimeIn<='" + _todate +
                    "' and CardGroupID LIKE '" + CardGroupID +
                    "%' and (LaneIDIn LIKE '%" + LaneID +
                    "%' or LaneIDOut LIKE '%" + LaneID +
                    "%') and (UserIDIn LIKE '%" + UserID +
                    "%' or UserIDOut LIKE '%" + UserID +
                    "%')" +
                    " and (CardNumber LIKE '%" + KeyWord +
                    "%' or CardNo LIKE '%" + KeyWord +
                    "%' or PlateIn LIKE N'%" + KeyWord +
                    "%' or PlateOut LIKE N'%" + KeyWord +
                    "%') order by DatetimeIn desc";
            }
            else
            {
                st = "select * from tblCardEvent where IsDelete=0 and EventCode='2' and" +
                    " DatetimeOut>='" + _fromdate +
                    "' and DatetimeOut<='" + _todate +
                    "' and CardGroupID LIKE '" + CardGroupID +
                    "%' and (LaneIDIn LIKE '%" + LaneID +
                    "%' or LaneIDOut LIKE '%" + LaneID +
                    "%') and (UserIDIn LIKE '%" + UserID +
                    "%' or UserIDOut LIKE '%" + UserID +
                    "%')" +
                    " and (CardNumber LIKE '%" + KeyWord +
                    "%' or CardNo LIKE '%" + KeyWord +
                    "%' or PlateIn LIKE N'%" + KeyWord +
                    "%' or PlateOut LIKE N'%" + KeyWord +
                    "%') order by DatetimeOut desc";
            }

            dtevent = StaticPool.mdbevent.FillData(st);
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    string _cardnumber = dr["CardNumber"].ToString();
                    string _platein = dr["PlateIn"].ToString();
                    string _plateout = dr["PlateOut"].ToString();

                    string _dtimein = dr["DatetimeIn"].ToString();
                    string _dtimeout = dr["DatetimeOut"].ToString();
                    string _picIn1 = dr["PicDirIn"].ToString();
                    string _picIn2 = dr["PicDirIn"].ToString().Replace("PLATEIN.JPG", "OVERVIEWIN.JPG");
                    string _picOut1 = dr["PicDirOut"].ToString();
                    string _picOut2 = dr["PicDirOut"].ToString().Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
                    string _cardgroupid = dr["CardGroupID"].ToString();
                    string _customername = dr["CustomerName"].ToString();
                    string _laneidin = dr["LaneIDIn"].ToString();
                    string _laneidout = dr["LaneIDOut"].ToString();
                    string _moneys = dr["Moneys"].ToString();
                    string _useridin = dr["UserIDIn"].ToString();
                    string _useridout = dr["UserIDOut"].ToString();

                    if (_platein != _plateout || string.IsNullOrWhiteSpace(_platein) || string.IsNullOrWhiteSpace(_plateout))
                    {
                        dt.Rows.Add(dr["CardNo"].ToString(), _cardnumber, _platein, _plateout, _dtimein, _dtimeout, _picIn1, _picIn2, _picOut1, _picOut2, _cardgroupid, _customername, _laneidin, _laneidout, _useridin, _useridout, _moneys, dr["Id"].ToString());
                    }
                }
            }

            //loop
            if (StaticPool.SystemUsingLoop() == true)
            {
                DataTable dtloopevent = null;

                if (IsFilterByTimeIn)
                {
                    st = "select * from tblLoopEvent where IsDelete=0 and EventCode='2' and" +
                        " DateTimeIn>='" + _fromdate +
                        "' and DateTimeIn<='" + _todate +
                        "' and CarType LIKE '%" + CardGroupID +
                        "%' and (LaneIDIn LIKE '%" + LaneID +
                        "%' or LaneIDOut LIKE '%" + LaneID +
                        "%') and (UserIDIn LIKE '%" + UserID +
                        "%' or UserIDOut LIKE '%" + UserID +
                        "%')" +
                        " and Plate LIKE N'%" + KeyWord +
                        "%' order by DatetimeIn desc";
                }
                else
                {
                    st = "select * from tblLoopEvent where IsDelete=0 and EventCode='2' and" +
                        " DateTimeOut>='" + _fromdate +
                        "' and DateTimeOut<='" + _todate +
                        "' and CarType LIKE '%" + CardGroupID +
                        "%' and (LaneIDIn LIKE '%" + LaneID +
                        "%' or LaneIDOut LIKE '%" + LaneID +
                        "%') and (UserIDIn LIKE '%" + UserID +
                        "%' or UserIDOut LIKE '%" + UserID +
                        "%')" +
                        " and Plate LIKE N'%" + KeyWord +
                        "%' order by DatetimeOut desc";
                }

                dtloopevent = StaticPool.mdbevent.FillData(st);
                if (dtloopevent != null && dtloopevent.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtloopevent.Rows)
                    {
                        string _cardnumber = "";
                        string _plate = dr["Plate"].ToString();

                        string _dtimein = dr["DatetimeIn"].ToString();
                        string _dtimeout = dr["DatetimeOut"].ToString();
                        string _picIn1 = dr["PicDirIn"].ToString();
                        string _picIn2 = dr["PicDirIn"].ToString().Replace("PLATEIN.JPG", "OVERVIEWIN.JPG");
                        string _picOut1 = dr["PicDirOut"].ToString();
                        string _picOut2 = dr["PicDirOut"].ToString().Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
                        string _cardgroupid = dr["CarType"].ToString();
                        string _customername = dr["CustomerName"].ToString();
                        string _laneidin = dr["LaneIDIn"].ToString();
                        string _laneidout = dr["LaneIDOut"].ToString();
                        string _moneys = dr["Moneys"].ToString();
                        string _useridin = dr["UserIDIn"].ToString();
                        string _useridout = dr["UserIDOut"].ToString();

                        dt.Rows.Add("", _cardnumber, _plate, _dtimein, _dtimeout, _picIn1, _picIn2, _picOut1, _picOut2, _cardgroupid, _customername, _laneidin, _laneidout, _useridin, _useridout, _moneys, dr["Id"].ToString());
                    }
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

    //export all page
    protected void Excel_Click(object sender, EventArgs e)
    {
        try
        {
            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
                //txtKeyWord.Value = KeyWord;
            }

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"].ToString();
                // cbCardGroup.Value = CardGroupID;
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

            if (Request.QueryString["IsFilterByTimeIn"] != null && Request.QueryString["IsFilterByTimeIn"].ToString() != "")
            {
                IsFilterByTimeIn = bool.Parse(Request.QueryString["IsFilterByTimeIn"].ToString());
                //chFilterByTimeIn.Checked = IsFilterByTimeIn;
                //chFilterByTimeOut.Checked = !chFilterByTimeIn.Checked;
            }

            if (Request.QueryString["LaneID"] != null && Request.QueryString["LaneID"].ToString() != "")
            {
                LaneID = Request.QueryString["LaneID"].ToString();
                //cbLane.Value = LaneID;
            }
            if (Request.QueryString["UserID"] != null && Request.QueryString["UserID"].ToString() != "")
            {
                UserID = Request.QueryString["UserID"].ToString();
                //cbUser.Value = UserID;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("CardNo", typeof(string));
            dt.Columns.Add("Mã thẻ", typeof(string));
            dt.Columns.Add("Biển số ra", typeof(string));
            dt.Columns.Add("Biển số vào", typeof(string));
            dt.Columns.Add("Thời gian vào", typeof(string));
            dt.Columns.Add("Thời gian ra", typeof(string));
            //dt.Columns.Add("PicIn1", typeof(string));
            //dt.Columns.Add("PicIn2", typeof(string));
            //dt.Columns.Add("PicOut1", typeof(string));
            //dt.Columns.Add("PicOut2", typeof(string));
            dt.Columns.Add("Nhóm thẻ", typeof(string));
            dt.Columns.Add("Khách hàng", typeof(string));
            dt.Columns.Add("Làn vào", typeof(string));
            dt.Columns.Add("Làn ra", typeof(string));
            dt.Columns.Add("Giám sát vào", typeof(string));
            dt.Columns.Add("Giám sát ra", typeof(string));
            dt.Columns.Add("Số tiền", typeof(string));
            //dt.Columns.Add("ID", typeof(string));
            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";


            //card
            DataTable dtevent = null;
            string st = "";
            if (IsFilterByTimeIn == true)
            {
                st = "select * from tblCardEvent where IsDelete=0 and EventCode='2' and" +
                    " DatetimeIn>='" + _fromdate +
                    "' and DatetimeIn<='" + _todate +
                    "' and CardGroupID LIKE '" + CardGroupID +
                    "%' and (LaneIDIn LIKE '%" + LaneID +
                    "%' or LaneIDOut LIKE '%" + LaneID +
                    "%') and (UserIDIn LIKE '%" + UserID +
                    "%' or UserIDOut LIKE '%" + UserID +
                    "%')" +
                    " and (CardNumber LIKE '%" + KeyWord +
                    "%' or CardNo LIKE '%" + KeyWord +
                    "%' or PlateIn LIKE N'%" + KeyWord +
                    "%' or PlateOut LIKE N'%" + KeyWord +
                    "%') order by DatetimeIn desc";
            }
            else
            {
                st = "select * from tblCardEvent where IsDelete=0 and EventCode='2' and" +
                    " DatetimeOut>='" + _fromdate +
                    "' and DatetimeOut<='" + _todate +
                    "' and CardGroupID LIKE '" + CardGroupID +
                    "%' and (LaneIDIn LIKE '%" + LaneID +
                    "%' or LaneIDOut LIKE '%" + LaneID +
                    "%') and (UserIDIn LIKE '%" + UserID +
                    "%' or UserIDOut LIKE '%" + UserID +
                    "%')" +
                    " and (CardNumber LIKE '%" + KeyWord +
                    "%' or CardNo LIKE '%" + KeyWord +
                    "%' or PlateIn LIKE N'%" + KeyWord +
                    "%' or PlateOut LIKE N'%" + KeyWord +
                    "%') order by DatetimeOut desc";
            }

            dtevent = StaticPool.mdbevent.FillData(st);
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    string _cardnumber = dr["CardNumber"].ToString();
                    string _platein = dr["PlateIn"].ToString();
                    string _plateout = dr["PlateOut"].ToString();

                    string _dtimein = this.GetDateTime(dr["DatetimeIn"].ToString());
                    string _dtimeout = this.GetDateTime(dr["DatetimeOut"].ToString());
                    //string _picIn1 = dr["PicDirIn"].ToString();
                    //string _picIn2 = dr["PicDirIn"].ToString().Replace("PLATEIN.JPG", "OVERVIEWIN.JPG");
                    //string _picOut1 = dr["PicDirOut"].ToString();
                    //string _picOut2 = dr["PicDirOut"].ToString().Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
                    string _cardgroupid = this.GetCardGroup(dr["CardGroupID"].ToString());
                    string _customername = dr["CustomerName"].ToString();
                    string _laneidin = this.GetLane(dr["LaneIDIn"].ToString());
                    string _laneidout = this.GetLane(dr["LaneIDOut"].ToString());
                    string _moneys = string.Format(new System.Globalization.CultureInfo("en-US"),"{0:0,0}", int.Parse(dr["Moneys"].ToString()));
                    string _useridin = this.GetUserName(dr["UserIDIn"].ToString());
                    string _useridout = this.GetUserName(dr["UserIDOut"].ToString());

                    if (_platein != _plateout || string.IsNullOrWhiteSpace(_platein) || string.IsNullOrWhiteSpace(_plateout)) {
                        dt.Rows.Add(dt.Rows.Count + 1, dr["CardNo"].ToString(), _cardnumber, _platein, _plateout, _dtimein, _dtimeout, _cardgroupid, _customername, _laneidin, _laneidout, _useridin, _useridout, _moneys);
                    }
                }
            }

            //loop
            if (StaticPool.SystemUsingLoop() == true)
            {
                DataTable dtloopevent = null;

                if (IsFilterByTimeIn)
                {
                    st = "select * from tblLoopEvent where IsDelete=0 and EventCode='2' and" +
                        " DateTimeIn>='" + _fromdate +
                        "' and DateTimeIn<='" + _todate +
                        "' and CarType LIKE '%" + CardGroupID +
                        "%' and (LaneIDIn LIKE '%" + LaneID +
                        "%' or LaneIDOut LIKE '%" + LaneID +
                        "%') and (UserIDIn LIKE '%" + UserID +
                        "%' or UserIDOut LIKE '%" + UserID +
                        "%')" +
                        " and Plate LIKE N'%" + KeyWord +
                        "%' order by DatetimeIn desc";
                }
                else
                {
                    st = "select * from tblLoopEvent where IsDelete=0 and EventCode='2' and" +
                        " DateTimeOut>='" + _fromdate +
                        "' and DateTimeOut<='" + _todate +
                        "' and CarType LIKE '%" + CardGroupID +
                        "%' and (LaneIDIn LIKE '%" + LaneID +
                        "%' or LaneIDOut LIKE '%" + LaneID +
                        "%') and (UserIDIn LIKE '%" + UserID +
                        "%' or UserIDOut LIKE '%" + UserID +
                        "%')" +
                        " and Plate LIKE N'%" + KeyWord +
                        "%' order by DatetimeOut desc";
                }

                dtloopevent = StaticPool.mdbevent.FillData(st);
                if (dtloopevent != null && dtloopevent.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtloopevent.Rows)
                    {
                        string _cardnumber = "";
                        string _plate = dr["Plate"].ToString();

                        string _dtimein = this.GetDateTime(dr["DatetimeIn"].ToString());
                        string _dtimeout = this.GetDateTime(dr["DatetimeOut"].ToString());
                        //string _picIn1 = dr["PicDirIn"].ToString();
                        //string _picIn2 = dr["PicDirIn"].ToString().Replace("PLATEIN.JPG", "OVERVIEWIN.JPG");
                        //string _picOut1 = dr["PicDirOut"].ToString();
                        //string _picOut2 = dr["PicDirOut"].ToString().Replace("PLATEOUT.JPG", "OVERVIEWOUT.JPG");
                        string _cardgroupid = this.GetCardGroup(dr["CarType"].ToString());
                        string _customername = dr["CustomerName"].ToString();
                        string _laneidin = this.GetLane(dr["LaneIDIn"].ToString());
                        string _laneidout = this.GetLane(dr["LaneIDOut"].ToString());
                        string _moneys = string.Format(new System.Globalization.CultureInfo("en-US"),"{0:0,0}", int.Parse(dr["Moneys"].ToString()));
                        string _useridin = this.GetUserName(dr["UserIDIn"].ToString());
                        string _useridout = this.GetUserName(dr["UserIDOut"].ToString());

                        dt.Rows.Add(dt.Rows.Count + 1, "", _cardnumber, _plate, _dtimein, _dtimeout, _cardgroupid, _customername, _laneidin, _laneidout, _useridin, _useridout, _moneys);
                    }
                }
            }

            string _title1 = "Báo cáo xe ra khỏi bãi";
            string _title2 = "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;

            GridView gvheader = StaticPool.CreateHeaderTable(_title1, _title2, StaticPool.GetUserName(ViewState["UserID"].ToString()));
            GridView gv = new GridView();
            gv.DataSource = dt;
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

            tb.Rows.Add(tr1);
            tb.Rows.Add(tr2);
            tb.Rows.Add(tr3);

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
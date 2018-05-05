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

public partial class QLXuatAn_ReportVehicleInAnyTimeHistory : System.Web.UI.Page
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

            if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Report_Report1", "Selects", "Parking") == false)
            {
                Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
            }

            div_alert.Visible = false;

            dtpFromDate.Value = FromDate = DateTime.Now.ToString("dd/MM/yyyy 00:00");
            dtpToDate.Value = ToDate = DateTime.Now.ToString("dd/MM/yyyy 23:59");

            //cardgroup
            //DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            //if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
            //{
            //    cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
            //    foreach (DataRow dr in dtCardGroup.Rows)
            //    {
            //        cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
            //    }


            //}

            //action


            //user



            BindDataList();

        }
    }

    public string GetCardGroup(string CardGroupID)
    {

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

    //public string GetVehicleGroupName(string vehiclegroupid)
    //{
    //    DataTable dt = StaticPool.mdb.FillData("select VehicleGroupName from tblVehicleGroup where VehicleGroupID='" + vehiclegroupid + "'");
    //    if (dt != null && dt.Rows.Count > 0)
    //        return dt.Rows[0]["VehicleGroupName"].ToString();
    //    return "";
    //}

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

            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
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
            //dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("VehicleGroupName", typeof(string));
            dt.Columns.Add("VehicleGroupID", typeof(string));
            dt.Columns.Add("Number", typeof(string));


            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00"); //dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = Convert.ToDateTime(dtpToDate.Value).ToString("yyyy/MM/dd HH:mm:59"); // dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            //DataTable dtevent = null;
            var queyFirst = new StringBuilder();
            queyFirst.AppendLine("SELECT * FROM(");
            queyFirst.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY SortOrder) AS RowNumber,VehicleGroupID, VehicleType, VehicleGroupName, SortOrder from tblVehicleGroup WHERE Inactive = 0");
            queyFirst.AppendLine(") as a");
            queyFirst.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, StaticPool.pagesize));

            queyFirst.AppendLine("SELECT COUNT(*) totalCount FROM tblVehicleGroup WHERE Inactive = 0");
            var total = 0;
            DataTable dtvehiclegroup = StaticPool.mdb.FillDataPaging_2(queyFirst.ToString(), ref total);
            if (dtvehiclegroup != null && dtvehiclegroup.Rows.Count > 0)
            {
                try
                {
                    var _total = 0;
                    foreach (DataRow dr in dtvehiclegroup.Rows)
                    {
                        int _totalVehicle = 0;//in but not out yet

                        var sbQuery = new StringBuilder();
                        sbQuery.AppendLine("select count(Id) from tblCardEvent where IsDelete=0" +
                            " and VehicleGroupID='" + dr["VehicleGroupID"].ToString() +
                            "' and ((DateTimeIn >='" + _fromdate +
                            "' and DateTimeIn <='" + _todate +
                            "' and DateTimeOut >'" + _todate +
                            "') OR (EventCode='1' and DateTimeIn >= '"+ _fromdate + "' and DateTimeIn <= '"+ _todate + "')) ");

                        DataTable dstemp = StaticPool.mdbevent.FillData(sbQuery.ToString());
                        if (dstemp != null && dstemp.Rows.Count > 0)
                        {
                            if (dstemp.Rows[0][0].ToString() != "0")
                                _totalVehicle = int.Parse(dstemp.Rows[0][0].ToString());
                        }

                        if (dr["VehicleType"].ToString() == "0")//car
                        {
                            if (StaticPool.SystemUsingLoop() == true)
                            {
                                sbQuery.Clear();
                                sbQuery.AppendLine("select count(Id) from tblLoopEvent where IsDelete=0" +
                                  "' and (DateTimeIn >='" + _fromdate +
                                    "' and DateTimeIn <='" + _todate +
                                    "' and DateTimeOut >'" + _todate +
                                  "') OR (EventCode='1' and DateTimeIn >= '" + _fromdate + "' and DateTimeIn <= '" + _todate + "') ");
                                dstemp = StaticPool.mdbevent.FillData(sbQuery.ToString());
                                if (dstemp != null && dstemp.Rows.Count > 0)
                                {
                                    if (dstemp.Rows[0][0].ToString() != "0")
                                        _totalVehicle += int.Parse(dstemp.Rows[0][0].ToString());

                                }
                            }
                        }
                        _total += _totalVehicle;
                        dt.Rows.Add(dr["VehicleGroupName"].ToString(), dr["VehicleGroupID"].ToString(), _totalVehicle!=0? _totalVehicle.ToString("###,###"):"0");
                    }
                    dt.Rows.Add("Tổng số", "", _total != 0 ? _total.ToString("###,###") : "0");

                }
                catch (Exception ex)
                {

                }
            }

            if (dt == null)
                return;

            if (dt != null && dt.Rows.Count > 0)
            {
                id_reportin.InnerText = "Số bản ghi (" + total + ")";
                //By HNG paging
                StaticPool.HNGpaging(dt, total, StaticPool.pagesize, pager, rpt_Card);
                //Bind resulted PageSource into the Repeater
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
            if (Request.QueryString["FromDate"] != null && Request.QueryString["FromDate"].ToString() != "")
            {
                FromDate = Request.QueryString["FromDate"].ToString();
                // dtpFromDate.Value = FromDate;
            }
            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(string));
            dt.Columns.Add("Nhóm xe", typeof(string));
            dt.Columns.Add("Số lượng", typeof(string));


            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = Convert.ToDateTime(dtpFromDate.Value).ToString("yyyy/MM/dd HH:mm:00");//dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            //string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            //DataTable dtevent = null;

            var queyFirst = new StringBuilder();
            //queyFirst.AppendLine("SELECT * FROM(");
            queyFirst.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY SortOrder) AS STT,VehicleGroupID, VehicleType, VehicleGroupName, SortOrder from tblVehicleGroup WITH (NOLOCK) WHERE Inactive = 0");
            //queyFirst.AppendLine(") as a");
            //queyFirst.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, StaticPool.pagesize));

            //queyFirst.AppendLine("SELECT COUNT(*) totalCount FROM tblVehicleGroup WHERE Inactive = 0");
            DataTable dtvehiclegroup = StaticPool.mdb.FillData(queyFirst.ToString());
            if (dtvehiclegroup != null && dtvehiclegroup.Rows.Count > 0)
            {
                //Kiểm tra có tồn tại tblCardEventHistory
                DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
                var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
                if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0) // Truong hop co tblCardEventHistory
                {
                    foreach (DataRow dr in dtvehiclegroup.Rows)
                    {
                        int _vehicle1 = 0;//in but not out yet
                        int _vehicle2 = 0;//in and out yet

                        var command1 = new StringBuilder();
                        command1.AppendLine("SELECT count(a.Id) FROM(");
                        command1.AppendLine("SELECT Id FROM [dbo].tblCardEvent ce WITH (NOLOCK) WHERE ce.[EventCode]='1'");
                        command1.AppendLine(string.Format("AND ce.[VehicleGroupID] = '{0}' AND ce.[DateTimeIn] <= '{1}'", dr["VehicleGroupID"].ToString(), _fromdate));
                        command1.AppendLine("Union");
                        command1.AppendLine("SELECT Id FROM [dbo].tblCardEventHistory ceh WITH (NOLOCK) WHERE ceh.[EventCode]='1'");
                        command1.AppendLine(string.Format("AND ceh.[VehicleGroupID] = '{0}' AND ceh.[DateTimeIn] <= '{1}'", dr["VehicleGroupID"].ToString(), _fromdate));
                        command1.AppendLine(") as a");

                        DataTable temp = StaticPool.mdbevent.FillData(command1.ToString());
                        if (temp != null && temp.Rows.Count > 0)
                        {
                            if (temp.Rows[0][0].ToString() != "")
                                _vehicle1 = int.Parse(temp.Rows[0][0].ToString());
                        }

                        var command2 = new StringBuilder();
                        command2.AppendLine("SELECT count(a.Id) FROM (");
                        command2.AppendLine("SELECT Id FROM [dbo].tblCardEvent ce WITH (NOLOCK) WHERE ce.[EventCode]='2'");
                        command2.AppendLine(string.Format("AND ce.[VehicleGroupID] = '{0}' AND ce.[DateTimeIn] <= '{1}' AND ce.[DateTimeOut] > '{1}'", dr["VehicleGroupID"].ToString(), _fromdate));
                        command2.AppendLine("Union");
                        command2.AppendLine("SELECT Id FROM [dbo].tblCardEventHistory ceh WITH (NOLOCK) WHERE ceh.[EventCode]='2'");
                        command2.AppendLine(string.Format("AND ceh.[VehicleGroupID] = '{0}' AND ceh.[DateTimeIn] <= '{1}' AND ceh.[DateTimeOut] > '{1}'", dr["VehicleGroupID"].ToString(), _fromdate));
                        command2.AppendLine(") as a");

                        temp = StaticPool.mdbevent.FillData(command2.ToString());

                        if (temp != null && temp.Rows.Count > 0)
                        {
                            if (temp.Rows[0][0].ToString() != "")
                                _vehicle2 = int.Parse(temp.Rows[0][0].ToString());
                        }

                        if (dr["VehicleType"].ToString() == "0")//car
                        {
                            if (StaticPool.SystemUsingLoop() == true)
                            {
                                temp = StaticPool.mdbevent.FillData("select count(Id) from tblLoopEvent WITH (NOLOCK) where EventCode='1'" +
                                    " and DateTimeIn<='" + _fromdate +
                                    "'");
                                if (temp != null && temp.Rows.Count > 0)
                                {
                                    if (temp.Rows[0][0].ToString() != "")
                                        _vehicle1 = _vehicle1 + int.Parse(temp.Rows[0][0].ToString());
                                }

                                temp = StaticPool.mdbevent.FillData("select count(Id) from tblLoopEvent WITH (NOLOCK) where EventCode='2' and IsDelete=0" +
                                  " and DateTimeIn<='" + _fromdate +
                                  "' and DateTimeOut>'" + _fromdate +
                                  "'");
                                if (temp != null && temp.Rows.Count > 0)
                                {
                                    if (temp.Rows[0][0].ToString() != "")
                                        _vehicle2 = _vehicle2 + int.Parse(temp.Rows[0][0].ToString());
                                }
                            }
                        }

                        dt.Rows.Add(dt.Rows.Count + 1, dr["VehicleGroupName"].ToString(), _vehicle1 + _vehicle2);
                    }
                }
                else // truong hop khong co tblCardEventHistory     
                {
                    foreach (DataRow dr in dtvehiclegroup.Rows)
                    {
                        int _vehicle1 = 0;//in but not out yet
                        int _vehicle2 = 0;//in and out yet

                        var sbQuery = new StringBuilder();
                        sbQuery.AppendLine("select count(Id) from tblCardEvent WITH (NOLOCK) where EventCode='1'" +
                            " and VehicleGroupID='" + dr["VehicleGroupID"].ToString() +
                            "' and DateTimeIn<='" + _fromdate +
                            "'");

                        sbQuery.AppendLine("select count(Id) from tblCardEvent WITH (NOLOCK) where EventCode='2' and IsDelete=0" +
                            " and VehicleGroupID='" + dr["VehicleGroupID"].ToString() +
                            "' and DateTimeIn<='" + _fromdate +
                            "' and DateTimeOut>'" + _fromdate +
                            "'");

                        DataSet dstemp = StaticPool.mdbevent.FillDataSet(sbQuery.ToString());
                        if (dstemp != null && dstemp.Tables.Count > 1)
                        {
                            if (dstemp.Tables[0].Rows[0][0].ToString() != "0")
                                _vehicle1 = int.Parse(dstemp.Tables[0].Rows[0][0].ToString());

                            if (dstemp.Tables[1].Rows[0][0].ToString() != "0")
                                _vehicle2 = int.Parse(dstemp.Tables[1].Rows[0][0].ToString());
                        }

                        if (dr["VehicleType"].ToString() == "0")//car
                        {
                            if (StaticPool.SystemUsingLoop() == true)
                            {
                                sbQuery.Clear();
                                sbQuery.AppendLine("select count(Id) from tblLoopEvent WITH (NOLOCK) where EventCode='1'" +
                                    " and DateTimeIn<='" + _fromdate +
                                    "'");
                                sbQuery.AppendLine("select count(Id) from tblLoopEvent WITH (NOLOCK) where EventCode='2' and IsDelete=0" +
                                  " and DateTimeIn<='" + _fromdate +
                                  "' and DateTimeOut>'" + _fromdate +
                                  "'");

                                if (dstemp != null && dstemp.Tables.Count > 1)
                                {
                                    if (dstemp.Tables[0].Rows[0][0].ToString() != "0")
                                        _vehicle1 = _vehicle1 + int.Parse(dstemp.Tables[0].Rows[0][0].ToString());

                                    if (dstemp.Tables[1].Rows[0][0].ToString() != "0")
                                        _vehicle2 = _vehicle2 + int.Parse(dstemp.Tables[1].Rows[0][0].ToString());
                                }
                            }
                        }

                        dt.Rows.Add(dt.Rows.Count + 1, dr["VehicleGroupName"].ToString(), _vehicle1 + _vehicle2);
                    }
                }
            }

            string _title1 = "Báo cáo xe trong bãi tại thời điểm bất kỳ";
            string _title2 = "Thời điểm: " + dtpFromDate.Value;

            try
            {
                BindDataToExcel(dt, _title1, _title2, ViewState["UserID"].ToString());
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
                Response.Redirect("~/QLXuatAn/ReportVehicleInAnyTime.aspx", false);
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
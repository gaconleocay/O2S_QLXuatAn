﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Data;

public partial class QLXuatAn_Log : System.Web.UI.Page
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

            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
                txtKeyWord.Value = KeyWord;
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
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("UserName", typeof(string));
            dt.Columns.Add("SubSystemCode", typeof(string));
            dt.Columns.Add("ObjectName", typeof(string));
            dt.Columns.Add("Actions", typeof(string));
            dt.Columns.Add("Description", typeof(string));
          

            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            string _fromdate = dtpFromDate.Value.Substring(6, 4) + "/" + dtpFromDate.Value.Substring(3, 2) + "/" + dtpFromDate.Value.Substring(0, 2) + " " + dtpFromDate.Value.Substring(11, 5);
            string _todate = dtpToDate.Value.Substring(6, 4) + "/" + dtpToDate.Value.Substring(3, 2) + "/" + dtpToDate.Value.Substring(0, 2) + " " + dtpToDate.Value.Substring(11, 5) + ":59";

            DataTable dtevent = null;
            dtevent = StaticPool.mdb.FillData("select * from tblLog where AppCode='Parking' and Actions<>'PrintReceipt' and" +
                " Date>='" + _fromdate +
                "' and Date<='" + _todate +
                "' and (UserName LIKE N'%"+KeyWord+
                "' or SubSystemCode LIKE N'%"+KeyWord+
                "' or ObjectName LIKE N'%"+KeyWord+
                "' or Actions LIKE N'%"+KeyWord+
                
                "') order by Date desc");
            if (dtevent != null && dtevent.Rows.Count > 0)
            {
                foreach (DataRow dr in dtevent.Rows)
                {
                    dt.Rows.Add(dr["Date"].ToString(),
                        dr["UserName"].ToString(),
                        dr["SubSystemCode"].ToString(),
                        dr["ObjectName"].ToString(),
                        dr["Actions"].ToString(),
                        dr["Description"].ToString());
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
}
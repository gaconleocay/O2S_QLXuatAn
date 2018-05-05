using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using System.Web.Script.Services;
using Futech.Helpers;
using System.Text;

public partial class QLXuatAn_ActiveCard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                string script = "$(document).ready(function () { $('[id*=LinkButton1]').click(); });";
                ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);

                div_alert.Visible = false;

                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";

                dtpExpireDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                // Nhom the
                DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup where CardType=0 order by SortOrder");
                if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                {
                    cbCardGroup.Items.Add(new ListItem("<< Tất cả nhóm thẻ >>", ""));
                    foreach (DataRow dr in dtCardGroup.Rows)
                    {
                        cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                    }
                }

                //Customergroup
                DataTable dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
                if (dtcustomergroup == null)
                {
                    dtcustomergroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
                    if (dtcustomergroup!=null && dtcustomergroup.Rows.Count > 0)
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

                // Khách hàng
                string CustomerGroupID = "";
                if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
                {
                    CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();

                }

                ////Lay khach hang theo nhom -> select
                //DataTable dtCustomer = StaticPool.mdb.FillData("select CustomerName, CustomerID from tblCustomer where CustomerGroupID LIKE N'%" + CustomerGroupID + "%' order by SortOrder");
                //if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                //{
                //    cbCustomer.Items.Add(new ListItem("<< Tất cả khách hàng >>", ""));
                //    foreach (DataRow dr in dtCustomer.Rows)
                //    {
                //        cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
                //    }
                //}



                GetCardList();




            }
            catch (Exception ex)
            {

            }
        }


    }

    private void GetCardList()
    {
        try
        {
            cbCardList.Items.Clear();
            if (Request.QueryString["KeyWord"] == null && Request.QueryString["CardGroupID"] == null &&
                Request.QueryString["CustomerID"] == null && Request.QueryString["CustomerGroupID"] == null)
            {
                return;
            }

            DataTable dtCard = null;
            string KeyWord = "", CardGroupID = "", CustomerID = "", CustomerGroupID = "";

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

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
                cbCustomerGroup.Value = CustomerGroupID;
            }
            if (Request.QueryString["cusName"] != null && Request.QueryString["cusName"].ToString() != "")
            {
                ViewState["cusName"] = Request.QueryString["cusName"].ToString();
            }
            var sb = new StringBuilder();
            sb.AppendLine("select ca.CardNo, ca.CardNumber, ca.CustomerID, ca.Plate1, ca.Plate2, ca.Plate3, ca.[ExpireDate], ca.CardGroupID, c.CustomerName ");
            sb.AppendLine(" from dbo.tblCard ca WITH (NOLOCK) ");
            sb.AppendLine(" LEFT JOIN dbo.tblCardGroup g ON ca.CardGroupID=g.CardGroupID and g.CardType=0  ");
            sb.AppendLine(" LEFT JOIN tblCustomer c ON ca.CustomerID = CONVERT(varchar(255),c.CustomerID)");
            
            sb.AppendLine(" where ca.IsDelete=0 and ca.IsLock=0");
            
            if (!string.IsNullOrWhiteSpace(KeyWord))
            {
                sb.AppendLine(" and (ca.CardNo LIKE N'%" + KeyWord + "%'");
                sb.AppendLine(" or ca.CardNumber LIKE N'%" + KeyWord + "%'");
                sb.AppendLine(" or ca.Plate1 LIKE N'%" + KeyWord + "%'");
                sb.AppendLine(" or ca.Plate2 LIKE N'%" + KeyWord + "%'");
                sb.AppendLine(" or ca.Plate3 LIKE N'%" + KeyWord + "%')");
            }

            if (!string.IsNullOrWhiteSpace(CardGroupID))
            {
                sb.AppendLine(" and ca.CardGroupID = '" + CardGroupID + "'");

            }
            if (!string.IsNullOrWhiteSpace(CustomerID))
            {
                sb.AppendLine(" and ca.CustomerID = '" + CustomerID + "'");
            }
            if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            {
                sb.AppendLine(" and c.CustomerGroupID='" + CustomerGroupID + "' ");
            }
            sb.AppendLine(" order by ca.SortOrder desc");

            dtCard = StaticPool.mdb.FillData(sb.ToString());

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCard.Rows)
                {
                    //string _customername = "";

                    //DataTable temp = StaticPool.mdb.FillData("select CustomerID, CustomerGroupID, CustomerName from tblCustomer where CustomerID='" + dr["CustomerName"].ToString() + "'");
                    //if (temp != null && temp.Rows.Count > 0)
                    //{
                    //    if (CustomerGroupID != "")
                    //    {
                    //        if (temp.Rows[0]["CustomerGroupID"].ToString() != CustomerGroupID)
                    //            continue;
                    //    }
                    //    _customername = temp.Rows[0]["CustomerName"].ToString();
                    //}
                    //else if (temp == null || (temp != null && temp.Rows.Count == 0))
                    //{
                    //    if (CustomerGroupID != "")
                    //        continue;
                    //}




                    string _cardnumber = dr["CardNumber"].ToString();
                    string _carddesription = dr["CardNumber"].ToString() + " * " +
                                             dr["CardNo"].ToString() + " * " + string.Format("{0}",
                                                 !string.IsNullOrWhiteSpace(dr["Plate1"].ToString())
                                                     ? dr["Plate1"].ToString()
                                                     : "");
                        
                    if (!string.IsNullOrWhiteSpace(dr["Plate2"].ToString()))
                        _carddesription = _carddesription + ";" + dr["Plate2"].ToString();
                    if ( !string.IsNullOrWhiteSpace(dr["Plate3"].ToString()))
                        _carddesription = _carddesription + ";" + dr["Plate3"].ToString();

                    string _expiredate = !string.IsNullOrWhiteSpace(dr["ExpireDate"].ToString())
                        ? DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd-MM-yyyy")
                        : "";

                    _carddesription = _carddesription + " * " + _expiredate + " * " + string.Format("{0}",!string.IsNullOrWhiteSpace(dr["CustomerName"].ToString())? dr["CustomerName"].ToString():"");

                    cbCardList.Items.Add(new ListItem(_carddesription, _cardnumber));

                }
            }

        }
        catch (Exception ex)
        {

        }
    }

    public static int PerCent { get; set; }

    public string GetComputerName(string PCID)
    {
        try
        {
            DataTable dt = StaticPool.mdb.FillData("select * from dbo.tblPC where PCID = '" + PCID + "'");
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["ComputerName"].ToString() + " (" + dt.Rows[0]["IPAddress"].ToString() + ")";
            else
                return "";
        }
        catch
        {
            return "";
        }
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
            if (StaticPool.CheckPermission(userid, "Parking_Device_Camera", "Deletes", "Parking"))
                return StaticPool.mdb.ExecuteCommand("delete from tblCamera where CameraID = '" + id + "'").ToString().ToLower();
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            //System.Threading.Thread.Sleep(5000);
            //LinkButton1.Enabled = false;
            //div_refresh.EnableViewState = false;
            DateTime dtime = DateTime.Now;
            //ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);

            string _newexpire = Convert.ToDateTime(dtpExpireDate.Value).ToString("yyyy-MM-dd"); //dtpExpireDate.Value.Substring(6, 4) + "/" + dtpExpireDate.Value.Substring(3, 2) + "/" + dtpExpireDate.Value.Substring(0, 2);

            if (!string.IsNullOrEmpty(hidValueSelected.Value))
            {
                var arrSelected = hidValueSelected.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var listCard = "";
                var count = 0;
                foreach (var item in arrSelected)
                {
                    count++;
                    listCard += string.Format("'{0}'{1}", item, count != arrSelected.Length ? "," : "");
                }


                var sb = new StringBuilder();
                int _feelevel = int.Parse(txtFee.Text.Replace(",", ""));
                sb.AppendLine("INSERT INTO tblActiveCard(Code, [Date], CardNumber, Plate, OldExpireDate, [Days], NewExpireDate, CardGroupID, CustomerGroupID, UserID, FeeLevel, CustomerID,IsDelete)");
                sb.AppendLine("SELECT CASE WHEN cus.CustomerCode IS NULL THEN '0' ELSE cus.CustomerCode END,GETDATE(), ca.Cardnumber");
                sb.AppendLine(", CAST(CASE WHEN ca.Plate2 <> '' THEN ca.Plate1 + ';' + ca.Plate2 WHEN ca.Plate3 <> '' THEN ca.Plate1 + ';' + ca.Plate2 + ';' + ca.Plate3 WHEN ca.Plate1 IS NULL THEN '' ELSE ca.Plate1 END AS nvarchar(50)) as Plate");
                sb.AppendLine(string.Format(", ca.[ExpireDate], DATEDIFF(DAY, ca.[ExpireDate], '{0}')", _newexpire));
                sb.AppendLine(string.Format(", '{0}', ca.CardGroupID, CASE WHEN  cus.CustomerGroupID IS NULL THEN '0' ELSE cus.CustomerGroupID END,'{2}','{1}', CASE WHEN ca.CustomerID IS NULL THEN '0' ELSE ca.CustomerID END,0", _newexpire, _feelevel, ViewState["UserID"].ToString()));
                sb.AppendLine("from tblCard ca");
                sb.AppendLine("LEFT join tblCustomer cus on ca.CustomerID = CONVERT(varchar(255), cus.CustomerID)");
                sb.AppendLine(string.Format("where ca.IsDelete = 0 and ca.CardNumber IN ({0})", listCard));

                //Neu so ngay gia han <0 va neu ko check thi ko cho gia han
                if (chbEnableMinusActive.Checked == false)
                {
                    sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
                }
                sb.AppendLine(string.Format("update tblCard set [ExpireDate] = '{1}' where IsDelete = 0 and CardNumber IN ({0})", listCard, _newexpire));
                if (chbEnableMinusActive.Checked == false)
                {
                    sb.AppendLine(string.Format("and DATEDIFF(DAY, [ExpireDate], '{0}') >=0  AND [ExpireDate] <= '{0}'", _newexpire));
                }
                StaticPool.mdb.FillData(sb.ToString());

                //foreach (var item in arrSelected)
                //{
                //    string _cardnumber = item;
                //    string _code = "";
                //    string _plate = "";
                //    DateTime _oldexpire = DateTime.Now;
                //    int _days = 0;
                //    string _cardgroupid = "";
                //    string _customergroupid = "";

                //    

                //    DataTable dtcard = StaticPool.mdb.FillData("select Cardnumber, Plate1, Plate2, Plate3, ExpireDate, CardGroupID, CustomerID from tblCard where IsDelete=0 and CardNumber='" + _cardnumber + "'");
                //    if (dtcard != null && dtcard.Rows.Count > 0)
                //    {
                //        DataRowView drv = dtcard.DefaultView[0];
                //        _plate = drv["Plate1"].ToString();
                //        if (drv["Plate2"].ToString() != "")
                //            _plate = _plate + ";" + drv["Plate2"].ToString();
                //        if (drv["Plate3"].ToString() != "")
                //            _plate = _plate + ";" + drv["Plate3"].ToString();

                //        _oldexpire = DateTime.Parse(drv["ExpireDate"].ToString());

                //        TimeSpan ts = DateTime.Parse(_newexpire).Subtract(_oldexpire);
                //        _days = ts.Days;

                //        _cardgroupid = drv["CardGroupID"].ToString();

                //        string _customerid = drv["CustomerID"].ToString();
                //        if (_customerid != "")
                //        {
                //            DataTable temp = StaticPool.mdb.FillData("select CustomerGroupID, CustomerCode from tblCustomer where CustomerID='" + _customerid + "'");
                //            if (temp != null && temp.Rows.Count > 0)
                //            {
                //                _customergroupid = temp.Rows[0]["CustomerGroupID"].ToString();
                //                _code = temp.Rows[0]["CustomerCode"].ToString();
                //            }
                //        }

                //        if (_days < 0 && chbEnableMinusActive.Checked == false)
                //            continue;

                //        if (StaticPool.mdb.ExecuteCommand("insert into tblActiveCard(Code, Date, CardNumber, Plate, OldExpireDate, Days, NewExpireDate, CardGroupID" +
                //            ", CustomerGroupID, UserID, FeeLevel, CustomerID) values('" +
                //            _code + "', '" +
                //            dtime.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                //            _cardnumber + "', N'" +
                //            _plate + "', '" +
                //            _oldexpire.ToString("yyyy/MM/dd") + "', '" +
                //            _days + "', '" +
                //            _newexpire + "', '" +
                //            _cardgroupid + "', '" +
                //            _customergroupid + "', '" +
                //            ViewState["UserID"].ToString() + "', '" +
                //            _feelevel + "', '" +
                //            _customerid +
                //            "')"))
                //        {
                //            StaticPool.mdb.ExecuteCommand("update tblCard set ExpireDate='" + _newexpire + "' where IsDelete=0 and CardNumber='" + _cardnumber + "'");
                //            //check again

                //        }


                //    }
                //}

                div_alert.Visible = true;
                id_alert.InnerText = "Đã xử lý xong";
            }

        }
        catch (Exception ex)
        {
            div_alert.Visible = true;
            div_alert.InnerText = ex.Message;
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static List<string> GetFees(string prefix)
    {
        List<string> fees = new List<string>();


        DataTable dt = StaticPool.mdb.FillData("select * from tblFee where FeeLevel LIKE N'%" + prefix + "%'");
        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                fees.Add(dr["FeeLevel"].ToString());
            }
        }


        return fees;

    }


    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public static List<string> GetCustomerByGroup(string customergroup)
    //{
    //    try
    //    {
    //        List<string> customers = new List<string>();
    //        DataTable dt = StaticPool.mdb.FillData("select CustomerID, CustomerName from tblCustomer where CustomerGroup LIKE N'%" + customergroup + "'");
    //        if (dt != null && dt.Rows.Count > 0)
    //        {
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                string st = dr["CustomerID"].ToString() + ";" + dr["CustomerName"].ToString();
    //                customers.Add(st);
    //            }

    //            return customers;
    //        }
    //    }
    //    catch
    //    {
    //    }
    //    return null;
    //}
    [WebMethod]
    public static string GetCustomerByGroup(string customergroupid)
    {
        try
        {

            string customers = "";

            DataTable dt = StaticPool.mdb.FillData("select CustomerID, CustomerName from tblCustomer where CustomerGroupID LIKE N'%" + customergroupid + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string st = dr["CustomerID"].ToString() + ";" + dr["CustomerName"].ToString();

                    if (customers == "")
                        customers = st;
                    else
                        customers = customers + "#" + st;

                }

                return customers;

            }
        }
        catch
        {
        }

        return "";
    }

    [WebMethod]
    public static int GetPercent()
    {
        return PerCent;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        System.Threading.Thread.Sleep(5000);
    }
}
using System;
using System.Web.UI.WebControls;
using System.Data;
using Futech.Helpers;
using System.Text;
using System.Web.Services;
using System.Web.Script.Services;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;

public partial class QLXuatAn_ActiveCard_v2 : System.Web.UI.Page
{

    static DataTable dtCusMap = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            //CacheLayer.Clear(StaticCached.c_cardActiveChoice); // xoa cached trong list active moi lan F5 trang
            try
            {
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
                    if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
                        CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtcustomergroup, StaticCached.TimeCache);
                }

                if (dtcustomergroup != null && dtcustomergroup.Rows.Count > 0)
                {
                    cbCustomerGroup.Items.Add(new ListItem("<<Tất cả nhóm KH>>", ""));
                    foreach (DataRow dr in dtcustomergroup.Rows)
                    {
                        cbCustomerGroup.Items.Add(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
                    }
                }

                //Get data
                GetCardList();
            }
            catch (Exception ex)
            {

            }
        }
        //Get Data list choice
        ViewState["totalCardChoice"] = 0;
        var dtCardActive = (DataTable)Session[StaticCached.c_cardActiveChoice]; //CacheLayer.Get<DataTable>(StaticCached.c_cardActiveChoice);

        if (dtCardActive != null && dtCardActive.Rows.Count > 0)
        {
            ViewState["totalCardChoice"] = dtCardActive.Rows.Count;
            Button2.Text = "Gia hạn " + dtCardActive.Rows.Count + " thẻ";
            rptCardChoice.DataSource = dtCardActive;
            rptCardChoice.DataBind();
        }
        else
        {
            Button2.Text = "Chọn ít nhất 1 thẻ để gia hạn";
        }

    }

    public static string GetCusName(string cusIs)
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
    private void GetCardList()
    {
        try
        {
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

            var pageIndex = 1;
            if (Request.QueryString["Page"] != null)
            {
                pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
            }
            var totalCount = 0;
            dtCard = ReportService.GetCardExprice(KeyWord, CardGroupID, CustomerID, CustomerGroupID, pageIndex, StaticPool.pagesize, ref totalCount);

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                var listCusId = "";
                foreach (DataRow item in dtCard.Rows)
                {
                    listCusId += item["CustomerID"] + ",";
                }
                dtCusMap = ReportService.GetCardByCustomerId(listCusId);

                foreach (DataRow dr in dtCard.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(dr["Plate2"].ToString()))
                        dr["Plate1"] += " / " + dr["Plate2"].ToString();
                    if (!string.IsNullOrWhiteSpace(dr["Plate3"].ToString()))
                        dr["Plate1"] += " / " + dr["Plate3"].ToString();

                    dr["CustomerName"] = GetCusName(dr["CustomerID"].ToString());
                    //string _expiredate = !string.IsNullOrWhiteSpace(dr["ExpireDate"].ToString())
                    //    ? DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd-MM-yyyy")
                    //    : "";

                }

                ViewState["totalCard"] = totalCount;
                btnSaveAll.Text = string.Format("Gia hạn {0} thẻ", Convert.ToDecimal(totalCount).ToString("##,###"));
                //By HNG paging
                StaticPool.HNGpaging(dtCard, totalCount, StaticPool.pagesize, pager, rptListCard);
                rptListCard.DataSource = dtCard;
                rptListCard.DataBind();
            }

        }
        catch (Exception ex)
        {

        }
    }

    public string classDateExprice(DateTime exPrice)
    {
        var str = "";

        if (exPrice.Date < DateTime.Now.Date)
        {
            str = "label label-danger";
        }
        return str;

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

    [WebMethod]
    public static string getSearchCardActiveAutocomplete(string KeyWord, string CardGroupID, string CustomerGroupID)
    {
        DataTable dtCard = null;
        var str = "{[]}";
        try
        {

            dtCard = ReportService.GetCardExprice_Autocomplete(KeyWord, CardGroupID, CustomerGroupID);

            if (dtCard != null && dtCard.Rows.Count > 0)
            {
                var listCusId = "";
                foreach (DataRow item in dtCard.Rows)
                {
                    listCusId += item["CustomerID"] + ",";
                }
                dtCusMap = ReportService.GetCardByCustomerId(listCusId);
                dtCard.Columns.Add("ExpireDateHtml");
                foreach (DataRow dr in dtCard.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(dr["Plate2"].ToString()))
                        dr["Plate1"] += " / " + dr["Plate2"].ToString();
                    if (!string.IsNullOrWhiteSpace(dr["Plate3"].ToString()))
                        dr["Plate1"] += " / " + dr["Plate3"].ToString();

                    dr["CustomerName"] = GetCusName(dr["CustomerID"].ToString());
                    if (!string.IsNullOrWhiteSpace(dr["ExpireDate"].ToString()) &&
                        Convert.ToDateTime(dr["ExpireDate"].ToString()) < DateTime.Now.Date)
                    {
                        dr["ExpireDateHtml"] = string.Format("<span class='label label-danger'>{0}</span>",
                            Convert.ToDateTime(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        dr["ExpireDateHtml"] = string.Format("<span>{0}</span>",
                            Convert.ToDateTime(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy"));
                    }
                }

            }
            str = StaticPool.DataTableToJsonObj(dtCard);
        }
        catch (Exception ex)
        {

        }
        return str.Trim();
    }



    [WebMethod]
    public static string addCardToChoice(string listCardId)
    {
        if (string.IsNullOrWhiteSpace(listCardId))
        {
            return StaticPool.DataTableToJsonObj(new DataTable());
        }


        var dtCardActive = (DataTable)HttpContext.Current.Session[StaticCached.c_cardActiveChoice]; //CacheLayer.Get<DataTable>(StaticCached.c_cardActiveChoice);
        if (dtCardActive == null)
        {
            dtCardActive = new DataTable();
            dtCardActive.Columns.Add("CardNumber");
            dtCardActive.Columns.Add("CardNo");
            dtCardActive.Columns.Add("Plate1");
            dtCardActive.Columns.Add("ExpireDate");
            dtCardActive.Columns.Add("CustomerName");
        }

        var dtListCardInfo = new DataTable();
        dtListCardInfo.Columns.Add("CardNumber");
        dtListCardInfo.Columns.Add("CardNo");
        dtListCardInfo.Columns.Add("Plate1");
        dtListCardInfo.Columns.Add("ExpireDate");
        dtListCardInfo.Columns.Add("ExpireDateHtml");
        dtListCardInfo.Columns.Add("CustomerName");

        var arrCardAdd = listCardId.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (dtCardActive != null && dtCardActive.Rows.Count > 0)
        {
            if (arrCardAdd.Length > 0)
            {
                for (int i = 0; i < arrCardAdd.Length; i++)
                {
                    var arrItem = arrCardAdd[i].Split(',');
                    if (arrItem.Length > 0)
                    {
                        var checkRow = dtCardActive.Select(string.Format("CardNumber = '{0}'", arrItem[0]));
                        //KT xem da co CardNumber trong danh sach chua
                        if (checkRow != null && checkRow.Length > 0)
                            continue;

                        var aRow = dtCardActive.NewRow();
                        aRow["CardNumber"] = arrItem[0];
                        aRow["CardNo"] = arrItem[1];
                        aRow["Plate1"] = arrItem[2];
                        aRow["ExpireDate"] = arrItem[3];
                        aRow["CustomerName"] = arrItem[4];
                        dtCardActive.Rows.Add(aRow);

                        var bRow = dtListCardInfo.NewRow();
                        bRow["CardNumber"] = arrItem[0];
                        bRow["CardNo"] = arrItem[1];
                        bRow["Plate1"] = arrItem[2];
                        bRow["ExpireDate"] = arrItem[3];
                        bRow["CustomerName"] = arrItem[4];
                        if (!string.IsNullOrWhiteSpace(bRow["ExpireDate"].ToString()) &&
                        Convert.ToDateTime(bRow["ExpireDate"].ToString()) < DateTime.Now.Date)
                        {
                            bRow["ExpireDateHtml"] = string.Format("<span class='label label-danger'>{0}</span>",
                                Convert.ToDateTime(bRow["ExpireDate"].ToString()).ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            bRow["ExpireDateHtml"] = string.Format("<span>{0}</span>",
                                Convert.ToDateTime(bRow["ExpireDate"].ToString()).ToString("dd/MM/yyyy"));
                        }
                        dtListCardInfo.Rows.Add(bRow);
                    }
                }

            }
        }
        else // add moi hoan toan
        {
            if (arrCardAdd.Length > 0)
            {
                for (int i = 0; i < arrCardAdd.Length; i++)
                {
                    var arrItem = arrCardAdd[i].Split(',');
                    if (arrItem.Length > 0)
                    {
                        var aRow = dtCardActive.NewRow();
                        aRow["CardNumber"] = arrItem[0];
                        aRow["CardNo"] = arrItem[1];
                        aRow["Plate1"] = arrItem[2];
                        aRow["ExpireDate"] = arrItem[3];
                        aRow["CustomerName"] = arrItem[4];
                        dtCardActive.Rows.Add(aRow);

                        var bRow = dtListCardInfo.NewRow();
                        bRow["CardNumber"] = arrItem[0];
                        bRow["CardNo"] = arrItem[1];
                        bRow["Plate1"] = arrItem[2];
                        bRow["ExpireDate"] = arrItem[3];
                        bRow["CustomerName"] = arrItem[4];
                        if (!string.IsNullOrWhiteSpace(bRow["ExpireDate"].ToString()) &&
                        Convert.ToDateTime(bRow["ExpireDate"].ToString()) < DateTime.Now.Date)
                        {
                            bRow["ExpireDateHtml"] = string.Format("<span class='label label-danger'>{0}</span>",
                                Convert.ToDateTime(bRow["ExpireDate"].ToString()).ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            bRow["ExpireDateHtml"] = string.Format("<span>{0}</span>",
                                Convert.ToDateTime(bRow["ExpireDate"].ToString()).ToString("dd/MM/yyyy"));
                        }
                        dtListCardInfo.Rows.Add(bRow);
                    }
                }

            }
        }

        // add cached
        if (dtCardActive != null && dtCardActive.Rows.Count > 0)
        {
            HttpContext.Current.Session[StaticCached.c_cardActiveChoice] = dtCardActive;
            //CacheLayer.Add(StaticCached.c_cardActiveChoice, dtCardActive, StaticCached.TimeCache);
        }
        // lay lai thong moi nhat dc add de them vao danh sach html
        var str = StaticPool.DataTableToJsonObj(dtListCardInfo); //JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);

        return str.Trim();
    }

    /// <summary>
    /// Xoa cac the da chon trong list
    /// </summary>
    /// <param name="id">CardNumber</param>
    /// <param name="chk">Xoa all hay xoa tung ban ghi</param>
    /// <returns></returns>
    [WebMethod]
    public static string deleteCardChoice(string id, bool chk)
    {
        var str = "";
        var dtCardActive = (DataTable)HttpContext.Current.Session[StaticCached.c_cardActiveChoice]; //CacheLayer.Get<DataTable>(StaticCached.c_cardActiveChoice);
        try
        {
            if (dtCardActive != null && dtCardActive.Rows.Count > 0) // xoa all
            {
                if (chk) // xoa tat ca
                {
                    dtCardActive = null;
                    dtCardActive = new DataTable();
                    dtCardActive.Columns.Add("CardNumber");
                    dtCardActive.Columns.Add("CardNo");
                    dtCardActive.Columns.Add("Plate1");
                    dtCardActive.Columns.Add("ExpireDate");
                    dtCardActive.Columns.Add("CustomerName");
                    HttpContext.Current.Session[StaticCached.c_cardActiveChoice] = null;//CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                }
                else // xoa theo id
                {
                    var newData = dtCardActive.Select(string.Format("CardNumber <>'{0}'", id));
                    if (newData.Length > 0)
                    {
                        dtCardActive = newData.CopyToDataTable();
                        //CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                        //CacheLayer.Add(StaticCached.c_cardActiveChoice, dtCardActive, StaticCached.TimeCache);
                        HttpContext.Current.Session[StaticCached.c_cardActiveChoice] = dtCardActive;
                    }
                    else
                    {
                        dtCardActive = null;
                        dtCardActive = new DataTable();
                        dtCardActive.Columns.Add("CardNumber");
                        dtCardActive.Columns.Add("CardNo");
                        dtCardActive.Columns.Add("Plate1");
                        dtCardActive.Columns.Add("ExpireDate");
                        dtCardActive.Columns.Add("CustomerName");
                        HttpContext.Current.Session[StaticCached.c_cardActiveChoice] = null; //CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            str = ex.Message;
        }
        str = StaticPool.DataTableToJsonObj(dtCardActive);
        return str.Trim();
    }
    /// <summary>
    /// Gia han toan bo the
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSaveAll_Click(object sender, EventArgs e)
    {
        ViewState["alertInfo"] = "";
        try
        {
            if (Request.QueryString["KeyWord"] == null && Request.QueryString["CardGroupID"] == null &&
                Request.QueryString["CustomerID"] == null && Request.QueryString["CustomerGroupID"] == null)
            {
                ViewState["alertInfo"] = "<span style='color:red;'>Thông tin lọc chưa chính xác</span>";
                return;
            }
            DataTable dtCard = null;
            //Ngày hết hạn mới
            string _newexpire = Convert.ToDateTime(dtpExpireDate.Value).ToString("yyyy-MM-dd");
            string KeyWord = "", CardGroupID = "", CustomerID = "", CustomerGroupID = "";

            if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
            {
                KeyWord = Request.QueryString["KeyWord"].ToString();
            }

            if (Request.QueryString["CardGroupID"] != null && Request.QueryString["CardGroupID"].ToString() != "")
            {
                CardGroupID = Request.QueryString["CardGroupID"].ToString();
            }

            if (Request.QueryString["CustomerID"] != null && Request.QueryString["CustomerID"].ToString() != "")
            {
                CustomerID = Request.QueryString["CustomerID"].ToString();
            }

            if (Request.QueryString["CustomerGroupID"] != null && Request.QueryString["CustomerGroupID"].ToString() != "")
            {
                CustomerGroupID = Request.QueryString["CustomerGroupID"].ToString();
            }
            if (Request.QueryString["cusName"] != null && Request.QueryString["cusName"].ToString() != "")
            {
                ViewState["cusName"] = Request.QueryString["cusName"].ToString();
            }

            var chk = ReportService.AddCardExprice(KeyWord, CardGroupID, CustomerID, CustomerGroupID, int.Parse(txtFee.Text.Replace(",", "")), _newexpire, ViewState["UserID"].ToString(), chbEnableMinusActive.Checked);
            if (chk)
            {
                ViewState["alertInfo"] = "Xử lý thẻ thành công";
                GetCardList();

                //Get Data list choice
                ViewState["totalCardChoice"] = 0;
                //CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                var dtCardActive = (DataTable)Session[StaticCached.c_cardActiveChoice]; //CacheLayer.Get<DataTable>(StaticCached.c_cardActiveChoice);

                if (dtCardActive != null && dtCardActive.Rows.Count > 0)
                {
                    ViewState["totalCardChoice"] = dtCardActive.Rows.Count;
                    Button2.Text = "Gia hạn " + dtCardActive.Rows.Count + " thẻ";
                    rptCardChoice.DataSource = dtCardActive;
                    rptCardChoice.DataBind();
                }
                else
                {
                    Button2.Text = "Chọn ít nhất 1 thẻ để gia hạn";
                    rptCardChoice.DataSource = null;
                    rptCardChoice.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            ViewState["alertInfo"] = "<span style='color:red;'>" + ex.Message + "</span>";
        }
    }

    /// <summary>
    /// Gia han the theo danh sach da chon
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        ViewState["alertInfo"] = "";
        try
        {
            var dtCardActive = (DataTable)Session[StaticCached.c_cardActiveChoice]; //CacheLayer.Get<DataTable>(StaticCached.c_cardActiveChoice);
            if (dtCardActive != null && dtCardActive.Rows.Count > 0)
            {
                //Ngày hết hạn mới
                string _newexpire = Convert.ToDateTime(dtpExpireDate.Value).ToString("yyyy-MM-dd");
                var count = 0;
                var listCardNumber = "";
                foreach (DataRow item in dtCardActive.Rows)
                {
                    count++;
                    if (!string.IsNullOrWhiteSpace(item["CardNumber"].ToString()))
                        listCardNumber += string.Format("'{0}'{1}", item["CardNumber"],
                            count != dtCardActive.Rows.Count ? "," : "");
                }

                var chk = ReportService.AddCardExpriceByListCardNumber(listCardNumber,
                    int.Parse(txtFee.Text.Replace(",", "")), _newexpire, ViewState["UserID"].ToString(),
                    chbEnableMinusActive.Checked);
                if (chk)
                {
                    ViewState["alertInfo"] = "Xử lý thẻ thành công";
                    //CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                    Session[StaticCached.c_cardActiveChoice] = null;
                    rptCardChoice.DataSource = null;
                    rptCardChoice.DataBind();
                    GetCardList();
                }
            }
            else
            {
                ViewState["alertInfo"] = "<span style='color:red;'>Không có thẻ nào được xử lý</span>";
                Session[StaticCached.c_cardActiveChoice] = null;
                //CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                rptCardChoice.DataSource = null;
                rptCardChoice.DataBind();
                GetCardList();
            }
        }
        catch (Exception ex)
        {
            ViewState["alertInfo"] = "<span style='color:red;'>" + ex.Message + "</span>";
        }
    }
    protected void lnkImportEx_Click(object sender, EventArgs e)
    {
        ViewState["alertInfo"] = "";
        if (FileUpload1 == null || string.IsNullOrWhiteSpace(FileUpload1.PostedFile.FileName))
        {
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Có lỗi sảy ra", "alert('bạn cần chọn file')", true);
            //Response.Redirect("~/QLXuatAn/ActiveCard_v2.aspx", false);
            ViewState["alertInfo"] = "<span style='color:red;'>Không có file được upload</span>";
            return;
        }
        string _title1 = "Danh sách thẻ";
        string _title2 = "";
        string myTempFile = Path.Combine(Path.GetTempPath(), FileUpload1.PostedFile.FileName);
        FileUpload1.SaveAs(myTempFile);
        var txtError = "";
        if (File.Exists(myTempFile))
        {
            var dt = StaticPool.ReadFromExcelActiveCard(myTempFile, ref txtError);
            if (!string.IsNullOrWhiteSpace(txtError))
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Có lỗi sảy ra", string.Format("alert('{0}')", txtError), true);
                //Response.Redirect("~/QLXuatAn/ActiveCard_v2.aspx", false);
                ViewState["alertInfo"] = "<span style='color:red;'>File .xlsx Upload không hợp lệ</span>";
                return;
            }

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {

                        if ((item["Mã thẻ"] == DBNull.Value || string.IsNullOrWhiteSpace(item["Mã thẻ"].ToString())) && item["Biển số"] == DBNull.Value || string.IsNullOrWhiteSpace(item["Biển số"].ToString()))
                            continue;

                        //Ngày hết hạn mới, nếu trong file excel để rỗng thì lấy từ dtpExpireDate.Value
                        if (item["Hạn SD"] == DBNull.Value || string.IsNullOrWhiteSpace(item["Hạn SD"].ToString()))
                        {
                            item["Hạn SD"] = Convert.ToDateTime(dtpExpireDate.Value).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            item["Hạn SD"] = Convert.ToDateTime(item["Hạn SD"]).ToString("yyyy-MM-dd");
                        }
                        var chkAmNgay = false;
                        if (item["Cho phép âm"] == DBNull.Value || string.IsNullOrWhiteSpace(item["Cho phép âm"].ToString()))
                        {
                            chkAmNgay = chbEnableMinusActive.Checked;
                        }
                        else
                        {
                            chkAmNgay = item["Cho phép âm"].ToString() != "0";
                        }

                        var mucPhi = Convert.ToInt32(item["Mức phí"] == DBNull.Value || string.IsNullOrWhiteSpace(item["Mức phí"].ToString()) ? txtFee.Text.Replace(",", "") : item["Mức phí"].ToString());
                        //add or update
                        ReportService.AddCardExpriceByListCardNumberByFile(string.Format("'{0}'", item["Mã thẻ"].ToString()), item["Biển số"].ToString(), mucPhi, item["Hạn SD"].ToString(), ViewState["UserID"].ToString(), chkAmNgay);
                    }
                    //CacheLayer.Clear(StaticCached.c_cardActiveChoice);
                }
                ViewState["alertInfo"] = "Cập nhập thành công";
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Thông báo", "alert('Cập nhập thành công');", true);
                //Response.Redirect("~/QLXuatAn/ActiveCard_v2.aspx", false);
            }
            catch (Exception ex)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}');", ex.Message), true);
                //Response.Redirect("~/QLXuatAn/ActiveCard_v2.aspx", false);
                ViewState["alertInfo"] = "<span style='color:red;'>" + ex.Message + "</span>";
            }
        }
    }
}
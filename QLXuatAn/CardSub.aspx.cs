using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_CardSub : System.Web.UI.Page
{
    DataTable dtCardMap = null;
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
                var KeyWord = "";
                if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
                {
                    KeyWord = Request.QueryString["KeyWord"].ToString();
                    txtKeyWord.Value = KeyWord;
                }

                var pageIndex = 1;
                if (Request.QueryString["Page"] != null)
                {
                    pageIndex = Convert.ToInt32(Request.QueryString["Page"].ToString());
                }
                var totalCount = 0;
                DataTable dtCard = ReportService.GetListSubCard(KeyWord, pageIndex, StaticPool.pagesize, ref totalCount);

                if (dtCard != null && dtCard.Rows.Count > 0)
                {
                    var listCardNumber = "";

                    foreach (DataRow item in dtCard.Rows)
                    {
                        listCardNumber += item["MainCard"] + ",";
                    }
                    dtCardMap = ReportService.GetCardByCardNumber(listCardNumber);

                    id_cardlist.InnerText = "Danh sách thẻ phụ (" + totalCount + ")";
                    //By HNG paging
                    StaticPool.HNGpaging(dtCard, totalCount, StaticPool.pagesize, pager, rpt_Card);
                    rpt_Card.DataSource = dtCard;
                    rpt_Card.DataBind();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }

    public string GetCardNo(string cardNumber)
    {
        var _cusName = "";
        if (dtCardMap != null && dtCardMap.Rows.Count > 0)
        {
            var rRow = dtCardMap.Select(string.Format("CardNumber = '{0}'", cardNumber));
            if (rRow.Length > 0)
                _cusName = rRow[0]["CardNo"].ToString();
        }
        return _cusName;
    }
    [System.Web.Services.WebMethod]
    public static string DeleteSubCard(string subCardId)
    {
        var chk = "1";
        try
        {
            if (subCardId != "")
            {
                var str = string.Format("UPDATE [dbo].[tblSubCard] SET IsDelete = 1 WHERE Id = {0}", subCardId);
                StaticPool.mdb.ExecuteCommand(str);
            }
        }
        catch (Exception)
        {
            chk = "0";
        }

        return chk;
    }
    protected void Excel_Click(object sender, EventArgs e)
    {
        var KeyWord = "";
        if (Request.QueryString["KeyWord"] != null && Request.QueryString["KeyWord"].ToString() != "")
        {
            KeyWord = Request.QueryString["KeyWord"].ToString();
        }
        DataTable dtCardExcel = ReportService.GetListSubCardExcel(KeyWord);

        string _title1 = "Danh sách thẻ phụ";
        string _title2 = "";// "Từ " + dtpFromDate.Value + " đến " + dtpToDate.Value;
        try
        {
            BindDataToExcel(dtCardExcel, _title1, _title2, ViewState["UserID"].ToString());
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}')", ex.Message), true);
        }

    }

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
    protected void ImportCardSub_Click(object sender, EventArgs e)
    {
        if (FileUpload1 == null || string.IsNullOrWhiteSpace(FileUpload1.PostedFile.FileName))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Có lỗi sảy ra", "alert('bạn cần chọn file')", true);
            Response.Redirect("~/QLXuatAn/CardSub.aspx", false);
            return;
        }
        string _title1 = "Danh sách thẻ phụ";
        string _title2 = "";
        string myTempFile = Path.Combine(Path.GetTempPath(), FileUpload1.PostedFile.FileName);
        FileUpload1.SaveAs(myTempFile);
        var txtError = "";
        if (File.Exists(myTempFile))
        {
            var dt = StaticPool.ReadFromExcelSubCard(myTempFile, ref txtError);
            if (!string.IsNullOrWhiteSpace(txtError))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Có lỗi sảy ra", string.Format("alert('{0}')", txtError), true);
                Response.Redirect("~/QLXuatAn/CardSub.aspx", false);
                return;
            }

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (DataRow item in dt.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(item["CardNumber"].ToString()))
                        {
                            continue;
                        }
                        sb.AppendLine(string.Format("IF((SELECT COUNT(*) FROM [dbo].[tblSubCard] WHERE [CardNumber] = '{0}') <= 0) BEGIN", item["CardNumber"].ToString()));
                        sb.AppendLine("INSERT INTO [dbo].[tblSubCard]");
                        sb.AppendLine("([MainCard]");
                        sb.AppendLine(",[CardNo]");
                        sb.AppendLine(",[CardNumber]");
                        sb.AppendLine(",[IsDelete])");
                        sb.AppendLine("VALUES");
                        sb.AppendLine(string.Format("('{0}','{1}','{2}',0)", item["MainCard"].ToString(), item["CardNo"].ToString(), item["CardNumber"].ToString()));
                        sb.AppendLine("END");

                    }
                    StaticPool.mdb.ExecuteCommand(sb.ToString());

                }

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Thông báo", "alert('Cập nhập thành công');", true);
                Response.Redirect("~/QLXuatAn/CardSub.aspx", false);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Lỗi", string.Format("alert('{0}');", ex.Message), true);
            }
        }
    }
    
}
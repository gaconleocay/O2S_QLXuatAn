using System;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;

public partial class UserControl_PrinterControl : System.Web.UI.UserControl
{
    private int _PageIndex = 1;
    private int _PageNext = 0;
    private int _TotalItem = 0;
    public int PageIndex
    {
        set { _PageIndex = value; }
    }
    public int PageNext
    {
        set { _PageNext = value; }
        //get
        //{
        //    return (ViewState["PageNext"] == null) ? StaticPool.pageLevelPrint : (int)ViewState["PageNext"];
        //}
    }

    public int TotalItem
    {
        set { _TotalItem = value; }
        //get
        //{
        //    return (ViewState["TotalItem"] == null) ? 0 : (int)ViewState["TotalItem"];
        //}
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            var pIdx = ((_PageIndex - 1) * _PageNext + 1);
            if (pIdx + _PageNext > _TotalItem)
            {
                _PageNext = _TotalItem;
            }

            int bRow = pIdx;
            int eRow = _PageNext;

            if (Request.QueryString["bRow"] != null && Request.QueryString["bRow"] != "")
            {
                bRow = Convert.ToInt32(Request.QueryString["bRow"]);
            }
            if (Request.QueryString["eRow"] != null && Request.QueryString["eRow"] != "")
            {
                eRow = Convert.ToInt32(Request.QueryString["eRow"]);
            }
            ViewState["PageIndex"] = bRow; // số dòng bắt đầu
            ViewState["PageNext"] = eRow; // số dòng kết thúc
            ViewState["TotalItem"] = _TotalItem; // tổng số dòng
            var pageCount = 0;
            if (_TotalItem != 0) 
            {
                pageCount = _TotalItem / _PageNext;
                if (_TotalItem % _PageNext != 0)
                {
                    pageCount = pageCount + 1;
                }
            }
            ViewState["TotalPage"] = pageCount; // tổng số trang
        }
    }

    protected void btnExportPDF_Click(object sender, EventArgs e)
    {
        //StringReader sr = new StringReader(Request.Form[hfGridHtml.UniqueID]);
        //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        //pdfDoc.Open();
        //XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //pdfDoc.Close();
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", string.Format("attachment;filename=báo-cáo-{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmm")));
        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Response.Write(pdfDoc);
        //Response.End();


        //Document document = new Document(PageSize.A4);
        //PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
        //document.Open();
        //HTMLWorker hw = new HTMLWorker(document);
        //StringReader sr = new StringReader(Request.Form[hfGridHtml.UniqueID]);
        //XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
        //hw.Parse(new StringReader(HTML));
        //document.Close();
        //Response.ContentType = "application/pdf";
        //Response.AddHeader("content-disposition", string.Format("attachment;filename=báo-cáo-{0}.pdf", DateTime.Now.ToString("ddMMyyyyHHmm")));
        //Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //Response.Write(writer);
        //Response.End();
        //PdfAction action = new PdfAction(PdfAction.PRINTDIALOG);
    }
}
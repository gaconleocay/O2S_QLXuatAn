using System;
using System.Collections.Generic;
using System.Configuration;
using Futech.Tools;
using System.Data;
using System.Web.UI.WebControls;
using SiteUtils;
using Futech.Helpers;
using System.Text;
using OfficeOpenXml.Style;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;

/// <summary>
/// Summary description for StaticPool
/// </summary>
public static class StaticPool
{
    public const int pagesize = 20;
    public const int pageLevelPrint = 25;
    public const int pageLevelPrint10 = 10;

    // CSDL
    static public MDB2014 mdb = new MDB2014(ConfigurationManager.AppSettings["SQLServerName"].ToString(), ConfigurationManager.AppSettings["SQLDatabaseName"].ToString(),
        ConfigurationManager.AppSettings["SQLAuthentication"].ToString(), ConfigurationManager.AppSettings["SQLUserName"].ToString(), ConfigurationManager.AppSettings["SQLPassword"].ToString());

    static public MDB2014 mdbevent = new MDB2014(ConfigurationManager.AppSettings["SQLServerName"].ToString(), ConfigurationManager.AppSettings["SQLDatabaseEventName"].ToString(),
        ConfigurationManager.AppSettings["SQLAuthentication"].ToString(), ConfigurationManager.AppSettings["SQLUserName"].ToString(), ConfigurationManager.AppSettings["SQLPassword"].ToString());

    public static bool CheckPermission(string UserID, string SubSystemCode, string Action, string AppCode)
    {
        DataTable dtUser = CacheLayer.Get<DataTable>(StaticCached.c_tblUserById + "_" + UserID);
        if (dtUser == null)
        {
            dtUser = mdb.FillData("select * from tblUser where UserID = '" + UserID + "'");
            if (dtUser!=null&& dtUser.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblUserById + "_" + UserID, dtUser, StaticCached.TimeCache);
        }
        //DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + UserID + "'");
        if (dtUser != null && dtUser.Rows.Count == 1)
        {
            if (bool.Parse(dtUser.Rows[0]["IsSystem"].ToString()))
                return true;
            else
            {
                if (CheckRoleSystem(dtUser.Rows[0]["UserID"].ToString(), AppCode))
                    return true;
                else
                {
                    DataTable dtRolePermissionMaping = StaticPool.mdb.FillData("select * from vGetUserJoinRoleAndPermisson where UserID = '" + dtUser.Rows[0]["UserID"].ToString() + "' and SubSystemCode = '" + SubSystemCode + "' and " + Action + " = 1");
                    if (dtRolePermissionMaping != null && dtRolePermissionMaping.Rows.Count > 0)
                        return true;
                }
            }
        }
        return false;
    }

    public static bool CheckRoleSystem(string UserID, string AppCode)
    {
        DataTable dtRole = StaticPool.mdb.FillData("select * from vGetUserJoinRole where UserID = '" + UserID + "' and IsSystem = 1 and (AppCode = 'KzBMS' or AppCode = '" + AppCode + "')");
        if (dtRole != null && dtRole.Rows.Count > 0)
            return true;
        else
            return false;
    }

    public static bool SystemUsingLoop()
    {
        try
        {
            DataTable dt = mdb.FillData("select LaneID from tblLane where IsLoop=1 and InActive=0");
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
        }
        catch
        { }
        return false;
    }

    public static bool LaneUsingLoop(string laneid)
    {
        try
        {
            DataTable dt = StaticPool.mdb.FillData("select LaneID from tblLane where IsLoop=1 and InActive=0 and LaneID='" + laneid + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
        }
        catch
        { }
        return false;
    }

    //public static string GetUserName(string userid)
    //{
    //    DataTable dt = StaticPool.mdb.FillData("select UserName from tblUser where UserID='" + userid + "'");
    //    if (dt != null && dt.Rows.Count > 0)
    //        return dt.Rows[0]["UserName"].ToString();
    //    return "";
    //}

    public static string GetCardNumberByID(string id)
    {
        try
        {
            DataTable dt = StaticPool.mdbevent.FillData("select CardNumber from tblCardEvent where Id='" + id + "'");
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["CardNumber"].ToString();
        }
        catch
        { }
        return "";
    }

    public static string GetPlateByID(string id)
    {
        try
        {
            DataTable dt = StaticPool.mdbevent.FillData("select Plate from tblLoopEvent where Id='" + id + "'");
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["Plate"].ToString();
        }
        catch
        { }
        return "";
    }

    public static void SaveLogFile(string username, string appcode, string subsystemcode, string objectname, string actions, string description)
    {
        try
        {
            //objectname: cardnumber, plate, customername...
            string pcname = System.Environment.MachineName;
            StaticPool.mdb.ExecuteCommand("insert into tblLog(Date, UserName, AppCode, SubSystemCode, ObjectName, Actions, Description, ComputerName) values('" +
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', N'" +
                username + "', N'" +
                appcode + "', N'" +
                subsystemcode + "', N'" +
                objectname + "', N'" +
                actions + "', N'" +
                description + "', N'" +
                pcname +
                "')");

        }
        catch
        {

        }
    }

    public static string GetStringChange(string oldstring, string newstring)
    {
        string st = oldstring + "->" + newstring;
        try
        {

            string[] _arrayoldstring = oldstring.Split(';');
            string[] _arraynewstring = newstring.Split(';');
            if (_arrayoldstring != null && _arrayoldstring.Length > 0 && _arraynewstring != null && _arraynewstring.Length > 0 && _arraynewstring.Length == _arrayoldstring.Length)
            {
                for (int i = 0; i < _arrayoldstring.Length; i++)
                {
                    if (_arrayoldstring[i] == _arraynewstring[i] && _arrayoldstring[i] != "")
                    {
                        if (StaticPool.CheckStringContainInOthers(_arrayoldstring[i], _arrayoldstring) == false && StaticPool.CheckStringContainInOthers(_arrayoldstring[i], _arraynewstring) == false)
                            st = st.Replace(_arrayoldstring[i], "");
                    }
                }
            }

            if (st.Length > 400)
                st = st.Substring(0, 400);

            return st;
        }
        catch (Exception ex)
        {
        }
        return "";

    }

    /// <summary>
    /// return true if contain in other string
    /// else return false
    /// </summary>
    /// <param name="_origin"></param>
    /// <param name="_array"></param>
    /// <returns></returns>
    private static bool CheckStringContainInOthers(string _origin, string[] _array)
    {
        int count = 0;
        for (int i = 0; i < _array.Length; i++)
        {
            if (_array[i].Contains(_origin) || _array[i] == _origin)
                count++;

            if (count > 1)
                return true;
        }

        return false;
    }

    public static string GetCustomerName(string customerid)
    {
        if (customerid == "")
            return "";
        DataTable dt = StaticPool.mdb.FillData("select CustomerName from tblCustomer where CustomerID='" + customerid + "'");
        if (dt != null && dt.Rows.Count > 0)
            return dt.Rows[0]["CustomerName"].ToString();

        return "";
    }

    public static GridView CreateHeaderTable(string title1, string title2, string username)
    {
        try
        {
            GridView gv = new GridView();
            gv.ShowHeader = false;
            gv.GridLines = GridLines.None;
            gv.Font.Bold = true;
            gv.Font.Size = 12;
            DataTable dt = new DataTable();
            dt.Columns.Add("header", typeof(string));
            DataTable dtconfig = StaticPool.mdb.FillData("select * from tblSystemConfig");
            if (dtconfig != null && dtconfig.Rows.Count > 0)
            {
                dt.Rows.Add(dtconfig.Rows[0]["Company"].ToString());
                dt.Rows.Add(dtconfig.Rows[0]["Address"].ToString());
                dt.Rows.Add(title1);
                dt.Rows.Add(title2);
                dt.Rows.Add("Người lập báo cáo: " + username);
                dt.Rows.Add("Người phê duyệt: ");
            }
            gv.DataSource = dt;
            gv.DataBind();
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                gv.Rows[i].Cells[0].ColumnSpan = 6;
            }
            return gv;

        }
        catch
        { }

        return null;
    }
    public static DataTable getHeaderExcel(string title1, string title2, string username)
    {
        try
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("header", typeof(string));
            DataTable dtconfig = StaticPool.mdb.FillData("select * from tblSystemConfig");
            if (dtconfig != null && dtconfig.Rows.Count > 0)
            {
                dt.Rows.Add(dtconfig.Rows[0]["Company"].ToString());
                dt.Rows.Add(dtconfig.Rows[0]["Address"].ToString());
                dt.Rows.Add(title1);
                dt.Rows.Add(title2);
                dt.Rows.Add("Người lập báo cáo: " + username);
                dt.Rows.Add("Người phê duyệt: ");
            }

            return dt;
        }
        catch
        { }

        return null;
    }


    public static GridView CreateFooterTable()
    {
        try
        {
            GridView gv = new GridView();
            gv.ShowHeader = false;
            gv.GridLines = GridLines.None;
            gv.Font.Bold = true;
            gv.Font.Size = 12;
            gv.Font.Italic = true;
            DataTable dt = new DataTable();
            dt.Columns.Add("header", typeof(string));
            dt.Columns.Add("header2", typeof(string));
            dt.Columns.Add("header3", typeof(string));
            dt.Columns.Add("header4", typeof(string));
            dt.Columns.Add("header5", typeof(string));
            dt.Columns.Add("header6", typeof(string));

            dt.Rows.Add("Người lập BC", "Người phê duyệt");
            gv.DataSource = dt;
            gv.DataBind();

            return gv;
        }
        catch
        { }
        return null;
    }

    public static string GetPeriodTime(DateTime dtimein, DateTime dtimeout)
    {
        try
        {
            long mdiff = Futech.Tools.DateUI.DateDiff(DateInterval.Second, dtimein, dtimeout);
            string _periodtime = mdiff / 3600 + "h" + (mdiff % 3600) / 60 + "m" + (mdiff % 3600) % 60 + "s";
            return _periodtime;
        }
        catch
        { }
        return "";
    }


    public static void HNGpaging(DataSet ds, int ps, CollectionPager pager, Repeater rpt)
    {
        var totalPageItem = Convert.ToInt32(ds.Tables[0].Rows[0]["totalCount"].ToString());
        var pageCount = totalPageItem / ps;
        if (totalPageItem % ps != 0)
        {
            pageCount = pageCount + 1;
        }
        //By HNG paging
        pager.MaxPages = pageCount;
        pager.TotalRecordFromData = totalPageItem;
        pager.PageCount = pageCount;
        pager.TotalRecordNow = ds.Tables[1].Rows.Count;
        pager.BindToControl = rpt;
        //---------------- end HNG paging
    }
    public static void HNGpaging(DataTable dt, int totalPageItem, int ps, CollectionPager pager, Repeater rpt)
    {
        var pageCount = totalPageItem / ps;
        if (totalPageItem % ps != 0)
        {
            pageCount = pageCount + 1;
        }
        //By HNG paging
        pager.MaxPages = pageCount;
        pager.TotalRecordFromData = totalPageItem;
        pager.PageCount = pageCount;
        pager.TotalRecordNow = dt.Rows.Count;
        pager.BindToControl = rpt;
        //---------------- end HNG paging
    }

    /// <summary>
    /// Hàm trả về đường dẫn ảnh 
    /// trước khi gọi hàm phải cấu hình IP qua IIS trỏ vào thư mục chứa ảnh
    /// thiết lập thông tin IP url vào web.config 
    /// add key="PathPicIn" 
    /// add key="PathPicOut"
    /// </summary>
    /// <param name="strReplace">chuỗi cũ cần được thay thế</param>
    /// <param name="IpUrl">Chuỗi được thay thế</param>
    /// <returns></returns>
    public static string getUrlPath(string imgPath, string strReplace, string IpUrl)
    {
        if (!string.IsNullOrWhiteSpace(strReplace))
        {
            imgPath = imgPath.Replace(strReplace, IpUrl).Replace(@"\", "/");
        }
        else
        {
            imgPath = IpUrl + imgPath;
        }
        return imgPath;
    }

    public static string DataTableToJsonObj(DataTable dt)
    {
        DataSet ds = new DataSet();
        ds.Merge(dt);
        StringBuilder JsonString = new StringBuilder();
        if (ds != null && ds.Tables[0].Rows.Count > 0)
        {
            JsonString.Append("[");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                JsonString.Append("{");
                for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                {
                    if (j < ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append(string.Format("\"{0}\":\"{1}\",", ds.Tables[0].Columns[j].ColumnName, ds.Tables[0].Rows[i][j].ToString().Trim()));
                    }
                    else if (j == ds.Tables[0].Columns.Count - 1)
                    {
                        JsonString.Append(string.Format("\"{0}\":\"{1}\"", ds.Tables[0].Columns[j].ColumnName, ds.Tables[0].Rows[i][j].ToString().Trim()));
                    }
                }
                if (i == ds.Tables[0].Rows.Count - 1)
                {
                    JsonString.Append("}");
                }
                else
                {
                    JsonString.Append("},");
                }
            }
            JsonString.Append("]");
            return JsonString.ToString();
        }
        else
        {
            return "[{}]";
        }
    }


    // Epplus Excel

    public static void BindingFormatForExcel(ExcelWorksheet worksheet, DataTable dt, DataTable dtHeader)
    {
        // Set default width cho tất cả column
        //worksheet.DefaultColWidth = 10;
        // Tự động xuống hàng khi text quá dài
        //worksheet.Cells.Style.WrapText = true;
        // Tạo header
        if (dtHeader != null && dtHeader.Rows.Count > 0)
        {
            for (int i = 0; i < dtHeader.Rows.Count; i++)
            {
                DataRow dr = dtHeader.Rows[i];
                worksheet.Cells[i + 1, 1].Value = dr["header"].ToString();
                worksheet.Cells[i + 1, 1].Style.Font.Bold = true;
            }
        }
        //worksheet.Cells[1, 1].Value = "ID";
        //worksheet.Cells[1, 2].Value = "Full Name";
        //worksheet.Cells[1, 3].Value = "Address";
        //worksheet.Cells[1, 4].Value = "Money";

        // Tao style cho header list
        using (var range = worksheet.Cells[dtHeader.Rows.Count + 2, 1, dtHeader.Rows.Count + 2, dt.Columns.Count])
        {

            //// Set PatternType
            //range.Style.Fill.PatternType = ExcelFillStyle.DarkGray;
            //// Set Màu cho Background
            //range.Style.Fill.BackgroundColor.SetColor(Color.Aqua);
            //// Canh giữa và đậm cho các text
            range.Style.Font.Bold = true;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //// Set Font cho text  trong Range hiện tại
            //range.Style.Font.SetFromFont(new Font("Arial", 10));
            //// Set Border
            //range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
            //// Set màu ch Border
            //range.Style.Border.Bottom.Color.SetColor(Color.Blue);
        }
        //worksheet.Cells.Style.Locked = true;
        if (dt != null)
        {
            // tạo header cho danh sách
            //worksheet.Cells[dtHeader.Rows.Count + 2, 1].LoadFromDataTable(dt, false, TableStyles.None);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn col = dt.Columns[i];
                worksheet.Cells[dtHeader.Rows.Count + 2, i + 1].Value = col.ColumnName;
                worksheet.Cells[dtHeader.Rows.Count + 2, i + 1].AutoFilter = true;
                //worksheet.Cells[dtHeader.Rows.Count + 2, i + 1].AutoFitColumns();
            }

            if (dt.Rows.Count > 0)
            {
                var rowStart = dtHeader.Rows.Count + 2;
                // Đỗ dữ liệu từ list vào 
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow item = dt.Rows[i];
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var _fromRow = rowStart + (i + 1);
                        var _fromCol = j + 1;
                        worksheet.Cells[_fromRow, _fromCol].Style.Numberformat.Format = "@";
                        worksheet.Cells[_fromRow, _fromCol].Value = item[j].ToString();
                        //worksheet.Cells[_fromRow, _fromCol].AutoFitColumns();
                    }
                }
            }
            
            //worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns(); 
            //const double minWidth = 0.00;
            //const double maxWidth = 50.00;
            //worksheet.Cells.AutoFitColumns(minWidth, maxWidth);
        }


        //// Format lại định dạng xuất ra ở cột Money
        //worksheet.Cells[2, 4, listItems.Count + 4, 4].Style.Numberformat.Format = "$#,##.00";
        //// fix lại width của column với minimum width là 15
        //worksheet.Cells[1, 1, listItems.Count + 5, 4].AutoFitColumns(15);

        //// Thực hiện tính theo formula trong excel
        //// Hàm Sum 
        //worksheet.Cells[listItems.Count + 3, 3].Value = "Total is :";
        //worksheet.Cells[listItems.Count + 3, 4].Formula = "SUM(D2:D" + (listItems.Count + 1) + ")";
        //// Hàm SumIf
        //worksheet.Cells[listItems.Count + 4, 3].Value = "Greater than 20050 :";
        //worksheet.Cells[listItems.Count + 4, 4].Formula = "SUMIF(D2:D" + (listItems.Count + 1) + ",\">20050\")";
        //// Tinh theo %
        //worksheet.Cells[listItems.Count + 5, 3].Value = "Percentatge: ";
        //worksheet.Cells[listItems.Count + 5, 4].Style.Numberformat.Format = "0.00%";
        //// Dòng này có nghĩa là ở column hiện tại lấy với địa chỉ (Row hiện tại - 1)/ (Row hiện tại - 2) Cùng một colum
        //worksheet.Cells[listItems.Count + 5, 4].FormulaR1C1 = "(R[-1]C/R[-2]C)";
    }
    public static Stream CreateExcelFile(Stream stream = null, DataTable dt = null, DataTable dtHeader = null)
    {
        using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
        {
            // Tạo author cho file Excel
            excelPackage.Workbook.Properties.Author = "Futech";
            // Tạo title cho file Excel
            excelPackage.Workbook.Properties.Title = "Báo cáo";
            // thêm tí comments vào làm màu 
            excelPackage.Workbook.Properties.Comments = "Báo cáo";
            // Add Sheet vào file Excel
            excelPackage.Workbook.Worksheets.Add("Sheet1");
            // Lấy Sheet bạn vừa mới tạo ra để thao tác 
            var workSheet = excelPackage.Workbook.Worksheets[1];
            //workSheet.Protection.IsProtected = false;
            // Đổ data vào Excel file
            //workSheet.Cells[1, 1].LoadFromDataTable(dt, true, TableStyles.Dark9);
            BindingFormatForExcel(workSheet, dt, dtHeader);
            excelPackage.Save();
            return excelPackage.Stream;
        }
    }

    public static DataTable ReadFromExcelfile(string path, DataTable dtHeader, ref string errorText)
    {
        try
        {
            // Khởi tạo data table
            DataTable dt = new DataTable();
            // Load file excel và các setting ban đầu
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count >= 1)
                {
                    // Lấy Sheet đầu tiện trong file Excel để truy vấn 
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    // Đọc tất cả các header
                    foreach (var firstRowCell in workSheet.Cells[dtHeader.Rows.Count + 2, 1, dtHeader.Rows.Count + 2,
                        workSheet.Dimension.End.Column])
                    {
                        if (!string.IsNullOrEmpty(firstRowCell.Text))
                            dt.Columns.Add(firstRowCell.Text);
                    }
                    // Đọc tất cả data bắt đầu từ row thứ n
                    for (var rowNumber = dtHeader.Rows.Count + 3; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        // Lấy 1 row trong excel để truy vấn
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, dt.Columns.Count];
                        // tạo 1 row trong data table
                        var newRow = dt.NewRow();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        dt.Rows.Add(newRow);
                    }
                }
                return dt;
            }
        }
        catch (Exception ex)
        {
            errorText = ex.Message;
        }
        return null;
    }

    public static DataTable ReadFromExcelActiveCard(string path, ref string errorText)
    {
        try
        {
            // Khởi tạo data table
            var dt = new DataTable();
            // Load file excel và các setting ban đầu
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count >= 1)
                {
                    // Lấy Sheet đầu tiện trong file Excel để truy vấn 
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    // Đọc tất cả các header
                    foreach (var firstRowCell in workSheet.Cells[8, 1, 8, workSheet.Dimension.End.Column])
                    {
                        dt.Columns.Add(firstRowCell.Text);
                    }
                    // Đọc tất cả data bắt đầu từ row thứ n
                    for (var rowNumber = 9; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        // Lấy 1 row trong excel để truy vấn
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        // tạo 1 row trong data table
                        var newRow = dt.NewRow();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        dt.Rows.Add(newRow);
                    }
                }
                return dt;
            }
        }
        catch (Exception ex)
        {
            errorText = ex.Message;
        }
        return null;
    }
    public static DataTable ReadFromExcelSubCard(string path, ref string errorText)
    {
        try
        {
            // Khởi tạo data table
            var dt = new DataTable();
            // Load file excel và các setting ban đầu
            using (ExcelPackage package = new ExcelPackage(new FileInfo(path)))
            {
                if (package.Workbook.Worksheets.Count >= 1)
                {
                    // Lấy Sheet đầu tiện trong file Excel để truy vấn 
                    ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                    // Đọc tất cả các header
                    foreach (var firstRowCell in workSheet.Cells[3, 1, 3, workSheet.Dimension.End.Column])
                    {
                        dt.Columns.Add(firstRowCell.Text);
                    }
                    // Đọc tất cả data bắt đầu từ row thứ n
                    for (var rowNumber = 4; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        // Lấy 1 row trong excel để truy vấn
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        // tạo 1 row trong data table
                        var newRow = dt.NewRow();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        dt.Rows.Add(newRow);
                    }
                }
                return dt;
            }
        }
        catch (Exception ex)
        {
            errorText = ex.Message;
        }
        return null;
    }
    public static string GetLane(string laneid)
    {
        if (string.IsNullOrWhiteSpace(laneid))
        {
            return "";
        }

        var dtlane = CacheLayer.Get<DataTable>(StaticCached.c_tblLane);
        if (dtlane == null)
        {
            dtlane = StaticPool.mdb.FillData("select LaneName, LaneID from tblLane order by SortOrder");
            if (dtlane.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblLane, dtlane, StaticCached.TimeCache);
        }
        var gName = "";
        if (dtlane != null)
        {
            var rRow = dtlane.Select(string.Format("LaneID = '{0}'", laneid));
            if (rRow[0] != null)
            {
                gName = rRow[0]["LaneName"].ToString();
            }

        }
        return gName;
    }

    public static string GetDateTime(string dtime)
    {
        if (dtime != "")
        {
            return DateTime.Parse(dtime).ToString("dd/MM/yyyy HH:mm:ss");
        }
        return "";
    }

    public static string GetUserName(string userid)
    {
        if (string.IsNullOrWhiteSpace(userid))
        {
            return "";
        }
        var dtuser = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        if (dtuser == null)
        {
            dtuser = mdb.FillData("select UserName, UserID from tblUser where IsLock=0  order by SortOrder");
            if (dtuser.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblUser, dtuser, StaticCached.TimeCache);
        }
        //DataTable dt = CacheLayer.Get<DataTable>(StaticCached.c_tblUser);
        var gName = "";
        if (dtuser != null && dtuser.Rows.Count > 0)
        {
            var rRow = dtuser.Select(string.Format("UserID = '{0}'", userid));
            if (rRow.Any())
            {
                gName = rRow[0]["UserName"].ToString();
            }
        }
        return gName;
    }

    public static string GetCardGroup(string CardGroupID)
    {
        if (string.IsNullOrWhiteSpace(CardGroupID))
        {
            return "";
        }

        if (CardGroupID == "LOOP_D")
            return "Vòng từ-Xe lượt";
        else if (CardGroupID == "LOOP_M")
            return "Vòng từ-Xe tháng";
        var dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
        if (dtCardGroup == null)
        {
            dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
            if (dtCardGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
        }
        var gName = "";
        if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
        {
            var rRow = dtCardGroup.Select(string.Format("CardGroupID = '{0}'", CardGroupID));
            if (rRow.Any())
            {
                gName = rRow[0]["CardGroupName"].ToString();
            }
        }
        return gName;
    }

    public static string GetCardStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Đã khóa thẻ</span>";
    }

    public static string GetCustomerGroupByName(string groupName)
    {
        var dtcustomergroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
        if (dtcustomergroup == null)
        {
            dtcustomergroup = mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
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

    public static string GetCustomerGroup(string customerid)
    {
        if (customerid != "" && customerid != "-1")
        {
            var dt = mdb.FillData("select CustomerGroupID from tblCustomer where CustomerID='" + customerid + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                var temp = mdb.FillData("select CustomerGroupName from tblCustomerGroup where CustomerGroupID='" + dt.Rows[0]["CustomerGroupID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                    return temp.Rows[0]["CustomerGroupName"].ToString();
                //return dt.Rows[0]["Address"].ToString();
            }
        }
        return "";
    }

    public static string GetExpireDate(string ExpireDate)
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


    public static string DataSetToJSON(DataSet ds)
    {

        var dict = new Dictionary<string, object>();
        foreach (DataTable dt in ds.Tables)
        {
            object[] arr = new object[dt.Rows.Count + 1];

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                arr[i] = dt.Rows[i].ItemArray;
            }

            dict.Add(dt.TableName, arr);
        }

        var json = new JavaScriptSerializer();
        return json.Serialize(dict);
    }

    public static string DataSetToJSON(DataTable table)
    {
        JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
        jsSerializer.MaxJsonLength = 2147483644;
        List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
        Dictionary<string, object> childRow;
        foreach (DataRow row in table.Rows)
        {
            childRow = new Dictionary<string, object>();
            foreach (DataColumn col in table.Columns)
            {
                childRow.Add(col.ColumnName, row[col]);
            }
            parentRow.Add(childRow);
        }
        return jsSerializer.Serialize(parentRow);
    }

}
using System;
using System.Data;
using System.Text;
using System.Web.Services.Discovery;

public static class ReportService
{
    public static DataTable GetListSubCard(string KeyWord, int pageIndex, int pageSize, ref int total)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SELECT * FROM(");
        sb.AppendLine("select ROW_NUMBER() OVER (ORDER BY Id desc ) AS RowNumber, *");
        sb.AppendLine(" from dbo.tblSubCard WITH (NOLOCK) ");
        
        sb.AppendLine(" where IsDelete = 0");

        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            sb.AppendLine(" AND (MainCard LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" OR CardNo LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" OR CardNumber LIKE N'%" + KeyWord + "%')");
        }

        sb.AppendLine(") as a");
        sb.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //Get Total
        sb.AppendLine("select Count(*) as totalCount");
        sb.AppendLine(" from dbo.tblSubCard WITH (NOLOCK) ");
        sb.AppendLine(" where IsDelete = 0");
        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            sb.AppendLine(" AND (MainCard LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" OR CardNo LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" OR CardNumber LIKE N'%" + KeyWord + "%')");
        }
        return StaticPool.mdb.FillDataPaging_2(sb.ToString(), ref total);
    }
    public static DataTable GetListSubCardExcel(string KeyWord)
    {
        var sb = new StringBuilder();
        sb.AppendLine("select ROW_NUMBER() OVER (ORDER BY Id desc ) AS STT, MainCard, CardNo, CardNumber");
        sb.AppendLine(" from dbo.tblSubCard WITH (NOLOCK) ");

        sb.AppendLine(" where IsDelete = 0");

        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            sb.AppendLine(" AND (MainCard LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" OR CardNo LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" OR CardNumber LIKE N'%" + KeyWord + "%')");
        }

        return StaticPool.mdb.FillData(sb.ToString());
    }

    //public static DataTable GetReportIn(string KeyWord, bool IsHaveTime, string fromdate, string todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize, ref int total)
    //{
    //    var query = new StringBuilder();
    //    //query.AppendLine("");

    //    query.AppendLine("select ROW_NUMBER() OVER(ORDER BY e.DatetimeIn desc) AS RowNumber,");
    //    query.AppendLine("e.Id, e.Plate, e.CardNo, e.CardNumber, e.DateTimeIn, e.PicIn1, e.PicIn2, e.CardGroupID, e.CustomerName, e.LaneIDIn, e.UserIDIn");
    //    query.AppendLine("INTO #Results FROM(");
    //    query.AppendLine("SELECT CONVERT(nvarchar(50),Id)+'_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn,'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
    //    query.AppendLine("FROM dbo.tblCardEvent e  WITH (NOLOCK)");
    //    query.AppendLine("where EventCode = '1'");
    //    if (IsHaveTime == true)
    //    {
    //        query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
    //    }
    //    if (!string.IsNullOrWhiteSpace(CardGroupID))
    //        query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
    //    if (!string.IsNullOrWhiteSpace(LaneID))
    //        query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
    //    if (!string.IsNullOrWhiteSpace(UserID))
    //        query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //        query.AppendLine(string.Format("and (CardNumber = '{0}' or CardNo LIKE '{0}' or PlateIn LIKE '{0}')", KeyWord));

    //    query.AppendLine("Union");

    //    query.AppendLine("SELECT CONVERT(nvarchar(50),Id)+'_LOOP' as Id, Plate, '' as CardNo, '' as CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CarType, CustomerName, LaneIDIn, UserIDIn");
    //    query.AppendLine("FROM dbo.tblLoopEvent  WITH (NOLOCK)");
    //    query.AppendLine("where EventCode = '1'");
    //    if (IsHaveTime == true)
    //    {
    //        query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
    //    }
    //    if (!string.IsNullOrWhiteSpace(CardGroupID))
    //        query.AppendLine(string.Format("and CarType = '{0}'", CardGroupID));
    //    if (!string.IsNullOrWhiteSpace(LaneID))
    //        query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
    //    if (!string.IsNullOrWhiteSpace(UserID))
    //        query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //        query.AppendLine(string.Format("and Plate LIKE '{0}'", KeyWord));
    //    query.AppendLine(") as e");

    //    query.AppendLine("SELECT COUNT(*) totalCount FROM #Results");
    //    query.AppendLine("SELECT * FROM #Results");
    //    query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
    //    query.AppendLine("DROP TABLE #Results");

    //    return StaticPool.mdbevent.FillDataPaging(query.ToString(), ref total);

    //}

    public static DataTable GetReportIn_2(string KeyWord, bool IsHaveTime, string fromdate, string todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        //query.AppendLine("");

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[DatetimeIn] desc) as RowNumber,a.*");
        query.AppendLine("FROM(");

        query.AppendLine("SELECT CONVERT(nvarchar(50), Id) + '_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
        query.AppendLine("FROM dbo.tblCardEvent e  WITH(NOLOCK)");
        query.AppendLine("where EventCode = '1'");
        if (IsHaveTime == true)
        {
            query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
        }
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT CONVERT(nvarchar(50), Id) + '_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
            query.AppendLine("FROM dbo.tblCardEventHistory e  WITH(NOLOCK)");
            query.AppendLine("where EventCode = '1'");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT CONVERT(nvarchar(50), Id) + '_LOOP' as Id, Plate, '' as CardNo, '' as CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CarType, CustomerName, LaneIDIn, UserIDIn");
            query.AppendLine("FROM dbo.tblLoopEvent  WITH(NOLOCK)");
            query.AppendLine("where EventCode = '1'");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CarType = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and Plate LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--Count
        query.AppendLine("select COUNT(*) as totalCount");
        query.AppendLine("FROM(");
        query.AppendLine("SELECT Id");
        query.AppendLine("FROM dbo.tblCardEvent e  WITH(NOLOCK)");
        query.AppendLine("where EventCode = '1'");
        if (IsHaveTime == true)
        {
            query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
        }
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT Id");
            query.AppendLine("FROM dbo.tblCardEventHistory e  WITH(NOLOCK)");
            query.AppendLine("where EventCode = '1'");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));
        }

        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT Id");
            query.AppendLine("FROM dbo.tblLoopEvent  WITH(NOLOCK)");
            query.AppendLine("where EventCode = '1'");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CarType = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and Plate LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as e");
        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total);

    }

    //Kiểm tra xe trong bãi trong quá khứ
    public static DataTable GetReportIn_History(string KeyWord, bool IsHaveTime, string fromdate, string todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        //query.AppendLine("");

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[DatetimeIn] desc) as RowNumber,a.*");
        query.AppendLine("FROM(");

        query.AppendLine("SELECT CONVERT(nvarchar(50), Id) + '_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
        query.AppendLine("FROM dbo.tblCardEvent  WITH(NOLOCK)");
        query.AppendLine("where IsDelete=0");
        if (IsHaveTime == true)
        {
            query.AppendLine(string.Format("and ((DateTimeIn >= '{0}' and DateTimeIn <= '{1}' and  DateTimeOut >'{1}')", fromdate, todate));
            query.AppendLine(string.Format("OR (EventCode='1' and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'))", fromdate, todate));
        }
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT CONVERT(nvarchar(50), Id) + '_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
            query.AppendLine("FROM dbo.tblCardEventHistory e  WITH(NOLOCK)");
            query.AppendLine("where IsDelete=0");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and ((DateTimeIn >= '{0}' and DateTimeIn <= '{1}' and  DateTimeOut >'{1}')", fromdate, todate));
                query.AppendLine(string.Format("OR (EventCode='1' and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'))", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT CONVERT(nvarchar(50), Id) + '_LOOP' as Id, Plate, '' as CardNo, '' as CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CarType, CustomerName, LaneIDIn, UserIDIn");
            query.AppendLine("FROM dbo.tblLoopEvent  WITH(NOLOCK)");
            query.AppendLine("where IsDelete=0");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and ((DateTimeIn >= '{0}' and DateTimeIn <= '{1}' and  DateTimeOut >'{1}')", fromdate, todate));
                query.AppendLine(string.Format("OR (EventCode='1' and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'))", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CarType = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and Plate LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--Count
        query.AppendLine("select COUNT(*) as totalCount");
        query.AppendLine("FROM(");
        query.AppendLine("SELECT Id");
        query.AppendLine("FROM dbo.tblCardEvent e  WITH(NOLOCK)");
        query.AppendLine("where IsDelete=0");
        if (IsHaveTime == true)
        {
            query.AppendLine(string.Format("and ((DateTimeIn >= '{0}' and DateTimeIn <= '{1}' and  DateTimeOut >'{1}')", fromdate, todate));
            query.AppendLine(string.Format("OR (EventCode='1' and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'))", fromdate, todate));
        }
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT Id");
            query.AppendLine("FROM dbo.tblCardEventHistory e  WITH(NOLOCK)");
            query.AppendLine("where IsDelete=0");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and ((DateTimeIn >= '{0}' and DateTimeIn <= '{1}' and  DateTimeOut >'{1}')", fromdate, todate));
                query.AppendLine(string.Format("OR (EventCode='1' and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'))", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));
        }

        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT Id");
            query.AppendLine("FROM dbo.tblLoopEvent  WITH(NOLOCK)");
            query.AppendLine("where IsDelete=0");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and ((DateTimeIn >= '{0}' and DateTimeIn <= '{1}' and  DateTimeOut >'{1}')", fromdate, todate));
                query.AppendLine(string.Format("OR (EventCode='1' and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'))", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CarType = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and Plate LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as e");
        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total);

    }
    public static DataTable GetReportInExcel(string KeyWord, bool IsHaveTime, string fromdate, string todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("");

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY e.DatetimeIn desc) AS STT,");
        query.AppendLine("e.CardNo, e.CardNumber as 'Mã thẻ', e.Plate as 'Biển số', (select convert(varchar(10), e.DateTimeIn, 103) + ' ' + left(convert(varchar(32), e.DateTimeIn, 108), 8)) as 'Thời gian vào', e.CardGroupID as 'Nhóm thẻ', e.CustomerName as 'Khách hàng', e.LaneIDIn as 'Làn vào', e.UserIDIn as 'Giám sát vào'");
        query.AppendLine("FROM(");
        query.AppendLine("SELECT CONVERT(nvarchar(50),Id)+'_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn,'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
        query.AppendLine("FROM dbo.tblCardEvent  WITH (NOLOCK)");
        query.AppendLine("where EventCode = '1'");
        if (IsHaveTime == true)
        {
            query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
        }
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT  CONVERT(nvarchar(50),Id)+'_CARD' as Id, PlateIn as Plate, CardNo, CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn,'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, CardGroupID, CustomerName, LaneIDIn, UserIDIn");
            query.AppendLine("FROM dbo.tblCardEventHistory  WITH(NOLOCK)");
            query.AppendLine("where EventCode = '1'");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CardGroupID = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and (CardNumber = '%{0}%' or CardNo LIKE '%{0}%' or PlateIn LIKE '%{0}%')", KeyWord));

        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("Union");
            query.AppendLine("SELECT CONVERT(nvarchar(50),Id)+'_LOOP' as Id, Plate, '' as CardNo, '' as CardNumber, DateTimeIn, PicDirIn as PicIn1, REPLACE(PicDirIn, 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, CarType, CustomerName, LaneIDIn, UserIDIn");
            query.AppendLine("FROM dbo.tblLoopEvent  WITH (NOLOCK)");
            query.AppendLine("where EventCode = '1'");
            if (IsHaveTime == true)
            {
                query.AppendLine(string.Format("and DateTimeIn >= '{0}' and DateTimeIn <= '{1}'", fromdate, todate));
            }
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("and CarType = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("and LaneIDIn = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("and UserIDIn = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("and Plate LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as e");

        //query.AppendLine("SELECT COUNT(*) totalCount FROM #Results");
        //query.AppendLine("SELECT * FROM #Results");
        //query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
        //query.AppendLine("DROP TABLE #Results");

        return StaticPool.mdbevent.FillData(query.ToString());

    }

    //public static DataTable GetCardList(string KeyWord, string CardGroupID, string CustomerID, string CardState, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    //{
    //    var query = new StringBuilder();
    //    //query.AppendLine("");

    //    query.AppendLine("SELECT ROW_NUMBER() OVER (ORDER BY ca.SortOrder DESC ) AS RowNumber");
    //    query.AppendLine(",ca.CardID");
    //    query.AppendLine(",ca.CardNo");
    //    query.AppendLine(",ca.CardNumber");
    //    query.AppendLine(",ca.CardGroupID");
    //    query.AppendLine(",ca.Plate1");
    //    query.AppendLine(",ca.[ExpireDate]");
    //    query.AppendLine(",ca.IsLock");
    //    query.AppendLine(",c.CustomerName");
    //    query.AppendLine(",c.[Address]");
    //    query.AppendLine(",cg.CustomerGroupName");
    //    query.AppendLine(",cag.CardGroupName");
    //    query.AppendLine(",ca.ImportDate");
    //    query.AppendLine("INTO #Results");
    //    query.AppendLine("FROM tblCard ca");
    //    query.AppendLine("left join tblCustomer c on ca.CustomerID = CONVERT(varchar(50),c.CustomerID)");
    //    query.AppendLine("left join tblCustomerGroup cg on CONVERT(varchar(50),cg.CustomerGroupID) = c.CustomerGroupID");
    //    query.AppendLine("left join tblCardGroup cag on CONVERT(varchar(50),cag.CardGroupID) = ca.CardGroupID");
    //    query.AppendLine("where ca.IsDelete=0 ");
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //    {
    //        query.AppendLine(string.Format("and (ca.CardNo LIKE N'%{0}%'", KeyWord));
    //        query.AppendLine(string.Format("or ca.CardNumber LIKE N'%{0}%' ", KeyWord));
    //        query.AppendLine(string.Format("or ca.Plate1 LIKE N'%{0}%' ", KeyWord));
    //        query.AppendLine(string.Format("or ca.Plate2 LIKE N'%{0}%' ", KeyWord));
    //        query.AppendLine(string.Format("or ca.Plate3 LIKE N'%{0}%' ", KeyWord));
    //        query.AppendLine(string.Format("or c.CustomerCode LIKE N'%{0}' ", KeyWord));
    //        query.AppendLine(string.Format("or c.Address LIKE N'%{0}' ", KeyWord));
    //        query.AppendLine(string.Format("or c.CustomerName LIKE N'%{0}') ", KeyWord));
    //    }

    //    if (!string.IsNullOrWhiteSpace(CardGroupID))
    //        query.AppendLine(string.Format("and ca.CardGroupID LIKE '%{0}%' ", CardGroupID));

    //    if (!string.IsNullOrWhiteSpace(CustomerID))
    //        query.AppendLine(string.Format("and ca.CustomerID LIKE '%{0}%' ", CustomerID));

    //    if (!string.IsNullOrWhiteSpace(CardState))
    //        query.AppendLine(string.Format("and ca.IsLock LIKE '%{0}%' ", CardState));
    //    if (!string.IsNullOrWhiteSpace(CustomerGroupID))
    //        query.AppendLine(string.Format("and c.CustomerGroupID LIKE '%{0}%' ", CustomerGroupID));

    //    query.AppendLine("SELECT COUNT(*) totalCount FROM #Results");

    //    query.AppendLine("SELECT * FROM #Results");

    //    query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

    //    query.AppendLine("DROP TABLE #Results");

    //    return StaticPool.mdb.FillDataPaging(query.ToString(), ref total);

    //}

    public static DataTable GetCardList_2(string KeyWord, string CaNo, string CardGroupID, string CustomerID, string CardState, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        //query.AppendLine("");
        query.AppendLine("SELECT * FROM(");
        query.AppendLine(string.Format("SELECT ROW_NUMBER() OVER (ORDER BY {0} ) AS RowNumber", !string.IsNullOrWhiteSpace(CaNo) ? "ca.CardNo " + CaNo : "ca.SortOrder DESC"));
        query.AppendLine(",ca.CardID");
        query.AppendLine(",ca.CardNo");
        query.AppendLine(",ca.CardNumber");
        query.AppendLine(",ca.CustomerID");
        query.AppendLine(",ca.CardGroupID");
        query.AppendLine(",ca.Plate1");
        query.AppendLine(",ca.[ExpireDate]");
        query.AppendLine(",ca.IsLock");
        query.AppendLine(",'' as CustomerName");
        query.AppendLine(",'' as [Address]");
        query.AppendLine(",'' as CustomerGroupName");
        query.AppendLine(",'' as CardGroupName");
        query.AppendLine(",ca.ImportDate");
        //query.AppendLine("INTO #Results");
        query.AppendLine("FROM tblCard ca  WITH (NOLOCK)");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine("left join tblCustomer c on ca.CustomerID = CONVERT(varchar(50),c.CustomerID)");
        //query.AppendLine("left join tblCustomerGroup cg on CONVERT(varchar(50),cg.CustomerGroupID) = c.CustomerGroupID");
        //query.AppendLine("left join tblCardGroup cag on CONVERT(varchar(50),cag.CardGroupID) = ca.CardGroupID");
        query.AppendLine("where ca.IsDelete=0 ");
        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            query.AppendLine(string.Format("and (ca.CardNo LIKE N'%{0}%'", KeyWord));
            query.AppendLine(string.Format("or ca.CardNumber LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate1 LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate2 LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate3 LIKE N'%{0}%' )", KeyWord));
            //query.AppendLine(string.Format("or c.CustomerCode LIKE N'%{0}%' ", KeyWord));
            //query.AppendLine(string.Format("or c.Address LIKE N'%{0}%' ", KeyWord));
            //query.AppendLine(string.Format("or c.CustomerName LIKE N'%{0}%') ", KeyWord));
        }

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and ca.CardGroupID = '{0}' ", CardGroupID));

        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("and ca.CustomerID = '{0}' ", CustomerID));

        if (!string.IsNullOrWhiteSpace(CardState))
            query.AppendLine(string.Format("and ca.IsLock = '{0}' ", CardState));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = '{0}' ", CustomerGroupID));
        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //-- COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount ");
        query.AppendLine("FROM tblCard ca WITH (NOLOCK)");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine("left join tblCustomer c on ca.CustomerID = CONVERT(varchar(50),c.CustomerID)");
        //query.AppendLine("left join tblCustomerGroup cg on CONVERT(varchar(50),cg.CustomerGroupID) = c.CustomerGroupID");
        //query.AppendLine("left join tblCardGroup cag on CONVERT(varchar(50),cag.CardGroupID) = ca.CardGroupID");
        query.AppendLine("where ca.IsDelete=0 ");
        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            query.AppendLine(string.Format("and (ca.CardNo LIKE N'%{0}%'", KeyWord));
            query.AppendLine(string.Format("or ca.CardNumber LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate1 LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate2 LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate3 LIKE N'%{0}%' )", KeyWord));
            //query.AppendLine(string.Format("or c.CustomerCode LIKE N'%{0}%' ", KeyWord));
            //query.AppendLine(string.Format("or c.Address LIKE N'%{0}%' ", KeyWord));
            //query.AppendLine(string.Format("or c.CustomerName LIKE N'%{0}%') ", KeyWord));
        }

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and ca.CardGroupID = '{0}' ", CardGroupID));

        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("and ca.CustomerID = '{0}' ", CustomerID));

        if (!string.IsNullOrWhiteSpace(CardState))
            query.AppendLine(string.Format("and ca.IsLock = '{0}' ", CardState));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = '{0}' ", CustomerGroupID));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);

    }


    public static DataTable GetCardExprice(string KeyWord, string CardGroupID, string CustomerID, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SELECT * FROM(");
        sb.AppendLine("select  ROW_NUMBER() OVER (ORDER BY ca.ExpireDate desc ) AS RowNumber, ca.CardNo, ca.CardNumber, ca.CustomerID, ca.Plate1, ca.Plate2, ca.Plate3, ca.[ExpireDate], ca.CardGroupID, '' as CustomerName ");
        sb.AppendLine(" from dbo.tblCard ca WITH (NOLOCK) ");
        sb.AppendLine(" LEFT JOIN dbo.tblCardGroup g ON ca.CardGroupID=g.CardGroupID");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
        {
            sb.AppendLine(" LEFT JOIN tblCustomer c ON ca.CustomerID = CONVERT(varchar(255),c.CustomerID)");
        }
        sb.AppendLine(" where ca.IsDelete=0 and ca.IsLock=0 and g.CardType=0");

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
        sb.AppendLine(") as a");
        sb.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //Get Total
        sb.AppendLine("select Count(*) as totalCount");
        sb.AppendLine(" from dbo.tblCard ca WITH (NOLOCK) ");
        sb.AppendLine(" LEFT JOIN dbo.tblCardGroup g ON ca.CardGroupID=g.CardGroupID");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
        {
            sb.AppendLine(" LEFT JOIN tblCustomer c ON ca.CustomerID = CONVERT(varchar(255),c.CustomerID)");
        }
        sb.AppendLine(" where ca.IsDelete=0 and ca.IsLock=0 and g.CardType=0 ");

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

        return StaticPool.mdb.FillDataPaging_2(sb.ToString(), ref total);

    }
    public static DataTable GetCardExprice_Autocomplete(string KeyWord, string CardGroupID, string CustomerGroupID)
    {
        var sb = new StringBuilder();
        sb.AppendLine("select TOP 20 ROW_NUMBER() OVER (ORDER BY ca.ExpireDate desc ) AS RowNumber, ca.CardNo, ca.CardNumber, ca.CustomerID, ca.Plate1, ca.Plate2, ca.Plate3, convert(nvarchar(MAX), ca.[ExpireDate], 103) as ExpireDate, ca.CardGroupID, '' as CustomerName ");
        sb.AppendLine(" from dbo.tblCard ca WITH (NOLOCK) ");
        sb.AppendLine(" LEFT JOIN dbo.tblCardGroup g ON ca.CardGroupID=g.CardGroupID  ");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
        {
            sb.AppendLine(" LEFT JOIN tblCustomer c ON ca.CustomerID = CONVERT(varchar(255),c.CustomerID)");
        }
        sb.AppendLine(" where ca.IsDelete=0 and ca.IsLock=0 and g.CardType=0");
        
        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            sb.AppendLine(" and (ca.CardNo LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" or ca.CardNumber LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" or ca.Plate1 LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" or ca.Plate2 LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(" or ca.Plate3 LIKE N'%" + KeyWord + "%'");
            //sb.AppendLine(" or c.CustomerName LIKE N'%" + KeyWord + "%'");
            sb.AppendLine(string.Format(" or convert(nvarchar(MAX), ca.[ExpireDate], 111) like '%{0}%' )",KeyWord));

        }
        if (!string.IsNullOrWhiteSpace(CardGroupID))
        {
            sb.AppendLine(" and ca.CardGroupID = '" + CardGroupID + "'");

        }
       
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
        {
            sb.AppendLine(" and c.CustomerGroupID='" + CustomerGroupID + "' ");
        }
        return StaticPool.mdb.FillData(sb.ToString());

    }
    public static bool AddCardExprice(string KeyWord, string CardGroupID, string CustomerID, string CustomerGroupID, int _feelevel, string _newexpire, string userId, bool chbEnableMinusActive)
    {
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO tblActiveCard(Code, [Date], CardNumber, Plate, OldExpireDate, [Days], NewExpireDate, CardGroupID, CustomerGroupID, UserID, FeeLevel, CustomerID,IsDelete)");
        sb.AppendLine("SELECT CASE WHEN cus.CustomerCode IS NULL THEN '0' ELSE cus.CustomerCode END,GETDATE(), ca.Cardnumber");
        sb.AppendLine(", CAST(CASE WHEN ca.Plate2 <> '' THEN ca.Plate1 + ';' + ca.Plate2 WHEN ca.Plate3 <> '' THEN ca.Plate1 + ';' + ca.Plate2 + ';' + ca.Plate3 WHEN ca.Plate1 IS NULL THEN '' ELSE ca.Plate1 END AS nvarchar(50)) as Plate");
        sb.AppendLine(string.Format(", ca.[ExpireDate], DATEDIFF(DAY, ca.[ExpireDate], '{0}')", _newexpire));
        sb.AppendLine(string.Format(", '{0}', ca.CardGroupID, CASE WHEN  cus.CustomerGroupID IS NULL THEN '0' ELSE cus.CustomerGroupID END,'{2}','{1}', CASE WHEN ca.CustomerID IS NULL THEN '0' ELSE ca.CustomerID END,0", _newexpire, _feelevel, userId));
        sb.AppendLine("from tblCard ca");
        //if (!string.IsNullOrWhiteSpace(CardGroupID))
        //{
            sb.AppendLine("LEFT JOIN dbo.tblCardGroup g ON ca.CardGroupID=g.CardGroupID");
        //}
        sb.AppendLine("LEFT join tblCustomer cus on ca.CustomerID = CONVERT(varchar(255), cus.CustomerID)");
        sb.AppendLine("where ca.IsDelete = 0 and ca.IsLock=0 and g.CardType=0");
        //Neu so ngay gia han <0 va neu ko check thi ko cho gia han
        if (chbEnableMinusActive == false)
        {
            sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
        }

        //Update theo filler
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
            sb.AppendLine(" and cus.CustomerGroupID='" + CustomerGroupID + "' ");
        }

        //Update lai bang tblCard
        sb.AppendLine("UPDATE");
        sb.AppendLine("ca");
        sb.AppendLine(string.Format("SET ca.ExpireDate = '{0}'", _newexpire));
        sb.AppendLine("FROM");
        sb.AppendLine("tblCard AS ca");
        //if (!string.IsNullOrWhiteSpace(CardGroupID))
        //{
            sb.AppendLine("LEFT JOIN dbo.tblCardGroup AS g ON ca.CardGroupID = g.CardGroupID");
        //}
        sb.AppendLine("LEFT join tblCustomer c on ca.CustomerID = CONVERT(varchar(255), c.CustomerID)");
        sb.AppendLine("WHERE IsDelete = 0 and ca.IsLock=0 and g.CardType = 0");
        if (chbEnableMinusActive == false)
        {
            sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
        }
        //Update theo filler
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

        return StaticPool.mdb.ExecuteCommand(sb.ToString());
    }

    public static bool AddCardExpriceByListCardNumber(string listCardNumber, int _feelevel, string _newexpire, string userId, bool chbEnableMinusActive)
    {
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO tblActiveCard(Code, [Date], CardNumber, Plate, OldExpireDate, [Days], NewExpireDate, CardGroupID, CustomerGroupID, UserID, FeeLevel, CustomerID,IsDelete)");
        sb.AppendLine("SELECT CASE WHEN cus.CustomerCode IS NULL THEN '0' ELSE cus.CustomerCode END,GETDATE(), ca.Cardnumber");
        sb.AppendLine(", CAST(CASE WHEN ca.Plate2 <> '' THEN ca.Plate1 + ';' + ca.Plate2 WHEN ca.Plate3 <> '' THEN ca.Plate1 + ';' + ca.Plate2 + ';' + ca.Plate3 WHEN ca.Plate1 IS NULL THEN '' ELSE ca.Plate1 END AS nvarchar(50)) as Plate");
        sb.AppendLine(string.Format(", ca.[ExpireDate], DATEDIFF(DAY, ca.[ExpireDate], '{0}')", _newexpire));
        sb.AppendLine(string.Format(", '{0}', ca.CardGroupID, CASE WHEN  cus.CustomerGroupID IS NULL THEN '0' ELSE cus.CustomerGroupID END,'{2}','{1}', CASE WHEN ca.CustomerID IS NULL THEN '0' ELSE ca.CustomerID END,0", _newexpire, _feelevel, userId));
        sb.AppendLine("from tblCard ca");
        sb.AppendLine("LEFT join tblCustomer cus on ca.CustomerID = CONVERT(varchar(255), cus.CustomerID)");
        sb.AppendLine("where ca.IsDelete = 0 and ca.IsLock=0");
        //Neu so ngay gia han <0 va neu ko check thi ko cho gia han
        if (chbEnableMinusActive == false)
        {
            sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
        }

        if (!string.IsNullOrWhiteSpace(listCardNumber))
        {
            //where in
            sb.AppendLine(string.Format(" and ca.CardNumber IN ({0})", listCardNumber));
        }

        //Update lai bang tblCard
        sb.AppendLine("UPDATE");
        sb.AppendLine("ca");
        sb.AppendLine(string.Format("SET ca.ExpireDate = '{0}'", _newexpire));
        sb.AppendLine("FROM");
        sb.AppendLine("tblCard AS ca");
        sb.AppendLine("LEFT join tblCustomer c on ca.CustomerID = CONVERT(varchar(255), c.CustomerID)");
        sb.AppendLine("WHERE IsDelete = 0 and ca.IsLock=0 ");

        if (chbEnableMinusActive == false)
        {
            sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
        }
        if (!string.IsNullOrWhiteSpace(listCardNumber))
        {
            //where in
            sb.AppendLine(string.Format(" and ca.CardNumber IN ({0})", listCardNumber));
        }

        return StaticPool.mdb.ExecuteCommand(sb.ToString());
    }
    public static bool AddCardExpriceByListCardNumberByFile(string listCardNumber, string plate, int _feelevel, string _newexpire, string userId, bool chbEnableMinusActive)
    {
        var sb = new StringBuilder();
        sb.AppendLine("INSERT INTO tblActiveCard(Code, [Date], CardNumber, Plate, OldExpireDate, [Days], NewExpireDate, CardGroupID, CustomerGroupID, UserID, FeeLevel, CustomerID,IsDelete)");
        sb.AppendLine("SELECT CASE WHEN cus.CustomerCode IS NULL THEN '0' ELSE cus.CustomerCode END,GETDATE(), ca.Cardnumber");
        sb.AppendLine(", CAST(CASE WHEN ca.Plate2 <> '' THEN ca.Plate1 + ';' + ca.Plate2 WHEN ca.Plate3 <> '' THEN ca.Plate1 + ';' + ca.Plate2 + ';' + ca.Plate3 WHEN ca.Plate1 IS NULL THEN '' ELSE ca.Plate1 END AS nvarchar(50)) as Plate");
        sb.AppendLine(string.Format(", ca.[ExpireDate], DATEDIFF(DAY, ca.[ExpireDate], '{0}')", _newexpire));
        sb.AppendLine(string.Format(", '{0}', ca.CardGroupID, CASE WHEN  cus.CustomerGroupID IS NULL THEN '0' ELSE cus.CustomerGroupID END,'{2}','{1}', CASE WHEN ca.CustomerID IS NULL THEN '0' ELSE ca.CustomerID END,0", _newexpire, _feelevel, userId));
        sb.AppendLine("from tblCard ca");
        sb.AppendLine("LEFT join tblCustomer cus on ca.CustomerID = CONVERT(varchar(255), cus.CustomerID)");
        sb.AppendLine("where ca.IsDelete = 0 and ca.IsLock=0");
        //Neu so ngay gia han <0 va neu ko check thi ko cho gia han
        if (chbEnableMinusActive == false)
        {
            sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
        }

        if (!string.IsNullOrWhiteSpace(listCardNumber))
        {
            //where in
            sb.AppendLine(string.Format(" and (ca.CardNumber IN ({0}) {1}", listCardNumber, !string.IsNullOrWhiteSpace(plate) ? string.Format("OR ca.Plate1 = '{0}' OR ca.Plate2 = '{0}'  OR ca.Plate3 = '{0}' )", plate) : ""));
        }

        //Update lai bang tblCard
        sb.AppendLine("UPDATE");
        sb.AppendLine("ca");
        sb.AppendLine(string.Format("SET ca.ExpireDate = '{0}'", _newexpire));
        sb.AppendLine("FROM");
        sb.AppendLine("tblCard AS ca");
        sb.AppendLine("LEFT join tblCustomer c on ca.CustomerID = CONVERT(varchar(255), c.CustomerID)");
        sb.AppendLine("WHERE IsDelete = 0 and ca.IsLock=0 ");

        if (chbEnableMinusActive == false)
        {
            sb.AppendLine(string.Format("and DATEDIFF(DAY, ca.[ExpireDate], '{0}') >=0  AND ca.[ExpireDate] <= '{0}'", _newexpire));
        }
        if (!string.IsNullOrWhiteSpace(listCardNumber))
        {
            //where in
            sb.AppendLine(string.Format(" and (ca.CardNumber IN ({0}) {1}", listCardNumber, !string.IsNullOrWhiteSpace(plate)?string.Format("OR ca.Plate1 = '{0}' OR ca.Plate2 = '{0}'  OR ca.Plate3 = '{0}' )", plate):""));
        }

        return StaticPool.mdb.ExecuteCommand(sb.ToString());
    }
    public static DataTable GetCardListExcel(string KeyWord, string CaNo, string CardGroupID, string CustomerID, string CardState, string CustomerGroupID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("");

        //query.AppendLine("SELECT * FROM(");
        //query.AppendLine("SELECT ROW_NUMBER() OVER (ORDER BY ca.SortOrder DESC ) AS STT");
        query.AppendLine(string.Format("SELECT ROW_NUMBER() OVER (ORDER BY {0} ) AS STT", !string.IsNullOrWhiteSpace(CaNo) ? "ca.CardNo " + CaNo : "ca.SortOrder DESC"));
        //query.AppendLine("ca.CardID");
        query.AppendLine(",ca.CardNo");
        query.AppendLine(",ca.CardNumber as 'Mã thẻ'");
        query.AppendLine(",cag.CardGroupName as 'Nhóm thẻ'");
        //query.AppendLine(",ca.CardGroupID");
        query.AppendLine(",ca.Plate1 as 'Biển số'");
        //query.AppendLine(",(CAST(DATEPART(day, ca.[ExpireDate]) as varchar(50))+'/'+ CAST(DATEPART(month, ca.[ExpireDate]) as varchar(50))+'/'+CAST(DATEPART(year, ca.[ExpireDate]) as varchar(50))) as 'Ngày hết hạn'");
        query.AppendLine(",(SELECT CONVERT(varchar(19), ca.[ExpireDate], 103)) as 'Ngày hết hạn'");
        query.AppendLine(",CAST( CASE WHEN ca.IsLock=1 THEN N'Khóa' ELSE N'Hoạt động' END AS nvarchar(50)) as 'Trạng thái'");
        query.AppendLine(",c.CustomerCode as 'Mã KH'");
        query.AppendLine(",c.CustomerName as 'Tên khách hàng'");
        query.AppendLine(",c.[Address] as 'Địa chỉ'");
        query.AppendLine(",cg.CustomerGroupName as 'Nhóm KH'");
        //query.AppendLine(",ca.ImportDate as 'Ngày nhập thẻ'");
        query.AppendLine(",(SELECT CONVERT(varchar(19), ca.[ImportDate], 103)) as 'Ngày nhập thẻ'");
        query.AppendLine(",'' as 'Loại xe'");
        query.AppendLine("FROM tblCard ca WITH (NOLOCK)");
        query.AppendLine("left join tblCustomer c on ca.CustomerID = CONVERT(varchar(50),c.CustomerID)");
        query.AppendLine("left join tblCustomerGroup cg on CONVERT(varchar(50),cg.CustomerGroupID) = c.CustomerGroupID");
        query.AppendLine("left join tblCardGroup cag on CONVERT(varchar(50),cag.CardGroupID) = ca.CardGroupID");
        query.AppendLine("where ca.IsDelete=0 ");
        if (!string.IsNullOrWhiteSpace(KeyWord))
        {
            query.AppendLine(string.Format("and (ca.CardNo LIKE N'%{0}%'", KeyWord));
            query.AppendLine(string.Format("or ca.CardNumber LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate1 LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate2 LIKE N'%{0}%' ", KeyWord));
            query.AppendLine(string.Format("or ca.Plate3 LIKE N'%{0}%') ", KeyWord));
        }

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("and ca.CardGroupID = '{0}' ", CardGroupID));

        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("and ca.CustomerID = '{0}' ", CustomerID));

        if (!string.IsNullOrWhiteSpace(CardState))
            query.AppendLine(string.Format("and ca.IsLock = '{0}' ", CardState));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = '{0}' ", CustomerGroupID));
        //query.AppendLine(") as a");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdb.FillData(query.ToString());

    }

    public static bool UpdateCustomerFromExcel(string CustomerId, string CustomerName, string CustomerCode, string CusAddress, string CustomerGroupId, string CardNumber, string CardGroupId, string uId)
    {
        var query = new StringBuilder();
        try
        {
            //query.AppendLine("");
            query.AppendLine(string.Format("IF((SELECT COUNT(*) FROM [dbo].[tblCustomer] WHERE CONVERT(varchar(50),[CustomerID]) = '{0}') > 0)", CustomerId));
            query.AppendLine("BEGIN");
            query.AppendLine("--Update khach hang");
            query.AppendLine("UPDATE [dbo].[tblCustomer]");
            query.AppendLine(string.Format("SET [CustomerName] = N'{0}'", CustomerName));
            query.AppendLine(string.Format(",[Address] = N'{0}'", CusAddress));
            query.AppendLine(string.Format(",[CustomerGroupID] = '{0}'", CustomerGroupId));
            query.AppendLine(string.Format(",[CustomerCode] = '{0}'", CustomerCode));
            query.AppendLine(string.Format("WHERE CONVERT(varchar(50),[CustomerID]) = '{0}'", CustomerId));
            query.AppendLine("END");
            query.AppendLine("ELSE");

            query.AppendLine("BEGIN");
            query.AppendLine("DECLARE @generated_keys table([Id] uniqueidentifier)");

            query.AppendLine("INSERT INTO [dbo].[tblCustomer]");
            query.AppendLine("([CustomerName]");
            query.AppendLine(",[CustomerCode]");
            query.AppendLine(",[Address]");
            query.AppendLine(",[CustomerGroupID]");
            query.AppendLine(",[EnableAccount]");
            query.AppendLine(",[Inactive])");
            query.AppendLine("OUTPUT inserted.CustomerID INTO @generated_keys");
            query.AppendLine(string.Format("VALUES(N'{0}','{1}',N'{2}','{3}',1,0)", CustomerName, CustomerCode, CusAddress, CustomerGroupId));

            query.AppendLine("--Update lai Card");
            query.AppendLine("UPDATE [dbo].[tblCard]");
            query.AppendLine("SET [CustomerID] = (SELECT Top 1 id FROM @generated_keys)");
            query.AppendLine(string.Format("WHERE IsDelete=0 AND [CardNumber] = '{0}'", CardNumber));

            //Update lai Process Card
            query.AppendLine("insert into tblCardProcess([Date], CardNumber, Actions, CardGroupID, UserID, CustomerID)");
            query.AppendLine(string.Format("values('{0}','{1}','RELEASE','{2}','{3}','{4}')", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CardNumber, CardGroupId, uId, CustomerId));
            query.AppendLine("END");

        }
        catch (Exception ex) { }

        return StaticPool.mdb.ExecuteCommand(query.ToString());
    }

    public static bool UpdateCardFromExcel(string CaNo, string CardNumber, string CardGroupId, string ExpireDate, string CustomerId, string ImportDate, string Plate, string IsLock, string VehicleType, string uId, string cusId)
    {
        var query = new StringBuilder();
        try
        {
            CaNo = !string.IsNullOrWhiteSpace(CaNo) ? CaNo.Trim() : "";
            CardNumber = !string.IsNullOrWhiteSpace(CardNumber) ? CardNumber.Trim() : "";

            CustomerId = !string.IsNullOrWhiteSpace(CustomerId) ? CustomerId.Trim() : "0";
            CardGroupId = !string.IsNullOrWhiteSpace(CardGroupId) ? CardGroupId.Trim() : "0";
            ImportDate = !string.IsNullOrWhiteSpace(ImportDate) ? Convert.ToDateTime(ImportDate.Trim()).ToString("yyyy-MM-dd") : Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            ExpireDate = !string.IsNullOrWhiteSpace(ExpireDate) ? Convert.ToDateTime(ExpireDate.Trim()).ToString("yyyy-MM-dd") : Convert.ToDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            Plate = !string.IsNullOrWhiteSpace(Plate) ? Plate.Trim() : "";
            IsLock = (IsLock == "Hoạt động") || IsLock == "" ? "0" : "1";
            VehicleType = !string.IsNullOrWhiteSpace(VehicleType) ? VehicleType.Trim() : "";
            //query.AppendLine("");
            query.AppendLine(string.Format("IF((SELECT COUNT(*) FROM[dbo].[tblCard] WHERE IsDelete =0 AND  [CardNumber] = '{0}') > 0)", CardNumber));
            query.AppendLine("BEGIN");
            query.AppendLine("UPDATE [dbo].[tblCard] SET");
            if (!string.IsNullOrWhiteSpace(CaNo))
                query.AppendLine(string.Format("[CardNo] = '{0}'", CaNo));

            if (!string.IsNullOrWhiteSpace(CustomerId))
                query.AppendLine(string.Format(",[CustomerID] = '{0}'", CustomerId));

            if (!string.IsNullOrWhiteSpace(CardGroupId))
                query.AppendLine(string.Format(",[CardGroupID] = '{0}'", CardGroupId));

            if (!string.IsNullOrWhiteSpace(ImportDate))
                query.AppendLine(string.Format(",[ImportDate] = '{0}'", ImportDate));

            if (!string.IsNullOrWhiteSpace(ExpireDate))
                query.AppendLine(string.Format(",[ExpireDate] = '{0}'", ExpireDate));

            if (!string.IsNullOrWhiteSpace(Plate))
                query.AppendLine(string.Format(",[Plate1] = '{0}'", Plate));

            if (!string.IsNullOrWhiteSpace(VehicleType))
                query.AppendLine(string.Format(",[VehicleName1] = N'{0}'", VehicleType));

            if (!string.IsNullOrWhiteSpace(IsLock))
                query.AppendLine(string.Format(",[IsLock] = '{0}'", IsLock));

            //query.AppendLine(string.Format(",[DateRegister] = '{0}'", _date.ToString("yyyy-MM-dd hh:mm:ss")));
            query.AppendLine(string.Format("WHERE [CardNumber] = '{0}' and  [IsDelete] = 0", CardNumber));
            query.AppendLine("END");
            query.AppendLine("ELSE");
            query.AppendLine("BEGIN");
            query.AppendLine(string.Format("INSERT INTO [dbo].[tblCard]([CardNo], [CardNumber],[CustomerID],[CardGroupID],[ImportDate],[ExpireDate],[Plate1],[IsLock],[IsDelete],[VehicleName1]) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',N'{9}')", CaNo, CardNumber, CustomerId, CardGroupId, ImportDate, ExpireDate, Plate, IsLock, 0, VehicleType));


            // update Process card
            query.AppendLine("insert into tblCardProcess([Date], CardNumber, Actions, CardGroupID, UserID)");
            query.AppendLine(string.Format("values('{0}', '{1}', 'ADD', '{2}', '{3}')", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CardNumber, CardGroupId, uId));

            query.AppendLine("END");
        }
        catch (Exception ex) { }

        return StaticPool.mdb.ExecuteCommand(query.ToString());
    }

    //public static DataTable GetReportInOut(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize, ref int total, bool IsFilterByTimeIn = true)
    //{
    //    var query = new StringBuilder();

    //    query.AppendLine(string.Format("select ROW_NUMBER() OVER(ORDER BY {0} desc) AS RowNumber,", IsFilterByTimeIn ? "a.[DatetimeIn]" : "a.[DatetimeOut]"));
    //    query.AppendLine("a.[CardNo], a.[CardNumber], a.[Plate], a.[DatetimeIn], a.[DatetimeOut], a.[PicIn1], a.[PicIn2], a.[PicOut1], a.[PicOut2], a.[CardGroupID], a.[CustomerName], a.[LaneIDIn], a.[LaneIDOut], a.[UserIDIn], a.[UserIDOut], a.[Moneys]");
    //    query.AppendLine("INTO #Result FROM(");

    //    DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
    //    if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
    //    {
    //        //tblCardEvent
    //        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[PicDirIn] as PicIn1, REPLACE(e.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, e.[PicDirOut] as PicOut1, REPLACE(e.[PicDirOut],'PLATEOUT.JPG','OVERVIEWOUT.JPG') as PicOut2, e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");

    //        query.AppendLine("FROM dbo.[tblCardEvent] e  WITH (NOLOCK)");

    //        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2'");
    //        //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn < '{1}'", _fromdate, _todate));

    //        query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND e.[DatetimeIn] >= '{0}' AND e.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));

    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND e.[CardNumber] LIKE '{0}' OR e.[CardNo] LIKE '{0}' OR e.[PlateIn] LIKE '{0}' OR e.[PlateOut] LIKE '{0}'", KeyWord));

    //        query.AppendLine("UNION");

    //        //tblCardEventHistory
    //        query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[PicDirIn] as PicIn1, REPLACE(eh.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, eh.[PicDirOut] as PicOut1, REPLACE(eh.[PicDirOut],'PLATEOUT.JPG','OVERVIEWOUT.JPG') as PicOut2, eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys]");

    //        query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");

    //        query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2'");
    //        //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn <= '{1}'", _fromdate, _todate));
    //        query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND eh.[DatetimeIn] >= '{0}' AND eh.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND eh.[LaneIDIn] = '{0}' OR eh.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND eh.[UserIDIn] = '{0}' OR eh.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND eh.[CardNumber] LIKE '{0}' OR eh.[CardNo] LIKE '{0}' OR eh.[PlateIn] LIKE '{0}' OR eh.[PlateOut] LIKE '{0}'", KeyWord));

    //        query.AppendLine("UNION");

    //        //tblLoopEvent
    //        query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[PicDirIn] as PicIn1, REPLACE(le.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, le.[PicDirOut] as PicOut1, REPLACE(le.[PicDirOut],'PLATEOUT.JPG','OVERVIEWOUT.JPG') as PicOut2, le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys]");

    //        query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

    //        query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
    //        //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn <= '{1}'", _fromdate, _todate));
    //        query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND le.[LaneIDIn] = '{0}' OR le.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND le.[UserIDIn] = '{0}' OR le.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND le.[Plate] LIKE '{0}'", KeyWord));
    //    }
    //    else
    //    {
    //        //tblCardEvent
    //        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[PicDirIn] as PicIn1, REPLACE(e.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, e.[PicDirOut] as PicOut1, REPLACE(e.[PicDirOut],'PLATEOUT.JPG','OVERVIEWOUT.JPG') as PicOut2, e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");

    //        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

    //        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2'");
    //        //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn <= '{1}'", _fromdate, _todate));
    //        query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND e.[DatetimeIn] >= '{0}' AND e.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND e.[CardNumber] LIKE '{0}' OR e.[CardNo] LIKE '{0}' OR e.[PlateIn] LIKE '{0}' OR e.[PlateOut] LIKE '{0}'", KeyWord));

    //        query.AppendLine("UNION");

    //        //tblLoopEvent
    //        query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[PicDirIn] as PicIn1, REPLACE(le.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, le.[PicDirOut] as PicOut1, REPLACE(le.[PicDirOut],'PLATEOUT.JPG','OVERVIEWOUT.JPG') as PicOut2, le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys]");

    //        query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

    //        query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
    //        //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn <= '{1}'", _fromdate, _todate));
    //        query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND le.[LaneIDIn] = '{0}' OR le.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND le.[UserIDIn] = '{0}' OR le.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND le.[Plate] LIKE '{0}'", KeyWord));

    //    }
    //    query.AppendLine(") as a");

    //    query.AppendLine("SELECT COUNT(*) totalCount FROM #Result");
    //    query.AppendLine("SELECT * FROM #Result");
    //    query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
    //    query.AppendLine("DROP TABLE #Result");

    //    return StaticPool.mdbevent.FillDataPaging(query.ToString(), ref total);
    //}

    public static DataTable GetReportInOut_2(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize, ref int total, bool IsFilterByTimeIn = true)
    {
        var query = new StringBuilder();
        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) as RowNumber,a.*");
        query.AppendLine("FROM(");
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST(CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[PicDirIn] as PicIn1, REPLACE(e.[PicDirIn], 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, e.[PicDirOut] as PicOut1, REPLACE(e.[PicDirOut], 'PLATEOUT.JPG', 'OVERVIEWOUT.JPG') as PicOut2, e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");

        query.AppendLine("FROM dbo.[tblCardEvent] e WITH(NOLOCK)");
        query.AppendLine(string.Format(" WHERE e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}'  AND e.[IsDelete] = 0 and e.[EventCode] = '2'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND (e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}')", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND (e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}')", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST(CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[PicDirIn] as PicIn1, REPLACE(e.[PicDirIn], 'PLATEIN.JPG', 'OVERVIEWIN.JPG') as PicIn2, e.[PicDirOut] as PicOut1, REPLACE(e.[PicDirOut], 'PLATEOUT.JPG', 'OVERVIEWOUT.JPG') as PicOut2, e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");

            query.AppendLine("FROM dbo.[tblCardEventHistory] e WITH(NOLOCK)");
            query.AppendLine(string.Format(" WHERE e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}'  AND e.[IsDelete] = 0 and e.[EventCode] = '2'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND (e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}')", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND (e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}')", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));
        }

        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[PicDirIn] as PicIn1, REPLACE(le.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, le.[PicDirOut] as PicOut1, REPLACE(le.[PicDirOut],'PLATEOUT.JPG','OVERVIEWOUT.JPG') as PicOut2, le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys]");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
            //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn <= '{1}'", _fromdate, _todate));
            query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND (le.[LaneIDIn] = '{0}' OR le.[LaneIDOut] = '{0}')", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND (le.[UserIDIn] = '{0}' OR le.[UserIDOut] = '{0}')", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        query.AppendLine(") as TEMP");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--Count Total
        query.AppendLine("SELECT COUNT(Id) as totalCount");
        query.AppendLine("FROM ( SELECT Id FROM dbo.[tblCardEvent]");
        query.AppendLine("e WITH(NOLOCK)");
        query.AppendLine(string.Format("WHERE e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}'  AND e.[IsDelete] = 0 and e.[EventCode] = '2'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND (e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}')", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND (e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}')", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT Id");
            query.AppendLine("FROM dbo.[tblCardEventHistory] e WITH(NOLOCK)");
            query.AppendLine(string.Format(" WHERE e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}'  AND e.[IsDelete] = 0 and e.[EventCode] = '2'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND (e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}')", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND (e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}')", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT Id");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
            //query.AppendLine(string.Format("AND DateTimeIn>='{0}' and DateTimeIn <= '{1}'", _fromdate, _todate));
            query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND (le.[LaneIDIn] = '{0}' OR le.[LaneIDOut] = '{0}')", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND (le.[UserIDIn] = '{0}' OR le.[UserIDOut] = '{0}')", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }

        query.AppendLine(") as e");
        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total);
    }

    public static DataTable GetReportInOutExcel(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, bool IsFilterByTimeIn = true, int pageIndex = 1, int pageSize = StaticPool.pagesize)
    {
        var query = new StringBuilder();
        var dtrtn = new DataTable();
        try
        {
            //query.AppendLine("SELECT * FROM(");
            query.AppendLine(string.Format("select ROW_NUMBER() OVER(ORDER BY {0} desc) AS STT,", IsFilterByTimeIn ? "a.[DatetimeIn]" : "a.[DatetimeOut]"));
            query.AppendLine("a.[CardNo], a.[CardNumber] AS 'Mã thẻ', a.[Plate] AS 'Biển số', (select convert(varchar(10), a.DateTimeIn, 103) + ' ' + left(convert(varchar(32), a.DateTimeIn, 108), 8)) AS 'Thời gian vào', (select convert(varchar(10), a.DatetimeOut, 103) + ' ' + left(convert(varchar(32), a.DatetimeOut, 108), 8)) AS 'Thời gian ra', a.[CardGroupID] AS 'Nhóm thẻ', a.[CustomerName] AS 'Khách hàng', a.[LaneIDIn] AS 'Làn vào', a.[LaneIDOut] AS 'Làn ra', a.[UserIDIn] AS 'Giám sát vào', a.[UserIDOut] AS 'Giám sát ra', a.[Moneys] AS 'Tiền'");
            query.AppendLine("FROM(");

            query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");
            query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
            query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2'");
            query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND e.[DatetimeIn] >= '{0}' AND e.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));

            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND e.[LaneIDIn] = '{0}' OR e.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND e.[UserIDIn] = '{0}' OR e.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%'", KeyWord));

            DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
            if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
            {
                query.AppendLine("UNION");
                query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys]");
                query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
                query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2'");
                query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND eh.[DatetimeIn] >= '{0}' AND eh.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate)));
                if (!string.IsNullOrWhiteSpace(CardGroupID))
                    query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
                if (!string.IsNullOrWhiteSpace(LaneID))
                    query.AppendLine(string.Format("AND eh.[LaneIDIn] = '{0}' OR eh.[LaneIDOut] = '{0}'", LaneID));
                if (!string.IsNullOrWhiteSpace(UserID))
                    query.AppendLine(string.Format("AND eh.[UserIDIn] = '{0}' OR eh.[UserIDOut] = '{0}'", UserID));
                if (!string.IsNullOrWhiteSpace(KeyWord))
                    query.AppendLine(string.Format("AND eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%'", KeyWord));
            }
            var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
            if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
            {
                query.AppendLine("UNION");
                query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys]");
                query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
                query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
                query.AppendLine(string.Format("{0}", IsFilterByTimeIn ? string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate) : string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate)));
                if (!string.IsNullOrWhiteSpace(CardGroupID))
                    query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
                if (!string.IsNullOrWhiteSpace(LaneID))
                    query.AppendLine(string.Format("AND le.[LaneIDIn] = '{0}' OR le.[LaneIDOut] = '{0}'", LaneID));
                if (!string.IsNullOrWhiteSpace(UserID))
                    query.AppendLine(string.Format("AND le.[UserIDIn] = '{0}' OR le.[UserIDOut] = '{0}'", UserID));
                if (!string.IsNullOrWhiteSpace(KeyWord))
                    query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
            }
            query.AppendLine(") as a");

            dtrtn = StaticPool.mdbevent.FillData(query.ToString());
        }
        catch (Exception ex)
        {
        }
        //query.AppendLine(") as TEMP");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
        return dtrtn;
    }

    public static DataTable GetReportVehicleComeIn(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        query.AppendLine("SELECT * FROM(");
        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeIn] desc) AS RowNumber,");
        query.AppendLine("a.[CardNo], a.[CardNumber], a.[Plate], a.[DatetimeIn], a.[PicIn1], a.[PicIn2], a.[CardGroupID], a.[CustomerName], a.[LaneIDIn], a.[UserIDIn]");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[PicDirIn] as PicIn1, REPLACE(e.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[UserIDIn]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0");
        query.AppendLine(string.Format("AND e.[DatetimeIn] >= '{0}' AND e.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDIn] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDIn] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[PicDirIn] as PicIn1, REPLACE(eh.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[UserIDIn]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0");
            query.AppendLine(string.Format("AND eh.[DatetimeIn] >= '{0}' AND eh.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDIn] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDIn] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%')", KeyWord));
        }

        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[PicDirIn] as PicIn1, REPLACE(le.[PicDirIn],'PLATEIN.JPG','OVERVIEWIN.JPG') as PicIn2, le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[UserIDIn]");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
            query.AppendLine(string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDIn] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDIn] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount FROM(");
        query.AppendLine("SELECT e.Id");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0");
        query.AppendLine(string.Format("AND e.[DatetimeIn] >= '{0}' AND e.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDIn] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDIn] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.Id");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0");
            query.AppendLine(string.Format("AND eh.[DatetimeIn] >= '{0}' AND eh.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDIn] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDIn] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%')", KeyWord));
        }

        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT le.Id");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
            query.AppendLine(string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDIn] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDIn] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total);
    }

    public static DataTable GetReportVehicleComeInExcel(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeIn] desc) AS STT,");
        query.AppendLine("a.[CardNo], a.[CardNumber] AS 'Mã thẻ', a.[Plate] AS 'Biển số', (select convert(varchar(10), a.DatetimeIn, 103) + ' ' + left(convert(varchar(32), a.DatetimeIn, 108), 8)) AS 'Thời gian vào', a.[CardGroupID] AS 'Nhóm thẻ', a.[CustomerName] AS 'Kháng hàng', a.[LaneIDIn] AS 'Làn vào', a.[UserIDIn] AS 'Giám sát vào'");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[UserIDIn]");

        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

        query.AppendLine("WHERE e.[IsDelete] = 0");
        query.AppendLine(string.Format("AND e.[DatetimeIn] >= '{0}' AND e.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDIn] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDIn] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            //tblCardEventHistory
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[UserIDIn]");

            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");

            query.AppendLine("WHERE eh.[IsDelete] = 0");
            query.AppendLine(string.Format("AND eh.[DatetimeIn] >= '{0}' AND eh.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDIn] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDIn] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%')", KeyWord));
        }

        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            //tblLoopEvent
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DateTimeIn], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[UserIDIn]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2'");
            query.AppendLine(string.Format("AND le.[DatetimeIn] >= '{0}' AND le.[DatetimeIn] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDIn] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDIn] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }

        query.AppendLine(") as a");
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
        return StaticPool.mdbevent.FillData(query.ToString());
    }

    //public static DataTable GetReportDetailMoneyCardDay(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize, ref int total, ref long _totalmoneys)
    //{
    //    var query = new StringBuilder();

    //    query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS RowNumber,");
    //    query.AppendLine("a.[CardNo], a.[CardNumber], a.[Plate], a.[DatetimeIn], a.[DatetimeOut], a.[CardGroupID], a.[CustomerName], a.[LaneIDIn], a.[LaneIDOut], a.[UserIDIn], a.[UserIDOut], a.[Moneys], '' AS TotalTimes, a.[Id] ");
    //    query.AppendLine("INTO #Result FROM(");

    //    DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
    //    if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
    //    {
    //        //tblCardEvent
    //        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS TotalTimes, e.[Id]");

    //        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

    //        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
    //        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND e.[CardNumber] LIKE '{0}' OR e.[CardNo] LIKE '{0}' OR e.[PlateIn] LIKE '{0}' OR e.[PlateOut] LIKE '{0}'", KeyWord));

    //        query.AppendLine("UNION");

    //        //tblCardEventHistory
    //        query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS TotalTimes, eh.[Id]");

    //        query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");

    //        query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
    //        query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND eh.[CardNumber] LIKE '{0}' OR eh.[CardNo] LIKE '{0}' OR eh.[PlateIn] LIKE '{0}' OR eh.[PlateOut] LIKE '{0}'", KeyWord));

    //        query.AppendLine("UNION");

    //        //tblLoopEvent
    //        query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS TotalTimes, le.[Id]");

    //        query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

    //        query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
    //        query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND le.[Plate] LIKE '{0}'", KeyWord));
    //    }
    //    else
    //    {
    //        //tblCardEvent
    //        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS TotalTimes, e.[Id]");

    //        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

    //        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
    //        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND e.[CardNumber] LIKE '{0}' OR e.[CardNo] LIKE '{0}' OR e.[PlateIn] LIKE '{0}' OR e.[PlateOut] LIKE '{0}'", KeyWord));

    //        query.AppendLine("UNION");

    //        //tblLoopEvent
    //        query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS TotalTimes, le.[Id]");

    //        query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

    //        query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
    //        query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
    //        if (!string.IsNullOrWhiteSpace(CardGroupID))
    //            query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
    //        if (!string.IsNullOrWhiteSpace(LaneID))
    //            query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
    //        if (!string.IsNullOrWhiteSpace(UserID))
    //            query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
    //        if (!string.IsNullOrWhiteSpace(KeyWord))
    //            query.AppendLine(string.Format("AND le.[Plate] LIKE '{0}'", KeyWord));

    //    }

    //    query.AppendLine(") as a");

    //    query.AppendLine("SELECT COUNT(*) totalCount, SUM(Moneys) AS totalMoney FROM #Result");
    //    query.AppendLine("SELECT * FROM #Result");
    //    query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
    //    query.AppendLine("DROP TABLE #Result");

    //    return StaticPool.mdbevent.FillDataPaging(query.ToString(), ref total, ref _totalmoneys);
    //}

    public static DataTable GetReportDetailMoneyCardDay_2(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize, ref int total, ref long _totalmoneys)
    {
        var query = new StringBuilder();
        query.AppendLine("SELECT * FROM(");
        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS RowNumber, a.*");
        query.AppendLine("FROM(");

        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS TotalTimes, e.[Id]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS TotalTimes, eh.[Id]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS TotalTimes, le.[Id]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        //}
        //else
        //{
        //    //tblCardEvent
        //    query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS TotalTimes, e.[Id]");

        //    query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

        //    query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        //    query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
        //    if (!string.IsNullOrWhiteSpace(CardGroupID))
        //        query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        //    if (!string.IsNullOrWhiteSpace(LaneID))
        //        query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        //    if (!string.IsNullOrWhiteSpace(UserID))
        //        query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        //    if (!string.IsNullOrWhiteSpace(KeyWord))
        //        query.AppendLine(string.Format("AND e.[CardNumber] LIKE '{0}' OR e.[CardNo] LIKE '{0}' OR e.[PlateIn] LIKE '{0}' OR e.[PlateOut] LIKE '{0}'", KeyWord));

        //    query.AppendLine("UNION");

        //    //tblLoopEvent
        //    query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS TotalTimes, le.[Id]");

        //    query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

        //    query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
        //    query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
        //    if (!string.IsNullOrWhiteSpace(CardGroupID))
        //        query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
        //    if (!string.IsNullOrWhiteSpace(LaneID))
        //        query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
        //    if (!string.IsNullOrWhiteSpace(UserID))
        //        query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
        //    if (!string.IsNullOrWhiteSpace(KeyWord))
        //        query.AppendLine(string.Format("AND le.[Plate] LIKE '{0}'", KeyWord));

        //}

        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount, SUM(a.Moneys) AS totalMoney FROM (");
        query.AppendLine("SELECT e.[Id], e.Moneys");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[Id], eh.Moneys");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%'", KeyWord));
        }
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT le.[Id], le.Moneys");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total, ref _totalmoneys);
    }



    public static DataTable GetReportDetailMoneyCardDay_CheckRecord(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int bRow, int eRow, ref int total, ref long _totalmoneys)
    {
        var query = new StringBuilder();
        query.AppendLine("SELECT * FROM(");
        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS RowNumber, a.*");
        query.AppendLine("FROM(");

        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS TotalTimes, e.[Id]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS TotalTimes, eh.[Id]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS TotalTimes, le.[Id]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }

        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN {0} AND {1}", bRow, eRow));

        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount, SUM(a.Moneys) AS totalMoney FROM (");
        query.AppendLine("SELECT e.[Id], e.Moneys");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[Id], eh.Moneys");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%'", KeyWord));
        }
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT le.[Id], le.Moneys");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total, ref _totalmoneys);
    }

    public static DataTable GetReportDetailMoneyCardDayExcel(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.DatetimeOut desc) AS STT, a.[CardNo], a.[CardNumber] AS 'Mã thẻ', a.[Plate] AS 'Biển số', (select convert(varchar(10), a.DatetimeIn, 103) + ' ' + left(convert(varchar(32), a.DatetimeIn, 108), 8)) AS 'Thời gian vào', (select convert(varchar(10), a.DatetimeOut, 103) + ' ' + left(convert(varchar(32), a.DatetimeOut, 108), 8)) AS 'Thời gian ra', a.[CardGroupID] AS 'Nhóm thẻ', a.[CustomerName] AS 'Khách hàng', a.[LaneIDIn] AS 'Làn vào', a.[LaneIDOut] AS 'Làn ra', a.[UserIDIn] AS 'Giám sát vào', a.[UserIDOut] AS 'Giám sát ra', a.[Moneys] AS 'Tiền', '' AS 'Tổng thời gian'");
        query.AppendLine("FROM(");
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS 'Tổng thời gian'");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS 'Tổng thời gian'");

            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");

            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS 'Tổng thời gian'");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        //}
        //else
        //{
        //    //tblCardEvent
        //    query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS 'Tổng thời gian'");

        //    query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

        //    query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        //    query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
        //    if (!string.IsNullOrWhiteSpace(CardGroupID))
        //        query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        //    if (!string.IsNullOrWhiteSpace(LaneID))
        //        query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        //    if (!string.IsNullOrWhiteSpace(UserID))
        //        query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        //    if (!string.IsNullOrWhiteSpace(KeyWord))
        //        query.AppendLine(string.Format("AND e.[CardNumber] LIKE '{0}' OR e.[CardNo] LIKE '{0}' OR e.[PlateIn] LIKE '{0}' OR e.[PlateOut] LIKE '{0}'", KeyWord));

        //    query.AppendLine("UNION");

        //    //tblLoopEvent
        //    query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS 'Tổng thời gian'");

        //    query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

        //    query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
        //    query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
        //    if (!string.IsNullOrWhiteSpace(CardGroupID))
        //        query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
        //    if (!string.IsNullOrWhiteSpace(LaneID))
        //        query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
        //    if (!string.IsNullOrWhiteSpace(UserID))
        //        query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
        //    if (!string.IsNullOrWhiteSpace(KeyWord))
        //        query.AppendLine(string.Format("AND le.[Plate] LIKE '{0}'", KeyWord));

        //}

        query.AppendLine(") as a");
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
        return StaticPool.mdbevent.FillData(query.ToString());
    }

    public static DataTable GetReportDetailMoneyCardDayFile(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID)
    {
        var query = new StringBuilder();

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS STT,");
        query.AppendLine("a.[CardNo], a.[CardNumber], a.[Plate], a.[DatetimeIn], a.[DatetimeOut], a.[CardGroupID], a.[CustomerName], a.[LaneIDIn], a.[LaneIDOut], a.[UserIDIn], a.[UserIDOut], a.[Moneys], '' AS 'TotalTimes'");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS 'TotalTimes'");

        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");

        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%'", KeyWord));
        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            //tblCardEventHistory
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS 'TotalTimes'");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%'", KeyWord));
        }

        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            //tblLoopEvent
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS 'TotalTimes'");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillData(query.ToString());
    }

    public static DataTable GetReportDetailMoneyCardMonth(string keyword, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, string UserID, int pageIndex, int pageSize, ref int total, ref long _totalmoneys)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY Date desc) AS RowNumber, Date, CardNumber, CardGroupID, Plate, CustomerID, CustomerGroupID, OldExpireDate, NewExpireDate, FeeLevel, UserID ");
        query.AppendLine(" FROM tblActiveCard WITH (NOLOCK) WHERE IsDelete = 0");
        query.AppendLine(string.Format("AND [Date] >= '{0}' AND [Date] < '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(keyword))
            query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", keyword));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND CustomerID = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND CustomerGroupID = '{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND UserID = '{0}'", UserID));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL
        query.AppendLine("SELECT COUNT(*) totalCount,SUM(FeeLevel) AS totalMoney FROM (");
        query.AppendLine("SELECT Id,FeeLevel FROM tblActiveCard WITH (NOLOCK) WHERE IsDelete = 0");
        query.AppendLine(string.Format("AND [Date] >= '{0}' AND [Date] < '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(keyword))
            query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", keyword));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND CustomerID = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND CustomerGroupID = '{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND UserID = '{0}'", UserID));

        query.AppendLine(") as a");


        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total, ref _totalmoneys);
    }


    public static DataTable GetReportDetailMoneyCardMonth_CheckRow(string keyword, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, string UserID, int pageIndex, int pageSize, int bRow, int eRow, ref int total, ref long _totalmoneys)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY Date desc) AS RowNumber, Date, CardNumber, CardGroupID, Plate, CustomerID, CustomerGroupID, OldExpireDate, NewExpireDate, FeeLevel, UserID ");
        query.AppendLine(" FROM tblActiveCard WITH (NOLOCK) WHERE IsDelete = 0");
        query.AppendLine(string.Format("AND [Date] >= '{0}' AND [Date] < '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(keyword))
            query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", keyword));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND CustomerID = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND CustomerGroupID = '{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND UserID = '{0}'", UserID));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN {0} AND {1}", bRow, eRow));

        //--COUNT TOTAL
        query.AppendLine("SELECT COUNT(*) totalCount,SUM(FeeLevel) AS totalMoney FROM (");
        query.AppendLine("SELECT Id,FeeLevel FROM tblActiveCard WITH (NOLOCK) WHERE IsDelete = 0");
        query.AppendLine(string.Format("AND [Date] >= '{0}' AND [Date] < '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(keyword))
            query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", keyword));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND CustomerID = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND CustomerGroupID = '{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND UserID = '{0}'", UserID));

        query.AppendLine(") as a");


        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total, ref _totalmoneys);
    }

    public static DataTable GetReportDetailMoneyCardMonthExcel(string keyword, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, string UserID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY Date desc) AS STT, (select convert(varchar(10), [Date], 103) + ' ' + left(convert(varchar(32), [Date], 108), 8)) AS 'Ngày thực hiện', CardNumber AS 'Mã thẻ', CardGroupID AS 'Nhóm thẻ', Plate AS 'Biển số', CustomerID AS 'Khách hàng', CustomerGroupID AS 'Nhóm khách hàng', (SELECT CONVERT(varchar(19),[OldExpireDate], 103)) AS 'Thời hạn cũ', (SELECT CONVERT(varchar(19),[NewExpireDate], 103)) AS 'Thời hạn mới', FeeLevel AS 'Số tiền', UserID AS 'NV thực hiện'");
        query.AppendLine("FROM tblActiveCard WITH (NOLOCK) WHERE IsDelete = 0");
        query.AppendLine(string.Format("AND [Date] >= '{0}' AND [Date] < '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(keyword))
            query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", keyword));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND CustomerID = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND CustomerGroupID = '{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND UserID = '{0}'", UserID));
        return StaticPool.mdb.FillData(query.ToString());
    }

    public static DataTable GetReportDetailMoneyCardMonthFile(string keyword, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, string UserID)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY Date desc) AS RowNumber, Date, CardNumber, CardGroupID, Plate, CustomerID, CustomerGroupID, OldExpireDate, NewExpireDate, FeeLevel, UserID");
        query.AppendLine("FROM tblActiveCard WITH (NOLOCK) WHERE IsDelete = 0");
        query.AppendLine(string.Format("AND [Date] >= '{0}' AND [Date] < '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(keyword))
            query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%'", keyword));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND CardGroupID = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND CustomerID = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND CustomerGroupID = '{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND UserID = '{0}'", UserID));


        return StaticPool.mdb.FillData(query.ToString());
    }

    public static DataTable GetReportDetailMoneyCardDay2(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize, ref int total, ref long _totalmoneys)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS RowNumber, a. *");
        query.AppendLine("FROM(");

        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS TotalTimes, e.[Id], pi.[PrintIndex], pi.[Para1], pi.[Para2]");

        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN dbo.[tblPrintIndex] pi on pi.[EventID] = e.[Id]");

        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] = '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        var isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            //tblCardEventHistory
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS TotalTimes, eh.[Id], pi.[PrintIndex], pi.[Para1], pi.[Para2]");

            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("LEFT JOIN dbo.[tblPrintIndex] pi on pi.[EventID] = eh.[Id]");

            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS TotalTimes, le.[Id], '' AS [PrintIndex], '' AS [Para1], '' AS [Para2]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));


        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount,SUM(Moneys) AS totalMoney FROM (");
        query.AppendLine("SELECT e.[Id], e.[Moneys]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN dbo.[tblPrintIndex] pi on pi.[EventID] = e.[Id]");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[Id], eh.[Moneys]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("LEFT JOIN dbo.[tblPrintIndex] pi on pi.[EventID] = eh.[Id]");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT [Id], [Moneys]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total, ref _totalmoneys);
    }

    public static DataTable GetReportDetailMoneyCardDay2Excel(string KeyWord, string UserID, string _fromdate, string _todate, string CardGroupID, string LaneID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS STT, a.[CardNo], a.[CardNumber] AS 'Mã thẻ', a.[Plate] AS 'Biển số', (select convert(varchar(10), a.[DatetimeIn], 103) + ' ' + left(convert(varchar(32), a.[DatetimeIn], 108), 8)) AS 'Thời gian vào', (select convert(varchar(10), a.[DatetimeOut], 103) + ' ' + left(convert(varchar(32), a.[DatetimeOut], 108), 8)) AS 'Thời gian ra', a.[CardGroupID] AS 'Nhóm thẻ', a.[CustomerName] AS 'Khách hàng', a.[LaneIDIn] AS 'Làn vào', a.[LaneIDOut] AS 'Làn ra', a.[UserIDIn] AS 'Giám sát vào', a.[UserIDOut] AS 'Giám sát ra', a.[Moneys] AS 'Tiền', '' AS 'Tổng thời gian', a.[PrintIndex] AS 'Số HĐ', a.[Para1] AS 'Mẫu số', a.[Para2] AS 'KH'");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNo], e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], '' AS 'Tổng thời gian', pri.[PrintIndex], pri.[Para1], pri.[Para2]");

        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN dbo.[tblPrintIndex] pri on pri.[EventID] = e.[Id]");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 0 AND e.[Moneys] > 0");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[CardNo], eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], '' AS 'Tổng thời gian', pi.[PrintIndex], pi.[Para1], pi.[Para2]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("LEFT JOIN dbo.[tblPrintIndex] pi on pi.[EventID] = eh.[Id]");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 0 AND eh.[Moneys] > 0");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNo, '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '' AS 'Tổng thời gian', '' AS [PrintIndex], '' AS [Para1], '' AS [Para2]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 0 AND le.[Moneys] > 0");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdbevent.FillData(query.ToString());
    }

    public static DataTable GetReportVehicleFreeDetail(string KeyWord, string _fromdate, string _todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize, ref int total, ref long totalMoney)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS RowNumber,");
        query.AppendLine("a.[CardNumber], a.[Plate], a.[DatetimeIn], a.[DatetimeOut], a.[CardGroupID], a.[CustomerName], a.[LaneIDIn], a.[LaneIDOut], a.[UserIDIn], a.[UserIDOut], a.[Moneys]");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 1");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 1");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys]");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 1");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }

        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL
        query.AppendLine("SELECT COUNT(*) totalCount, SUM(Moneys) AS totalMoney FROM (");
        query.AppendLine("SELECT e.Id,e.Moneys");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 1");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT  eh.Id,eh.Moneys");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 1");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            query.AppendLine("SELECT  le.Id,le.Moneys");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 1");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }

        query.AppendLine(") as a");


        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total, ref totalMoney);
    }

    public static DataTable GetReportVehicleFreeDetailExcel(string KeyWord, string _fromdate, string _todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS STT,");
        query.AppendLine("a.[CardNumber] AS 'Mã thẻ', a.[Plate] AS 'Biển số', (select convert(varchar(10), a.[DatetimeIn], 103) + ' ' + left(convert(varchar(32), a.[DatetimeIn], 108), 8)) AS 'Thời gian vào', (select convert(varchar(10), a.[DatetimeOut], 103) + ' ' + left(convert(varchar(32), a.[DatetimeOut], 108), 8)) AS 'Thời gian ra', a.[CardGroupID] AS 'Nhóm thẻ', a.[CustomerName] AS 'Khách hàng', a.[LaneIDIn] AS 'Làn vào', a.[LaneIDOut] AS 'Làn ra', a.[UserIDIn] AS 'Giám sát vào', a.[UserIDOut] AS 'Giám sát ra', a.[Moneys] AS 'Tiền'");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.[IsFree] = 1");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");

            //tblCardEventHistory
            query.AppendLine("SELECT eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys]");

            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");

            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.[IsFree] = 1");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }

        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");

            //tblLoopEvent
            query.AppendLine("SELECT '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys]");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.[IsFree] = 1");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }

        query.AppendLine(") as a");
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdbevent.FillData(query.ToString());
    }

    public static DataTable GetReportVehicleFreeDetail2(string KeyWord, string _fromdate, string _todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize, ref int total, ref long totalMoney,ref long totalfreemoneys)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS RowNumber,");
        query.AppendLine("a.[CardNumber], a.[Plate], a.[DatetimeIn], a.[DatetimeOut], a.[CardGroupID], a.[CustomerName], a.[LaneIDIn], a.[LaneIDOut], a.[UserIDIn], a.[UserIDOut], CONVERT(varchar,a.[Moneys]) as Moneys, CONVERT(varchar,a.[FreeType]) as FreeType");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], e.[FreeType]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.FreeType<>''");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            //tblCardEventHistory
            query.AppendLine("SELECT eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], eh.[FreeType]");

            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");

            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.FreeType <>''");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            //tblLoopEvent
            query.AppendLine("SELECT '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], le.FreeType");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.FreeType<>''");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        query.AppendLine(") as C1");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL
        query.AppendLine("SELECT COUNT(*) totalCount, SUM(Moneys) AS totalMoney, SUM(FreeType) as totalfreemoneys FROM (");
        query.AppendLine("SELECT e.Id,e.Moneys, CONVERT(INT,e.FreeType) as FreeType");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.FreeType<>''");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            //tblCardEventHistory
            query.AppendLine("SELECT  eh.Id,eh.Moneys, CONVERT(INT,eh.FreeType) as FreeType");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.FreeType<>''");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '%{0}%')", KeyWord));
        }
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            //tblLoopEvent
            query.AppendLine("SELECT  le.Id,le.Moneys, CONVERT(INT,le.FreeType) as FreeType");
            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");
            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.FreeType<>''");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillDataPaging_3(query.ToString(), ref total, ref totalMoney, ref totalfreemoneys);
    }

    public static DataTable GetReportVehicleFreeDetail2Excel(string KeyWord, string _fromdate, string _todate, string CardGroupID, string LaneID, string UserID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();

        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("select ROW_NUMBER() OVER(ORDER BY a.[DatetimeOut] desc) AS STT,");
        query.AppendLine("a.[CardNumber] AS 'Mã thẻ', a.[Plate] AS 'Biển số', (select convert(varchar(10), a.[DatetimeIn], 103) + ' ' + left(convert(varchar(32), a.[DatetimeIn], 108), 8)) AS 'Thời gian vào', (select convert(varchar(10), a.[DatetimeOut], 103) + ' ' + left(convert(varchar(32), a.[DatetimeOut], 108), 8)) AS 'Thời gian ra', a.[CardGroupID] AS 'Nhóm thẻ', a.[CustomerName] AS 'Khách hàng', a.[LaneIDIn] AS 'Làn vào', a.[LaneIDOut] AS 'Làn ra', a.[UserIDIn] AS 'Giám sát vào', a.[UserIDOut] AS 'Giám sát ra', a.[Moneys] AS 'Tiền', a.[FreeType] AS 'Tiền miễn phí'");
        query.AppendLine("FROM(");
        //tblCardEvent
        query.AppendLine("SELECT e.[CardNumber], CAST( CASE WHEN e.[PlateIn] <> '' THEN e.[PlateIn] ELSE e.[PlateOut] END AS nvarchar(50)) as Plate, e.[DatetimeIn], e.[DatetimeOut], e.[CardGroupID], e.[CustomerName], e.[LaneIDIn], e.[LaneIDOut], e.[UserIDIn], e.[UserIDOut], e.[Moneys], e.[FreeType]");
        query.AppendLine("FROM dbo.[tblCardEvent] e WITH (NOLOCK)");
        query.AppendLine("WHERE e.[IsDelete] = 0 and e.[EventCode] = '2' AND e.FreeType<>''");
        query.AppendLine(string.Format("AND e.[DatetimeOut] >= '{0}' AND e.[DatetimeOut] <= '{1}' ", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND e.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND e.[LaneIDOut] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND e.[UserIDOut] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (e.[CardNumber] LIKE '%{0}%' OR e.[CardNo] LIKE '%{0}%' OR e.[PlateIn] LIKE '%{0}%' OR e.[PlateOut] LIKE '%{0}%')", KeyWord));

        DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        {
            query.AppendLine("UNION");
            //tblCardEventHistory
            query.AppendLine("SELECT eh.[CardNumber], CAST( CASE WHEN eh.[PlateIn] <> '' THEN eh.[PlateIn] ELSE eh.[PlateOut] END AS nvarchar(50)) as Plate, eh.[DatetimeIn], eh.[DatetimeOut], eh.[CardGroupID], eh.[CustomerName], eh.[LaneIDIn], eh.[LaneIDOut], eh.[UserIDIn], eh.[UserIDOut], eh.[Moneys], eh.[FreeType]");
            query.AppendLine("FROM dbo.[tblCardEventHistory] eh WITH (NOLOCK)");
            query.AppendLine("WHERE eh.[IsDelete] = 0 and eh.[EventCode] = '2' AND eh.FreeType<>''");
            query.AppendLine(string.Format("AND eh.[DatetimeOut] >= '{0}' AND eh.[DatetimeOut] <= '{1}' ", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND eh.[CardGroupID] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND eh.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND eh.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND (eh.[CardNumber] LIKE '%{0}%' OR eh.[CardNo] LIKE '%{0}%' OR eh.[PlateIn] LIKE '%{0}%' OR eh.[PlateOut] LIKE '{0}')", KeyWord));
        }
        var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        {
            query.AppendLine("UNION");
            //tblLoopEvent
            query.AppendLine("SELECT '' AS CardNumber, le.[Plate], le.[DatetimeIn], le.[DatetimeOut], le.[CarType], le.[CustomerName], le.[LaneIDIn], le.[LaneIDOut], le.[UserIDIn], le.[UserIDOut], le.[Moneys], '0' AS FreeType");

            query.AppendLine("FROM dbo.[tblLoopEvent] le WITH (NOLOCK)");

            query.AppendLine("WHERE le.[IsDelete] = 0 and le.[EventCode] = '2' AND le.FreeType<>''");
            query.AppendLine(string.Format("AND le.[DatetimeOut] >= '{0}' AND le.[DatetimeOut] <= '{1}'", _fromdate, _todate));
            if (!string.IsNullOrWhiteSpace(CardGroupID))
                query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
            if (!string.IsNullOrWhiteSpace(LaneID))
                query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneID));
            if (!string.IsNullOrWhiteSpace(UserID))
                query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserID));
            if (!string.IsNullOrWhiteSpace(KeyWord))
                query.AppendLine(string.Format("AND le.[Plate] LIKE '%{0}%'", KeyWord));
        }
        query.AppendLine(") as a");
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdbevent.FillData(query.ToString());
    }

    public static DataTable GetReportCardExpired(string KeyWord, string _fromdate, string _todate, string CardGroupID, string CustomerID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        //tblCard
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY c.[ExpireDate] desc) AS RowNumber, c.[CardNo], c.[CardNumber],'' as Plate, c.[Plate1], c.[Plate2], c.[Plate3], c.[ExpireDate], c.[CardGroupID], cg.CardGroupName, c.[CustomerID],cu.CustomerName");
        query.AppendLine("FROM dbo.[tblCard] c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCardGroup cg ON c.CardGroupID = CONVERT(nvarchar(255), cg.CardGroupID)");
        query.AppendLine("LEFT JOIN tblCustomer cu ON c.CustomerID = CONVERT(nvarchar(255), cu.CustomerID)");

        query.AppendLine("WHERE c.[IsDelete] = 0");
        query.AppendLine(string.Format("AND c.[ExpireDate] >= '{0}' AND c.[ExpireDate] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND c.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND c.[CustomerID] = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (c.[CardNumber] LIKE '%{0}%' OR c.[CardNo] LIKE '%{0}%' OR c.[Plate1] LIKE '%{0}%' OR c.[Plate2] LIKE '%{0}%' OR c.[Plate3] LIKE '%{0}%')", KeyWord));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        query.AppendLine("SELECT COUNT( c.CardID) totalCount");
        query.AppendLine("FROM dbo.[tblCard] c WITH (NOLOCK)");
        query.AppendLine("WHERE c.[IsDelete] = 0");
        query.AppendLine(string.Format("AND c.[ExpireDate] >= '{0}' AND c.[ExpireDate] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND c.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND c.[CustomerID] = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (c.[CardNumber] LIKE '%{0}%' OR c.[CardNo] LIKE '%{0}%' OR c.[Plate1] LIKE '%{0}%' OR c.[Plate2] LIKE '%{0}%' OR c.[Plate3] LIKE '%{0}%')", KeyWord));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);
    }
    public static DataTable GetReportCardExpiredExcel(string KeyWord, string _fromdate, string _todate, string CardGroupID, string CustomerID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();

        //tblCard
        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY c.[ExpireDate] desc) AS STT, c.[CardNo] as 'Số thẻ', c.[CardNumber] as 'Mã thẻ', c.[Plate1] as 'Biển số 1', c.[Plate2] as 'Biển số 2', c.[Plate3] as 'Biển số 3', (SELECT CONVERT(varchar(19), c.[ExpireDate], 103)) as 'Ngày hết hạn', cg.CardGroupName as 'Nhóm thẻ',cu.CustomerName as 'Tên KH'");
        query.AppendLine("FROM dbo.[tblCard] c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCardGroup cg ON c.CardGroupID = CONVERT(nvarchar(255), cg.CardGroupID)");
        query.AppendLine("LEFT JOIN tblCustomer cu ON c.CustomerID = CONVERT(nvarchar(255), cu.CustomerID)");

        query.AppendLine("WHERE c.[IsDelete] = 0");
        query.AppendLine(string.Format("AND c.[ExpireDate] >= '{0}' AND c.[ExpireDate] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND c.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND c.[CustomerID] = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (c.[CardNumber] LIKE '%{0}%' OR c.[CardNo] LIKE '%{0}%' OR c.[Plate1] LIKE '%{0}%' OR c.[Plate2] LIKE '%{0}%' OR c.[Plate3] LIKE '%{0}%')", KeyWord));
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));
        return StaticPool.mdb.FillData(query.ToString());
    }
    public static DataTable GetReportCardProcessDetail(string KeyWord, string customerGroupId, string _fromdate, string _todate, string CardGroupID, string Actions, string UserID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        //tblCardProcess
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY [Date] desc) AS RowNumber,cu.CustomerName,cu.[Address],cu.[CustomerGroupID],c.*");
        query.AppendLine("FROM dbo.[tblCardProcess] c WITH (NOLOCK)");
        //query.AppendLine("LEFT JOIN tblCardGroup cg ON c.CardGroupID = CONVERT(nvarchar(255), cg.CardGroupID)");
        //query.AppendLine("LEFT JOIN tblUser u on c.UserID = CONVERT(nvarchar(255), u.UserID)");
        //query.AppendLine("LEFT JOIN tblCard ca ON c.CardNumber = ca.CardNumber AND ca.IsDelete = 0");
        query.AppendLine("LEFT JOIN tblCustomer cu ON c.CustomerID = CONVERT(nvarchar(255),cu.CustomerID) --AND (c.Actions='RELEASE' OR c.Actions='RETURN')");
        query.AppendLine("WHERE 1 = 1");
        query.AppendLine(string.Format("AND c.[Date] >= '{0}' AND c.[Date] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND c.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(Actions))
            query.AppendLine(string.Format("AND c.[Actions] = '{0}'", Actions));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND c.[UserID] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(customerGroupId))
            query.AppendLine(string.Format("AND cu.[CustomerGroupID] = '{0}'", customerGroupId));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND c.[CardNumber] LIKE '%{0}%'", KeyWord));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL
        query.AppendLine("SELECT COUNT(Id) totalCount");
        query.AppendLine("FROM dbo.[tblCardProcess] c WITH (NOLOCK)");
        //query.AppendLine("LEFT JOIN tblCardGroup cg ON c.CardGroupID = CONVERT(nvarchar(255), cg.CardGroupID)");
        //query.AppendLine("LEFT JOIN tblUser u on c.UserID = CONVERT(nvarchar(255), u.UserID)");
        //query.AppendLine("LEFT JOIN tblCard ca ON c.CardNumber = ca.CardNumber AND ca.IsDelete = 0");
        query.AppendLine("LEFT JOIN tblCustomer cu ON c.CustomerID = CONVERT(nvarchar(255),cu.CustomerID) --AND (c.Actions='RELEASE' OR c.Actions='RETURN')");
        query.AppendLine("WHERE 1 = 1");
        query.AppendLine(string.Format("AND c.[Date] >= '{0}' AND c.[Date] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND c.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(Actions))
            query.AppendLine(string.Format("AND c.[Actions] = '{0}'", Actions));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND c.[UserID] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(customerGroupId))
            query.AppendLine(string.Format("AND cu.[CustomerGroupID] = '{0}'", customerGroupId));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND c.[CardNumber] LIKE '%{0}%'", KeyWord));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);
    }
    public static DataTable GetReportCardProcessDetailExcel(string KeyWord, string customerGroupId, string _fromdate, string _todate, string CardGroupID, string Actions, string UserID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();

        //tblCardProcess
        //query.AppendLine("SELECT * FROM(");

        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY [Date] desc) AS STT, (select convert(varchar(10), c.[Date], 103) + ' ' + left(convert(varchar(32), c.[Date], 108), 8)) as 'Thời gian', ca.CardNo, c.CardNumber as 'Mã thẻ', cg.CardGroupName as 'Nhóm thẻ', c.Actions as 'Hành vi',cu.CustomerName as 'Chủ thẻ', '' as 'Nhóm KH',cu.[Address] as 'Địa chỉ' ,  CAST( CASE WHEN ca.[Plate2] <> '' THEN ca.[Plate1]+'_'+ca.Plate2 WHEN ca.Plate3 <> '' THEN ca.[Plate1]+'_'+ca.Plate2+'_'+ca.Plate3 ELSE ca.[Plate1] END AS nvarchar(50)) as 'Biển số', u.UserName as 'NV thực hiện',cu.[CustomerGroupID]");
        query.AppendLine("FROM dbo.[tblCardProcess] c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCardGroup cg ON c.CardGroupID = CONVERT(nvarchar(255), cg.CardGroupID)");
        query.AppendLine("LEFT JOIN tblUser u on c.UserID = CONVERT(nvarchar(255), u.UserID)");
        query.AppendLine("LEFT JOIN tblCard ca ON c.CardNumber = ca.CardNumber AND ca.IsDelete = 0");
        query.AppendLine("LEFT JOIN tblCustomer cu ON c.CustomerID = CONVERT(nvarchar(255),cu.CustomerID) AND (c.Actions='RELEASE' OR c.Actions='RETURN')");
        query.AppendLine("WHERE 1 = 1");
        query.AppendLine(string.Format("AND c.[Date] >= '{0}' AND c.[Date] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND c.[CardGroupID] = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(Actions))
            query.AppendLine(string.Format("AND c.[Actions] = '{0}'", Actions));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND c.[UserID] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND c.[CardNumber] LIKE '%{0}%'", KeyWord));
        if (!string.IsNullOrWhiteSpace(customerGroupId))
            query.AppendLine(string.Format("AND cu.[CustomerGroupID] = '{0}'", customerGroupId));
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdb.FillData(query.ToString());
    }
    public static DataTable GetReportAlarm(string KeyWord, string _fromdate, string _todate, string LaneID, string UserID, string AlarmCode, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        query.AppendLine("select DISTINCT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[Date] desc) AS RowNumber, a.* , a.PicDir AS PicIn1, REPLACE(REPLACE(a.PicDir,'PLATEIN.JPG','OVERVIEWIN.JPG'),'PLATEOUT.JPG','OVERVIEWOUT.JPG') AS PicIn2");
        query.AppendLine("FROM dbo.[tblAlarm] a WITH (NOLOCK)");
        query.AppendLine("WHERE 1 = 1");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(AlarmCode))
            query.AppendLine(string.Format("AND a.[AlarmCode] = '{0}'", AlarmCode));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND a.[LaneID] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND a.[UserID] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.[CardNumber] LIKE '%{0}%' OR a.[Plate] LIKE '%{0}%')", KeyWord));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //COUNT TOTAL
        query.AppendLine("SELECT COUNT(Id) totalCount");
        query.AppendLine("FROM dbo.[tblAlarm] a WITH (NOLOCK)");
        query.AppendLine("WHERE 1 = 1");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(AlarmCode))
            query.AppendLine(string.Format("AND a.[AlarmCode] = '{0}'", AlarmCode));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND a.[LaneID] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND a.[UserID] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.[CardNumber] LIKE '%{0}%' OR a.[Plate] LIKE '%{0}%')", KeyWord));

        return StaticPool.mdbevent.FillDataPaging_2(query.ToString(), ref total);
    }
    public static DataTable GetReportAlarmExcel(string KeyWord, string _fromdate, string _todate, string LaneID, string UserID, string AlarmCode, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //tblAlarm
        //query.AppendLine("select * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[Date] desc) AS STT, (select convert(varchar(10), a.[Date], 103) + ' ' + left(convert(varchar(32), a.[Date], 108), 8)) as 'Thời gian',a.[CardNumber] as 'Mã thẻ', a.[Plate] as 'Biển số', a.[AlarmCode] as 'Cảnh báo', a.Description as 'Diễn giải' ,a.[LaneID] as 'Làn' ,a.[UserID] as 'Người dùng'");
        query.AppendLine("FROM dbo.[tblAlarm] a WITH (NOLOCK)");
        query.AppendLine("WHERE 1 = 1");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));

        if (!string.IsNullOrWhiteSpace(AlarmCode))
            query.AppendLine(string.Format("AND a.[AlarmCode] = '{0}'", AlarmCode));
        if (!string.IsNullOrWhiteSpace(LaneID))
            query.AppendLine(string.Format("AND a.[LaneID] = '{0}'", LaneID));
        if (!string.IsNullOrWhiteSpace(UserID))
            query.AppendLine(string.Format("AND a.[UserID] = '{0}'", UserID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.[CardNumber] LIKE '%{0}%' OR a.[Plate] LIKE '%{0}%')", KeyWord));
        //query.AppendLine(") as a");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));


        return StaticPool.mdbevent.FillData(query.ToString());
    }
    public static DataTable GetReportTotalMoneyByCardGroup(string CardGroupID, string _fromdate, string _todate)
    {
        var query = new StringBuilder();
        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[CardGroupID] = '{0}'", CardGroupID));
        return StaticPool.mdbevent.FillData(query.ToString());
    }
    public static DataTable GetReportTotalMoneyByCardGroupUnion(string CardGroupID, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        query.AppendLine("select SUM(a.TotalMoney) FROM(");

        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[CardGroupID] = '{0}'", CardGroupID));

        //DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        //if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        //{
        query.AppendLine("UNION");

        //tblCardEventHistory
        query.AppendLine("SELECT SUM(ceh.[Moneys]) AS TotalMoney FROM tblCardEventHistory ceh WITH (NOLOCK) where ceh.[EventCode] = '2' and ceh.[IsDelete] = 0 and ceh.[IsFree] = 0 and ceh.[Moneys] > 0");
        query.AppendLine(string.Format("AND ceh.[DateTimeOut] >= '{0}' AND ceh.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ceh.[CardGroupID] = '{0}'", CardGroupID));
        //}
        //var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        //if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        //{
        //    query.AppendLine("UNION");

        //    //tblLoopEvent
        //    query.AppendLine("SELECT SUM(le.[Moneys]) AS TotalMoney FROM tblLoopEvent le WITH (NOLOCK) where le.[EventCode] = '2' and le.[IsDelete] = 0 and le.[IsFree] = 0 and le.[Moneys] > 0");
        //    query.AppendLine(string.Format("AND le.[DateTimeOut] >= '{0}' AND le.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        //    query.AppendLine(string.Format("AND le.[CarType] = '{0}'", CardGroupID));
        //}
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillData(query.ToString());
    }
    public static DataTable GetReportTotalMoneyByLane(string LaneIDOut, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[LaneIDOut] = '{0}'", LaneIDOut));

        return StaticPool.mdbevent.FillData(query.ToString());
    }
    public static DataTable GetReportTotalMoneyByLaneAndLoop(string LaneIDOut, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        query.AppendLine("select SUM(a.TotalMoney) FROM(");
        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[LaneIDOut] = '{0}'", LaneIDOut));
        query.AppendLine("UNION");
        //tblLoopEvent
        query.AppendLine("SELECT SUM(le.[Moneys]) AS TotalMoney FROM tblLoopEvent le WITH (NOLOCK) where le.[EventCode] = '2' and le.[IsDelete] = 0 and le.[IsFree] = 0 and le.[Moneys] > 0");
        query.AppendLine(string.Format("AND le.[DateTimeOut] >= '{0}' AND le.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneIDOut));
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillData(query.ToString());
    }


    public static DataTable GetReportTotalMoneyByLaneUnion(string LaneIDOut, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        query.AppendLine("select SUM(a.TotalMoney) FROM(");
        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[LaneIDOut] = '{0}'", LaneIDOut));

        //DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        //if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        //{
        query.AppendLine("UNION");

        //tblCardEventHistory
        query.AppendLine("SELECT SUM(ceh.[Moneys]) AS TotalMoney FROM tblCardEventHistory ceh WITH (NOLOCK) where ceh.[EventCode] = '2' and ceh.[IsDelete] = 0 and ceh.[IsFree] = 0 and ceh.[Moneys] > 0");
        query.AppendLine(string.Format("AND ceh.[DateTimeOut] >= '{0}' AND ceh.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ceh.[LaneIDOut] = '{0}'", LaneIDOut));
        //}
        //var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        //if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        //{
        query.AppendLine("UNION");
        //tblLoopEvent
        query.AppendLine("SELECT SUM(le.[Moneys]) AS TotalMoney FROM tblLoopEvent le WITH (NOLOCK) where le.[EventCode] = '2' and le.[IsDelete] = 0 and le.[IsFree] = 0 and le.[Moneys] > 0");
        query.AppendLine(string.Format("AND le.[DateTimeOut] >= '{0}' AND le.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND le.[LaneIDOut] = '{0}'", LaneIDOut));
        //}
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillData(query.ToString());
    }
    public static DataTable GetReportTotalMoneyByUser(string UserIDOut, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[UserIDOut] = '{0}'", UserIDOut));

        return StaticPool.mdbevent.FillData(query.ToString());
    }

    public static DataTable GetReportTotalMoneyByUserAndLoop(string UserIDOut, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        query.AppendLine("select SUM(a.TotalMoney) FROM(");
        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[UserIDOut] = '{0}'", UserIDOut));

        query.AppendLine("UNION");
        //tblLoopEvent
        query.AppendLine("SELECT SUM(le.[Moneys]) AS TotalMoney FROM tblLoopEvent le WITH (NOLOCK) where le.[EventCode] = '2' and le.[IsDelete] = 0 and le.[IsFree] = 0 and le.[Moneys] > 0");
        query.AppendLine(string.Format("AND le.[DateTimeOut] >= '{0}' AND le.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserIDOut));
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillData(query.ToString());
    }
    public static DataTable GetReportTotalMoneyByUserUnion(string UserIDOut, string _fromdate, string _todate)
    {
        var query = new StringBuilder();

        query.AppendLine("select SUM(a.TotalMoney) FROM(");
        //tblCardEvent
        query.AppendLine("SELECT SUM(ce.[Moneys]) AS TotalMoney FROM tblCardEvent ce WITH (NOLOCK) where ce.[EventCode] = '2' and ce.[IsDelete] = 0 and ce.[IsFree] = 0 and ce.[Moneys] > 0");
        query.AppendLine(string.Format("AND ce.[DateTimeOut] >= '{0}' AND ce.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ce.[UserIDOut] = '{0}'", UserIDOut));
        //DataTable isExistedEventHistory = StaticPool.mdbevent.FillData("SELECT COUNT(*) AS RN FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'tblCardEventHistory'");
        //if (Convert.ToInt32(isExistedEventHistory.Rows[0]["RN"]) > 0)
        //{
        query.AppendLine("UNION");
        //tblCardEventHistory
        query.AppendLine("SELECT SUM(ceh.[Moneys]) AS TotalMoney FROM tblCardEventHistory ceh WITH (NOLOCK) where ceh.[EventCode] = '2' and ceh.[IsDelete] = 0 and ceh.[IsFree] = 0 and ceh.[Moneys] > 0");
        query.AppendLine(string.Format("AND ceh.[DateTimeOut] >= '{0}' AND ceh.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND ceh.[UserIDOut] = '{0}'", UserIDOut));
        //}
        //var isLoopEvent = StaticPool.mdbevent.FillData("SELECT COUNT(Id) as TotalRow FROM tblLoopEvent");
        //if (isLoopEvent != null && Convert.ToInt32(isLoopEvent.Rows[0]["TotalRow"]) > 0)
        //{
        query.AppendLine("UNION");
        //tblLoopEvent
        query.AppendLine("SELECT SUM(le.[Moneys]) AS TotalMoney FROM tblLoopEvent le WITH (NOLOCK) where le.[EventCode] = '2' and le.[IsDelete] = 0 and le.[IsFree] = 0 and le.[Moneys] > 0");
        query.AppendLine(string.Format("AND le.[DateTimeOut] >= '{0}' AND le.[DateTimeOut] <= '{1}'", _fromdate, _todate));
        query.AppendLine(string.Format("AND le.[UserIDOut] = '{0}'", UserIDOut));
        //}
        query.AppendLine(") as a");

        return StaticPool.mdbevent.FillData(query.ToString());
    }

    //public static DataTable GetActiveCardList(string KeyWord, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    //{
    //    var query = new StringBuilder();

    //    query.AppendLine("SELECT * FROM(");
    //    query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY a.[Date] desc) AS RowNumber, ID,[Date],[CardNumber],[Plate],[OldExpireDate],[Days],[NewExpireDate],[CardGroupID],[CustomerGroupID],[CustomerID],[UserID],[FeeLevel] ");
    //    query.AppendLine("FROM tblActiveCard  WITH (NOLOCK) WHERE IsDelete=0");
    //    query.AppendLine(string.Format("AND Date >= '{0}' AND Date <= '{1}'", _fromdate, _todate));
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //        query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", KeyWord));
    //    if (!string.IsNullOrWhiteSpace(CardGroupID))
    //        query.AppendLine(string.Format("AND CardGroupID  LIKE '%{0}%'", CardGroupID));
    //    if (!string.IsNullOrWhiteSpace(CustomerID))
    //        query.AppendLine(string.Format("AND CustomerID  LIKE '%{0}%'", CustomerID));
    //    if (!string.IsNullOrWhiteSpace(CustomerGroupID))
    //        query.AppendLine(string.Format("AND CustomerGroupID  LIKE '%{0}%'", CustomerGroupID));

    //    query.AppendLine(") as C1");
    //    query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

    //    query.AppendLine("SELECT COUNT(ID) totalCount");
    //    query.AppendLine("FROM tblActiveCard  WITH (NOLOCK) WHERE IsDelete=0");
    //    query.AppendLine(string.Format("AND Date >= '{0}' AND Date <= '{1}'", _fromdate, _todate));
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //        query.AppendLine(string.Format("AND (Plate LIKE '%{0}%' OR CardNumber LIKE '%{0}%')", KeyWord));
    //    if (!string.IsNullOrWhiteSpace(CardGroupID))
    //        query.AppendLine(string.Format("AND CardGroupID  LIKE '%{0}%'", CardGroupID));
    //    if (!string.IsNullOrWhiteSpace(CustomerID))
    //        query.AppendLine(string.Format("AND CustomerID  LIKE '%{0}%'", CustomerID));
    //    if (!string.IsNullOrWhiteSpace(CustomerGroupID))
    //        query.AppendLine(string.Format("AND CustomerGroupID  LIKE '%{0}%'", CustomerGroupID));

    //    return StaticPool.mdb.FillDataPaging(query.ToString(), ref total);
    //}
    public static DataTable GetActiveCardList_2(string KeyWord, string CaNo, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();

        query.AppendLine("SELECT * FROM(");
        query.AppendLine(string.Format("SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS RowNumber,'' as CardNo, a.ID,[Date],a.[CardNumber],a.[Plate],a.[OldExpireDate],a.[Days],a.[NewExpireDate],a.[CardGroupID],a.[CustomerGroupID],a.[CustomerID],a.[UserID],a.[FeeLevel], '' as CustomerName, '' as CustomerAddress", !string.IsNullOrWhiteSpace(CaNo) ? "c.CardNo " + CaNo : "[Date] desc"));
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        if (!string.IsNullOrWhiteSpace(CaNo))
            query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        //query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount");
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        if (!string.IsNullOrWhiteSpace(CaNo))
            query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        //query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);
    }
    public static DataTable GetActiveCardList_2Excel(string KeyWord, string CaNo, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();

        //query.AppendLine("SELECT * FROM(");

        query.AppendLine(string.Format("SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS STT,c.CardNo as 'Số thẻ', (SELECT CONVERT(varchar(19), a.[Date], 103)) as 'Ngày tháng',a.[CardNumber] as 'Mã thẻ',a.[Plate] as 'Biển số',(SELECT CONVERT(varchar(19), a.[OldExpireDate], 103)) as 'Thời hạn cũ',a.[Days],(SELECT CONVERT(varchar(19), a.[NewExpireDate], 103)) as 'Thời hạn mới',a.[CardGroupID] as 'Nhóm thẻ',a.[UserID] as 'Người dùng',a.[FeeLevel] as 'Mức phí' , cu.CustomerName as 'Tên KH',a.[CustomerGroupID] as 'Nhóm KH'", !string.IsNullOrWhiteSpace(CaNo) ? "c.CardNo " + CaNo : "[Date]"));
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));

        //query.AppendLine(") as a");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdb.FillData(query.ToString());
    }

    public static DataTable GetActiveCardList_2Print(string KeyWord, string CaNo, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();

        // max row 500
        query.AppendLine("SELECT * FROM(");

        query.AppendLine(string.Format("SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS STT,c.CardNo as 'Số thẻ', (SELECT CONVERT(varchar(19), a.[Date], 103)) as 'Ngày tháng',a.[CardNumber] as 'Mã thẻ',a.[Plate] as 'Biển số',(SELECT CONVERT(varchar(19), a.[OldExpireDate], 103)) as 'Thời hạn cũ',a.[Days],(SELECT CONVERT(varchar(19), a.[NewExpireDate], 103)) as 'Thời hạn mới',a.[CardGroupID] as 'Nhóm thẻ',a.[UserID] as 'Người dùng',a.[FeeLevel] as 'Mức phí' , cu.CustomerName as 'Tên KH',a.[CustomerGroupID] as 'Nhóm KH'", !string.IsNullOrWhiteSpace(CaNo) ? "c.CardNo " + CaNo : "[Date]"));
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));
        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount");
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        if (!string.IsNullOrWhiteSpace(CaNo))
            query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        //query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));


        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);
    }

    public static DataTable GetActiveCardList_2PrintCheckRow(string KeyWord, string CaNo, string _fromdate, string _todate, string CardGroupID, string CustomerID, string CustomerGroupID, int bRow, int eRow, ref int total)
    {
        var query = new StringBuilder();

        // max row 500
        query.AppendLine("SELECT * FROM(");

        query.AppendLine(string.Format("SELECT ROW_NUMBER() OVER(ORDER BY {0}) AS STT,c.CardNo as 'Số thẻ', (SELECT CONVERT(varchar(19), a.[Date], 103)) as 'Ngày tháng',a.[CardNumber] as 'Mã thẻ',a.[Plate] as 'Biển số',(SELECT CONVERT(varchar(19), a.[OldExpireDate], 103)) as 'Thời hạn cũ',a.[Days],(SELECT CONVERT(varchar(19), a.[NewExpireDate], 103)) as 'Thời hạn mới',a.[CardGroupID] as 'Nhóm thẻ',a.[UserID] as 'Người dùng',a.[FeeLevel] as 'Mức phí' , cu.CustomerName as 'Tên KH',a.[CustomerGroupID] as 'Nhóm KH'", !string.IsNullOrWhiteSpace(CaNo) ? "c.CardNo " + CaNo : "[Date]"));
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));
        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE STT BETWEEN {0} AND {1}", bRow, eRow));


        //--COUNT TOTAL RECORD
        query.AppendLine("SELECT COUNT(*) totalCount");
        query.AppendLine("FROM tblActiveCard a WITH (NOLOCK)");
        //query.AppendLine("LEFT JOIN tblCard c ON a.CardNumber = c.CardNumber and c.IsDelete=0");
        //query.AppendLine("LEFT JOIN tblCustomer cu ON a.CustomerID = CONVERT(nvarchar(50),cu.CustomerID)");
        query.AppendLine("WHERE a.IsDelete=0");
        query.AppendLine(string.Format("AND a.[Date] >= '{0}' AND a.[Date] <= '{1}'", _fromdate, _todate));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("AND (a.Plate LIKE '%{0}%' OR a.CardNumber LIKE '%{0}%')", KeyWord));
        if (!string.IsNullOrWhiteSpace(CardGroupID))
            query.AppendLine(string.Format("AND a.CardGroupID  = '{0}'", CardGroupID));
        if (!string.IsNullOrWhiteSpace(CustomerID))
            query.AppendLine(string.Format("AND a.CustomerID  = '{0}'", CustomerID));
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("AND a.CustomerGroupID  = '{0}'", CustomerGroupID));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);
    }


    public static DataTable GetCustomerBylistId(string listId)
    {
        var strList = "";
        var arr = listId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var count = 0;
        foreach (var item in arr)
        {
            count++;
            strList += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
        }
        var query = new StringBuilder();
        query.AppendFormat("SELECT CustomerID, CustomerName, Address FROM tblCustomer WHERE CustomerID IN ({0})", strList);
        return StaticPool.mdb.FillData(query.ToString());

    }
    //public static DataTable GetCustomerList(string KeyWord, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    //{
    //    var query = new StringBuilder();
    //    //query.AppendLine("");
    //    query.AppendLine("SELECT * FROM(");
    //    //query.AppendLine("select");
    //    //query.AppendLine(" e.[CustomerID],e.CustomerGroupName, e.CompartmentName, e.CardNo, e.CardNumber, e.Plate,e.[CustomerCode],e.[CustomerName],e.[Address],e.[IDNumber],e.[Mobile],e.[CustomerGroupID],e.[Description],e.[EnableAccount],e.[Account],e.[Password],e.[Avatar],e.[Inactive],e.[SortOrder],e.[CompartmentID]");
    //    //query.AppendLine("FROM(");
    //    query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY e.SortOrder desc) AS RowNumber, c.[CustomerID],g.CustomerGroupName, p.CompartmentName, '' as CardNo, '' as CardNumber, '' as Plate, c.[CustomerCode], c.[CustomerName], c.[Address], c.[IDNumber], c.[Mobile], c.[CustomerGroupID], c.[Description], c.[EnableAccount], c.[Account], c.[Password], c.[Avatar], c.[Inactive], c.[SortOrder], c.[CompartmentID]");
    //    query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
    //    query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
    //    query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
    //    query.AppendLine("where 1 = 1");

    //    if (!string.IsNullOrWhiteSpace(CustomerGroupID))
    //        query.AppendLine(string.Format("and c.CustomerGroupID LIKE N'%{0}%'", CustomerGroupID));
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //        query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

    //    query.AppendLine(") as e");
    //    query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

    //    //--COUNT- TOLTAL
    //    query.AppendLine("SELECT COUNT(CustomerID) totalCount");
    //    query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
    //    query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
    //    query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
    //    query.AppendLine("where 1 = 1");

    //    if (!string.IsNullOrWhiteSpace(CustomerGroupID))
    //        query.AppendLine(string.Format("and c.CustomerGroupID LIKE N'%{0}%'", CustomerGroupID));
    //    if (!string.IsNullOrWhiteSpace(KeyWord))
    //        query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

    //    query.AppendLine(") as e");
    //    return StaticPool.mdb.FillDataPaging(query.ToString(), ref total);

    //}
    public static DataTable GetCustomerList_2(string KeyWord, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        //query.AppendLine("");

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY c.SortOrder desc) AS RowNumber, c.[CustomerID],g.CustomerGroupName, p.CompartmentName, '' as CardNo, '' as CardNumber, '' as Plate, c.[CustomerCode], c.[CustomerName], c.[Address], c.[IDNumber], c.[Mobile], c.[CustomerGroupID], c.[Description], c.[EnableAccount], c.[Account], c.[Password], c.[Avatar], c.[Inactive], c.[SortOrder], c.[CompartmentID]");
        query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
        query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
        query.AppendLine("where 1 = 1");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = N'{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //COUNT TOTALRECORD
        query.AppendLine("SELECT COUNT(*) totalCount");
        query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
        //query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
        //query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
        query.AppendLine("where 1 = 1");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = N'{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);

    }
    public static DataTable GetCustomerListExcel(string KeyWord, string CustomerGroupID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("");
        //query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY c.SortOrder desc) AS STT,  c.[CustomerCode] as 'Mã KH', c.[CustomerName] as 'Tên KH',g.CustomerGroupName as 'Nhóm KH', c.[Address] as 'Địa chỉ', c.[Mobile] as 'Điện thoại', '' as CardNo, '' as 'Mã thẻ', '' as 'Biển số'");
        query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
        //query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
        query.AppendLine("where 1 = 1");

        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = N'{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));
        //query.AppendLine(") as C1");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdb.FillData(query.ToString());

    }
    public static DataTable GetCardByCustomer(string customerIdList)
    {
        var str = "";
        if (!string.IsNullOrWhiteSpace(customerIdList))
        {
            var arr = customerIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var item in arr)
            {
                count++;
                str += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
            }
        }


        var query = new StringBuilder();
        query.AppendLine(string.Format("select CustomerID, CardNo, CardNumber, Plate1, Plate2, Plate3 from tblCard WITH (NOLOCK) where CustomerID IN ({0})", str));

        return StaticPool.mdb.FillData(query.ToString());
    }

    public static DataTable GetCardByCardNumber(string customerIdList)
    {
        var str = "";
        if (!string.IsNullOrWhiteSpace(customerIdList))
        {
            var arr = customerIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var item in arr)
            {
                count++;
                str += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
            }
        }


        var query = new StringBuilder();
        query.AppendLine(string.Format("select CardNo, CardNumber from tblCard  WITH (NOLOCK) where CardNumber IN ({0}) AND IsDelete=0", str));

        return StaticPool.mdb.FillData(query.ToString());
    }

    public static DataTable GetCardByCardNumberForCardProcess(string customerIdList)
    {
        var str = "";
        if (!string.IsNullOrWhiteSpace(customerIdList))
        {
            var arr = customerIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var item in arr)
            {
                count++;
                str += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
            }
        }

        var query = new StringBuilder();
        query.AppendLine(string.Format("select CardNo, CardNumber, CAST( CASE WHEN [Plate2] <> '' THEN [Plate1]+'_'+Plate2 WHEN Plate3 <> '' THEN [Plate1]+'_'+Plate2+'_'+Plate3 ELSE [Plate1] END AS nvarchar(50)) as Plate from tblCard  WITH (NOLOCK) where CardNumber IN ({0}) AND IsDelete=0", str));

        return StaticPool.mdb.FillData(query.ToString());
    }



    public static DataTable GetCardSubByCardNumber(string cardIdList)
    {
        var str = "";
        if (!string.IsNullOrWhiteSpace(cardIdList))
        {
            var arr = cardIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var item in arr)
            {
                count++;
                str += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
            }
        }
        var query = new StringBuilder();
        query.AppendLine(string.Format("SELECT * FROM [dbo].[tblSubCard] WHERE MainCard IN ({0}) AND IsDelete=0", str));

        return StaticPool.mdb.FillData(query.ToString());
    }
    public static DataTable GetCardByCustomerId(string customerIdList)
    {
        var str = "";
        if (!string.IsNullOrWhiteSpace(customerIdList))
        {
            var arr = customerIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var item in arr)
            {
                count++;
                str += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
            }
        }


        var query = new StringBuilder();
        query.AppendLine(string.Format("select CustomerID,CustomerName,CustomerCode,Address,CustomerGroupID from tblCustomer  WITH (NOLOCK) where  CONVERT(nvarchar(255), CustomerID) IN ({0})", str));

        return StaticPool.mdb.FillData(query.ToString());
    }

    public static DataTable GetCardByCustomerWithGroup(string customerIdList)
    {
        var str = "";
        if (!string.IsNullOrWhiteSpace(customerIdList))
        {
            var arr = customerIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 0;
            foreach (var item in arr)
            {
                count++;
                str += string.Format("'{0}'{1}", item, count != arr.Length ? "," : "");
            }
        }


        var query = new StringBuilder();
        query.AppendLine("select c.CustomerID, c.CardNo, c.CardNumber, c.Plate1, c.Plate2, c.Plate3, ca.CardGroupName,c.IsLock,c.ExpireDate,c.ImportDate from tblCard c  WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCardGroup ca On c.CardGroupID = CONVERT(varchar(255), ca.CardGroupID)");
        query.AppendLine(string.Format("where CustomerID IN ({0})", str));

        return StaticPool.mdb.FillData(query.ToString());
    }
    public static DataTable GetReportCustomerList_m9(string KeyWord, string CustomerGroupID, int pageIndex, int pageSize, ref int total)
    {
        var query = new StringBuilder();
        //query.AppendLine("");

        query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY c.SortOrder desc) AS RowNumber, c.CustomerID, c.[CustomerCode],g.CustomerGroupName, c.[CustomerName], c.[Address], c.[Mobile], '' as CardGroupName, '' as CardNo, '' as CardNumber, '' as Plate, '' as [ExpireDate], '' as ImportDate,'' as IsLock");
        query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
        //query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
        query.AppendLine("where 1 = 1");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = N'{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

        query.AppendLine(") as a");
        query.AppendLine(string.Format("WHERE RowNumber BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        //COUNT TOTALRECORD
        query.AppendLine("SELECT COUNT(*) totalCount");
        query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
        //query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
        //query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
        query.AppendLine("where 1 = 1");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = N'{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

        return StaticPool.mdb.FillDataPaging_2(query.ToString(), ref total);

    }
    public static DataTable GetReportCustomerList_m9Excel(string KeyWord, string CustomerGroupID, int pageIndex, int pageSize)
    {
        var query = new StringBuilder();
        //query.AppendLine("");
        //query.AppendLine("SELECT * FROM(");
        query.AppendLine("SELECT ROW_NUMBER() OVER(ORDER BY c.SortOrder desc) AS STT, c.[CustomerCode] as 'Mã KH',g.CustomerGroupName as 'Nhóm KH', c.[CustomerName] as 'Tên KH', c.[Address] as 'Địa chỉ', c.[Mobile] as 'Điện thoại', '' as 'Nhóm thẻ', '' as CardNo, '' as 'Mã thẻ', '' as 'Biển số', '' as 'Ngày hết hạn', '' as 'Ngày nhập thẻ','' as 'Trạng thái', c.CustomerID");
        query.AppendLine("FROM tblCustomer c WITH (NOLOCK)");
        query.AppendLine("LEFT JOIN tblCustomerGroup g On c.CustomerGroupID = CONVERT(varchar(255),g.CustomerGroupID)");
        //query.AppendLine("LEFT JOIN tblCompartment p On c.CompartmentID = CONVERT(varchar(255),p.CompartmentID)");
        query.AppendLine("where 1 = 1");
        if (!string.IsNullOrWhiteSpace(CustomerGroupID))
            query.AppendLine(string.Format("and c.CustomerGroupID = N'{0}'", CustomerGroupID));
        if (!string.IsNullOrWhiteSpace(KeyWord))
            query.AppendLine(string.Format("and (c.CustomerName LIKE N'%{0}%' or c.CustomerCode LIKE '%{0}%' or c.Mobile LIKE N'%{0}%')", KeyWord));

        //query.AppendLine(") as a");
        //query.AppendLine(string.Format("WHERE STT BETWEEN (({0}-1) * {1} + 1) AND ({0} * {1})", pageIndex, pageSize));

        return StaticPool.mdb.FillData(query.ToString());
    }


    public static DataTable GetCardToSearchComplete(string keyword)
    {
        var query = new StringBuilder();

        query.AppendLine("select TOP 10 c.CardNumber as itemvalue, CAST(CASE WHEN c.CardNo <> '' THEN c.CardNo + '/' + c.CardNumber ELSE c.CardNumber  END AS nvarchar(50)) as itemtext from tblCard c  WITH (NOLOCK)");
        query.AppendLine("inner join tblCardgroup g on c.CardGroupID = CONVERT(varchar(255), g.CardGroupID)");
        query.AppendLine("where c.IsDelete = 0");
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query.AppendLine(string.Format("AND (c.CardNumber like '%{0}%' OR c.CardNo Like '%{0}%')", keyword));
        }
        query.AppendLine("order by c.SortOrder");
        query.AppendLine("");

        return StaticPool.mdb.FillData(query.ToString());

    }




}

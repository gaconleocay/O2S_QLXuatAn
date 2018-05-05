using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


public partial class QLXuatAn_PrintReceiptNOIBAI : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
               
                string id = "";
                if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
                    id = Request.QueryString["Id"].ToString();

                if (Session["Para1"] == null || Session["Para2"] == null)
                {
                    DataTable temp = StaticPool.mdb.FillData("select * from tblSystemConfig");
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        Session["Para1"] = temp.Rows[0]["Para1"].ToString();
                        Session["Para2"] = temp.Rows[0]["Para2"].ToString();
                    }
                }

                DataTable dtevent = StaticPool.mdbevent.FillData("select * from tblCardEvent where Id='" + id + "'");
                if (dtevent != null && dtevent.Rows.Count > 0)
                {
                    DataRowView drv = dtevent.DefaultView[0];
                    DateTime _dtimein = DateTime.Parse(drv["DatetimeIn"].ToString());
                    DateTime _dtimeout = DateTime.Parse(drv["DatetimeOut"].ToString());
                    string _cardgroupname = "";
                    string _cardgrouptype = "";

                    DataTable temp = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID='" + drv["CardGroupID"].ToString() + "'");
                    if (temp != null && temp.Rows.Count > 0)
                    {
                        _cardgroupname = temp.Rows[0]["CardGroupName"].ToString();
                        if (temp.Rows[0]["CardType"].ToString() == "0")
                            _cardgrouptype = "Vé tháng";
                        else if (temp.Rows[0]["CardType"].ToString() == "1")
                            _cardgrouptype = "Vé lượt";
                        else if (temp.Rows[0]["CardType"].ToString() == "1")
                            _cardgrouptype = "VIP";
                    }

                    string _para1 = "";
                    string _para2 = "";
                    string _card = GetPrintIndex(id, ref _para1, ref _para2).ToString("0000000");//drv["CardNumber"].ToString();
                    string _plate = (drv["PlateIn"].ToString() == "" ? drv["PlateOut"].ToString() : drv["PlateIn"].ToString());
                    string _parkingtime = StaticPool.GetPeriodTime(_dtimein, _dtimeout);
                    string _money = string.Format(new System.Globalization.CultureInfo("en-US"),"{0:0,0}", int.Parse(drv["Moneys"].ToString()));

                    CardGroupName.InnerText = _cardgroupname;
                    CardGroupType.InnerText = _cardgrouptype;
                    Para.InnerText = "Mẫu số: " + _para1 + "&nbsp; ; &nbsp; &nbsp; &nbsp; &nbsp;KH: " + _para2;
                    Para.InnerText = Server.HtmlDecode(Para.InnerText);
                    DateIn.InnerText = _dtimein.ToString("dd/MM/yyyy");
                    DateOut.InnerText = _dtimeout.ToString("dd/MM/yyyy");
                    TimeIn.InnerText = _dtimein.ToString("HH:mm:ss");
                    TimeOut.InnerText = _dtimeout.ToString("HH:mm:ss");
                    Card.InnerText = "Số: " + _card;
                    Plate.InnerText = _plate;
                    ParkingTime.InnerText = StaticPool.GetPeriodTime(_dtimein, _dtimeout);
                    Moneys.InnerText = "GIÁ TIỀN: " + _money + " đ";
                }
            }
            catch
            { }
        }


    }

    private int GetPrintIndex(string id, ref string para1, ref string para2)
    {
        try
        {
            DataTable temp = StaticPool.mdbevent.FillData("select * from tblPrintIndex where EventID='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                para1 = temp.Rows[0]["Para1"].ToString();
                para2 = temp.Rows[0]["Para2"].ToString();
                return int.Parse(temp.Rows[0]["PrintIndex"].ToString());
            }
            else
            {
                para1 = Session["Para1"].ToString();
                para2 = Session["Para2"].ToString();

                DataTable dt = StaticPool.mdbevent.FillData("select top 1 * from tblPrintIndex where" +
                       " Para1=N'" + para1 +
                       "' and Para2=N'" + para2 +
                    //" Date>='" + _firstofyear +
                    //"' and Date<='" + _lastofyear +
                       "' order by ID desc");
                if (dt != null && dt.Rows.Count == 0)
                {
                    int _newindex = 1;
                    StaticPool.mdbevent.ExecuteCommand("insert into tblPrintIndex(Date, EventID, PrintIndex, Para1, Para2) values('" +
                        DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                        id + "', '" +
                        _newindex + "', N'" +
                        para1 + "', N'" +
                        para2 +

                        "')");
                    return _newindex;
                }
                else if (dt != null && dt.Rows.Count > 0)
                {
                    int _newindex = int.Parse(dt.Rows[0]["PrintIndex"].ToString()) + 1;
                    StaticPool.mdbevent.ExecuteCommand("insert into tblPrintIndex(Date, EventID, PrintIndex, Para1, Para2) values('" +
                        DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                        id + "', '" +
                        _newindex + "', N'" +
                        para1 + "', N'" +
                        para2 +
                        "')");
                    return _newindex;
                }

                //string year = DateTime.Now.Year.ToString("0000");
                //string _firstofyear = DateTime.Now.ToString("yyyy/01/01");
                //string _lastofyear = DateTime.Now.ToString("yyyy/12/31 23:59:59");

                //DataTable dt = StaticPool.mdbevent.FillData("select top 1 * from tblPrintIndex where" +
                //    " Date>='" + _firstofyear +
                //    "' and Date<='" + _lastofyear +
                //    "' order by ID desc");
                //if (dt != null && dt.Rows.Count == 0)
                //{
                //    int _newindex = 1;
                //    StaticPool.mdbevent.ExecuteCommand("insert into tblPrintIndex(Date, EventID, PrintIndex) values('" +
                //        DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                //        id + "', '" +
                //        _newindex +
                //        "')");
                //    return _newindex;
                //}
                //else if (dt != null && dt.Rows.Count > 0)
                //{
                //    int _newindex = int.Parse(dt.Rows[0]["PrintIndex"].ToString()) + 1;
                //    StaticPool.mdbevent.ExecuteCommand("insert into tblPrintIndex(Date, EventID, PrintIndex) values('" +
                //        DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                //        id + "', '" +
                //        _newindex +
                //        "')");
                //    return _newindex;
                //}
            }

        }
        catch
        {
        }
        return 0;
    }
}
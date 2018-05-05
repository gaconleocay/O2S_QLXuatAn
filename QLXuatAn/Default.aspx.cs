using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class QLXuatAn_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;

                // Thoi gian bao cao
                ViewState["FromDate"] = DateTime.Now.ToString("yyyy/MM/dd");
                ViewState["ToDate"] = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");
                // Tuy chon thoi gian
                cbFrequencyDateType.Items.Clear();
                for (int i = 0; i < DateUI.FrequencyDateTypeString().Length - 2; i++)
                {
                    cbFrequencyDateType.Items.Add(new ListItem(DateUI.FrequencyDateTypeString()[i], i.ToString()));
                }

                if (Request.QueryString["FrequencyDateType"] != null)
                {
                    cbFrequencyDateType.SelectedIndex = int.Parse(Request.QueryString["FrequencyDateType"].ToString());
                    string[] rangeDate = DateUI.GetRangeOfDate((DateUI.FrequencyDateType)cbFrequencyDateType.SelectedIndex, DateTime.Now, "dd/MM/yyyy");
                    ViewState["FromDate"] = Convert.ToDateTime(rangeDate[0]).ToString("yyyy/MM/dd"); //rangeDate[0].Substring(6, 4) + "/" + rangeDate[0].Substring(3, 2) + "/" + rangeDate[0].Substring(0, 2) + " 00:00:00";
                    ViewState["ToDate"] = Convert.ToDateTime(rangeDate[1]).AddDays(1).ToString("yyyy/MM/dd");//rangeDate[1].Substring(6, 4) + "/" + rangeDate[1].Substring(3, 2) + "/" + rangeDate[1].Substring(0, 2) + " 23:59:59";
                }

                int _vehiclein = 0;
                int _vehicleout = 0;
                DataTable dt = StaticPool.mdbevent.FillData("select count(id) from tblCardEvent  WITH (NOLOCK) where IsDelete=0 and (EventCode='1' or EventCode='2') and" +
                    " DateTimeIn>='" + ViewState["FromDate"].ToString() +
                    "' and DateTimeIn<='" + ViewState["ToDate"].ToString() +
                    "'");

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() != "")
                    {
                        _vehiclein = int.Parse(dt.Rows[0][0].ToString());
                    }
                }
                if (StaticPool.SystemUsingLoop() == true)
                {
                    dt = StaticPool.mdbevent.FillData("select count(id) from tblLoopEvent WITH (NOLOCK) where IsDelete=0 and (EventCode='1' or EventCode='2') and" +
                            " DateTimeIn >='" + ViewState["FromDate"].ToString() +
                            "' and DateTimeIn <='" + ViewState["ToDate"].ToString() +
                            "'");
                    if (dt.Rows[0][0].ToString() != "")
                    {
                        _vehiclein = _vehiclein + int.Parse(dt.Rows[0][0].ToString());
                    }

                }
                // Xe trong bai
                id_dashboard1.InnerText = _vehiclein.ToString();







                dt = StaticPool.mdbevent.FillData("select count(id) from tblCardEvent WITH (NOLOCK) where IsDelete=0 and EventCode='2' and" +
                            " DateTimeIn >='" + ViewState["FromDate"].ToString() +
                            "' and DateTimeIn <='" + ViewState["ToDate"].ToString() + "'" +
                    " and DateTimeOut >='" + ViewState["FromDate"].ToString() +
                    "' and DateTimeOut <='" + ViewState["ToDate"].ToString() +
                    "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() != "")
                    {
                        _vehicleout = int.Parse(dt.Rows[0][0].ToString());
                    }
                }

                if (StaticPool.SystemUsingLoop() == true)
                {
                    dt = StaticPool.mdbevent.FillData("select count(id) from tblLoopEvent WITH (NOLOCK) where IsDelete=0 and EventCode='2' and" +
                            " DateTimeIn >='" + ViewState["FromDate"].ToString() +
                            "' and DateTimeIn <='" + ViewState["ToDate"].ToString() + "'" +
                        " and" +
                                " DateTimeOut >='" + ViewState["FromDate"].ToString() +
                                "' and DateTimeOut <='" + ViewState["ToDate"].ToString() +
                                "'");
                    if (dt.Rows[0][0].ToString() != "")
                    {
                        _vehicleout =_vehicleout+ int.Parse(dt.Rows[0][0].ToString());
                    }
                }
                // Xe ra
                id_dashboard2.InnerText = _vehicleout.ToString();
                // Thu tien ve luot
                //id_dashboard3.InnerText = "0 (đ)";

                //// Luot an nut
                //id_dashboard4.InnerText = "0";

            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }
        }
    }

    public string formatMoney(string money)
    {
        if (double.Parse(money) == 0)
            return "0";
        else
            return String.Format("{0:000,0}", Convert.ToDouble(money));
    }
}
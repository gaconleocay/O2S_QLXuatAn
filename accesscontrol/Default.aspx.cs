using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class accesscontrol_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;

                // Thoi gian bao cao
                ViewState["FromDate"] = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00";
                ViewState["ToDate"] = DateTime.Now.ToString("yyyy/MM/dd") + " 23:59:59";

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
                    ViewState["FromDate"] = rangeDate[0].Substring(6, 4) + "/" + rangeDate[0].Substring(3, 2) + "/" + rangeDate[0].Substring(0, 2) + " 00:00:00";
                    ViewState["ToDate"] = rangeDate[1].Substring(6, 4) + "/" + rangeDate[1].Substring(3, 2) + "/" + rangeDate[1].Substring(0, 2) + " 23:59:59";
                }

                DataTable dt = null;

                id_dashboard1.InnerText = "0";

                id_dashboard2.InnerText = "0";

                id_dashboard3.InnerText = "0";

                id_dashboard4.InnerText = "0";

            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }
        }
    }
}
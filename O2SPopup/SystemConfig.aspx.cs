using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;

public partial class O2SPopup_SystemConfig : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_smartcard.Visible = false;

                // Check xem nguoi dung nay co quyen truy cap chuc nang nay khong
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_System_SystemConfig", "Selects", "Parking"))
                {
                    div_alert.Visible = false;
                    div_info.Visible = false;
                    DataTable dtSystemConfig = StaticPool.mdb.FillData("select * from tblSystemConfig");
                    if (dtSystemConfig != null && dtSystemConfig.Rows.Count > 0)
                    {
                        DataRow dr = dtSystemConfig.Rows[0];
                        ViewState["SystemConfigID"] = dr["SystemConfigID"].ToString();
                        txtCompany.Text = dr["Company"].ToString();
                        txtTel.Text = dr["Tel"].ToString();
                        txtFax.Text = dr["Fax"].ToString();
                        txtAddress.Text = dr["Address"].ToString();
                        txtSystemCode.Attributes.Add("value", dr["SystemCode"].ToString());
                        txtKeyA.Attributes.Add("value", dr["KeyA"].ToString());
                        txtKeyB.Attributes.Add("value", dr["KeyB"].ToString());

                    }
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Message.aspx?Message=" + ex.Message);
            }
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            if (ViewState["SystemConfigID"] != null && ViewState["SystemConfigID"].ToString() != "")
            {
                if (Request.Cookies["UserID"] != null && StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_System_SystemConfig", "Updates", "Parking"))
                {
                    // Update
                    StaticPool.mdb.ExecuteCommand("update tblSystemConfig set Company = N'" + txtCompany.Text +
                        "', Tel = '" + txtTel.Text +
                        "', Fax = '" + txtFax.Text +
                        "', Address = N'" + txtAddress.Text +
                        "', SystemCode = '" + txtSystemCode.Text +
                        "', KeyA = '" + txtKeyA.Text +
                        "', KeyB = '" + txtKeyB.Text +
                        "' where SystemConfigID = '" + ViewState["SystemConfigID"].ToString() + "'", ref result);
                    txtSystemCode.Attributes.Add("value", txtSystemCode.Text);
                    txtKeyA.Attributes.Add("value", txtKeyA.Text);
                    txtKeyB.Attributes.Add("value", txtKeyB.Text);
                }
                else
                {
                    // Hien thi thong bao
                    div_alert.Visible = true;
                    id_alert.InnerText = "Bạn không có quyền thực hiện chức năng này!";
                    return;
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_info.Visible = false;
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
            {
                div_info.Visible = true;
                div_alert.Visible = false;
                id_info.InnerText = "Lưu thành công!";
            }
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
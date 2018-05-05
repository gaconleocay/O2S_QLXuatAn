using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;

public partial class QLXuatAn_SystemConfig : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_smartcard.Visible = false;

                ViewState["OldSystemConfig"] = "";

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
                        txtFeeName.Text = dr["FeeName"].ToString();//bool.Parse(dr["EnableDeleteCardFailed"].ToString());
                        chEnableDeleteCardFailed.Checked = bool.Parse(dr["EnableDeleteCardFailed"].ToString());
                        chEnableSoundAlarm.Checked = bool.Parse(dr["EnableSoundAlarm"].ToString());
                        chEnableAlarmMessageBox.Checked = bool.Parse(dr["EnableAlarmMessageBox"].ToString());
                        chEnableAlarmMessageBoxIn.Checked = bool.Parse(dr["EnableAlarmMessageBoxIn"].ToString());
                        txtTax.Text = dr["Tax"].ToString();
                        txtDelayTime.Text = dr["DelayTime"].ToString();
                        //txtSystemCode.Attributes.Add("value", dr["SystemCode"].ToString());
                        //txtKeyA.Attributes.Add("value", dr["KeyA"].ToString());
                        //txtKeyB.Attributes.Add("value", dr["KeyB"].ToString());

                        txtPara1.Text = dr["Para1"].ToString();
                        txtPara2.Text = dr["Para2"].ToString();

                        ViewState["OldSystemConfig"] = txtCompany.Text +
                            ";" + txtTel.Text +
                            ";" + txtFax.Text +
                            ";" + txtAddress.Text +
                            ";" + txtFeeName.Text +
                            ";" + chEnableDeleteCardFailed.Checked.ToString() +
                            ";" + chEnableSoundAlarm.Checked.ToString() +
                            ";" + chEnableAlarmMessageBox.Checked.ToString() +
                            ";" + chEnableAlarmMessageBoxIn.Checked.ToString() +
                            ";" + txtTax.Text;

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
                    if (StaticPool.mdb.ExecuteCommand("update tblSystemConfig set Company = N'" + txtCompany.Text +
                         "', Tel = '" + txtTel.Text +
                         "', Fax = '" + txtFax.Text +
                         "', Address = N'" + txtAddress.Text +
                         "', FeeName='" + txtFeeName.Text +
                         "', EnableDeleteCardFailed='" + chEnableDeleteCardFailed.Checked +
                         "', EnableSoundAlarm='" + chEnableSoundAlarm.Checked +
                         "', Logo='" + txtLogo.Text +
                         "', EnableAlarmMessageBox='" + chEnableAlarmMessageBox.Checked +
                         "', EnableAlarmMessageBoxIn='" + chEnableAlarmMessageBoxIn.Checked +
                         "', Tax='"+txtTax.Text+
                         "', DelayTime='"+txtDelayTime.Text+
                         "', Para1=N'"+txtPara1.Text+
                         "', Para2=N'"+txtPara2.Text+
                        //"', SystemCode = '" + txtSystemCode.Text +
                        //"', KeyA = '" + txtKeyA.Text +
                        //"', KeyB = '" + txtKeyB.Text +
                         "' where SystemConfigID = '" + ViewState["SystemConfigID"].ToString() + "'", ref result))

                    {
                        string _newsystemconfig = txtCompany.Text +
                            ";" + txtTel.Text +
                            ";" + txtFax.Text +
                            ";" + txtAddress.Text +
                            ";" + txtFeeName.Text +
                            ";" + chEnableDeleteCardFailed.Checked.ToString() +
                            ";" + chEnableSoundAlarm.Checked.ToString() +
                            ";" + chEnableAlarmMessageBox.Checked.ToString()+
                            ";" + chEnableAlarmMessageBoxIn.Checked.ToString() +
                            ";" + txtTax.Text;

                        string _des = StaticPool.GetStringChange(ViewState["OldSystemConfig"].ToString(), _newsystemconfig);

                        StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_System_SystemConfig", "SystemParams", "Sửa", _des);
                    }

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
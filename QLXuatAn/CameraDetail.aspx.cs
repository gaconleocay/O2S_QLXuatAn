using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class QLXuatAn_CameraDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                cbPC.DataSource = StaticPool.mdb.FillData("select (ComputerName + ' (' + IPAddress + ')') as ComputerName, PCID from tblPC");
                cbPC.DataValueField = "PCID";
                cbPC.DataTextField = "ComputerName";
                cbPC.DataBind();

                ViewState["OldCamera"] = "";

                if (Request.QueryString["PCID"] != null && Request.QueryString["PCID"].ToString() != "")
                {
                    cbPC.Value = Request.QueryString["PCID"].ToString();
                }

                for (int i = 0; i < SystemUI.GetCameraTypes().Length; i++)
                {
                    cbCameraType.Items.Add(SystemUI.GetCameraTypes()[i]);
                }

                for (int i = 0; i < SystemUI.GetStreamTypes().Length; i++)
                {
                    cbStreamType.Items.Add(SystemUI.GetStreamTypes()[i]);
                }

                for (int i = 0; i < SystemUI.GetSDKs().Length; i++)
                {
                    cbSDK.Items.Add(SystemUI.GetSDKs()[i]);
                }

                div_alert.Visible = false;
                if (Request.QueryString["CameraID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Camera", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_cameradetail.InnerText = "Sửa camera";
                    ViewState["CameraID"] = Request.QueryString["CameraID"].ToString();
                    DataTable dtCamera = StaticPool.mdb.FillData("select * from tblCamera where CameraID = '" + ViewState["CameraID"].ToString() + "'");
                    if (dtCamera != null && dtCamera.Rows.Count > 0)
                    {
                        DataRow dr = dtCamera.Rows[0];
                        txtCameraName.Text = dr["CameraName"].ToString();
                        txtHttpURL.Text = dr["HttpURL"].ToString();
                        txtHttpPort.Text = dr["HttpPort"].ToString();
                        txtRtspPort.Text = dr["RtspPort"].ToString();
                        cbPC.Value = dr["PCID"].ToString();
                        txtUserName.Text = dr["UserName"].ToString();
                        txtPassword.Attributes.Add("value", CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                        txtChannel.Value = dr["Channel"].ToString();
                        txtResolution.Text = dr["Resolution"].ToString();
                        cbCameraType.Value = dr["CameraType"].ToString();
                        cbStreamType.Value = dr["StreamType"].ToString();
                        cbSDK.Value = dr["SDK"].ToString();
                        txtFrameRate.Value = dr["FrameRate"].ToString();
                        chbEnableRecording.Checked = bool.Parse(dr["EnableRecording"].ToString());
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());

                        ViewState["OldCamera"] = txtCameraName.Text +
                            ";" + cbPC.Items[cbPC.SelectedIndex].Text +
                            ";" + txtHttpURL.Text +
                            ";" + txtHttpPort.Text +
                            ";" + txtRtspPort.Text +
                            ";" + txtUserName.Text +
                            //";" + txtPassword.Text +
                            ";" + txtChannel.Value +
                            ";" + cbCameraType.Items[cbCameraType.SelectedIndex].Text +
                            ";" + cbStreamType.Items[cbStreamType.SelectedIndex].Text +
                            ";" + txtFrameRate.Value +
                            ";" + cbSDK.Items[cbSDK.SelectedIndex].Text +
                            ";" + chbEnableRecording.Checked.ToString() +
                            ";" + chbInactive.Checked.ToString();

                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Camera", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_cameradetail.InnerText = "Thêm camera mới";
                    ViewState["CameraID"] = null;
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

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            div_alert.Visible = false;

            if (txtCameraName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên camera không được để trống!";
                return;
            }

            string Cgi = SystemUI.GetCgiByCameraType(cbCameraType.Value, txtFrameRate.Value, txtResolution.Text, cbSDK.Value);

            if (ViewState["CameraID"] != null && ViewState["CameraID"].ToString() != "")
            {
                // Update
                if (StaticPool.mdb.ExecuteCommand("update tblCamera set CameraName = N'" + txtCameraName.Text +
                    "', PCID = '" + cbPC.Value +
                    "', HttpURL = '" + txtHttpURL.Text +
                    "', HttpPort = '" + txtHttpPort.Text +
                    "', RtspPort = '" + txtRtspPort.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    ", EnableRecording = " + (chbEnableRecording.Checked ? 1 : 0) +
                    ", Resolution = '" + txtResolution.Text +
                    "', CameraType = '" + cbCameraType.Value +
                    "', StreamType = '" + cbStreamType.Value +
                    "', SDK = '" + cbSDK.Value +
                    "', UserName = '" + txtUserName.Text +
                    "', Password = '" + CryptorEngine.Encrypt(txtPassword.Text, true) +
                    "', Channel = " + txtChannel.Value +
                    ", FrameRate = '" + txtFrameRate.Value +
                    "', Cgi = '" + Cgi +
                    "' where CameraID = '" + ViewState["CameraID"].ToString() + "'", ref result))
                {
                    string _newcamera = txtCameraName.Text +
                            ";" + cbPC.Items[cbPC.SelectedIndex].Text +
                            ";" + txtHttpURL.Text +
                            ";" + txtHttpPort.Text +
                            ";" + txtRtspPort.Text +
                            ";" + txtUserName.Text +
                            //";" + txtPassword.Text +
                            ";" + txtChannel.Value +
                            ";" + cbCameraType.Items[cbCameraType.SelectedIndex].Text +
                            ";" + cbStreamType.Items[cbStreamType.SelectedIndex].Text +
                            ";" + txtFrameRate.Value +
                            ";" + cbSDK.Items[cbSDK.SelectedIndex].Text +
                            ";" + chbEnableRecording.Checked.ToString() +
                            ";" + chbInactive.Checked.ToString();
                    string _des = StaticPool.GetStringChange(ViewState["OldCamera"].ToString(), _newcamera);

                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Camera", txtCameraName.Text, "Sửa", _des);

                }
            }
            else
            {
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblCamera (CameraName, HttpURL, HttpPort, RtspPort, PCID, Inactive, EnableRecording, Resolution, CameraType, StreamType, SDK, UserName, Password, Channel, FrameRate, Cgi) values(N'" +
                     txtCameraName.Text + "', '" +
                     txtHttpURL.Text + "', '" +
                     txtHttpPort.Text + "', '" +
                     txtRtspPort.Text + "', '" +
                     cbPC.Value + "', " +
                     (chbInactive.Checked ? 1 : 0) + ", " +
                     (chbEnableRecording.Checked ? 1 : 0) + ", '" +
                     txtResolution.Text + "', '" +
                     cbCameraType.Value + "', '" +
                     cbStreamType.Value + "', '" +
                     cbSDK.Value + "', '" +
                     txtUserName.Text + "', '" +
                     CryptorEngine.Encrypt(txtPassword.Text, true) + "', " +
                     txtChannel.Value + ", " +
                     txtFrameRate.Value + ", '" +
                     Cgi +
                     "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Camera", txtCameraName.Text, "Thêm", "");
                }
            }

            txtPassword.Attributes.Add("value", txtPassword.Text);

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Camera.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
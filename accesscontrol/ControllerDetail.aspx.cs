using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class accesscontrol_ControllerDetail : System.Web.UI.Page
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

                for (int i = 0; i < SystemUI.GetLineTypes().Length; i++)
                {
                    cbLineType.Items.Add(SystemUI.GetLineTypes()[i]);
                }

                div_alert.Visible = false;
                if (Request.QueryString["ControllerID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Device_Controller", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_controllerdetail.InnerText = "Sửa bộ điều khiển";
                    ViewState["ControllerID"] = Request.QueryString["ControllerID"].ToString();
                    DataTable dtController = StaticPool.mdb.FillData("select * from tblController where ControllerID = '" + ViewState["ControllerID"].ToString() + "'");
                    if (dtController != null && dtController.Rows.Count > 0)
                    {
                        DataRow dr = dtController.Rows[0];
                        txtControllerName.Text = dr["ControllerName"].ToString();
                        cbPC.Value = dr["PCID"].ToString();
                        cbCommunicationType.SelectedIndex = int.Parse(dr["CommunicationType"].ToString());
                        txtComport.Text = dr["Comport"].ToString();
                        txtBaudrate.Text = dr["Baudrate"].ToString();
                        cbLineType.SelectedIndex = int.Parse(dr["LineTypeID"].ToString());
                        cbReader1Type.SelectedIndex = int.Parse(dr["Reader1Type"].ToString());
                        cbReader2Type.SelectedIndex = int.Parse(dr["Reader2Type"].ToString());
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());

                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Device_Controller", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_controllerdetail.InnerText = "Thêm bộ điều khiển mới";
                    ViewState["ControllerID"] = null;
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

            if (txtControllerName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên bộ điều khiển không được để trống!";
                return;
            }

            if (txtControllerName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên bộ điều khiển không được để trống!";
                return;
            }

            if (ViewState["ControllerID"] != null && ViewState["ControllerID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblController set ControllerName = N'" + txtControllerName.Text +
                    "', PCID = '" + cbPC.Value +
                    "', CommunicationType = " + cbCommunicationType.SelectedIndex +
                    ", Comport = '" + txtComport.Text +
                    "', Baudrate = '" + txtBaudrate.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    ", LineTypeID = " + cbLineType.SelectedIndex +
                    ", Reader1Type = " + cbReader1Type.SelectedIndex +
                    ", Reader2Type = " + cbReader2Type.SelectedIndex +
                    " where ControllerID = '" + ViewState["ControllerID"].ToString() + "'", ref result);
            }
            else
            {
                // Them moi
                StaticPool.mdb.ExecuteCommand("insert into tblController (ControllerName, CommunicationType, Comport, Baudrate, PCID, Inactive, LineTypeID, Reader1Type, Reader2Type) values(N'" +
                    txtControllerName.Text + "', " +
                    cbCommunicationType.SelectedIndex + ", '" +
                    txtComport.Text + "', '" +
                    txtBaudrate.Text + "', '" +
                    cbPC.Value + "', " +
                    (chbInactive.Checked ? 1 : 0) + ", " +
                    cbLineType.SelectedIndex + ", " +
                    cbReader1Type.SelectedIndex + ", " +
                    cbReader2Type.SelectedIndex + 
                    ")", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Controller.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
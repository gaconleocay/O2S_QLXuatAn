using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class QLXuatAn_GateDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;
                ViewState["OldGate"] = "";
                if (Request.QueryString["GateID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Gate", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_gatedetail.InnerText = "Sửa cổng";
                    ViewState["GateID"] = Request.QueryString["GateID"].ToString();
                    DataTable dtGate = StaticPool.mdb.FillData("select * from tblGate where GateID = '" + ViewState["GateID"].ToString() + "'");
                    if (dtGate != null && dtGate.Rows.Count > 0)
                    {
                        DataRow dr = dtGate.Rows[0];
                        txtDescription.Text = dr["Description"].ToString();
                        txtGateName.Text = dr["GateName"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());
                        ViewState["OldGate"] = txtGateName.Text + ";" + txtDescription.Text + ";" + chbInactive.Checked.ToString();
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Gate", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_gatedetail.InnerText = "Thêm cổng mới";
                    ViewState["GateID"] = null;
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

            if (txtGateName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên cổng không được để trống!";
                return;
            }

            if (ViewState["GateID"] != null && ViewState["GateID"].ToString() != "")
            {
                // Update
                if (StaticPool.mdb.ExecuteCommand("update tblGate set Description = N'" + txtDescription.Text +
                     "', GateName = N'" + txtGateName.Text +
                     "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                     " where GateID = '" + ViewState["GateID"].ToString() + "'", ref result))
                {
                    string _newgate = txtGateName.Text + ";" + txtDescription.Text + ";" + chbInactive.Checked.ToString();
                    string _des = StaticPool.GetStringChange(ViewState["OldGate"].ToString(), _newgate);
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Gate", txtGateName.Text, "Sửa", _des);
                }

            }
            else
            {
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblGate (Description, GateName, Inactive) values(N'" +
                    txtDescription.Text + "', N'" +
                    txtGateName.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) +
                    ")", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Gate", txtGateName.Text, "Thêm", "");
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Gate.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
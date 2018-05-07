using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class accesscontrol_PCDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;
                if (Request.QueryString["PCID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Device_PC", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_pcdetail.InnerText = "Sửa máy tính";
                    ViewState["PCID"] = Request.QueryString["PCID"].ToString();
                    DataTable dtPC = StaticPool.mdb.FillData("select * from tblPC where PCID = '" + ViewState["PCID"].ToString() + "'");
                    if (dtPC != null && dtPC.Rows.Count > 0)
                    {
                        DataRow dr = dtPC.Rows[0];
                        txtDescription.Text = dr["Description"].ToString();
                        txtComputerName.Text = dr["ComputerName"].ToString();
                        txtIPAddress.Text = dr["IPAddress"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Device_PC", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_pcdetail.InnerText = "Thêm máy tính mới";
                    ViewState["PCID"] = null;
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

            if (txtComputerName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên máy tính không được để trống!";
                return;
            }

            if (ViewState["PCID"] != null && ViewState["PCID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblPC set Description = N'" + txtDescription.Text +
                    "', ComputerName = '" + txtComputerName.Text +
                    "', IPAddress = '" + txtIPAddress.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    " where PCID = '" + ViewState["PCID"].ToString() + "'", ref result);
            }
            else
            {
                // Them moi
                StaticPool.mdb.ExecuteCommand("insert into tblPC (Description, ComputerName, IPAddress, Inactive) values(N'" +
                    txtDescription.Text + "', '" +
                    txtComputerName.Text + "', '" +
                    txtIPAddress.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) +
                    ")", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("PC.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
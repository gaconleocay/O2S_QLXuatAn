using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class QLXuatAn_PCDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                cbGate.DataSource = StaticPool.mdb.FillData("select * from tblGate order by SortOrder");
                cbGate.DataValueField = "GateID";
                cbGate.DataTextField = "GateName";
                cbGate.DataBind();
                ViewState["OldPC"] = "";
                div_alert.Visible = false;
                if (Request.QueryString["PCID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_PC", "Updates", "Parking"))
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
                        txtPicPathIn.Text = dr["PicPathIn"].ToString();
                        txtPicPathOut.Text = dr["PicPathOut"].ToString();
                        txtVideoPath.Text = dr["VideoPath"].ToString();
                        cbGate.SelectedValue = dr["GateID"].ToString();

                        ViewState["OldPC"] = txtComputerName.Text +
                            ";" + txtIPAddress.Text +
                            ";" + cbGate.SelectedItem.Text +
                            ";" + txtDescription.Text +
                            ";" + txtPicPathIn.Text +
                            ";" + txtPicPathOut.Text +
                            ";" + txtVideoPath.Text +
                            ";" + chbInactive.Checked.ToString();
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_PC", "Inserts", "Parking"))
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
                if (StaticPool.mdb.ExecuteCommand("update tblPC set Description = N'" + txtDescription.Text +
                     "', ComputerName = '" + txtComputerName.Text +
                     "', IPAddress = '" + txtIPAddress.Text +
                     "', GateID = '" + cbGate.SelectedValue +
                     "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                     ", PicPathIn='" + txtPicPathIn.Text +
                     "', PicPathOut='" + txtPicPathOut.Text +
                     "', VideoPath='" + txtVideoPath.Text +
                     "' where PCID = '" + ViewState["PCID"].ToString() + "'", ref result))
                {
                    string _newpc = txtComputerName.Text +
                            ";" + txtIPAddress.Text +
                            ";" + cbGate.SelectedItem.Text +
                            ";" + txtDescription.Text +
                            ";" + txtPicPathIn.Text +
                            ";" + txtPicPathOut.Text +
                            ";" + txtVideoPath.Text +
                            ";" + chbInactive.Checked.ToString();
                    string _des = StaticPool.GetStringChange(ViewState["OldPC"].ToString(), _newpc);

                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_PC", txtComputerName.Text, "Sửa", _des);
                }

            }
            else
            {
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblPC (Description, ComputerName, IPAddress, GateID, Inactive, PicPathIn, PicPathOut, VideoPath) values(N'" +
                     txtDescription.Text + "', '" +
                     txtComputerName.Text + "', '" +
                     txtIPAddress.Text + "', '" +
                     cbGate.SelectedValue + "', " +
                     (chbInactive.Checked ? 1 : 0) + ", '" +
                     txtPicPathIn.Text + "', '" +
                     txtPicPathOut.Text + "', '" +
                     txtVideoPath.Text +
                     "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_PC", txtComputerName.Text, "Thêm", "");
                }
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
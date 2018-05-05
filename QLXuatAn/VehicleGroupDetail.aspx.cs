using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class QLXuatAn_VehicleGroupDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ViewState["OldVehicleGroup"] = "";
                // Loai xe
                for (int i = 0; i < SystemUI.GetVehicleTypes().Length; i++)
                {
                    cbVehicleType.Items.Add(SystemUI.GetVehicleTypes()[i]);
                }

                div_alert.Visible = false;
                if (Request.QueryString["VehicleGroupID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_VehicleGroup", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_vehiclegroupdetail.InnerText = "Sửa";
                    ViewState["VehicleGroupID"] = Request.QueryString["VehicleGroupID"].ToString();
                    DataTable dtVehicleGroup = StaticPool.mdb.FillData("select * from tblVehicleGroup where VehicleGroupID = '" + ViewState["VehicleGroupID"].ToString() + "'");
                    if (dtVehicleGroup != null && dtVehicleGroup.Rows.Count > 0)
                    {
                        DataRow dr = dtVehicleGroup.Rows[0];
                        txtVehicleGroupName.Text = dr["VehicleGroupName"].ToString();
                        cbVehicleType.SelectedIndex = int.Parse(dr["VehicleType"].ToString());
                        txtLimitNumber.Text = dr["LimitNumber"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());

                        ViewState["OldVehicleGroup"] = txtVehicleGroupName.Text +
                        ";" + cbVehicleType.Items[cbVehicleType.SelectedIndex].Text +
                        ";" + txtLimitNumber.Text +
                        ";" + chbInactive.Checked.ToString();
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_VehicleGroup", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_vehiclegroupdetail.InnerText = "Thêm nhóm xe mới";
                    ViewState["VehicleGroupID"] = null;
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

            if (txtVehicleGroupName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên nhóm xe không được để trống!";
                return;
            }

            if (ViewState["VehicleGroupID"] != null && ViewState["VehicleGroupID"].ToString() != "")
            {
                // Update
                if (StaticPool.mdb.ExecuteCommand("update tblVehicleGroup set VehicleGroupName = N'" + txtVehicleGroupName.Text +
                    "', VehicleType = " + cbVehicleType.SelectedIndex +
                    ", LimitNumber = " + txtLimitNumber.Text +
                    ", Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    " where VehicleGroupID = '" + ViewState["VehicleGroupID"].ToString() + "'", ref result))
                {
                    string _newvehiclegroup = txtVehicleGroupName.Text +
                        ";" + cbVehicleType.Items[cbVehicleType.SelectedIndex].Text +
                        ";" + txtLimitNumber.Text +
                        ";" + chbInactive.Checked.ToString();
                    string _des = StaticPool.GetStringChange(ViewState["OldVehicleGroup"].ToString(), _newvehiclegroup);
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_VehicleGroup", txtVehicleGroupName.Text, "Sửa", _des);
                }
            }
            else
            {
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblVehicleGroup (VehicleGroupName, VehicleType, LimitNumber, Inactive) values(N'" +
                     txtVehicleGroupName.Text + "', " +
                     cbVehicleType.SelectedIndex + ", " +
                     txtLimitNumber.Text + ", " +
                     (chbInactive.Checked ? 1 : 0) +
                     ")", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_VehicleGroup", txtVehicleGroupName.Text, "Thêm", "");
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("VehicleGroup.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;

public partial class QLXuatAn_CompartmentDetail : System.Web.UI.Page
{
    private string codeTable = "Compartment";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ViewState["OldCompartment"] = "";

                div_alert.Visible = false;
                if (Request.QueryString["CompartmentID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_Compartment", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_compartment.InnerText = "Sửa";
                    ViewState["CompartmentID"] = Request.QueryString["CompartmentID"].ToString();
                    DataTable dtCompartment = StaticPool.mdb.FillData("select * from tblCompartment where CompartmentID = '" + ViewState["CompartmentID"].ToString() + "'");
                    if (dtCompartment != null && dtCompartment.Rows.Count > 0)
                    {
                        DataRow dr = dtCompartment.Rows[0];
                        txtCompartmentName.Text = dr["CompartmentName"].ToString();
                        ViewState["OldCompartment"] = txtCompartmentName.Text;

                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_Compartment", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_compartment.InnerText = "Thêm nhóm căn hộ";
                    ViewState["CompartmentID"] = null;
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

            if (txtCompartmentName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên căn không được để trống!";
                return;
            }

            

            if (ViewState["CompartmentID"] != null && ViewState["CompartmentID"].ToString() != "")
            {
                string getexit = string.Format("select * from tblCompartment where CompartmentName = N'{0}' and CompartmentID <> '{1}'", txtCompartmentName.Text, ViewState["CompartmentID"].ToString());

                DataTable objExist = StaticPool.mdb.FillData(getexit);
                if (objExist != null && objExist.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Tên căn đã tồn tại!";
                    return;
                }

                string command = string.Format("update tblCompartment set CompartmentName = N'{0}' where CompartmentID = '{1}'", txtCompartmentName.Text, ViewState["CompartmentID"].ToString());
                // Update
                if (StaticPool.mdb.ExecuteCommand(command))
                {
                    string _newCompartment = txtCompartmentName.Text;
                    string _des = StaticPool.GetStringChange(ViewState["OldCompartment"].ToString(), _newCompartment);
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_Compartment", txtCompartmentName.Text, "Sửa", _des);
                }
                div_alert.Visible = true;
            }
            else
            {
                string getexit = string.Format("select * from tblCompartment where CompartmentName = N'{0}'", txtCompartmentName.Text);

                DataTable objExist = StaticPool.mdb.FillData(getexit);
                if (objExist != null && objExist.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Tên căn đã tồn tại!";
                    return;
                }

                string command = string.Format("insert into tblCompartment (CompartmentName) values (N'{0}')", txtCompartmentName.Text);
                if (StaticPool.mdb.ExecuteCommand(command))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_Compartment", txtCompartmentName.Text, "Thêm", "");
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Compartment.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
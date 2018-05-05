using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class QLXuatAn_LEDDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;



                ViewState["LEDID"] = "";
                if (Request.QueryString["LEDID"] != null)
                    ViewState["LEDID"] = Request.QueryString["LEDID"].ToString();

                for (int i = 0; i < Futech.Tools.SystemUI.GetLedType().Length; i++)
                {
                    cbLedType.Items.Add(Futech.Tools.SystemUI.GetLedType()[i]);
                }

                //pc
                cbPC.DataSource = StaticPool.mdb.FillData("select (ComputerName + ' (' + IPAddress + ')') as ComputerName, PCID from tblPC");
                cbPC.DataValueField = "PCID";
                cbPC.DataTextField = "ComputerName";
                cbPC.DataBind();

                if (ViewState["LEDID"].ToString() != "")
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Led", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_detail.InnerText = "Sửa";

                    DataTable dt = StaticPool.mdb.FillData("select * from tblLed where LEDID='" + ViewState["LEDID"].ToString() + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRowView drv = dt.DefaultView[0];
                        txtName.Text = drv["LEDName"].ToString();
                        txtComport.Text = drv["Comport"].ToString();
                        txtBaudrate.Text = drv["Baudrate"].ToString();
                        txtAddress.Text = drv["Address"].ToString();
                        cbPC.Value = drv["PCID"].ToString();
                        cbLedType.Value = drv["LedType"].ToString();
                        cbSide.Value = drv["SideIndex"].ToString();
                        chEnable.Checked = !bool.Parse(drv["EnableLED"].ToString());
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Led", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_detail.InnerText = "Thêm";
                }
            }
            catch (Exception ex)
            {
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

            if (txtName.Text == "")
            {
                div_alert.Visible = true;
                id_alert.InnerText = "Tên không được trống";
                return;
            }

            if (ViewState["LEDID"].ToString() == "")
            {
                DataTable temp = StaticPool.mdb.FillData("select FeeName from tblLed where LedName=N'" + txtName.Text + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Tên đã tồn tại";
                    return;
                }

                if (StaticPool.mdb.ExecuteCommand("insert into tblLed(LEDName, PCID, Comport, Baudrate, SideIndex, Address, LedType, EnableLED) values(N'" +
                    txtName.Text + "', '" +
                    cbPC.Value + "', '" +
                    txtComport.Text + "', '" +
                    txtBaudrate.Text + "', '" +
                    cbSide.Value + "', '" +
                    txtAddress.Text + "', '" +
                    cbLedType.Value + "', '" +
                    (!chEnable.Checked) +

                    "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Led", txtName.Text, "Thêm", "");
                }




            }
            else
            {
                DataTable temp = StaticPool.mdb.FillData("select LEDName from tblFee where LEDName=N'" + txtName.Text + "' and LEDID<>'" + ViewState["LEDID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Tên đã tồn tại";
                    return;
                }

                if (StaticPool.mdb.ExecuteCommand("update tblLED set" +
                    " LEDName=N'" + txtName.Text +
                    "', PCID='" + cbPC.Value +
                    "', Comport='" + txtComport.Text +
                    "', Baudrate='" + txtBaudrate.Text +
                    "', SideIndex='" + cbSide.Value +
                    "', LedType='" + cbLedType.Value +
                    "', EnableLED='" + (!chEnable.Checked) +
                    "', Address='" + txtAddress.Text +
                    "' where LEDID='" + ViewState["LEDID"].ToString() +
                    "'", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Led", txtName.Text, "Sửa", "");
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("LED.aspx");

        }
        catch (Exception ex)
        {
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
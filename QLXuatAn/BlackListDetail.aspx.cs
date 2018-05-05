using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class QLXuatAn_BlackListDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;



                ViewState["ID"] = "";
                if (Request.QueryString["ID"] != null)
                    ViewState["ID"] = Request.QueryString["ID"].ToString();

                //for (int i = 0; i < Futech.Tools.SystemUI.GetLedType().Length; i++)
                //{
                //    cbLedType.Items.Add(Futech.Tools.SystemUI.GetLedType()[i]);
                //}

                ////pc
                //cbPC.DataSource = StaticPool.mdb.FillData("select (ComputerName + ' (' + IPAddress + ')') as ComputerName, PCID from tblPC");
                //cbPC.DataValueField = "PCID";
                //cbPC.DataTextField = "ComputerName";
                //cbPC.DataBind();

                if (ViewState["ID"].ToString() != "")
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_BlackList", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_detail.InnerText = "Sửa";

                    DataTable dt = StaticPool.mdb.FillData("select * from tblBlackList where ID='" + ViewState["ID"].ToString() + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRowView drv = dt.DefaultView[0];
                        txtName.Text = drv["Name"].ToString();
                        txtPlate.Text = drv["Plate"].ToString();
                        txtDescription.Text = drv["Description"].ToString();
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_BlackList", "Inserts", "Parking"))
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

            if (txtPlate.Text == "")
            {
                div_alert.Visible = true;
                id_alert.InnerText = "Biển số không được trống";
                return;
            }

            if (ViewState["ID"].ToString() == "")
            {
                //DataTable temp = StaticPool.mdb.FillData("select FeeName from tblLed where LedName=N'" + txtName.Text + "'");
                //if (temp != null && temp.Rows.Count > 0)
                //{
                //    div_alert.Visible = true;
                //    id_alert.InnerText = "Tên đã tồn tại";
                //    return;
                //}

                if (StaticPool.mdb.ExecuteCommand("insert into tblBlackList(Name, Plate, Description) values(N'"+
                    txtName.Text+"', N'"+
                    txtPlate.Text.Replace("-","").Replace(".","").Replace(" ","")+"', N'"+
                    txtDescription.Text+
                    "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_Blacklist", txtPlate.Text, "Thêm", "");
                }




            }
            else
            {
                //DataTable temp = StaticPool.mdb.FillData("select LEDName from tblFee where LEDName=N'" + txtName.Text + "' and LEDID<>'" + ViewState["LEDID"].ToString() + "'");
                //if (temp != null && temp.Rows.Count > 0)
                //{
                //    div_alert.Visible = true;
                //    id_alert.InnerText = "Tên đã tồn tại";
                //    return;
                //}

                if (StaticPool.mdb.ExecuteCommand("update tblBlackList set"+
                    " Name=N'"+txtName.Text+
                    "', Plate=N'" + txtPlate.Text.Replace("-", "").Replace(".", "").Replace(" ", "") +
                    "', Description=N'"+txtDescription.Text+
                    "' where Id='"+ViewState["ID"].ToString()+
                    "'", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_Blacklist", txtPlate.Text, "Sửa", "");
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("BlackList.aspx");

        }
        catch (Exception ex)
        {
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
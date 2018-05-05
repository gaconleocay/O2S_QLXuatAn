using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;

public partial class QLXuatAn_Profile : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;
                div_info.Visible = false;
                if (Request.Cookies["UserID"] != null)
                {
                    ViewState["UserID"] = Request.Cookies["UserID"].Value;
                    DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + ViewState["UserID"].ToString() + "'");
                    if (dtUser != null && dtUser.Rows.Count > 0)
                    {
                        DataRow dr = dtUser.Rows[0];
                        txtCode.Text = dr["UserCode"].ToString();
                        txtFullName.Text = dr["FullName"].ToString();
                        txtUserName.Text = dr["UserName"].ToString();
                        txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                        txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                    }
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
            if (txtPassword.Text != txtRePassword.Text)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mật khẩu không trùng nhau!";
                return;
            }

            if (ViewState["UserID"] != null && ViewState["UserID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblUser set FullName = N'" + txtFullName.Text +
                    "', Password = '" + Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) +
                    "' where UserID = '" + ViewState["UserID"].ToString() + "'", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                div_info.Visible = false;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;

public partial class accesscontrol_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.QueryString["status"] != null && Request.QueryString["status"].ToString() == "logout")
                {
                    Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(-1);
                }
                else
                {
                    if (Request.Cookies["UserName"] != null)
                        txtUserName.Text = Request.Cookies["UserName"].Value;
                    if (Request.Cookies["PWD"] != null)
                        txtPassword.Attributes.Add("value", Request.Cookies["PWD"].Value);
                    if (Request.Cookies["UserName"] != null && Request.Cookies["PWD"] != null)
                        chbIsRemember.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    protected void Login(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            DataTable dtUser = StaticPool.mdb.FillData("spLogin", new string[] { "@UserName", "@Password" }, new string[] { txtUserName.Text, CryptorEngine.Encrypt(txtPassword.Text, true) }, ref result);
            //DataTable dtUser = StaticPool.mdb.FillData("select * from tblEmployee where Email = '" + txtUserName.Text + "' and Password = '" + txtPassword.Text + "'");

            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                if (chbIsRemember.Checked)
                {
                    Response.Cookies["UserName"].Value = txtUserName.Text;
                    Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                    Response.Cookies["PWD"].Value = txtPassword.Text;
                    Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(30);
                }
                else
                {
                    Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(-1);
                }

                // Luu vao cookies
                Response.Cookies["UserID"].Value = dtUser.Rows[0]["UserID"].ToString();
                Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(30);
                Response.Redirect("Default.aspx");
                return;
            }
            else if (dtUser == null)
                Response.Write(result);
            else
            {
                Response.Write("Login Fail");
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

}
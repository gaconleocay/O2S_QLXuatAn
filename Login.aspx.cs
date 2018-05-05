using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;
using Futech.Helpers;
using System.Web.Services;

public partial class QLXuatAn_Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                txtLogoName.InnerText = GetLogoNameFromConfig();
                if (Request.QueryString["status"] != null && Request.QueryString["status"].ToString() == "logout")
                {
                    Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(-1);
                    Session["LogoName"] = null;
                    txtLogoName.InnerText = GetLogoNameFromConfig();

                    CacheLayer.ClearAll();
                }
                //else
                //{
                //    if (Request.Cookies["UserName"] != null)
                //        txtUserName.Text = Request.Cookies["UserName"].Value;
                //    if (Request.Cookies["PWD"] != null)
                //        txtPassword.Attributes.Add("value", Request.Cookies["PWD"].Value);
                //    //if (Request.Cookies["UserName"] != null && Request.Cookies["PWD"] != null)
                //    //    chbIsRemember.Checked = true;
                //}
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
            //if (SystemUI.CheckHardKey() == false)
            //{
            //    Response.Write("Cannot find dongle");
            //    //return;
            //}

            string st = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS" +
                           " WHERE TABLE_NAME = 'tblSystemConfig' AND COLUMN_NAME = 'Para1')" +
                       " BEGIN" +
                           " ALTER TABLE tblSystemConfig ADD Para1 nvarchar(50) not null default('01/VE009')" +
                       " END";

            if (StaticPool.mdb.ExecuteCommand(st))
            {
                //SystemUI.SaveLogFile("SystemConfig - Create Para1 successful");
            }
            else
            {
                //SystemUI.SaveLogFile("SystemConfig(Para1) - Exist or failed");
            }

            st = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS" +
                   " WHERE TABLE_NAME = 'tblSystemConfig' AND COLUMN_NAME = 'Para2')" +
               " BEGIN" +
                   " ALTER TABLE tblSystemConfig ADD Para2 nvarchar(50) not null default('AC/17T')" +
               " END";

            if (StaticPool.mdb.ExecuteCommand(st))
            {
                // SystemUI.SaveLogFile("SystemConfig - Create Para2 successful");
            }
            else
            {
                // SystemUI.SaveLogFile("SystemConfig(Para2) - Exist or failed");
            }

            st = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS" +
                   " WHERE TABLE_NAME = 'tblLogCardCustomer' AND COLUMN_NAME = 'CardIsLockHidden')" +
               " BEGIN" +
                       " ALTER TABLE tblLogCardCustomer ADD CardIsLockHidden bit null" +
                       " EXEC(' Update tblLogCardCustomer SET CardIsLockHidden = CardIsLock ')" +
                       " EXEC(' Update tblLogCardCustomer set CardIsLockHidden = 1" +
                       " WHERE CardNumber IN(" +
                       " SELECT distinct lcc.CardNumber FROM tblLogCardCustomer lcc" +
                       " inner join tblCard c on lcc.CardNumber = c.CardNumber" +
                       " WHERE c.IsLock = 1" +
                       " )')" +
                       " EXEC(' Update tblLogCardCustomer set CardIsLockHidden = 0" +
                       " WHERE CardNumber IN(" +
                       " SELECT distinct lcc.CardNumber FROM tblLogCardCustomer lcc" +
                       " inner join tblCard c on lcc.CardNumber = c.CardNumber" +
                       " WHERE c.IsLock = 0" +
                       " )')" +
                " END";

            if (StaticPool.mdb.ExecuteCommand(st))
            {

            }
            else
            {
                // SystemUI.SaveLogFile("SystemConfig(Para2) - Exist or failed");
            }

            st = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS" +
                          " WHERE TABLE_NAME = 'tblAlarm' AND COLUMN_NAME = 'Description')" +
                      " BEGIN" +
                          " ALTER TABLE tblAlarm ADD Description nvarchar(100) not null default('')" +
                      " END";
            if (StaticPool.mdbevent.ExecuteCommand(st))//for future
            {
               
            }
            else
            {
               
            }

            st = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'tblSubCard')" +
                          " BEGIN" +
                              " CREATE TABLE tblSubCard(ID int IDENTITY(1,1) PRIMARY KEY, MainCard nvarchar(50), CardNo nvarchar(50), CardNumber nvarchar(50),IsDelete bit not null default(0))" +
                          " END";
            if (StaticPool.mdb.ExecuteCommand(st))
            { }

            st = "IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS" +
                         " WHERE TABLE_NAME = 'tblCard' AND COLUMN_NAME = 'ChkRelease')" +
                     " BEGIN" +
                         " ALTER TABLE tblCard ADD ChkRelease bit not null default(0)" +
                     " END";
            if(StaticPool.mdb.ExecuteCommand(st))
            { }

            string result = "";
            DataTable dtUser = StaticPool.mdb.FillData("spLogin", new string[] { "@UserName", "@Password" }, new string[] { txtUserName.Text, CryptorEngine.Encrypt(txtPassword.Text, true) }, ref result);
            //DataTable dtUser = StaticPool.mdb.FillData("select * from tblEmployee where Email = '" + txtUserName.Text + "' and Password = '" + txtPassword.Text + "'");

            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                //if (chbIsRemember.Checked)
                //{
                //    Response.Cookies["UserName"].Value = txtUserName.Text;
                //    Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                //    Response.Cookies["PWD"].Value = txtPassword.Text;
                //    Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(30);
                //}
                //else
                //{
                //    Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                //    Response.Cookies["PWD"].Expires = DateTime.Now.AddDays(-1);
                //}

                if (StaticPool.CheckPermission(dtUser.Rows[0]["UserID"].ToString(), "Parking_Login_Web", "Selects", "Parking") == false)
                {
                    Response.Write("You have not permission to login");
                    return;
                }

                DataTable temp = StaticPool.mdb.FillData("select FeeName from tblSystemConfig");
                if (temp != null && temp.Rows.Count > 0)
                {
                    Session["FeeName"] = temp.Rows[0]["FeeName"].ToString();
                }

                StaticPool.SaveLogFile(txtUserName.Text, "Parking", "Parking_Login_Web", txtUserName.Text, "Đăng nhập", "Web");

                // Luu vao cookies
                Response.Cookies["UserID"].Value = dtUser.Rows[0]["UserID"].ToString();
                Response.Cookies["UserID"].Expires = DateTime.Now.AddDays(1);
                CacheLayer.Add(StaticCached.c_tblUserById + "_" + dtUser.Rows[0]["UserID"].ToString(), dtUser, StaticCached.TimeCache);
                Response.Redirect("QLXuatAn/Default.aspx");
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

    public string GetLogoNameFromConfig()
    {
        string logoname = "Kztek JSC";
        Session["FeeName"] = "";
        //if (Session["LogoName"] == null)
        //{
        string command = "select top 1 * from tblSystemConfig";

        DataTable logo = StaticPool.mdb.FillData(command);
        if (logo != null && logo.Rows.Count > 0)
        {
            if (logo.Rows[0]["Logo"].ToString() != "")
                logoname = logo.Rows[0]["Logo"].ToString();

            Session["FeeName"] = logo.Rows[0]["FeeName"].ToString();

        }

        Session["LogoName"] = logoname;



        // }
        //else
        //{
        //    logoname = (string)Session["LogoName"];
        //}

        return logoname;
    }



    //button clear cached
    [WebMethod]
    public static bool cleanCached()
    {
        var chk = true;
        try
        {
            CacheLayer.ClearAll();
        }
        catch { chk = false; }

        return chk;
    }
}
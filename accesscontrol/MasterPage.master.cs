using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.IO;
using System.Web.Script.Serialization;

public partial class accesscontrol_MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            try
            {
                if (Request.Cookies["UserID"] == null)
                    Response.Redirect("Login.aspx");
                else
                {
                    DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + Request.Cookies["UserID"].Value.ToString() + "'");
                    if (dtUser != null && dtUser.Rows.Count == 1)
                    {
                        id_userinfo.InnerHtml = "<small>Xin chào </small>" + dtUser.Rows[0]["UserName"].ToString();
                        if (!bool.Parse(dtUser.Rows[0]["IsSystem"].ToString()))
                        {
                            if (!SetRolePermissionMaping(dtUser.Rows[0]["UserID"].ToString(), "AccessControl"))
                                Response.Redirect("Login.aspx");
                        }
                    }
                    else
                        Response.Redirect("Login.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    private bool SetRolePermissionMaping(string UserID, string AppCode)
    {
        if (StaticPool.CheckRoleSystem(UserID, AppCode))
            return true;
        else
        {
            bool result = false;
            DataTable dtSubSystem = StaticPool.mdb.FillData("select * from tblSubSystem where AppCode = '" + AppCode + "' order by SortOrder");
            if (dtSubSystem != null && dtSubSystem.Rows.Count > 0)
            {
                foreach (DataRow drSubSystem in dtSubSystem.Rows)
                {
                    Control control = this.FindControl(drSubSystem["SubSystemCode"].ToString());
                    DataTable dtRolePermissionMaping = StaticPool.mdb.FillData("select * from vGetUserJoinRoleAndPermisson where UserID = '" + UserID + "' and SubSystemID = '" + drSubSystem["SubSystemID"].ToString() + "'");
                    if (dtRolePermissionMaping != null && dtRolePermissionMaping.Rows.Count > 0)
                    {
                        DataRow drtem = dtRolePermissionMaping.Rows[0];
                        if (!bool.Parse(drtem["Selects"].ToString()))
                            control.Visible = false;
                        else
                            result = true;
                    }
                    else if (control != null)
                    {
                        control.Visible = false;
                    }
                }
            }
            return result;
        }
    }
}

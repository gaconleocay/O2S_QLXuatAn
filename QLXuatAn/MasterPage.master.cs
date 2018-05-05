using System;
using System.Web.UI;
using System.Data;
using Futech.Helpers;

public partial class QLXuatAn_MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            try
            {
                //if (Session["LogoName"] != null && Session["LogoName"].ToString().Contains("Kztek") == false)
                //    txtLogoName.InnerText = Session["LogoName"].ToString();
                //else
                txtLogoName.InnerText = GetLogoNameFromConfig();
                id_userphoto1.Visible = false;

                if (txtLogoName.InnerText.Contains("Kztek")==false)
                {
                    boxCommunity.Visible = false;
                    id_userphoto.Visible = false;
                    id_userphoto1.Visible = true;
                }
                else
                {
                    boxCommunity.Visible = true;
                    id_userphoto.Visible = true;
                    id_userphoto1.Visible = false;
                }

                if (Request.Cookies["UserID"] == null)
                    Response.Redirect("../Login.aspx", false);
                else
                {
                    DataTable dtUser = CacheLayer.Get<DataTable>(StaticCached.c_tblUserById + "_" + Request.Cookies["UserID"].Value.ToString());
                    if (dtUser == null)
                    {
                        dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + Request.Cookies["UserID"].Value.ToString() + "'");
                        if (dtUser.Rows.Count > 0)
                            CacheLayer.Add(StaticCached.c_tblUserById + "_" + Request.Cookies["UserID"].Value.ToString(), dtUser, StaticCached.TimeCache);
                    }
                    //DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + Request.Cookies["UserID"].Value.ToString() + "'");
                    if (dtUser != null && dtUser.Rows.Count == 1)
                    {
                        id_userinfo.InnerHtml = "<small>Xin chào </small>" + dtUser.Rows[0]["UserName"].ToString();
                        if (!bool.Parse(dtUser.Rows[0]["IsSystem"].ToString()))
                        {
                            if (!SetRolePermissionMaping(dtUser.Rows[0]["UserID"].ToString(), "Parking"))
                                Response.Redirect("../Login.aspx", false);
                        }
                    }
                    else
                        Response.Redirect("../Login.aspx",false);
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
        try
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
                        string st = drSubSystem["SubSystemCode"].ToString();
                        Control control = this.FindControl(drSubSystem["SubSystemCode"].ToString());
                        if (control == null)
                            continue;
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
        catch
        { }
        return false;
    }

    public string GetLogoNameFromConfig()
    {
        string logoname = "Kztek JSC";

        if (Session["LogoName"] == null)
        {
            string command = "select top 1 Logo from tblSystemConfig";

            DataTable logo = StaticPool.mdb.FillData(command);
            if (logo != null && logo.Rows.Count > 0)
            {
                if (logo.Rows[0]["Logo"].ToString() != "")
                    logoname = logo.Rows[0]["Logo"].ToString();
            }

            Session["LogoName"] = logoname;
        }
        else
        {
            logoname = (string)Session["LogoName"];
        }

        return logoname;
    }
}

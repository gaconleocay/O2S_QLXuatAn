using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;

public partial class accesscontrol_CardTypeDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;
                if (Request.QueryString["CardGroupID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_List_CardGroup", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_cardgroupdetail.InnerText = "Sửa nhóm thẻ";
                    ViewState["CardGroupID"] = Request.QueryString["CardGroupID"].ToString();
                    DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + ViewState["CardGroupID"].ToString() + "'");
                    if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                    {
                        DataRow dr = dtCardGroup.Rows[0];
                        txtDescription.Text = dr["Description"].ToString();
                        txtCardGroupName.Text = dr["CardGroupName"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_List_CardGroup", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_cardgroupdetail.InnerText = "Thêm nhóm thẻ mới";
                    ViewState["CardGroupID"] = null;
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

            if (txtCardGroupName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên nhóm thẻ không được để trống!";
                return;
            }

            if (ViewState["CardGroupID"] != null && ViewState["CardGroupID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblCardGroup set Description = N'" + txtDescription.Text +
                    "', CardGroupName = N'" + txtCardGroupName.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    " where CardGroupID = '" + ViewState["CardGroupID"].ToString() + "'", ref result);
            }
            else
            {
                // Them moi
                StaticPool.mdb.ExecuteCommand("insert into tblCardGroup (Description, CardGroupName, Inactive) values(N'" +
                    txtDescription.Text + "', N'" +
                    txtCardGroupName.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) + 
                    ")", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("CardGroup.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
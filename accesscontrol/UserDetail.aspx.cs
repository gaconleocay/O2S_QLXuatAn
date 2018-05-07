using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;

public partial class accesscontrol_UserDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                cbRoles.DataSource = StaticPool.mdb.FillData("select * from tblRole where AppCode = 'AccessControl' order by SortOrder");
                cbRoles.DataTextField = "RoleName";
                cbRoles.DataValueField = "RoleID";
                cbRoles.DataBind();

                div_alert.Visible = false;
                string UserCode = "";
                if (Request.QueryString["UserID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_System_User", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_userdetail.InnerText = "Sửa người dùng";
                    ViewState["UserID"] = Request.QueryString["UserID"].ToString();
                    DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserID = '" + ViewState["UserID"].ToString() + "'");
                    if (dtUser != null && dtUser.Rows.Count > 0)
                    {
                        DataRow dr = dtUser.Rows[0];

                        if (bool.Parse(dr["IsSystem"].ToString()))
                            Response.Redirect("Message.aspx?Message=" + "Đây là người dùng nghầm định của hệ thống. Bạn không được phép sửa người dùng này!", false);

                        UserCode = dr["UserCode"].ToString();
                        txtFullName.Text = dr["FullName"].ToString();
                        ViewState["UserName"] = dr["UserName"].ToString();
                        txtUserName.Text = dr["UserName"].ToString();
                        txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                        txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                        chbIsLockUser.Checked = bool.Parse(dr["IsLock"].ToString());

                        DataTable dtUserJoinRole = StaticPool.mdb.FillData("select * from tblUserJoinRole where UserID = '" + dr["UserID"].ToString() + "'");
                        if (dtUserJoinRole != null && dtUserJoinRole.Rows.Count > 0)
                        {
                            foreach (ListItem item in cbRoles.Items)
                            {
                                foreach (DataRow drRole in dtUserJoinRole.Rows)
                                {
                                    if (item.Value == drRole["RoleID"].ToString())
                                        item.Selected = true;
                                }
                            }
                        }

                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_System_User", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_userdetail.InnerText = "Thêm người dùng mới";
                    ViewState["UserID"] = null;
                }

                for (int i = 1; i <= 99; i++)
                {
                    DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserCode = '" + i.ToString("00") + "'");
                    if (dtUser == null || dtUser.Rows.Count == 0 || i.ToString("00") == UserCode)
                    {
                        cbUserCode.Items.Add(new ListItem(i.ToString("00"), i.ToString("00")));
                    }
                }
                cbUserCode.Value = UserCode;
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

            if (txtFullName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Họ tên không được để trống!";
                return;
            }

            if (txtUserName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên đăng nhập không được để trống!";
                return;
            }

            DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser where UserName = N'" + txtUserName.Text + "'");
            if (dtUser != null && dtUser.Rows.Count > 0 && (ViewState["UserName"] == null || txtUserName.Text != ViewState["UserName"].ToString()))
            {
                txtPassword.Attributes.Add("value", txtPassword.Text);
                txtRePassword.Attributes.Add("value", txtRePassword.Text);

                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác!";
                return;
            }

            if (ViewState["UserID"] != null && ViewState["UserID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblUser set FullName = N'" + txtFullName.Text +
                    "', UserName = N'" + txtUserName.Text +
                    "', Password = '" + Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) +
                    "', IsLock = " + (chbIsLockUser.Checked ? 1 : 0) +
                    ", UserCode = '" + cbUserCode.Value + "'" +
                    " where UserID = '" + ViewState["UserID"].ToString() + "'", ref result);
            }
            else
            {
                // Them moi
                ViewState["UserID"] = Guid.NewGuid().ToString();
                StaticPool.mdb.ExecuteCommand("insert into tblUser (UserID, FullName, UserName, Password, IsLock, UserCode) values('" +
                    ViewState["UserID"].ToString() + "', N'" + 
                    txtFullName.Text + "', N'" +
                    txtUserName.Text + "', '" +
                    Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) + "', " +
                    (chbIsLockUser.Checked ? 1 : 0) + ", '" +
                    cbUserCode.Value + "'" +
                    ")", ref result);
            }

            // Phan quyen nguoi dung
            foreach (ListItem item in cbRoles.Items)
            {
                StaticPool.mdb.ExecuteCommand("delete from tblUserJoinRole where UserID = '" + ViewState["UserID"].ToString() + "' and RoleID = '" + item.Value + "'", ref result);
                if (item.Selected)
                {
                    StaticPool.mdb.ExecuteCommand("insert into tblUserJoinRole (UserID, RoleID) values('" +
                    ViewState["UserID"].ToString() + "', '" +
                    item.Value + 
                    "')", ref result);
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("User.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
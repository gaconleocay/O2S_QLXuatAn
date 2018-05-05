using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class QLXuatAn_FeeDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;



                ViewState["FeeID"] = "";
                if (Request.QueryString["FeeID"] != null)
                    ViewState["FeeID"] = Request.QueryString["FeeID"].ToString();

                //cardgroup
                DataTable dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup where CardType=0 order by SortOrder");
                if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCardGroup.Rows)
                    {
                        cbCardGroup.Items.Add(new ListItem(dr["CardGroupName"].ToString(), dr["CardGroupID"].ToString()));
                    }
                }

                if (ViewState["FeeID"].ToString() != "")
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_Fee", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_feedetail.InnerText = "Sửa";

                    DataTable dt = StaticPool.mdb.FillData("select * from tblFee where FeeID='" + ViewState["FeeID"].ToString() + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRowView drv = dt.DefaultView[0];
                        txtFeeName.Text = drv["FeeName"].ToString();
                        cbCardGroup.Value = drv["CardGroupID"].ToString();
                        txtFeeLevel.Text = drv["FeeLevel"].ToString();
                        if (drv["Units"].ToString() != "")
                        {
                            string[] st = drv["Units"].ToString().Split('_');
                            if (st != null && st.Length == 2)
                            {
                                txtUnitNumber.Text = st[0];


                                cbUnitText.Value = st[1];
                            }
                        }
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_Fee", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_feedetail.InnerText = "Thêm";
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

            if (txtFeeName.Text == "")
            {
                div_alert.Visible = true;
                id_alert.InnerText = "Tên không được trống";
                return;
            }
            string _feelevel = txtFeeLevel.Text.Replace(",", "");
            string _units = txtUnitNumber.Text + "_" + cbUnitText.Value;

            if (ViewState["FeeID"].ToString() == "")
            {
                DataTable temp = StaticPool.mdb.FillData("select FeeName from tblFee where FeeName=N'" + txtFeeName.Text + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Tên đã tồn tại";
                    return;
                }

                if (StaticPool.mdb.ExecuteCommand("insert into tblFee(FeeName, CardGroupID, FeeLevel, Units) values(N'" +
                    txtFeeName.Text + "', '" +
                    cbCardGroup.Value + "', '" +
                    _feelevel + "', N'" +
                    _units +
                    "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_Fee", txtFeeName.Text, "Thêm", "");
                }

              


            }
            else
            {
                DataTable temp = StaticPool.mdb.FillData("select FeeName from tblFee where FeeName=N'" + txtFeeName.Text + "' and FeeID<>'" + ViewState["FeeID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Tên đã tồn tại";
                    return;
                }

                if (StaticPool.mdb.ExecuteCommand("update tblFee set"+
                    " FeeName=N'"+txtFeeName.Text+
                    "', CardGroupID='"+cbCardGroup.Value+
                    "', FeeLevel='"+_feelevel+
                    "', Units=N'"+_units+
                    "' where FeeID='"+ViewState["FeeID"].ToString()+
                    "'", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_Fee", txtFeeName.Text, "Sửa", "");
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Fee.aspx");

        }
        catch (Exception ex)
        {
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }
}
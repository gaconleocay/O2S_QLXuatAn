using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using System.Data;
using Futech.Tools;

public partial class accesscontrol_CardDetailModal : System.Web.UI.Page
{
    public string Id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                // Nhom the
                cbCardGroup.DataSource = StaticPool.mdb.FillData("select * from tblCardGroup");
                cbCardGroup.DataTextField = "CardGroupName";
                cbCardGroup.DataValueField = "CardGroupID";
                cbCardGroup.DataBind();

                if (Request.QueryString["CustomerID"] != null)
                {
                    ViewState["CustomerID"] = Request.QueryString["CustomerID"].ToString();
                }
                else
                {
                    ViewState["CustomerID"] = "";
                }

                dtpExpireDate.Value = DateTime.Now.AddMonths(6).ToString("dd/MM/yyyy");

                if (Request.QueryString["Id"] != null)
                {
                    Id = Request.QueryString["Id"].ToString();
                    id_carddetail.InnerText = "Sửa thẻ";
                    DataTable dt = StaticPool.mdb.FillData("select * from tblCard where CardID = '" + Id + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        txtCardNo.Value = dr["CardNo"].ToString();
                        txtCardNumber.Value = dr["CardNumber"].ToString();
                        cbCardGroup.Value = dr["CardGroupID"].ToString();
                        if (dr["ExpireDate"].ToString() != "")
                            dtpExpireDate.Value = DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy");
                        chbIsLock.Checked = bool.Parse(dr["IsLock"].ToString());
                    }
                }
                else
                {
                    txtCardNo.Value = "";
                    txtCardNumber.Value = "";
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    [WebMethod]
    public static string Save(string Id, string CardNo, string CardNumber, string CardGroupID, string CustomerID, string ExpireDate, string IsLock)
    {
        try
        {
            string result = "";
            ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);

            if (CardNumber == "")
                return "Mã thẻ không được để trống!";

            DataTable dtCard = StaticPool.mdb.FillData("select * from tblCard where CardNumber = '" + CardNumber + "'");
            if (dtCard != null && dtCard.Rows.Count > 0 && ((dtCard.Rows[0]["CardID"].ToString() != Id && Id != "")||Id=="") )
                return "Mã thẻ đã khai báo! Vui lòng nhập mã thẻ khác.";

            if (Id == "")
            {
                // insert
                if (StaticPool.mdb.ExecuteCommand("insert into tblCard (CardNo, CardNumber, CardGroupID, CustomerID, ExpireDate, IsLock) values('" + CardNo +
                    "', '" + CardNumber +
                    "', '" + CardGroupID +
                    "', '" + CustomerID + 
                    "', '" + ExpireDate +
                    "', " + (bool.Parse(IsLock) ? 1 : 0) +
                    ")", ref result))
                    return "true";
                else
                {
                    return result;
                }
            }
            else
            {
                // update
                if (StaticPool.mdb.ExecuteCommand("update tblCard set CardNo = '" + CardNo +
                    "', CardNumber = '" + CardNumber +
                    "', CardGroupID = '" + CardGroupID +
                    "', ExpireDate = '" + ExpireDate +
                    "', IsLock = " + (bool.Parse(IsLock) ? 1 : 0) +
                    " where CardID = '" + Id + "'", ref result))
                    return "true";
                else
                    return result;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
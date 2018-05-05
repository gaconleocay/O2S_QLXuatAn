using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using Futech.Tools;
using System.Web.Script.Serialization;

public partial class QLXuatAn_ReportFilter : System.Web.UI.Page
{
    public string PageName = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.QueryString["PageName"] != null)
                {
                    PageName = Request.QueryString["PageName"].ToString();
                }

                // Loai the
                DataTable dtCardType = StaticPool.mdb.FillData("select * from tblCardType order by Id");
                cbCardType.Items.Add(new ListItem("", ""));
                if (dtCardType != null && dtCardType.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCardType.Rows)
                    {
                        cbCardType.Items.Add(new ListItem(dr["Name"].ToString(), dr["Code"].ToString()));
                    }
                }

                if (Session["CardType"] != null)
                    cbCardType.Value = Session["CardType"].ToString();

                // Nhom san pham
                DataTable dtProductCategory = StaticPool.mdb.FillData("select * from tblProductCategory order by Id");
                cbProductCategory.Items.Add(new ListItem("", ""));
                if (dtProductCategory != null && dtProductCategory.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtProductCategory.Rows)
                    {
                        cbProductCategory.Items.Add(new ListItem(dr["Name"].ToString(), dr["Id"].ToString()));
                    }
                }

                if (Session["ProductCategory"] != null)
                {
                    cbProductCategory.Value = Session["ProductCategory"].ToString();
                    DataTable dt = StaticPool.mdb.FillData("select * from tblProduct where ProductCategoryID='" + cbProductCategory.Value + "' order by Id");
                    cbProduct.Items.Add(new ListItem("", ""));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            cbProduct.Items.Add(new ListItem(dr["Name"].ToString(), dr["Code"].ToString()));
                        }
                        if (Session["Product"] != null)
                            cbProduct.Value = Session["Product"].ToString();
                    }
                }

                // Nguoi dung
                DataTable dtUser = StaticPool.mdb.FillData("select * from tblUser order by Id");
                cbUser.Items.Add(new ListItem("", ""));
                if (dtUser != null && dtUser.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtUser.Rows)
                    {
                        cbUser.Items.Add(new ListItem(dr["FullName"].ToString(), dr["Code"].ToString()));
                    }
                }

                if (Session["UserCode"] != null)
                    cbUser.Value = Session["UserCode"].ToString();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    public string FromDate()
    {
        try
        {
            if (Session["FromDate"] != null && Session["FromDate"].ToString() != "")
                return DateTime.Parse(Session["FromDate"].ToString()).ToString("dd/MM/yyyy");
            else
                return DateTime.Now.ToString("dd/MM/yyyy");
        }
        catch
        {
            return DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    public string ToDate()
    {
        try
        {
            if (Session["ToDate"] != null && Session["ToDate"].ToString() != "")
                return DateTime.Parse(Session["ToDate"].ToString()).ToString("dd/MM/yyyy");
            else
                return DateTime.Now.ToString("dd/MM/yyyy");
        }
        catch
        {
            return DateTime.Now.ToString("dd/MM/yyyy");
        }
    }

    [WebMethod()]
    public static string GetProductByProductCategory(string ProductCategoryID)
    {
        List<ListItem> list = new List<ListItem>();
        DataTable dt = StaticPool.mdb.FillData("select * from tblProduct where ProductCategoryID='" + ProductCategoryID + "' order by Id");
        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                ListItem item = new ListItem();
                item.Value = row["Code"].ToString();
                item.Text = row["Name"].ToString();
                list.Add(item);
            }
        }
        System.Web.Script.Serialization.JavaScriptSerializer jsonSerialiser = new JavaScriptSerializer();

        return jsonSerialiser.Serialize(list).ToString();
    }
}
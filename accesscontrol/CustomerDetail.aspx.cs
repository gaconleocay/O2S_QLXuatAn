using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;
using System.Xml;

public partial class accesscontrol_CustomerDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                ViewState["NewID"] = "";
                ViewState["CurrentID"] = "";
                ViewState["Avatar"] = "";
                id_avatar.Value = "";

                DisplayCustomerGroup();

                div_alert.Visible = false;
                if (Request.QueryString["CustomerID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Customer", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_customerdetail.InnerText = "Sửa khách hàng";
                    ViewState["CustomerID"] = Request.QueryString["CustomerID"].ToString();
                    ViewState["CurrentID"] = Request.QueryString["CustomerID"].ToString();
                    DataTable dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer where CustomerID = '" + ViewState["CustomerID"].ToString() + "'");
                    if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                    {
                        DataRow dr = dtCustomer.Rows[0];
                        txtCustomerCode.Text = dr["CustomerCode"].ToString();
                        txtCustomerName.Text = dr["CustomerName"].ToString();
                        txtAddress.Text = dr["Address"].ToString();
                        txtIDNumber.Text = dr["IDNumber"].ToString();
                        txtMobile.Text = dr["Mobile"].ToString();
                        cbCustomerGroup.Value = dr["CustomerGroupID"].ToString();
                        txtDescription.Text = dr["Description"].ToString();

                        chbEnableAccount.Checked = bool.Parse(dr["EnableAccount"].ToString());
                        ViewState["Account"] = dr["Account"].ToString();
                        txtAccount.Text = dr["Account"].ToString();
                        txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                        txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));

                        if (dr["Avatar"].ToString() != "")
                        {
                            picAvatar.HRef = dr["Avatar"].ToString();
                            preViewAvatar.Src = dr["Avatar"].ToString();
                            ViewState["Avatar"] = dr["Avatar"].ToString();
                            id_avatar.Value = dr["Avatar"].ToString();
                        }
                        else
                        {
                            picAvatar.HRef = "../assets/avatars/noPhotoAvailable.jpg";
                            preViewAvatar.Src = "../assets/avatars/noPhotoAvailable.jpg";
                        }

                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());
                    }

                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Customer", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    ViewState["NewID"] = Guid.NewGuid().ToString();
                    ViewState["CurrentID"] = ViewState["NewID"].ToString();
                    ViewState["CustomerID"] = null;

                    id_customerdetail.InnerText = "Thêm khách hàng mới";
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

    private void CreateXmlDocumentFromDataTable(DataTable dt, string parentID, XmlNode parentNode, ref XmlDocument XDoc, ref DataTable itemTable)
    {
        try
        {
            DataRow[] drs;
            if (parentID != "")
                drs = dt.Select("ParentID = '" + parentID.ToString() + "'");
            else
                drs = dt.Select("ParentID is null or ParentID = ''");

            foreach (DataRow row in drs)
            {
                CreateXmlDocumentFromDataTable(dt, row["CustomerGroupID"].ToString(), InsertNode(row, parentNode, ref XDoc, ref itemTable), ref XDoc, ref itemTable);
            }
        }
        catch (Exception ex)
        {

        }
    }

    public XmlNode InsertNode(DataRow Row, XmlNode parentNode, ref XmlDocument XDoc, ref DataTable itemTable)
    {
        try
        {
            XmlElement NewNode = XDoc.CreateElement("_" + Row["CustomerGroupID"].ToString());
            NewNode.SetAttribute("CustomerGroupID", Row["CustomerGroupID"].ToString());
            NewNode.SetAttribute("CustomerGroupName", Row["CustomerGroupName"].ToString());
            NewNode.SetAttribute("ParentID", Row["ParentID"].ToString());
            NewNode.SetAttribute("Description", Row["Description"].ToString());
            NewNode.SetAttribute("Inactive", Row["Inactive"].ToString());

            itemTable.Rows.Add(new string[] { Row["CustomerGroupID"].ToString(), Row["CustomerGroupName"].ToString(), Row["ParentID"].ToString(), Row["Description"].ToString(), Row["Inactive"].ToString() });
            if (parentNode == null)
                XDoc.AppendChild(NewNode);  // root node
            else
                parentNode.AppendChild(NewNode);
        }
        catch (Exception ex)
        {

        }
        return XDoc.SelectSingleNode(String.Format("//*[@CustomerGroupID=\"{0}\"]", Row["CustomerGroupID"].ToString()));
    }

    public void DisplayCustomerGroup()
    {
        DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup order by CustomerGroupID");
        // create an XmlDocument (with an XML declaration)
        XmlDocument XDoc = new XmlDocument();
        XmlDeclaration XDec = XDoc.CreateXmlDeclaration("1.0", null, null);
        XDoc.AppendChild(XDec);

        DataTable itemTable = new DataTable("CustomerGroup");
        itemTable.Columns.Add("CustomerGroupID");
        itemTable.Columns.Add("CustomerGroupName");
        itemTable.Columns.Add("ParentID");
        itemTable.Columns.Add("Description");
        itemTable.Columns.Add("Inactive");

        CreateXmlDocumentFromDataTable(dtCustomerGroup, "", null, ref XDoc, ref itemTable);

        // we cannot bind the TreeView directly to an XmlDocument
        // so we must create an XmlDataSource and assign the XML text
        XmlDataSource XDdataSource = new XmlDataSource();
        XDdataSource.ID = DateTime.Now.Ticks.ToString();  // unique ID is required
        XDdataSource.Data = XDoc.OuterXml;

        cbCustomerGroup.DataSource = itemTable;
        cbCustomerGroup.DataTextField = "CustomerGroupName";
        cbCustomerGroup.DataValueField = "CustomerGroupID";
        cbCustomerGroup.DataBind();
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            div_alert.Visible = false;

            if (txtCustomerName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên cổng không được để trống!";
                return;
            }

            if (chbEnableAccount.Checked && txtPassword.Text != txtRePassword.Text)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mật khẩu không trùng nhau!";
                return;
            }
            else if(!chbEnableAccount.Checked)
            {
                txtAccount.Text = "";
                txtPassword.Text = "";
            }

            DataTable dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer where Account = N'" + txtAccount.Text + "'");
            if (chbEnableAccount.Checked && dtCustomer != null && dtCustomer.Rows.Count > 0 && (ViewState["Account"] == null || txtAccount.Text != ViewState["Account"].ToString()))
            {
                txtPassword.Attributes.Add("value", txtPassword.Text);
                txtRePassword.Attributes.Add("value", txtRePassword.Text);

                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác!";
                return;
            }

            if (ViewState["CustomerID"] != null && ViewState["CustomerID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblCustomer set Description = N'" + txtDescription.Text +
                    "', CustomerName = N'" + txtCustomerName.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    ", CustomerCode = '" + txtCustomerCode.Text + 
                    "', Address = '" + txtAddress.Text + 
                    "', IDNumber = '" + txtIDNumber.Text + 
                    "', Mobile = '" + txtMobile.Text + 
                    "', CustomerGroupID = '" + cbCustomerGroup.Value +
                    "', EnableAccount = " + (chbEnableAccount.Checked ? 1 : 0) +
                    ", Account = '" + txtAccount.Text +
                    "', Password = '" + Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) +
                    "', Avatar = '" + id_avatar.Value + 
                    "' where CustomerID = '" + ViewState["CustomerID"].ToString() + "'", ref result);
            }
            else
            {
                // Them moi
                StaticPool.mdb.ExecuteCommand("insert into tblCustomer (CustomerID, Description, CustomerName, Inactive, CustomerCode, Address, IDNumber, Mobile, CustomerGroupID, EnableAccount, Account, Password, Avatar) values(N'" +
                    ViewState["NewID"] + "', '" + 
                    txtDescription.Text + "', N'" +
                    txtCustomerName.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) + ", '" + 
                    txtCustomerCode.Text + "', '" + 
                    txtAddress.Text + "', '" +
                    txtIDNumber.Text + "', '" +
                    txtMobile.Text + "', '" + 
                    cbCustomerGroup.Value + "', " + 
                    (chbEnableAccount.Checked?1:0) + ", '" +
                    txtAccount.Text + "', '" +
                    Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) + "', '" +
                    id_avatar.Value + 
                    "')", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
            {
                Response.Redirect("Customer.aspx");
            }
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

    protected void Cancel(object sender, EventArgs e)
    {
        try
        {
            // Xoa the da tao ra
            if (ViewState["NewID"]!=null && ViewState["NewID"].ToString() != "")
            {
                StaticPool.mdb.ExecuteCommand("delete from tblCard where CustomerID = '" + ViewState["NewID"].ToString() + "'");
            }
            Response.Redirect("Customer.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
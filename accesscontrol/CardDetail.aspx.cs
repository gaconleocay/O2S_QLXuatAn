using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;
using System.Xml;

public partial class accesscontrol_CardDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;

                ViewState["Avatar"] = "";
                DisplayCustomerGroup();

                // Nhom the
                cbCardGroup.DataSource = StaticPool.mdb.FillData("select * from tblCardGroup");
                cbCardGroup.DataTextField = "CardGroupName";
                cbCardGroup.DataValueField = "CardGroupID";
                cbCardGroup.DataBind();

                dtpExpireDate.Value = DateTime.Now.AddMonths(6).ToString("dd/MM/yyyy");

                if (Request.QueryString["CardID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Card", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    ViewState["CardID"] = Request.QueryString["CardID"].ToString();
                    id_carddetail.InnerText = "Sửa thẻ";
                    DataTable dt = StaticPool.mdb.FillData("select * from tblCard where CardID = '" + ViewState["CardID"] + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        txtCardNo.Value = dr["CardNo"].ToString();
                        txtCardNumber.Value = dr["CardNumber"].ToString();
                        cbCardGroup.Value = dr["CardGroupID"].ToString();
                        if (dr["ExpireDate"].ToString() != "")
                            dtpExpireDate.Value = DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy");
                        chbIsLock.Checked = bool.Parse(dr["IsLock"].ToString());
                        ViewState["CustomerID"] = dr["CustomerID"].ToString();

                        // Thong tin khach hang
                        DataTable dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer where CustomerID = '" + ViewState["CustomerID"].ToString() + "'");
                        if (ViewState["CustomerID"].ToString() != "" && dtCustomer != null && dtCustomer.Rows.Count > 0)
                        {
                            DataRow drCustomer = dtCustomer.Rows[0];
                            txtCustomerCode.Text = drCustomer["CustomerCode"].ToString();
                            txtCustomerName.Text = drCustomer["CustomerName"].ToString();
                            txtAddress.Text = drCustomer["Address"].ToString();
                            txtIDNumber.Text = drCustomer["IDNumber"].ToString();
                            txtMobile.Text = drCustomer["Mobile"].ToString();
                            cbCustomerGroup.Value = drCustomer["CustomerGroupID"].ToString();
                            txtDescription.Text = drCustomer["Description"].ToString();

                            chbEnableAccount.Checked = bool.Parse(drCustomer["EnableAccount"].ToString());
                            ViewState["Account"] = drCustomer["Account"].ToString();
                            txtAccount.Text = drCustomer["Account"].ToString();
                            txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(drCustomer["Password"].ToString(), true));
                            txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(drCustomer["Password"].ToString(), true));

                            if (drCustomer["Avatar"].ToString() != "")
                            {
                                picAvatar.HRef = drCustomer["Avatar"].ToString();
                                preViewAvatar.Src = drCustomer["Avatar"].ToString();
                                ViewState["Avatar"] = drCustomer["Avatar"].ToString();
                                id_avatar.Value = drCustomer["Avatar"].ToString();
                            }
                            else
                            {
                                picAvatar.HRef = "../assets/avatars/noPhotoAvailable.jpg";
                                preViewAvatar.Src = "../assets/avatars/noPhotoAvailable.jpg";
                            }

                            chbInactive.Checked = bool.Parse(drCustomer["Inactive"].ToString());
                        }
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_Card", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_carddetail.InnerText = "Thêm thẻ mới";
                    txtCardNo.Value = "";
                    txtCardNumber.Value = "";
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

    //protected void Selection_ChangeCustomer(object sender, EventArgs e)
    //{


    //}

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
            string ExpireDate = "";
            ExpireDate = dtpExpireDate.Value.Substring(6, 4) + "/" + dtpExpireDate.Value.Substring(3, 2) + "/" + dtpExpireDate.Value.Substring(0, 2);

            string NewCustomerID = "";

            if (txtCardNumber.Value == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mã thẻ không được để trống!";
                return;
            }

            DataTable dtCard = StaticPool.mdb.FillData("select * from tblCard where CardNumber = '" + txtCardNumber.Value + "'");
            if (dtCard != null && dtCard.Rows.Count > 0 && ((ViewState["CardID"] != null && dtCard.Rows[0]["CardID"].ToString() != ViewState["CardID"].ToString() && ViewState["CardID"].ToString() != "") || ViewState["CardID"] == null || ViewState["CardID"].ToString() == ""))
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mã thẻ đã khai báo! Vui lòng nhập mã thẻ khác.";
                return;
            }

            if ((ViewState["CustomerID"] == null || ViewState["CustomerID"].ToString() == "") && txtCustomerName.Text != "")
            {
                // them moi khach hang
                NewCustomerID = Guid.NewGuid().ToString();
                InsertCustomer(NewCustomerID);
                ViewState["CustomerID"] = NewCustomerID;

            }
            else if (ViewState["CustomerID"] != null && ViewState["CustomerID"].ToString() != "")
            {
                // update
                UpdateCustomer(ViewState["CustomerID"].ToString());
            }

            if (ViewState["CardID"] == null || ViewState["CardID"].ToString() == "")
            {
                // insert
                StaticPool.mdb.ExecuteCommand("insert into tblCard (CardNo, CardNumber, CardGroupID, CustomerID, ExpireDate, IsLock) values('" + txtCardNo.Value +
                    "', '" + txtCardNumber.Value +
                    "', '" + cbCardGroup.Value +
                    "', '" + NewCustomerID +
                    "', '" + ExpireDate +
                    "', " + (chbIsLock.Checked ? 1 : 0) +
                    ")", ref result);
            }
            else
            {

                // update
                StaticPool.mdb.ExecuteCommand("update tblCard set CardNo = '" + txtCardNo.Value +
                    "', CardNumber = '" + txtCardNumber.Value +
                    "', CardGroupID = '" + cbCardGroup.Value +
                    "', CustomerID = '" + ViewState["CustomerID"].ToString() +
                    "', ExpireDate = '" + ExpireDate +
                    "', IsLock = " + (chbIsLock.Checked ? 1 : 0) +
                    " where CardID = '" + ViewState["CardID"].ToString() + "'", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Card.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

    private string InsertCustomer(string CustomerID)
    {
        string result = "";

        // Them moi
        StaticPool.mdb.ExecuteCommand("insert into tblCustomer (CustomerID, Description, CustomerName, Inactive, CustomerCode, Address, IDNumber, Mobile, CustomerGroupID, EnableAccount, Account, Password, Avatar) values(N'" +
                    CustomerID + "', '" +
                    txtDescription.Text + "', N'" +
                    txtCustomerName.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) + ", '" +
                    txtCustomerCode.Text + "', '" +
                    txtAddress.Text + "', '" +
                    txtIDNumber.Text + "', '" +
                    txtMobile.Text + "', '" +
                    cbCustomerGroup.Value + "', " +
                    (chbEnableAccount.Checked ? 1 : 0) + ", '" +
                    txtAccount.Text + "', '" +
                    Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) + "', '" +
                    id_avatar.Value +
                    "')", ref result);

        return result;
    }

    private string UpdateCustomer(string CustomerID)
    {
        string result = "";

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
            "' where CustomerID = '" + CustomerID + "'", ref result);

        return result;
    }

}
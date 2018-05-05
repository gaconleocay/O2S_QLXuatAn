using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using Futech.Helpers;

public partial class QLXuatAn_CustomerDetail : System.Web.UI.Page
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

                ViewState["OldCustomer"] = "";

                //--
                DisplayCustomerGroup();
                DisplayCompartment();

                div_alert.Visible = false;
                if (Request.QueryString["CustomerID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Customer", "Updates", "Parking"))
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
                        cbCompartment.Value = dr["CompartmentId"].ToString();
                        txtDescription.Text = dr["Description"].ToString();

                        chbEnableAccount.Checked = bool.Parse(dr["EnableAccount"].ToString());
                        ViewState["Account"] = dr["Account"].ToString();
                        txtAccount.Text = dr["Account"].ToString();
                        try
                        {
                            txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                            txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(dr["Password"].ToString(), true));
                        }
                        catch
                        { }

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

                        ViewState["OldCustomer"] = txtCustomerCode.Text +
                            ";" + txtCustomerName.Text +
                            ";" + txtAddress.Text +
                            ";" + txtIDNumber.Text +
                            ";" + txtMobile.Text +
                            ";" + cbCustomerGroup.Items[cbCustomerGroup.SelectedIndex].Text +
                            ";" + txtDescription.Text +
                            ";" + chbEnableAccount.Checked.ToString() +
                            ";" + txtAccount.Text +
                            ";" + chbInactive.Checked.ToString();
                    }

                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Customer", "Inserts", "Parking"))
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
        DataTable dtCustomerGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCustomerGroup);
        if (dtCustomerGroup == null)
        {
            dtCustomerGroup = StaticPool.mdb.FillData("select CustomerGroupID,ParentID,CustomerGroupCode, CustomerGroupName, Description, Inactive, SortOrder from tblCustomerGroup order by SortOrder");
            if (dtCustomerGroup!=null && dtCustomerGroup.Rows.Count > 0)
                CacheLayer.Add(StaticCached.c_tblCustomerGroup, dtCustomerGroup, StaticCached.TimeCache);
        }
        //DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup order by CustomerGroupID");
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

    public void DisplayCompartment()
    {
        DataTable dtGroup = StaticPool.mdb.FillData("select * from tblCompartment order by SortOrder asc");

        if (dtGroup != null && dtGroup.Rows.Count > 0)
        {
            cbCompartment.Items.Add(new ListItem("<< Tất cả căn hộ >>", ""));
            foreach (DataRow dr in dtGroup.Rows)
            {
                cbCompartment.Items.Add(new ListItem(dr["CompartmentName"].ToString(), dr["CompartmentID"].ToString()));
            }
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            div_alert.Visible = false;

            if (txtCustomerName.Text == ""||txtCustomerCode.Text=="")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mã, tên khách hàng không được để trống!";
                return;
            }

            //if (string.IsNullOrWhiteSpace(cbCompartment.Value))
            //{
            //    // Hien thi thong bao loi
            //    div_alert.Visible = true;
            //    id_alert.InnerText = "Vui lòng nhập căn hộ của khách hàng";
            //    return;
            //}

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
                DataTable temp = StaticPool.mdb.FillData("select CustomerCode from tblCustomer where CustomerCode =N'" + txtCustomerCode.Text + "' and CustomerID<>'" + ViewState["CustomerID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Mã khách hàng đã tồn tại";
                    return;
                }
                // Update
                if (StaticPool.mdb.ExecuteCommand("update tblCustomer set Description = N'" + txtDescription.Text +
                    "', CustomerName = N'" + txtCustomerName.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    ", CustomerCode = N'" + txtCustomerCode.Text +
                    "', Address = N'" + txtAddress.Text +
                    "', IDNumber = '" + txtIDNumber.Text +
                    "', Mobile = '" + txtMobile.Text +
                    "', CustomerGroupID = '" + cbCustomerGroup.Value +
                    "', CompartmentId = '" + cbCompartment.Value +
                    "', EnableAccount = " + (chbEnableAccount.Checked ? 1 : 0) +
                    ", Account = '" + txtAccount.Text +
                    "', Password = '" + Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) +
                    "', Avatar = '" + id_avatar.Value +
                    "' where CustomerID = '" + ViewState["CustomerID"].ToString() + "'", ref result))
                {
                    string _newcustomer = txtCustomerCode.Text +
                            ";" + txtCustomerName.Text +
                            ";" + txtAddress.Text +
                            ";" + txtIDNumber.Text +
                            ";" + txtMobile.Text +
                            ";" + cbCustomerGroup.Items[cbCustomerGroup.SelectedIndex].Text +
                            ";" + txtDescription.Text +
                            ";" + chbEnableAccount.Checked.ToString() +
                            ";" + txtAccount.Text +
                            ";" + chbInactive.Checked.ToString();
                    string _des = StaticPool.GetStringChange(ViewState["OldCustomer"].ToString(), _newcustomer);
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Customer", txtCustomerName.Text, "Sửa", _des);

                }
            }
            else
            {
                DataTable temp = StaticPool.mdb.FillData("select CustomerCode from tblCustomer where CustomerCode =N'" + txtCustomerCode.Text + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Mã khách hàng đã tồn tại";
                    return;
                }
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblCustomer (CustomerID, Description, CustomerName, Inactive, CustomerCode, Address, IDNumber, Mobile, CustomerGroupID, EnableAccount, Account, Password, Avatar,CompartmentId) values(N'" +
                     ViewState["NewID"] + "', N'" +
                     txtDescription.Text + "', N'" +
                     txtCustomerName.Text + "', " +
                     (chbInactive.Checked ? 1 : 0) + ", N'" +
                     txtCustomerCode.Text + "', N'" +
                     txtAddress.Text + "', '" +
                     txtIDNumber.Text + "', '" +
                     txtMobile.Text + "', '" +
                     cbCustomerGroup.Value + "', " +
                     (chbEnableAccount.Checked ? 1 : 0) + ", '" +
                     txtAccount.Text + "', '" +
                     Futech.Tools.CryptorEngine.Encrypt(txtPassword.Text, true) + "', '" +
                     id_avatar.Value + "', '" +
                     cbCompartment.Value +
                     "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Customer", txtCustomerName.Text, "Thêm", "");

                }
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
            // Xoa the da tao ra, neu la the nhap tay. 
            // Neu la the nhap tu danh sach the thi sao
            if (ViewState["NewID"]!=null && ViewState["NewID"].ToString() != "")
            {
               DataTable dt = StaticPool.mdb.FillData("select * from tblCard where CustomerID='" + ViewState["NewID"].ToString() + "'");
               if (dt != null && dt.Rows.Count > 0)
               {
                   foreach (DataRow dr in dt.Rows)
                   {
                       string _cardnumber = dr["CardNumber"].ToString();
                       string _id = dr["CardID"].ToString();
                       string _description = dr["Description"].ToString();
                       string _cardgroupid = dr["CardGroupID"].ToString();
                       if (_description == "Manual")
                       {
                           DeleteInvalidCardInLogCardCustomer(dr["CardNumber"].ToString());

                           if (StaticPool.mdb.ExecuteCommand("delete from tblCard where CardID = '" + _id + "'"))
                           {
                               //delete card
                               StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID) values('" +
                                 DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                                 _cardnumber + "', '" +
                                 "DELETE" + "', '" +
                                 _cardgroupid + "', '" +
                                 Request.Cookies["UserID"].Value.ToString() +
                                 "')");

                               //log

                               StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Card", _cardnumber, "Xóa", "id=" + _id);


                           }
                       }
                       else
                       {
                           if (StaticPool.mdb.ExecuteCommand("update tblCard set CustomerID='' where CardID='" + _id + "'"))
                           {
                               //return
                               StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                                  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                                  _cardnumber + "', '" +
                                  "RETURN" + "', '" +
                                  "" + "', '" +
                                  Request.Cookies["UserID"].Value.ToString() + "', '" +
                                  ViewState["NewID"].ToString() +
                                  "')");

                               //log

                               StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Card", _cardnumber, "Sửa", "return_card");
                           }
                       }
                   }
                   
               }
              
               
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

    private void DeleteInvalidCardInLogCardCustomer(string cardnumber) {
        string command = string.Format("delete from tblLogCardCustomer where CardNumber like '{0}'", cardnumber);
        StaticPool.mdb.ExecuteCommand(command);
    }
}
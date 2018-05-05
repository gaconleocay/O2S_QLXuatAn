using System;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;
using System.Xml;
using Futech.Helpers;
using System.Web.Services;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;

public partial class QLXuatAn_CardDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ViewState["First"] = "0";
        if (!IsPostBack)
        {
            try
            {
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";

                boxNewDateTime.Visible = false;

                div_alert.Visible = false;

                ViewState["CardDetailID"] = "";

                if (Request.QueryString["CardID"] != null)
                {
                    boxRegisterDate.Visible = true;
                    boxReleaseDate.Visible = true;
                    ViewState["CardDetailID"] = "true";
                }
                else
                {
                    boxRegisterDate.Visible = false;
                    boxReleaseDate.Visible = false;
                    ViewState["CardDetailID"] = "false";
                }

                ViewState["Avatar"] = "";
                DisplayCustomerGroup();

                ViewState["First"] = "1";


                // Nhom the
                DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
                if (dtCardGroup == null)
                {
                    dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
                    if (dtCardGroup.Rows.Count > 0)
                        CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
                }
                cbCardGroup.DataSource = dtCardGroup; //StaticPool.mdb.FillData("select * from tblCardGroup");
                cbCardGroup.DataTextField = "CardGroupName";
                cbCardGroup.DataValueField = "CardGroupID";
                cbCardGroup.DataBind();

                // Căn

                DataTable dtCompartment = StaticPool.mdb.FillData("select * from tblCompartment order by SortOrder asc");
                cbCompartment.Items.Add(new ListItem("<< >>", ""));
                if (dtCompartment != null && dtCompartment.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtCompartment.Rows)
                    {
                        cbCompartment.Items.Add(new ListItem(dr["CompartmentName"].ToString(), dr["CompartmentID"].ToString()));
                    }
                }

                //// Khach hang
                //DataTable dtCustomer = StaticPool.mdb.FillData("select * from tblCustomer order by SortOrder");
                //cbCustomer.Items.Add(new ListItem("<< >>", ""));
                //if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                //{
                //    foreach (DataRow dr in dtCustomer.Rows)
                //    {
                //        cbCustomer.Items.Add(new ListItem(dr["CustomerName"].ToString(), dr["CustomerID"].ToString()));
                //    }
                //}

                dtpExpireDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["ExpireDate"] = dtpExpireDate.Value;

                dtpRegisterDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["DateRegister"] = dtpRegisterDate.Value;

                dtpReleaseDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                ViewState["DateRelease"] = dtpReleaseDate.Value;


                //dtpNewRegisterDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                //ViewState["NewDateRegister"] = dtpNewRegisterDate.Value;

                //dtpNewReleaseDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                //ViewState["NewDateRelease"] = dtpNewReleaseDate.Value;


                hidRegisterDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                hidReleaseDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                hidCustomer.Value = "";

                if (Request.QueryString["CardID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Card", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    ViewState["CardID"] = Request.QueryString["CardID"].ToString();
                    id_carddetail.InnerText = "Sửa thẻ";
                    DataTable dt = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and  CardID = '" + ViewState["CardID"] + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        txtCardNo.Value = dr["CardNo"].ToString();
                        txtCardNumber.Value = dr["CardNumber"].ToString();
                        ViewState["CardNumber"] = dr["CardNumber"].ToString();
                        cbCardGroup.Value = dr["CardGroupID"].ToString();

                        if (dr["ExpireDate"].ToString() != "")
                        {
                            dtpExpireDate.Value = DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy");
                            ViewState["ExpireDate"] = dtpExpireDate.Value;
                        }

                        if (dr["DateRegister"].ToString() != "")
                        {
                            dtpRegisterDate.Value = DateTime.Parse(dr["DateRegister"].ToString()).ToString("dd/MM/yyyy");
                            ViewState["DateRegister"] = dtpRegisterDate.Value;
                            hidRegisterDate.Value = DateTime.Parse(dr["DateRegister"].ToString()).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dtpRegisterDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                            ViewState["DateRegister"] = dtpRegisterDate.Value;
                            hidRegisterDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                        }

                        if (dr["DateRelease"].ToString() != "")
                        {
                            dtpReleaseDate.Value = DateTime.Parse(dr["DateRelease"].ToString()).ToString("dd/MM/yyyy");
                            ViewState["DateRelease"] = dtpReleaseDate.Value;
                            hidReleaseDate.Value = DateTime.Parse(dr["DateRelease"].ToString()).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dtpReleaseDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                            ViewState["DateRelease"] = dtpReleaseDate.Value;
                            hidReleaseDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                        }
                        
                        DataTable objCustomer = StaticPool.mdb.FillData("select * from tblCustomer where CustomerID = '" + dr["CustomerID"].ToString() + "'");

                        chbIsLock.Checked = bool.Parse(dr["IsLock"].ToString());
                        txtPlate1.Value = dr["Plate1"].ToString();
                        txtVehicleName1.Value = dr["VehicleName1"].ToString();
                        txtPlate2.Value = dr["Plate2"].ToString();
                        txtVehicleName2.Value = dr["VehicleName2"].ToString();
                        txtPlate3.Value = dr["Plate3"].ToString();
                        txtVehicleName3.Value = dr["VehicleName3"].ToString();
                        ViewState["CustomerID"] = dr["CustomerID"].ToString();

                        if (objCustomer != null && objCustomer.Rows.Count > 0)
                        {
                            ViewState["CompartmentID"] = objCustomer.Rows[0]["CompartmentId"];
                        }
                        else
                        {
                            ViewState["CompartmentID"] = "";
                        }

                        //cbCustomer.SelectedValue = ViewState["CustomerID"].ToString();
                        cbCompartment.Value = ViewState["CompartmentID"].ToString();
                        txtCardNumber.Attributes.Add("readonly", "readonly");
                        dtpExpireDate.Attributes.Add("disabled", "true");

                        //get all customer Info
                        Selection_ChangeCustomer(null, null);
                        boxNewDateTime.Visible = false;


                        //Get List sub Card
                        var listSubCard = StaticPool.mdb.FillData(string.Format("SELECT * FROM [dbo].[tblSubCard] WHERE IsDelete=0 AND MainCard = '{0}'", dr["CardNumber"].ToString()));
                        if(listSubCard!=null && listSubCard.Rows.Count > 0)
                        {
                            rptListSubCard.DataSource = listSubCard;
                            rptListSubCard.DataBind();
                        }
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Card", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_carddetail.InnerText = "Thêm thẻ mới";
                    txtCardNo.Value = "";
                    txtCardNumber.Value = "";
                    ViewState["CustomerID"] = "";
                    ViewState["CompartmentID"] = "";
                }
            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                boxNewDateTime.Visible = false;
                id_alert.InnerText = ex.Message;
            }
        }
    }
    //HNG: lay thong tin khach hang bind vao form khi chon 1 khach hang dung autocomplete
    [WebMethod]
    public static string getCusInfo(string cusId)
    {
        if (string.IsNullOrWhiteSpace(cusId))
        {
            return StaticPool.DataTableToJsonObj(new DataTable());
        }

        string commandstring = "";
        commandstring = "select Description, LTRIM(RTRIM(CustomerName)) as CustomerName, Inactive,CustomerCode,Address,IDNumber,Mobile, CustomerGroupID,EnableAccount,Account, Password , Avatar from tblcustomer WITH(NOLOCK) where CustomerID ='" + cusId + "'";
        var str = "";
        var dt = StaticPool.mdb.FillData(commandstring);
        if (dt != null && dt.Rows.Count > 0)
        {
            foreach (DataRow item in dt.Rows)
            {
                if (item["Password"] != null && item["Password"].ToString() != "")
                    item["Password"] = Futech.Tools.CryptorEngine.Decrypt(item["Password"].ToString(), true);
            }
            str = StaticPool.DataTableToJsonObj(dt); //JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);
        }

        return str.Trim();
    }
    protected void Selection_ChangeCustomer(object sender, EventArgs e)
    {
        boxNewDateTime.Visible = true;
        if (!string.IsNullOrWhiteSpace(ViewState["CustomerID"].ToString()))
        {
            // Thong tin khach hang
            DataTable dtCustomerInfo = StaticPool.mdb.FillData("select  CustomerID,CustomerCode,CustomerName,Address,IDNumber,Mobile, CustomerGroupID, Description, EnableAccount, Account, Password, Avatar, Inactive, CompartmentId  from tblCustomer where CustomerID = '" + ViewState["CustomerID"] + "'");
            if (dtCustomerInfo != null && dtCustomerInfo.Rows.Count > 0)
            {
                DataRow drCustomer = dtCustomerInfo.Rows[0];
                ViewState["CustomerID"] = drCustomer["CustomerID"].ToString();
                //hidCustomer.Value = drCustomer["CustomerID"].ToString();
                ViewState["CompartmentID"] = drCustomer["CompartmentId"].ToString();
                txtCustomerCode.Text = drCustomer["CustomerCode"].ToString();
                txtCustomerName.Text = drCustomer["CustomerName"].ToString();
                txtAddress.Text = drCustomer["Address"].ToString();
                txtIDNumber.Text = drCustomer["IDNumber"].ToString();
                txtMobile.Text = drCustomer["Mobile"].ToString();
                cbCustomerGroup.Value = drCustomer["CustomerGroupID"].ToString();
                cbCompartment.Value = drCustomer["CompartmentId"].ToString();
                txtDescription.Text = drCustomer["Description"].ToString();

                chbEnableAccount.Checked = bool.Parse(drCustomer["EnableAccount"].ToString());
                ViewState["Account"] = drCustomer["Account"].ToString();
                txtAccount.Text = drCustomer["Account"].ToString();
                try
                {
                    txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(drCustomer["Password"].ToString(), true));
                    txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt(drCustomer["Password"].ToString(), true));
                }
                catch
                { }


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
        else
        {
            ViewState["CustomerID"] = "";
            ViewState["CompartmentID"] = "";
            txtCustomerCode.Text = "";
            txtCustomerName.Text = "";
            txtAddress.Text = "";
            txtIDNumber.Text = "";
            txtMobile.Text = "";
            cbCustomerGroup.Value = "";
            cbCompartment.Value = "";
            txtDescription.Text = "";
            //hidCustomer.Value = "";

            chbEnableAccount.Checked = false;
            ViewState["Account"] = "";
            txtAccount.Text = "";
            ////txtPassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt("", true));
            ////txtRePassword.Attributes.Add("value", Futech.Tools.CryptorEngine.Decrypt("", true));


            picAvatar.HRef = "../assets/avatars/noPhotoAvailable.jpg";
            preViewAvatar.Src = "../assets/avatars/noPhotoAvailable.jpg";

            chbInactive.Checked = false;
        }
        dtpExpireDate.Value = ViewState["ExpireDate"].ToString();


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
            if (dtCustomerGroup.Rows.Count > 0)
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
    protected void AddSubCard(object sender, EventArgs e)
    {
        ViewState["errSubCard"] = "";
        ViewState["First"] = "3";
        // thêm mới và sửa
        // -Add
        if (ViewState["CardID"] == null || string.IsNullOrWhiteSpace(ViewState["CardID"].ToString()))
        {
            if (!string.IsNullOrWhiteSpace(txtSubCardCode.Value))
            {
                Session["SubCard"] = txtSubCardCode.Value; // lưu khi thêm mới THẺ
                ViewState["errSubCard"] = "Đã lưu thông tin thẻ phụ";
            }
            else
            {
                ViewState["errSubCard"] = "Nhập thông tin thẻ phụ";
            }
        }
        else
        {
            //Lưu ngay khi bấm nút
            if (!string.IsNullOrWhiteSpace(txtSubCardCode.Value))
            {
                //Update Card
                var sb = new StringBuilder();
                sb.AppendLine("UPDATE [dbo].[tblSubCard]");
                sb.AppendLine(string.Format("SET MainCard = '{0}'", ViewState["CardNumber"]));
                sb.AppendLine(string.Format("WHERE CardNumber = '{0}'", txtSubCardCode.Value));
                if (StaticPool.mdb.ExecuteCommand(sb.ToString()))
                {
                    //Get List sub Card
                    var listSubCard = StaticPool.mdb.FillData(string.Format("SELECT * FROM [dbo].[tblSubCard] WHERE IsDelete=0 AND MainCard = '{0}'", ViewState["CardNumber"].ToString()));
                    if (listSubCard != null && listSubCard.Rows.Count > 0)
                    {
                        rptListSubCard.DataSource = listSubCard;
                        rptListSubCard.DataBind();
                    }

                    ViewState["errSubCard"] = "Thêm mới thẻ phụ thành công";
                }
                else
                {
                    ViewState["errSubCard"] = "Thẻ phụ không tồn tại";
                }

               
            }
            else
            {
                ViewState["errSubCard"] = "Nhập thông tin thẻ phụ";
            }
        }

        
    }
    [System.Web.Services.WebMethod]
    public static string DeleteSubCard(string subCardId)
    {
        var chk = "1";
        try
        {
            if (subCardId != "")
            {
                var str = string.Format("UPDATE [dbo].[tblSubCard] SET MainCard = '' WHERE Id = {0}", subCardId);
                StaticPool.mdb.ExecuteCommand(str);
            }
        }
        catch (Exception)
        {
            chk = "0";
        }

        return chk;
    }
    [WebMethod]
    public static List<ListItem> getCardNumberByAutocomplete(string name)
    {
        var list = new List<ListItem>();
        //var firstItem = new ListItem
        //{
        //    Value = "",
        //    Text = "Xóa khách hàng"
        //};
        //list.Add(firstItem);;
        string commandstring = "";
        if (!string.IsNullOrWhiteSpace(name))
            commandstring = "select top 10 CardNumber, CardNo from tblSubCard where CardNumber like N'%" + name + "%' OR CardNo LIKE N'%" + name + "%' order by CardNumber, CardNo ";
        else
            commandstring = "select top 10 CardNumber, CardNo from tblSubCard  order by CardNumber,CardNo";

        DataTable dt = StaticPool.mdb.FillData(commandstring);
        if (dt.Rows.Count != 0)
        {
            foreach (DataRow dr in dt.Rows)
            {
                ListItem item = new ListItem();
                item.Value = dr["CardNumber"].ToString();
                if (!string.IsNullOrWhiteSpace(dr["CardNumber"].ToString()))
                    item.Text = dr["CardNumber"].ToString().Trim();
                if (!string.IsNullOrWhiteSpace(dr["CardNo"].ToString()))
                {
                    item.Text = dr["CardNumber"] + @" / " + dr["CardNo"];
                }
                list.Add(item);
            }
        }

        return list; //new JavaScriptSerializer().Serialize(list).ToString();
    }
    protected void Save(object sender, EventArgs e)
    {
        boxNewDateTime.Visible = false;

        try
        {
            string result = "";
            string ExpireDate = "";
            string RegisterDate = "";
            string ReleaseDate = "";
            string CancelDate = "";
            if (!string.IsNullOrWhiteSpace(hidCustomer.Value) && hidCustomer.Value != "0")
            {
                ViewState["CustomerID"] = hidCustomer.Value;
            }

            //Expired
            if (dtpExpireDate.Value != "")
            {
                ExpireDate = Convert.ToDateTime(dtpExpireDate.Value).ToString("yyyy/MM/dd"); //dtpExpireDate.Value.Substring(6, 4) + "/" + dtpExpireDate.Value.Substring(3, 2) + "/" + dtpExpireDate.Value.Substring(0, 2);
            }
            else if (ViewState["ExpireDate"].ToString() != "")
            {
                ExpireDate = ViewState["ExpireDate"].ToString().Substring(6, 4) + "/" + ViewState["ExpireDate"].ToString().Substring(3, 2) + "/" + ViewState["ExpireDate"].ToString().Substring(0, 2);
            }

            //Register
            if (dtpRegisterDate.Value != "")
            {
                RegisterDate = Convert.ToDateTime(dtpRegisterDate.Value).ToString("yyyy/MM/dd"); //dtpRegisterDate.Value.Substring(6, 4) + "/" + dtpRegisterDate.Value.Substring(3, 2) + "/" + dtpRegisterDate.Value.Substring(0, 2);
            }
            else if (ViewState["DateRegister"].ToString() != "")
            {
                RegisterDate = ViewState["DateRegister"].ToString().Substring(6, 4) + "/" + ViewState["DateRegister"].ToString().Substring(3, 2) + "/" + ViewState["DateRegister"].ToString().Substring(0, 2);
            }

            //Release
            if (dtpReleaseDate.Value != "")
            {
                ReleaseDate = Convert.ToDateTime(dtpReleaseDate.Value).ToString("yyyy/MM/dd"); //dtpReleaseDate.Value.Substring(6, 4) + "/" + dtpReleaseDate.Value.Substring(3, 2) + "/" + dtpReleaseDate.Value.Substring(0, 2);
            }
            else if (ViewState["DateRelease"].ToString() != "")
            {
                ReleaseDate = ViewState["DateRelease"].ToString().Substring(6, 4) + "/" + ViewState["DateRelease"].ToString().Substring(3, 2) + "/" + ViewState["DateRelease"].ToString().Substring(0, 2);
            }

            string NewCustomerID = "";

            if (txtCardNumber.Value == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mã thẻ không được để trống!";
                return;
            }

            //if (string.IsNullOrWhiteSpace(cbCompartment.Value))
            //{
            //    // Hien thi thong bao loi
            //    div_alert.Visible = true;
            //    id_alert.InnerText = "Vui lòng chọn căn hộ của khách hàng!";
            //    return;
            //}

            string reportmessage = "";
            bool isValid = ValidDate(RegisterDate, ReleaseDate, ref reportmessage);
            if (!isValid)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = reportmessage;
                return;
            }

            DataTable dtCard = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and CardNumber = '" + txtCardNumber.Value + "'");
            if (dtCard != null && dtCard.Rows.Count > 0 && ((ViewState["CardID"] != null && dtCard.Rows[0]["CardID"].ToString() != ViewState["CardID"].ToString() && ViewState["CardID"].ToString() != "") || ViewState["CardID"] == null || ViewState["CardID"].ToString() == ""))
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Mã thẻ đã khai báo! Vui lòng nhập mã thẻ khác.";
                return;
            }


            if ((ViewState["CustomerID"] == null || ViewState["CustomerID"].ToString() == "" || ViewState["CustomerID"].ToString() == "0") && txtCustomerName.Text != "" && (hidCustomer.Value == "" || hidCustomer.Value == "0") && txtCustomerCode.Text != "")
            {
                DataTable temp = StaticPool.mdb.FillData("select CustomerID from tblCustomer where CustomerCode=N'" + txtCustomerCode.Text + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Mã KH đã tồn tại";
                    return;
                }
                // them moi khach hang
                NewCustomerID = Guid.NewGuid().ToString();
                InsertCustomer(NewCustomerID);
                ViewState["CustomerID"] = NewCustomerID;

            }
            else if (ViewState["CustomerID"] != null && ViewState["CustomerID"].ToString() != "" && ViewState["CustomerID"].ToString() != "0")
            {
                DataTable temp = StaticPool.mdb.FillData("select CustomerCode from tblCustomer where CustomerCode =N'" + txtCustomerCode.Text + "' and CustomerID<>'" + ViewState["CustomerID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    div_alert.Visible = true;
                    id_alert.InnerText = "Mã khách hàng đã tồn tại";
                    return;
                }
                // update
                UpdateCustomer(ViewState["CustomerID"].ToString());
            }

           

            if (ViewState["CardID"] == null || ViewState["CardID"].ToString() == "")
            {
                //CancelDate
                CancelDate = chbIsLock.Checked ? DateTime.Now.ToString("yyyy/MM/dd") : null;

                // insert
                if (StaticPool.mdb.ExecuteCommand("insert into tblCard (CardNo, CardNumber, CardGroupID, CustomerID, [ExpireDate], IsLock, Plate1, VehicleName1, Plate2, VehicleName2, Plate3, VehicleName3, ImportDate, [Description], DateRegister, DateRelease,DateCancel) values('" + txtCardNo.Value +
                     "', '" + txtCardNumber.Value +
                     "', '" + cbCardGroup.Value +
                     "', '" + ViewState["CustomerID"].ToString() +
                     "', '" + ExpireDate +
                     "', " + (chbIsLock.Checked ? 1 : 0) +
                     ", N'" + txtPlate1.Value +
                     "', N'" + txtVehicleName1.Value +
                     "', N'" + txtPlate2.Value +
                     "', N'" + txtVehicleName2.Value +
                     "', N'" + txtPlate3.Value +
                     "', N'" + txtVehicleName3.Value +
                     "', '" + DateTime.Now.ToString("yyyy/MM/dd") +
                     "', '" + "Manual" +
                     "', '" + RegisterDate +
                     "', '" + ReleaseDate +
                     string.Format("{0}", !string.IsNullOrWhiteSpace(CancelDate) ? "', '" + CancelDate + "'" : "', null") +
                     ")", ref result))
                {
                    //insert process card
                    StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID) values('" +
                        DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                        txtCardNumber.Value + "', '" +
                        "ADD" + "', '" +
                        cbCardGroup.Value + "', '" +
                        Request.Cookies["UserID"].Value.ToString() +
                        "')");

                    // realease or not
                    if (NewCustomerID != "")
                    {
                        StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                            txtCardNumber.Value + "', '" +
                            "RELEASE" + "', '" +
                            cbCardGroup.Value + "', '" +
                            Request.Cookies["UserID"].Value.ToString() + "', '" +
                            NewCustomerID +
                            "')");
                    }

                    //add SubCard
                    var _subCard = (string)Session["SubCard"];
                    if (!string.IsNullOrWhiteSpace(_subCard))
                    {
                        //Update Card
                        var sb = new StringBuilder();
                        sb.AppendLine("UPDATE [dbo].[tblSubCard]");
                        sb.AppendLine(string.Format("SET MainCard = '{0}'", txtCardNumber.Value));
                        sb.AppendLine(string.Format("WHERE CardNumber = '{0}'", _subCard));
                        StaticPool.mdb.ExecuteCommand(sb.ToString());
                    }
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Card", txtCardNumber.Value, "Thêm", "");


                    if (!string.IsNullOrWhiteSpace(ViewState["CustomerID"].ToString()) && ViewState["CustomerID"].ToString() != "0")
                    {
                        string FormatPlate = "";

                        if (!string.IsNullOrWhiteSpace(txtPlate1.Value))
                        {
                            FormatPlate += txtPlate1.Value;
                        }
                        if (!string.IsNullOrWhiteSpace(txtPlate2.Value))
                        {
                            FormatPlate += "_" + txtPlate2.Value;
                        }
                        if (!string.IsNullOrWhiteSpace(txtPlate3.Value))
                        {
                            FormatPlate += "_" + txtPlate3.Value;
                        }
                        if (!string.IsNullOrWhiteSpace(txtPlate1.Value) || !string.IsNullOrWhiteSpace(txtPlate2.Value) || !string.IsNullOrWhiteSpace(txtPlate3.Value))
                        {
                            CreateLogCardCustomer(ViewState["CustomerID"].ToString(), txtCardNumber.Value, "Create", RegisterDate, ReleaseDate, CancelDate, Request.Cookies["UserID"].Value.ToString(), FormatPlate, cbCardGroup.Value, cbCompartment.Value, txtCustomerName.Text, cbCustomerGroup.Value, txtCustomerCode.Text, chbIsLock.Checked ? 1 : 0, txtCardNo.Value);
                        }
                    }
                }
            }
            else
            {
                string _oldcustomerid = "";
                bool _oldState = false;
                string _oldcardexist = "";

                DataTable temp = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and  CardID='" + ViewState["CardID"].ToString() + "'");
                if (temp != null && temp.Rows.Count > 0)
                {
                    _oldcustomerid = temp.Rows[0]["CustomerID"].ToString();
                    _oldState = bool.Parse(temp.Rows[0]["IsLock"].ToString());

                    _oldcardexist = temp.Rows[0]["CardNo"].ToString() +
                            ";" + temp.Rows[0]["CardNumber"].ToString() +
                            ";" + temp.Rows[0]["CardGroupID"].ToString() +
                            ";" + temp.Rows[0]["Plate1"].ToString() + "_" + temp.Rows[0]["VehicleName1"].ToString() +
                            ";" + temp.Rows[0]["Plate2"] + "_" + temp.Rows[0]["VehicleName2"].ToString() +
                            ";" + temp.Rows[0]["Plate3"] + "_" + temp.Rows[0]["VehicleName3"].ToString() +
                            ";" + temp.Rows[0]["IsLock"].ToString();


                }
                if (chbIsLock.Checked)
                {
                    if (!string.IsNullOrWhiteSpace(temp.Rows[0]["DateCancel"].ToString()))
                    {
                        CancelDate = DateTime.Parse(temp.Rows[0]["DateCancel"].ToString()).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        CancelDate = DateTime.Now.ToString("yyyy/MM/dd");
                    }
                }
                else
                {
                    CancelDate = "";
                }

                //if (_oldcustomerid != ViewState["CustomerID"].ToString())
                //{
                //    //Register
                //    if (dtpNewRegisterDate.Value != "")
                //    {
                //        RegisterDate = dtpNewRegisterDate.Value.Substring(6, 4) + "/" + dtpNewRegisterDate.Value.Substring(3, 2) + "/" + dtpNewRegisterDate.Value.Substring(0, 2);
                //    }
                //    else if (ViewState["NewDateRegister"].ToString() != "")
                //    {
                //        RegisterDate = ViewState["NewDateRegister"].ToString().Substring(6, 4) + "/" + ViewState["NewDateRegister"].ToString().Substring(3, 2) + "/" + ViewState["NewDateRegister"].ToString().Substring(0, 2);
                //    }

                //    //Release
                //    if (dtpNewReleaseDate.Value != "")
                //    {
                //        ReleaseDate = dtpNewReleaseDate.Value.Substring(6, 4) + "/" + dtpNewReleaseDate.Value.Substring(3, 2) + "/" + dtpNewReleaseDate.Value.Substring(0, 2);
                //    }
                //    else if (ViewState["NewDateRelease"].ToString() != "")
                //    {
                //        ReleaseDate = ViewState["NewDateRelease"].ToString().Substring(6, 4) + "/" + ViewState["NewDateRelease"].ToString().Substring(3, 2) + "/" + ViewState["NewDateRelease"].ToString().Substring(0, 2);
                //    }
                //}

                // update
                if (StaticPool.mdb.ExecuteCommand("update tblCard set CardNo = N'" + txtCardNo.Value +
                    //"', CardNumber = '" + txtCardNumber.Value +
                    "', CardGroupID = '" + cbCardGroup.Value +
                    "', CustomerID = '" + ViewState["CustomerID"].ToString() +
                    //"', ExpireDate = '" + ExpireDate +
                    "', DateRelease = '" + ReleaseDate +
                    "', DateRegister = '" + RegisterDate +
                    "', IsLock = " + (chbIsLock.Checked ? 1 : 0) +
                    ", Plate1 = N'" + txtPlate1.Value +
                    "', VehicleName1 = N'" + txtVehicleName1.Value +
                    "', Plate2 = N'" + txtPlate2.Value +
                    "', VehicleName2 = N'" + txtVehicleName2.Value +
                    "', Plate3 = N'" + txtPlate3.Value +
                    "', VehicleName3 = N'" + txtVehicleName3.Value +
                    string.Format("{0}", !string.IsNullOrWhiteSpace(CancelDate) ? "', DateCancel = '" + CancelDate + "'" : "', DateCancel = null") +
                    " where CardID = '" + ViewState["CardID"].ToString() + "'", ref result))
                {
                    // return if _oldcustomerid!=""&&viewstate[CustomerID]=""
                    // change if _oldcustomer!=viewstate["CustomerID"]
                    if (_oldcustomerid != "" && (ViewState["CustomerID"].ToString() == "" || ViewState["CustomerID"].ToString() != "0"))
                    {
                        //The bi huy bo thong tin khach hang
                        StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                           DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                           txtCardNumber.Value + "', '" +
                           "RETURN" + "', '" +
                           cbCardGroup.Value + "', '" +
                           Request.Cookies["UserID"].Value.ToString() + "', '" +
                           _oldcustomerid +
                           "')");
                    }
                    //The bi thay doi thong tin khach hang
                    else if (_oldcustomerid != "" && (!string.IsNullOrWhiteSpace(ViewState["CustomerID"].ToString()) || ViewState["CustomerID"].ToString() != "0") && _oldcustomerid != ViewState["CustomerID"].ToString())
                    {
                        string _des = StaticPool.GetCustomerName(_oldcustomerid) + "->" + StaticPool.GetCustomerName(ViewState["CustomerID"].ToString());
                        if (_des.Length > 100)
                            _des = _des.Substring(0, 100);
                        //change
                        StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID, Description) values('" +
                           DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                           txtCardNumber.Value + "', '" +
                           "CHANGE" + "', '" +
                           cbCardGroup.Value + "', '" +
                           Request.Cookies["UserID"].Value.ToString() + "', '" +
                           ViewState["CustomerID"].ToString() + "', N'" +
                           _des +
                           "')");
                    }
                    //the duoc cap phat cho khach hang
                    else if (_oldcustomerid == "" && (!string.IsNullOrWhiteSpace(ViewState["CustomerID"].ToString()) || ViewState["CustomerID"].ToString() != "0"))
                    {
                        //release
                        StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                            txtCardNumber.Value + "', '" +
                            "RELEASE" + "', '" +
                            cbCardGroup.Value + "', '" +
                            Request.Cookies["UserID"].Value.ToString() + "', '" +
                            ViewState["CustomerID"].ToString() +
                            "')");
                    }

                    if (_oldState != chbIsLock.Checked)
                    {
                        //oldstate=false,newstate=true->lock
                        //oldstate=true, newstate=false->unlock
                        string _statechange = chbIsLock.Checked == true ? "LOCK" : "UNLOCK";
                        //state change
                        StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                            txtCardNumber.Value + "', '" +
                           _statechange + "', '" +
                            cbCardGroup.Value + "', '" +
                            Request.Cookies["UserID"].Value.ToString() + "', '" +
                            ViewState["CustomerID"].ToString() +
                            "')");
                    }

                    string _newcardexist = txtCardNo.Value +
                        ";" + txtCardNumber.Value +
                        ";" + cbCardGroup.Value +
                        ";" + txtPlate1.Value + "_" + txtVehicleName1.Value +
                        ";" + txtPlate2.Value + "_" + txtVehicleName2.Value +
                        ";" + txtPlate3.Value + "_" + txtVehicleName3.Value +
                        ";" + chbIsLock.Checked.ToString();
                    string _description = StaticPool.GetStringChange(_oldcardexist, _newcardexist);

                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Card", txtCardNumber.Value, "Sửa", _description);


                    string oldRegistedDate = hidRegisterDate.Value.Substring(6, 4) + "/" + hidRegisterDate.Value.Substring(3, 2) + "/" + hidRegisterDate.Value.Substring(0, 2);

                    string oldReleaseDate = hidReleaseDate.Value.Substring(6, 4) + "/" + hidReleaseDate.Value.Substring(3, 2) + "/" + hidReleaseDate.Value.Substring(0, 2);

                    if (!RegisterDate.Equals(oldRegistedDate) || !ReleaseDate.Equals(oldReleaseDate) || !ViewState["CustomerID"].ToString().Equals(hidCustomer.Value))
                    {
                        string FormatPlate = "";

                        if (!string.IsNullOrWhiteSpace(txtPlate1.Value))
                        {
                            FormatPlate += txtPlate1.Value;
                        }
                        if (!string.IsNullOrWhiteSpace(txtPlate2.Value))
                        {
                            FormatPlate += "_" + txtPlate2.Value;
                        }
                        if (!string.IsNullOrWhiteSpace(txtPlate3.Value))
                        {
                            FormatPlate += "_" + txtPlate3.Value;
                        }

                        if (!string.IsNullOrWhiteSpace(txtPlate1.Value) || !string.IsNullOrWhiteSpace(txtPlate2.Value) || !string.IsNullOrWhiteSpace(txtPlate3.Value))
                        {
                            CreateLogCardCustomer(ViewState["CustomerID"].ToString(), txtCardNumber.Value, "Update", RegisterDate, ReleaseDate, CancelDate, Request.Cookies["UserID"].Value.ToString(), FormatPlate, cbCardGroup.Value, cbCompartment.Value, txtCustomerName.Text, cbCustomerGroup.Value, txtCustomerCode.Text, 0, txtCardNo.Value);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(txtPlate1.Value) || !string.IsNullOrWhiteSpace(txtPlate2.Value) || !string.IsNullOrWhiteSpace(txtPlate3.Value))
                    {
                        //Update LogCardCustomer
                        UpdateLogCardCustomer(ViewState["CustomerID"].ToString(), txtCardNumber.Value, RegisterDate, ReleaseDate, cbCompartment.Value, chbIsLock.Checked ? 1 : 0, CancelDate);

                        //Update trạng thái
                        //UpdateLogCardCustomer(ViewState["CustomerID"].ToString(), txtCardNumber.Value, cbCompartment.Value, chbIsLock.Checked ? 1 : 0);
                    }
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                boxNewDateTime.Visible = false;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Card.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            boxNewDateTime.Visible = false;
            id_alert.InnerText = ex.Message;
        }
    }

    private string InsertCustomer(string CustomerID)
    {
        string result = "";

        // Them moi
        if (StaticPool.mdb.ExecuteCommand("insert into tblCustomer (CustomerID, Description, CustomerName, Inactive, CustomerCode, Address, IDNumber, Mobile, CustomerGroupID, EnableAccount, Account, Password, Avatar, CompartmentId) values(N'" +
                    CustomerID + "', N'" +
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
                    id_avatar.Value +
                    cbCompartment.Value + "', '" +
                    "')", ref result))
        {
            StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Customer", txtCustomerName.Text, "Thêm", "");

        }

        return result;
    }

    private string UpdateCustomer(string CustomerID)
    {
        string result = "";
        string _oldcustomer = "";
        DataTable dt = StaticPool.mdb.FillData("select * from tblCustomer where CustomerID='" + CustomerID + "'");
        if (dt != null && dt.Rows.Count > 0)
        {
            DataRowView drv = dt.DefaultView[0];
            _oldcustomer = drv["CustomerCode"].ToString() +
                ";" + drv["CustomerName"].ToString() +
                ";" + drv["Address"].ToString() +
                ";" + drv["IDNumber"].ToString() +
                ";" + drv["Mobile"].ToString() +
                ";" + drv["CustomerGroupID"].ToString() +
                ";" + drv["EnableAccount"].ToString() +
                ";" + drv["Account"].ToString() +
                ";" + drv["Inactive"].ToString();


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
                "' where CustomerID = '" + CustomerID + "'", ref result))
            {
                string _newcustomer = txtCustomerCode.Text +
                    ";" + txtCustomerName.Text +
                    ";" + txtAddress.Text +
                    ";" + txtIDNumber.Text +
                    ";" + txtMobile.Text +
                    ";" + cbCustomerGroup.Value +
                    ";" + chbEnableAccount.Checked.ToString() +
                    ";" + txtAccount.Text +
                    ";" + chbInactive.Checked.ToString();
                string _des = StaticPool.GetStringChange(_oldcustomer, _newcustomer);
                StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Customer", txtCustomerName.Text, "Sửa", _des);

            }
        }
        else if (dt != null && dt.Rows.Count == 0)
        {
            InsertCustomer(CustomerID);
        }

        return result;
    }

    /// <summary>
    /// Tạo log card với customer
    /// </summary>
    /// <param name="newcustomer">Khách hàng mới</param>
    /// <param name="cardnumber">Mã thẻ</param>
    /// <param name="action">Hành động (Create / Update)</param>
    /// <param name="DateRegisted">Giá trị </param>
    /// <param name="DateReleased"></param>
    private void CreateLogCardCustomer(string newcustomer, string cardnumber, string action, string DateRegisted, string DateReleased, string DateCanceled, string userid, string plate, string cardgroup, string compartment, string customername, string customergroup, string customercode, int islock, string cardno)
    {
        string format = "yyyy/MM/dd HH:mm:ss";
        var timenew = DateTime.Now;

        string command = "";

        if (!string.IsNullOrWhiteSpace(DateCanceled))
        {
            command = string.Format("INSERT into tblLogCardCustomer (CustomerID,CardNumber,Actions,DateChanged,DateRegisted,DateReleased,DateCanceled,UserID,CardGroupID,CompartmentID,Plate,CustomerName,CustomerGroupID,CustomerCode,CardIsLock,CardNo) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',N'{11}','{12}','{13}',{14},'{15}')", newcustomer, cardnumber, action, timenew.ToString(format), DateRegisted, DateReleased, DateCanceled, userid, cardgroup, !string.IsNullOrWhiteSpace(compartment) ? compartment : Guid.Empty.ToString(), plate, customername, customergroup, customercode, islock, !string.IsNullOrWhiteSpace(cardno) ? cardno : "");
        }
        else
        {
            command = string.Format("INSERT into tblLogCardCustomer (CustomerID,CardNumber,Actions,DateChanged,DateRegisted,DateReleased,DateCanceled,UserID,CardGroupID,CompartmentID,Plate,CustomerName,CustomerGroupID,CustomerCode,CardIsLock,CardNo) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}',null,'{6}','{7}','{8}','{9}',N'{10}','{11}','{12}',{13},'{14}')", newcustomer, cardnumber, action, timenew.ToString(format), DateRegisted, DateReleased, userid, cardgroup, !string.IsNullOrWhiteSpace(compartment) ? compartment : Guid.Empty.ToString(), plate, customername, customergroup, customercode, islock, !string.IsNullOrWhiteSpace(cardno) ? cardno : "");
        }

        StaticPool.mdb.ExecuteCommand(command);
    }

    public void UpdateLogCardCustomer(string newcustomer, string cardnumber, string DateRegisted, string DateReleased, string compartment, int islock, string dateCancel)
    {
        string updatecommand = string.Format("update tblLogCardCustomer set CardIsLock = {0}, DateCanceled = {6} where CustomerID = '{1}' and CardNumber = '{2}' and DateRegisted = '{3}' and DateReleased = '{4}' and CompartmentID = '{5}'", islock, newcustomer, cardnumber, DateRegisted, DateReleased, !string.IsNullOrWhiteSpace(compartment) ? compartment : Guid.Empty.ToString(), !string.IsNullOrWhiteSpace(dateCancel) ? "'" + dateCancel + "'" : "null");

        StaticPool.mdb.ExecuteCommand(updatecommand);
    }

    public void UpdateLogCardCustomer(string newcustomer, string cardnumber, string compartment, int islock)
    {
        string updatecommand = string.Format("update tblLogCardCustomer set CardIsLockHidden = {0} where CustomerID = '{1}' and CardNumber = '{2}' and CompartmentID = '{3}'", islock, newcustomer, cardnumber, !string.IsNullOrWhiteSpace(compartment) ? compartment : Guid.Empty.ToString());

        StaticPool.mdb.ExecuteCommand(updatecommand);
    }

    private static void CreateLogEditDateCard(string userid, string cardnumber, string datetype, string datevalue)
    {
        string command = "";

        if (!string.IsNullOrWhiteSpace(datevalue))
        {
            datevalue = datevalue.Substring(6, 4) + "/" + datevalue.Substring(3, 2) + "/" + datevalue.Substring(0, 2);

            command = string.Format("INSERT into tblLogEditDateCard (UserID,CardNumber,DateType,DateValue) VALUES ('{0}','{1}','{2}','{3}')", userid, cardnumber, datetype, datevalue);
        }
        else
        {
            command = string.Format("INSERT into tblLogEditDateCard (UserID,CardNumber,DateType,DateValue) VALUES ('{0}','{1}','{2}',null)", userid, cardnumber, datetype);
        }


        StaticPool.mdb.ExecuteCommand(command);
    }

    public bool ValidDate(string registerdate, string releasedate, ref string message)
    {
        bool isSuccess = false;

        DateTime register = DateTime.Parse(registerdate);
        DateTime release = DateTime.Parse(releasedate);

        if (register <= release)
        {
            isSuccess = true;
        }
        else
        {
            isSuccess = false;
            message = "Ngày phát hàng không được nhỏ hơn ngày đăng ký";
        }

        return isSuccess;
    }

    [System.Web.Services.WebMethod]
    public static void ChangeRegisterDate(string cardnumber, string registerdate, string userid)
    {
        CreateLogEditDateCard(StaticPool.GetUserName(userid), cardnumber, "RG", registerdate);
    }

    [System.Web.Services.WebMethod]
    public static void ChangeReleaseDate(string cardnumber, string releasedate, string userid)
    {
        CreateLogEditDateCard(StaticPool.GetUserName(userid), cardnumber, "RL", releasedate);
    }
}
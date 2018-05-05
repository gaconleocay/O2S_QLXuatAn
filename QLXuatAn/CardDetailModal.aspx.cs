using System;
using System.Web.Services;
using System.Data;
using Futech.Helpers;

public partial class QLXuatAn_CardDetailModal : System.Web.UI.Page
{
    public string Id = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";
                // Nhom the
                DataTable dtCardGroup = CacheLayer.Get<DataTable>(StaticCached.c_tblCardGroup);
                if (dtCardGroup == null)
                {
                    dtCardGroup = StaticPool.mdb.FillData("select CardGroupName, CardGroupID from tblCardGroup order by SortOrder");
                    if (dtCardGroup.Rows.Count > 0)
                        CacheLayer.Add(StaticCached.c_tblCardGroup, dtCardGroup, StaticCached.TimeCache);
                }
                cbCardGroup.DataSource = dtCardGroup;// StaticPool.mdb.FillData("select * from tblCardGroup");
                cbCardGroup.DataTextField = "CardGroupName";
                cbCardGroup.DataValueField = "CardGroupID";
                cbCardGroup.DataBind();

                //ViewState["OldCard"] = "";


                if (Request.QueryString["CustomerID"] != null)
                {
                    ViewState["CustomerID"] = Request.QueryString["CustomerID"].ToString();
                }
                else
                {
                    ViewState["CustomerID"] = "";
                }

                dtpExpireDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                dtpRegisterDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                dtpReleaseDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                hidRegisterDate.Value = DateTime.Now.ToString("dd/MM/yyyy");
                hidReleaseDate.Value = DateTime.Now.ToString("dd/MM/yyyy");

                if (Request.QueryString["Id"] != null && Request.QueryString["Id"].ToString() != "")
                {
                    Id = Request.QueryString["Id"].ToString();
                    id_carddetail.InnerText = "Sửa thẻ";
                    DataTable dt = StaticPool.mdb.FillData("select * from tblCard where CardID = '" + Id + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        txtCardNo.Value = dr["CardNo"].ToString();
                        txtCardNumber.Value = dr["CardNumber"].ToString();
                        txtCardNumber.EnableViewState = false;
                        cbCardGroup.Value = dr["CardGroupID"].ToString();
                        if (dr["ExpireDate"].ToString() != "")
                            dtpExpireDate.Value = DateTime.Parse(dr["ExpireDate"].ToString()).ToString("dd/MM/yyyy");
                        if (dr["DateRegister"].ToString() != "")
                        {
                            dtpRegisterDate.Value = DateTime.Parse(dr["DateRegister"].ToString()).ToString("dd/MM/yyyy");
                            hidRegisterDate.Value = DateTime.Parse(dr["DateRegister"].ToString()).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dtpRegisterDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                            hidRegisterDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                        }

                        if (dr["DateRelease"].ToString() != "")
                        {
                            dtpReleaseDate.Value = DateTime.Parse(dr["DateRelease"].ToString()).ToString("dd/MM/yyyy");
                            hidReleaseDate.Value = DateTime.Parse(dr["DateRelease"].ToString()).ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            dtpReleaseDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                            hidReleaseDate.Value = DateTime.Parse(dr["ImportDate"].ToString()).ToString("dd/MM/yyyy");
                        }
                        chbIsLock.Checked = bool.Parse(dr["IsLock"].ToString());
                        txtPlate1.Value = dr["Plate1"].ToString();
                        txtVehicleName1.Value = dr["VehicleName1"].ToString();
                        txtPlate2.Value = dr["Plate2"].ToString();
                        txtVehicleName2.Value = dr["VehicleName2"].ToString();
                        txtPlate3.Value = dr["Plate3"].ToString();
                        txtVehicleName3.Value = dr["VehicleName3"].ToString();
                        txtCardNumber.Attributes.Add("readonly", "readonly");
                        dtpExpireDate.Attributes.Add("disabled", "true");



                    }
                }
                else
                {
                    id_carddetail.InnerText = "Thêm thẻ";
                    txtCardNo.Value = "";
                    txtCardNumber.Value = "";
                    //card list, cards do not belong anyone
                    //cbCardList.DataSource = StaticPool.mdb.FillData("select * from tblCard where CustomerID='' or CustomerID='-1'");
                    //cbCardList.DataTextField = "CardNumber";
                    //cbCardList.DataValueField = "CardNumber";

                    //cbCardList.DataBind();
                    //cbCardList.SelectedIndex = -1;

                    //DataTable dtcard = StaticPool.mdb.FillData("select * from tblCard where (CustomerID='' or CustomerID='-1') and IsDelete=0 order by SortOrder");
                    //if (dtcard != null && dtcard.Rows.Count > 0)
                    //{
                    //    cbCardList.Items.Add(new ListItem("", ""));
                    //    foreach (DataRow dr in dtcard.Rows)
                    //    {
                    //        DataTable dtgroup = StaticPool.mdb.FillData("select CardType from tblCardgroup where CardGroupID='" + dr["CardGroupID"].ToString() + "'");

                    //        if (dtgroup != null && dtgroup.Rows.Count > 0)
                    //        {
                    //            if (dtgroup.Rows[0]["CardType"].ToString() == "1")
                    //                continue;
                    //        }
                    //        string _itemvalue = dr["CardNumber"].ToString();
                    //        string _itemtext = dr["CardNumber"].ToString();
                    //        if (dr["CardNo"].ToString() != "")
                    //            _itemtext = dr["CardNo"].ToString() + "/" + _itemtext;


                    //        cbCardList.Items.Add(new ListItem(_itemtext, _itemvalue));
                    //    }
                    //}
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    [WebMethod]
    public static string Save(string Id, string CardNo, string CardNumber, string CardGroupID, string CustomerID, string ExpireDate, string ReleaseDate, string RegisterDate, string IsLock, string Plate1, string VehicleName1, string Plate2, string VehicleName2, string Plate3, string VehicleName3, string cardID, string OldRegisterDate, string OldReleaseDate, string userID)
    {
        try
        {
            string customername = "";
            string compartment = "";
            string customergroup = "";
            string customercode = "";

            string result = "";
            ExpireDate = ExpireDate.Substring(6, 4) + "/" + ExpireDate.Substring(3, 2) + "/" + ExpireDate.Substring(0, 2);
            RegisterDate = RegisterDate.Substring(6, 4) + "/" + RegisterDate.Substring(3, 2) + "/" + RegisterDate.Substring(0, 2);
            ReleaseDate = ReleaseDate.Substring(6, 4) + "/" + ReleaseDate.Substring(3, 2) + "/" + ReleaseDate.Substring(0, 2);
            OldRegisterDate = OldRegisterDate.Substring(6, 4) + "/" + OldRegisterDate.Substring(3, 2) + "/" + OldRegisterDate.Substring(0, 2);
            OldReleaseDate = OldReleaseDate.Substring(6, 4) + "/" + OldReleaseDate.Substring(3, 2) + "/" + OldReleaseDate.Substring(0, 2);
            string CancelDate = "";



            if (CardNumber == "")
                return "Mã thẻ không được để trống!";

            //DataTable dtCard = StaticPool.mdb.FillData("select * from tblCard where CardNumber = '" + CardNumber + "'");
            //if (dtCard != null && dtCard.Rows.Count > 0 && ((dtCard.Rows[0]["CardID"].ToString() != Id && Id != "")||Id=="") )
            //    return "Mã thẻ đã khai báo! Vui lòng nhập mã thẻ khác.";
            string _oldcard = "";
            bool _oldcardstate = false;

            DataTable dtoldcard = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and Cardnumber='" + CardNumber + "'");
            if (dtoldcard != null && dtoldcard.Rows.Count > 0)
            {
                DataRowView drv = dtoldcard.DefaultView[0];
                _oldcard = drv["CardNo"].ToString() +
                    ";" + drv["CardNumber"].ToString() +
                    ";" + drv["CardGroupID"].ToString() +
                    ";" + drv["Plate1"].ToString() + "_" + drv["VehicleName1"].ToString() +
                    ";" + drv["Plate2"] + "_" + drv["VehicleName2"].ToString() +
                    ";" + drv["Plate3"] + "_" + drv["VehicleName3"].ToString() +
                    ";" + drv["IsLock"].ToString();

                _oldcardstate = bool.Parse(drv["IsLock"].ToString());
            }

            if (Id == "")
            {

                if (cardID != "")//from selection cardlist box
                {
                    DataTable temp = StaticPool.mdb.FillData("select * from tblCard where CardID='" + cardID + "' and CardNumber='" + CardNumber + "'");
                    if (temp != null && temp.Rows.Count == 1)//exist card
                    {
                        string _oldcardexist = temp.Rows[0]["CardNo"].ToString() +
                            ";" + temp.Rows[0]["CardNumber"].ToString() +
                            ";" + temp.Rows[0]["CardGroupID"].ToString() +
                            ";" + temp.Rows[0]["Plate1"].ToString() + "_" + temp.Rows[0]["VehicleName1"].ToString() +
                            ";" + temp.Rows[0]["Plate2"] + "_" + temp.Rows[0]["VehicleName2"].ToString() +
                            ";" + temp.Rows[0]["Plate3"] + "_" + temp.Rows[0]["VehicleName3"].ToString() +
                            ";" + temp.Rows[0]["IsLock"].ToString();
                        bool _oldstate = bool.Parse(temp.Rows[0]["IsLock"].ToString());

                        if (_oldstate)
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

                        // update
                        if (StaticPool.mdb.ExecuteCommand("update tblCard set CardNo = N'" + CardNo +
                            "', CardGroupID = '" + CardGroupID +
                            "', CustomerID='" + CustomerID +
                            //"', ExpireDate = '" + ExpireDate +
                            "', IsLock = " + (bool.Parse(IsLock) ? 1 : 0) +
                            ", Plate1 = N'" + Plate1 +
                            "', VehicleName1 = N'" + VehicleName1 +
                            "', Plate2 = N'" + Plate2 +
                            "', VehicleName2 = N'" + VehicleName2 +
                            "', Plate3 = N'" + Plate3 +
                            "', VehicleName3 = N'" + VehicleName3 +
                            "', DateRegister = '" + RegisterDate +
                            "', DateRelease = '" + ReleaseDate +
                            string.Format("{0}", !string.IsNullOrWhiteSpace(CancelDate) ? "', DateCancel = '" + CancelDate + "'" : "', DateCancel = null") +
                            " where CardID = '" + cardID + "'", ref result))
                        {
                            string _newcardexist = CardNo +
                                ";" + CardNumber +
                                ";" + CardGroupID +
                                ";" + Plate1 + "_" + VehicleName1 +
                                ";" + Plate2 + "_" + VehicleName2 +
                                ";" + Plate3 + "_" + VehicleName3 +
                                ";" + IsLock;

                            string _des = StaticPool.GetStringChange(_oldcardexist, _newcardexist);
                            //release for customer
                            StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                              DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                              CardNumber + "', '" +
                              "RELEASE" + "', '" +
                              CardGroupID + "', '" +
                              userID + "', '" +
                              CustomerID +
                              "')");

                            DataTable dtCustomer = StaticPool.mdb.FillData(string.Format("select * from tblCustomer where CustomerID='{0}'", CustomerID));

                            if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                            {
                                customername = dtCustomer.Rows[0]["CustomerName"].ToString();
                                customergroup = dtCustomer.Rows[0]["CustomerGroupID"].ToString();
                                compartment = dtCustomer.Rows[0]["CompartmentId"].ToString();
                                customercode = dtCustomer.Rows[0]["CustomerCode"].ToString();
                            }
                            string FormatPlate = "";

                            if (!string.IsNullOrWhiteSpace(Plate1))
                            {
                                FormatPlate += Plate1;
                            }
                            if (!string.IsNullOrWhiteSpace(Plate2))
                            {
                                FormatPlate += "_" + Plate2;
                            }
                            if (!string.IsNullOrWhiteSpace(Plate3))
                            {
                                FormatPlate += "_" + Plate3;
                            }


                            StaticPool.SaveLogFile(StaticPool.GetUserName(userID), "Parking", "Parking_Card", CardNumber, "Sửa", _des);
                            if (!RegisterDate.Equals(OldRegisterDate) || !ReleaseDate.Equals(OldReleaseDate) || _oldcardstate != bool.Parse(IsLock))
                            {
                                if (!string.IsNullOrWhiteSpace(Plate1) || !string.IsNullOrWhiteSpace(Plate2) || !string.IsNullOrWhiteSpace(Plate3)) {
                                    CreateLogCardCustomer(CustomerID, CardNumber, "Update", RegisterDate, ReleaseDate, CancelDate, userID, FormatPlate, CardGroupID, compartment, customername, customergroup, customercode, bool.Parse(IsLock) ? 1 : 0, CardNo);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Plate1) || !string.IsNullOrWhiteSpace(Plate2) || !string.IsNullOrWhiteSpace(Plate3))
                            {
                                UpdateLogCardCustomer(CustomerID, CardNumber, RegisterDate, ReleaseDate, compartment, bool.Parse(IsLock) ? 1 : 0, CancelDate);

                                //UpdateLogCardCustomer(CustomerID, CardNumber, compartment, bool.Parse(IsLock) ? 1 : 0);
                            }
                            return "true";
                        }
                        else
                            return result;
                    }
                }

                DataTable dtcard = StaticPool.mdb.FillData("select CardNumber from tblCard where IsDelete=0 and CardNumber='" + CardNumber + "'");
                if (dtcard != null && dtcard.Rows.Count > 0)
                {
                    return "Mã thẻ đã khai báo! Vui lòng nhập mã thẻ khác.";
                }

                if (bool.Parse(IsLock))
                {
                    CancelDate = DateTime.Now.ToString("yyyy/MM/dd");
                }
                else
                {
                    CancelDate = null;
                }

                // insert
                if (StaticPool.mdb.ExecuteCommand("insert into tblCard (CardNo, CardNumber, CardGroupID, CustomerID, ExpireDate, IsLock, Plate1, VehicleName1, Plate2, VehicleName2, Plate3, VehicleName3, ImportDate, Description,DateRegister,DateRelease,DateCancel) values('" + CardNo +
                    "', '" + CardNumber +
                    "', '" + CardGroupID +
                    "', '" + CustomerID +
                    "', '" + ExpireDate +
                    "', " + (bool.Parse(IsLock) ? 1 : 0) +
                    ", N'" + Plate1 +
                    "', N'" + VehicleName1 +
                    "', N'" + Plate2 +
                    "', N'" + VehicleName2 +
                    "', N'" + Plate3 +
                    "', N'" + VehicleName3 +
                    "', '" + DateTime.Now.ToString("yyyy/MM/dd") +
                    "', '" + "Manual" +
                    "', '" + RegisterDate +
                    "', '" + ReleaseDate +
                    string.Format("{0}", !string.IsNullOrWhiteSpace(CancelDate) ? "', '" + CancelDate + "'" : "', null") +
                    ")", ref result))
                {
                    //insert process card

                    DateTime dtime = DateTime.Now;

                    StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID) values('" +
                        dtime.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                        CardNumber + "', '" +
                        "ADD" + "', '" +
                        CardGroupID + "', '" +
                        userID +
                        "')");

                    //release for customer
                    StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                       dtime.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                       CardNumber + "', '" +
                       "RELEASE" + "', '" +
                       CardGroupID + "', '" +
                       userID + "', '" +
                       CustomerID +
                       "')");

                    //log
                    StaticPool.SaveLogFile(StaticPool.GetUserName(userID), "Parking", "Parking_Card", CardNumber, "Thêm", "");

                    DataTable dtCustomer = StaticPool.mdb.FillData(string.Format("select * from tblCustomer where CustomerID='{0}'", CustomerID));

                    if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                    {
                        customername = dtCustomer.Rows[0]["CustomerName"].ToString();
                        customergroup = dtCustomer.Rows[0]["CustomerGroupID"].ToString();
                        compartment = dtCustomer.Rows[0]["CompartmentId"].ToString();
                        customercode = dtCustomer.Rows[0]["CustomerCode"].ToString();
                    }
                    string FormatPlate = "";

                    if (!string.IsNullOrWhiteSpace(Plate1))
                    {
                        FormatPlate += Plate1;
                    }
                    if (!string.IsNullOrWhiteSpace(Plate2))
                    {
                        FormatPlate += "_" + Plate2;
                    }
                    if (!string.IsNullOrWhiteSpace(Plate3))
                    {
                        FormatPlate += "_" + Plate3;
                    }

                    if (!string.IsNullOrWhiteSpace(Plate1) || !string.IsNullOrWhiteSpace(Plate2) || !string.IsNullOrWhiteSpace(Plate3))
                    {
                        CreateLogCardCustomer(CustomerID, CardNumber, "Create", RegisterDate, ReleaseDate, CancelDate, userID, FormatPlate, CardGroupID, compartment, customername, customergroup, customercode, bool.Parse(IsLock) ? 1 : 0, CardNo);
                    }

                    return "true";
                }
                else
                {
                    return result;
                }
            }
            else
            {

                if (bool.Parse(IsLock))
                {
                    DataTable temp = StaticPool.mdb.FillData("select * from tblCard where CardNumber='" + CardNumber + "'");
                    if (temp != null && temp.Rows.Count > 0)
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
                }
                else
                {
                    CancelDate = "";
                }

                // update
                if (StaticPool.mdb.ExecuteCommand("update tblCard set CardNo = N'" + CardNo +
                    "', CardNumber = '" + CardNumber +
                    "', CardGroupID = '" + CardGroupID +
                    "', ExpireDate = '" + ExpireDate +
                    "', DateRegister = '" + RegisterDate +
                    "', DateRelease = '" + ReleaseDate +
                    string.Format("{0}", !string.IsNullOrWhiteSpace(CancelDate) ? "', DateCancel = '" + CancelDate + "'" : "', DateCancel = null") +
                    ", IsLock = " + (bool.Parse(IsLock) ? 1 : 0) +
                    ", Plate1 = N'" + Plate1 +
                    "', VehicleName1 = N'" + VehicleName1 +
                    "', Plate2 = N'" + Plate2 +
                    "', VehicleName2 = N'" + VehicleName2 +
                    "', Plate3 = N'" + Plate3 +
                    "', VehicleName3 = N'" + VehicleName3 +
                    "' where CardID = '" + Id + "'", ref result))
                {
                    //change card info

                    string _newcard = CardNo +
                            ";" + CardNumber +
                            ";" + CardGroupID +
                            ";" + DateTime.Parse(ExpireDate).ToString("dd/MM/yyyy") +
                            ";" + Plate1 + "_" + VehicleName1 +
                            ";" + Plate2 + "_" + VehicleName2 +
                            ";" + Plate3 + "_" + VehicleName3 +
                            ";" + IsLock.ToString();
                    string _des = StaticPool.GetStringChange(_oldcard, _newcard);

                    if (_oldcardstate != bool.Parse(IsLock))
                    {
                        //oldstate=false,newstate=true->lock
                        //oldstate=true, newstate=false->unlock
                        string _statechange = bool.Parse(IsLock) == true ? "LOCK" : "UNLOCK";
                        //state change
                        StaticPool.mdb.ExecuteCommand("insert into tblCardProcess(Date, CardNumber, Actions, CardGroupID, UserID, CustomerID) values('" +
                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" +
                            CardNumber + "', '" +
                           _statechange + "', '" +
                            CardGroupID + "', '" +
                            userID + "', '" +
                            CustomerID +
                            "')");
                    }

                    DataTable dtCustomer = StaticPool.mdb.FillData(string.Format("select * from tblCustomer where CustomerID='{0}'", CustomerID));

                    if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                    {
                        customername = dtCustomer.Rows[0]["CustomerName"].ToString();
                        customergroup = dtCustomer.Rows[0]["CustomerGroupID"].ToString();
                        compartment = dtCustomer.Rows[0]["CompartmentId"].ToString();
                        customercode = dtCustomer.Rows[0]["CustomerCode"].ToString();
                    }
                    string FormatPlate = "";

                    if (!string.IsNullOrWhiteSpace(Plate1))
                    {
                        FormatPlate += Plate1;
                    }
                    if (!string.IsNullOrWhiteSpace(Plate2))
                    {
                        FormatPlate += "_" + Plate2;
                    }
                    if (!string.IsNullOrWhiteSpace(Plate3))
                    {
                        FormatPlate += "_" + Plate3;
                    }


                    StaticPool.SaveLogFile(StaticPool.GetUserName(userID), "Parking", "Parking_Card", CardNumber, "Sửa", _des);

                    if (!RegisterDate.Equals(OldRegisterDate) || !ReleaseDate.Equals(OldReleaseDate))
                    {
                        if (!string.IsNullOrWhiteSpace(Plate1) || !string.IsNullOrWhiteSpace(Plate2) || !string.IsNullOrWhiteSpace(Plate3))
                        {
                            CreateLogCardCustomer(CustomerID, CardNumber, "Update", RegisterDate, ReleaseDate, CancelDate, userID, FormatPlate, CardGroupID, compartment, customername, customergroup, customercode, bool.Parse(IsLock) ? 1 : 0, CardNo);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(Plate1) || !string.IsNullOrWhiteSpace(Plate2) || !string.IsNullOrWhiteSpace(Plate3))
                    {
                        UpdateLogCardCustomer(CustomerID, CardNumber, RegisterDate, ReleaseDate, compartment, bool.Parse(IsLock) ? 1 : 0, CancelDate);
                        //UpdateLogCardCustomer(CustomerID, CardNumber, compartment, bool.Parse(IsLock) ? 1 : 0);
                    }
                    

                    return "true";
                }
                else
                    return result;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }

    [WebMethod]
    public static string UpdateDateRegister(string Id, string CardNo, string CardNumber, string CardGroupID, string CustomerID, string ExpireDate, string ReleaseDate, string RegisterDate, string IsLock, string Plate1, string VehicleName1, string Plate2, string VehicleName2, string Plate3, string VehicleName3, string cardID, string userID)
    {
        string result = "";

        try
        {
            string command = "";

            if (!string.IsNullOrWhiteSpace(RegisterDate))
            {
                RegisterDate = RegisterDate.Substring(6, 4) + "/" + RegisterDate.Substring(3, 2) + "/" + RegisterDate.Substring(0, 2);

                command = string.Format("INSERT into tblLogEditDateCard (UserID,CardNumber,DateType,DateValue) VALUES ('{0}','{1}','{2}','{3}')", StaticPool.GetUserName(userID), CardNumber, "RG", RegisterDate);
            }
            else
            {
                command = string.Format("INSERT into tblLogEditDateCard (UserID,CardNumber,DateType,DateValue) VALUES ('{0}','{1}','{2}',null)", StaticPool.GetUserName(userID), CardNumber, "RG");
            }


            StaticPool.mdb.ExecuteCommand(command, ref result);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return result;
    }

    [WebMethod]
    public static string UpdateDateRelease(string Id, string CardNo, string CardNumber, string CardGroupID, string CustomerID, string ExpireDate, string ReleaseDate, string RegisterDate, string IsLock, string Plate1, string VehicleName1, string Plate2, string VehicleName2, string Plate3, string VehicleName3, string cardID, string userID)
    {
        string result = "";

        try
        {
            string command = "";

            if (!string.IsNullOrWhiteSpace(ReleaseDate))
            {
                ReleaseDate = ReleaseDate.Substring(6, 4) + "/" + ReleaseDate.Substring(3, 2) + "/" + ReleaseDate.Substring(0, 2);

                command = string.Format("INSERT into tblLogEditDateCard (UserID,CardNumber,DateType,DateValue) VALUES ('{0}','{1}','{2}','{3}')", StaticPool.GetUserName(userID), CardNumber, "RL", ReleaseDate);
            }
            else
            {
                command = string.Format("INSERT into tblLogEditDateCard (UserID,CardNumber,DateType,DateValue) VALUES ('{0}','{1}','{2}',null)", StaticPool.GetUserName(userID), CardNumber, "RL");
            }


            StaticPool.mdb.ExecuteCommand(command, ref result);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return result;
    }

    [WebMethod]
    public static string Change(string cardnumber)
    {
        try
        {
            DataTable dt = StaticPool.mdb.FillData("select * from tblCard where IsDelete=0 and CardNumber='" + cardnumber + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                string _cardID = dt.Rows[0]["CardID"].ToString();
                string _cardno = dt.Rows[0]["CardNo"].ToString();
                string _expiredate = DateTime.Parse(dt.Rows[0]["ExpireDate"].ToString()).ToString("dd/MM/yyyy");
                string _cardgroupname = dt.Rows[0]["CardgroupID"].ToString();

                string st = _cardID + ";" + _cardno + ";" + _expiredate + ";" + _cardgroupname;
                return st;
            }
        }
        catch
        {
        }
        return "";
    }

    private static void CreateLogCardCustomer(string newcustomer, string cardnumber, string action, string DateRegisted, string DateReleased, string DateCanceled, string userid, string plate, string cardgroup, string compartment, string customername, string customergroup, string customercode, int islock, string cardno)
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

    public static void UpdateLogCardCustomer(string newcustomer, string cardnumber, string DateRegisted, string DateReleased, string compartment, int islock, string datecancel)
    {
        string updatecommand = string.Format("update tblLogCardCustomer set CardIsLock = {0}, DateCanceled = {6} where CustomerID = '{1}' and CardNumber = '{2}' and DateRegisted = '{3}' and DateReleased = '{4}' and CompartmentID = '{5}'", islock, newcustomer, cardnumber, DateRegisted, DateReleased, !string.IsNullOrWhiteSpace(compartment) ? compartment : Guid.Empty.ToString(), !string.IsNullOrWhiteSpace(datecancel) ? "'" + datecancel + "'" : "null");

        StaticPool.mdb.ExecuteCommand(updatecommand);
    }

    public static void UpdateLogCardCustomer(string newcustomer, string cardnumber, string compartment, int islock)
    {
        string updatecommand = string.Format("update tblLogCardCustomer set CardIsLockHidden = {0} where CustomerID = '{1}' and CardNumber = '{2}' and CompartmentID = '{3}'", islock, newcustomer, cardnumber, !string.IsNullOrWhiteSpace(compartment) ? compartment : Guid.Empty.ToString());

        StaticPool.mdb.ExecuteCommand(updatecommand);
    }
}
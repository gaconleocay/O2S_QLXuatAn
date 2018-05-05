using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Helpers;

public partial class QLXuatAn_CardTypeDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                cbVehicleGroup.DataSource = StaticPool.mdb.FillData("select * from tblVehicleGroup");
                cbVehicleGroup.DataValueField = "VehicleGroupID";
                cbVehicleGroup.DataTextField = "VehicleGroupName";
                cbVehicleGroup.DataBind();

                cbLane.DataSource = StaticPool.mdb.FillData("select * from tblLane");
                cbLane.DataValueField = "LaneID";
                cbLane.DataTextField = "LaneName";
                cbLane.DataBind();

                // Thoi gian ban ngay
                dtpDaytimeFrom.Value = "8:00";
                dtpDaytimeTo.Value = "18:00";

                ViewState["OldCardGroup"] = "";

                div_alert.Visible = false;
                if (Request.QueryString["CardGroupID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_CardGroup", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_cardgroupdetail.InnerText = "Sửa";
                    ViewState["CardGroupID"] = Request.QueryString["CardGroupID"].ToString();
                    DataTable dtCardGroup = StaticPool.mdb.FillData("select * from tblCardGroup where CardGroupID = '" + ViewState["CardGroupID"].ToString() + "'");
                    if (dtCardGroup != null && dtCardGroup.Rows.Count > 0)
                    {
                        DataRow dr = dtCardGroup.Rows[0];
                        txtDescription.Text = dr["Description"].ToString();
                        txtCardGroupName.Text = dr["CardGroupName"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());

                        cbCardType.SelectedIndex = int.Parse(dr["CardType"].ToString());
                        cbVehicleGroup.Value = dr["VehicleGroupID"].ToString();
                        dtpDaytimeFrom.Value = dr["DaytimeFrom"].ToString();
                        dtpDaytimeTo.Value = dr["DaytimeTo"].ToString();

                        cbFormulation.SelectedIndex = int.Parse(dr["Formulation"].ToString());

                        txtEachFee.Text = dr["EachFee"].ToString();
                        txtBlock0.Text = dr["Block0"].ToString();
                        txtTime0.Text = dr["Time0"].ToString();
                        txtBlock1.Text = dr["Block1"].ToString();
                        txtTime1.Text = dr["Time1"].ToString();
                        txtBlock2.Text = dr["Block2"].ToString();
                        txtTime2.Text = dr["Time2"].ToString();
                        txtBlock3.Text = dr["Block3"].ToString();
                        txtTime3.Text = dr["Time3"].ToString();
                        txtBlock4.Text = dr["Block4"].ToString();
                        txtTime4.Text = dr["Time4"].ToString();
                        txtBlock5.Text = dr["Block5"].ToString();
                        txtTime5.Text = dr["Time5"].ToString();
                        txtTimePeriods.Text = dr["TimePeriods"].ToString();
                        txtCosts.Text = dr["Costs"].ToString();
                        chIsHaveMoneyExcessTime.Checked = bool.Parse(dr["IsHaveMoneyExcessTime"].ToString());
                        chEnableFree.Checked = bool.Parse(dr["EnableFree"].ToString());
                        txtFreeTime.Text = dr["FreeTime"].ToString();
                        chIsCheckPlate.Checked = bool.Parse(dr["IsCheckPlate"].ToString());
                        chIsHaveMoneyExpiredDate.Checked = bool.Parse(dr["IsHaveMoneyExpiredDate"].ToString());

                        string[] laneids = dr["LaneIDs"].ToString().Split(';');
                        if (laneids != null && laneids.Length > 0)
                        {
                            foreach (ListItem item in cbLane.Items)
                            {
                                for (int i = 0; i < laneids.Length; i++)
                                    if (item.Value == laneids[i])
                                        item.Selected = true;
                            }
                        }

                        string lanenames = "";
                        foreach (ListItem item in cbLane.Items)
                        {
                            if (item.Selected)
                            {
                                if (lanenames == "")
                                {
                                  
                                    lanenames = item.Text;
                                }
                                else
                                {
                                    lanenames = lanenames + "_" + item.Text;
                                }
                            }
                        }

                        ViewState["OldCardGroup"] = txtCardGroupName.Text +
                            ";" + txtDescription.Text +
                            ";" + cbCardType.Items[cbCardType.SelectedIndex].Text +
                            ";" + chIsHaveMoneyExcessTime.Checked.ToString() +
                            ";" + cbVehicleGroup.Items[cbVehicleGroup.SelectedIndex].Text +
                            ";" + lanenames +
                            ";" + chEnableFree.Checked.ToString() + "_" + txtFreeTime.Text+
                            ";" + dtpDaytimeFrom.Value +
                            ";" + dtpDaytimeTo.Value +
                            ";" + cbFormulation.Items[cbFormulation.SelectedIndex].Text +
                            ";" + txtEachFee.Text +
                            ";" + txtBlock0.Text.Replace(",", "") + "_" + txtTime0.Text +
                            ";" + txtBlock1.Text.Replace(",", "") + "_" + txtTime1.Text +
                            ";" + txtBlock2.Text.Replace(",", "") + "_" + txtTime2.Text +
                            ";" + txtBlock3.Text.Replace(",", "") + "_" + txtTime3.Text +
                            ";" + txtBlock4.Text.Replace(",", "") + "_" + txtTime4.Text +
                            ";" + txtBlock5.Text.Replace(",", "") + "_" + txtTime5.Text +
                            ";" + txtTimePeriods.Text.Replace(";", "_") +
                            ";" + txtCosts.Text.Replace(";", "_") +
                            ";" + chbInactive.Checked.ToString();

                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_List_CardGroup", "Inserts", "Parking"))
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

            if (txtEachFee.Text == "")
                txtEachFee.Text = "0";
            if (txtBlock0.Text == "")
                txtBlock0.Text = "0";
            if (txtTime0.Text == "")
                txtTime0.Text = "0";

            if (txtBlock1.Text == "")
                txtBlock1.Text = "0";
            if (txtTime1.Text == "")
                txtTime1.Text = "0";

            if (txtBlock2.Text == "")
                txtBlock2.Text = "0";
            if (txtTime2.Text == "")
                txtTime2.Text = "0";

            if (txtBlock3.Text == "")
                txtBlock3.Text = "0";
            if (txtTime3.Text == "")
                txtTime3.Text = "0";

            if (txtBlock4.Text == "")
                txtBlock4.Text = "0";
            if (txtTime4.Text == "")
                txtTime4.Text = "0";

            if (txtBlock5.Text == "")
                txtBlock5.Text = "0";
            if (txtTime5.Text == "")
                txtTime5.Text = "0";

            string laneids = "";
            string lanenames = "";
            foreach (ListItem item in cbLane.Items)
            {
                if (item.Selected)
                {
                    if (laneids == "")
                    {
                        laneids = item.Value;
                        lanenames = item.Text;
                    }
                    else
                    {
                        laneids = laneids + ";" + item.Value;
                        lanenames = lanenames + "_" + item.Text;
                    }
                }
            }

            if (laneids == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Bạn chưa phân quyền cho nhóm thẻ!";
                return;
            }

            if (ViewState["CardGroupID"] != null && ViewState["CardGroupID"].ToString() != "")
            {
                // Update
                if (StaticPool.mdb.ExecuteCommand("update tblCardGroup set Description = N'" + txtDescription.Text +
                    "', CardGroupName = N'" + txtCardGroupName.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    ", CardType = " + cbCardType.SelectedIndex +
                    ", VehicleGroupID = '" + cbVehicleGroup.Value +
                    "', DaytimeFrom = '" + dtpDaytimeFrom.Value +
                    "', DaytimeTo = '" + dtpDaytimeTo.Value +
                    "', Formulation = " + cbFormulation.SelectedIndex +
                    ", EachFee = " + txtEachFee.Text.Replace(",", "") +
                    ", Block0 = " + txtBlock0.Text.Replace(",", "") +
                    ", Time0 = " + txtTime0.Text +
                    ", Block1 = " + txtBlock1.Text.Replace(",", "") +
                    ", Time1 = " + txtTime1.Text +
                    ", Block2 = " + txtBlock2.Text.Replace(",", "") +
                    ", Time2 = " + txtTime2.Text +
                    ", Block3 = " + txtBlock3.Text.Replace(",", "") +
                    ", Time3 = " + txtTime3.Text +
                    ", Block4 = " + txtBlock4.Text.Replace(",", "") +
                    ", Time4 = " + txtTime4.Text +
                    ", Block5 = " + txtBlock5.Text.Replace(",", "") +
                    ", Time5 = " + txtTime5.Text +
                    ", TimePeriods = '" + txtTimePeriods.Text +
                    "', Costs = '" + txtCosts.Text +
                    "', LaneIDs = '" + laneids +
                    "', IsHaveMoneyExcessTime='" + chIsHaveMoneyExcessTime.Checked +
                    "', EnableFree='"+chEnableFree.Checked+
                    "', FreeTime='"+txtFreeTime.Text+
                    "', IsCheckPlate='"+chIsCheckPlate.Checked+
                    "', IsHaveMoneyExpiredDate='"+chIsHaveMoneyExpiredDate.Checked+
                    "' where CardGroupID = '" + ViewState["CardGroupID"].ToString() + "'", ref result))
                {
                    string _newcardgroup = txtCardGroupName.Text +
                            ";" + txtDescription.Text +
                            ";" + cbCardType.Items[cbCardType.SelectedIndex].Text +
                            ";" + chIsHaveMoneyExcessTime.Checked.ToString() +
                            ";" + cbVehicleGroup.Items[cbVehicleGroup.SelectedIndex].Text +
                            ";" + lanenames +
                            ";" + chEnableFree.Checked.ToString() + "_" + txtFreeTime.Text +
                            ";" + dtpDaytimeFrom.Value +
                            ";" + dtpDaytimeTo.Value +
                            ";" + cbFormulation.Items[cbFormulation.SelectedIndex].Text +
                            ";" + txtEachFee.Text +
                            ";" + txtBlock0.Text.Replace(",", "") + "_" + txtTime0.Text +
                            ";" + txtBlock1.Text.Replace(",", "") + "_" + txtTime1.Text +
                            ";" + txtBlock2.Text.Replace(",", "") + "_" + txtTime2.Text +
                            ";" + txtBlock3.Text.Replace(",", "") + "_" + txtTime3.Text +
                            ";" + txtBlock4.Text.Replace(",", "") + "_" + txtTime4.Text +
                            ";" + txtBlock5.Text.Replace(",", "") + "_" + txtTime5.Text +
                            ";" + txtTimePeriods.Text.Replace(";", "_") +
                            ";" + txtCosts.Text.Replace(";", "_") +
                            ";" + chbInactive.Checked.ToString();

                    string _des = StaticPool.GetStringChange(ViewState["OldCardGroup"].ToString(), _newcardgroup);

                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_CardGroup", txtCardGroupName.Text, "Sửa", _des);
                }
            }
            else
            {
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblCardGroup (Description, CardGroupName, Inactive, CardType, VehicleGroupID, DaytimeFrom, DaytimeTo, Formulation, EachFee, Block0, Time0, Block1, Time1, Block2, Time2, Block3, Time3, Block4, Time4, Block5, Time5,TimePeriods, Costs, LaneIDs, IsHaveMoneyExcessTime, EnableFree, FreeTime, IsCheckPlate, IsHaveMoneyExpiredDate) values(N'" +
                     txtDescription.Text + "', N'" +
                     txtCardGroupName.Text + "', " +
                     (chbInactive.Checked ? 1 : 0) + ", " +
                     cbCardType.SelectedIndex + ", '" +
                     cbVehicleGroup.Value + "', '" +
                     dtpDaytimeFrom.Value + "', '" +
                     dtpDaytimeTo.Value + "', " +
                     cbFormulation.SelectedIndex + ", " +
                     txtEachFee.Text.Replace(",", "") + ", " +
                     txtBlock0.Text.Replace(",", "") + ", " +
                     txtTime0.Text + ", " +
                     txtBlock1.Text.Replace(",", "") + ", " +
                     txtTime1.Text + ", " +
                     txtBlock2.Text.Replace(",", "") + ", " +
                     txtTime2.Text + ", " +
                     txtBlock3.Text.Replace(",", "") + ", " +
                     txtTime3.Text + ", " +
                     txtBlock4.Text.Replace(",", "") + ", " +
                     txtTime4.Text + ", " +
                     txtBlock5.Text.Replace(",", "") + ", " +
                     txtTime5.Text + ", '" +
                     txtTimePeriods.Text + "', '" +
                     txtCosts.Text + "', '" +
                     laneids +"', '"+
                     chIsHaveMoneyExcessTime.Checked+"', '"+
                     chEnableFree.Checked+"', '"+
                     txtFreeTime.Text+"', '"+
                     chIsCheckPlate.Checked+"', '"+
                     chIsHaveMoneyExpiredDate.Checked+
                     "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_List_CardGroup", txtCardGroupName.Text, "Thêm", "");
                    CacheLayer.Clear(StaticCached.c_tblCardGroup);
                }

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
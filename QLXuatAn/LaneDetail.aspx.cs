using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;
using Futech.Helpers;

public partial class QLXuatAn_LaneDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_lane2.Visible = false;

                ViewState["PC"] = "";

                ViewState["C1"] = "";
                ViewState["C2"] = "";
                ViewState["C3"] = "";
                ViewState["C4"] = "";
                ViewState["C5"] = "";
                ViewState["C6"] = "";

                ViewState["Controller"] = "";

                ViewState["OldLane"] = "";

                cbPC.DataSource = StaticPool.mdb.FillData("select (ComputerName + ' (' + IPAddress + ')') as ComputerName, PCID from tblPC");
                cbPC.DataValueField = "PCID";
                cbPC.DataTextField = "ComputerName";
                cbPC.DataBind();
                cbPC.SelectedIndex = 0;

                for (int i = 0; i < SystemUI.GetLaneTypes().Length; i++)
                {
                    cbLaneType.Items.Add(SystemUI.GetLaneTypes()[i]);
                }

                div_alert.Visible = false;
                if (Request.QueryString["LaneID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Lane", "Updates", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_lanedetail.InnerText = "Sửa làn vào/ra";
                    ViewState["LaneID"] = Request.QueryString["LaneID"].ToString();
                    DataTable dtLane = StaticPool.mdb.FillData("select * from tblLane where LaneID = '" + ViewState["LaneID"].ToString() + "'");
                    if (dtLane != null && dtLane.Rows.Count > 0)
                    {
                        DataRow dr = dtLane.Rows[0];
                        txtLaneName.Text = dr["LaneName"].ToString();
                        cbPC.SelectedValue = dr["PCID"].ToString();
                        cbLaneType.SelectedIndex = int.Parse(dr["LaneType"].ToString());
                        chbIsLoop.Checked = bool.Parse(dr["IsLoop"].ToString());
                        //chbIsCheckPlate.Checked = bool.Parse(dr["IsCheckPlate"].ToString());
                        chbIsPrint.Checked = bool.Parse(dr["IsPrint"].ToString());
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());
                        chIsFree.Checked = bool.Parse(dr["IsFree"].ToString());
                        chIsLED.Checked = bool.Parse(dr["IsLED"].ToString());
                        //chIsCheckPlateIn.Checked = bool.Parse(dr["IsCheckPlateIn"].ToString());
                        cbCheckPlateLevelIn.Value = dr["CheckPlateLevelIn"].ToString();
                        cbCheckPlateLevelOut.Value = dr["CheckPlateLevelOut"].ToString();
                        chAccessForEachSide.Checked = bool.Parse(dr["AccessForEachSide"].ToString());

                        ViewState["PC"] = dr["PCID"].ToString();

                        ViewState["C1"] = dr["C1"].ToString();
                        ViewState["C2"] = dr["C2"].ToString();
                        ViewState["C3"] = dr["C3"].ToString();
                        ViewState["C4"] = dr["C4"].ToString();
                        ViewState["C5"] = dr["C5"].ToString();
                        ViewState["C6"] = dr["C6"].ToString();

                        ViewState["Controller"] = dr["Controller"].ToString();

                        string _c1 = "";
                        if (cbC1.Items.Count > 0)
                            _c1 = cbC1.Items[cbC1.SelectedIndex].Text;
                        string _c2 = "";
                        if (cbC1.Items.Count > 0)
                            _c2 = cbC1.Items[cbC2.SelectedIndex].Text;
                        string _c3 = "";
                        if (cbC3.Items.Count > 0)
                            _c3 = cbC1.Items[cbC3.SelectedIndex].Text;
                        string _c4 = "";
                        if (cbC4.Items.Count > 0)
                            _c4 = cbC1.Items[cbC4.SelectedIndex].Text;
                        string _c5 = "";
                        if (cbC5.Items.Count > 0)
                            _c5 = cbC1.Items[cbC5.SelectedIndex].Text;
                        string _c6 = "";
                        if (cbC6.Items.Count > 0)
                            _c6 = cbC1.Items[cbC6.SelectedIndex].Text;

                        ViewState["OldLane"] = txtLaneName.Text +
                            ";" + cbPC.Items[cbPC.SelectedIndex].Text +
                            ";" + cbLaneType.Items[cbLaneType.SelectedIndex].Text +
                            ";" + _c1 +
                            ";" + _c2 +
                            ";" + _c3 +
                            ";" + _c4 +
                            ";" + _c5 +
                            ";" + _c6 +
                            ";" + cbCheckPlateLevelIn.Items[cbCheckPlateLevelIn.SelectedIndex].Text +
                            ";" + cbCheckPlateLevelOut.Items[cbCheckPlateLevelOut.SelectedIndex].Text +
                            ";" + chbIsLoop.Checked.ToString() +
                            ";" + chbIsPrint.Checked.ToString() +
                            ";" + chIsFree.Checked.ToString() +
                            ";" + chIsLED.Checked.ToString() +
                            ";"+chAccessForEachSide.Checked.ToString()+
                            ";" + chbInactive.Checked.ToString();

                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "Parking_Device_Lane", "Inserts", "Parking"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_lanedetail.InnerText = "Thêm làn vào/ra mới";
                    ViewState["LaneID"] = null;
                }

                //cbController.DataSource = StaticPool.mdb.FillData("select (ControllerName + ' (' + Comport + ')') as ControllerName, ControllerID from tblController where PCID = '" + cbPC.SelectedValue + "'");
                //cbController.DataValueField = "ControllerID";
                //cbController.DataTextField = "ControllerName";
                //cbController.DataBind();

                //if (ViewState["Controller"].ToString() != "")
                //    cbController.Value = ViewState["Controller"].ToString();

                DataTable dtCamera = StaticPool.mdb.FillData("select (CameraName + ' (' + HttpURL + ')') as CameraName, CameraID from tblCamera where PCID = '" + cbPC.SelectedValue + "'");
                cbC1.Items.Add(new ListItem("", ""));
                cbC2.Items.Add(new ListItem("", ""));
                cbC3.Items.Add(new ListItem("", ""));
                cbC4.Items.Add(new ListItem("", ""));
                cbC5.Items.Add(new ListItem("", ""));
                cbC6.Items.Add(new ListItem("", ""));
                foreach (DataRow dr in dtCamera.Rows)
                {
                    cbC1.Items.Add(new ListItem(dr["CameraName"].ToString(), dr["CameraID"].ToString()));
                    cbC2.Items.Add(new ListItem(dr["CameraName"].ToString(), dr["CameraID"].ToString()));
                    cbC3.Items.Add(new ListItem(dr["CameraName"].ToString(), dr["CameraID"].ToString()));
                    cbC4.Items.Add(new ListItem(dr["CameraName"].ToString(), dr["CameraID"].ToString()));
                    cbC5.Items.Add(new ListItem(dr["CameraName"].ToString(), dr["CameraID"].ToString()));
                    cbC6.Items.Add(new ListItem(dr["CameraName"].ToString(), dr["CameraID"].ToString()));
                }

                string str = ViewState["C1"].ToString();
                cbC1.Value = ViewState["C1"].ToString();
                cbC2.Value = ViewState["C2"].ToString();
                cbC3.Value = ViewState["C3"].ToString();
                cbC4.Value = ViewState["C4"].ToString();
                cbC5.Value = ViewState["C5"].ToString();
                cbC6.Value = ViewState["C6"].ToString();

                if (cbLaneType.SelectedIndex > 1)
                    div_lane2.Visible = true;

            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }
        }
    }

    protected void Selection_ChangePC(object sender, EventArgs e)
    {
        DataTable dtCamera = StaticPool.mdb.FillData("select (CameraName + ' (' + HttpURL + ')') as CameraName, CameraID from tblCamera where PCID = '" + cbPC.SelectedValue + "'");
        cbC1.Items.Clear();
        cbC1.Items.Add(new ListItem("", ""));
        cbC2.Items.Clear();
        cbC2.Items.Add(new ListItem("", ""));
        cbC3.Items.Clear();
        cbC3.Items.Add(new ListItem("", ""));
        cbC4.Items.Clear();
        cbC4.Items.Add(new ListItem("", ""));
        cbC5.Items.Clear();
        cbC5.Items.Add(new ListItem("", ""));
        cbC6.Items.Clear();
        cbC6.Items.Add(new ListItem("", ""));
        foreach (DataRow dr in dtCamera.Rows)
        {
            ListItem item = new ListItem();
            item.Value = dr["CameraID"].ToString();
            item.Text = dr["CameraName"].ToString();
            cbC1.Items.Add(item);
            cbC2.Items.Add(item);
            cbC3.Items.Add(item);
            cbC4.Items.Add(item);
            cbC5.Items.Add(item);
            cbC6.Items.Add(item);
        }

        //cbController.DataSource = StaticPool.mdb.FillData("select (ControllerName + ' (' + Comport + ')') as ControllerName, ControllerID from tblController where PCID = '" + cbPC.SelectedValue + "'");
        //cbController.DataValueField = "ControllerID";
        //cbController.DataTextField = "ControllerName";
        //cbController.DataBind();

        if (ViewState["PC"] != null && cbPC.SelectedValue == ViewState["PC"].ToString())
        {
            cbC1.Value = ViewState["C1"].ToString();
            cbC2.Value = ViewState["C2"].ToString();
            cbC3.Value = ViewState["C3"].ToString();
            cbC4.Value = ViewState["C4"].ToString();
            cbC5.Value = ViewState["C5"].ToString();
            cbC6.Value = ViewState["C6"].ToString();

            //cbController.Value = ViewState["Controller"].ToString();
        }

    }

    protected void Selection_ChangeLaneType(object sender, EventArgs e)
    {
        if (cbLaneType.SelectedIndex > 1)
            div_lane2.Visible = true;
        else
            div_lane2.Visible = false;

    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            div_alert.Visible = false;

            if (txtLaneName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên cổng vào/ra không được để trống!";
                return;
            }

            if (ViewState["LaneID"] != null && ViewState["LaneID"].ToString() != "")
            {
                // Update
                if (StaticPool.mdb.ExecuteCommand("update tblLane set LaneName = N'" + txtLaneName.Text +
                    "', PCID = '" + cbPC.SelectedValue +
                    "', LaneType = " + cbLaneType.SelectedIndex +
                    ", IsLoop = " + (chbIsLoop.Checked ? 1 : 0) +
                    ", CheckPlateLevelIn = '" + cbCheckPlateLevelIn.Value +
                    "', CheckPlateLevelOut='" + cbCheckPlateLevelOut.Value +
                    "', IsPrint = " + (chbIsPrint.Checked ? 1 : 0) +
                    ", Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    ", IsFree='" + chIsFree.Checked +
                    "', IsLED='" + chIsLED.Checked +
                    "', AccessForEachSide='"+chAccessForEachSide.Checked+
                    "', C1 = '" + cbC1.Value +
                    "', C2 = '" + cbC2.Value +
                    "', C3 = '" + cbC3.Value +
                    "', C4 = '" + cbC4.Value +
                    "', C5 = '" + cbC5.Value +
                    "', C6 = '" + cbC6.Value +
                    //"', Controller = '" + cbController.Value + 
                    "' where LaneID = '" + ViewState["LaneID"].ToString() + "'", ref result))
                {
                    string _c1 = "";
                    if (cbC1.Items.Count > 0)
                        _c1 = cbC1.Items[cbC1.SelectedIndex].Text;
                    string _c2 = "";
                    if (cbC1.Items.Count > 0)
                        _c2 = cbC1.Items[cbC2.SelectedIndex].Text;
                    string _c3 = "";
                    if (cbC3.Items.Count > 0)
                        _c3 = cbC1.Items[cbC3.SelectedIndex].Text;
                    string _c4 = "";
                    if (cbC4.Items.Count > 0)
                        _c4 = cbC1.Items[cbC4.SelectedIndex].Text;
                    string _c5 = "";
                    if (cbC5.Items.Count > 0)
                        _c5 = cbC1.Items[cbC5.SelectedIndex].Text;
                    string _c6 = "";
                    if (cbC6.Items.Count > 0)
                        _c6 = cbC1.Items[cbC6.SelectedIndex].Text;

                    string _newlane = txtLaneName.Text +
                        ";" + cbPC.Items[cbPC.SelectedIndex].Text +
                        ";" + cbLaneType.Items[cbLaneType.SelectedIndex].Text +
                        ";" + _c1 +
                        ";" + _c2 +
                        ";" + _c3 +
                        ";" + _c4 +
                        ";" + _c5 +
                        ";" + _c6 +
                        ";" + cbCheckPlateLevelIn.Items[cbCheckPlateLevelIn.SelectedIndex].Text +
                        ";" + cbCheckPlateLevelOut.Items[cbCheckPlateLevelOut.SelectedIndex].Text +
                        ";" + chbIsLoop.Checked.ToString() +
                        ";" + chbIsPrint.Checked.ToString() +
                        ";" + chIsFree.Checked.ToString() +
                        ";" + chIsLED.Checked.ToString() +
                        ";" + chAccessForEachSide.Checked.ToString() +
                        ";" + chbInactive.Checked.ToString();
                    string _des = StaticPool.GetStringChange(ViewState["OldLane"].ToString(), _newlane);
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Lane", txtLaneName.Text, "Sửa", _des);
                }
            }
            else
            {
                // Them moi
                if (StaticPool.mdb.ExecuteCommand("insert into tblLane (LaneName, LaneType, PCID, IsLoop, CheckPlateLevelIn, CheckPlateLevelOut, IsPrint, Inactive, C1, C2, C3, C4, C5, C6, IsFree, IsLED, AccessForEachSide) values(N'" +
                    txtLaneName.Text + "', " +
                    cbLaneType.SelectedIndex + ", '" +
                    cbPC.SelectedValue + "', " +
                    (chbIsLoop.Checked ? 1 : 0) + ", '" +
                    cbCheckPlateLevelIn.Value + "', '" +
                    cbCheckPlateLevelOut.Value + "', " +
                    (chbIsPrint.Checked ? 1 : 0) + ", " +
                    (chbInactive.Checked ? 1 : 0) + ", '" +
                    cbC1.Value + "', '" +
                    cbC2.Value + "', '" +
                    cbC3.Value + "', '" +
                    cbC4.Value + "', '" +
                    cbC5.Value + "', '" +
                    cbC6.Value + "', '" +
                    chIsFree.Checked + "', '" +
                    chIsLED.Checked +"', '"+
                    chAccessForEachSide.Checked+
                    //cbController.Value + 
                    "')", ref result))
                {
                    StaticPool.SaveLogFile(StaticPool.GetUserName(Request.Cookies["UserID"].Value.ToString()), "Parking", "Parking_Device_Lane", txtLaneName.Text, "Thêm", "");
                    CacheLayer.Clear(StaticCached.c_tblLane);
                }
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Lane.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
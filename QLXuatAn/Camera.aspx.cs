using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;

public partial class QLXuatAn_Camera : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                // Check xem nguoi dung nay co quyen truy cap chuc nang nay khong
                if (Request.Cookies["UserID"] != null)
                    ViewState["UserID"] = Request.Cookies["UserID"].Value.ToString();
                else
                    ViewState["UserID"] = "";
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "Parking_Device_Camera", "Selects", "Parking"))
                {
                    string result = "";
                    div_alert.Visible = false;

                    DataTable dtPC = StaticPool.mdb.FillData("select * from tblPC");
                    cbPC.Items.Add(new ListItem("<< Hiển thị tất cả >>", ""));
                    if (dtPC != null && dtPC.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtPC.Rows)
                        {
                            cbPC.Items.Add(new ListItem(dr["ComputerName"].ToString() + " (" + dr["IPAddress"].ToString() + ")", dr["PCID"].ToString()));
                        }
                    }

                    if (Request.QueryString["PCID"] != null && Request.QueryString["PCID"].ToString() != "")
                    {
                        ViewState["PCID"] = Request.QueryString["PCID"].ToString();
                        cbPC.Value = ViewState["PCID"].ToString();
                        rpt_Camera.DataSource = StaticPool.mdb.FillData("select * from dbo.tblCamera where PCID = '" + ViewState["PCID"].ToString() + "' order by PCID", ref result);
                    }
                    else
                    {
                        rpt_Camera.DataSource = StaticPool.mdb.FillData("select * from dbo.tblCamera order by PCID", ref result);
                    }

                    rpt_Camera.DataBind();
                    if (result != "")
                    {
                        div_alert.Visible = true;
                        id_alert.InnerText = result;
                    }
                }
                else
                {
                    Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }

    public string GetComputerName(string PCID)
    {
        try
        {
            DataTable dt = StaticPool.mdb.FillData("select * from dbo.tblPC where PCID = '" + PCID + "'");
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["ComputerName"].ToString() + " (" + dt.Rows[0]["IPAddress"].ToString() + ")";
            else
                return "";
        }
        catch
        {
            return "";
        }
    }

    public string GetStatus(string status)
    {
        if (!bool.Parse(status))
            return "<span class='label label-sm label-success'>Hoạt động</span>";
        else
            return "<span class='label label-sm label-warning'>Ngừng kích hoạt</span>";
    }

    [WebMethod]
    public static string Delete(string id, string userid)
    {
        try
        {
            if (StaticPool.CheckPermission(userid, "Parking_Device_Camera", "Deletes", "Parking")==false)
                return "Bạn không có quyền thực hiện chức năng này!";

            DataTable temp = StaticPool.mdb.FillData("select LaneID from tblLane where" +
                " C1='" + id +
                "' or C2='" + id +
                "' or C3='" + id +
                "' or C4='" + id +
                "' or C5='" + id +
                "' or C6='" + id +
                "'");
          
            if (temp != null && temp.Rows.Count > 0)
            {
                return "Camera đang sử dụng, không xóa được";
            }

            if (temp == null)
                return "Failed";

            string _cameraname = "";
            temp = StaticPool.mdb.FillData("select CameraName from tblCamera where CameraId='" + id + "'");
            if (temp != null && temp.Rows.Count > 0)
            {
                _cameraname = temp.Rows[0]["CameraName"].ToString();

            }

            if (StaticPool.mdb.ExecuteCommand("delete from tblCamera where CameraID = '" + id + "'"))
            {
                StaticPool.SaveLogFile(StaticPool.GetUserName(userid), "Parking", "Parking_Device_Camera", _cameraname, "Xóa", "id=" + id);
                return "true";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return "";
    }
}
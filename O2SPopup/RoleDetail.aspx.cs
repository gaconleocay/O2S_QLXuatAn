using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;
using System.Xml;
using System.Web.UI.HtmlControls;

public partial class O2SPopup_RoleDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                div_alert.Visible = false;
                if (Request.QueryString["RoleID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "O2SPopup_System_Role", "Updates", "KzBMS"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_roledetail.InnerText = "Sửa vai trò";
                    ViewState["RoleID"] = Request.QueryString["RoleID"].ToString();
                    DataTable dtRole = StaticPool.mdb.FillData("select * from tblRole where RoleID = '" + ViewState["RoleID"].ToString() + "'");
                    if (dtRole != null && dtRole.Rows.Count > 0)
                    {
                        DataRow dr = dtRole.Rows[0];
                        if (bool.Parse(dr["IsSystem"].ToString()))
                            Response.Redirect("Message.aspx?Message=" + "Đây là vai trò nghầm định của hệ thống. Bạn không được phép sửa vai trò này!", false);

                        txtDescription.Text = dr["Description"].ToString();
                        txtRoleName.Text = dr["RoleName"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "O2SPopup_System_Role", "Inserts", "KzBMS"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_roledetail.InnerText = "Thêm vai trò mới";
                    ViewState["RoleID"] = null;
                }

                DisplaySubSystem();

            }
            catch (Exception ex)
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = ex.Message;
            }
        }
    }

    public void DisplaySubSystem()
    {
        DataTable dtSubSystem = StaticPool.mdb.FillData("select * from tblSubSystem where AppCode = 'KzBMS' order by SortOrder");
        // create an XmlDocument (with an XML declaration)
        XmlDocument XDoc = new XmlDocument();
        XmlDeclaration XDec = XDoc.CreateXmlDeclaration("1.0", null, null);
        XDoc.AppendChild(XDec);

        DataTable itemTable = new DataTable("SubSystem");
        itemTable.Columns.Add("SubSystemID");
        itemTable.Columns.Add("ParentID");
        itemTable.Columns.Add("SubSystemCode");
        itemTable.Columns.Add("SubSystemName");
        itemTable.Columns.Add("Selects");
        itemTable.Columns.Add("Inserts");
        itemTable.Columns.Add("Updates");
        itemTable.Columns.Add("Deletes");
        itemTable.Columns.Add("Exports");

        CreateXmlDocumentFromDataTable(dtSubSystem, "", null, ref XDoc, ref itemTable);

        // we cannot bind the TreeView directly to an XmlDocument
        // so we must create an XmlDataSource and assign the XML text
        XmlDataSource XDdataSource = new XmlDataSource();
        XDdataSource.ID = DateTime.Now.Ticks.ToString();  // unique ID is required
        XDdataSource.Data = XDoc.OuterXml;

        rpt_SubSystem.DataSource = itemTable;
        rpt_SubSystem.DataBind();
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
                CreateXmlDocumentFromDataTable(dt, row["SubSystemID"].ToString(), InsertNode(row, parentNode, ref XDoc, ref itemTable), ref XDoc, ref itemTable);
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
            XmlElement NewNode = XDoc.CreateElement("_" + Row["SubSystemID"].ToString());
            NewNode.SetAttribute("SubSystemID", Row["SubSystemID"].ToString());
            NewNode.SetAttribute("ParentID", Row["ParentID"].ToString());
            NewNode.SetAttribute("SubSystemCode", Row["SubSystemCode"].ToString());
            NewNode.SetAttribute("SubSystemName", Row["SubSystemName"].ToString());
            NewNode.SetAttribute("Selects", Row["Selects"].ToString());
            NewNode.SetAttribute("Inserts", Row["Inserts"].ToString());
            NewNode.SetAttribute("Updates", Row["Updates"].ToString());
            NewNode.SetAttribute("Deletes", Row["Deletes"].ToString());
            NewNode.SetAttribute("Exports", Row["Exports"].ToString());

            itemTable.Rows.Add(Row["SubSystemID"].ToString(), Row["ParentID"].ToString(), Row["SubSystemCode"].ToString(), Row["SubSystemName"].ToString(), Row["Selects"].ToString(), Row["Inserts"].ToString(), Row["Updates"].ToString(), Row["Deletes"].ToString(), Row["Exports"].ToString());
            if (parentNode == null)
                XDoc.AppendChild(NewNode);  // root node
            else
                parentNode.AppendChild(NewNode);
        }
        catch (Exception ex)
        {

        }
        return XDoc.SelectSingleNode(String.Format("//*[@SubSystemID=\"{0}\"]", Row["SubSystemID"].ToString()));
    }

    protected void rpt_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        DataRowView drv = (DataRowView)e.Item.DataItem;
        if (drv != null)
        {
            bool selects = false, inserts = false, updates = false, deletes = false, exports = false;
            if (ViewState["RoleID"] != null && ViewState["RoleID"].ToString() != "")
            {
                DataTable dtRolePermissionMaping = StaticPool.mdb.FillData("select * from tblRolePermissionMaping where RoleID = '" + ViewState["RoleID"].ToString() + "' and SubSystemID = '" + drv["SubSystemID"].ToString() + "'");
                string str = "select * from tblRolePermissionMaping where RoleID = '" + ViewState["RoleID"].ToString() + "' and SubSystemID = '" + drv["SubSystemID"].ToString() + "'";
                if (dtRolePermissionMaping != null && dtRolePermissionMaping.Rows.Count > 0)
                {
                    DataRow dr = dtRolePermissionMaping.Rows[0];
                    selects = bool.Parse(dr["Selects"].ToString());
                    inserts = bool.Parse(dr["Inserts"].ToString());
                    updates = bool.Parse(dr["Updates"].ToString());
                    deletes = bool.Parse(dr["Deletes"].ToString());
                    exports = bool.Parse(dr["Exports"].ToString());
                }
            }

            // Selects
            var checkbox = e.Item.FindControl("chbSelects") as HtmlInputCheckBox;
            if (checkbox != null && !bool.Parse(drv["Selects"].ToString()))
                checkbox.Visible = false;
            else if (checkbox != null)
                checkbox.Checked = selects;


            // Inserts
            checkbox = e.Item.FindControl("chbInserts") as HtmlInputCheckBox;
            if (checkbox != null && !bool.Parse(drv["Inserts"].ToString()))
                checkbox.Visible = false;
            else if (checkbox != null)
                checkbox.Checked = inserts;

            // Updates
            checkbox = e.Item.FindControl("chbUpdates") as HtmlInputCheckBox;
            if (checkbox != null && !bool.Parse(drv["Updates"].ToString()))
                checkbox.Visible = false;
            else if (checkbox != null)
                checkbox.Checked = updates;

            // Deletes
            checkbox = e.Item.FindControl("chbDeletes") as HtmlInputCheckBox;
            if (checkbox != null && !bool.Parse(drv["Deletes"].ToString()))
                checkbox.Visible = false;
            else if (checkbox != null)
                checkbox.Checked = deletes;

            // Exports
            checkbox = e.Item.FindControl("chbExports") as HtmlInputCheckBox;
            if (checkbox != null && !bool.Parse(drv["Exports"].ToString()))
                checkbox.Visible = false;
            else if (checkbox != null)
                checkbox.Checked = exports;
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            div_alert.Visible = false;

            if (txtRoleName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên vai trò không được để trống!";
                return;
            }

            if (ViewState["RoleID"] != null && ViewState["RoleID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblRole set Description = N'" + txtDescription.Text +
                    "', RoleName = N'" + txtRoleName.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    " where RoleID = '" + ViewState["RoleID"].ToString() + "'", ref result);
            }
            else
            {
                // Them moi
                ViewState["RoleID"] = Guid.NewGuid().ToString();
                StaticPool.mdb.ExecuteCommand("insert into tblRole (RoleID, Description, RoleName, Inactive, AppCode) values('" +
                    ViewState["RoleID"].ToString() + "', N'" +
                    txtDescription.Text + "', N'" +
                    txtRoleName.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) + ", '" +
                    "KzBMS" + 
                    "')", ref result);
            }

            // Update phan quyen
            StaticPool.mdb.ExecuteCommand("delete from tblRolePermissionMaping where RoleID = '" + ViewState["RoleID"].ToString() + "'");
            foreach (RepeaterItem rpitem in rpt_SubSystem.Items)
            {
                string SubSystemID = "";
                HiddenField hfid = (HiddenField)rpitem.FindControl("SubSystemID");
                if (hfid != null)
                    SubSystemID = hfid.Value;

                bool selects = false, inserts = false, updates = false, deletes = false, exports = false;

                //select
                var chkselects = rpitem.FindControl("chbSelects") as HtmlInputCheckBox;
                if (chkselects != null && chkselects.Checked)
                    selects = true;

                //insert
                var chkinsert = rpitem.FindControl("chbInserts") as HtmlInputCheckBox;
                if (chkinsert != null && chkinsert.Checked)
                    inserts = true;

                //update
                var chkupdate = rpitem.FindControl("chbUpdates") as HtmlInputCheckBox;
                if (chkupdate != null && chkupdate.Checked)
                    updates = true;

                //delete
                var chkdelete = rpitem.FindControl("chbDeletes") as HtmlInputCheckBox;
                if (chkdelete != null && chkdelete.Checked)
                    deletes = true;

                //export
                var chkexport = rpitem.FindControl("chbExports") as HtmlInputCheckBox;
                if (chkexport != null && chkexport.Checked)
                    exports = true;

                StaticPool.mdb.ExecuteCommand("insert into tblRolePermissionMaping (RoleID, SubSystemID, Selects, Inserts, Updates, Deletes, Exports) values('" +
                    ViewState["RoleID"].ToString() + "', '" +
                    SubSystemID + "', " +
                    (selects ? 1 : 0) + ", " +
                    (inserts ? 1 : 0) + ", " +
                    (updates ? 1 : 0) + ", " +
                    (deletes ? 1 : 0) + ", " +
                    (exports ? 1 : 0) +
                    ")", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("Role.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
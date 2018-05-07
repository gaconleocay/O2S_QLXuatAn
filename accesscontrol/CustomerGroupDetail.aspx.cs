using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using Futech.Tools;
using System.Xml;

public partial class accesscontrol_CustomerGroupDetail : System.Web.UI.Page
{
    private string codeTable = "CustomerGroup";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                //DataTable dtParent = StaticPool.mdb.FillData("select * from tblCustomerGroup order by CustomerGroupID");
                //cbParent.Items.Add(new ListItem("", ""));
                //if (dtParent != null && dtParent.Rows.Count > 0)
                //{
                //    foreach (DataRow dr in dtParent.Rows)
                //    {
                //        cbParent.Items.Add(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
                //    }
                //}
                DisplayCustomerGroup();

                div_alert.Visible = false;
                if (Request.QueryString["CustomerGroupID"] != null)
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_List_CustomerGroup", "Updates", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_customergroupdetail.InnerText = "Sửa máy tính";
                    ViewState["CustomerGroupID"] = Request.QueryString["CustomerGroupID"].ToString();
                    DataTable dtCustomerGroup = StaticPool.mdb.FillData("select * from tblCustomerGroup where CustomerGroupID = '" + ViewState["CustomerGroupID"].ToString() + "'");
                    if (dtCustomerGroup != null && dtCustomerGroup.Rows.Count > 0)
                    {
                        DataRow dr = dtCustomerGroup.Rows[0];
                        txtCustomerGroupName.Text = dr["CustomerGroupName"].ToString();
                        cbParent.Value = dr["ParentID"].ToString();
                        txtDescription.Text = dr["Description"].ToString();
                        chbInactive.Checked = bool.Parse(dr["Inactive"].ToString());

                        cbParent.Items.Remove(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
                    }
                }
                else
                {
                    if (Request.Cookies["UserID"] == null || !StaticPool.CheckPermission(Request.Cookies["UserID"].Value.ToString(), "AccessControl_List_CustomerGroup", "Inserts", "AccessControl"))
                        Response.Redirect("Message.aspx?Message=" + "Bạn không có quyền thực hiện chức năng này!", false);

                    id_customergroupdetail.InnerText = "Thêm nhóm xe mới";
                    ViewState["CustomerGroupID"] = null;
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
                CreateXmlDocumentFromDataTable(dt, row[codeTable + "ID"].ToString(), InsertNode(row, parentNode, ref XDoc, ref itemTable), ref XDoc, ref itemTable);
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

        DataTable dtParent = itemTable;
        cbParent.Items.Add(new ListItem("", ""));
        if (dtParent != null && dtParent.Rows.Count > 0)
        {
            foreach (DataRow dr in dtParent.Rows)
            {
                cbParent.Items.Add(new ListItem(dr["CustomerGroupName"].ToString(), dr["CustomerGroupID"].ToString()));
            }
        }
    }

    protected void Save(object sender, EventArgs e)
    {
        try
        {
            string result = "";
            div_alert.Visible = false;

            if (txtCustomerGroupName.Text == "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = "Tên nhóm khách hàng không được để trống!";
                return;
            }

            if (ViewState["CustomerGroupID"] != null && ViewState["CustomerGroupID"].ToString() != "")
            {
                // Update
                StaticPool.mdb.ExecuteCommand("update tblCustomerGroup set CustomerGroupName = N'" + txtCustomerGroupName.Text +
                    "', ParentID = '" + cbParent.Value + 
                    "', Description = '" + txtDescription.Text +
                    "', Inactive = " + (chbInactive.Checked ? 1 : 0) +
                    " where CustomerGroupID = '" + ViewState["CustomerGroupID"].ToString() + "'", ref result);
                div_alert.Visible = true;
            }
            else
            {
                // Them moi
                StaticPool.mdb.ExecuteCommand("insert into tblCustomerGroup (CustomerGroupName, ParentID, Description, Inactive) values(N'" +
                    txtCustomerGroupName.Text + "', '" +
                    cbParent.Value + "', '" + 
                    txtDescription.Text + "', " +
                    (chbInactive.Checked ? 1 : 0) +
                    ")", ref result);
            }

            if (result != "")
            {
                // Hien thi thong bao loi
                div_alert.Visible = true;
                id_alert.InnerText = result;
            }
            else
                Response.Redirect("CustomerGroup.aspx");
        }
        catch (Exception ex)
        {
            // Hien thi thong bao loi
            div_alert.Visible = true;
            id_alert.InnerText = ex.Message;
        }
    }

}
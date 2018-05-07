using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Web.Services;
using Futech.Tools;
using System.Xml;

public partial class accesscontrol_CustomerGroup : System.Web.UI.Page
{
    private string codeTable = "CustomerGroup";

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
                if (StaticPool.CheckPermission(ViewState["UserID"].ToString(), "AccessControl_List_CustomerGroup", "Selects", "AccessControl"))
                {
                    //rpt_CustomerGroup.DataSource = StaticPool.mdb.FillData("select * from tblCustomerGroup");
                    //rpt_CustomerGroup.DataBind();
                    DisplayCustomerGroup();
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

        rpt_CustomerGroup.DataSource = itemTable;
        rpt_CustomerGroup.DataBind();
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
            if (StaticPool.CheckPermission(userid, "AccessControl_List_CustomerGroup", "Deletes", "AccessControl"))
                return StaticPool.mdb.ExecuteCommand("delete from tblCustomerGroup where CustomerGroupID = '" + id + "'").ToString().ToLower();
            else
                return "Bạn không có quyền thực hiện chức năng này!";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
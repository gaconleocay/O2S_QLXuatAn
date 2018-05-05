using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Services;
using System.Web.UI.WebControls;

public partial class QLXuatAn_CardSubDetail : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            try
            {
                if (Request.QueryString["SubCardId"] != null)
                {
                    ViewState["SubCardId"] = Request.QueryString["SubCardId"].ToString();
                    DataTable dt = StaticPool.mdb.FillData("select * from [tblSubCard] where Id = '" + ViewState["SubCardId"] + "'");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow model = dt.Rows[0];
                        if (model["MainCard"] != DBNull.Value && !string.IsNullOrWhiteSpace(model["MainCard"].ToString()))
                        {
                            txtMainCard.Value = model["MainCard"].ToString();
                            ViewState["MainCard"] = txtMainCard.Value;
                        }

                        if (model["CardNo"] != DBNull.Value && !string.IsNullOrWhiteSpace(model["CardNo"].ToString()))
                            txtCardNo.Value = model["CardNo"].ToString();
                        if (model["CardNumber"] != DBNull.Value && !string.IsNullOrWhiteSpace(model["CardNumber"].ToString()))
                            txtCardNumber.Value = model["CardNumber"].ToString();

                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
    protected void Save(object sender, EventArgs e)
    {
        ViewState["errSubCard"] = "";

        try
        {
            if (string.IsNullOrWhiteSpace(txtCardNumber.Value))
            {
                ViewState["errSubCard"] = "Mã thẻ phụ không để trống";
                txtCardNumber.Focus();
                return;
            }

            //Insert
            var sb = new StringBuilder();
            if (ViewState["SubCardId"] == null)
            {
                sb.AppendLine(string.Format("IF((SELECT COUNT(*) FROM [dbo].[tblSubCard] WHERE [CardNumber] = '{0}') <= 0) BEGIN", txtCardNumber.Value));
                sb.AppendLine("INSERT INTO [dbo].[tblSubCard]");
                sb.AppendLine("([MainCard]");
                sb.AppendLine(",[CardNo]");
                sb.AppendLine(",[CardNumber]");
                sb.AppendLine(",[IsDelete])");
                sb.AppendLine("VALUES");
                sb.AppendLine(string.Format("('{0}','{1}','{2}',0)", txtMainCard.Value, txtCardNo.Value, txtCardNumber.Value));
                sb.AppendLine("END");
            }
            else
            {
                sb.AppendLine("UPDATE [dbo].[tblSubCard]");
                sb.AppendLine(string.Format("SET [MainCard]='{0}'", txtMainCard.Value));
                sb.AppendLine(string.Format(",[CardNo]='{0}'", txtCardNo.Value));
                sb.AppendLine(string.Format(",[CardNumber]='{0}'", txtCardNumber.Value));
                sb.AppendLine(string.Format("WHERE id = {0}", ViewState["SubCardId"].ToString()));
            }

            StaticPool.mdb.ExecuteCommand(sb.ToString());

        }
        catch (Exception ex)
        {
            ViewState["errSubCard"] = ex.Message;
            return;
        }
        Response.Redirect("CardSub.aspx");
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
            commandstring = "select top 10 CardNumber, CardNo from tblCard where CardNumber like N'%" + name + "%' OR CardNo LIKE N'%" + name + "%' order by CardNumber, CardNo ";
        else
            commandstring = "select top 10 CardNumber, CardNo from tblCard  order by CardNumber,CardNo";

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
}
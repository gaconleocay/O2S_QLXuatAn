using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class QLXuatAn_TestProgess : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
            ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
        }       
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
        //ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
        // Add Fake Delay to simulate long running process.

        //System.Threading.Thread.Sleep(5000);
        LoadCustomers();
    }

    private void LoadCustomers()
    {
        //string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //string query = "SELECT * FROM tblCustomer";// WHERE Country = @Country OR @Country = ''";
        //SqlCommand cmd = new SqlCommand(query);
        ////cmd.Parameters.AddWithValue("@Country", ddlCountries.SelectedItem.Value);
        //using (SqlConnection con = new SqlConnection(strConnString))
        //{
        //    using (SqlDataAdapter sda = new SqlDataAdapter())
        //    {
        //        cmd.Connection = con;
        //        sda.SelectCommand = cmd;
        //        using (DataSet ds = new DataSet())
        //        {
        //            sda.Fill(ds, "Customers");
        //            gvCustomers.DataSource = ds;
        //            gvCustomers.DataBind();
        //        }
        //    }
        //}

        //string script = "$(document).ready(function () { $('[id*=btnSubmit]').click(); });";
        //ClientScript.RegisterStartupScript(this.GetType(), "load", script, true);
        //System.Threading.Thread.Sleep(5000);

        for (int i = 0; i < 5000; i++)
        {
            //System.Threading.Thread.Sleep(1);
            gvCustomers.DataSource = StaticPool.mdb.FillData("select * from tblCustomer");
        }

        //gvCustomers.DataSource = StaticPool.mdb.FillData("select * from tblCustomer");
        gvCustomers.DataBind();
        
    }
}
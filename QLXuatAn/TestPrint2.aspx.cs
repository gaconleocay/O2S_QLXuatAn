﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class QLXuatAn_TestPrint2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Company.InnerText = "Công ty: Futech";
        Address.InnerText = "Địa chỉ: Ha Noi";
        Tax.InnerText = "MST: 12345678";
    }
}
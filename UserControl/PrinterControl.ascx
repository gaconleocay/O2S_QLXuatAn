<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PrinterControl.ascx.cs" Inherits="UserControl_PrinterControl" %>
<div class="boxPrintHeader">
    <style>
        .tblPrintControl td{
            padding:5px;
        }
    </style>
    <table class="tblPrintControl">
        <tr>
            <td>Từ dòng
            </td>
            <td>
                <a href="javascript:void(0)" onclick="btnBackPagePrint();">
                    <i class="glyphicon glyphicon-menu-left" style="font-size: 25px;line-height: 34px;"></i>
                </a>
            </td>
            <td>
                <input type="text" class="form-control" style="width: 50px;" value="<%=ViewState["PageIndex"] %>" name="txtbRow" />
            </td>
            <td>đến dòng </td>
            <td>
                <div class="clearfix">
                    <div class="pull-left">
                        <input type="text" class="form-control" style="width: 50px;"  value="<%=ViewState["PageNext"] %>" name="txteRow"/>
                    </div>
                    <div class="pull-left">
                        <span style="line-height: 34px; display: block; padding-left: 5px;">/<span name="boxTotalItem"><%=ViewState["TotalItem"] %></span></span>
                    </div>
                </div>
            </td>
            <td>
                <a href="javascript:void(0)" onclick="btnNextPagePrint();">
                    <i class="glyphicon glyphicon-menu-right" style="font-size: 25px;line-height: 34px;"></i>
                </a>
            </td>
            <td>
                <button type="button" class="btn btn-sm btn-primary" style="padding: 4px 30px;" onclick="btnFilterPagePrint();">Lọc</button>
            </td>
            <td>
                <button class="btn btn-sm btn-grey" onclick="printDiv('listDataAjax')">In</button>
            </td>
            <%--<td>
                <asp:Button ID="btnExportPDF" class="btn btn-sm btn-warning" runat="server" Text="PDF" OnClick="btnExportPDF_Click" OnClientClick="btnPDFExport()"/>
                <asp:HiddenField ID = "hfGridHtml" runat = "server" />
            </td>--%>
            <%--<td>
                <button class="btn btn-sm btn-purple">WORD</button>
            </td>--%>
        </tr>
    </table>
</div>



<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="PrintFileReportDetailMoneyCardMonth_v2.aspx.cs" Inherits="QLXuatAn_PrintFileReportDetailMoneyCardMonth_v2" %>

<%@ Register Src="~/UserControl/PrinterControl.ascx" TagPrefix="uc1" TagName="PrinterControl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" runat="Server">
    <form id="form1" runat="server">
        <i>(Tối đa in <%=ViewState["pageSize"] %> dòng mỗi lần)</i>
        <uc1:PrinterControl runat="server" ID="PrinterControl" />
        <div class="" id="listDataAjax">
            <style>
                .table-bordered > tbody > tr > th, .table-bordered > tbody > tr > td {
                    border: 1px solid #ddd;
                    padding: 8px;
                    line-height: 1.42857143;
                    vertical-align: top;
                }
            </style>
            <h2 style="font-family: Arial">Chi tiết thu tiền thẻ tháng</h2>
            <div id="div_information" style="margin: 10px auto; font-family: Arial" runat="server">
                Từ ngày <strong><%=Request.QueryString["FromDate"].ToString() %></strong> đến ngày <strong><%=Request.QueryString["ToDate"].ToString() %></strong>
            </div>
            <asp:Repeater ID="rpt_Card" runat="server">
                <HeaderTemplate>
                    <table class="table table-striped table-bordered table-hover">
                        <thead>
                            <tr>
                                <th>STT</th>
                                <th>Ngày thực hiện</th>
                                <th>Mã thẻ</th>
                                <th class="hidden-480">Nhóm thẻ</th>
                                <th class="hidden-480">Biển số</th>
                                <th class="hidden-480">Khách hàng</th>
                                <th class="hidden-480">Thời hạn cũ</th>
                                <th class="hidden-480">Thời hạn mới</th>
                                <th class="hidden-480">Số tiền</th>
                                <th class="hidden-480">NV thực hiện</th>

                            </tr>
                        </thead>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr>
                        <td><%#(Container.ItemIndex+1).ToString()%></td>
                        <td><%# StaticPool.GetDateTime(((System.Data.DataRowView)Container.DataItem)["Date"].ToString()) %></td>

                        <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNumber"]%></td>
                        <td class="hidden-480">
                            <%#StaticPool.GetCardGroup(((System.Data.DataRowView)Container.DataItem)["CardGroupID"].ToString())%>
                        </td>
                        <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Plate"]%></td>
                        <td class="hidden-480">
                            <%#GetCustomerName(((System.Data.DataRowView)Container.DataItem)["CustomerID"].ToString())%>
                        </td>
                        <td class="hidden-480"><%#Convert.ToDateTime(((System.Data.DataRowView)Container.DataItem)["OldExpireDate"].ToString()).ToString("dd/MM/yyyy")%></td>
                        <td class="hidden-480"><%#Convert.ToDateTime(((System.Data.DataRowView)Container.DataItem)["NewExpireDate"].ToString()).ToString("dd/MM/yyyy")%></td>
                        <td class="hidden-480"><%#Convert.ToDecimal(((System.Data.DataRowView)Container.DataItem)["FeeLevel"]).ToString("###,###")%></td>
                        <td class="hidden-480">
                            <%#StaticPool.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserID"].ToString())%>
                        </td>
                        
                    </tr>
                </ItemTemplate>

                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </form>
</asp:Content>


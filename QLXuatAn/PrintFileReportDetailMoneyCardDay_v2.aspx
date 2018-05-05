<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="PrintFileReportDetailMoneyCardDay_v2.aspx.cs" Inherits="QLXuatAn_PrintFileReportDetailMoneyCardDay_v2"  ValidateRequest="false" %>

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
                .table-bordered > tbody > tr > th,.table-bordered > tbody > tr > td {
                    border: 1px solid #ddd;
                    padding: 8px;
                    line-height: 1.42857143;
                    vertical-align: top;
                }
            </style>
            <h2 style="font-family: Arial">Chi tiết thu tiền thẻ lượt </h2>
            <div id="div_information" style="margin: 10px auto;font-family: Arial" runat="server">
                Từ ngày <strong><%=Request.QueryString["FromDate"].ToString() %></strong> đến ngày <strong><%=Request.QueryString["ToDate"].ToString() %></strong>
            </div>
            <table class="table table-bordered" style="font-family: Arial;font-size:12px;border-collapse: collapse;border:solid 1px #ddd;">
                <asp:Repeater ID="rpt_Card" runat="server">
                    <HeaderTemplate>
                        <tr>
                            <th>STT</th>
                            <th>CardNo</th>
                            <th>Mã thẻ</th>
                            <th>Biển số</th>
                            <th>Thời gian vào</th>
                            <th>Thời gian ra</th>
                            <th class="hidden-480">Nhóm thẻ</th>
                            <th class="hidden-480">Khách hàng</th>
                            <th class="hidden-480">Làn vào</th>
                            <th class="hidden-480">Làn ra</th>
                            <th class="hidden-480">Giám sát vào</th>
                            <th class="hidden-480">Giám sát ra</th>
                            <th class="hidden-480">Số tiền</th>
                            <th class="hidden-480">Tổng TG</th>
                        </tr>
                    </HeaderTemplate>

                    <ItemTemplate>
                        <%--<div class="convert_tbl_row rowLoading">
                    <div>
                         <i class="ace-icon fa fa-spinner fa-spin orange bigger-125"></i><span class="NhapNhayText">đang tải dữ liệu ...</span>
                    </div>
                </div>--%>
                        <tr>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["RowNumber"] %></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNo"] %></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNumber"] %></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["Plate"]%></td>
                            <td><%# StaticPool.GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeIn"].ToString()) %></td>
                            <td><%# StaticPool.GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeOut"].ToString()) %></td>
                            <td class="hidden-480"><%#StaticPool.GetCardGroup(((System.Data.DataRowView)Container.DataItem)["CardGroupID"].ToString())%></td>
                            <td class="hidden-480"><%# ((System.Data.DataRowView)Container.DataItem)["CustomerName"] %></td>
                            <td class="hidden-480"><%#StaticPool.GetLane(((System.Data.DataRowView)Container.DataItem)["LaneIDIn"].ToString())%></td>
                            <td class="hidden-480"><%#StaticPool.GetLane(((System.Data.DataRowView)Container.DataItem)["LaneIDOut"].ToString())%></td>
                            <td class="hidden-480"><%#StaticPool.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserIDIn"].ToString())%></td>
                            <td class="hidden-480"><%#StaticPool.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserIDOut"].ToString())%></td>
                            <td class="hidden-480"><%# Convert.ToDecimal(((System.Data.DataRowView)Container.DataItem)["Moneys"]).ToString("###,###") %></td>
                            <td class="hidden-480"><%# ((System.Data.DataRowView)Container.DataItem)["TotalTimes"] %></td>
                        </tr>
                    </ItemTemplate>

                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>

            </table>
        </div>
    </form>
</asp:Content>


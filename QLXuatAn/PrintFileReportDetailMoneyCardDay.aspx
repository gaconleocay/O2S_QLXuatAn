﻿<%@ Page Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="PrintFileReportDetailMoneyCardDay.aspx.cs" Inherits="QLXuatAn_PrintFileReportDetailMoneyCardDay" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {



        });

        function printDiv(divID) {
            //Get the HTML of div
            var divElements = document.getElementById(divID).innerHTML;
            //Get the HTML of whole page
            var oldPage = document.body.innerHTML;

            //Reset the page's HTML with div's HTML only
            document.body.innerHTML =
                "<html><head><title></title></head><body>" +
                divElements + "</body>";

            //Print Page
            window.print();

            //Restore orignal HTML
            document.body.innerHTML = oldPage;


        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" runat="Server">
    <ul class="breadcrumb">
        <li>
            <i class="ace-icon fa fa-home home-icon"></i>
            <a href="#">Trang chủ</a>
        </li>
        <li class="active"> In file </li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" runat="Server">
    <div class="page-header">
        <h1 id="id_carddetail" runat="server">In PDF
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" runat="Server">
    <div class="col-xs-12">
        <div class="row">
            <div class="widget-body">
                <div class="widget-main">
                    <div class="form-group">
                        <div class="col-sm-2 no-padding-left">
                            <button type="button" id="btnAddCamera" onclick="printDiv('Print')" class="btn btn-info btn-sm">
                                <i class="fa fa-print"></i>
                                Print
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="hr hr-16 hr-dotted"></div>
        <div id="Print">
            <asp:Repeater ID="rpt_Card" runat="server">
                <HeaderTemplate>
                    <h2 style="text-align: center">Chi tiết thu tiền thẻ lượt </h2>
                    <div id="div_information" style="margin: 10px auto" runat="server">
                        Từ ngày <strong><%=Request.QueryString["FromDate"].ToString() %></strong> đến ngày <strong><%=Request.QueryString["ToDate"].ToString() %></strong>
                    </div>

                    <table class="table table-bordered">
                        <thead>
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
                        </thead>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr>
                        <td><%#(Container.ItemIndex+1).ToString()%></td>
                        <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNo"] %></td>
                        <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNumber"] %></td>
                        <td><%# ((System.Data.DataRowView)Container.DataItem)["Plate"]%></td>
                        <td><%# GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeIn"].ToString()) %></td>
                        <td><%# GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeOut"].ToString()) %></td>

                        <td class="hidden-480">
                            <%#((System.Data.DataRowView)Container.DataItem)["CardGroupID"].ToString()%>
                        </td>
                        <td><%# ((System.Data.DataRowView)Container.DataItem)["CustomerName"] %></td>
                        <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["LaneIDIn"]%></td>
                        <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["LaneIDOut"]%></td>
                        <td class="hidden-480">
                            <%#((System.Data.DataRowView)Container.DataItem)["UserIDIn"]%>
                        </td>
                        <td class="hidden-480">
                            <%#((System.Data.DataRowView)Container.DataItem)["UserIDOut"]%>
                        </td>
                        <td><%# Convert.ToDecimal(((System.Data.DataRowView)Container.DataItem)["Moneys"]).ToString("###,###") %></td>
                        <td class="hidden-480"><%# ((System.Data.DataRowView)Container.DataItem)["TotalTimes"] %></td>
                    </tr>
                </ItemTemplate>

                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

        </div>
    </div>
</asp:Content>




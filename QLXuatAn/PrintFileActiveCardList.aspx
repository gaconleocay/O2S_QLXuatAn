<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintFileActiveCardList.aspx.cs" Inherits="QLXuatAn_PrintFileActiveCardList" MasterPageFile="~/QLXuatAn/MasterPage.master" %>
<%@ Register Src="~/UserControl/PrinterControl.ascx" TagPrefix="uc1" TagName="PrinterControl" %>
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
    <%--<div class="page-header">
        <h1 id="id_carddetail" runat="server">In danh sách thẻ đã gia hạn
        </h1>
    </div>--%>
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
            <h2 style="font-family: Arial">Danh sách thẻ gia hạn</h2>
            <div id="div_information" style="margin: 10px auto;font-family: Arial" runat="server">
                Từ ngày <strong><%=Request.QueryString["FromDate"].ToString() %></strong> đến ngày <strong><%=Request.QueryString["ToDate"].ToString() %></strong>
            </div>
            <table class="table table-bordered table-hover" style="font-family: Arial;font-size:12px;border-collapse: collapse;border:solid 1px #ddd;">
                <asp:Repeater id="rpt_Card" runat="server">
                            <HeaderTemplate>
		                            <thead>
			                            <tr>
                                            <th>STT</th>
				                            <th>Ngày thực hiện</th>
                                            <th>Số thẻ</th>
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
                                    <td class="STT"><%# ((System.Data.DataRowView)Container.DataItem)["STT"]%></td>
				                   <td><%# DateTime.Parse(((System.Data.DataRowView)Container.DataItem)["Ngày tháng"].ToString()).ToString("dd/MM/yyyy HH:mm") %></td>
                                  <td><%# ((System.Data.DataRowView)Container.DataItem)["Số thẻ"]%></td>
                                    <td><%# ((System.Data.DataRowView)Container.DataItem)["Mã thẻ"]%></td>
                                    <td class="hidden-480">
                                        <%#StaticPool.GetCardGroup(((System.Data.DataRowView)Container.DataItem)["Nhóm thẻ"].ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Biển số"]%></td>
                                     <td class="hidden-480">
                                        <%#((System.Data.DataRowView)Container.DataItem)["Tên KH"].ToString()%>
				                    </td>
                                     <td class="hidden-480"><%#DateTime.Parse(((System.Data.DataRowView)Container.DataItem)["Thời hạn cũ"].ToString()).ToString("dd/MM/yyyy")%></td>
                                    <td class="hidden-480"><%#DateTime.Parse(((System.Data.DataRowView)Container.DataItem)["Thời hạn mới"].ToString()).ToString("dd/MM/yyyy")%></td>
                                 	<td class="hidden-480"><%#Convert.ToDecimal(((System.Data.DataRowView)Container.DataItem)["Mức phí"].ToString()).ToString("##,###").Replace(".",",")%></td>	
                                    <td class="hidden-480">
                                        <%#StaticPool.GetUserName(((System.Data.DataRowView)Container.DataItem)["Người dùng"].ToString())%>
				                    </td>		                    
			                    </tr>
                            </ItemTemplate>

                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:Repeater>

            </table>
        </div>
    </form>

    <div class="col-xs-12">
        <%--<div class="row">
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
        <div class="hr hr-16 hr-dotted"></div>--%>
        <div id="Print">
            

        </div>
    </div>
</asp:Content>





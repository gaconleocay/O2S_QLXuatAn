﻿<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ReportVehicleFreeDetail.aspx.cs" Inherits="QLXuatAn_ReportVehicleFreeDetail" %>

<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_report").addClass("active");
            $(".id_page_report").addClass("open");

            $(".id_page_report5").addClass("open");
            $(".id_page_report5_1").addClass("active");
           // $(".id_page_report5").addClass("open");
           // $(".id_page_reportdetailcardday").addClass("active");

            if (!ace.vars['old_ie']) $('#<%=dtpFromDate.ClientID%>').datetimepicker({
                //format: 'MM/DD/YYYY h:mm:ss A',//use this option to display seconds

                format: 'DD/MM/YYYY HH:mm',
                icons: {
                    time: 'fa fa-clock-o',
                    date: 'fa fa-calendar',
                    up: 'fa fa-chevron-up',
                    down: 'fa fa-chevron-down',
                    previous: 'fa fa-chevron-left',
                    next: 'fa fa-chevron-right',
                    today: 'fa fa-arrows ',
                    clear: 'fa fa-trash',
                    close: 'fa fa-times'
                }
            }).next().on(ace.click_event, function () {
                $(this).prev().focus();
            });

            if (!ace.vars['old_ie']) $('#<%=dtpToDate.ClientID%>').datetimepicker({
                //format: 'MM/DD/YYYY h:mm:ss A',//use this option to display seconds
                format: 'DD/MM/YYYY HH:mm',
                icons: {
                    time: 'fa fa-clock-o',
                    date: 'fa fa-calendar',
                    up: 'fa fa-chevron-up',
                    down: 'fa fa-chevron-down',
                    previous: 'fa fa-chevron-left',
                    next: 'fa fa-chevron-right',
                    today: 'fa fa-arrows ',
                    clear: 'fa fa-trash',
                    close: 'fa fa-times'
                }
            }).next().on(ace.click_event, function () {
                $(this).prev().focus();
            });

            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("ActiveCardList.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
                        if (result.d.toString() == "true") {
                            element.remove();
                        }
                        else
                            alert(result.d.toString());
                    });
                }

            });

            $('#btnRefresh').click(function (e) {
                e.preventDefault();
                window.location.href = 'ReportVehicleFreeDetail.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&FromDate=' + document.getElementById("<%= dtpFromDate.ClientID %>").value +
                    '&ToDate=' + document.getElementById("<%= dtpToDate.ClientID %>").value +
                    '&LaneID=' + document.getElementById("<%= cbLane.ClientID %>").value +
                    '&UserID=' + document.getElementById("<%= cbUser.ClientID %>").value;

            });


        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
<ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Lượt vào ra miễn phí</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
<div class="page-header">
        <h1 id="id_cardlist" runat="server">
	        Lượt vào ra miễn phí
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
 <div class="alert alert-info" id="div_alert" runat="server">
		<button type="button" class="close" data-dismiss="alert">
			<i class="ace-icon fa fa-times"></i>
		</button>
        <i class="ace-icon fa fa-exclamation-triangle"></i>
		<span id="id_alert" runat = "server"></span>
		<br />
	</div>

    <div class="col-xs-12">
        <form id="frm1" class="form-horizontal" runat="server">
        <div class="col-xs-12">
            <div class="row">
                <div class="widget-body">
				    <div class="widget-main">
                    
                        <div class="form-group">
                            <label class="col-sm-1 control-label no-padding-right" for="form-field-1"> Từ ngày </label>
				            <div class="col-sm-2">
					            <div class="input-group">					        
                                    <input id="dtpFromDate" type="text" class="form-control" runat="server" />
								    <span class="input-group-addon">
									    <i class="fa fa-clock-o bigger-110"></i>
								    </span>                                                   
					            </div>                                        
				            </div>

                            <label class="col-sm-1 control-label no-padding-right" for="form-field-1"> Đến ngày </label>
				            <div class="col-sm-2">
					            <div class="input-group">
						            <input id="dtpToDate" type="text" class="form-control" runat="server" />
								    <span class="input-group-addon">
									    <i class="fa fa-clock-o bigger-110"></i>
								    </span>   
					            </div>
				            </div>
                    
                      
                        
                        </div>
                     
                        <div class="form-group">
                            <div class="col-sm-2 no-padding-left">      
                                <select class="form-control" id="cbCardGroup" runat = "server">
                                    
                                </select> 
                            </div>

                            <div class="col-sm-2 no-padding-left">      
                                <select class="form-control" id="cbLane" runat = "server">
                                    
                                </select> 
                            </div>
                            <div class="col-sm-2 no-padding-left">      
                                <select class="form-control" id="cbUser" runat = "server">
                                    
                                </select> 
                            </div>

                            <div class="col-sm-2 no-padding-left"">
                                <input type="text" id="txtKeyWord" placeholder="Từ khóa tìm kiếm" class="form-control" runat="server" />
				            </div>
                    
                            <div class="col-sm-1 no-padding-left">    
                                <button type="button" class="btn btn-white btn-default" id="btnRefresh">
                                    <i class="fa fa-refresh"></i>
                                    Nạp lại
                                </button>
                               
                            </div>
                            <div class="col-sm-1 no-padding-left">    
                                <asp:LinkButton ID="LinkButton1" OnClick="Excel_Click" runat="server" class="btn btn-white btn-default export">
                                    <i class="fa fa-file-excel-o green"></i>
				                    <span class="">Excel</span>
                                </asp:LinkButton>
                            </div>
                        </div>

				    </div>
			    </div>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <div class="widget-box">
			        <div class="widget-main no-padding">
                        <asp:Repeater id="rpt_Card" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
                                            <th>STT</th>
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
                                           
				                          
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
                                    <td><%#(Container.ItemIndex+1).ToString()%></td>
				                    <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNumber"] %></td>          
                                    <td><%# ((System.Data.DataRowView)Container.DataItem)["Plate"]%></td>
                                    <td><%# GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeIn"].ToString()) %></td>
                                    <td><%# GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeOut"].ToString()) %></td>

                                    <td class="hidden-480">
                                        <%#this.GetCardGroup(((System.Data.DataRowView)Container.DataItem)["CardGroupID"].ToString())%>
				                    </td>
                                     <td><%# ((System.Data.DataRowView)Container.DataItem)["CustomerName"] %></td>
                                    <td class="hidden-480"><%#GetLane(((System.Data.DataRowView)Container.DataItem)["LaneIDIn"].ToString())%></td>
                                    <td class="hidden-480"><%#GetLane(((System.Data.DataRowView)Container.DataItem)["LaneIDOut"].ToString())%></td>
                                    <td class="hidden-480">
                                        <%#this.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserIDIn"].ToString())%>
				                    </td>
                                     <td class="hidden-480">
                                        <%#this.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserIDOut"].ToString())%>
				                    </td>	
                                     <td><%# Convert.ToDecimal(((System.Data.DataRowView)Container.DataItem)["Moneys"]).ToString("###,###") %></td>		                    
				                   
			                    </tr>
                            </ItemTemplate>

                            <FooterTemplate>
	                            </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="space-10"></div>
    <div class="dataTables_wrapper">
               <div class="row">
                   <cc1:CollectionPager ID="pager" runat="server" PageSize="20" ShowLabel="false" MaxPages="10000"></cc1:CollectionPager>
               </div>
            </div>
                
                    <%--<table>
                        <tr>
                            <td colspan="5">
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" width="80">
                                <asp:LinkButton ID="lnkFirst" runat="server" class="btn btn-white btn-default" OnClick="lnkFirst_Click"><i class="ace-icon fa fa-angle-double-left"></i></asp:LinkButton>
                            </td>
                            <td align="center" valign="top" width="80">
                                <asp:LinkButton ID="lnkPrevious" runat="server" class="btn btn-white btn-default" OnClick="lnkPrevious_Click"><i class="ace-icon fa fa-angle-left"></i></asp:LinkButton>
                            </td>
                            <td>
                                <asp:DataList ID="RepeaterPaging" runat="server" 
                                    OnItemCommand="RepeaterPaging_ItemCommand" 
                                    OnItemDataBound="RepeaterPaging_ItemDataBound" RepeatDirection="Horizontal">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="Pagingbtn" runat="server" class="btn btn-white btn-default hidden-480"
                                            CommandArgument='<%#DataBinder.Eval(Container.DataItem, "PageIndex")%>' CommandName="newpage" 
                                            Text='<%#DataBinder.Eval(Container.DataItem, "PageText")%>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:DataList>
                            </td>
                            <td align="center" valign="top" width="80">
                                <asp:LinkButton ID="lnkNext" runat="server" class="btn btn-white btn-default" OnClick="lnkNext_Click"><i class="ace-icon fa fa-angle-right"></i></asp:LinkButton>
                            </td>
                            <td align="center" valign="top" width="80">
                                <asp:LinkButton ID="lnkLast" runat="server" class="btn btn-white btn-default" OnClick="lnkLast_Click"><i class="ace-icon fa fa-angle-double-right"></i></asp:LinkButton>
                            </td>
                        </tr>             
                        <tr>
                            <td align="center" valign="top" width="80">
                                &nbsp;</td>
                            <td align="center" valign="top" width="80">
                                &nbsp;</td>
                            <td align="center">
                                <asp:Label ID="lblpage" runat="server" Text=""></asp:Label>
                            </td>
                            <td align="center" valign="top" width="80">
                                &nbsp;</td>
                            <td align="center" valign="top" width="80">
                                &nbsp;</td>
                        </tr>
                    </table>--%>
               

            </div>
        </div>
         </form>
    </div><!-- /.col -->
</asp:Content>


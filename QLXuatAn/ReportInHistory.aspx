﻿<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ReportInHistory.aspx.cs" Inherits="ReportInHistory" %>
<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_report1").addClass("active");
            $(".id_page_report1").addClass("open");

            $(".id_page_report").addClass("active");
            $(".id_page_report").addClass("open");

            $(".Parking_Report_ReportInHistory").addClass("active");



            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("ReportIn.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
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
                window.location.href = 'ReportInHistory.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&FromDate=' + document.getElementById("<%= dtpFromDate.ClientID %>").value +
                    '&ToDate=' + document.getElementById("<%= dtpToDate.ClientID %>").value +
                    '&IsHaveTime=' + document.getElementById("<%= chIsHaveTime.ClientID %>").checked +
                    '&LaneID=' + document.getElementById("<%= cbLane.ClientID %>").value +
                    '&UserID=' + document.getElementById("<%= cbUser.ClientID %>").value;
            });


        });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Báo cáo xe trong bãi</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_reportin" runat="server">
	        Báo cáo xe trong bãi
        </h1>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_PageContent" Runat="Server">

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
                                <input id="dtpFromDate" type="text" class="form-control datePicker" runat="server" />
								<span class="input-group-addon">
									<i class="fa fa-clock-o bigger-110"></i>
								</span>                                                   
					        </div>                                        
				        </div>

                        <label class="col-sm-1 control-label no-padding-right" for="form-field-1"> Đến ngày </label>
				        <div class="col-sm-2">
					        <div class="input-group">
						        <input id="dtpToDate" type="text" class="form-control datePicker" runat="server" />
								<span class="input-group-addon">
									<i class="fa fa-clock-o bigger-110"></i>
								</span>   
					        </div>
				        </div>
                    
                        <div class="col-sm-2">  
				            <label>
				                <input type="checkbox" class="ace" id="chIsHaveTime" runat="server" checked/>
				                <span class="lbl"> Lọc theo thời gian</span>
			               </label>
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
                                            <th>CardNo</th>
                                            <th>Mã thẻ</th>
                                            <th>Biển số</th>
                                            <th>Thời gian vào</th>
                                            <th>Ảnh vào</th>
                                            <th class="hidden-480">Nhóm thẻ</th>                                           
                                            <th class="hidden-480">Khách hàng</th>
                                            <th class="hidden-480">Làn vào</th>
                                            <th class="hidden-480">Giám sát vào</th>                                         
				                            <th class="hidden-480"></th>
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
                                  
                                     <td class="ace-thumbnails clearfix">
                                        <a href="GetImageFromFile.ashx?FileName=<%#DataBinder.Eval(Container.DataItem, "PicIn1")%>" title="<%#GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeIn"].ToString())%>" data-rel="colorbox">
										    <img width="25" height="25" alt="25x25" src="GetImageFromFile.ashx?FileName=<%#DataBinder.Eval(Container.DataItem, "PicIn1")%>" />
									    </a>
                                        <a href="GetImageFromFile.ashx?FileName=<%#DataBinder.Eval(Container.DataItem, "PicIn2")%>" title="<%#GetDateTime(((System.Data.DataRowView)Container.DataItem)["DatetimeIn"].ToString())%>" data-rel="colorbox">
										    <img width="25" height="25" alt="25x25" src="GetImageFromFile.ashx?FileName=<%#DataBinder.Eval(Container.DataItem, "PicIn2")%>" />
									    </a>
                                    </td>
                                    

                                    <td class="hidden-480">
                                        <%#this.GetCardGroup(((System.Data.DataRowView)Container.DataItem)["CardGroupID"].ToString())%>
				                    </td>
                                     <td><%# ((System.Data.DataRowView)Container.DataItem)["CustomerName"] %></td>
                                    <td class="hidden-480"><%#GetLane(((System.Data.DataRowView)Container.DataItem)["LaneIDIn"].ToString())%></td>
                                  
                                    <td class="hidden-480">
                                        <%#this.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserIDIn"].ToString())%>
				                    </td>                     
				                     <td id="<%#((System.Data.DataRowView)Container.DataItem)["Id"]%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-danger delete">
							                        <i class="ace-icon fa fa-trash-o bigger-120"></i>
						                        </button>
					                        </div>
                                        </div>

                                        <div class="hidden-md hidden-lg">
									        <div class="inline pos-rel">
										        <button class="btn btn-minier btn-primary dropdown-toggle" data-toggle="dropdown" data-position="auto">
											        <i class="ace-icon fa fa-cog icon-only bigger-110"></i>
										        </button>

										        <ul class="dropdown-menu dropdown-only-icon dropdown-yellow dropdown-menu-right dropdown-caret dropdown-close">
											       
											        <li>
												        <a href="#" class="tooltip-success delete" data-rel="tooltip" title="Xóa">
													        <span class="green">
														        <i class="ace-icon fa fa-trash-o bigger-120"></i>
													        </span>
												        </a>
											        </li>
										        </ul>
									        </div>
								        </div>

				                    </td>
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
            </div>
        </div>
         </form>
    </div><!-- /.col -->    
   
    
</asp:Content>
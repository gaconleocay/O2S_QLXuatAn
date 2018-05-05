<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportTotalCustomer2.aspx.cs" MasterPageFile="~/QLXuatAn/MasterPage.master" Inherits="QLXuatAn_ReportTotalCustomer2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_report").addClass("active");
            $(".id_page_report").addClass("open");
            $(".id_page_report9").addClass("active");
            $(".id_page_report9").addClass("open");
            $(".id_page_reportcustomer_total").addClass("active");

            if (!ace.vars['touch']) {
                $('.chosen-select').chosen({ allow_single_deselect: true });
                //resize the chosen on window resize

                $(window)
					.off('resize.chosen')
					.on('resize.chosen', function () {
					    $('.chosen-select').each(function () {
					        var $this = $(this);
					        $this.next().css({ 'width': $this.parent().width() });
					    })
					}).trigger('resize.chosen');
                //resize chosen on sidebar collapse/expand
                $(document).on('settings.ace.chosen', function (e, event_name, event_val) {
                    if (event_name != 'sidebar_collapsed') return;
                    $('.chosen-select').each(function () {
                        var $this = $(this);
                        $this.next().css({ 'width': $this.parent().width() });
                    })
                });
            }

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
                window.location.href = 'ReportTotalCustomer2.aspx?FromDate=' + document.getElementById("<%= dtpFromDate.ClientID %>").value +
                    '&ToDate=' + document.getElementById("<%= dtpToDate.ClientID %>").value +
                    '&CompartmentId=' + document.getElementById("<%= cbCompartment.ClientID %>").value +
                    '&IsFilterByTimeRegister=' + document.getElementById("<%= chFilterByTimeRegister.ClientID %>").checked
                    ;
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
		<li class="active">Báo cáo xe ra</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
<div class="page-header">
        <h1 id="id_cardlist" runat="server">
	        Báo cáo thẻ xe
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
                    
                            <div class="col-sm-3">
                                <div class="radio">
						            <label>
							            <input name="form-field-radio" type="radio" id="chFilterByTimeRegister" class="ace" runat="server" />
							            <span class="lbl"> Lọc theo thời gian ngày đăng ký</span>
						            </label>

					            </div>
                            </div>

                            <div class="col-sm-3">
					            <div class="radio">
						            <label>
							            <input name="form-field-radio" type="radio" id="chFilterByTimeRelease" class="ace" runat="server" checked/>
							            <span class="lbl"> Lọc theo thời gian phát</span>
						            </label>
					            </div>
                            </div>  
                        
                        </div>
                     
                        <div class="form-group">
                            <div class="col-sm-2 no-padding-left"">
                                <select id="cbCompartment" class="chosen-select" runat="server">
                                </select>
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
                        <asp:Repeater id="rpt_ReportCustomerDetail2" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
                                            <th class="center">STT</th>
                                            <th class="center">Căn</th>
                                            <th class="center">Ôtô đăng ký</th>
                                            <th class="center">Ôtô hủy</th>
                                            <th class="center">Ôtô sử dụng</th>
                                            <th class="center">Xe máy đăng ký</th>
                                            <th class="center">Xe máy hủy</th>
                                            <th class="center">Xe máy sử dụng</th>
                                            <th class="center">Xe đạp đăng ký</th>
                                            <th class="center">Xe đạp hủy</th>
                                            <th class="center">Xe đạp sử dụng</th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
                                    <td class="center"><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td class="text-left"><%# ((System.Data.DataRowView)Container.DataItem)["CompartmentName"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountRegistedCar"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountLockCar"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountUseCar"]%></td>

                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountRegistedMotorcycle"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountLockMotorcycle"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountUseMotorcycle"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountRegistedBicycle"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountLockBicycle"]%></td>
                                    <td class="text-right"><%# ((System.Data.DataRowView)Container.DataItem)["CountUseBicycle"]%></td>
                                   
			                    </tr>
                            </ItemTemplate>

                            <FooterTemplate>
	                            </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="space-10"></div>
    
                
                    <table>
                        <tr>
                            <td colspan="6">
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
                    </table>
               

            </div>
        </div>
         </form>
    </div><!-- /.col -->
</asp:Content>
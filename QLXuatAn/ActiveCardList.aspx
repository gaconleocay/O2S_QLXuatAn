<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ActiveCardList.aspx.cs" Inherits="QLXuatAn_ActiveCardList" %>

<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

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

            $("#nav-customer-input").focus(function () {
                this.select();
            });
            $("#nav-customer-input").change(function () {
                if ($(this).val() == '') {
                    $('#<%=cbCustomer.ClientID%>').val('');
                }
            });
            $("#nav-customer-input").autocomplete({
                source: function (request, response) {
                    var itemnamecodes = new Array();
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'Card.aspx/getCustomerByAutocomplete',
                        dataType: "json",
                        data: "{ 'name': '" + request.term + "'}",
                        success: function (result) {
                            //debugger;
                            for (var i = 0; i < result.d.length; i++) {
                                //alert(result.d[i].Text + '-' + result.d[i].Value);
                                itemnamecodes[i] = { label: result.d[i].Text, Id: result.d[i].Value };
                            }
                            response(itemnamecodes);
                        },
                        error: function (result) {
                            //debugger;
                            alert(result);
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    //debugger;
                    $('#<%=cbCustomer.ClientID%>').val(ui.item.Id);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });


            $(".id_page_active_card").addClass("active");
            $(".id_page_active_card").addClass("open");
            $(".id_page_active_list").addClass("active");


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
                window.location.href = 'ActiveCardList.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value +
                    '&CustomerID=' + $('#<%=cbCustomer.ClientID%>').val() +
                    '&cusName=' + $('#nav-customer-input').val() +
                    '&FromDate=' + document.getElementById("<%= dtpFromDate.ClientID %>").value +
                    '&ToDate=' + document.getElementById("<%= dtpToDate.ClientID %>").value;
            });

            $('#btnPrintFile').click(function (e) {
                e.preventDefault();
                window.location.href = 'PrintFileActiveCardList.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value +
                    '&CustomerID=' + $('#<%=cbCustomer.ClientID%>').val()+
                    '&FromDate=' + document.getElementById("<%= dtpFromDate.ClientID %>").value +
                    '&ToDate=' + document.getElementById("<%= dtpToDate.ClientID %>").value;
            });

            $('a[name=btnCaNoSort]').click(function (e) {
                e.preventDefault();
                var cmd = $(this);
                var _val = cmd.find('input[name=hidCaSort]').val();
                if (_val == 'asc') {
                    _val = 'desc';
                } else if (_val == 'desc') {
                    _val = 'asc';
                }
                window.location.href = 'ActiveCardList.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value +
                    '&CustomerID=' + $('#<%=cbCustomer.ClientID%>').val() +
                    '&CaNoSort=' + _val +
                    '&Page=<%=ViewState["Page"]%>' +
                    '&cusName=' + $('#nav-customer-input').val() +
                    '&FromDate=' + document.getElementById("<%= dtpFromDate.ClientID %>").value +
                    '&ToDate=' + document.getElementById("<%= dtpToDate.ClientID %>").value;
            });



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

        });
    </script>
    <style>
        .cSort{
            color:#337ab7!important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
<ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Thẻ</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
 <div class="page-header">
        <h1 id="id_cardlist" runat="server">
	        Danh sách thẻ
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
 <%--Hien thi thong bao tai day--%>
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
                                <select class="form-control" id="cbCustomerGroup" runat = "server">
                                    
                                </select> 
                            </div>

                             <div class="col-sm-2 no-padding-left"> 
				                <%--<select class="chosen-select form-control" id="cbCustomer" data-placeholder="Select customer..."  runat = "server">
                               
                                </select>--%> 					                   
                                  <div class="plcCustomerId">
                                    <input type="text" placeholder="Nhập tên hoặc mã KH" class="nav-search-input" id="nav-customer-input" value="<%=ViewState["cusName"] %>"/>
                                    <input type="hidden" name="cbCustomer" id="cbCustomer" runat="server" value="" />
                                </div>
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

                            <div class="col-sm-1 no-padding-left">    
                                <button type="button" class="btn btn-white btn-default" id="btnPrintFile">
                                    <i class="fa fa-print"></i>
                                    In ấn
                                </button>
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
				                            <th>Ngày thực hiện</th>
                                            <th>
                                                <a href="javácript:void(0)" style="color:#707070;" name="btnCaNoSort">
                                                    Số thẻ
                                                    
                                                    <%if (ViewState["CaNoSort"].ToString() == "asc")
                                                        { %>
                                                    <i class="glyphicon glyphicon-sort-by-attributes-alt cSort" title="Số thẻ giảm dần"></i>
                                                    <%}
    else
    { %>
                                                    <i class="glyphicon glyphicon-sort-by-attributes cSort" title="Số thẻ tăng dần"></i>
                                                    <%} %>
                                                    <input type="hidden" name="hidCaSort" value="<%=ViewState["CaNoSort"].ToString() %>" />
                                                </a>
                                            </th>
                                            <th>Mã thẻ
                                            </th>
                                            <th class="hidden-480">Nhóm thẻ</th>
                                            <th class="hidden-480">Biển số</th>
                                            <th class="hidden-480">Khách hàng</th>
                                            <th class="hidden-480">Địa chỉ</th>
                                            <th class="hidden-480">Thời hạn cũ</th>
                                            <th class="hidden-480">Thời hạn mới</th>
				                            <th class="hidden-480">Số tiền</th>
                                            <th class="hidden-480">NV thực hiện</th>
				                            <th></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
                                    <td class="center">
                                        <%# Container.ItemIndex + 1 %>
                                    </td>
				                   <td><%# DateTime.Parse(((System.Data.DataRowView)Container.DataItem)["Date"].ToString()).ToString("dd/MM/yyyy HH:mm") %></td>
                                  <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNo"]%></td>
                                    <td><%# ((System.Data.DataRowView)Container.DataItem)["CardNumber"]%></td>
                                    <td class="hidden-480">
                                        <%#this.GetCardGroup(((System.Data.DataRowView)Container.DataItem)["CardGroupID"].ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Plate"]%></td>
                                     <td class="hidden-480">
                                        <%#((System.Data.DataRowView)Container.DataItem)["CustomerName"].ToString()%>
				                    </td>
                                    <td class="hidden-480">
                                        <%#((System.Data.DataRowView)Container.DataItem)["CustomerAddress"].ToString()%>
				                    </td>
                                     <td class="hidden-480"><%#DateTime.Parse(((System.Data.DataRowView)Container.DataItem)["OldExpireDate"].ToString()).ToString("dd/MM/yyyy")%></td>
                                    <td class="hidden-480"><%#DateTime.Parse(((System.Data.DataRowView)Container.DataItem)["NewExpireDate"].ToString()).ToString("dd/MM/yyyy")%></td>
                                 	<td class="hidden-480"><%#Convert.ToDecimal(((System.Data.DataRowView)Container.DataItem)["FeeLevel"].ToString()).ToString("##,###").Replace(".",",")%></td>	
                                    <td class="hidden-480">
                                        <%#StaticPool.GetUserName(((System.Data.DataRowView)Container.DataItem)["UserID"].ToString())%>
				                    </td>		                    
				                    <td id="<%# ((System.Data.DataRowView)Container.DataItem)["ID"]%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-danger delete" title="Xóa" >
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


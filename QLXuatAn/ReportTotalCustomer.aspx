<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ReportTotalCustomer.aspx.cs" Inherits="QLXuatAn_ReportTotalCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_report").addClass("active");
            $(".id_page_report").addClass("open");
            $(".id_page_report9").addClass("open");
            $(".id_page_reportcustomer_total").addClass("active");
          

          
            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("Customer.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
                        if (result.d.toString() == "true") {
                            element.remove();
                        }
                        else
                            alert(result.d.toString());
                    });
                }

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

            $('#btnRefresh').click(function (e) {
                e.preventDefault();
                window.location.href = 'ReportTotalCustomer.aspx?CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value;

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
		<li class="active">Khách hàng</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_customerlist" runat="server">
	       BC tổng hợp khách hàng
        </h1>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_PageContent" Runat="Server">
 
    <div class="col-xs-12">
        <form id="frm1" class="form-horizontal" runat="server">
        <div class="col-xs-12">
	    <div class="row">
            <div class="widget-body">
				<div class="widget-main">

                       
					
                        <div class="form-group">
                           <%-- <label class="col-sm-1 control-label no-padding-right" for="form-field-1"> Từ ngày </label>
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
				            </div>--%>
                       
                            <div class="col-sm-2 no-padding-left">      
                                <select class="chosen-select form-control" id="cbCustomerGroup" runat = "server">
                                    
                                </select> 
                            </div>
                             <%--<div class="col-sm-2 no-padding-left"">
                                <input type="text" id="txtKeyWord" placeholder="Từ khóa tìm kiếm" class="form-control" runat="server" />
				            </div>--%>
                       
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
                        <asp:Repeater id="rpt_Customer" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
                                            <th>STT</th>
                                            <th>Nhóm khách hàng</th>
                                            <th class="hidden-480">Ôtô_Đăng ký</th>
                                            <th class="hidden-480">Ôtô_Hủy</th>
                                            <th class="hidden-480">Ôtô_Sử dụng</th>
				                            <th class="hidden-480">XM_ Đăng ký</th>
                                            <th class="hidden-480">XM_Hủy</th>
                                            <th class="hidden-480">XM_Sử dụng</th>
                                            <th class="hidden-480">XĐĐ_ Đăng ký</th>
                                            <th class="hidden-480">XĐĐ_Hủy</th>
                                            <th class="hidden-480">XĐĐ_Sử dụng</th>
                                           
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CustomerGroupName")%></td>     
                                    <td class="hidden-480">
                                        <%#DataBinder.Eval(Container.DataItem, "CarReg").ToString()%>
				                    </td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "CarLock")%></td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "CarUsing")%></td>
				                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "MotorReg")%></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["MotorLock"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["MotorUsing"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["BicycleReg"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["BicycleLock"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["BicycleUsing"].ToString() %></td>
				                   
			                    </tr>
                            </ItemTemplate>

                            <FooterTemplate>
	                            </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <div class="space-10"></div>
    
                <%--<form id="frm1" class="form-horizontal" runat="server">--%>
                    <table>
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
                    </table>
               <%-- </form>--%>

            </div>
        </div>

     </form>  
     
    </div><!-- /.col -->
     
</asp:Content>



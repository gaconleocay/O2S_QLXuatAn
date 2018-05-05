<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ReportCustomer2.aspx.cs" Inherits="QLXuatAn_ReportCustomer2" %>
<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_report").addClass("active");
            $(".id_page_report").addClass("open");
            $(".id_page_report9").addClass("open");
            $(".id_page_reportcustomer_detail").addClass("active");



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
                window.location.href = 'ReportCustomer2.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value;

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
	        Danh sách khách hàng
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
                            <%--<label class="col-sm-1 control-label no-padding-right" for="form-field-1"> Từ ngày </label>
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
                        <asp:Repeater id="rpt_Customer" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
                                            <th>STT</th>
				                            <th>Mã KH</th>
                                            <th>Nhóm khách hàng</th>
                                            <th class="hidden-480">Họ tên</th>
                                            <th class="hidden-480">Địa chỉ</th>
                                            <th class="hidden-480">Điện thoại</th>
                                            <th class="hidden-480">Nhóm thẻ</th>
                                            <th class="hidden-480">CardNo</th>
                                            <th class="hidden-480">Mã thẻ</th>
                                            <th class="hidden-480">Biển số</th>
                                            <th class="hidden-480">Ngày hết hạn</th>
                                            <th class="hidden-480">Ngày nhập thẻ</th>
                                            <th class="hidden-480">Trạng thái</th>
                                           
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CustomerCode")%></td>     
                                    <td class="hidden-480">
                                        <%#DataBinder.Eval(Container.DataItem, "CustomerGroupName").ToString()%>
				                    </td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "CustomerName")%></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Address"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Mobile"].ToString() %></td>
                                      <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["CardGroupName"].ToString() %></td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
				                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Plate"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["ExpireDate"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["ImportDate"].ToString() %></td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["IsLock"].ToString() %></td>
				                   
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


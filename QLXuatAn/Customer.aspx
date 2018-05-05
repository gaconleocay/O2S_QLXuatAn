<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Customer.aspx.cs" Inherits="QLXuatAn_Customer" %>
<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('#<%=txtKeyWord.ClientID %>').keypress(function (event) {
                var keycode = event.keyCode || event.which;
                if (keycode == '13') {
                    $('#btnRefresh').click();
                }
            });
            $(".id_page_customer").addClass("active");

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

            $('#btnRefresh').click(function (e) {
                e.preventDefault();
                window.location.href = 'Customer.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value + '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value;
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
                            <div class="col-sm-1 no-padding-left"> 
                                <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='CustomerDetail.aspx'">
                                    <i class="fa fa-plus"></i>
                                    Thêm
                                </button>
                            </div>
                       
                            <div class="col-sm-2 no-padding-left">      
                                <select class="form-control" id="cbCustomerGroup" runat = "server">
                                    
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
                                            <th>Họ tên</th>
                                            <th class="hidden-480">Nhóm khách hàng</th>
                                            <th class="hidden-480">Căn</th>
                                            <th class="hidden-480">Miêu tả</th>
				                            <th class="hidden-480">Trạng thái</th>
                                            <th class="hidden-480">CardNo</th>
                                            <th class="hidden-480">Thẻ</th>
                                            <th class="hidden-480">Biển số</th>
				                            <th style="width: 90px;"></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CustomerCode")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CustomerName")%></td>
                                    <td class="hidden-480">
                                        <%#DataBinder.Eval(Container.DataItem, "CustomerGroupName")%>
				                    </td>
                                    <td class="hidden-480">
                                        <%#DataBinder.Eval(Container.DataItem, "CompartmentName")%>
                                        
				                    </td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "Description")%></td>
				                    <td class="hidden-480">
                                        <%#this.GetStatus(DataBinder.Eval(Container.DataItem, "Inactive").ToString())%>
				                    </td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNo") %></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNumber") %></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "Plate") %></td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "CustomerID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button type="button" class="btn btn-xs btn-info detail" onclick="window.location.href='CustomerDetail.aspx?CustomerID=<%#DataBinder.Eval(Container.DataItem, "CustomerID")%>'">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
						                        </button>
						                        <button type="button" class="btn btn-xs btn-danger delete">
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
												        <a href="CustomerDetail.aspx?CustomerID=<%#DataBinder.Eval(Container.DataItem, "CustomerID")%>" class="tooltip-info" data-rel="tooltip" title="Sửa">
													        <span class="blue">
														        <i class="ace-icon fa fa-pencil bigger-120"></i>
													        </span>
												        </a>
											        </li>
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



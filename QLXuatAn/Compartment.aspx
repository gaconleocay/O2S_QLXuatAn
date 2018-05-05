<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Compartment.aspx.cs" Inherits="QLXuatAn_Compartment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_list").addClass("active");
            $(".id_page_list").addClass("open");
            $(".id_page_compartment").addClass("active");

            $('.tree1').treegrid();

            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("Compartment.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
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
                window.location.href = 'Compartment.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value;
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
        <li>
			<a href="#">Danh mục</a>
		</li>
		<li class="active">Căn hộ</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1>
	        Danh sách Căn hộ
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
                    <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='CompartmentDetail.aspx'">
                        <i class="fa fa-plus"></i>
                        Thêm mới
                    </button>
                </div> 
                    <div class="col-sm-2">
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
                        <asp:Repeater id="rpt_Compartment" runat="server">
                            <HeaderTemplate>
	                            <table class="tree1 table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
				                            <th style="width:10%">Stt</th>
                                            <th style="width:75%">Tên căn hộ</th>
				                            <th style="width:15%"></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CompartmentName")%></td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "CompartmentID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
                                                <a href="CompartmentDetail.aspx?CompartmentID=<%#DataBinder.Eval(Container.DataItem, "CompartmentID")%>" class="btn btn-xs btn-info detail" title="Sửa">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
                                                </a>
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
												        <a href="CompartmentDetail.aspx?CompartmentID=<%#DataBinder.Eval(Container.DataItem, "CompartmentID")%>" class="tooltip-info" data-rel="tooltip" title="Sửa">
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
    
                    <table>
                        <tr>
                            <td colspan="3">
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



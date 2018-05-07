<%@ Page Title="" Language="C#" MasterPageFile="~/accesscontrol/MasterPage.master" AutoEventWireup="true" CodeFile="Card.aspx.cs" Inherits="accesscontrol_Card" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_card").addClass("active");

            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("Card.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
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
                window.location.href = 'Card.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerID=' + document.getElementById("<%= cbCustomer.ClientID %>").value;
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
		<li class="active">Thẻ</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_cardlist" runat="server">
	        Danh sách thẻ
        </h1>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_PageContent" Runat="Server">

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
        <div class="row">
            <div class="widget-body">
				<div class="widget-main">
					<form class="form-inline">
                        <div class="form-group">
                            <div class="col-sm-2 no-padding-left"> 
                                <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='CardDetail.aspx'">
                                    <i class="fa fa-plus"></i>
                                    Thêm mới
                                </button>
                            </div>
                        </div>
                        <div class="form-group">
				            <div class="col-sm-3 no-padding-left"">
                                <input type="text" id="txtKeyWord" placeholder="Từ khóa tìm kiếm" class="form-control" runat="server" />
				            </div>
			            </div>
                        <div class="form-group">
                            <div class="col-sm-3 no-padding-left">      
                                <select class="form-control" id="cbCardGroup" runat = "server">
                                    
                                </select> 
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-3 no-padding-left">      
                                <select class="form-control" id="cbCustomer" runat = "server">
                                    
                                </select> 
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-3 no-padding-left">    
                                <button type="button" class="btn btn-white btn-default" id="btnRefresh">
                                    <i class="fa fa-refresh"></i>
                                    Nạp lại
                                </button>
                            </div>
                        </div>
                    </form>
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
				                            <th>Số thẻ</th>
                                            <th>Mã thẻ</th>
                                            <th class="hidden-480">Nhóm thẻ</th>
                                            <th class="hidden-480">Ngày hết hạn</th>
                                            <%--<th class="hidden-480">Miêu tả</th>--%>
				                            <th class="hidden-480">Trạng thái</th>
				                            <th></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <%--<td><%#(Container.ItemIndex+1).ToString()%></td>--%>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                    <td class="hidden-480">
                                        <%#this.GetCardGroup(DataBinder.Eval(Container.DataItem, "CardGroupID").ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#this.GetExpireDate(DataBinder.Eval(Container.DataItem, "ExpireDate").ToString())%></td>
                                    <%--<td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "Description")%></td>--%>
				                    <td class="hidden-480">
                                        <%#this.GetCardStatus(DataBinder.Eval(Container.DataItem, "IsLock").ToString())%>
				                    </td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "CardID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-info detail" title="Sửa" onclick="window.location.href='CardDetail.aspx?CardID=<%#DataBinder.Eval(Container.DataItem, "CardID")%>'">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
						                        </button>
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
												        <a class="tooltip-info detail" data-rel="tooltip" title="Sửa" href="CardDetail.aspx?CardID=<%#DataBinder.Eval(Container.DataItem, "CardID")%>">
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
    
                <form id="frm1" class="form-horizontal" runat="server">
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
                </form>

            </div>
        </div>

    </div><!-- /.col -->
</asp:Content>



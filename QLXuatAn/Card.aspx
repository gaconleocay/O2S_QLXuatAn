<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Card.aspx.cs" Inherits="QLXuatAn_Card" %>
<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <%--<script type="text/javascript" src="../assets/js/jquery.js"></script>--%>
    <script src="../Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('#<%=txtKeyWord.ClientID %>').keypress(function (event) {
                var keycode = event.keyCode || event.which;
                if (keycode == '13') {
                    $('#btnRefresh').click();
                }
            });
            $("#nav-customer-input").focus(function () {
                this.select();
            });
            $("#nav-customer-input").change(function () {
                if ($(this).val() == '') {
                    $('.plcCustomerId').find('input[type=hidden]').val('');
                    return false;
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
                    $('.plcCustomerId').find('input[type=hidden]').val(ui.item.Id);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });


            $(".id_page_active_card_mgr").addClass("active open");
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
                    '&CustomerID=' + $('.plcCustomerId').find('input[type=hidden]').val() +
                        '&cusName=' + $('#nav-customer-input').val() +
                    '&CardState=' + document.getElementById("<%= cbCardState.ClientID %>").value +
                    '&CustomerGroupID=' + document.getElementById("<%=cbCustomerGroup.ClientID %>").value;
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
                
                    window.location.href = 'Card.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                        '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerID=' + $('.plcCustomerId').find('input[type=hidden]').val() +
                        '&cusName=' + $('#nav-customer-input').val() +
                        '&CaNoSort=' + _val +
                    '&CardState=' + document.getElementById("<%= cbCardState.ClientID %>").value +
                    '&CustomerGroupID=' + document.getElementById("<%=cbCustomerGroup.ClientID %>").value;
            });
        });
    </script>
    <style>
        .cSort{
            color:#337ab7!important;
        }
        .ulListSubCard{
            margin-left: 0;
            list-style:none;
            width:100px;
        }
    </style>
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
        <form id="frm1" class="form-horizontal" runat="server">
        <div class="col-xs-12">
        <div class="row">
            <div class="widget-body">
				<div class="widget-main">
					<%--<form class="form-inline">--%>
                        <div class="form-group">
                            <div class="col-sm-1 no-padding-left"> 
                                <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='CardDetail_2.aspx'">
                                    <i class="fa fa-plus"></i>
                                    Thêm
                                </button>
                            </div>
                            
                            <div class="col-sm-2 no-padding-left">      
                                <select class="form-control" id="cbCardGroup" runat = "server">
                                    
                                </select> 
                            </div>
                            <div class="col-sm-2 no-padding-left">      
                                <select class="form-control" id="cbCustomerGroup" runat = "server">
                                    
                                </select> 
                            </div>
                            <div class="col-sm-2 no-padding-left">      
                               <%-- <select class="form-control" id="cbCustomer" runat = "server">
                                    
                                </select> --%>
                                <div class="plcCustomerId">
                                    <input type="text" placeholder="Nhập tên hoặc mã KH" class="nav-search-input" id="nav-customer-input" value="<%=ViewState["cusName"] %>"/>
                                    <input type="hidden" name="CustomerID" runat="server" value="" />
                                </div>
                            </div>
                           
                             <div class="col-sm-1 no-padding-left">      
                                <select class="form-control" id="cbCardState" runat = "server">
                                    <option value="">Tất cả thẻ</option>
                                    <option value="0">Thẻ hoạt động</option>
                                    <option value="1">Thẻ khóa</option>
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
                        </div>
                       
                    <div class="form-group">
                        <div class="col-sm-1 no-padding-left">    
                            <asp:LinkButton ID="LinkButton3" OnClick="Excel_Click" runat="server" class="btn btn-white btn-default export">
                                <i class="fa fa-file-excel-o green"></i>
                                <span class="">Export</span>
                            </asp:LinkButton>
                        </div>
                        <div class="col-sm-1 no-padding-left">    
                            <a class="btn btn-white btn-default export" data-toggle="modal" data-target="#ShowBoxUploadExcelCard">
                                <i class="fa fa-file-excel-o pink"></i>
                                <span class="">Import</span>
                            </a>
                            <!-- Modal -->
                            <div class="modal fade" id="ShowBoxUploadExcelCard" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                                <div class="modal-dialog" role="document">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="exampleModalLabel">Chọn file Upload</h5>
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span>
                                            </button>
                                        </div>
                                        <div class="modal-body">
                                            <div style="padding: 10px;">
                                                <asp:FileUpload ID="FileUpload1" runat="server"/>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <asp:LinkButton ID="LinkButton4" OnClick="LinkButton2_Click" Text="Cập nhập" runat="server" class="btn btn-info"></asp:LinkButton>
                                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Thoát</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                           
                        </div>
                    </div>
                   <%-- </form>--%>
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
				                            <th style="width:75px;">
                                                <a href="javascript:void(0)" style="color:#707070;" name="btnCaNoSort">
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
                                            <th>Mã thẻ</th>
                                            <th>Thẻ phụ</th>
                                            <th class="hidden-480">Nhóm thẻ</th>
                                            <th class="hidden-480">Biển số</th>
                                            <th class="hidden-480">Ngày hết hạn</th>
                                            <%--<th class="hidden-480">Miêu tả</th>--%>
				                            <th class="hidden-480">Trạng thái</th>
                                            <th class="hidden-480">Khách hàng</th>
                                            <th class="hidden-480">Địa chỉ</th>
                                            <th class="hidden-480">Nhóm KH</th>
                                            <th class="hidden-480">Ngày nhập thẻ</th>
				                            <th style="width:75px;"></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                    <td>
                                        <%#Server.HtmlDecode(this.GetListCardSub(DataBinder.Eval(Container.DataItem, "CardNumber").ToString()))%>
                                    </td>
                                    <td class="hidden-480">
                                        <%#DataBinder.Eval(Container.DataItem, "CardGroupName")%>
                                        <%#StaticPool.GetCardGroup(DataBinder.Eval(Container.DataItem, "CardGroupID").ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "Plate1")%></td>
                                    <td class="hidden-480"><%#StaticPool.GetExpireDate(DataBinder.Eval(Container.DataItem, "ExpireDate").ToString())%></td>
                                    <%--<td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "Description")%></td>--%>
				                    <td class="hidden-480">
                                        <%#StaticPool.GetCardStatus(DataBinder.Eval(Container.DataItem, "IsLock").ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["CustomerName"].ToString()%></td>
                                     <td class="hidden-480"><%#((System.Data.DataRowView)Container.DataItem)["Address"].ToString() %></td>
                                    <td class="hidden-480">
                                        <%#this.GetCustomerGroup(DataBinder.Eval(Container.DataItem, "CustomerID").ToString())%>
                                    </td>
                                    <td class="hidden-480"><%#this.GetImportDate(DataBinder.Eval(Container.DataItem, "ImportDate").ToString())%></td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "CardID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button type="button" class="btn btn-xs btn-info detail" title="Sửa" onclick="window.location.href='CardDetail_2.aspx?CardID=<%#DataBinder.Eval(Container.DataItem, "CardID")%>'">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
						                        </button>
						                        <button type="button" class="btn btn-xs btn-danger delete" title="Xóa" >
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
												        <a class="tooltip-info detail" data-rel="tooltip" title="Sửa" href="CardDetail_2.aspx?CardID=<%#DataBinder.Eval(Container.DataItem, "CardID")%>">
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



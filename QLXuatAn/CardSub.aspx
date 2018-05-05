<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="CardSub.aspx.cs" Inherits="QLXuatAn_CardSub" %>
<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script>
        $(function () {

            $(".id_page_active_card_mgr").addClass("active open");
            $(".id_page_Subcard").addClass("active");


            $("body").on('click', 'a[name=btnDeleteSub]', function () {
                var cmd = $(this);
                if (confirm('Bạn có chắc chắn muốn xóa thẻ phụ này?')) {
                    var _v = cmd.attr('idata');
                    if (_v != undefined && _v != '') {
                        $.ajax({
                            method: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: 'CardSub.aspx/DeleteSubCard',
                            dataType: "json",
                            data: "{ 'subCardId': '" + _v + "'}",
                            success: function (result) {
                                if (result.d == "1") {
                                    cmd.parent().parent().fadeOut();
                                } else {
                                    alert('Xóa không thành công');
                                }
                            },
                            error: function (result) {
                                //debugger;
                                alert(result);
                            }
                        });
                    }
                }
            });


            $('#btnRefresh').click(function (e) {
                e.preventDefault();
                window.location.href = 'CardSub.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value;
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
		<li class="active">Thẻ phụ</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_cardlist" runat="server">
	        Danh sách thẻ phụ
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
    <form id="frm1" class="form-horizontal" runat="server">
    <div class="col-xs-12">
            <div class="widget-body">
				<div class="widget-main">
					<%--<form class="form-inline">--%>
                        <div class="form-group">
                            <div class="col-sm-1 no-padding-left"> 
                                <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='CardSubDetail.aspx'">
                                    <i class="fa fa-plus"></i>
                                    Thêm
                                </button>
                            </div>
                             <div class="col-sm-4 no-padding-left"">
                                <input type="text" id="txtKeyWord" placeholder="Từ khóa tìm kiếm" class="form-control" runat="server" />
				            </div>
                             <div class="col-sm-1 no-padding-left">    
                                <button type="button" class="btn btn-white btn-default" id="btnRefresh">
                                    <i class="fa fa-refresh"></i>
                                    Nạp lại
                                </button>
                            </div>
                            <div class="col-sm-1 no-padding-left">    
                                <button type="button" class="btn btn-warning btn-sm" onclick="window.location.href='CardSub.aspx'">
                                    <i class="fa fa-retweet"></i>
                                    Reset
                                </button>
                            </div>

                            <div class="col-sm-1 no-padding-left">    
                            <a class="btn btn-white btn-default export" data-toggle="modal" data-target="#ShowBoxUploadExcelSubCard">
                                <i class="fa fa-file-excel-o pink"></i>
                                <span class="">Import</span>
                            </a>
                            <!-- Modal -->
                            <div class="modal fade" id="ShowBoxUploadExcelSubCard" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                                <div class="modal-dialog" role="document">
                                    <div class="modal-content">
                                        <div class="modal-header">
                                            <h5 class="modal-title" id="exampleModalLabel">Chọn file Upload</h5>
                                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                <span aria-hidden="true">&times;</span>
                                            </button>
                                        </div>
                                        <div class="modal-body">
                                            <a href="/QLXuatAn/files/addCardSub.xlsx" style="padding-left:10px;">Tải xuống file mẫu <i class="fa fa-download" aria-hidden="true"></i></a>
                                            <div style="padding: 10px;">
                                                <asp:FileUpload ID="FileUpload1" runat="server"/>
                                            </div>
                                        </div>
                                        <div class="modal-footer">
                                            <asp:LinkButton ID="LinkButton4" OnClick="ImportCardSub_Click" Text="Cập nhập" runat="server" class="btn btn-info btn-sm"></asp:LinkButton>
                                            <button type="button" class="btn btn-secondary btn-sm" data-dismiss="modal">Thoát</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                           
                        </div>
                             <div class="col-sm-1 no-padding-left">    
                            <asp:LinkButton ID="LinkButton3" OnClick="Excel_Click" runat="server" class="btn btn-white  btn-default export">
                                <i class="fa fa-file-excel-o green"></i>
                                <span class="">Export</span>
                            </asp:LinkButton>
                        </div>
                        </div>
				</div>
			</div>
        </div>
        <div class="col-xs-12">
                <div class="widget-box">
			        <div class="widget-main no-padding">
                        <asp:Repeater id="rpt_Card" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
                                            <th style="width:30px;">STT</th>
                                            <th>Số thẻ chính</th>
                                            <th>Thẻ chính</th>
                                            <th>CardNo(thẻ phụ)</th>
                                            <th>Mã thẻ phụ</th>
                                            <th class="col-xs-1">Thao tác</th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td align="center"><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#this.GetCardNo(DataBinder.Eval(Container.DataItem, "MainCard").ToString())%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "MainCard")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                    <td>
                                        <button type="button" class="btn btn-xs btn-info detail" title="Sửa" onclick="window.location.href='CardSubDetail.aspx?SubCardId=<%#DataBinder.Eval(Container.DataItem, "Id")%>'">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
						                        </button>
                                        <a name="btnDeleteSub" idata="<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn btn-xs btn-danger delete" title="Xóa">
                                                        <i class="ace-icon fa fa-trash-o bigger-120"></i>
                                                    </a>
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
        </form>
</asp:Content>


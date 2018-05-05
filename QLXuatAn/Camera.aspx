<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Camera.aspx.cs" Inherits="QLXuatAn_Camera" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_device").addClass("active");
            $(".id_page_device").addClass("open");
            $(".id_page_camera").addClass("active");

            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("Camera.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
                        if (result.d.toString() == "true") {
                            element.remove();
                        }
                        else
                            alert(result.d.toString());
                    });
                }

            });

            // Them camera
            $('#btnAddCamera').click(function (e) {
                e.preventDefault();
                window.location.href = 'CameraDetail.aspx?PCID=' + document.getElementById("<%= cbPC.ClientID %>").value;
            });

            // Thuc hien bao cao
            $('#btnRefresh').click(function (e) {
                e.preventDefault();
                window.location.href = 'Camera.aspx?PCID=' + document.getElementById("<%= cbPC.ClientID %>").value;
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
			<a href="#">Cài đặt thiết bị</a>
		</li>
		<li class="active">Camera</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1>
	        Danh sách Camera
        </h1>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_PageContent" Runat="Server">

    <%--Hien thi thong bao tai day--%>
    <div class="alert alert-warning" id="div_alert" runat="server">
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
                                <button type="button" id="btnAddCamera" class="btn btn-info btn-sm">
                                    <i class="fa fa-plus"></i>
                                    Thêm mới
                                </button>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-3 no-padding-left">      
                                <select class="form-control" id="cbPC" runat = "server">
                                    
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
                        <asp:Repeater id="rpt_Camera" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
				                            <th>Stt</th>
				                            <th>Tên Camera</th>
                                            <th class="hidden-480">Địa chỉ</th>
                                            <th class="hidden-480">Máy tính</th>
				                            <th class="hidden-480">Trạng thái</th>
				                            <th></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CameraName")%></td>
                                    <td class="hidden-480">
                                        <a href="http://<%#DataBinder.Eval(Container.DataItem, "HttpURL") + ":" + DataBinder.Eval(Container.DataItem, "HttpPort")%>" target="_blank" class="tooltip-info" data-rel="tooltip" title="Đi đến trang Camera">
                                            <%#DataBinder.Eval(Container.DataItem, "HttpURL") + ":" + DataBinder.Eval(Container.DataItem, "HttpPort")%>
                                        </a>
                                    </td>
                                    <td class="hidden-480">
                                        <%#this.GetComputerName(DataBinder.Eval(Container.DataItem, "PCID").ToString())%>
				                    </td>
				                    <td class="hidden-480">
                                        <%#this.GetStatus(DataBinder.Eval(Container.DataItem, "Inactive").ToString())%>
				                    </td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "CameraID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-info detail" onclick="window.location.href='CameraDetail.aspx?CameraID=<%#DataBinder.Eval(Container.DataItem, "CameraID")%>'">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
						                        </button>
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
												        <a href="CameraDetail.aspx?CameraID=<%#DataBinder.Eval(Container.DataItem, "CameraID")%>" class="tooltip-info" data-rel="tooltip" title="Sửa">
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
            </div>
        </div>

    </div><!-- /.col -->
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Lane.aspx.cs" Inherits="QLXuatAn_Lane" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_device").addClass("active");
            $(".id_page_device").addClass("open");
            $(".id_page_lane").addClass("active");

            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("Lane.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
                        if (result.d.toString() == "true") {
                            element.remove();
                        }
                        else
                            alert(result.d.toString());
                    });
                }

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
		<li class="active">làn vào/ra</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1>
	        Danh sách làn vào/ra
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
                    <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='LaneDetail.aspx'">
                        <i class="fa fa-plus"></i>
                        Thêm mới
                    </button>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-12">
                <div class="widget-box">
			        <div class="widget-main no-padding">
                        <asp:Repeater id="rpt_Lane" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
				                            <th>Stt</th>
				                            <th>Tên</th>
                                            <th class="hidden-480">Loại làn vào/ra</th>
                                            <th class="hidden-480">Máy tính</th>
				                            <th class="hidden-480">Trạng thái</th>
				                            <th></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "LaneName")%></td>
                                    <td class="hidden-480">
                                        <%#this.GetLaneTypeName(DataBinder.Eval(Container.DataItem, "LaneType").ToString())%>
				                    </td>
                                    <td class="hidden-480">
                                        <%#this.GetComputerName(DataBinder.Eval(Container.DataItem, "PCID").ToString())%>
				                    </td>
				                    <td class="hidden-480">
                                        <%#this.GetStatus(DataBinder.Eval(Container.DataItem, "Inactive").ToString())%>
				                    </td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "LaneID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-info detail" onclick="window.location.href='LaneDetail.aspx?LaneID=<%#DataBinder.Eval(Container.DataItem, "LaneID")%>'">
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
												        <a href="LaneDetail.aspx?LaneID=<%#DataBinder.Eval(Container.DataItem, "LaneID")%>" class="tooltip-info" data-rel="tooltip" title="Sửa">
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





<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="VehicleGroup.aspx.cs" Inherits="QLXuatAn_VehicleGroup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_list").addClass("active");
            $(".id_page_list").addClass("open");
            $(".id_page_vehiclegroup").addClass("active");

            $('.delete').click(function (e) {
                e.preventDefault();

                var id = $(this).closest('td').attr("id");
                var element = $(this).closest('tr');

                if (confirm("Bạn có muốn xóa bản ghi này không?")) {

                    AjaxPOST("VehicleGroup.aspx/Delete", '{"id":"' + id + '", "userid":"' + '<%=this.ViewState["UserID"].ToString()%>' + '"}').success(function (result) {
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
			<a href="#">Danh mục</a>
		</li>
		<li class="active">Nhóm xe</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1>
	        Danh sách nhóm xe
        </h1>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_PageContent" Runat="Server">

    <div class="col-xs-12">
	    <div class="row">
            <div class="widget-body">
				<div class="widget-main">
                    <button type="button" class="btn btn-info btn-sm" onclick="window.location.href='VehicleGroupDetail.aspx'">
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
                        <asp:Repeater id="rpt_VehicleGroup" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
				                            <th>Stt</th>
                                            <th>Tên nhóm xe</th>
                                            <th class="hidden-480">Loại xe</th>
                                            <th class="hidden-480">Số lượng giới hạn</th>
				                            <th class="hidden-480">Trạng thái</th>
				                            <th></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <td><%#(Container.ItemIndex+1).ToString()%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "VehicleGroupName")%></td>
                                    <td class="hidden-480">
                                        <%#this.GetVehicleTypeName(DataBinder.Eval(Container.DataItem, "VehicleType").ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "LimitNumber")%></td>
				                    <td class="hidden-480">
                                        <%#this.GetStatus(DataBinder.Eval(Container.DataItem, "Inactive").ToString())%>
				                    </td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "VehicleGroupID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-info detail" onclick="window.location.href='VehicleGroupDetail.aspx?VehicleGroupID=<%#DataBinder.Eval(Container.DataItem, "VehicleGroupID")%>'">
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
												        <a href="VehicleGroupDetail.aspx?VehicleGroupID=<%#DataBinder.Eval(Container.DataItem, "VehicleGroupID")%>" class="tooltip-info" data-rel="tooltip" title="Sửa">
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


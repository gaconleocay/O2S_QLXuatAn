<%@ Page Title="" Language="C#" MasterPageFile="~/O2SPopup/MasterPage.master" AutoEventWireup="true" CodeFile="RoleDetail.aspx.cs" Inherits="O2SPopup_RoleDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $(".id_page_system").addClass("active");
            $(".id_page_system").addClass("open");
            $(".id_page_role").addClass("active");

            $('.tree1').treegrid();

        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
        <li>
			<a href="#">Hệ thống</a>
		</li>
		<li class="active">Vai trò & quyền hạn</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_roledetail" runat="server">
	        Thêm vai trò & quyền hạn
        </h1>
    </div>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
    <div class="col-xs-12">
        <%--Hien thi thong bao tai day--%>
        <div class="alert alert-warning" id="div_alert" runat="server">
			<button type="button" class="close" data-dismiss="alert">
				<i class="ace-icon fa fa-times"></i>
			</button>
            <i class="ace-icon fa fa-exclamation-triangle"></i>
			<span id="id_alert" runat = "server"></span>
			<br />
		</div>
    
    </div>

    <div class="col-xs-12">
        <form id="frm_UserDetail" class="form-horizontal" runat="server">
            <!-- #section:elements.tab -->
		    <div class="tabbable">
			    <ul class="nav nav-tabs" id="myTab">
				    <li class="active">
					    <a data-toggle="tab" href="#id_role_info">
						    <i class="green ace-icon fa fa-credit-card bigger-120"></i>
						    Thông tin chung
					    </a>
				    </li>

				    <li>
					    <a data-toggle="tab" href="#id_subsystem">
                            <i class="green ace-icon fa fa-user bigger-120"></i>
						    Phân quyền
						    <%--<span class="badge badge-info">2</span>--%>
					    </a>
				    </li>

			    </ul>

                <div class="tab-content">
				    <div id="id_role_info" class="tab-pane fade in active">	
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Vai trò </label>
				            <div class="col-sm-5">
					            <asp:TextBox id="txtRoleName" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Miêu tả </label>
				            <div class="col-sm-9">
					            <asp:TextBox id="txtDescription" runat="server" class="form-control" placeholder="" Text="Bãi xe thông minh"/>
				            </div>
			            </div>
                        <div class="form-group">
                            <div class="col-sm-offset-3 col-sm-5">  
				                <label class="inline">
				                    <input type="checkbox" class="ace" id="chbInactive" runat="server"/>
				                    <span class="lbl"> Ngừng kích hoạt</span>
			                    </label>
                            </div>
			            </div>
                    </div>

                    <div id="id_subsystem" class="tab-pane fade">
                        <div class="form-group">
				            <%--<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Phân quyền </label>--%>
				            <div class="col-sm-12">
					            <div class="widget-box">
			                        <div class="widget-main no-padding">
                                        <asp:Repeater id="rpt_SubSystem" runat="server" OnItemDataBound="rpt_ItemDataBound">
                                            <HeaderTemplate>
	                                            <table class="tree1 table table-striped table-bordered table-hover">
		                                            <thead>
			                                            <tr>
				                                            <%--<th>Stt</th>--%>
                                                            <th>Vai trò</th>
                                                            <th class="hidden-480">Mã chức năng</th>
                                                            <th>Sử dụng</th>
                                                            <th>Thêm</th>
                                                            <th>Sửa</th>
                                                            <th>Xóa</th>
                                                            <th>Xuất khẩu</th>
			                                            </tr>
		                                            </thead>
                                            </HeaderTemplate>

                                            <ItemTemplate>
			                                    <tr class="treegrid-<%#DataBinder.Eval(Container.DataItem, "SubSystemID")%> treegrid-parent-<%#DataBinder.Eval(Container.DataItem, "ParentID")%>">
				                                    <%--<td><%#(Container.ItemIndex+1).ToString()%></td>--%>
                                                    <td><%#DataBinder.Eval(Container.DataItem, "SubSystemName")%></td>
                                                    <td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "SubSystemCode")%></td>
                                                    <td>
                                                        <asp:HiddenField ID="SubSystemID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SubSystemID")%>' />
                                                        <label class="inline">
				                                            <input type="checkbox" class="ace" id="chbSelects" runat="server"/>
				                                            <span class="lbl"></span>
			                                            </label>
				                                    </td>
                                                    <td>
                                                        <label class="inline">
				                                            <input type="checkbox" class="ace" id="chbInserts" runat="server"/>
				                                            <span class="lbl"></span>
			                                            </label>
				                                    </td>
                                                    <td>
                                                        <label class="inline">
				                                            <input type="checkbox" class="ace" id="chbUpdates" runat="server"/>
				                                            <span class="lbl"></span>
			                                            </label>
				                                    </td>
                                                    <td>
                                                        <label class="inline">
				                                            <input type="checkbox" class="ace" id="chbDeletes" runat="server"/>
				                                            <span class="lbl"></span>
			                                            </label>
				                                    </td>
                                                    <td>
                                                        <label class="inline">
				                                            <input type="checkbox" class="ace" id="chbExports" runat="server"/>
				                                            <span class="lbl"></span>
			                                            </label>
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
                    </div>
			    </div>
		    </div>
            
            <div class="clearfix form-actions">
				<div class="col-md-offset-3 col-md-9">
                    <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-info">
                        <i class="ace-icon fa fa-check bigger-110"></i>
						<span class="bigger-110">Lưu lại</span>
                    </asp:LinkButton>

                    &nbsp; &nbsp; &nbsp;
					<button class="btn" type="button" onclick="window.location.href='Role.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>

</asp:Content>



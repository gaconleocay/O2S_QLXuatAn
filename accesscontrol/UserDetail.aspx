<%@ Page Title="" Language="C#" MasterPageFile="~/accesscontrol/MasterPage.master" AutoEventWireup="true" CodeFile="UserDetail.aspx.cs" Inherits="accesscontrol_UserDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_system").addClass("active");
            $(".id_page_system").addClass("open");
            $(".id_page_user").addClass("active");

            var demo1 = $('#<%=cbRoles.ClientID%>').bootstrapDualListbox({ infoTextFiltered: '<span class="label label-purple label-lg">Filtered</span>' });
            var container1 = demo1.bootstrapDualListbox('getContainer');
            container1.find('.btn').addClass('btn-white btn-info btn-bold');

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
		<li class="active">Người dùng</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_userdetail" runat="server">
	        Thêm người dùng
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
            <i class="ace-icon fa fa-exclamation-triangle bigger-200"></i>
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
					    <a data-toggle="tab" href="#id_user_info">
						    <i class="green ace-icon fa fa-user bigger-120"></i>
						    Thông tin chung
					    </a>
				    </li>

				    <li>
					    <a data-toggle="tab" href="#id_role_info">
                            <i class="green ace-icon fa fa-cog bigger-120"></i>
						    Phân quyền
						    <%--<span class="badge badge-info">2</span>--%>
					    </a>
				    </li>

			    </ul>

                <div class="tab-content">
                    <div id="id_user_info" class="tab-pane fade in active">	
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Mã người dùng </label>
				            <div class="col-sm-3">
					            <select class="form-control" id="cbUserCode" runat = "server">

                                </select> 
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Họ tên </label>
				            <div class="col-sm-5">
					            <asp:TextBox id="txtFullName" runat="server" class="form-control" placeholder="Họ tên"/>
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên đăng nhập </label>
				            <div class="col-sm-5">
					            <asp:TextBox id="txtUserName" runat="server" class="form-control" placeholder="Tên đăng nhập"/> 
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Mật khẩu </label>
				            <div class="col-sm-5">                   
					            <asp:TextBox id="txtPassword" TextMode="password" runat="server" class="form-control" placeholder="Mật khẩu"/>                   
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Nhập lại mật khẩu </label>
				            <div class="col-sm-5">                   
					            <asp:TextBox id="txtRePassword" TextMode="password" runat="server" class="form-control" placeholder="Nhập lại mật khẩu"/>                   
				            </div>
			            </div>
                        <div class="form-group">
                            <div class="col-sm-offset-3 col-sm-5">  
				                <label class="inline">
				                    <input type="checkbox" class="ace" id="chbIsLockUser" runat="server"/>
				                    <span class="lbl"> Khóa người dùng này</span>
			                    </label>
                            </div>
			            </div>
                    </div>

				    <div id="id_role_info" class="tab-pane fade">
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-top" for="cbLane"> Phân quyền </label>
				            <div class="col-sm-8">
					            <!-- #section:plugins/input.duallist -->
					            <select multiple="" style="height:300px;" class="form-control" name="duallistbox_demo1[]" id="cbRoles" runat="server" >
						
					            </select>

					            <!-- /section:plugins/input.duallist -->
					            <%--<div class="hr hr-16 hr-dotted"></div>--%>
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
					<button class="btn" type="button" onclick="window.location.href='User.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>

        </form>
    </div>

</asp:Content>


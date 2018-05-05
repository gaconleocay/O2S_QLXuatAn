<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Profile.aspx.cs" Inherits="QLXuatAn_Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Hồ sơ cá nhân</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_profile" runat="server">
	        Hồ sơ cá nhân
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

        <div class="alert alert-info" id="div_info" runat="server">
			<button type="button" class="close" data-dismiss="alert">
				<i class="ace-icon fa fa-times"></i>
			</button>
            <i class="ace-icon fa fa-check"></i>
			<span id="id_info" runat = "server"></span>
			<br />
		</div>

        <form id="frm_UserDetail" class="form-horizontal" runat="server">
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Mã người dùng </label>
				<div class="col-sm-3">
					<asp:TextBox id="txtCode" runat="server" class="form-control" placeholder="Mã người dùng" ReadOnly/> 
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
					<asp:TextBox id="txtUserName" runat="server" class="form-control" placeholder="Tên đăng nhập" ReadOnly/> 
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

            <div class="clearfix form-actions">
				<div class="col-md-offset-3 col-md-9">
                    <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-info">
                        <i class="ace-icon fa fa-check bigger-110"></i>
						<span class="bigger-110">Lưu lại</span>
                    </asp:LinkButton>

				</div>
			</div>
        </form>

    </div>
</asp:Content>

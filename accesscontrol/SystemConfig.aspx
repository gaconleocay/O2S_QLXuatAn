<%@ Page Title="" Language="C#" MasterPageFile="~/accesscontrol/MasterPage.master" AutoEventWireup="true" CodeFile="SystemConfig.aspx.cs" Inherits="accesscontrol_SystemConfig" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_system").addClass("active");
            $(".id_page_system").addClass("open");
            $(".id_page_systemconfig").addClass("active");
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
		<li class="active">Tham số hệ thống</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <%--<div class="page-header">
        <h1>
	        Thiết lập tham số hệ thống
        </h1>
    </div>--%>
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
            <h3 class="header smaller lighter blue">
				Thông tin đơn vị sử dụng
			</h3>

            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên đơn vị sử dụng </label>
				<div class="col-sm-9">
					<asp:TextBox id="txtCompany" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Số điện thoại </label>
				<div class="col-sm-3">
					<asp:TextBox id="txtTel" runat="server" class="form-control" placeholder=""/> 
				</div>
                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Số fax </label>
				<div class="col-sm-3">
					<asp:TextBox id="txtFax" runat="server" class="form-control" placeholder=""/> 
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Địa chỉ </label>
				<div class="col-sm-9">
					<asp:TextBox id="txtAddress" runat="server" class="form-control" placeholder=""/> 
				</div>
			</div>

            <div id="div_smartcard" runat="server">
                <h3 class="header smaller lighter blue">
				    Tham số đầu đọc
			    </h3>

                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Mã hệ thống </label>
				    <div class="col-sm-3">                   
					    <asp:TextBox id="txtSystemCode" TextMode="password" runat="server" class="form-control" placeholder=""/>                   
				    </div>
			    </div>
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Key A </label>
				    <div class="col-sm-3">                   
					    <asp:TextBox id="txtKeyA" TextMode="password" runat="server" class="form-control" placeholder=""/>                   
				    </div>
                    <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Key B </label>
				    <div class="col-sm-3">                   
					    <asp:TextBox id="txtKeyB" TextMode="password" runat="server" class="form-control" placeholder=""/>                   
				    </div>
			    </div>
            </div>
            <%--<h3 class="header smaller lighter blue">
				Tham số khác
			</h3>

            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chbIsPointLevel" runat="server"/>
				        <span class="lbl"> Tính điểm thưởng cho thẻ thành viên</span>
			        </label>
                </div>
			</div>--%>

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


<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="LEDDetail.aspx.cs" Inherits="QLXuatAn_LEDDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $(".id_page_device").addClass("active");
            $(".id_page_device").addClass("open");
            $(".id_page_led").addClass("active");

            $('#<%=txtBaudrate.ClientID%>').keydown(function (e) {
                AllowUint(e); // Chi cho phep nhap so
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
       <%-- <li>
			<a href="#">Cài đặt thiết bị</a>
		</li>--%>
		<li class="active">LED</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
<div class="page-header">
        <h1 id="id_detail" runat="server">
	        Thêm
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
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên  </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtName" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Máy tính</label>
				<div class="col-sm-5">                   
					<select class="form-control" id="cbPC" runat = "server">

                    </select>               
				</div>
                
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> COM(IP)  </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtComport" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Baudrate(Port) </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtBaudrate" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Địa chỉ(232/485/422) </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtAddress" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Giao diện</label>
				<div class="col-sm-5">                   
					<select class="form-control" id="cbSide" runat = "server">
                        <option value="0">Trái</option>
                        <option value="1">Phải</option>
                        <option value="2">Phải+Trái</option>
                    </select>               
				</div>         
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Thiết bị hiển thị</label>
				<div class="col-sm-5">                   
					<select class="form-control" id="cbLedType" runat = "server">
                        

                    </select>               
				</div>         
			</div>
             <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chEnable" runat="server"/>
				        <span class="lbl"> Ngừng kích hoạt</span>
			        </label>
                </div>
			</div>

            
            
            <div class="clearfix form-actions">
				<div class="col-md-offset-3 col-md-9">
                    <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-info">
                        <i class="ace-icon fa fa-check bigger-110"></i>
						<span class="bigger-110">Lưu lại</span>
                    </asp:LinkButton>

                    &nbsp; &nbsp; &nbsp;
					<button class="btn" type="button" onclick="window.location.href='LED.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>
</asp:Content>


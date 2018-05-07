<%@ Page Title="" Language="C#" MasterPageFile="~/accesscontrol/MasterPage.master" AutoEventWireup="true" CodeFile="ControllerDetail.aspx.cs" Inherits="accesscontrol_ControllerDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $(".id_page_device").addClass("active");
            $(".id_page_device").addClass("open");
            $(".id_page_controller").addClass("active");

            $('#<%=txtBaudrate.ClientID%>').keydown(function (e) {
                AllowUint(e); // Chi cho phep nhap so
            });

            myFunction();

        });

        function myFunction() {
            var select = document.getElementById('<%=cbCommunicationType.ClientID%>');
            //alert(select.selectedIndex);
            if (select.selectedIndex == 0) {
                document.getElementById("labComport").innerHTML = "Địa chỉ IP";
                document.getElementById("labBaudrate").innerHTML = "Port";
                document.getElementById('<%=txtComport.ClientID%>').placeholder = "192.168.3.xx";
                document.getElementById('<%=txtBaudrate.ClientID%>').placeholder = "80";
            }
            else {
                document.getElementById("labComport").innerHTML = "Comport";
                document.getElementById("labBaudrate").innerHTML = "Baudrate";
                document.getElementById('<%=txtComport.ClientID%>').placeholder = "COM1";
                document.getElementById('<%=txtBaudrate.ClientID%>').placeholder = "9600";
            }
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
        <li>
			<a href="#">Cài đặt thiết bị</a>
		</li>
		<li class="active">Bộ điều khiển</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_controllerdetail" runat="server">
	        Thêm bộ điều khiển mới
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
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên bộ điều khiển </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtControllerName" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Máy tính </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbPC" runat = "server">

                    </select>   
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Truyền thông </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbCommunicationType" runat = "server" onchange="myFunction()">
                        <option>TCP/IP</option>
                        <option>RS232/485/422</option>
                    </select>   
				</div>
			</div>
            <div class="form-group">
				<label id="labComport" class="col-sm-3 control-label no-padding-right" for="form-field-1"> Comport/IP </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtComport" runat="server" class="form-control" placeholder=""/>
				</div>
            </div>
            <div class="form-group">
                <label id="labBaudrate" class="col-sm-3 control-label no-padding-right" for="form-field-1"> Baudrate/Port </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtBaudrate" runat="server" class="form-control" Text="" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Loại bộ điều khiển </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbLineType" runat = "server">
                    </select>   
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Đầu đọc 1 </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbReader1Type" runat = "server">
                        <option>Vào</option>
                        <option>Ra</option>
                    </select>   
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Đầu đọc 2 </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbReader2Type" runat = "server">
                        <option>Vào</option>
                        <option>Ra</option>
                    </select>   
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
            
            <div class="clearfix form-actions">
				<div class="col-md-offset-3 col-md-9">
                    <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-info">
                        <i class="ace-icon fa fa-check bigger-110"></i>
						<span class="bigger-110">Lưu lại</span>
                    </asp:LinkButton>

                    &nbsp; &nbsp; &nbsp;
					<button class="btn" type="button" onclick="window.location.href='Controller.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>

</asp:Content>


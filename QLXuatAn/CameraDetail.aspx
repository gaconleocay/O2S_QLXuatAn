<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="CameraDetail.aspx.cs" Inherits="QLXuatAn_CameraDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_device").addClass("active");
            $(".id_page_device").addClass("open");
            $(".id_page_camera").addClass("active");

            $('#<%=txtHttpPort.ClientID%>').keydown(function (e) {
                AllowUint(e); // Chi cho phep nhap so
            });

            $('#<%=txtRtspPort.ClientID%>').keydown(function (e) {
                AllowUint(e); // Chi cho phep nhap so
            });

            $('#<%=txtChannel.ClientID%>').ace_spinner({ min: 0, max: 100, step: 1, btn_up_class: 'btn-info', btn_down_class: 'btn-info' })
				.closest('.ace-spinner')
				.on('changed.fu.spinbox', function () {
				    //alert($('#spinner1').val())
				});

            $('#<%=txtFrameRate.ClientID%>').ace_spinner({ min: 5, max: 20, step: 5, btn_up_class: 'btn-info', btn_down_class: 'btn-info' })
				.closest('.ace-spinner')
				.on('changed.fu.spinbox', function () {
				    //alert($('#spinner1').val())
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
        <li>
			<a href="#">Cài đặt thiết bị</a>
		</li>
		<li class="active">Camera</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_cameradetail" runat="server">
	        Thêm camera
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
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên camera </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtCameraName" runat="server" class="form-control" placeholder=""/>
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
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Địa chỉ IP </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtHttpURL" runat="server" class="form-control" placeholder=""/>
				</div>
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Cổng Http </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtHttpPort" runat="server" class="form-control" Text="80" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Cổng Rtsp </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtRtspPort" runat="server" class="form-control" Text = "554" placeholder=""/>
				</div>
            </div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên đăng nhập </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtUserName" runat="server" class="form-control" placeholder=""/>
				</div>
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Mật khẩu </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtPassword" TextMode="password" runat="server" class="form-control" placeholder=""/>    
				</div>
			</div>  
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Kênh </label>
				<div class="col-sm-5">
                    <input type="text" id="txtChannel" runat="server" value="0"/>
				</div>
            </div>    
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Loại camera </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbCameraType" runat = "server">
                    </select>   
				</div>
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Định dạng hình ảnh </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbStreamType" runat = "server">
                    </select>   
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Độ phân giải </label>
				<div class="col-sm-5">
					<asp:TextBox id="txtResolution" runat="server" class="form-control" Text = "1920x1080" placeholder=""/> 
				</div>  
            </div>
            <div class="form-group">
                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Số hình/giây </label>
				<div class="col-sm-5">
                    <input type="text" id="txtFrameRate" runat="server" value="5" />
				</div>                
			</div> 
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> SDK </label>
				<div class="col-sm-5">
					<select class="form-control" id="cbSDK" runat = "server">
                    </select>   
				</div>
			</div>  
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-2">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chbEnableRecording" runat="server"/>
				        <span class="lbl"> Ghi hình camera</span>
			        </label>
                </div>
                <div class="col-sm-offset-0 col-sm-3">  
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
					<button class="btn" type="button" onclick="window.location.href='Camera.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>

</asp:Content>


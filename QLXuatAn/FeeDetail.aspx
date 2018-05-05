<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="FeeDetail.aspx.cs" Inherits="QLXuatAn_FeeDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $(".id_page_list").addClass("active");
            $(".id_page_list").addClass("open");
            $(".id_page_fee").addClass("active");

            $('#<%=txtUnitNumber.ClientID%>').keydown(function (e) {
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
		<li class="active">Phí thuê bao</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
<div class="page-header">
        <h1 id="id_feedetail" runat="server">
	        Thêm phí
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
					<asp:TextBox id="txtFeeName" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Nhóm thẻ</label>
				<div class="col-sm-5">                   
					<select class="form-control" id="cbCardGroup" runat = "server">

                    </select>               
				</div>
                
			</div>

            
           
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Mức phí  </label>
				<div class="col-sm-3">
					<asp:TextBox id="txtFeeLevel" runat="server" class="form-control money" placeholder=""/>
				</div>
                <label class="col-sm-1 control-label no-padding-right" for="form-field-2"> VND/</label>
                <div class="col-sm-1">
					<asp:TextBox id="txtUnitNumber" runat="server" class="form-control" Text="1" placeholder=""/>
				</div>
             
				<div class="col-sm-2">                   
					<select class="form-control" id="cbUnitText" runat = "server">           
                        <option value="0">Tháng</option>
                        <option value="1">Quý</option>
                        <option value="2">Năm</option>
                        <option value="3">Ngày</option>
                    </select>               
				</div>
			</div>
            
            
            
            <div class="clearfix form-actions">
				<div class="col-md-offset-3 col-md-9">
                    <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-info">
                        <i class="ace-icon fa fa-check bigger-110"></i>
						<span class="bigger-110">Lưu lại</span>
                    </asp:LinkButton>

                    &nbsp; &nbsp; &nbsp;
					<button class="btn" type="button" onclick="window.location.href='Fee.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>
</asp:Content>


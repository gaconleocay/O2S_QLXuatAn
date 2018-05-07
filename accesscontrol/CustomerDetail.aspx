<%@ Page Title="" Language="C#" MasterPageFile="~/accesscontrol/MasterPage.master" AutoEventWireup="true" CodeFile="CustomerDetail.aspx.cs" Inherits="accesscontrol_CustomerDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_customer").addClass("active");

            LoadAjaxContent2("CardDetailModal.aspx", { 'Id': '', 'CustomerID': '<%=this.ViewState["CurrentID"].ToString()%>' }, 'id_carddetailmodal');

            LoadAjaxContent2("CardModal.aspx", { 'CustomerID': '<%=this.ViewState["CurrentID"].ToString()%>' }, 'id_customer_card');

            $('#fileAvatar').change(function () {
                uploadFile(this.files[0]);
            });          

        });

        function openFileOption() {
            //alert("hi");
            $("#fileAvatar").click();
        }

        function cancelImg() {
            //alert("hi");
            var Avatar = document.getElementById("<%= id_avatar.ClientID %>");
            Avatar.value = '<%=this.ViewState["Avatar"].ToString()%>';
            if (Avatar.value == '') {
                document.getElementById('<%=preViewAvatar.ClientID%>').src = "../assets/avatars/noPhotoAvailable.jpg";
                document.getElementById('<%=picAvatar.ClientID%>').href = "../assets/avatars/noPhotoAvailable.jpg";
            }
            else {
                document.getElementById('<%=preViewAvatar.ClientID%>').src = Avatar.value;
                document.getElementById('<%=picAvatar.ClientID%>').href = Avatar.value;
            }
        }

        function deleteImg() {
            //alert("hi");
            document.getElementById('<%=preViewAvatar.ClientID%>').src = "../assets/avatars/noPhotoAvailable.jpg";
            document.getElementById('<%=picAvatar.ClientID%>').href = "../assets/avatars/noPhotoAvailable.jpg";
            var Avatar = document.getElementById("<%= id_avatar.ClientID %>");
            Avatar.value = "";
        }

        function uploadFile(file) {
            var formData = new FormData();
            formData.append('file', file); // $('#fileAvatar')[0].files[0]);
            formData.append("uploads", "avatar");
            $.ajax({
                url: 'UploaderPhotos.ashx',
                data: formData,
                processData: false,
                contentType: false,
                type: 'POST',
                success: function (data) {
                    //alert(data);
                    //document.getElementById("preViewAvatar").src = data;
                    document.getElementById('<%=preViewAvatar.ClientID%>').src = "../" + data;
                    document.getElementById('<%=picAvatar.ClientID%>').href = "../" + data;
                    var Avatar = document.getElementById("<%= id_avatar.ClientID %>");
                    Avatar.value = "../" + data;
                },
                error: function (errorData) {
                    alert(errorData.responseText);
                }
            });
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Khách hàng</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_customerdetail" runat="server">
	        Thêm khách hàng
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
        <div id="id_carddetailmodal"></div>
    </div>

    <div class="col-xs-12">
        <form id="frm_CustomerDetail" class="form-horizontal" runat="server">
            <!-- #section:elements.tab -->
		    <div class="tabbable">
			    <ul class="nav nav-tabs" id="myTab">
				    <li class="active">
					    <a data-toggle="tab" href="#id_customer_info">
						    <i class="green ace-icon fa fa-user bigger-120"></i>
						    Thông tin chung
					    </a>
				    </li>

				    <li>
					    <a data-toggle="tab" href="#id_customer_card">
                            <i class="green ace-icon fa fa-credit-card bigger-120"></i>
						    Thẻ
						    <%--<span class="badge badge-info">2</span>--%>
					    </a>
				    </li>

			    </ul>

			    <div class="tab-content">
				    <div id="id_customer_info" class="tab-pane fade in active">	
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Mã khách hàng </label>
				            <div class="col-sm-5">
					            <asp:TextBox id="txtCustomerCode" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>			
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Họ tên </label>
				            <div class="col-sm-5">
					            <asp:TextBox id="txtCustomerName" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Nhóm khách hàng </label>
				            <div class="col-sm-5">
					            <select class="form-control" id="cbCustomerGroup" runat = "server">

                                </select>   
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Địa chỉ</label>
				            <div class="col-sm-9">
					            <asp:TextBox id="txtAddress" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Chứng minh thư </label>
				            <div class="col-sm-3">
					            <asp:TextBox id="txtIDNumber" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Số điện thoại </label>
				            <div class="col-sm-3">
					            <asp:TextBox id="txtMobile" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Miêu tả </label>
				            <div class="col-sm-9">
					            <asp:TextBox TextMode="MultiLine" Rows="5" id="txtDescription" runat="server" class="form-control" placeholder=""/>
				            </div>
			            </div>
                        <div class="form-group">
                            <div class="col-sm-offset-3 col-sm-5">  
				                <label class="inline">
				                    <input type="checkbox" class="ace" id="chbEnableAccount" runat="server"/>
				                    <span class="lbl"> Cho phép đăng nhập</span>
			                    </label>
                            </div>
			            </div>
                        <div class="form-group">
				            <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên đăng nhập </label>
				            <div class="col-sm-5">
					            <asp:TextBox id="txtAccount" runat="server" class="form-control" placeholder="Tên đăng nhập"/> 
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
                            <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Ảnh đăng ký </label>
                            <input id="fileAvatar" type="file" size="40" accept="image/*" style="display:none"/>
                            <asp:HiddenField id="id_avatar" runat="server" />
				            <div class="col-sm-5">   
                                <ul class="ace-thumbnails clearfix">  
                                    <li>
									    <a href="../assets/avatars/noPhotoAvailable.jpg" data-rel="colorbox" id="picAvatar" runat="server">
										    <img style="max-width:250px; max-height:250px; " class="img-responsive" alt="Ảnh đăng ký" src="../assets/avatars/noPhotoAvailable.jpg" id="preViewAvatar" runat="server" />
										    <div class="text">
											    <div class="inner">Ảnh đăng ký</div>
										    </div>
									    </a>

									    <div class="tools tools-bottom">                                        
                                            <a href="#" onclick="openFileOption();">
											    <i class="ace-icon fa fa-upload"></i>
										    </a>

										    <a href="#" onclick="cancelImg();">
											    <i class="ace-icon fa fa-times red"></i>
										    </a>

										    <a href="#"  onclick="deleteImg();">
											    <i class="ace-icon fa fa-trash-o"></i>
										    </a>
									    </div>
								    </li>
                                </ul>

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

				    <div id="id_customer_card" class="tab-pane fade">
                        

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
                        <asp:LinkButton ID="LinkButton2" OnClick="Cancel" runat="server" class="btn">
                            <i class="ace-icon fa fa-undo bigger-110"></i>
						    <span class="bigger-110">Quay lại</span>
                        </asp:LinkButton>

				    </div>
		    </div>
        
            </form>
                 
    </div>

</asp:Content>



<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="LaneDetail.aspx.cs" Inherits="QLXuatAn_LaneDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_device").addClass("active");
            $(".id_page_device").addClass("open");
            $(".id_page_lane").addClass("active");     

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
		<li class="active">Làn vào/ra</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_lanedetail" runat="server">
	        Thêm làn vào/ra mới
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
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên làn </label>
				<div class="col-sm-8">
					<asp:TextBox id="txtLaneName" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Máy tính </label>
				<div class="col-sm-8">
                    <asp:DropDownList id="cbPC" class="form-control" AutoPostBack="True" OnSelectedIndexChanged = "Selection_ChangePC" runat="server">

                    </asp:DropDownList>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Loại làn</label>
				<div class="col-sm-8">
                    <asp:DropDownList id="cbLaneType" class="form-control" AutoPostBack="True" OnSelectedIndexChanged = "Selection_ChangeLaneType" runat="server">

                    </asp:DropDownList>
				</div>
			</div>
            <div class="form-group">
                <div class="col-xs-12 col-sm-6">
                    <div class="form-group">
				        <label class="col-sm-6 control-label no-padding-right" for="form-field-1"> C1. Ô tô  </label>
				        <div class="col-sm-6">
					        <select class="form-control" id="cbC1" runat = "server">

                            </select>   
				        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-6 control-label no-padding-right" for="form-field-1"> C2. Xe máy </label>
				        <div class="col-sm-6">
					        <select class="form-control" id="cbC2" runat = "server">

                            </select>   
				        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-6 control-label no-padding-right" for="form-field-1"> C3. Toàn cảnh </label>
				        <div class="col-sm-6">
					        <select class="form-control" id="cbC3" runat = "server">

                            </select>   
				        </div>
                    </div>
                </div>

                <div class="col-xs-12 col-sm-6" id="div_lane2" runat="server">
                    <div class="form-group">
				        <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> C4. Ô tô  </label>
				        <div class="col-sm-7">
					        <select class="form-control" id="cbC4" runat = "server">

                            </select>   
				        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> C5. Xe máy </label>
				        <div class="col-sm-7">
					        <select class="form-control" id="cbC5" runat = "server">

                            </select>   
				        </div>
                    </div>
                    <div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> C6. Toàn cảnh </label>
				        <div class="col-sm-7">
					        <select class="form-control" id="cbC6" runat = "server">

                            </select>   
				        </div>
                    </div>
                </div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Kiểm tra BS lúc vào </label>
				<div class="col-sm-3">
					<select class="form-control" id="cbCheckPlateLevelIn" runat = "server">
                        <option value="1">So sánh >=4 ký tự</option>
                        <option value="2">So sánh tất cả ký tự</option>
                        <option value="0">Không kiểm tra</option>
                    </select>  
				</div>
			</div>
             <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Kiểm tra BS lúc ra </label>
				<div class="col-sm-3">
					<select class="form-control" id="cbCheckPlateLevelOut" runat = "server">                     
                        <option value="1">So sánh >=4 ký tự</option>
                        <option value="2">So sánh tất cả ký tự</option>
                        <option value="0">Không kiểm tra</option>
                    </select>  
				</div>
			</div>
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chbIsLoop" runat="server"/>
				        <span class="lbl"> Sử dụng vòng từ (loop)</span>
			        </label>
                </div>
			</div>
           <%-- <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chbIsCheckPlate" runat="server"/>
				        <span class="lbl"> Kiểm tra biển số lúc ra (toàn bộ ký tự)</span>
			        </label>
                </div>
			</div>--%>
            <%--<div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chIsCheckPlateIn" runat="server"/>
				        <span class="lbl"> Kiểm tra biển số lúc vào</span>
			        </label>
                </div>
			</div>--%>
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chbIsPrint" runat="server"/>
				        <span class="lbl"> Tự động in biên lai</span>
			        </label>
                </div>
			</div>
             <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chIsFree" runat="server"/>
				        <span class="lbl"> Nút miễn phí cho xe ưu tiên</span>
			        </label>
                </div>
			</div>
            <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chIsLED" runat="server"/>
				        <span class="lbl"> Hiển thị LED</span>
			        </label>
                </div>
			</div>
             <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chAccessForEachSide" runat="server"/>
				        <span class="lbl"> Phân quyền cho từng nửa giao diện trái-phải</span>
			        </label>
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
					<button class="btn" type="button" onclick="window.location.href='Lane.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>

</asp:Content>




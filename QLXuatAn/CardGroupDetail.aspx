<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="CardGroupDetail.aspx.cs" Inherits="QLXuatAn_CardTypeDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_list").addClass("active");
            $(".id_page_list").addClass("open");
            $(".id_page_cardgroup").addClass("active");

            $('#<%=dtpDaytimeFrom.ClientID%>').timepicker({
                minuteStep: 1,
                showSeconds: false,
                showMeridian: false
            }).next().on(ace.click_event, function () {
                $(this).prev().focus();
            });

            $('#<%=dtpDaytimeTo.ClientID%>').timepicker({
                minuteStep: 1,
                showSeconds: false,
                showMeridian: false
            }).next().on(ace.click_event, function () {
                $(this).prev().focus();
            });

            var demo1 = $('#<%=cbLane.ClientID%>').bootstrapDualListbox({ infoTextFiltered: '<span class="label label-purple label-lg">Filtered</span>' });
            var container1 = demo1.bootstrapDualListbox('getContainer');
            container1.find('.btn').addClass('btn-white btn-info btn-bold');

            $('.uint').keydown(function (e) {
                AllowUint(e); // Chi cho phep nhap so nguyen duong
            });

            var select = document.getElementById('<%=cbFormulation.ClientID%>');
            //alert(select.selectedIndex);
            if (select.selectedIndex == 0) {
                $("#div_eachfee").show();
                $("#div_block").hide();
                $("#div_timeperiods").hide();
            }
            else if (select.selectedIndex == 1) {
                $("#div_eachfee").hide();
                $("#div_block").show();
                $("#div_timeperiods").hide();
            }
            else if (select.selectedIndex == 2) {
                $("#div_eachfee").hide();
                $("#div_block").hide();
                $("#div_timeperiods").show();
            }

            // Thuc hien bao cao
            $('#<%=cbFormulation.ClientID%>').change(function (e) {
                e.preventDefault();
                //alert("hi");
                var select = document.getElementById('<%=cbFormulation.ClientID%>');
                //alert(select.selectedIndex);
                if (select.selectedIndex == 0) {
                    $("#div_eachfee").show();
                    $("#div_block").hide();
                    $("#div_timeperiods").hide();
                }
                else if (select.selectedIndex == 1) {
                    $("#div_eachfee").hide();
                    $("#div_block").show();
                    $("#div_timeperiods").hide();
                }
                else if (select.selectedIndex == 2) {
                    $("#div_eachfee").hide();
                    $("#div_block").hide();
                    $("#div_timeperiods").show();
                }
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
			<a href="#">Danh mục</a>
		</li>
		<li class="active">Nhóm thẻ</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1 id="id_cardgroupdetail" runat="server">
	        Thêm nhóm thẻ
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
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tên nhóm thẻ </label>
				<div class="col-sm-4">
					<asp:TextBox id="txtCardGroupName" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Miêu tả </label>
				<div class="col-sm-4">
					<asp:TextBox id="txtDescription" runat="server" class="form-control" placeholder=""/>
				</div>
			</div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Loại thẻ </label>
				<div class="col-sm-4">
					<select class="form-control" id="cbCardType" runat = "server">
                        <option>Thuê bao</option>
                        <option>Thẻ lượt</option>
                        <option>Miễn phí</option>
                    </select>   
				</div>
                <div class="col-sm-offset-0 col-sm-3">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chIsHaveMoneyExcessTime" runat="server"/>
				        <span class="lbl"> Tính tiền thẻ thuê bao quá giờ</span>
			        </label>
                </div>
            </div>
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Nhóm xe </label>
				<div class="col-sm-4">
					<select class="form-control" id="cbVehicleGroup" runat = "server">
                    </select>   
				</div>
            </div>

            <div class="hr hr-16 hr-dotted"></div>

            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-top" for="cbLane"> Phân làn cho nhóm thẻ </label>
				<div class="col-sm-8">
					<!-- #section:plugins/input.duallist -->
					<select multiple="" class="form-control" name="duallistbox_demo1[]" id="cbLane" runat="server" >
						
					</select>

					<!-- /section:plugins/input.duallist -->
					<%--<div class="hr hr-16 hr-dotted"></div>--%>
				</div>
            </div>

            <div class="hr hr-16 hr-dotted"></div>
             <div class="form-group">
                <div class="col-sm-offset-3 col-sm-4">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chEnableFree" runat="server"/>
				        <span class="lbl"> Miễn phí trong khoảng thời gian(phút) <=</span>                        
			        </label>                                      
                </div>
                <div class="col-sm-1">                  
				       <asp:TextBox id="txtFreeTime" runat="server" class="form-control" placeholder="" Text="0"/>				      
				</div>
			</div> 
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> TG ban ngày(TG hợp lệ thẻ thuê bao) từ </label>
				<div class="col-sm-2">
					<div class="input-group bootstrap-timepicker">
						<input id="dtpDaytimeFrom" type="text" class="form-control" runat="server"/>
						<span class="input-group-addon">
							<i class="fa fa-clock-o bigger-110"></i>
						</span>
					</div>
				</div>
                <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> đến </label>
				<div class="col-sm-2">
					<div class="input-group bootstrap-timepicker">
						<input id="dtpDaytimeTo" type="text" class="form-control" runat="server"/>
						<span class="input-group-addon">
							<i class="fa fa-clock-o bigger-110"></i>
						</span>
					</div>
				</div>
			</div>
           
            <div class="form-group">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Tính phí theo </label>
				<div class="col-sm-4">
					<select class="form-control" id="cbFormulation" runat = "server">
                        <option>Lượt</option>
                        <option>Block</option>
                        <option>Khoảng thời gian</option>
                    </select>   
				</div>
            </div>
            <div class="form-group" id="div_eachfee">
				<label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Phí lượt (VNĐ) </label>
				<div class="col-sm-2">
					<asp:TextBox id="txtEachFee" runat="server" class="form-control money" placeholder="" Text="0"/>
				</div>
			</div>
            <div id="div_block">
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Block 0 (VNĐ) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtBlock0" runat="server" class="form-control money" placeholder="" Text="0"/>
				    </div>
                    <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Thời gian (Phút) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtTime0" runat="server" class="form-control uint" placeholder="" Text="0"/>
				    </div>
			    </div>
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Block 1 (VNĐ) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtBlock1" runat="server" class="form-control money" placeholder="" Text="0"/>
				    </div>
                    <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Thời gian (Phút) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtTime1" runat="server" class="form-control uint" placeholder="" Text="0"/>
				    </div>
			    </div>
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Block 2 (VNĐ) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtBlock2" runat="server" class="form-control money" placeholder="" Text="0"/>
				    </div>
                    <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Thời gian (Phút) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtTime2" runat="server" class="form-control uint" placeholder="" Text="0"/>
				    </div>
			    </div>
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Block 3 (VNĐ) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtBlock3" runat="server" class="form-control money" placeholder="" Text="0"/>
				    </div>
                    <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Thời gian (Phút) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtTime3" runat="server" class="form-control uint" placeholder="" Text="0"/>
				    </div>
			    </div>
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Block 4 (VNĐ) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtBlock4" runat="server" class="form-control money" placeholder="" Text="0"/>
				    </div>
                    <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Thời gian (Phút) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtTime4" runat="server" class="form-control uint" placeholder="" Text="0"/>
				    </div>
			    </div>
                 <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Block 5 (VNĐ) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtBlock5" runat="server" class="form-control money" placeholder="" Text="0"/>
				    </div>
                    <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Thời gian (Phút) </label>
				    <div class="col-sm-2">
					    <asp:TextBox id="txtTime5" runat="server" class="form-control uint" placeholder="" Text="0"/>
				    </div>
			    </div>
            </div>
            <div id="div_timeperiods">
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Khoảng thời gian </label>
				    <div class="col-sm-8">
					    <asp:TextBox id="txtTimePeriods" runat="server" class="form-control" placeholder="7:00-19:00-22:00-7:00"/>
				    </div>
			    </div>
                <div class="form-group">
				    <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Phí tương ứng (VNĐ) </label>
				    <div class="col-sm-8">
					    <asp:TextBox id="txtCosts" runat="server" class="form-control" placeholder="3000-4000-7000"/>
				    </div>
			    </div>
            </div>
             <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chIsCheckPlate" runat="server"/>
				        <span class="lbl"> Kiểm tra biển số</span>
			        </label>
                </div>
			</div>
             <div class="form-group">
                <div class="col-sm-offset-3 col-sm-5">  
				    <label class="inline">
				        <input type="checkbox" class="ace" id="chIsHaveMoneyExpiredDate" runat="server"/>
				        <span class="lbl"> Tính tiền thuê bao hết hạn sử dụng</span>
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
					<button class="btn" type="button" onclick="window.location.href='CardGroup.aspx'">
						<i class="ace-icon fa fa-undo bigger-110"></i>
						Quay lại
					</button>

				</div>
			</div>
        </form>
    </div>

</asp:Content>


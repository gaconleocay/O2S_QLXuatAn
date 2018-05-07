<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardDetailModal.aspx.cs" Inherits="accesscontrol_CardDetailModal" %>

<div id="modal-form" class="modal" tabindex="-1">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal">&times;</button>
				<h4 class="blue bigger" id="id_carddetail" runat="server">Thêm mới thẻ</h4>
			</div>

			<div class="modal-body">           
				<div class="row">
					<div class="col-xs-12">
                        <div class="form-horizontal">
                            <%--<h5 class="header smaller lighter blue">
				                Thời gian
			                </h5>--%>
                            <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Số thứ tự </label>
				                <div class="col-sm-4">
                                    <input type="text" id="txtCardNo" placeholder="" class="form-control" runat="server" />
				                </div>
			                </div>
                            <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Mã thẻ </label>
				                <div class="col-sm-4">
                                    <input type="text" id="txtCardNumber" placeholder="" class="form-control" runat="server" />
				                </div>
			                </div>
						    <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Nhóm thẻ</label>
				                <div class="col-sm-8">                   
					                <select class="form-control" id="cbCardGroup" runat = "server">

                                    </select>               
				                </div>
			                </div>
                            <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-1"> Ngày hết hạn </label>
				                <div class="col-sm-4">
					                <div class="input-group">
						                <input class="form-control date-picker input-append date" id="dtpExpireDate" runat="server" type="text" data-date-format="dd/mm/yyyy"/>
						                <span class="input-group-addon">
							                <i class="fa fa-calendar bigger-110"></i>
						                </span>
					                </div>
				                </div>
			                </div>
                            <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-5">  
				                    <label class="inline">
				                        <input type="checkbox" class="ace" id="chbIsLock" runat="server"/>
				                        <span class="lbl"> Khóa thẻ</span>
			                        </label>
                                </div>
			                </div>
                        </div>
					</div>
				</div>
			</div>

			<div class="modal-footer">
                <button class="btn btn-sm btn-primary" id="btnSave">
					<i class="ace-icon fa fa-check"></i>
					Đồng ý
				</button>

				<button class="btn btn-sm" data-dismiss="modal" id="btnClose">
					<i class="ace-icon fa fa-times"></i>
					Hủy bỏ
				</button>			
			</div>

		</div>
	</div>
</div>

<script type="text/javascript">
    jQuery(document).ready(function () {


        $('.modal').on('hidden.bs.modal', function () {
            //alert("hi");
            document.getElementById('<%=txtCardNo.ClientID%>').value = "";
            document.getElementById('<%=txtCardNumber.ClientID%>').value = "";
            document.getElementById('<%=cbCardGroup.ClientID%>').selectedIndex = 0;
        });

        $('#btnSave').click(function (e) {

            AjaxPOST("CardDetailModal.aspx/Save", '{"Id":"' + '<%=this.Id%>' +
                    '", "CardNo":"' + document.getElementById('<%=txtCardNo.ClientID%>').value +
                    '", "CardNumber":"' + document.getElementById('<%=txtCardNumber.ClientID%>').value +
                    '", "CardGroupID":"' + document.getElementById('<%=cbCardGroup.ClientID%>').value +
                    '", "CustomerID":"' + '<%=this.ViewState["CustomerID"].ToString()%>' +
                    '", "ExpireDate":"' + document.getElementById('<%=dtpExpireDate.ClientID%>').value +
                    '", "IsLock":"' + document.getElementById('<%=chbIsLock.ClientID%>').checked +
                    '"}').success(function (result) {
                        //alert(result.d.toString());
                        if (result.d.toString() == "true") {
                            $("#btnClose").click(); // dong hoi thoai
                            LoadAjaxContent2("CardDetailModal.aspx", { 'Id': '', 'CustomerID': '<%=this.ViewState["CustomerID"].ToString()%>' }, 'id_carddetailmodal');
                            LoadAjaxContent2("CardModal.aspx", { 'CustomerID': '<%=this.ViewState["CustomerID"].ToString()%>' }, 'id_customer_card');
                        }
                        else {
                            alert(result.d.toString());
                        }
                    });

        });

    });
</script>

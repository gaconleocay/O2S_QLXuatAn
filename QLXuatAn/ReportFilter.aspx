<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="ReportFilter.aspx.cs" Inherits="QLXuatAn_ReportFilter" %>


<div id="modal-form" class="modal" tabindex="-1">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header">
				<button type="button" class="close" data-dismiss="modal">&times;</button>
				<h4 class="blue bigger">Lọc báo cáo</h4>
			</div>

			<div class="modal-body">           
				<div class="row">
					<div class="col-xs-12">
                        <div class="form-horizontal">
                            <%--<h5 class="header smaller lighter blue">
				                Thời gian
			                </h5>--%>
                            <div class="form-group">
				                <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Từ ngày </label>
				                <div class="col-sm-4">
					                <div class="input-group">
						                <input class="form-control date-picker input-append date" id="id-date-picker-1" type="text" data-date-format="dd/mm/yyyy"/>
						                <span class="input-group-addon">
							                <i class="fa fa-calendar bigger-110"></i>
						                </span>
					                </div>
				                </div>
                                <label class="col-sm-2 control-label no-padding-right" for="form-field-1"> Đến ngày </label>
				                <div class="col-sm-4">
					                <div class="input-group">
						                <input class="form-control date-picker" id="id-date-picker-2" type="text" data-date-format="dd/mm/yyyy"/>
						                <span class="input-group-addon">
							                <i class="fa fa-calendar bigger-110"></i>
						                </span>
					                </div>
				                </div>
			                </div>
                            <h5 class="header smaller lighter blue">
				                Đối tượng
			                </h5>
						    <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Loại thẻ</label>
				                <div class="col-sm-9">                   
					                <select class="form-control" id="cbCardType" runat = "server">

                                    </select>               
				                </div>
			                </div>
                            <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Nhóm sản phẩm</label>
				                <div class="col-sm-9">                   
					                <select class="form-control" id="cbProductCategory" runat = "server">

                                    </select>               
				                </div>
			                </div>
                            <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Sản phẩm</label>
				                <div class="col-sm-9">                   
					                <select class="form-control" id="cbProduct" runat = "server">

                                    </select>               
				                </div>
			                </div>
                            <div class="form-group">
				                <label class="col-sm-3 control-label no-padding-right" for="form-field-2"> Nhân viên BH</label>
				                <div class="col-sm-9">                   
					                <select class="form-control" id="cbUser" runat = "server">

                                    </select>               
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

				<button class="btn btn-sm" data-dismiss="modal">
					<i class="ace-icon fa fa-times"></i>
					Hủy bỏ
				</button>			
			</div>

		</div>
	</div>
</div>

<script type="text/javascript">
    jQuery(document).ready(function () {

        document.getElementById("id-date-picker-1").value = '<%= this.FromDate() %>';
        document.getElementById("id-date-picker-2").value = '<%= this.ToDate() %>';

        //datepicker plugin
        //link
        $('.date-picker').datepicker({
            autoclose: true,
            todayHighlight: true
        })
        //show datepicker when clicking on the icon
        .next().on(ace.click_event, function () {
            $(this).prev().focus();
        });

        $(document).on('change', "#cbProductCategory", function () {

            AjaxPOST("ReportFilter.aspx/GetProductByProductCategory", '{"ProductCategoryID":"' + document.getElementById("cbProductCategory").value +
                    '"}').success(function (result) {
                        //alert(result.d.toString());
                        var jsonData = JSON.parse(result.d.toString());
                        if (jsonData != null && jsonData.length >= 1) {
                            //alert(jsonData.length.toString());
                            $('#cbProduct').empty();
                            $('#cbProduct').append($('<option>', {
                                value: '',
                                text: ''
                            }));
                            for (var i = 0; i < jsonData.length; i++) {
                                //alert(jsonData[i].name);
                                $('#cbProduct').append($('<option>', {
                                    value: jsonData[i].Value,
                                    text: jsonData[i].Text
                                }));
                            }
                        }
                        else {
                            $('#cbProduct').empty();
                        }
                    });

        });


        $('#btnSave').click(function (e) {
            window.location.href = '<%= this.PageName %>?CardType=' + document.getElementById("<%= cbCardType.ClientID %>").value +
                '&ProductCategory=' + document.getElementById("<%= cbProductCategory.ClientID %>").value +
                '&Product=' + document.getElementById("<%= cbProduct.ClientID %>").value +
                '&UserCode=' + document.getElementById("<%= cbUser.ClientID %>").value +
                '&FromDate=' + document.getElementById("id-date-picker-1").value +
                '&ToDate=' + document.getElementById("id-date-picker-2").value;
        });

    });
</script>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardDetailModal.aspx.cs"
    Inherits="QLXuatAn_CardDetailModal" %>
<style>
    .plcCardId{position:relative;}
    .boxAutocompleteCard{
        margin-left:0px;
        list-style:none;
        padding:0;
        width:100%;
        display:none;
        position:absolute;
        top:30px;
        z-index:9;
    }
</style>
<div id="modal-form" class="modal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">
                    &times;</button>
                <h4 class="blue bigger" id="id_carddetail" runat="server">
                    Thêm mới thẻ</h4>
            </div>
            <div class="modal-body">
                <div class="row">
                    <div class="col-xs-12">
                        <div class="form-horizontal">
                            <%--<h5 class="header smaller lighter blue">
				                Thời gian
			                </h5>--%>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-2">
                                    Danh sách thẻ</label>
                                <div class="col-sm-8">
                                    <%--<select class="form-control chosen-select" id="cbCardList" data-placeholder="Select card..."
                                        onchange="myFunction()" runat="server">
                                    </select>--%>

                                    <div class="plcCardId">
                                    <input type="text" placeholder="Tìm mã thẻ" class="nav-search-input form-control" id="nav-card-input"/>
                                    <input type="hidden" name="CardId" runat="server" value="" />
                                        <ul class="boxAutocompleteCard ui-autocomplete ui-menu">
                                           <%-- <li class="ui-menu-item" id="123">
                                                123
                                            </li>
                                            <li class="ui-menu-item" id="123">
                                                123
                                            </li>
                                            <li class="ui-menu-item" id="123">
                                                123
                                            </li>--%>
                                        </ul>
                                </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Số thứ tự
                                </label>
                                <div class="col-sm-4">
                                    <input type="text" id="txtCardNo" placeholder="" class="form-control" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Mã thẻ
                                </label>
                                <div class="col-sm-4">
                                    <input type="text" id="txtCardNumber" placeholder="" class="form-control" runat="server" />
                                    <input type="hidden" id="cardID" value="" class="form-control" runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-2">
                                    Nhóm thẻ</label>
                                <div class="col-sm-8">
                                    <select class="form-control" id="cbCardGroup" runat="server">
                                    </select>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Ngày đăng ký
                                </label>
                                <div class="col-sm-4">
                                    <div class="input-group">
                                        <input class="form-control datePicker" id="dtpRegisterDate" runat="server"
                                            type="text"/>
                                        <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                                    </div>
                                    <%--<asp:HiddenField id="hidRegisterDate" runat="server"/>--%>
                                    <input type="hidden" id="hidRegisterDate" runat="server"/>
                                </div>
                                <div class="col-sm-3">
                                   <%-- <button class="btn btn-default btn-sm" id="btnUpdateDateRegister">
                                        <i class="ace-icon fa fa-refresh bigger-110"></i><span class="">Cập nhập</span>
                                    </button>--%>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Ngày phát
                                </label>
                                <div class="col-sm-4">
                                    <div class="input-group">
                                        <input class="form-control datePicker" id="dtpReleaseDate" runat="server"
                                            type="text" />
                                        <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                                    </div>
                                    <%--<asp:HiddenField  />--%>
                                    <input type="hidden" id="hidReleaseDate" runat="server"/>
                                </div>
                                <div class="col-sm-3">
                                    <%--<button class="btn btn-default btn-sm" id="btnUpdateDateRelease">
                                        <i class="ace-icon fa fa-refresh bigger-110"></i><span class="">Cập nhập</span>
                                    </button>--%>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Ngày hết hạn
                                </label>
                                <div class="col-sm-4">
                                    <div class="input-group">
                                        <input class="form-control datePicker" id="dtpExpireDate" runat="server"
                                            type="text" />
                                        <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Xe 1
                                </label>
                                <div class="col-sm-4">
                                    <input type="text" id="txtPlate1" placeholder="Biển số xe 1" class="form-control"
                                        runat="server" />
                                </div>
                                <div class="col-sm-4">
                                    <input type="text" id="txtVehicleName1" placeholder="Tên xe 1" class="form-control"
                                        runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Xe 2
                                </label>
                                <div class="col-sm-4">
                                    <input type="text" id="txtPlate2" placeholder="Biển số xe 2" class="form-control"
                                        runat="server" />
                                </div>
                                <div class="col-sm-4">
                                    <input type="text" id="txtVehicleName2" placeholder="Tên xe 2" class="form-control"
                                        runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Xe 3
                                </label>
                                <div class="col-sm-4">
                                    <input type="text" id="txtPlate3" placeholder="Biển số xe 3" class="form-control"
                                        runat="server" />
                                </div>
                                <div class="col-sm-4">
                                    <input type="text" id="txtVehicleName3" placeholder="Tên xe 3" class="form-control"
                                        runat="server" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-5">
                                    <label class="inline">
                                        <input type="checkbox" class="ace" id="chbIsLock" runat="server" />
                                        <span class="lbl">Khóa thẻ</span>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-sm btn-primary" id="btnSave">
                    <i class="ace-icon fa fa-check"></i>Đồng ý
                </button>
                <button class="btn btn-sm" data-dismiss="modal" id="btnClose">
                    <i class="ace-icon fa fa-times"></i>Hủy bỏ
                </button>
            </div>
        </div>
    </div>
</div>
<script type="text/javascript">
    jQuery(document).ready(function () {
        
        if (!ace.vars['old_ie']) $('.datePicker').datetimepicker({
            //format: 'MM/DD/YYYY h:mm:ss A',//use this option to display seconds
            format: 'DD/MM/YYYY',
            icons: {
                time: 'fa fa-clock-o',
                date: 'fa fa-calendar',
                up: 'fa fa-chevron-up',
                down: 'fa fa-chevron-down',
                previous: 'fa fa-chevron-left',
                next: 'fa fa-chevron-right',
                today: 'fa fa-arrows ',
                clear: 'fa fa-trash',
                close: 'fa fa-times'
            }
        }).next().on(ace.click_event, function () {
            $(this).prev().focus();
        });
        /////////////////////////////
        $('.modal').on('shown.bs.modal', function () {
            if (!ace.vars['touch']) {

                //$('.chosen-select').chosen({ allow_single_deselect: true });
                //resize the chosen on window resize
                $(window)
                    .off('resize.chosen')
                    .on('resize.chosen', function () {
                        $('.chosen-select').each(function () {
                            var $this = $(this);
                            $this.next().css({ 'width': $this.parent().width() });
                        })
                    }).trigger('resize.chosen');
                //resize chosen on sidebar collapse/expand
                $(document).on('settings.ace.chosen', function (e, event_name, event_val) {
                    if (event_name != 'sidebar_collapsed') return;
                    $('.chosen-select').each(function () {
                        var $this = $(this);
                        $this.next().css({ 'width': $this.parent().width() });
                    })
                });
                $('#chosen-multiple-style .btn').on('click', function (e) {
                    var target = $(this).find('input[type=radio]');
                    var which = parseInt(target.val());
                    if (which == 2) $('#form-field-select-4').addClass('tag-input-style');
                    else $('#form-field-select-4').removeClass('tag-input-style');
                });


            }


            $("#nav-card-input").focus(function () {
                this.select();
            });
            $("#nav-card-input").keyup(function () {
                var cmd = $(this);
                if (cmd.val() == '') {
                    $('.plcCardId').find('input[type=hidden]').val('');
                    $('.plcCardId').find('ul.boxAutocompleteCard').hide();
                    return false;
                }
                $('.plcCardId').find('ul.boxAutocompleteCard').show();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'Card.aspx/getCardByAutocomplete',
                    dataType: "json",
                    data: "{ 'name': '" + cmd.val() + "'}",
                    success: function (result) {
                        //debugger;
                        var strLi = '';
                        for (var i = 0; i < result.d.length; i++) {
                            //alert(result.d[i].Text + '-' + result.d[i].Value);
                            //itemnamecodes[i] = { label: result.d[i].Text, Id: result.d[i].Value };
                            strLi += '<li class="ui-menu-item" id="' + result.d[i].Value + '">' + result.d[i].Text + '</li>';
                        }
                        //response(itemnamecodes);
                        $('.plcCardId').find('ul.boxAutocompleteCard').html(strLi);
                    },
                    error: function (result) {
                        //debugger;
                        alert(result);
                    }
                });
            });

            $('.plcCardId').on('click', 'ul.boxAutocompleteCard li', function () {
                var cmd = $(this);
                $('#nav-card-input').val(cmd.text());
                $('.plcCardId input[type=hidden]').val(cmd.attr('id'));
                cmd.parent().hide();
                myFunction(cmd.attr('id'));
            });
            //$("#nav-card-input").autocomplete({
            //    delay: 300,
            //    source: function (request, response) {
            //        var itemnamecodes = new Array();
                    
            //    },
            //    minLength: 1,
            //    select: function (event, ui) {
            //        debugger;
            //        $('.plcCardId').find('input[type=hidden]').val(ui.item.Id);
            //        myFunction(ui.item.Id);
            //    },
            //    open: function () {
            //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            //    },
            //    close: function () {
            //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            //    }
            //});
        });
        ////////////////////////////
        $('#btnUpdateDateRelease').click(function () {
            AjaxPOST("CardDetailModal.aspx/UpdateDateRelease", '{"Id":"' + '<%=this.Id%>' +
                    '", "CardNo":"' + document.getElementById('<%=txtCardNo.ClientID%>').value +
                    '", "CardNumber":"' + document.getElementById('<%=txtCardNumber.ClientID%>').value +
                    '", "CardGroupID":"' + document.getElementById('<%=cbCardGroup.ClientID%>').value +
                    '", "CustomerID":"' + '<%=this.ViewState["CustomerID"].ToString()%>' +
                    '", "ExpireDate":"' + document.getElementById('<%=dtpExpireDate.ClientID%>').value +
                    '", "RegisterDate":"' + document.getElementById('<%=dtpRegisterDate.ClientID%>').value +
                    '", "ReleaseDate":"' + document.getElementById('<%=dtpReleaseDate.ClientID%>').value +
                    '", "IsLock":"' + document.getElementById('<%=chbIsLock.ClientID%>').checked +
                    '", "Plate1":"' + document.getElementById('<%=txtPlate1.ClientID%>').value +
                    '", "VehicleName1":"' + document.getElementById('<%=txtVehicleName1.ClientID%>').value +
                    '", "Plate2":"' + document.getElementById('<%=txtPlate2.ClientID%>').value +
                    '", "VehicleName2":"' + document.getElementById('<%=txtVehicleName2.ClientID%>').value +
                    '", "Plate3":"' + document.getElementById('<%=txtPlate3.ClientID%>').value +
                    '", "VehicleName3":"' + document.getElementById('<%=txtVehicleName3.ClientID%>').value +
                    '", "cardID":"' + document.getElementById('<%=cardID.ClientID%>').value +
                    '", "userID":"' + '<%=this.ViewState["UserID"].ToString()%>' +
                    '"}').success(function (result) {
                        alert("Cập nhật thành công")
                    });

        });
        ///////////////////////////////
        $('#btnUpdateDateRegister').click(function () {
            AjaxPOST("CardDetailModal.aspx/UpdateDateRegister", '{"Id":"' + '<%=this.Id%>' +
                        '", "CardNo":"' + document.getElementById('<%=txtCardNo.ClientID%>').value +
                        '", "CardNumber":"' + document.getElementById('<%=txtCardNumber.ClientID%>').value +
                        '", "CardGroupID":"' + document.getElementById('<%=cbCardGroup.ClientID%>').value +
                        '", "CustomerID":"' + '<%=this.ViewState["CustomerID"].ToString()%>' +
                        '", "ExpireDate":"' + document.getElementById('<%=dtpExpireDate.ClientID%>').value +
                        '", "RegisterDate":"' + document.getElementById('<%=dtpRegisterDate.ClientID%>').value +
                        '", "ReleaseDate":"' + document.getElementById('<%=dtpReleaseDate.ClientID%>').value +
                        '", "IsLock":"' + document.getElementById('<%=chbIsLock.ClientID%>').checked +
                        '", "Plate1":"' + document.getElementById('<%=txtPlate1.ClientID%>').value +
                        '", "VehicleName1":"' + document.getElementById('<%=txtVehicleName1.ClientID%>').value +
                        '", "Plate2":"' + document.getElementById('<%=txtPlate2.ClientID%>').value +
                        '", "VehicleName2":"' + document.getElementById('<%=txtVehicleName2.ClientID%>').value +
                        '", "Plate3":"' + document.getElementById('<%=txtPlate3.ClientID%>').value +
                        '", "VehicleName3":"' + document.getElementById('<%=txtVehicleName3.ClientID%>').value +
                        '", "cardID":"' + document.getElementById('<%=cardID.ClientID%>').value +
                        '", "userID":"' + '<%=this.ViewState["UserID"].ToString()%>' +
                        '"}').success(function (result) {
                            alert("Cập nhật thành công")
                        });
        });
        ////////////////////////////////
        $('#btnSave').click(function (e) {

            AjaxPOST("CardDetailModal.aspx/Save", '{"Id":"' + '<%=this.Id%>' +
                    '", "CardNo":"' + document.getElementById('<%=txtCardNo.ClientID%>').value +
                    '", "CardNumber":"' + document.getElementById('<%=txtCardNumber.ClientID%>').value +
                    '", "CardGroupID":"' + document.getElementById('<%=cbCardGroup.ClientID%>').value +
                    '", "CustomerID":"' + '<%=this.ViewState["CustomerID"].ToString()%>' +
                    '", "ExpireDate":"' + document.getElementById('<%=dtpExpireDate.ClientID%>').value +
                    '", "RegisterDate":"' + document.getElementById('<%=dtpRegisterDate.ClientID%>').value +
                    '", "ReleaseDate":"' + document.getElementById('<%=dtpReleaseDate.ClientID%>').value +
                    '", "IsLock":"' + document.getElementById('<%=chbIsLock.ClientID%>').checked +
                    '", "Plate1":"' + document.getElementById('<%=txtPlate1.ClientID%>').value +
                    '", "VehicleName1":"' + document.getElementById('<%=txtVehicleName1.ClientID%>').value +
                    '", "Plate2":"' + document.getElementById('<%=txtPlate2.ClientID%>').value +
                    '", "VehicleName2":"' + document.getElementById('<%=txtVehicleName2.ClientID%>').value +
                    '", "Plate3":"' + document.getElementById('<%=txtPlate3.ClientID%>').value +
                    '", "VehicleName3":"' + document.getElementById('<%=txtVehicleName3.ClientID%>').value +
                    '", "cardID":"' + document.getElementById('<%=cardID.ClientID%>').value +
                    '", "OldRegisterDate":"' + document.getElementById('<%=hidRegisterDate.ClientID%>').value +
                    '", "OldReleaseDate":"' + document.getElementById('<%=hidReleaseDate.ClientID%>').value +
                    '", "userID":"' + '<%=this.ViewState["UserID"].ToString()%>' +
                    '"}').success(function (result) {
                        //                        alert(result.d.toString());
                        //                        alert(result.d.toString());
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


    function myFunction(_id) {
        var obj = {};
        obj.cardnumber = _id;


        AjaxPostMultiParams("CardDetailModal.aspx/Change", obj).success(function (result) {
            if (result.d.toString() != "") {

                //                alert(result.d.toString());
                var tem = result.d.toString().split(';');
                if (tem != null && tem.length == 4) {



                    document.getElementById('<%=cardID.ClientID%>').value = tem[0];
                    document.getElementById('<%=txtCardNo.ClientID%>').value = tem[1];
                    document.getElementById('<%=txtCardNumber.ClientID%>').value = obj.cardnumber;
                    document.getElementById('<%=dtpExpireDate.ClientID%>').value = tem[2];
                    document.getElementById('<%=cbCardGroup.ClientID%>').value = tem[3];

                    document.getElementById('<%=dtpExpireDate.ClientID%>').setAttribute('readonly', 'readonly');
                    document.getElementById('<%=chbIsLock.ClientID%>').disabled = true;
                }
                else
                    alert("Failed");
            }
        });
    }
</script>
<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true"
    CodeFile="CardDetail.aspx.cs" Inherits="QLXuatAn_CardDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- JS Global Compulsory -->
    <script src="../Scripts/jquery-1.4.1.min.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $("#plcInputCardNumber").keydown(function (event) {
                if (event.keyCode == 13) {
                    if ($("#plcInputCardNumber").val().length == 0) {
                        event.preventDefault();
                        return false;
                    }
                }
            });
            //Autocomplêt CardSub
            $("#plcInputCardNumber").autocomplete({
                source: function (request, response) {
                    var itemnamecodes = new Array();
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'CardDetail.aspx/getCardNumberByAutocomplete',
                        dataType: "json",
                        data: "{ 'name': '" + request.term + "'}",
                        success: function (result) {
                            //debugger;
                            for (var i = 0; i < result.d.length; i++) {
                                //alert(result.d[i].Text + '-' + result.d[i].Value);
                                itemnamecodes[i] = { label: result.d[i].Text, Id: result.d[i].Value };
                            }
                            response(itemnamecodes);
                        },
                        error: function (result) {
                            //debugger;
                            alert(result);
                        }
                    });
                },
                minLength: 1,
                select: function (event, ui) {
                    //debugger;
                    $('#<%=txtSubCardCode.ClientID%>').val(ui.item.Id);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });


            $("body").on('click', 'a[name=btnDeleteSub]', function () {
                var cmd = $(this);
                if (confirm('Bạn có chắc chắn muốn xóa thẻ phụ này?')) {
                    var _v = cmd.attr('idata');
                    if (_v != undefined && _v != '') {
                        $.ajax({
                            method: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: 'CardDetail.aspx/DeleteSubCard',
                            dataType: "json",
                            data: "{ 'subCardId': '" + _v + "'}",
                            success: function (result) {
                                if (result.d=="1") {
                                    cmd.parent().parent().fadeOut();
                                } else {
                                    alert('Xóa không thành công');
                                }
                            },
                            error: function (result) {
                                //debugger;
                                alert(result);
                            }
                        });
                    }
                }
            });


            $("#nav-customer-input").focus(function () {
                this.select();
            });

            $('body').on('click', '#ui-id-2 li', function () {
                //alert($(this).text());
                if ($(this).text() === 'Xóa khách hàng') {
                    $('#<%=hidCustomer.ClientID%>').val('');
                $('#<%=txtDescription.ClientID%>').val('');
                $('#<%=txtCustomerName.ClientID%>').val('');
                    $('#<%=chbInactive.ClientID%>').prop('checked', false);

                    $('#<%=txtCustomerCode.ClientID%>').prop('value', '');
                    $('#<%=txtAddress.ClientID%>').val('');
                    $('#<%=txtIDNumber.ClientID%>').val('');
                    $('#<%=txtMobile.ClientID%>').val('');
                    $('#<%=cbCustomerGroup.ClientID%>').val('');
                    $('#<%=chbEnableAccount.ClientID%>').prop('checked', false);
                    $('#<%=txtAccount.ClientID%>').val('');
                    $('#<%=txtPassword.ClientID%>').val('');
                    $('#<%=id_avatar.ClientID%>').val('');
                //return false;
            }
            var _id = $('#<%=hidCustomer.ClientID%>').val();

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'CardDetail.aspx/getCusInfo',
                dataType: "json",
                data: "{ 'cusId': '" + _id + "'}",
                success: function (result) {
                    var obj = JSON.parse(result.d);
                    $.each(obj, function (i, item) {
                        //alert(v.CustomerName);
                        $('#<%=txtDescription.ClientID%>').val($.trim(item.Description));
                            $('#<%=txtCustomerName.ClientID%>').val($.trim(item.CustomerName));
                            if (item.Inactive == "True") {
                                $('#<%=chbInactive.ClientID%>').prop('checked', true);
                            } else {
                                $('#<%=chbInactive.ClientID%>').prop('checked', false);
                            }

                            $('#<%=txtCustomerCode.ClientID%>').val($.trim(item.CustomerCode));
                            $('#<%=txtAddress.ClientID%>').val($.trim(item.Address));
                            $('#<%=txtIDNumber.ClientID%>').val($.trim(item.IDNumber));
                            $('#<%=txtMobile.ClientID%>').val($.trim(item.Mobile));
                            $('#<%=cbCustomerGroup.ClientID%>').val(item.CustomerGroupID);
                            if (item.EnableAccount == "True") {
                                $('#<%=chbEnableAccount.ClientID%>').prop('checked', true);
                            } else {
                                $('#<%=chbEnableAccount.ClientID%>').prop('checked', false);
                            }
                            $('#<%=txtAccount.ClientID%>').val($.trim(item.Account));
                            $('#<%=txtPassword.ClientID%>').val(item.Password);
                            $('#<%=id_avatar.ClientID%>').val(item.Avatar);

                        });
                    },
                    Error: function () {
                        alert('error');
                    }

                });
        });


        $("#nav-customer-input").keydown(function (event) {
            if (event.keyCode == 13) {

                if ($('#<%=hidCustomer.ClientID%>').val()=== '') {
                    $('#<%=hidCustomer.ClientID%>').val('');
                    $('#<%=txtDescription.ClientID%>').val('');
                    $('#<%=txtCustomerName.ClientID%>').val('');
                    $('#<%=chbInactive.ClientID%>').prop('checked', false);

                    $('#<%=txtCustomerCode.ClientID%>').prop('value', '');
                    $('#<%=txtAddress.ClientID%>').val('');
                    $('#<%=txtIDNumber.ClientID%>').val('');
                    $('#<%=txtMobile.ClientID%>').val('');
                    $('#<%=cbCustomerGroup.ClientID%>').val('');
                    $('#<%=chbEnableAccount.ClientID%>').prop('checked', false);
                    $('#<%=txtAccount.ClientID%>').val('');
                    $('#<%=txtPassword.ClientID%>').val('');
                    $('#<%=id_avatar.ClientID%>').val('');
                    //return false;
                }
                var _id = $('#<%=hidCustomer.ClientID%>').val();

                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'CardDetail.aspx/getCusInfo',
                    dataType: "json",
                    data: "{ 'cusId': '" + _id + "'}",
                    success: function (result) {
                        var obj = JSON.parse(result.d);
                        $.each(obj, function (i, item) {
                            //alert(v.CustomerName);
                            $('#<%=txtDescription.ClientID%>').val($.trim(item.Description));
                            $('#<%=txtCustomerName.ClientID%>').val($.trim(item.CustomerName));
                            if (item.Inactive == "True") {
                                $('#<%=chbInactive.ClientID%>').prop('checked', true);
                            } else {
                                $('#<%=chbInactive.ClientID%>').prop('checked', false);
                            }

                            $('#<%=txtCustomerCode.ClientID%>').val($.trim(item.CustomerCode));
                            $('#<%=txtAddress.ClientID%>').val($.trim(item.Address));
                            $('#<%=txtIDNumber.ClientID%>').val($.trim(item.IDNumber));
                            $('#<%=txtMobile.ClientID%>').val($.trim(item.Mobile));
                            $('#<%=cbCustomerGroup.ClientID%>').val(item.CustomerGroupID);
                            if (item.EnableAccount == "True") {
                                $('#<%=chbEnableAccount.ClientID%>').prop('checked', true);
                            } else {
                                $('#<%=chbEnableAccount.ClientID%>').prop('checked', false);
                            }
                            $('#<%=txtAccount.ClientID%>').val($.trim(item.Account));
                            $('#<%=txtPassword.ClientID%>').val(item.Password);
                            $('#<%=id_avatar.ClientID%>').val(item.Avatar);

                        });
                    },
                    Error: function () {
                        alert('error');
                    }

                });

                if ($("#nav-customer-input").val().length == 0) {
                    event.preventDefault();
                    return false;
                }
            }
        });
        $("#nav-customer-input").autocomplete({
            source: function (request, response) {
                var itemnamecodes = new Array();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'Card.aspx/getCustomerByAutocomplete',
                    dataType: "json",
                    data: "{ 'name': '" + request.term + "'}",
                    success: function (result) {
                        //debugger;
                        itemnamecodes[0] = { label: 'Xóa khách hàng', Id: '0' };
                        for (var i = 0; i < result.d.length; i++) {
                            //alert(result.d[i].Text + '-' + result.d[i].Value);
                            itemnamecodes[i + 1] = { label: result.d[i].Text, Id: result.d[i].Value };
                        }
                        response(itemnamecodes);
                    },
                    error: function (result) {
                        //debugger;
                        alert(result);
                    }
                });
            },
            minLength: 1,
            select: function (event, ui) {
                //debugger;
                $('.plcCustomerId').find('input[type=hidden]').val(ui.item.Id);
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            }
        });

        $(".id_page_active_card_mgr").addClass("active open");
        $(".id_page_card").addClass("active");



        $('#fileAvatar').change(function () {
            uploadFile(this.files[0]);
        });


        if ('<%=this.ViewState["First"].ToString()%>' == "0") {

                $('.nav-tabs > li.active').removeClass('active');
                $('#id_li_customer').addClass('active');
                $('#id_card_info').removeClass('in active');
                $('#id_card_customer').addClass('in active');


                if (!ace.vars['touch']) {
                    $('.chosen-select').chosen({ allow_single_deselect: true });
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
                }
            } else if ('<%=ViewState["First"].ToString()%>' == "3") {
                $('.nav-tabs > li.active').removeClass('active');
                $('#id_li_SubCard').addClass('active');
                $('.tab-content .tab-pane').removeClass('in active');
                $('#id_Card_SubCard').addClass('in active');
            }

            $("a[href='#id_card_customer']").on('shown.bs.tab', function (e) {
                //alert("hiii");

                if (!ace.vars['touch']) {
                    $('.chosen-select').chosen({ allow_single_deselect: true });
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
                }

            });

            $('.search-choice-close').click(function () {
                alert("hi");
            });

            $("#btnUpdateRegisterDay").click(function () {
                $.ajax({
                    type: "POST",
                    url: "CardDetail.aspx/ChangeRegisterDate",
                    data: "{'cardnumber':'" + document.getElementById('<%=txtCardNumber.ClientID%>').value + "', 'registerdate':'" + document.getElementById('<%=dtpRegisterDate.ClientID%>').value + "', 'userid':'" + '<%=this.ViewState["UserID"].ToString()%>' + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function () {

                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            });

            $("#btnUpdateReleaseDay").click(function () {
                $.ajax({
                    type: "POST",
                    url: "CardDetail.aspx/ChangeReleaseDate",
                    data: "{'cardnumber':'" + document.getElementById('<%=txtCardNumber.ClientID%>').value + "', 'releasedate':'" + $('#nav-customer-input').val() + "', 'userid':'" + '<%=this.ViewState["UserID"].ToString()%>' + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function () {

                    },
                    failure: function (response) {
                        alert(response.d);
                    }
                });
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
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" runat="Server">
    <ul class="breadcrumb">
        <li><i class="ace-icon fa fa-home home-icon"></i><a href="#">Trang chủ</a> </li>
        <li class="active">Thẻ</li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" runat="Server">
    <div class="page-header">
        <h1 id="id_carddetail" runat="server">Thêm thẻ
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" runat="Server">
    <div class="col-xs-12">
        <%--Hien thi thong bao tai day--%>
        <div class="alert alert-warning" id="div_alert" runat="server">
            <button type="button" class="close" data-dismiss="alert">
                <i class="ace-icon fa fa-times"></i>
            </button>
            <i class="ace-icon fa fa-exclamation-triangle"></i><span id="id_alert" runat="server"></span>
            <br />
        </div>
    </div>
    <div class="col-xs-12">
        <div id="id_carddetailmodal">
        </div>
    </div>
    <div class="col-xs-12">
        <form id="frm_CardDetail" class="form-horizontal" runat="server">
            <!-- #section:elements.tab -->
            <div class="tabbable">
                <ul class="nav nav-tabs" id="myTab">
                    <li class="active">
                        <a data-toggle="tab" href="#id_card_info">
                            <i class="fa fa-credit-card"></i>Thông tin thẻ 
                        </a>
                    </li>
                    <li id="id_li_customer">
                        <a data-toggle="tab" href="#id_card_customer">
                            <i class="fa fa-user"></i>Khách hàng
                        </a>
                    </li>
                    <li id="id_li_SubCard">
                        <a data-toggle="tab" href="#id_Card_SubCard">
                            <i class="fa fa-credit-card-alt"></i>Thẻ phụ
                        </a>
                    </li>
                </ul>
                <div class="tab-content">
                    <div id="id_card_info" class="tab-pane fade in active">
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
                                <div class="input-group ">
                                    <input class="form-control datePicker" id="dtpRegisterDate"
                                        runat="server" type="text" />
                                    <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                                </div>
                                <asp:HiddenField ID="hidRegisterDate" runat="server" />
                            </div>
                            <div class="col-sm-3">
                                <div id="boxRegisterDate" runat="server">
                                    <%--<a class="btn btn-default btn-sm" id="btnUpdateRegisterDay">
                                    <i class="ace-icon fa fa-refresh bigger-110"></i>
						            <span class="">Cập nhập</span>
                                </a>--%>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                Ngày phát thẻ
                            </label>
                            <div class="col-sm-4">
                                <div class="input-group ">
                                    <input class="form-control datePicker" id="dtpReleaseDate"
                                        runat="server" type="text" />
                                    <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                                </div>
                                <asp:HiddenField ID="hidReleaseDate" runat="server" />
                            </div>
                            <div class="col-sm-3">
                                <div id="boxReleaseDate" runat="server">
                                    <%--<a class="btn btn-default btn-sm" id="btnUpdateReleaseDay">
                                    <i class="ace-icon fa fa-refresh bigger-110"></i>
						            <span class="">Cập nhập</span>
                                </a>--%>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                Ngày hết hạn
                            </label>
                            <div class="col-sm-4">
                                <div class="input-group ">
                                    <input class="form-control datePicker" id="dtpExpireDate"
                                        runat="server" type="text" />
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
                    <div id="id_card_customer" class="tab-pane fade">
                        <div class="form-group">
                            <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                Khách hàng
                            </label>
                            <div class="col-sm-5 plcCustomerId" id="BoxCustomer">
                                <%--<asp:DropDownList ID="cbCustomer" class="chosen-select form-control disabled" AutoPostBack="True"
                                data-placeholder="Chọn khách hàng..." OnSelectedIndexChanged="Selection_ChangeCustomer"
                                runat="server">
                            </asp:DropDownList>
                            <asp:HiddenField id="hidCustomer" runat="server"/>--%>
                                <input type="text" placeholder="Nhập tên khách hàng" class="nav-search-input form-control" id="nav-customer-input" value="<%=ViewState["cusName"] %>" />
                                <input type="hidden" name="hidCustomer" id="hidCustomer" runat="server" value=""/>
                            </div>
                            <%-- <div class="col-sm-3">
                            <a href="javascript:void(0)" id="btnEditBoxCustomer" class="btn btn-primary btn-sm"><i class="ace-icon fa fa-pencil"> Sửa</i></a>
                            <a href="javascript:void(0)" id="btnSaveBoxCustomer" class="btn btn-success btn-sm disabled"><i class="ace-icon fa fa-save"> Lưu</i></a>
                        </div>--%>
                        </div>
                        <div id="boxNewDateTime" runat="server">
                            <%--<div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="dtpNewRegisterDate">
                            Ngày đăng ký
                        </label>
                        <div class="col-sm-5">
                        <div class="input-group ">
                                <input class="form-control date-picker input-append date input-date" id="dtpNewRegisterDate"
                                    runat="server" type="text" data-date-format="dd/mm/yyyy" />
                                <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                            </div>
                        </div>
                    </div>--%>
                            <%--<div class="form-group">
                        <label class="col-sm-3 control-label no-padding-right" for="dtpNewReleaseDate">
                            Ngày phát
                        </label>
                        <div class="col-sm-5">
                            <div class="input-group ">
                                <input class="form-control date-picker input-append date input-date" id="dtpNewReleaseDate"
                                    runat="server" type="text" data-date-format="dd/mm/yyyy" />
                                <span class="input-group-addon"><i class="fa fa-calendar bigger-110"></i></span>
                            </div>
                        </div>
                    </div>--%>
                        </div>
                        <h4 class="header smaller lighter blue">Thông tin chi tiết</h4>
                        <div id="id_customer_info">
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Mã khách hàng
                                </label>
                                <div class="col-sm-5">
                                    <asp:TextBox ID="txtCustomerCode" runat="server" class="form-control" placeholder="Mã khách hàng" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Họ tên
                                </label>
                                <div class="col-sm-5">
                                    <asp:TextBox ID="txtCustomerName" runat="server" class="form-control" placeholder="Họ tên" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Nhóm khách hàng
                                </label>
                                <div class="col-sm-5">
                                    <select class="form-control" id="cbCustomerGroup" runat="server">
                                    </select>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Căn hộ
                                </label>
                                <div class="col-sm-5">
                                    <select class="form-control chosen-select" id="cbCompartment" runat="server">
                                    </select>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Địa chỉ</label>
                                <div class="col-sm-9">
                                    <asp:TextBox ID="txtAddress" runat="server" class="form-control" placeholder="" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Chứng minh thư
                                </label>
                                <div class="col-sm-3">
                                    <asp:TextBox ID="txtIDNumber" runat="server" class="form-control" placeholder="" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Số điện thoại
                                </label>
                                <div class="col-sm-3">
                                    <asp:TextBox ID="txtMobile" runat="server" class="form-control" placeholder="" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Miêu tả
                                </label>
                                <div class="col-sm-9">
                                    <asp:TextBox TextMode="MultiLine" Rows="5" ID="txtDescription" runat="server" class="form-control"
                                        placeholder="" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-5">
                                    <label class="inline">
                                        <input type="checkbox" class="ace" id="chbEnableAccount" runat="server" />
                                        <span class="lbl">Cho phép đăng nhập</span>
                                    </label>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                                    Tên đăng nhập
                                </label>
                                <div class="col-sm-5">
                                    <asp:TextBox ID="txtAccount" runat="server" class="form-control" placeholder="Tên đăng nhập" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-2">
                                    Mật khẩu
                                </label>
                                <div class="col-sm-5">
                                    <asp:TextBox ID="txtPassword" TextMode="password" runat="server" class="form-control"
                                        placeholder="Mật khẩu" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-2">
                                    Nhập lại mật khẩu
                                </label>
                                <div class="col-sm-5">
                                    <asp:TextBox ID="txtRePassword" TextMode="password" runat="server" class="form-control"
                                        placeholder="Nhập lại mật khẩu" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="col-sm-3 control-label no-padding-right" for="form-field-2">
                                    Ảnh đăng ký
                                </label>
                                <input id="fileAvatar" type="file" size="40" accept="image/*" style="display: none" />
                                <asp:HiddenField ID="id_avatar" runat="server" />
                                <div class="col-sm-5">
                                    <ul class="ace-thumbnails clearfix">
                                        <li><a href="../assets/avatars/noPhotoAvailable.jpg" data-rel="colorbox" id="picAvatar"
                                            runat="server">
                                            <img style="max-width: 250px; max-height: 250px;" class="img-responsive" alt="Ảnh đăng ký"
                                                src="../assets/avatars/noPhotoAvailable.jpg" id="preViewAvatar" runat="server" />
                                            <div class="text">
                                                <div class="inner">
                                                    Ảnh đăng ký
                                                </div>
                                            </div>
                                        </a>
                                            <div class="tools tools-bottom">
                                                <a href="#" onclick="openFileOption();"><i class="ace-icon fa fa-upload"></i></a>
                                                <a href="#" onclick="cancelImg();"><i class="ace-icon fa fa-times red"></i></a><a
                                                    href="#" onclick="deleteImg();"><i class="ace-icon fa fa-trash-o"></i></a>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-5">
                                    <label class="inline">
                                        <input type="checkbox" class="ace" id="chbInactive" runat="server" />
                                        <span class="lbl">Ngừng kích hoạt</span>
                                    </label>

                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="id_Card_SubCard" class="tab-pane fade">
                        <div class="form-group">
                            <label class="col-sm-1 control-label no-padding-right" for="form-field-1">
                                Mã thẻ phụ
                            </label>
                            <div class="col-sm-3">
                                <input type="text" id="plcInputCardNumber" placeholder="Mã thẻ phụ" class="nav-search-input form-control"/>
                                <input type="hidden" name="txtSubCardCode" id="txtSubCardCode" runat="server" value=""/>
                                <p style="color: red; font-style: italic; margin-top: 5px;">
                                    <%=ViewState["errSubCard"]  %>
                                </p>
                            </div>
                            <div class="col-md-2">
                                <asp:LinkButton runat="server" OnClick="AddSubCard" ID="btnAddNewsCustomer" class="btn btn-success btn-sm">Thêm</asp:LinkButton>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-5">
                                <table class="table table-striped table-bordered table-hover">
                                    <tr>
                                        <th width="30">STT</th>
                                        <th>Mã thẻ phụ</th>
                                        <th>CardNo</th>
                                        <th>Thao tác</th>
                                    </tr>
                                    <asp:Repeater ID="rptListSubCard" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td align="center"><%#(Container.ItemIndex+1).ToString()%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                                <td>
                                                    <a name="btnDeleteSub" idata="<%#DataBinder.Eval(Container.DataItem, "Id")%>" class="btn btn-xs btn-danger delete" title="Xóa">
                                                        <i class="ace-icon fa fa-trash-o bigger-120"></i>
                                                    </a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </div>
                        </div>
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
                <button class="btn" type="button" onclick="window.location.href='Card.aspx'">
                    <i class="ace-icon fa fa-undo bigger-110"></i>Quay lại
                </button>
                </div>
            </div>
        </form>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ActiveCard_v2.aspx.cs" Inherits="QLXuatAn_ActiveCard_v2" %>

<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>        
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <!-- JS Global Compulsory -->
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <%--<script src="../assets/js/google-api/jquery-ui.min.js" type="text/javascript"></script>--%>
    <%--<link href="../assets/css/google-api/jquery-ui.css" rel="Stylesheet" type="text/css" />--%>

    <style type="text/css">
        .loading {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            width: 200px;
            height: 150px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }

        .tblCusFilter .itempading {
            padding: 6px;
            margin-bottom: 5px;
        }

        .tblCusFilter2 td {
        }

        .divOver {
            overflow: auto;
            max-height: 500px;
            height: 250px;
            border: 1px solid #ccc;
        }

        .aceMap {
            position: inherit !important;
        }

        .modelPaging .pagination > li > a, .modelPaging .pagination > .active > span, .modelPaging .pagination > .disabled > span {
            padding: 1px 5px;
            font-size: 12px;
        }

        .modelPaging .pagination {
            margin: 0 !important;
        }

            .modelPaging .pagination > li {
                float: left;
            }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=txtKeyWord.ClientID %>').keypress(function (event) {
                var keycode = event.keyCode || event.which;
                //alert(keycode)
                if (keycode === 13) {
                    $('#btnRefresh').click();
                    return false;
                }
            });
            if (!ace.vars['old_ie']) $('#<%=dtpExpireDate.ClientID%>').datetimepicker({
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

            $("#nav-customer-input").focus(function () {
                this.select();
            });
            $("#nav-customer-input").change(function () {
                if ($(this).val() == '') {
                    $('.plcCustomerId').find('input[type=hidden]').val('');
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
                    $('.plcCustomerId').find('input[type=hidden]').val(ui.item.Id);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });

            //autocomplete
            $(".autosuggest").autocomplete({
                //            source: availableTags
                source: function (request, response) {
                    //                 alert("hi");
                    $.ajax({
                        url: "ActiveCard_v2.aspx/GetFees",
                        data: "{ 'prefix': '" + request.term + "'}",
                        dataType: "json",
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        success: function (data) {
                            response($.map(data.d, function (item) {
                                return {
                                    value: item
                                }
                            }))
                        },
                        error: function (response) {
                            alert(response.responseText);
                        },
                        failure: function (response) {
                            alert(response.responseText);
                        }
                    });
                }
            });

            $(".id_page_active_card").addClass("active");
            $(".id_page_active_card").addClass("open");
            $(".id_page_active_new").addClass("active");


            $('body').on('click', '#btnRefresh', function (e) {
                window.location.href = 'ActiveCard_v2.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                    '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerID=' + $('.plcCustomerId').find('input[type=hidden]').val() +
                    '&cusName=' + $('#nav-customer-input').val() +
                    '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value;
            });


            $('#chkAll').click(function () {

                $('#tblListCard tbody input.chkItem').prop('checked', $(this).is(':checked'));

                var listCheck = '';
                $('#tblListCard tbody input.chkItem:checked').each(function () {
                    listCheck += $(this).val() + ';';
                });

                if (listCheck !== '') {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'ActiveCard_v2.aspx/addCardToChoice',
                        dataType: "json",
                        data: "{ 'listCardId': '" + listCheck + "'}",
                        success: function (result) {
                            //debugger;
                            if (result != '') {
                                var obj = JSON.parse(result.d);
                                var countRow = $('#tblListCardChoice tbody tr').size();
                                var listTR = '';
                                $.each(obj, function (i, item) {
                                    if (item.CardNumber != undefined && item.CardNumber != null && item.CardNumber != '') {
                                        countRow = countRow + 1;
                                        listTR += '<tr>';
                                        listTR += '<td align="center">' + countRow + '</td>';
                                        listTR += '<td>' + item.CardNumber + '</td>';
                                        listTR += '<td>' + item.CardNo + '</td>';
                                        listTR += '<td>' + item.Plate1 + '</td>';
                                        listTR += '<td>';
                                        listTR += '<span class="">' + item.ExpireDate + '</span>';
                                        listTR += '</td>';
                                        listTR += '<td>';
                                        listTR += item.CustomerName;
                                        listTR += '</td>';
                                        listTR += '<td align="center">';
                                        listTR += '<a href=\"javascript:void(0)\" onclick=\"deleteCardChoice(\'' + item.CardNumber + '\',false)\">Xóa</a>';
                                        listTR += '</td>';
                                        listTR += '</tr>';
                                    }
                                });

                                $('#tblListCardChoice tbody').append(listTR);
                                if (listTR != '') {
                                    $('#<%=Button2.ClientID%>').val('Gia hạn ' + countRow + ' thẻ');
                                    $('#plcTotalCardChoice').text(countRow);
                                } else {
                                    if (countRow > 0) {
                                        $('#<%=Button2.ClientID%>').val('Gia hạn ' + countRow + ' thẻ');
                                        $('#plcTotalCardChoice').text(countRow);
                                    } else {
                                        $('#<%=Button2.ClientID%>').val('Chọn ít nhất 1 thẻ để gia hạn');
                                    }

                                }
                            }

                        },
                        Error: function () {
                            alert('error');
                        }

                    });
                }
            });

            //Add Card to choice list

            $('#tblListCard tbody').on('click', 'input.chkItem', function () {
                var _v = $(this).val() + ';';
                //alert(_v);
                //debugger;
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ActiveCard_v2.aspx/addCardToChoice',
                    dataType: "json",
                    data: "{ 'listCardId': '" + _v + "'}",
                    success: function (result) {
                        //debugger;
                        if (result != '') {
                            var obj = JSON.parse(result.d);
                            var countRow = $('#tblListCardChoice tbody tr').size();
                            var listTR = '';
                            $.each(obj, function (i, item) {
                                if (item.CardNumber != undefined && item.CardNumber != null && item.CardNumber != '') {
                                    countRow = countRow + 1;
                                    listTR += '<tr>';
                                    listTR += '<td align="center">' + countRow + '</td>';
                                    listTR += '<td>' + item.CardNumber + '</td>';
                                    listTR += '<td>' + item.CardNo + '</td>';
                                    listTR += '<td>' + item.Plate1 + '</td>';
                                    listTR += '<td>';
                                    listTR += item.ExpireDateHtml;
                                    listTR += '</td>';
                                    listTR += '<td>';
                                    listTR += item.CustomerName;
                                    listTR += '</td>';
                                    listTR += '<td align="center">';
                                    listTR += '<a href=\"javascript:void(0)\" onclick=\"deleteCardChoice(\'' + item.CardNumber + '\',false)\">Xóa</a>';
                                    listTR += '</td>';
                                    listTR += '</tr>';
                                }

                            });

                            $('#tblListCardChoice tbody').append(listTR);
                            if (listTR != '') {
                                $('#<%=Button2.ClientID%>').val('Gia hạn ' + countRow + ' thẻ');
                                $('#plcTotalCardChoice').text(countRow);
                            } else {
                                if (countRow > 0) {
                                    $('#<%=Button2.ClientID%>').val('Gia hạn ' + countRow + ' thẻ');
                                    $('#plcTotalCardChoice').text(countRow);
                                } else {
                                    $('#<%=Button2.ClientID%>').val('Chọn ít nhất 1 thẻ để gia hạn');
                                }
                            }
                        }
                    },
                    Error: function () {
                        alert('error');
                    }

                });
            });


            //Seảch autôcmplete CardActive all
            $("input[name=txtSearchActiveAll]").keyup(function () {
                //alert("OK");
                var _txt = $(this).val();
                //if (_txt == undefined || _txt === '') {
                //    return false;
                //}
                var _cardGroupId = $('#<%=cbCardGroup.ClientID%>').val();
                var _customerGroupId = $('#<%=cbCustomerGroup.ClientID%>').val();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ActiveCard_v2.aspx/getSearchCardActiveAutocomplete',
                    dataType: "json",
                    data: "{ 'KeyWord': '" + _txt + "','CardGroupID':'" + _cardGroupId + "','CustomerGroupID':'" + _customerGroupId + "'}",
                    success: function (result) {
                        //debugger;
                        if (result != '') {
                            var obj = JSON.parse(result.d);
                            var countRow = 0;
                            var listTR = '';
                            $.each(obj, function (i, item) {
                                if (item.CardNumber != undefined && item.CardNumber != null && item.CardNumber != '') {
                                    countRow = countRow + 1;
                                    listTR += '<tr>';
                                    listTR += '<td align="center">' + countRow + '</td>';
                                    listTR += '<td>' + item.CardNumber + '</td>';
                                    listTR += '<td>' + item.CardNo + '</td>';
                                    listTR += '<td>' + item.Plate1 + '</td>';
                                    listTR += '<td>' + decodeURI(item.ExpireDateHtml);
                                    //listTR += '<span class="">' + item.ExpireDate + '</span>';
                                    listTR += '</td>';
                                    listTR += '<td>';
                                    listTR += item.CustomerName;
                                    listTR += '</td>';
                                    listTR += '<td align="center">';
                                    listTR += '<label class="inline">';
                                    listTR += '<input type="checkbox" id="chkItem' + countRow + '" class="ace aceMap chkItem" value="' + item.CardNumber + ',' + item.CardNo + ',' + item.Plate1 + ',' + item.ExpireDate + ',' + item.CustomerName + '" />';
                                    listTR += '<span class="lbl"></span>';
                                    listTR += '</label>';
                                    listTR += '</td>';
                                    listTR += '</tr>';
                                }

                            });

                            $('#tblListCard tbody').html(listTR);

                        }
                    },
                    Error: function () {
                        alert('error');
                    }

                });

            });

            $('#<%=btnSaveAll.ClientID%>').click(function () {
                if (confirm("Bạn muốn " + $('#<%=btnSaveAll.ClientID%>').val() + "?")) {
                    return true;
                }
                return false;
            });

        });

        function deleteCardChoice(_id, chk) {
            //chk=true: delete all
            if (confirm('bạn có chắc chắn muốn xóa dữ liệu đã chọn')) {
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'ActiveCard_v2.aspx/deleteCardChoice',
                    dataType: "json",
                    data: "{ 'id': '" + _id.trim() + "','chk': '" + chk + "'}",
                    success: function (result) {
                        //debugger;
                        if (result != '') {
                            var obj = JSON.parse(result.d);
                            var countRow = $('#tblListCardChoice tbody tr').size();
                            var listTR = '';
                            $.each(obj, function (i, item) {
                                if (item.CardNumber != undefined && item.CardNumber != null && item.CardNumber !== '') {
                                    listTR += '<tr>';
                                    listTR += '<td align="center">' + (parseInt(countRow) + 1) + '</td>';
                                    listTR += '<td>' + item.CardNumber + '</td>';
                                    listTR += '<td>' + item.CardNo + '</td>';
                                    listTR += '<td>' + item.Plate1 + '</td>';
                                    listTR += '<td>';
                                    listTR += '<span class="">' + item.ExpireDate + '</span>';
                                    listTR += '</td>';
                                    listTR += '<td>';
                                    listTR += item.CustomerName;
                                    listTR += '</td>';
                                    listTR += '<td align="center">';
                                    listTR += '<a href=\"javascript:void(0)\" onclick=\"deleteCardChoice(\'' + item.CardNumber + '\',false)\">Xóa</a>';
                                    listTR += '</td>';
                                    listTR += '</tr>';
                                }
                            });

                            $('#tblListCardChoice tbody').html(listTR);
                            if (listTR != '') {
                                $('#<%=Button2.ClientID%>').val('Gia hạn ' + obj.length + ' thẻ');
                                $('#plcTotalCardChoice').text(obj.length);
                            } else {
                                $('#<%=Button2.ClientID%>').val('Chọn ít nhất 1 thẻ để gia hạn');
                            }

                        }
                    },
                    Error: function () {
                        alert('error');
                    }

                });
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" runat="Server">
    <ul class="breadcrumb">
        <li>
            <i class="ace-icon fa fa-home home-icon"></i>
            <a href="#">Trang chủ</a>
        </li>
        <li class="active">Gia hạn thẻ</li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" runat="Server">
    <div class="page-header">
        <h1 id="id_cardlist" runat="server">Danh sách thẻ
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" runat="Server">

    <div class="col-xs-12">

        <form id="frm_CardDetail" class="form-horizontal" runat="server" method="post">
        <% if (ViewState["alertInfo"] != null && (string)ViewState["alertInfo"] != "")
           {%>
            <div class="alert alert-block alert-success">
                <i class="ace-icon fa fa-bullhorn"></i> <%=ViewState["alertInfo"] %>
            </div>
        <%} %>
            <div class="clearfix">
                <div class="col-xs-12">
                    <div class="row">
                        <div class="widget-body">
                            <div class="widget-main">
                                <div class="tblCusFilter">
                                    <div class="col-md-2 col-xs-12 itempading">
                                        <select class="form-control" id="cbCardGroup" runat="server"></select>
                                    </div>
                                    <div class="col-md-2 col-xs-12 itempading">
                                        <select class="form-control" id="cbCustomerGroup" runat="server"></select>
                                    </div>
                                    <div class="col-md-2 col-xs-12 itempading">
                                        <div class="plcCustomerId">
                                            <input type="text" placeholder="Nhập tên hoặc mã KH" class="nav-search-input form-control" id="nav-customer-input" value="<%=ViewState["cusName"] %>" />
                                            <input type="hidden" name="cbCustomer" id="cbCustomer" runat="server" value="" />
                                        </div>
                                    </div>
                                    <div class="col-md-2 col-xs-12 itempading">
                                        <input type="text" id="txtKeyWord" placeholder="Từ khóa tìm kiếm" class="form-control" runat="server" />
                                    </div>
                                    <div class="col-md-4 col-xs-12 itempading">
                                        <button type="button" class="btn btn-white btn-default" id="btnRefresh">
                                            <i class="fa fa-refresh"></i>
                                            Nạp lại
                                        </button>
                                        <a class="btn btn-white btn-default export" data-toggle="modal" data-target="#ShowBoxUploadExcelActiveCard">
                                            <i class="fa fa-file-excel-o pink"></i>
                                            <span class="">Import</span>
                                        </a>
                                        <!-- Modal -->
                                        <div class="modal fade" id="ShowBoxUploadExcelActiveCard" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                                            <div class="modal-dialog" role="document">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <h5 class="modal-title" id="exampleModalLabel">Chọn file Upload</h5>
                                                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                            <span aria-hidden="true">&times;</span>
                                                        </button>
                                                    </div>
                                                    <div class="modal-body">
                                                        <p style="padding-left: 10px;">
                                                <a href="/QLXuatAn/files/Gia-han-the.xlsx">Tải file mẫu Import <i class="fa fa-download" aria-hidden="true"></i></a>
                                            </p>
                                                        <div style="padding: 10px;">
                                                            <asp:FileUpload ID="FileUpload1" runat="server" />
                                                        </div>
                                                    </div>
                                                    <div class="modal-footer">
                                                        <asp:LinkButton ID="lnkImportEx" Text="Cập nhập" runat="server" class="btn btn-info" OnClick="lnkImportEx_Click"></asp:LinkButton>
                                                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Thoát</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="boxMore">
                                    <div class="col-md-3 col-xs-12 itempading">
                                        <div class="form-inline">
                                            <div class="form-group">
                                                <label>Mức phí: </label>
                                                <asp:TextBox ID="txtFee" runat="server" class="form-control autosuggest money" placeholder="" Text="0" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-4 col-xs-12 itempading">
                                        <div class="form-inline">
                                            <div class="form-group">
                                                <label>Hạn SD: </label>
                                                <div class="input-group">
                                                    <input class="form-control" id="dtpExpireDate" runat="server" type="text" />
                                                    <span class="input-group-addon">
                                                        <i class="fa fa-calendar bigger-110"></i>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-3 col-xs-12 itempading">
                                        <div class="form-inline">
                                            <div class="form-group">
                                                <input type="checkbox" class="ace" id="chbEnableMinusActive" runat="server" />
                                                <label for="<%=chbEnableMinusActive.ClientID %>" class="lbl">Cho phép gia hạn âm ngày</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="clearfix">
                <div class="hr hr-16 hr-dotted"></div>
                <div class="clearfix">
                    <div class="col-md-6 col-xs-12">

                        <div style="font-size: 14px; height: 35px; overflow: hidden; margin: 3px 0;" class="clearfix">
                            <input type="text" class="form-control" name="txtSearchActiveAll" placeholder="Số thẻ * Mã thẻ * Biển số * Thời hạn * Tên KH" value="" />
                        </div>
                        <div class="divOver">
                            <table class="table table-striped table-bordered table-hover" id="tblListCard">
                                <thead>
                                    <tr style="background-image: linear-gradient(to bottom, #ffffff 0%, #efefef 100%);">
                                        <th style="width: 25px;">STT</th>
                                        <th>Mã thẻ</th>
                                        <th>Số thẻ</th>
                                        <th>Biển số</th>
                                        <th>Thời hạn cũ</th>
                                        <th>Tên KH</th>
                                        <th style="width: 45px; text-align: center;">
                                            <label class="inline" style="margin-bottom: 0px;">
                                                <input type="checkbox" id="chkAll" class="ace" value="0" />
                                                <span class="lbl"></span>
                                            </label>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptListCard" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td align="center"><%#DataBinder.Eval(Container.DataItem, "RowNumber")%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "Plate1")%></td>
                                                <td>
                                                    <span class="<%#this.classDateExprice(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpireDate"))) %>"><%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpireDate")).ToString("dd/MM/yyyy") %></span>
                                                </td>
                                                <td>
                                                    <%#((System.Data.DataRowView)Container.DataItem)["CustomerName"].ToString()%>
                                                </td>
                                                <td align="center">
                                                    <label class="inline">
                                                        <input type="checkbox" id="chkItem<%#(Container.ItemIndex+1) %>" class="ace aceMap chkItem" value="<%#DataBinder.Eval(Container.DataItem, "CardNumber")%>,<%#DataBinder.Eval(Container.DataItem, "CardNo")%>,<%#DataBinder.Eval(Container.DataItem, "Plate1")%>,<%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpireDate")).ToString("dd/MM/yyyy") %>,<%#((System.Data.DataRowView)Container.DataItem)["CustomerName"].ToString()%>" />
                                                        <span class="lbl"></span>
                                                    </label>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                        <div style="margin: 5px 0;" class="clearfix">
                            <% if (Convert.ToInt32(ViewState["totalCard"]) <= 0)
                               { %>
                            Không có thẻ nào hết hạn hôm nay
                        <% }
                               else
                               { %>
                            <div class="modelPaging clearfix">
                                <cc1:CollectionPager ID="pager" runat="server" PageSize="20" ShowLabel="false" MaxPages="10000"></cc1:CollectionPager>
                            </div>
                            <% } %>
                        </div>

                        <div class="clearfix form-actions" style="margin-top: 0; padding: 0;">
                            <asp:Button ID="btnSaveAll" runat="server" class="btn btn-info ace-icon fa fa-check bigger-160" Text="Gia hạn thẻ" OnClick="btnSaveAll_Click" />
                        </div>
                    </div>
                    <div class="col-md-6 col-xs-12">
                        <div style="font-size: 14px; height: 35px; overflow: hidden; margin: 3px 0;">
                            Thẻ đã chọn <span id="plcTotalCardChoice"><%=ViewState["totalCardChoice"] %></span>
                        </div>
                        <div class="divOver">
                            <table class="table table-striped table-bordered table-hover" id="tblListCardChoice">
                                <thead>
                                    <tr style="background-image: linear-gradient(to bottom, #ffffff 0%, #fdf9d8 100%); height: 38px;">
                                        <th style="width: 25px;">STT</th>
                                        <th>Mã thẻ</th>
                                        <th>Số thẻ</th>
                                        <th>Biển số</th>
                                        <th>Thời hạn cũ</th>
                                        <th>Tên KH</th>
                                        <th align="center">
                                            <a href="javascript:void(0)" title="Xóa tất cả" onclick="deleteCardChoice('0',true)" style="color: #a10a0a;">
                                                <span style="color: #a10a0a;">Xóa tất</span>
                                            </a>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptCardChoice" runat="server">

                                        <ItemTemplate>

                                            <tr>
                                                <td align="center"><%#(Container.ItemIndex+1).ToString()%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                                <td><%#DataBinder.Eval(Container.DataItem, "Plate1")%></td>
                                                <td>
                                                    <span class="<%#this.classDateExprice(Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpireDate"))) %>"><%#Convert.ToDateTime(DataBinder.Eval(Container.DataItem, "ExpireDate")).ToString("dd/MM/yyyy") %></span>
                                                </td>
                                                <td>
                                                    <%#((System.Data.DataRowView)Container.DataItem)["CustomerName"].ToString()%>
                                                </td>
                                                <td align="center">
                                                    <a href="javascript:void(0)" onclick="deleteCardChoice('<%#DataBinder.Eval(Container.DataItem, "CardNumber")%>',false)">Xóa</a>
                                                </td>
                                            </tr>


                                        </ItemTemplate>
                                    </asp:Repeater>

                                </tbody>
                            </table>

                        </div>

                        <div class="clearfix form-actions" style="margin-top: 0; padding: 52px 0 0 0">
                            <asp:Button ID="Button2" runat="server" class="btn btn-warning ace-icon fa fa-check bigger-160" OnClick="Button2_Click" />
                        </div>
                    </div>
                </div>
               
            </div>
        </form>
    </div>
    <!-- /.col -->
</asp:Content>


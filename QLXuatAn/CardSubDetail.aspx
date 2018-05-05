<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="CardSubDetail.aspx.cs" Inherits="QLXuatAn_CardSubDetail" %>

<%@ Register TagPrefix="cc1" Namespace="SiteUtils" Assembly="HNG.CollectionPager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../Scripts/jquery-1.4.1.min.js"></script>
    <script>
        jQuery(document).ready(function () {

            $("#plcCardNumber").keydown(function (event) {
                if (event.keyCode == 13) {
                    if ($("#plcCardNumber").val().length == 0) {
                        event.preventDefault();
                        return false;
                    }
                }
            });
            //Autocomplêt CardSub
            $("#plcCardNumber").autocomplete({
                source: function (request, response) {
                    var itemnamecodes = new Array();
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: 'CardSubDetail.aspx/getCardNumberByAutocomplete',
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
                    $('#<%=txtMainCard.ClientID%>').val(ui.item.Id);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" runat="Server">
    <ul class="breadcrumb">
        <li>
            <i class="ace-icon fa fa-home home-icon"></i>
            <a href="#">Trang chủ</a>
        </li>
        <li class="active">Thẻ phụ</li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" runat="Server">
    <div class="page-header">
        <h1 id="id_cardlist" runat="server">Thêm thẻ phụ
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" runat="Server">
    <form id="frm_CardSubDetail" class="form-horizontal" runat="server">
        <div class="form-group">
            <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                Thẻ chính
            </label>
            <div class="col-sm-4">
                <input type="text" id="plcCardNumber" placeholder="CardNumber | CardNo" class="nav-search-input form-control" value="<%=ViewState["MainCard"] %>"/>
                <input type="hidden" name="txtMainCard" id="txtMainCard" runat="server" value=""/>
            </div>
        </div>
        <div class="form-group">
            <label class="col-sm-3 control-label no-padding-right" for="form-field-1">
                Thẻ phụ 
            </label>
            <div class="col-sm-2">
                <input type="text" id="txtCardNumber" placeholder="" class="form-control" runat="server" />
                
                <br />
                <span class="red">
                    <%=ViewState["errSubCard"] %>
                </span>
            </div>
            <label class="col-sm-1 control-label no-padding-right">
                CardNo
            </label>
            <div class="col-sm-1">
                <input type="text" id="txtCardNo" placeholder="" class="form-control" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <label class="col-sm-3 control-label no-padding-right" for="form-field-2"></label>
            <div class="col-sm-8">
                <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-sm btn-info">
                            <i class="ace-icon fa fa-check bigger-110"></i>
						    <span class="bigger-110">Lưu lại</span>
                </asp:LinkButton>
                &nbsp; &nbsp; &nbsp;
                <button class="btn btn-sm" type="button" onclick="window.location.href='CardSub.aspx'">
                    <i class="ace-icon fa fa-undo bigger-110"></i>Quay lại
                </button>
            </div>
        </div>
    </form>
</asp:Content>


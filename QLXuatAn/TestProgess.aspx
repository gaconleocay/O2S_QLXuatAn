<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="TestProgess.aspx.cs" Inherits="QLXuatAn_TestProgess" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">
    .modal
    {
        position: fixed;
        top: 0;
        left: 0;
        background-color: black;
        z-index: 99;
        opacity: 0.8;
        filter: alpha(opacity=80);
        -moz-opacity: 0.8;
        min-height: 100%;
        width: 100%;
    }
    .loading
    {
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
</style>
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
<script type="text/javascript">
    function ShowProgress() {
        setTimeout(function () {
            var modal = $('<div />');
            modal.addClass("modal");
            $('body').append(modal);
            var loading = $(".loading");
            loading.show();
            var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
            var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
            loading.css({ top: top, left: left });
        }, 200);
    }
//    $('form').live("submit", function () {
//        ShowProgress();
//    });

    $('form').live("submit", function () {
        ShowProgress();
        });


</script>

<script type="text/javascript">
    $(document).ready(function () {
        $("#myButton").click(function () {
            alert("oK");
            $("#form1").submit();
        });
    });
</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
<form id="form1" runat="server">
 <input type="submit" style="display:none" />

Country: <asp:DropDownList ID="ddlCountries" runat="server">
    <asp:ListItem Text="All" Value="" />
    <asp:ListItem Text="USA" Value="USA" />
    <asp:ListItem Text="Brazil" Value="Brazil" />
    <asp:ListItem Text="France" Value="France" />
    <asp:ListItem Text="Germany" Value="Germany" />
</asp:DropDownList>
<label for="modify"><i class="ace-icon fa fa-check bigger-110"></i></label>
<asp:Button ID="btnSubmit1" runat="server" Text="Load Customers" CssClass="btn btn-info ace-icon fa fa-check bigger-110"
    OnClick="btnSubmit_Click"  />



<input type="button" value="Submit" id="myButton" />

<hr />



<asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="CustomerId" HeaderText="Customer Id" />
        <asp:BoundField DataField="CustomerName" HeaderText="Contact Name" />
        <asp:BoundField DataField="Address" HeaderText="City" />
    </Columns>
</asp:GridView>
<div class="loading" align="center">
    Processing. Please wait.<br />
    <br />
    <img src="loader.gif" alt="" />
</div>
    </form>
</asp:Content>


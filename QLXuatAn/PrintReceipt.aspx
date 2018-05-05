<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="PrintReceipt.aspx.cs" Inherits="QLXuatAn_PrintReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {



        });

        function printDiv(divID) {
            //Get the HTML of div
            var divElements = document.getElementById(divID).innerHTML;
            //Get the HTML of whole page
            var oldPage = document.body.innerHTML;

            //Reset the page's HTML with div's HTML only
            document.body.innerHTML =
              "<html><head><title></title></head><body>" +
              divElements + "</body>";

            //Print Page
            window.print();

            //Restore orignal HTML
            document.body.innerHTML = oldPage;


        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
 <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">In biên lai</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
<div class="page-header">
        <h1 id="id_carddetail" runat="server">
	        In biên lai
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
<div class="col-xs-12">
 <div class="row">
    <div class="widget-body">
		<div class="widget-main">
			
                <div class="form-group">
                    <div class="col-sm-2 no-padding-left"> 
                        <button type="button" id="btnAddCamera" onclick="printDiv('Print')" class="btn btn-info btn-sm">
                            <i class="fa fa-print"></i>
                            Print
                        </button>
                    </div>
                </div>
               
              
           
		</div>
	</div>
</div>
 <div class="hr hr-16 hr-dotted"></div>
     <div id="Print">
      
       <h3 class="center">BIÊN LAI THU TIỀN</h3>
       
        <div class="col-sm-offset-3">
            <p id="Company" runat="server"></p>
            <p id="Address" runat="server"></p>
            <p id="Tax" runat="server"></p>
        </div>
        <div class="col-sm-offset-2 bigger-120">
            <p id="Customer">Khách hàng: Nguyễn Cao Cường</p>
            <p id="CardNumber">Mã thẻ: 0908882222</p>
            <p id="Plate">Biển số: 29N1-29158</p>
            <p id="CustomerAddress">Địa chỉ: Hà Nội</p>
            <p id="OldExpire">Thời hạn cũ: 07/06/2016</p>
            <p id="NewExpire">Thời hạn mới: 08/08/2016</p>
            <p id="Money">Số tiền: 1,000,000</p>

        </div>
        <div class="space-20"></div>
        <div class="">
             <span class="col-sm-offset-2">Xác nhận KH</span>
            <span class="col-sm-offset-5">Người lập phiếu</span>
        </div>
    </div>
</div>
</asp:Content>


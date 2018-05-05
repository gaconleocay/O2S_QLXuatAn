<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="TestPrint2.aspx.cs" Inherits="QLXuatAn_TestPrint2" %>

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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">

<div class="col-xs-12">
 <div class="row">
    <div class="widget-body">
		<div class="widget-main">
			
                <div class="form-group">
                    <div class="col-sm-2 no-padding-left"> 
                        <button type="button" id="btnAddCamera" onclick="printDiv('Print')" class="btn btn-info btn-sm">
                            <i class="fa fa-plus"></i>
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
            <p id="Customer">KH</p>
            <p id="CardNumber">Ma the</p>
            <p id="Plate">Bien so</p>
            <p id="OldExpire">07/06/2016</p>
            <p id="NewExpire">08/08/2016</p>
            <p id="Money" class="money">1000000</p>

        </div>
        <div class="space-20"></div>
        <div class="">
             <span class="col-sm-offset-2">Xác nhận KH</span>
            <span class="col-sm-offset-5">Người lập phiếu</span>
        </div>
    </div>
</div>
</asp:Content>


<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="QLXuatAn_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {

            $(".id_page_default").addClass("active");

            // Thuc hien bao cao
            $('#btnRefresh').click(function (e) {
                e.preventDefault();
                window.location.href = 'Default.aspx?FrequencyDateType=' + document.getElementById("<%= cbFrequencyDateType.ClientID %>").selectedIndex;
            });

        });
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Bàn làm việc</li>
	</ul>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
    <div class="page-header">
        <h1>
	        Bàn làm việc
        </h1>       
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_PageContent" Runat="Server">
    
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

    <div class="col-sm-12">
        <div class="row">
            <div class="widget-body">
				<div class="widget-main">
					<form class="form-inline">
                        <div class="form-group">
                            <div class="col-sm-3 no-padding-left">      
                                <select class="form-control" id="cbFrequencyDateType" runat = "server">
                                    
                                </select> 
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-3 no-padding-left">    
                                <button type="button" class="btn btn-white btn-default" id="btnRefresh">
                                    <i class="fa fa-refresh"></i>
                                    Nạp lại
                                </button>
                            </div>
                        </div>
					</form>
				</div>
			</div>
        </div>              
    </div>

    <div class="col-xs-12">        
        <%--<div class="space-16"></div>--%>

	    <!-- #section:pages/dashboard.infobox -->
	    <div class="infobox infobox-green">
		    <div class="infobox-data">
			    <span class="infobox-data-number" id="id_dashboard1" runat="server">0</span>
			    <div class="infobox-content">xe vào</div>
		    </div>
	    </div>

	    <div class="infobox infobox-blue">
		    <div class="infobox-data">
			    <span class="infobox-data-number" id="id_dashboard2" runat="server">0</span>
			    <div class="infobox-content">xe ra</div>
		    </div>
	    </div>

	   <%-- <div class="infobox infobox-pink">
		    <div class="infobox-data">
			    <span class="infobox-data-number" id="id_dashboard3" runat="server">0</span>
			    <div class="infobox-content">thu tiền vé lượt</div>
		    </div>
	    </div>--%>

	   <%-- <div class="infobox infobox-red">
		    <div class="infobox-data">
			    <span class="infobox-data-number" id="id_dashboard4" runat="server">0</span>
			    <div class="infobox-content">lượt ấn nút</div>
		    </div>
	    </div>--%>

        <%--<div class="infobox infobox-orange2">
			<div class="infobox-data">
				<span class="infobox-data-number" id="id_dashboard5" runat="server">2167</span>
				<div class="infobox-content">thẻ đang hoạt động</div>
			</div>
		</div>--%>

        <div class="space-16"></div>

	<!-- /section:pages/dashboard.infobox.dark -->
    </div>

</asp:Content>


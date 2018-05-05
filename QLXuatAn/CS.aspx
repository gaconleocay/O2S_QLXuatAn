<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="CS.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<%--<link href="css/jquery.autocomplete.css" rel="stylesheet" type="text/css" />--%>
<%--<script src="assets/js/jquery-1.4.1.min.js" type="text/javascript"></script>--%>

<link rel="stylesheet" href="../assets/css/bootstrap.css" />
	<link rel="stylesheet" href="../components/font-awesome/css/font-awesome.css" />

	<!-- page specific plugin styles -->
    <link rel="stylesheet" href="../components/_mod/jquery-ui.custom/jquery-ui.custom.css" />
	<link rel="stylesheet" href="../components/chosen/chosen.css" />
	<link rel="stylesheet" href="../components/bootstrap-datepicker/dist/css/bootstrap-datepicker3.css" />
	<link rel="stylesheet" href="../components/bootstrap-timepicker/css/bootstrap-timepicker.css" />
	<link rel="stylesheet" href="../components/bootstrap-daterangepicker/daterangepicker.css" />
	<link rel="stylesheet" href="../components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css" />
	<link rel="stylesheet" href="../components/mjolnic-bootstrap-colorpicker/dist/css/bootstrap-colorpicker.css" />
    <link rel="stylesheet" href="../components/bootstrap-duallistbox/dist/bootstrap-duallistbox.css" />
	<link rel="stylesheet" href="../components/bootstrap-multiselect/dist/css/bootstrap-multiselect.css" />
	<link rel="stylesheet" href="../components/select2/dist/css/select2.css" />
    <link rel="stylesheet" href="../components/jquery-colorbox/example1/colorbox.css" />

    <link rel="stylesheet" href="../components/treegrid-flexible/css/jquery.treegrid.css"/>

    <link rel="stylesheet" href="../assets/css/datepicker.css" />
	<link rel="stylesheet" href="../assets/css/colorpicker.css" />
    <link rel="stylesheet" href="../assets/css/bootstrap-editable.css" />

	<!-- text fonts -->
	<link rel="stylesheet" href="../assets/css/ace-fonts.css" />

	<!-- ace styles -->
	<link rel="stylesheet" href="../assets/css/ace.css" class="ace-main-stylesheet" />

	<!--[if lte IE 9]>
	<link rel="stylesheet" href="../assets/css/ace-part2.css" class="ace-main-stylesheet" />
	<![endif]-->
	<link rel="stylesheet" href="../assets/css/ace-skins.css" />
	<link rel="stylesheet" href="../assets/css/ace-rtl.css" />

	<!--[if lte IE 9]>
		<link rel="stylesheet" href="../assets/css/ace-ie.css" />
	<![endif]-->

	<!-- inline styles related to this page -->

	<!-- ace settings handler -->
	<script src="../assets/js/ace-extra.js"></script>


 <script src="../components/_mod/jquery-ui.custom/jquery-ui.custom.js"></script>
	<script src="../components/jqueryui-touch-punch/jquery.ui.touch-punch.js"></script>
	<script src="../components/chosen/chosen.jquery.js"></script>
	<script src="../components/fuelux/js/spinbox.js"></script>
	<script src="../components/bootstrap-datepicker/dist/js/bootstrap-datepicker.js"></script>
	<script src="../components/bootstrap-timepicker/js/bootstrap-timepicker.js"></script>
	<script src="../components/moment/moment.js"></script>
	<script src="../components/bootstrap-daterangepicker/daterangepicker.js"></script>
	<script src="../components/eonasdan-bootstrap-datetimepicker/src/js/bootstrap-datetimepicker.js"></script>
	<script src="../components/mjolnic-bootstrap-colorpicker/dist/js/bootstrap-colorpicker.js"></script>
	<script src="../components/jquery-knob/js/jquery.knob.js"></script>
	<script src="../components/autosize/dist/autosize.js"></script>
	<script src="../components/jquery-inputlimiter/jquery.inputlimiter.js"></script>
	<script src="../components/jquery.maskedinput/dist/jquery.maskedinput.js"></script>
	<script src="../components/_mod/bootstrap-tag/bootstrap-tag.js"></script>
    <script src="../components/_mod/bootstrap-duallistbox/jquery.bootstrap-duallistbox.js"></script>
	<script src="../components/raty/lib/jquery.raty.js"></script>
	<script src="../components/_mod/bootstrap-multiselect/bootstrap-multiselect.js"></script>
	<script src="../components/select2/dist/js/select2.js"></script>
	<script src="../components/typeahead.js/dist/typeahead.jquery.js"></script>
    <script src="../components/jquery-colorbox/jquery.colorbox.js"></script>

    <script src="../components/treegrid-flexible/js/jquery.treegrid.js"></script>

	<!-- ace scripts -->
    <script src="../assets/js/src/elements.scroller.js"></script>
	<script src="../assets/js/src/elements.colorpicker.js"></script>
	<script src="../assets/js/src/elements.fileinput.js"></script>
	<script src="../assets/js/src/elements.typeahead.js"></script>
	<script src="../assets/js/src/elements.wysiwyg.js"></script>
	<script src="../assets/js/src/elements.spinner.js"></script>
	<script src="../assets/js/src/elements.treeview.js"></script>
	<script src="../assets/js/src/elements.wizard.js"></script>
	<script src="../assets/js/src/elements.aside.js"></script>
	<script src="../assets/js/src/ace.js"></script>
	<script src="../assets/js/src/ace.basics.js"></script>
	<script src="../assets/js/src/ace.scrolltop.js"></script>
	<script src="../assets/js/src/ace.ajax-content.js"></script>
	<script src="../assets/js/src/ace.touch-drag.js"></script>
	<script src="../assets/js/src/ace.sidebar.js"></script>
	<script src="../assets/js/src/ace.sidebar-scroll-1.js"></script>
	<script src="../assets/js/src/ace.submenu-hover.js"></script>
	<script src="../assets/js/src/ace.widget-box.js"></script>
	<script src="../assets/js/src/ace.settings.js"></script>
	<script src="../assets/js/src/ace.settings-rtl.js"></script>
	<script src="../assets/js/src/ace.settings-skin.js"></script>
	<script src="../assets/js/src/ace.widget-on-reload.js"></script>
	<script src="../assets/js/src/ace.searchbox-autocomplete.js"></script>
   
    <script src="../assets/js/jquery.autosize.js"></script>

    <%--Custome Futech--%>
    <script src="../assets/js/custom.js"></script>




<script src="../jquery-1.4.1.min.js" type="text/javascript"></script>

<script src="../assets/js/jquery.autocomplete.js" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $("#<%=txtSearch.ClientID%>").autocomplete('Search_CS.ashx');
    });       
</script> 
</head>
<body>
    <form id="form1" runat="server">
    <!-- #section:basics/content.searchbox -->
					<div class="nav-search" id="nav-search2">
						<form class="form-search">
							<span class="input-icon">
								<input type="text" placeholder="Tìm kiếm ..." class="nav-search-input" id="nav-search-input2" autocomplete="off" />
								<i class="ace-icon fa fa-search nav-search-icon"></i>
							</span>
						</form>
					</div><!-- /.nav-search -->
    <div>
        <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
    </div>
    </form>
</body>
</html>

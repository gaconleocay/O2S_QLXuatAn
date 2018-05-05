<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="ActiveCard.aspx.cs" Inherits="QLXuatAn_ActiveCard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
 <!-- JS Global Compulsory -->			
    <script type="text/javascript" src="../assets/js/jquery.js"></script>
   <%--<script src="../assets/js/google-api/jquery-ui.min.js" type="text/javascript"></script>--%>
   <%--<link href="../assets/css/google-api/jquery-ui.css" rel="Stylesheet" type="text/css" />--%>

   <style type="text/css">
       .modal {
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
   </style>

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
    $('form').on("submit", function () {
        ShowProgress();
    });
</script>

    <script type="text/javascript">
        $(document).ready(function () {

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
                        url: "ActiveCard.aspx/GetFees",
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


            //$("#demo2-add").click(function () {
            //    demo2.append('<option value="apples">Apples</option><option value="oranges" selected>Oranges</option>');
            //    demo2.trigger('bootstrapduallistbox.refresh');
            //});

                var demo2 = $('#<%=cbCardList.ClientID%>').bootstrapDualListbox({
                nonselectedlistlabel: 'Non-selected',
                selectedlistlabel: 'Selected',
                preserveselectiononmove: 'moved',
                moveonselect: false
            });

                $('#<%=cbCardList.ClientID%>').on('change', function () {
                    var _val = '';
                    $(this).find('option:selected').each(function () {
                        //alert($(this).val());
                        _val += $(this).val() + ',';
                    });

                    $('#<%=hidValueSelected.ClientID %>').val(_val);
                    //console.log('triggers');
                });

            var container2 = demo2.bootstrapDualListbox('getContainer');
            container2.find('.btn').addClass('btn-white btn-info btn-bold');

            $('#btnRefresh').click(function (e) {

                e.preventDefault();
                window.location.href = 'ActiveCard.aspx?KeyWord=' + document.getElementById("<%= txtKeyWord.ClientID %>").value +
                                   '&CardGroupID=' + document.getElementById("<%= cbCardGroup.ClientID %>").value +
                    '&CustomerID=' + $('.plcCustomerId').find('input[type=hidden]').val() +
                    '&cusName=' + $('#nav-customer-input').val() +
                    '&CustomerGroupID=' + document.getElementById("<%= cbCustomerGroup.ClientID %>").value;
            });


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

        function myFunction() {
            var obj = {};
            obj.customergroupid = document.getElementById("<%=cbCustomerGroup.ClientID %>").value;

                AjaxPostMultiParams("ActiveCard.aspx/GetCustomerByGroup", obj).success(function (result) {
                    if (result.d.toString() != "") {

                        $('.chosen-select').empty();
                        $('.chosen-select').trigger('chosen:updated');

                        var tem = result.d.toString().split('#');
                        var i = 0;

                        var opt1 = document.createElement("option");
                        opt1.value = "";
                        opt1.text = "";
                        document.getElementById($('.plcCustomerId').find('input[type=hidden]').val()).options.add(opt1);

                        for (i = 0; i < tem.length; i++) {
                            // Create an Option object       
                            var opt = document.createElement("option");

                            // Assign text and value to Option object

                            opt.value = tem[i].split(';')[0];
                            opt.text = tem[i].split(';')[1];

                            // Add an Option object to Drop Down List Box
                            document.getElementById($('.plcCustomerId').find('input[type=hidden]').val()).options.add(opt);
                        }
                        $('.chosen-select').trigger('chosen:updated');

                    }
                    else {
                        $('.chosen-select').empty();
                        $('.chosen-select').trigger('chosen:updated');
                    }

                });
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
 <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Gia hạn thẻ</li>
	</ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
 <div class="page-header">
        <h1 id="id_cardlist" runat="server">
	        Danh sách thẻ
        </h1>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
 <%--Hien thi thong bao tai day--%>
    <div class="alert alert-warning" id="div_alert" runat="server">
		<button type="button" class="close" data-dismiss="alert">
			<i class="ace-icon fa fa-times"></i>
		</button>
        <i class="ace-icon fa fa-exclamation-triangle"></i>
		<span id="id_alert" runat = "server"></span>
		<br />
	</div>

   
    <div class="col-xs-12">
     
       <form id="frm_CardDetail" class="form-horizontal" runat="server">
        <div class="col-xs-12">
	        <div class="row">
                 <div class="widget-body">
				    <div class="widget-main">

                        <div class="form-group" id="div_refresh" runat="server">
				        

                                <div class="col-sm-3 no-padding-left">      
                                    <select class="form-control" id="cbCardGroup" runat = "server">
                                    
                                    </select> 
                                </div>

                                 <div class="col-sm-3 no-padding-left">      
                                    <select class="form-control" id="cbCustomerGroup" onchange="myFunction()" runat = "server">
                                    
                                    </select> 
                                </div>

                                <div class="col-sm-2 no-padding-left"> 
				                   <%--<select class="chosen-select form-control" id="cbCustomer" data-placeholder="Select customer..."  runat = "server">
                                    </select>--%> 					                   

                                    <div class="plcCustomerId">
                                     <input type="text" placeholder="Nhập tên hoặc mã KH" class="nav-search-input" id="nav-customer-input" value="<%=ViewState["cusName"] %>"/>
                                    <input type="hidden" name="cbCustomer" id="cbCustomer" runat="server" value="" />
                                </div>

				                </div>

                                  <div class="col-sm-offset-0 col-sm-2 no-padding-left"">
                                    <input type="text" id="txtKeyWord" placeholder="Từ khóa tìm kiếm" class="form-control" runat="server" />
				                </div>

                                <div class="col-sm-2 no-padding-left">    
                                <button type="button" class="btn btn-white btn-default" id="btnRefresh">
                                    <i class="fa fa-refresh"></i>
                                    Nạp lại
                                </button>
                            </div>

                        </div>
 

                        <div class="form-group">

                            <label class="col-sm-1 control-label no-padding-right">Mức phí</label>                     
						    <div class="col-sm-2">
                                <asp:TextBox id="txtFee" runat="server" class="form-control autosuggest money" placeholder="" Text="0" />		
						    </div>

				            <label class="col-sm-1 control-label no-padding-right" for="form-field-1"> Hạn SD </label>
				            <div class="col-sm-2">
					            <div class="input-group">
						            <input class="form-control" id="dtpExpireDate" runat="server" type="text"/>
						            <span class="input-group-addon">
							            <i class="fa fa-calendar bigger-110"></i>
						            </span>
					            </div>
				            </div>

			            </div>

                     <div class="form-group">
                        <div class="col-sm-offset-1 col-sm-3">  
				            <label class="inline">
				                <input type="checkbox" class="ace" id="chbEnableMinusActive" runat="server"/>
				                <span class="lbl"> Cho phép gia hạn âm ngày</span>
			                </label>
                        </div>
			        </div>
                  	        
                    </div>
                   
			    </div>
		    </div>
        </div>

         <div class="col-xs-12">
         <div class="hr hr-16 hr-dotted"></div>

         <h2 class="col-sm-offset-1 light-blue smaller-90">
            Định dạng: Mã thẻ * CardNo * Biển số * Thời hạn cũ * Tên khách hàng
         </h2>
        <div class="form-group">
			<label class="col-sm-1 control-label no-padding-top" for="cbCardList"> Chọn thẻ gia hạn </label>
			<div class="col-sm-11">
				<!-- #section:plugins/input.duallist -->
				<select multiple="true" class="form-control" name="duallistbox_demo1[]" id="cbCardList" runat="server" >
						
				</select>
                <input type="hidden" id="hidValueSelected" value="" runat="server" />
				<!-- /section:plugins/input.duallist -->
				<%--<div class="hr hr-16 hr-dotted"></div>--%>
			</div>
        </div>

        <div class="hr hr-16 hr-dotted"></div>

        <div class="loading" align="center">
            Processing. Please wait.<br />
            <br />
            <img src="loader.gif" alt="" />
        </div>

       
        <div class="clearfix form-actions">
				<div class="col-md-offset-3 col-md-9">
                   <%-- <asp:LinkButton ID="LinkButton1" OnClick="Save" runat="server" class="btn btn-info">
                        <i class="ace-icon fa fa-check bigger-110"></i>
						<span class="bigger-110">Lưu lại</span>
                    </asp:LinkButton>--%>

                    <asp:Button ID="Button1" OnClick="Save" runat="server" class="btn btn-info ace-icon fa fa-check bigger-160" Text="Đồng ý "/>
                 <%--   &nbsp; &nbsp; &nbsp;--%>
                  <%--  <button class="btn" type="button" onclick="window.location.href='Card.aspx'">
					<i class="ace-icon fa fa-undo bigger-110"></i>
					Quay lại
				</button>--%>

				</div>
		</div>
        </div>
        </form>
     
 
    
    </div><!-- /.col -->
    
    
   
</asp:Content>


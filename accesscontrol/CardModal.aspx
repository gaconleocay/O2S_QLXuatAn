<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CardModal.aspx.cs" Inherits="accesscontrol_CardModal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="row">
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
        </div>

        <div class="row">
            <div class="widget-body">
				<div class="widget-main">
                    <button type="button" class="btn btn-info btn-sm" data-toggle="modal" data-target="#modal-form" id="btnAddCard">
                        <i class="fa fa-plus"></i>
                        Thêm mới
                    </button>
                </div>
            </div>
        </div>
        				    
        <div class="row">
            <div class="col-xs-12">
                <div class="widget-box">
			        <div class="widget-main no-padding">
                        <asp:Repeater id="rpt_Card" runat="server">
                            <HeaderTemplate>
	                            <table class="table table-striped table-bordered table-hover">
		                            <thead>
			                            <tr>
				                            <th>Số thẻ</th>
                                            <th>Mã thẻ</th>
                                            <th class="hidden-480">Nhóm thẻ</th>
                                            <th class="hidden-480">Ngày hết hạn</th>
                                            <%--<th class="hidden-480">Miêu tả</th>--%>
				                            <th class="hidden-480">Trạng thái</th>
				                            <th></th>
			                            </tr>
		                            </thead>
                            </HeaderTemplate>

                            <ItemTemplate>
			                    <tr>
				                    <%--<td><%#(Container.ItemIndex+1).ToString()%></td>--%>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNo")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "CardNumber")%></td>
                                    <td class="hidden-480">
                                        <%#this.GetCardGroup(DataBinder.Eval(Container.DataItem, "CardGroupID").ToString())%>
				                    </td>
                                    <td class="hidden-480"><%#this.GetExpireDate(DataBinder.Eval(Container.DataItem, "ExpireDate").ToString())%></td>
                                    <%--<td class="hidden-480"><%#DataBinder.Eval(Container.DataItem, "Description")%></td>--%>
				                    <td class="hidden-480">
                                        <%#this.GetCardStatus(DataBinder.Eval(Container.DataItem, "IsLock").ToString())%>
				                    </td>
				                    <td id="<%#DataBinder.Eval(Container.DataItem, "CardID")%>">
					                    <div class="hidden-sm hidden-xs btn-group">
                                            <div class="btn-group">
						                        <button class="btn btn-xs btn-info detail" title="Sửa">
							                        <i class="ace-icon fa fa-pencil bigger-120"></i>
						                        </button>
						                        <button class="btn btn-xs btn-danger delete" title="Xóa" >
							                        <i class="ace-icon fa fa-trash-o bigger-120"></i>
						                        </button>
					                        </div>
                                        </div>

                                        <div class="hidden-md hidden-lg">
									        <div class="inline pos-rel">
										        <button class="btn btn-minier btn-primary dropdown-toggle" data-toggle="dropdown" data-position="auto">
											        <i class="ace-icon fa fa-cog icon-only bigger-110"></i>
										        </button>

										        <ul class="dropdown-menu dropdown-only-icon dropdown-yellow dropdown-menu-right dropdown-caret dropdown-close">
											        <li>
												        <a class="tooltip-info detail" data-rel="tooltip" title="Sửa">
													        <span class="blue">
														        <i class="ace-icon fa fa-pencil bigger-120"></i>
													        </span>
												        </a>
											        </li>
											        <li>
												        <a href="#" class="tooltip-success delete" data-rel="tooltip" title="Xóa">
													        <span class="green">
														        <i class="ace-icon fa fa-trash-o bigger-120"></i>
													        </span>
												        </a>
											        </li>
										        </ul>
									        </div>
								        </div>

				                    </td>
			                    </tr>
                            </ItemTemplate>

                            <FooterTemplate>
	                            </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

<script type="text/javascript">
    jQuery(document).ready(function () {

        $('.detail').click(function (e) {
            e.preventDefault();

            var id = $(this).closest('td').attr("id");
            var element = $(this).closest('tr');

            LoadAjaxContent2("CardDetailModal.aspx", { 'Id': id, 'CustomerID': '<%=this.ViewState["CustomerID"].ToString()%>' }, 'id_carddetailmodal');

            $("#btnAddCard").click();

        });

        $('.delete').click(function (e) {
            e.preventDefault();

            var id = $(this).closest('td').attr("id");
            var element = $(this).closest('tr');

            if (confirm("Bạn có muốn xóa bản ghi này không?")) {
                AjaxPOST("CardModal.aspx/Delete", '{"id":"' + id + '"}').success(function (result) {
                    if (result.d.toString() == "true") {
                        element.remove();
                    }
                    else
                        alert(result.d.toString());
                });
            }

        });

    });
</script>


<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="accesscontrol_Login" %>

<!DOCTYPE html>
<html lang="en">
	<head>
		<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
		<meta charset="utf-8" />
		<title>Kz-iWachPro</title>

		<meta name="description" content="User login page" />
		<meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0" />

		<!-- bootstrap & fontawesome -->
		<link rel="stylesheet" href="../assets/css/bootstrap.css" />
		<link rel="stylesheet" href="../assets/css/font-awesome.css" />

		<!-- text fonts -->
		<link rel="stylesheet" href="../assets/css/ace-fonts.css" />

		<!-- ace styles -->
		<link rel="stylesheet" href="../assets/css/ace.css" />

		<!--[if lte IE 9]>
			<link rel="stylesheet" href="../assets/css/ace-part2.css" />
		<![endif]-->
		<link rel="stylesheet" href="../assets/css/ace-rtl.css" />

		<!--[if lte IE 9]>
		  <link rel="stylesheet" href="../assets/css/ace-ie.css" />
		<![endif]-->

		<!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->

		<!--[if lt IE 9]>
		<script src="../assets/js/html5shiv.js"></script>
		<script src="../assets/js/respond.js"></script>
		<![endif]-->
	</head>

	<body class="login-layout blur-login">
		<div class="main-container">
			<div class="main-content">
				<div class="row">
                    
                    <div class="space-16"></div>

					<div class="col-sm-10 col-sm-offset-1">
						<div class="login-container">
							<div class="center">
								<h1>
									<i class="ace-icon fa fa-diamond green"></i>
									<span class="red">Kz-iWachPro</span>
									<span class="white" id="id-text2">Application</span>
								</h1>
								<h4 class="light-blue" id="id-company-text">&copy; Kztek JSC</h4>
							</div>

							<div class="space-16"></div>

							<div class="position-relative">
								<div id="login-box" class="login-box visible widget-box no-border">
									<div class="widget-body">
										<div class="widget-main">
											<h4 class="header blue lighter bigger">
												<i class="ace-icon fa fa-coffee green"></i>
												Thông tin đăng nhập
											</h4>

											<div class="space-6"></div>

											<form id="frmLogin" runat="server">
												<fieldset>
													<label class="block clearfix">
														<span class="block input-icon input-icon-right">
                                                            <asp:TextBox id="txtUserName" runat="server" class="form-control" placeholder="Tên đăng nhập"/>
															<i class="ace-icon fa fa-user"></i>
														</span>
													</label>

													<label class="block clearfix">
														<span class="block input-icon input-icon-right">
                                                            <asp:TextBox id="txtPassword" TextMode="password" runat="server" class="form-control" placeholder="Mật khẩu"/>
															<i class="ace-icon fa fa-lock"></i>
														</span>
													</label>

                                                    <label class="block clearfix">
                                                        <label class="inline">
															<input type="checkbox" class="ace" id="chbIsRemember" runat="server"/>
															<span class="lbl"> Duy trì đăng nhập</span>
														</label>
                                                    </label>

													<label class="block clearfix">
                                                        <asp:LinkButton ID="btnLogin" OnClick="Login" runat="server" class="btn btn-sm btn-primary">
                                                           <i class="ace-icon fa fa-key"></i>
															<span class="bigger-110">Đăng nhập</span>
                                                        </asp:LinkButton>
													</div>

													<div class="space-4"></div>
												</fieldset>
											</form>
											
										</div><!-- /.widget-main -->

									</div><!-- /.widget-body -->
								</div><!-- /.login-box -->

							</div><!-- /.position-relative -->

						</div>
					</div><!-- /.col -->
				</div><!-- /.row -->
			</div><!-- /.main-content -->
		</div><!-- /.main-container -->

		<!-- basic scripts -->

		<!--[if !IE]> -->
		<script type="text/javascript">
		    window.jQuery || document.write("<script src='../assets/js/jquery.js'>" + "<" + "/script>");
		</script>

		<!-- <![endif]-->

		<!--[if IE]>
        <script type="text/javascript">
         window.jQuery || document.write("<script src='../assets/js/jquery1x.js'>"+"<"+"/script>");
        </script>
        <![endif]-->
		<script type="text/javascript">
		    if ('ontouchstart' in document.documentElement) document.write("<script src='../assets/js/jquery.mobile.custom.js'>" + "<" + "/script>");
		</script>

		<!-- inline scripts related to this page -->
		<script type="text/javascript">

		    //you don't need this, just used for changing background
		    jQuery(function ($) {

		        

		    });
		</script>
	</body>
</html>


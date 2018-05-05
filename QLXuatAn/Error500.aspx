<%@ Page Title="" Language="C#" MasterPageFile="~/QLXuatAn/MasterPage.master" AutoEventWireup="true" CodeFile="Error500.aspx.cs" Inherits="QLXuatAn_Error500" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content_BreadCrumb" Runat="Server">
    <ul class="breadcrumb">
		<li>
			<i class="ace-icon fa fa-home home-icon"></i>
			<a href="#">Trang chủ</a>
		</li>
		<li class="active">Error 500</li>
	</ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Content_PageHeader" Runat="Server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="Content_PageContent" Runat="Server">
    
	<!-- #section:pages/error -->
	<div class="error-container">
		<div class="well">
			<h1 class="grey lighter smaller">
				<span class="blue bigger-125">
					<i class="ace-icon fa fa-random"></i>
					500
				</span>
				Something Went Wrong
			</h1>

			<hr />
			<h3 class="lighter smaller">
				But we are working
				<i class="ace-icon fa fa-wrench icon-animated-wrench bigger-125"></i>
				on it!
			</h3>

			<div class="space"></div>

			<div>
				<h4 class="lighter smaller">Meanwhile, try one of the following:</h4>

				<ul class="list-unstyled spaced inline bigger-110 margin-15">
					<li>
						<i class="ace-icon fa fa-hand-o-right blue"></i>
						Read the faq
					</li>

					<li>
						<i class="ace-icon fa fa-hand-o-right blue"></i>
						Give us more info on how this specific error occurred!
					</li>
				</ul>
			</div>

			<hr />
			<div class="space"></div>

			<div class="center">
				<a href="javascript:history.back()" class="btn btn-grey">
					<i class="ace-icon fa fa-arrow-left"></i>
					Go Back
				</a>

				<a href="#" class="btn btn-primary">
					<i class="ace-icon fa fa-tachometer"></i>
					Dashboard
				</a>
			</div>
		</div>
	</div>

	<!-- /section:pages/error -->
</asp:Content>


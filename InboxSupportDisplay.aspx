<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="InboxSupportDisplay.aspx.cs" Inherits="LIMS.InboxSupportDisplay" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <style type="text/css">
		.emails-table { width: 600px; border: solid 1px #444444; }
		.emails-table-header { font-family: "Trebuchet MS"; font-size: 9pt;
			background-color: #0099B9; font-weight: bold; color: white;
			text-align: center; border: solid 1px #444444; }
		.emails-table-header-cell { font-family: "Georgia"; font-size: 9pt;
			font-weight: bold; border: solid 1px #666666; padding: 6px; }
		.emails-table-cell { font-family: "Georgia"; font-size: 9pt;
			border: solid 1px #666666; padding: 6px; }
		.emails-table-footer { border: solid 1px #666666; padding: 3px;
			width: 50%; }
		.email-datetime { float: right; color: #666666; }		
		
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
          <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card"><hr class="violet"><div class="header">
                        
                           
                            <div class="row">
                                 <div class="col-sm-6">
                                        <h2>HEMSHELL SUPPORT EMAILS</h2>
                                </div>                                
                            </div>                            
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <div>
		<asp:Literal ID="DebugLiteral" runat="server" />
    
    <table class="table table-bordered table-striped table-hover js-basic-example dataTable">
    <tr>
		<td class="emails-table-header" colspan="2">
		Email #<asp:Literal ID="EmailIdLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-header-cell">Date &amp; Time</td>
		<td class="emails-table-cell">
			<asp:Literal ID="DateLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-header-cell">From</td>
		<td class="emails-table-cell">
			<asp:Literal ID="FromLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-header-cell">Subject</td>
		<td class="emails-table-cell">
			<asp:Literal ID="SubjectLiteral" runat="server" /></td>
    </tr>
    <tr id="AttachementsRow" runat="server">
		<td class="emails-table-header-cell">Attachments</td>
		<td class="emails-table-cell">
			<asp:Literal ID="AttachmentsLiteral" runat="server" /></td>
    </tr>
     <tr>
		<td class="emails-table-cell" colspan="2">
			<asp:Literal ID="HeadersLiteral" runat="server" /></td>
    </tr>
    <tr>
		<td class="emails-table-cell" colspan="2">
			<asp:Literal ID="BodyLiteral" runat="server" /></td>
    </tr>
    </table>
                                    </div>
                            </div>
                        </div>
                    </div>
            </div>
</asp:Content>

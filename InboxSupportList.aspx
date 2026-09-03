<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="InboxSupportList.aspx.cs" Inherits="LIMS.InboxSupportList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .emails-table {
            width: 600px;
            border: solid 1px #444444;
        }

        .emails-table-header {
            font-family: "Trebuchet MS";
            font-size: 9pt;
            background-color: #0099B9;
            color: white;
            border: solid 1px #444444;
        }

        .emails-table-header-cell {
            font-family: "Georgia";
            font-size: 9pt;
            font-weight: bold;
            border: solid 1px #666666;
            padding: 6px;
        }

        .emails-table-cell {
            font-family: "Georgia";
            font-size: 9pt;
            border: solid 1px #666666;
            padding: 6px;
        }

        .emails-table-footer {
            border: solid 1px #666666;
            padding: 3px;
            width: 50%;
        }

        .email-datetime {
            float: right;
            color: #666666;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
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
                            <%--<div class="row">
                                <div class="col-sm-6">
                                    Show
                                <asp:DropDownList ID="ddlCount" runat="server" >
                                    <asp:ListItem>100</asp:ListItem>
                                    <asp:ListItem>1000</asp:ListItem>
                                </asp:DropDownList>
                                    Entries<br />
                                </div>
                            </div>--%>
                            <asp:Table ID="EmailsTable" class="table table-bordered table-striped table-hover js-basic-example dataTable" runat="server">
                                <asp:TableHeaderRow>
                                    <asp:TableHeaderCell CssClass="emails-table-header"
                                        ColumnSpan="4">
                                        Listing emails 
			<asp:Literal ID="EmailFromLiteral" runat="server" />-
			<asp:Literal ID="EmailToLiteral" runat="server" />
                                        of 
			<asp:Literal ID="EmailTotalLiteral" runat="server" />
                                        for 
			<asp:Literal ID="EmailLiteral" runat="server" />
                                    </asp:TableHeaderCell>
                                </asp:TableHeaderRow>
                                <asp:TableRow>
                                    <asp:TableCell Width="2%">
				#</asp:TableCell>
                                    <asp:TableCell CssClass="emails-table-header-cell">
				From</asp:TableCell>
                                    <asp:TableCell CssClass="emails-table-header-cell">
				Subject</asp:TableCell>
                                    <asp:TableCell CssClass="emails-table-header-cell">
				Date &amp; Time</asp:TableCell>
                                </asp:TableRow>
                                <asp:TableFooterRow>
                                    <asp:TableCell CssClass="emails-table-footer" ColumnSpan="4">
                                        <asp:Table ID="FooterTable" Width="100%"
                                            BorderWidth="0" runat="server">
                                            <asp:TableRow>
                                                <asp:TableCell>
                                                    <asp:Literal ID="PreviousPageLiteral" runat="server" />
                                                </asp:TableCell>
                                                <asp:TableCell HorizontalAlign="Right">
                                                    <asp:Literal ID="NextPageLiteral" runat="server" />
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
                                    </asp:TableCell>
                                </asp:TableFooterRow>
                            </asp:Table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="EnquiryList.aspx.cs" Inherits="LIMS.EnquiryList" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <style>
        .form-control {
            width: 100%;
        }

        .auto-style1 {
            display: block;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            -o-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
            background-image: none;
        }

        .card {
            margin-bottom: 30px;
            border: none;
            border-radius: 5px;
            box-shadow: 0 0.46875rem 2.1875rem rgba(90,97,105,0.1), 0 0.9375rem 1.40625rem rgba(90,97,105,0.1), 0 0.25rem 0.53125rem rgba(90,97,105,0.12), 0 0.125rem 0.1875rem rgba(90,97,105,0.1);
        }

        .violet {
            border-top: 3px solid #9c27b0;
            margin: 0;
        }

        .header {
            padding: 20px;
            margin-bottom: 20px;
        }

            .header h2 {
                margin: 0;
                color: #444;
                font-size: 24px;
            }

        .body {
            padding: 20px;
        }

        .btn-group {
            margin-bottom: 20px;
            margin-right: 10px;
        }

        .btn-secondary {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
            margin-right: 5px;
        }

            .btn-secondary:hover {
                background-color: #5a6268;
                border-color: #545b62;
            }

        .btn-primary.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
            background-color: #2196F3;
            border-color: #2196F3;
        }

        .btn-danger.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }

        .table {
            margin-bottom: 0;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0,0,0,.075);
        }

        .table-responsive {
            min-height: .01%;
            overflow-x: auto;
        }

        .hyperlink-column a {
            color: #2196F3;
            text-decoration: none;
        }

            .hyperlink-column a:hover {
                text-decoration: underline;
            }

        .btn-primary.active,
        .btn-primary:active,
        .btn-primary:focus {
            background-color: #8e44ad !important;
            color: #ffffff !important;
            border-color: #732d91 !important;
        }

        .btn-primary:hover {
            background-color: #9b59b6 !important;
            color: #ffffff !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>ENQUIRY LIST</h2>
                </div>
                <div class="body">

                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="btn-group btn-group-toggle">
                                <asp:HiddenField ID="hfActivePanel" runat="server" Value="New" />
                                <asp:Button ID="btnNew" runat="server" Text="NEW" CssClass="btn btn-primary" OnClick="btnNew_Click" />
                                <asp:Button ID="btnQuoteReceived" runat="server" Text="QUOTE RECEIVED" CssClass="btn btn-primary" OnClick="btnQuoteReceived_Click" />
                                <asp:Button ID="btnAccepted" runat="server" Text="ACCEPTED" CssClass="btn btn-primary" OnClick="btnAccepted_Click" />
                                <asp:Button ID="btnPOGenerated" runat="server" Text="PO GENERATED" CssClass="btn btn-primary" OnClick="btnPOGenerated_Click" Visible="true" />
                                <asp:Button ID="btnOthers" runat="server" Text="OTHERS" CssClass="btn btn-primary" OnClick="btnOthers_Click" />
                                <asp:HiddenField ID="hdStatusID" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix mt-3">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <h5>Enquiry List</h5>
                            <div class="table-responsive">
                                <asp:GridView ID="gvEnquiry" runat="server"
                                    CssClass="table table-bordered table-striped table-hover"
                                    AutoGenerateColumns="false"  OnRowDataBound="gvEnquiry_RowDataBound" OnRowCommand="gvEnquiry_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                <asp:Label ID="lblItemID" Text='<%# Eval("ItemID").ToString() %>' runat="server" Visible="false" />
                                                <asp:Label ID="lblItemCode" Text='<%# Eval("ItemCode").ToString() %>' runat="server" Visible="false" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Created" HeaderText="Enq.Dt." DataFormatString="{0:dd/MMM/yyyy}">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Enq#">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypEnquiryID" runat="server" Text='<%# Eval("EnquiryID").ToString().ToUpper() %>'
                                                    NavigateUrl='<%# String.Format("~/generateEnquiryReport.aspx?EnquiryID={0}", Eval("EnquiryID")) %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Item Description">
                                            <ItemTemplate>
                                                <div>
                                                    <asp:Label ID="Label2" runat="server" Text='<%# Eval("ItemDescription") %>'></asp:Label>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Enquiries Sent">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSent" runat="server" Text='<%# Eval("Sent").ToString().ToUpper() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblVendors" runat="server" Text='<%# Eval("Sent").ToString().ToUpper() %>' Visible="false"></asp:Label>
                                                <asp:HyperLink ID="hypVendor" runat="server" Text='<%# Eval("NameValues").ToString().ToUpper() %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("QuotationStatus").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Reminder">
                                            <ItemTemplate>
                                                <asp:Button ID="btnReminder" runat="server" CommandName="Reminder" Text="Reminder"
                                                    CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                    CssClass="btn btn-primary btn-sm" Visible="true" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Release">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRelease" runat="server" CommandName="Release" Text="Release"
                                                    CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                    CssClass="btn btn-danger btn-sm" Visible="true"
                                                    OnClientClick="return confirm('Are you sure you want to release this item from Enquiry?');" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix mt-3">
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <div class="table-responsive">
                                <asp:GridView ID="gvOtherEnquiry" runat="server" OnRowDataBound="gvOtherEnquiry_RowDataBound"
                                    CssClass="table table-bordered table-striped table-hover"
                                    AutoGenerateColumns="false" OnRowCommand="gvOtherEnquiry_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                <asp:Label ID="lblItemID" Text='<%# Eval("ItemID").ToString() %>' runat="server" Visible="false" />
                                                <asp:Label ID="lblItemCode" Text='<%# Eval("ItemCode").ToString() %>' runat="server" Visible="false" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Created" HeaderStyle-Width="8%" HeaderText="Enq.Dt." DataFormatString="{0:dd/MMM/yyyy}">
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:BoundField>
                                        <asp:TemplateField HeaderText="Enq#">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypEnquiryID" runat="server" Text='<%# Eval("EnquiryID").ToString().ToUpper() %>'
                                                    NavigateUrl='<%# Eval("EnquiryID", "QuoteAnalysis.aspx?ID={0}") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ItemDescription" HeaderStyle-Width="20%" HeaderText="ItemDescription">
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:BoundField>
                                       <%-- <asp:TemplateField HeaderText="Enquiries Sent">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSent" runat="server" Text='<%# Eval("NameValues").ToString().ToUpper() %>' Visible="true"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="Received">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypVendor" runat="server" Text='<%# Eval("vendorName").ToString().ToUpper() %>'
                                                    Target="_blank" Visible='<%# (Eval("QuotationStatus").ToString() == "PO Generated") ? false : true %>'></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Price">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Rate").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("QuotationStatus").ToString() %>'></asp:Label>
                                                <asp:Label ID="lblEnquiryID" runat="server" Text='<%# Eval("EnquiryID").ToString() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblVendorID" runat="server" Text='<%# Eval("VendorID").ToString() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblVendorQuotationID" runat="server" Text='<%# Eval("VendorQuotationID").ToString() %>' Visible="false"></asp:Label><br />
                                                <asp:HyperLink ID="hypQuote" runat="server" Text="File"
                                                    NavigateUrl='<%# "VendorQuotation/" + Eval("ReportFileName") + ".html" %>' Target="_blank" Visible="true"></asp:HyperLink><br />
                                                <asp:HyperLink ID="hyRegerenate" runat="server" Text="Generate"
                                                    NavigateUrl='<%# String.Format("~/GenerateVendorQuotation.aspx?ID={0}&type={1}", Eval("VendorID"), Eval("VendorQuotationID")) %>'
                                                    Target="_blank" Visible="true"></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Resend">
                                            <ItemTemplate>
                                                <asp:Button ID="btnResend" runat="server" CommandName="Send" Text="Resend"
                                                    CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                    CssClass="btn btn-primary btn-sm"
                                                    Visible='<%# (Eval("QuotationStatus").ToString() == "Accepted") || (Eval("QuotationStatus").ToString() == "Resent") || (Eval("QuotationStatus").ToString() == "PO Generated")? false : true %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Release">
                                            <ItemTemplate>
                                                <asp:Button ID="btnRelease" runat="server" CommandName="Release" Text="Release"
                                                    CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                    CssClass="btn btn-danger btn-sm"
                                                    Visible='<%# (Eval("QuotationStatus").ToString() == "Accepted") || (Eval("QuotationStatus").ToString() == "Resent")|| (Eval("QuotationStatus").ToString() == "PO Generated") ? false : true %>'
                                                    OnClientClick="return confirm('Are you sure you want to release this item from Enquiry?');" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    <asp:HiddenField ID="hdButton" runat="server" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="VendorQuotationManagement.aspx.cs" Inherits="LIMS.VendorQuotationManagement" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="AjaxToolkit" %>

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

        .table {
            margin-bottom: 0;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0,0,0,.075);
        }

        .btn-primary {
            background-color: #2196F3;
            border-color: #2196F3;
        }

        .btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }

        .mail-sent-label {
            color: #28a745;
            font-weight: bold;
            font-size: 12px;
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
                    <h2>VENDOR QUOTATION MANAGEMENT</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="row clearfix">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvQuotation" runat="server" CssClass="table table-bordered table-striped table-hover"
                                            AutoGenerateColumns="false" DataKeyNames="VendorQuotationID,VendorID2,EnquiryID,Action"
                                            OnRowCommand="gvQuotation_RowCommand">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl#">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%"></ItemStyle>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Enq#">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hypEnquiryID" runat="server" Text='<%# Eval("EnquiryID").ToString().ToUpper() %>'
                                                            NavigateUrl='<%# String.Format("~/GenerateEnquiryReport.aspx?EnquiryID={0}", Eval("EnquiryID")) %>' Target="_blank"></asp:HyperLink>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Vendor Quotation #">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdVendorQuotationID" runat="server" Value='<%# Eval("VendorQuotationID") %>' />
                                                        <asp:HiddenField ID="hdVendorID" runat="server" Value='<%# Eval("VendorID") %>' />
                                                        <asp:Label ID="lblVendorQuotationID" runat="server" Text='<%# "VQ-" + Eval("VendorQuotationID").ToString().ToUpper() %>' Visible="false"></asp:Label>
                                                        <asp:HyperLink ID="hypVendorQuotationID" runat="server"
                                                            Text='<%# "VQ-" + Eval("VendorQuotationID").ToString().ToUpper() %>'
                                                            NavigateUrl='<%# "GenerateVendorQuotation.aspx?type=" + Eval("VendorQuotationID") + "&ID=" + Eval("VendorID2") %>'
                                                            Target="_blank"></asp:HyperLink>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Vendor">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblVendor" runat="server" Text='<%# Eval("VendorName").ToString().ToUpper() %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Category">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCategory" runat="server" Text='<%# Eval("PurchaseSubGroupName").ToString().ToUpper() %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total Ordered Qty">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("TotalQuantity").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Quotation Number">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQuotationNumber" runat="server" Text='<%# Eval("QuotationNumber").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Delivery Time">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDeliveryTime" runat="server" Text='<%# Eval("DeliveryTime").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Remarks">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRemarks" runat="server" Text='<%# Eval("Remarks").ToString() %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnResend" runat="server" CommandName="Send" Text="Resend"
                                                            CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'
                                                            CssClass="btn btn-primary btn-xs"
                                                            Visible='<%# Eval("Action").ToString() != "6" %>' />
                                                        <asp:Label ID="lblMailSent" runat="server" Text="Mail sent for Revision"
                                                            CssClass="mail-sent-label"
                                                            Visible='<%# Eval("Action").ToString() == "6" %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="alert alert-info">No records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

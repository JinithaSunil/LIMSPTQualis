<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="QuoteAnalysis.aspx.cs" Inherits="LIMS.QuoteAnalysis" %>

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
        }

            .header h1 {
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

        .btn-primary {
            color: #fff;
            background-color: #2196F3;
            border-color: #2196F3;
        }

            .btn-primary:hover {
                background-color: #0b7dda;
                border-color: #0a6ebd;
            }

        .btn-price {
            color: #fff;
            background-color: #4CAF50;
            border-color: #4CAF50;
        }

            .btn-price:hover {
                background-color: #3e8e41;
                border-color: #367d39;
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

        /* Enhanced table container styling similar to first code */
        .table-container {
            overflow-x: auto;
            overflow-y: visible;
            max-width: 100%;
            border: 1px solid #dee2e6;
            border-radius: 0.375rem;
            background-color: #fff;
        }

            .table-container::-webkit-scrollbar {
                height: 8px;
            }

            .table-container::-webkit-scrollbar-track {
                background: #f1f1f1;
                border-radius: 10px;
            }

            .table-container::-webkit-scrollbar-thumb {
                background: #888;
                border-radius: 10px;
            }

                .table-container::-webkit-scrollbar-thumb:hover {
                    background: #555;
                }

        .table {
            margin-bottom: 0;
            width: 100%;
            max-width: 100%;
            background-color: transparent;
            border-collapse: collapse;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0,0,0,.075);
        }

        .table-responsive {
            min-height: .01%;
            overflow-x: auto;
        }

        .table-striped > tbody > tr:nth-of-type(odd) {
            background-color: #f9f9f9;
        }

        .table-th {
            background-color: #6c757d;
        }

        .table-bordered {
            border: 1px solid #ddd;
        }

            .table-bordered > thead > tr > th,
            .table-bordered > tbody > tr > th,
            .table-bordered > tfoot > tr > th,
            .table-bordered > thead > tr > td,
            .table-bordered > tbody > tr > td,
            .table-bordered > tfoot > tr > td {
                border: 1px solid #ddd;
            }

            /* Enhanced table header styling */
            .table-bordered thead th {
                background-color: #6c757d;
                border-bottom: 2px solid #dee2e6;
                font-weight: 600;
                color: #495057;
            }

        .hyperlink-column a {
            color: #2196F3;
            text-decoration: none;
        }

            .hyperlink-column a:hover {
                text-decoration: underline;
            }

        .panel_header {
            padding: 15px;
            background-color: #f5f5f5;
            border-bottom: 1px solid #ddd;
            margin-bottom: 20px;
            width: 100%;
        }

            .panel_header h3 {
                margin: 0;
                font-size: 18px;
                color: #333;
            }

        .padding-0 {
            padding: 0 !important;
        }

        .page-title {
            margin-bottom: 20px;
        }

            .page-title h1 {
                margin: 0;
                font-size: 28px;
                color: #333;
            }

        .quotation-container {
            display: grid;
            grid-template-columns: 1fr 1fr; /* Two equal columns */
            gap: 20px; /* Space between items */
        }

            .quotation-container .table-responsive {
                width: 100%; /* Each table takes full width of its grid cell */
            }

        @media (max-width: 768px) {
            .quotation-container {
                grid-template-columns: 1fr; /* Single column on mobile */
            }
        }

        /* Currency Selection Styles */
        .currency-selection-panel {
            background: linear-gradient(135deg, #b34788 0%, #b34788 100%);
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 25px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            color: white;
        }

        .currency-title {
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 15px;
            display: block;
        }

        .currency-selection-panel .radio-list {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
        }

            .currency-selection-panel .radio-list input[type="radio"] {
                margin-right: 8px;
                transform: scale(1.3);
            }

            .currency-selection-panel .radio-list label {
                background: rgba(255,255,255,0.2);
                border-radius: 20px;
                font-weight: 500;
                cursor: pointer;
                transition: all 0.3s ease;
                display: flex;
                align-items: center;
                margin-right: 25px;
                padding-right: 10px;
            }

                .currency-selection-panel .radio-list label:hover {
                    background: rgba(255,255,255,0.3);
                    transform: translateY(-2px);
                }

        /* Currency indicator in quotes */
        .currency-indicator {
            background: #b34788;
            color: white;
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: bold;
            display: inline-block;
            margin-left: 10px;
        }

        /* Ranking styles */
        .rank-first {
            color: #b34788;
            font-weight: bold;
            font-size: 16px;
        }

        .rank-not-available {
            color: #b34788;
            font-style: italic;
            font-size: 12px;
        }

        .rank-error {
            color: #ff0000;
            font-style: italic;
            font-size: 12px;
        }

        /* Quote table enhancements */
        .quote-table {
            border: 2px solid #e9ecef;
            border-radius: 8px;
            overflow: hidden;
            margin-bottom: 20px;
            background: white;
            box-shadow: 0 0.46875rem 2.1875rem rgba(90,97,105,0.1), 0 0.9375rem 1.40625rem rgba(90,97,105,0.1), 0 0.25rem 0.53125rem rgba(90,97,105,0.12), 0 0.125rem 0.1875rem rgba(90,97,105,0.1);
        }

        .quote-header {
            background: #dee2e6;
            padding: 15px;
            border-bottom: 2px solid #dee2e6;
            text-align: center;
        }

        .vendor-name {
            color: #444;
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 5px;
        }

        .quote-info {
            color: #6c757d;
            font-size: 14px;
        }

        /* Table styles */
        .quote-details-table {
            width: 100%;
            border-collapse: collapse;
            background: white;
        }

            .quote-details-table th {
                font-weight: 600;
                padding: 12px 8px;
                border-bottom: 2px solid #dee2e6;
                text-align: center;
            }

            .quote-details-table td {
                padding: 10px 8px;
                border-bottom: 1px solid #dee2e6;
                vertical-align: middle;
                border: 1px solid #ddd;
            }

            .quote-details-table tbody tr:hover {
                background-color: rgba(0,0,0,.075);
            }

            .quote-details-table tbody tr:nth-of-type(odd) {
                background-color: #f9f9f9;
            }

        .total-row {
            background: #f8f9fa !important;
            font-weight: bold;
        }

        .grand-total-row {
            background: #e9ecef !important;
            font-weight: bold;
            font-size: 16px;
        }

        /* Button styling */
        .btn-accept {
            background-color: #b34788;
            border: none;
            color: white;
            padding: 8px 20px;
            border-radius: 5px;
            font-weight: 500;
            transition: all 0.3s ease;
        }

            .btn-accept:hover {
                background-color: #3e8e41;
                border-color: #367d39;
                color: white;
            }

        .btn-generate-po {
            background-color: #2196F3;
            border: none;
            color: white;
            padding: 8px 20px;
            border-radius: 5px;
            font-weight: 500;
            transition: all 0.3s ease;
        }

            .btn-generate-po:hover {
                background-color: #0b7dda;
                border-color: #0a6ebd;
                color: white;
            }

        .action-row {
            background: #f8f9fa;
            border-top: 2px solid #dee2e6;
        }

            .action-row td {
                padding: 15px 8px;
            }

        .form-control:focus {
            border-color: #80bdff;
            box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25);
        }

        .btn-accept.disabled {
            opacity: 0.6;
            cursor: not-allowed;
            pointer-events: none;
        }
        /* Responsive adjustments */
        @media (max-width: 768px) {
            .quote-details-table {
                font-size: 12px;
            }

                .quote-details-table th,
                .quote-details-table td {
                    padding: 6px 4px;
                }

            .btn-accept,
            .btn-generate-po {
                padding: 6px 12px;
                font-size: 12px;
                margin: 2px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="upt1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h1>QUOTE ANALYSIS</h1>
                        </div>
                        <div class="body">
                            <!-- Currency Selection Panel -->

                            <div class="row clearfix">
                                <asp:Panel ID="pnlCurrencySelection" runat="server" CssClass="currency-selection-panel" Visible="false">
                                    <asp:Label ID="lblCurrencyTitle" runat="server" Text="Multiple currencies detected. Please select currency to compare:" CssClass="currency-title"></asp:Label>
                                    <asp:RadioButtonList ID="rblCurrency" runat="server"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="rblCurrency_SelectedIndexChanged"
                                        CssClass="radio-list"
                                        RepeatDirection="Horizontal"
                                        RepeatLayout="Flow">
                                    </asp:RadioButtonList>
                                </asp:Panel>
                            </div>

                            <div class="row clearfix">
                                <div class="col-12">
                                    <div class="quotation-container">
                                        <asp:Repeater ID="rptCategory" runat="server" OnItemDataBound="OnItemDataBound">
                                            <ItemTemplate>
                                                <div class="table-responsive">
                                                    <div class="quote-table">
                                                        <!-- Quote Header -->
                                                        <div class="quote-header">
                                                            <div class="vendor-name">
                                                                <asp:Label ID="lblName" runat="server" Text='<%#Eval("VendorName") %>'></asp:Label>
                                                                <asp:Label ID="lblCurrencyDisplay" runat="server" CssClass="currency-indicator"></asp:Label>
                                                            </div>
                                                            <div class="quote-info">
                                                                <asp:Label ID="lblContactPerson" runat="server"></asp:Label><br />
                                                                <asp:Label ID="lblID" runat="server" Text='<%#Eval("Vendor") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="lblCurrencyId" runat="server" Text='<%#Eval("QuoteCurrencyId") %>' Visible="false"></asp:Label>
                                                                <strong>Enquiry No.: </strong>
                                                                <asp:HyperLink ID="hypEnquiryID" runat="server"
                                                                    Text='<%# Eval("EnquiryID").ToString().ToUpper() + " Dt. " + Eval("EnquiryDate", "{0:dd/MMM/yyyy}") %>'
                                                                    NavigateUrl='<%# String.Format("~/GenerateEnquiryReport.aspx?ID={0}", Eval("EnquiryID")) %>'
                                                                    Target="_blank"
                                                                    Style="color: #b34788;"></asp:HyperLink><br />
                                                                <strong>Quotation No.: </strong>
                                                                <asp:HyperLink ID="hypQuote" runat="server"
                                                                    Text='<%# Eval("QuotationNumber").ToString().ToUpper() %>'
                                                                    NavigateUrl='<%# "VendorQuotation/" + Eval("ReportFileName") + ".html" %>'
                                                                    Target="_blank"
                                                                    Visible="true"
                                                                    Style="color: #b34788;"></asp:HyperLink>
                                                            </div>
                                                        </div>

                                                        <!-- Action Row moved to top -->
                                                        <div class="action-row" style="padding: 15px;">
                                                            <div class="row">
                                                                <div class="col-md-10">
                                                                    <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" placeholder="Reason for Accepting"></asp:TextBox>
                                                                </div>
                                                                <div class="col-md-2">
                                                                    <asp:HyperLink ID="hypPO" runat="server" Text='<%# "PO-" + Eval("PurchaseOrderID") %>' NavigateUrl='<%# "~/PO/PO-" + Eval("PurchaseOrderID") + ".html" %>' Visible="false" Target="_blank"></asp:HyperLink>
                                                                    <asp:Button ID="btnAccept" runat="server" Text="Accept" CssClass="btn btn-accept me-2" OnClick="btnAccept_Click" />
                                                                    <asp:Button ID="btnGeneratePO" runat="server" Text="Generate PO" CssClass="btn btn-generate-po" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"Vendor") %>' OnClick="btnGeneratePO_Click" />
                                                                </div>
                                                            </div>
                                                        </div>

                                                        <!-- Quote Details Table -->
                                                        <table class="quote-details-table">
                                                            <thead style="background-color: #dee2e6">
                                                                <tr style="background-color: #dee2e6">
                                                                    <th>Item</th>
                                                                    <th style="text-align: center">Qty</th>
                                                                    <th style="text-align: center">Unit</th>
                                                                    <th style="text-align: center; width: 92PX;">Delivery Time(In Days)</th>
                                                                    <th style="text-align: center">Rate</th>
                                                                    <th style="text-align: center">Discount%</th>
                                                                    <th style="text-align: center">Disc Amt.</th>
                                                                    <th style="text-align: right">Total</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater ID="rptGallery" runat="server">
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:HyperLink ID="hypItemRate" runat="server" Target="_blank" Text=' <%#Eval("ItemDescription") %>'></asp:HyperLink><br />
                                                                                <span><%#Eval("ItemSpecification") %></span>
                                                                            </td>
                                                                            <td style="text-align: center"><%#Convert.ToInt32(Eval("AvailableQuantity")) %></td>
                                                                            <td style="text-align: center"><%#Eval("UOMName") %></td>
                                                                            <td style="text-align: center"><%#Eval("DeliveryTime") %></td>
                                                                            <td style="text-align: center"><%#Eval("Rate") %></td>
                                                                            <td style="text-align: center"><%#Eval("Discount") %></td>
                                                                            <td style="text-align: center"><%#Eval("DiscountApplied") %></td>
                                                                            <td style="text-align: right"><%#Eval("TotalAfterDiscount") %></td>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                                <tr class="total-row">
                                                                    <td colspan="7"><b>Sub Total</b></td>
                                                                    <td style="text-align: right">
                                                                        <asp:Label ID="lblSubTotal" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr class="total-row">
                                                                    <td colspan="7">
                                                                        <asp:Label ID="lblAdditionalChargeDescription" runat="server"></asp:Label>
                                                                    </td>
                                                                    <td style="text-align: right">
                                                                        <asp:Label ID="lblAdditionalCharges" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr class="total-row">
                                                                    <td colspan="7"><b>VAT Amount</b></td>
                                                                    <td style="text-align: right">
                                                                        <asp:Label ID="lblVATAmount" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr class="grand-total-row">
                                                                    <td colspan="7"><b>Grand Total</b></td>
                                                                    <td style="text-align: right">
                                                                        <asp:Label ID="lblGrandTotal" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="7"><b>Payment</b></td>
                                                                    <td style="text-align: right">
                                                                        <asp:Label ID="lblPayment" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="7"><b>Remarks</b></td>
                                                                    <td style="text-align: right">
                                                                        <asp:Label ID="lblRemarks" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td colspan="7"><b>Rank</b></td>
                                                                    <td style="text-align: right;">
                                                                        <asp:Label ID="lblRank" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


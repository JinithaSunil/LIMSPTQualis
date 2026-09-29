<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PurchaseOrder.aspx.cs" Inherits="LIMS.PurchaseOrder1" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

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

        /* Enhanced table container styling similar to second code */
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
            background-color: #f8f9fa;
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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>PURCHASE ORDER</h2>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12 col-12 padding-0">
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                    <label for="txtPONumber">PO Number</label>
                                    <span id="Span3" style="color: Red" runat="server">*</span>
                                    <asp:TextBox ID="txtPONumber" Width="100%" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="txtDate">Date</label>
                                    <asp:TextBox ID="txtDate" Width="100%" class="form-control" runat="server" disabled="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="txtQuoteNo">Quote No.</label>
                                    <asp:TextBox ID="txtQuoteNo" Width="100%" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                            </div>
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                    <label for="txtVendor">Company/Vendor</label>
                                    <asp:TextBox ID="txtVendor" Width="100%" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="txtEmail">Email</label>
                                    <asp:TextBox ID="txtEmail" Width="100%" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-4 mb-3">
                                    <label for="txtContactPerson">Contact Person</label>
                                    <asp:TextBox ID="txtContactPerson" Width="100%" class="form-control" runat="server"></asp:TextBox>
                                    <asp:Label ID="lblContactPerson" runat="server" Visible="false"></asp:Label>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="col-md-4 mb-3">
                                    <label for="txtcurrency">Currency</label>
                                    <asp:TextBox ID="txtcurrency" Width="100%" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <label for="txtSpecialInstruction">Special Instruction</label>
                                    <asp:TextBox ID="txtSpecialInstruction" Width="100%" class="form-control" Placeholder="Instructions If any" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-4 mb-3">
                                    <asp:Panel ID="pnlVATControls" runat="server" Visible="false">
                                        <label for="ddlVAT">VAT(%)</label>
                                        <asp:DropDownList ID="ddlVAT" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlVAT_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </asp:Panel>
                                </div>


                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12 col-12 padding-0">
                            <div class="table-container">
                                <asp:GridView ID="gvPO" runat="server" class="table table-th table-bordered table-striped table-hover"
                                    AutoGenerateColumns="false" OnRowDataBound="gvPO_RowDataBound" ShowFooter="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="1%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item Description">
                                            <ItemTemplate>
                                                <asp:Label ID="lblItemCode" Text='<%# Eval("ItemCode").ToString()%>' runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblItemID" Text='<%# Eval("ItemID").ToString()%>' runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblVendorQuotationID" Text='<%# Eval("VendorQuotationID").ToString()%>' runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblVendorQuotationDetailsID" Text='<%# Eval("VendorQuotationDetailsID").ToString()%>' runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblEnquiryDetailsID" Text='<%# Eval("EnquiryDetailsID").ToString()%>' runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblPODetails" Text='<%# Eval("PODetailsID").ToString()%>' runat="server" Visible="false"></asp:Label>
                                                <asp:Label ID="lblItemDescription" Text='<%# Eval("ItemDescription").ToString().ToUpper() %>' runat="server" /><br />
                                                <asp:Label ID="lblItemSpecification" Text='<%# Eval("ItemSpecification").ToString().ToUpper() %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delivery Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDeliveryDate" Text='<%#Eval("DeliveryDate", "{0:dd/MMM/yyyy}") %>' runat="server"></asp:Label>
                                                <asp:TextBox ID="txtDeliveryDate" Visible="false" runat="server" CssClass="form-control" Text='<%#Eval("DeliveryDate", "{0:dd/MMM/yyyy}") %>' AutoComplete="false" Font-Size="11px"></asp:TextBox>
                                                <ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" TargetControlID="txtDeliveryDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                                <asp:Label ID="lblAdditionalChargeDescription" Text='<%# Eval("AdditionalChargeDescription").ToString().ToUpper() %>' runat="server" Visible="false" />
                                                <asp:Label ID="lblAdditionalChargesValue" Text='<%# Eval("AdditionalCharges").ToString().ToUpper() %>' runat="server" Visible="false"/>
                                            </ItemTemplate>
                                            <ItemStyle Width="6%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Enq Qty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" Text='<%# Eval("EnqQty").ToString() %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="3%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Unit">
                                            <ItemTemplate>
                                                <asp:Label ID="lblUOMName" Text='<%# Eval("UOMName").ToString() %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="2%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRate" Text='<%# Eval("Rate").ToString() %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="3%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qty">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtQty" runat="server" Text='<%# Eval("Quantity").ToString() %>' OnTextChanged="txtQty_TextChanged" AutoPostBack="true" Width="100%" CssClass="form-control" />
                                                <ajaxToolkit:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" runat="server" FilterType="Numbers, Custom" ValidChars="." TargetControlID="txtQty" />
                                                <asp:HiddenField ID="hdRemainingQty" runat="server" Value='<%# Eval("RemainingQty").ToString() %>' />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="lblAmount" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dis.%">
                                            <ItemTemplate>
                                                <asp:Label ID="lbldiscount" runat="server" Font-Bold="true" />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblSubTotal1" runat="server" Text="SUB-TOTAL" Font-Bold="true"></asp:Label><br />
                                                <asp:Label ID="lblAdditionalCharges1" runat="server" Font-Bold="true"></asp:Label><br />
                                                <asp:Panel ID="pnlVATFooter" runat="server" Visible="false">
                                                    <asp:Label ID="lblVATPercentText" runat="server" Text="VAT %" Font-Bold="true"></asp:Label><br />
                                                    <asp:Label ID="lblVATText" runat="server" Text="VAT" Font-Bold="true"></asp:Label><br />
                                                </asp:Panel>
                                                <asp:Label ID="lblTotal1" runat="server" Text="GRAND TOTAL" Font-Bold="true"></asp:Label>
                                            </FooterTemplate>
                                            <ItemStyle Width="2%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dis. (Amt.)">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDiscountAmount" runat="server" />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <%-- Empty because footer is merged with previous column --%>
                                            </FooterTemplate>

                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Amount">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTotalAmount" runat="server" />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="lblSubtotal" runat="server" Font-Bold="true"></asp:Label><br />
                                                <asp:Label ID="lblAdditionalCharges" runat="server" Font-Bold="true"></asp:Label><br />
                                                <asp:Panel ID="pnlVATFooterValues" runat="server" Visible="false">
                                                    <asp:Label ID="lblVATpercentage" runat="server" Font-Bold="true"></asp:Label><br />
                                                    <asp:Label ID="lblVAT" runat="server" Font-Bold="true"></asp:Label><br />
                                                </asp:Panel>
                                                <asp:Label ID="lblGrandTotal" runat="server" Font-Bold="true"></asp:Label>
                                            </FooterTemplate>

                                            <ItemStyle Width="7%"></ItemStyle>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                    <div class="row clearfix">
                        <div class="col-12">
                            <div class="panel_header">
                                <h3 class="title float-left">TERMS & CONDITIONS</h3>
                            </div>
                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row clearfix">
                        <div class="col-md-6 mb-3">
                            <label for="txtDelivery">Delivery</label>
                            <asp:TextBox ID="txtDelivery" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="col-md-6 mb-3">
                            <label for="txtPayment">Payment</label>
                            <asp:DropDownList ID="ddlPaymentTerms" runat="server" CssClass="form-control"></asp:DropDownList>
                            <%--<asp:TextBox ID="txtPayment" Width="100%" class="form-control" runat="server"></asp:TextBox>--%>
                        </div>
                    </div>
                    <div class="row clearfix">

                        <div class="col-md-6 mb-3">
                            <label for="txtGuarantee">Guarantee/Warranty</label>
                            <asp:TextBox ID="txtGuarantee" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtRemarks">Remarks</label>
                            <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-md-12 text-center mt-4">
                            <asp:Button ID="BtnSave" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="BtnSave_Click" />
                            <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="btn btn-price" OnClick="btnUpdate_Click" Visible="false" />
                        </div>
                    </div>
                </div>
            </div>
            <asp:HiddenField ID="hdPurchaseOrderID" runat="server" />
        </div>
    </div>
</asp:Content>

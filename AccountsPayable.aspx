<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" EnableViewState="true" AutoEventWireup="true" CodeBehind="AccountsPayable.aspx.cs" Inherits="LIMS.AP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-size: 12px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>ACCOUNTS PAYABLE</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <div class="row">
                                     <div class="col-sm-4">
                                        <div class="form-group">
                                            <b>Vendors</b>
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlVendors" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlVendors_SelectedIndexChanged" AutoPostBack="true" ></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>PO List</b>
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlPO" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPO_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <div class="form-group">
                                            <b>Amount</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" Placeholder="Amount" Enabled="false"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <div class="form-group">
                                            <b>TOP</b>
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlPaymentTerms" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-1">
                                        <div class="form-group">
                                            <b>Currency</b>
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <b>GRN List</b>
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlGRN" runat="server" CssClass="form-control"></asp:DropDownList>
                                                <asp:HiddenField ID="hdGRNID" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnAdd" runat="server" Text="+" OnClick="btnAdd_Click" CssClass="btn btn-primary" />
                                        <asp:Button ID="btnClear" runat="server" Text="X" OnClick="btnClear_Click" CssClass="btn btn-primary" />
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>Vendor Tax Invoice Number</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtVendorTaxInvoiceNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>Vendor Invoice Number</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <div class="form-group">
                                            <b>Invoice Date</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtInvoiceDate" runat="server" CssClass="form-control" Placeholder="yyyy-mm-dd" TextMode="Date"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <div class="form-group">
                                            <b>Due Date</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" Placeholder="yyyy-mm-dd" TextMode="Date"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:Button ID="btnAddInvoice" runat="server" CssClass="btn btn-primary" Text="Add Invoice" OnClick="btnAddInvoice_Click" />
                                    </div>
                                </div>
                                <div class="row">
                                    <asp:GridView ID="gvGRN" runat="server" DataKeyNames="InwardID" CssClass="table"
                                        AutoGenerateColumns="false" ShowFooter="True" OnRowDataBound="gvGRN_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="ReceivedDate" HeaderText="GR Dt." DataFormatString="{0:dd-MM-yyyy}" />
                                            <asp:BoundField DataField="GRNNumber" HeaderText="GR No." />
                                            <asp:BoundField DataField="ItemCode" HeaderText="Item Code" />
                                            <asp:BoundField DataField="ItemDescription" HeaderText="Item" />
                                            <asp:BoundField DataField="Quantity" HeaderText="Qty" DataFormatString="{0:N0}" />
                                            <asp:BoundField DataField="UOMName" HeaderText="Measr" />
                                            <asp:TemplateField HeaderText="Price" ItemStyle-HorizontalAlign="Right">
                                                <ItemTemplate>
                                                    <%# ToIDR(Convert.ToDecimal(Eval("rate"))) %>
                                                </ItemTemplate>

                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Disc (%)" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# Eval("Discount", "{0:N0}") %>%   
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# ToIDR(Convert.ToDecimal(Eval("amount"))) %>
                                                </ItemTemplate>

                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:HiddenField ID="hdTotal" runat="server" />
                                </div>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            <div class="card">
                <asp:RadioButtonList ID="radFilter" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radFilter_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Value="1" Selected="True">Invoiced - Unpaid</asp:ListItem>
                    <asp:ListItem Value="2">Invoiced - Paid</asp:ListItem>
                    <asp:ListItem Value="3">UnInvoiced</asp:ListItem>
                </asp:RadioButtonList>
                <asp:GridView ID="gvInvoices" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvInvoices_RowDataBound"
                    DataKeyNames="PurchaseOrderID" OnRowCommand="gvInvoices_RowCommand" AllowSorting="true" OnSorting="gvInvoices_Sorting" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl.No" ItemStyle-Width="70">
                            <ItemTemplate>
                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                            </ItemTemplate>
                            <ItemStyle Width="30px"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="PI#">
                            <ItemTemplate>
                                <asp:HyperLink ID="hypPI" runat="server" Text='<%#Eval("PINumber") %>' NavigateUrl='<%#"generatepurchaseinvoice.aspx?i=" + Eval("InvoiceID") %>' Target="_blank"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PODate" HeaderText="PO Date" SortExpression="PODate" ItemStyle-Width="10%" DataFormatString="{0:dd MMM yyyy}" />
                        <asp:BoundField DataField="PONumber" HeaderText="PO#" SortExpression="PONumber" ItemStyle-Width="20%" />
                        <asp:BoundField DataField="VendorName" HeaderText="Supplier" SortExpression="VendorName" ItemStyle-Width="20%" />
                        <asp:BoundField DataField="GrandTotal" HeaderText="Amount" SortExpression="GrandTotal" ItemStyle-Width="15%" />
                        <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" SortExpression="Status">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkStatus" runat="server" CssClass="badge bg-primary"
                                    Text='<%# Eval("Status") == DBNull.Value ? "UnInvoiced" : "Invoiced" %>'
                                    Style='<%# Eval("Status") == DBNull.Value ? "background-color:red;color:white": "background-color:green;color:white" %>'
                                    CommandName="SelectPO" CommandArgument='<%# Eval("PurchaseOrderID") %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Width="30px"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Paid Amt." ItemStyle-Width="20%" ItemStyle-Font-Bold="true">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPayment" runat="server"
                                    Text='<%#Eval("AmountPaid") %>' CommandName="Payments" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle Width="30px"></ItemStyle>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="InvoiceList.aspx.cs" Inherits="LIMS.InvoiceList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        [type="checkbox"]:not(:checked), [type="checkbox"]:checked {
            position: absolute;
            left: 30px;
            opacity: 100;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <fieldset>--%>
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h2>INVOICE LIST</h2>
                        </div>

                    </div>
                    <br />
                    <div class="row" style="font-size: 12px">
                        <div class="col-sm-1">
                            Invoice No.
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            Customer:  
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtCustomer" runat="server" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ServiceMethod="GetCustomers" MinimumPrefixLength="1"
                                CompletionInterval="10" EnableCaching="false" CompletionSetCount="1" TargetControlID="txtCustomer"
                                ID="AutoCompleteExtender2" runat="server" FirstRowSelected="false">
                            </asp:AutoCompleteExtender>
                        </div>
                        <div class="col-sm-1">Paid:   </div>
                        <div class="col-sm-2">
                            <asp:DropDownList ID="ddlPaid" runat="server" CssClass="form-control" Width="100px">
                                <asp:ListItem>All</asp:ListItem>
                                <asp:ListItem>Yes</asp:ListItem>
                                <asp:ListItem>No</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-1">
                            Request No.  
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtRequestNo" runat="server" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row" style="font-size: 12px">
                        <div class="col-sm-1">
                            Invoice Date >=
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtInvDate1" runat="server" TextMode="Date" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            Invoice Date <=
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtInvDate2" runat="server" TextMode="Date" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            Grand Total >=
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtTotal1" runat="server" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            Grand Total <=
                        </div>
                        <div class="col-sm-2">
                            <asp:TextBox ID="txtTotal2" runat="server" CssClass="form-control" OnTextChanged="Filter" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-4"></div>
                        <div class="col-sm-4" style="text-align: center">
                            <asp:Button ID="btnApplyFilter" runat="server" Text="Apply" CssClass="btn btn-primary" OnClick="btnApplyFilter_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn btn-primary" OnClick="btnClearFilter_Click" />
                            &nbsp;&nbsp;
                            <asp:Button ID="bnCoretax" runat="server" Text="CoreTax" CssClass="btn btn-primary" OnClick="bnCoretax_Click"/>
                        &nbsp;&nbsp;
                            <asp:Button ID="btnPay" runat="server" Text="Pay" CssClass="btn btn-primary" OnClick="btnPay_Click"/>
                        </div>
                        <div class="col-sm-4"></div>
                    </div>
                </div>
                <div class="body">
                    <div class="table-responsive">
                        <asp:Panel ID="pnlView" runat="server">
                            <b>Record(s): <span id="spanCount" runat="server">0</span></b>
                            <asp:Button ID="btnMerge" runat="server" Text="Merge Files" OnClick="btnMerge_Click" CssClass="btn btn-primary" />
                            <asp:GridView ID="gvInvoices"
                                runat="server"
                                AutoGenerateColumns="false"        
                                AllowSorting="true"
                                OnSorting="gvInvoices_Sorting"
                                DataKeyNames="InvoiceID,BalanceDue,InvoiceDate"
                                class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                <Columns>
                                    <asp:TemplateField Visible="true">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                            <asp:HiddenField ID="hdQuotationID" runat="server" Value='<%#Eval("QuotationID") %>' />
                                            <asp:HiddenField ID="hdInvoiceID" runat="server" Value='<%#Eval("InvoiceID") %>' />
                                            <asp:HiddenField ID="hdCompanyID" runat="server" Value='<%#Eval("CompanyID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="CompanyName" SortExpression="CompanyName" HeaderText="Customer" HeaderStyle-Width="20%" />
                                    <asp:TemplateField HeaderText="InvoiceNumber" SortExpression="InvoiceNumber">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hypInvoice" runat="server" Text='<%# Eval("InvoiceNumber") %>' NavigateUrl='<%#"Invoice/INV-" + Eval("QuotationID") + ".htm" %>' Target="_blank"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="InvoiceDate" SortExpression="InvoiceDate" HeaderText="Invoice Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                    <asp:BoundField DataField="DueDate" SortExpression="DueDate" HeaderText="Due Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                    <asp:TemplateField HeaderText="Currency" SortExpression="CurrencyCode">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCurrencyCode" runat="server" Text='<%#Eval("CurrencyCode") %>'></asp:Label>
                                            <asp:Label ID="lblCurrencyID" runat="server" Text='<%#Eval("CurrencyID") %>' Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TotalAmount" SortExpression="TotalAmount" HeaderText="Total Amount" />
                                    <asp:BoundField DataField="TotalAmount" SortExpression="TotalAmount" HeaderText="Converted" />
                                    <asp:BoundField DataField="AmountPaid" SortExpression="AmountPaid" HeaderText="Amount Paid" />
                                    <asp:BoundField DataField="BalanceDue" SortExpression="BalanceDue" HeaderText="Balance Amt." />
                                    <asp:BoundField DataField="RequestNo" SortExpression="RequestNo" HeaderText="Request#" />
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </div>
                </div>
            </div>
            <%-- </fieldset>
                </ContentTemplate>
            </asp:UpdatePanel>--%>
        </div>
    </div>

</asp:Content>

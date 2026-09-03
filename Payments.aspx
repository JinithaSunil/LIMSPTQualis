<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Payments.aspx.cs" Inherits="LIMS.Payments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>PAYMENTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-3">
                            <b>Payment Date</b>
                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-sm-9">
                            <b>Reference</b>
                            <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" Placeholder="Reference"></asp:TextBox>
                            <asp:HiddenField ID="hdPOID" runat="server" />
                            <asp:HiddenField ID="hdInvoiceID" runat="server" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-3">
                            <b>Paid From:</b>
                            <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:HiddenField ID="hdPaymentMode" runat="server" />
                            <asp:HiddenField ID="hdAccountCode" runat="server" />
                            <asp:HiddenField ID="hdAccountID" runat="server" />
                        </div>
                        <div class="col-sm-3">
                            <b>Account</b>
                            <asp:DropDownList ID="ddlCOAAccount" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCOAAccount_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:DropDownList ID="ddlCOA" runat="server" CssClass="form-control" Visible="false"></asp:DropDownList>
                            <asp:HiddenField ID="hdCOAAccountID" runat="server" />
                            <asp:HiddenField ID="hdAccountCategoryID" runat="server" />
                        </div>
                        <div class="col-sm-3">
                            <b>Payee Type</b>
                            <asp:DropDownList ID="ddlPayeeType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPayeeType_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem>Employee</asp:ListItem>
                                <asp:ListItem>Customer</asp:ListItem>
                                <asp:ListItem Selected="True">Vendor</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-3">
                            <b>Payee</b>
                            <asp:DropDownList ID="ddlPayee" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:HiddenField ID="hdPayeeID" runat="server" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-3">
                            <b>PO Amount</b>
                            <asp:TextBox ID="txtPOAmount" runat="server" CssClass="form-control" Enabled="false" Placeholder="Amount"></asp:TextBox>
                            <asp:Label ID="lblPONumber" runat="server" Visible="false"></asp:Label>
                            <asp:Label ID="lblSupplierInvoiceNo" runat="server" Visible="false"></asp:Label>
                        </div>
                        <div class="col-sm-3">
                            <b>Amount Paid</b>
                            <asp:TextBox ID="txtAmountPaid" runat="server" CssClass="form-control" Enabled="false" Placeholder="Amount Paid"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <b>Pay Amount</b>
                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" Placeholder="Amount" OnTextChanged="txtAmount_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <b>Balance Amount</b>
                            <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" Enabled="false" Placeholder="Balance Amount"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6">
                            <b>Payment Mode</b>
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <asp:RadioButtonList ID="radPaymentMode" runat="server" RepeatDirection="Horizontal" CssClass="form-check" OnSelectedIndexChanged="radPaymentMode_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem>CASH</asp:ListItem>
                                            <asp:ListItem>CHEQUE</asp:ListItem>
                                            <asp:ListItem>BANK</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblCheque" runat="server" visible="false" class="table" style="text-align: center">
                                <tr>
                                    <td>
                                        <b>Bank / Account#</b>
                                        <asp:TextBox ID="txtBankAccount" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                               
                                    <td>
                                        <b>Cheque#</b>
                                        <asp:TextBox ID="txtChequeNo" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                               
                                    <td>
                                        <b>Cheque Dt.</b><asp:TextBox ID="txtChequeDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <table id="tblBank" runat="server" visible="false" class="table" style="text-align: center">
                                <tr>
                                    <td>
                                        <b>Bank Name</b>
                                        <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                                    <td>
                                        <b>Bank / Account#</b>
                                        <asp:TextBox ID="txtBankAccount1" runat="server" CssClass="form-control"></asp:TextBox>
                                    </td>
                                    <td>
                                        <b>Transfer Dt.</b><asp:TextBox ID="txtTransferDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <br />

                        </div>
                      
                        <div class="col-sm-6">
                            <b>Notes</b>
                            <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <br />
                            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add Payment" OnClick="btnAdd_Click" />
                        </div>
                    </div>
                </div>
                <br />
                <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="false" Font-Size="12px" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                    <Columns>
                        <asp:TemplateField HeaderText="Voucher#">
                            <ItemTemplate>
                                <asp:HyperLink ID="hypVoucher" runat="server" NavigateUrl='<%# "generateVoucher.aspx?o=1&x=" + Eval("paymentID") %>' Text='<%#Eval("VoucherNumber")%>' Target="_blank"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PaymentDate" HeaderText="Date" DataFormatString="{0:dd.MM.yyyy}" />
                        <asp:BoundField DataField="PayeeType" HeaderText="Payee Type" />
                        <asp:BoundField DataField="AccountName" HeaderText="Paid From" />
                        <asp:BoundField DataField="ReferenceNo" HeaderText="Description" />
                        <asp:BoundField DataField="PayeeName" HeaderText="Payee" />
                        <asp:BoundField DataField="PaymentAmount" HeaderText="Amount" />

                    </Columns>
                </asp:GridView>

            </div>
        </div>
    </div>

</asp:Content>

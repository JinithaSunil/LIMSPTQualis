<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Receipts.aspx.cs" Inherits="LIMS.Receipts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <%--    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>--%>
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>RECEIPTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-3">
                            <b>Receipt Date</b>
                            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-sm-9">
                            <b>Reference</b>
                            <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" Placeholder="Reference"></asp:TextBox>
                            <asp:HiddenField ID="hdInvoiceID" runat="server" />
                            <asp:HiddenField ID="hdInvoiceARID" runat="server" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-3">
                            <b>Received In:</b>
                            <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlAccount_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            <asp:HiddenField ID="hdAccountCode" runat="server" />
                            <asp:HiddenField ID="hdAccountID" runat="server" />
                        </div>
                        <div class="col-sm-3">
                            <b>Account</b>
                            <asp:DropDownList ID="ddlCOA" runat="server" CssClass="form-control"></asp:DropDownList>
                            <asp:HiddenField ID="hdCOAAccountID" runat="server" />
                            <asp:HiddenField ID="hdAccountCategoryID" runat="server" />
                        </div>
                        <div class="col-sm-3">
                            <b>Payee Type</b>
                            <asp:DropDownList ID="ddlPayeeType" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPayeeType_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem>Other</asp:ListItem>
                                <asp:ListItem>Customer</asp:ListItem>
                                <asp:ListItem>Vendor</asp:ListItem>
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
                            <b>Invoice Amount</b>
                            <asp:TextBox ID="txtPOAmount" runat="server" CssClass="form-control" Enabled="false" Placeholder="Invoice Amount"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <b>Amount Received</b>
                            <asp:TextBox ID="txtAmountPaid" runat="server" CssClass="form-control" Enabled="false" Placeholder="Amount Paid"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <b>Receive Amount</b>
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
                        <div class="col-sm-2">
                            <br />
                            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary" Text="Add Payment" OnClick="btnAdd_Click" />
                        </div>
                    </div>
                </div>
                <br />
                <asp:GridView ID="gvReceipts" runat="server" AutoGenerateColumns="false" Font-Size="12px" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                    <Columns>
                        <asp:TemplateField HeaderText="Voucher#">
                            <ItemTemplate>
                                <asp:HyperLink ID="hypVoucher" runat="server" NavigateUrl="#" Text='<%#Eval("VoucherNumber")%>' Target="_blank"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ReceiptDate" HeaderText="Date" DataFormatString="{0:dd.MM.yyyy}" />
                        <asp:BoundField DataField="PayerType" HeaderText="Payee Type" />
                        <asp:BoundField DataField="ReferenceNo" HeaderText="Description" />
                        <asp:BoundField DataField="PayerName" HeaderText="Payee" />
                        <asp:BoundField DataField="ReceiptAmount" HeaderText="Amount" />

                    </Columns>
                </asp:GridView>

            </div>
        </div>
    </div>
    <%--</fieldset>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAdd" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>--%>
</asp:Content>

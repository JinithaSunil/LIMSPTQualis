<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" Async="true" AutoEventWireup="true" CodeBehind="ReceiptAdvance.aspx.cs" Inherits="LIMS.ReceiptAdvance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .cb label {
            height: 43px;
        }

        .rbWidth label {
            height: 40px;
        }

        [type="radio"]:not(:checked) + label {
            padding-left: 26px;
            height: 43px;
            line-height: 25px;
            font-size: 13px;
            font-weight: normal;
        }

        [type="radio"]:checked + label {
            padding-left: 26px;
            height: 43px;
            line-height: 25px;
            font-size: 13px;
            font-weight: normal;
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
                            <h2>ADVANCE RECEIPT</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-12">
                            <%--                            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <fieldset>--%>
                            <div class="row">
                                <div class="col-sm-3">
                                    <b>Receipt Date</b>
                                    <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-sm-9">
                                    <b>Reference</b>
                                    <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" Placeholder="Reference" Enabled="false"></asp:TextBox>
                                    <asp:HiddenField ID="hdQuotationID" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Customers</b>
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlCustomers" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlCustomers_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Quotation List</b>
                                        <asp:DropDownList ID="ddlQuotation" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlQuotation_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Currency</b>
                                        <div class="form-line">
                                            <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control">
                                                <asp:ListItem>IDR</asp:ListItem>
                                                <asp:ListItem>USD</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <b>Amount</b>
                                        <div class="form-line">
                                            <asp:TextBox ID="txtAmount" runat="server" CssClass="form-control" Placeholder="Amount"></asp:TextBox>
                                        </div>
                                    </div>
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
                                    <div class="form-group">
                                        <b>Advance Amount</b>
                                        <div class="form-line">
                                            <asp:TextBox ID="txtAdvanceAmount" runat="server" CssClass="form-control" Placeholder="Amount"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
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
                            </div>
                            <div class="row">
                                <div class="col-sm-6">
                                    <b>Notes</b>
                                    <asp:TextBox ID="txtNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4"></asp:TextBox>
                                </div>
                                <div class="col-sm-3">
                                    <asp:Button ID="btnSaveAdvance" runat="server" CssClass="btn btn-primary" Text="Save Advance" OnClick="btnSaveAdvance_Click" />
                                </div>
                            </div>

                        </div>
                        <asp:HiddenField ID="hdInvoiceID" runat="server" />
                        <asp:HiddenField ID="hdAmountPaid" runat="server" />
                        <br />
                        <%--   </fieldset>
                                </ContentTemplate>
                            </asp:UpdatePanel>--%>
                    </div>
                </div>

            </div>
        </div>
    </div>
    </div>
</asp:Content>

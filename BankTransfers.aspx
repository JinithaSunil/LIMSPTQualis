<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="BankTransfers.aspx.cs" Inherits="LIMS.BankTransfers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <!-- Basic Examples -->
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <h2>BANK TRANSFERS</h2>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="body">
                                <div class="row">

                                    <!-- Filters -->
                                    <table class="filterTable" style="text-align: center; font-weight: bold">
                                        <tr>
                                            <td>From Date</td>
                                            <td>To Date</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control" /></td>                                          
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server"
                                                    Text="Search" CssClass="btn btn-primary"
                                                    OnClick="btnSearch_Click" />
                                            </td>
                                        </tr>
                                    </table>

                                    <br />
                                    <asp:RadioButtonList ID="radChoice" runat="server" OnSelectedIndexChanged="radChoice_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="RECEIPT" Selected="True">Receipt</asp:ListItem>
                                        <asp:ListItem Value="PAYMENT">Payment</asp:ListItem>
                                        
                                    </asp:RadioButtonList>
                                    <!-- Cheque Register Grid -->
                                </div>
                                <div class="row">
                                    <asp:GridView ID="gvBankTransfers" runat="server" DataKeyNames="VoucherID" AutoGenerateColumns="false" CssClass="table">
                                        <Columns>
                                            <asp:BoundField DataField="VoucherNo" HeaderText="Voucher" />
                                            <asp:BoundField DataField="PartyName" HeaderText="Party" />
                                            <asp:BoundField DataField="BankName" HeaderText="Bank Name" />
                                            <asp:BoundField DataField="BankAccountID" HeaderText="Account#" />
                                            <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                            <asp:BoundField DataField="TransferDate" HeaderText="Transfer Dt." DataFormatString="{0:dd.MM.yyyy}" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

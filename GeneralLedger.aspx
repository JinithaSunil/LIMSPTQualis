<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GeneralLedger.aspx.cs" Inherits="LIMS.GeneralLedger" %>

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
                                        <h2>GENERAL LEDGER</h2>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-3">
                                        <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-3">
                                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                     <div class="col-sm-3">
                                         <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary" />
                                         </div>
                                </div>
                            </div>
                            <div class="body">
                                <div class="row">
                                    Opening Balance: <asp:Label ID="lblOpeningBalance" runat="server"></asp:Label>
                                </div>
                                <div class="row">
                                    <asp:GridView ID="gvGeneralLedger" runat="server"
                                        AutoGenerateColumns="False" CssClass="table">
                                        <Columns>
                                            <asp:BoundField DataField="VoucherDate" HeaderText="Date" />
                                            <asp:BoundField DataField="VoucherNo" HeaderText="Voucher No" />
                                            <asp:BoundField DataField="VoucherType" HeaderText="Type" />
                                            <asp:BoundField DataField="Narration" HeaderText="Narration" />

                                            <asp:BoundField DataField="DebitAmount" HeaderText="Debit"
                                                ItemStyle-HorizontalAlign="Right" />

                                            <asp:BoundField DataField="CreditAmount" HeaderText="Credit"
                                                ItemStyle-HorizontalAlign="Right" />

                                            <asp:BoundField DataField="RunningBalance" HeaderText="Balance"
                                                ItemStyle-HorizontalAlign="Right" />
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

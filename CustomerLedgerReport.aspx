<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="CustomerLedgerReport.aspx.cs" Inherits="LIMS.CustomerLedgerReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #header {
            display: none;
        }

        @media print {
            #divSearch,
            #btnPrint,
            #footer {
                display: none !important;
            }

            #header {
                display: block;
                font-family: "Courier New";
                margin-bottom: 10px;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>CUSTOMER LEDGER</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div id="divSearch" class="row">
                        <div class="col-md-4">
                            <label>Customer</label>
                            <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>

                        <div class="col-md-2">
                            <label>From Date</label>
                            <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <label>To Date</label>
                            <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <br />
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success w-100" OnClick="btnSearch_Click" />
                        </div>
                        <div class="col-md-2">
                            <br />
                            <button onclick="window.print()" class="btn btn-primary">Print Ledger</button>
                        </div>
                    </div>
                    <div id="header" class="row">

                        <div style="font-family: Courier New; font-size: 13px">
                            Customer :
                            <asp:Label ID="lblCustomer" runat="server"></asp:Label><br />
                            From Date :
                            <asp:Label ID="lblFrom" runat="server"></asp:Label>
                            Thru Date :
                            <asp:Label ID="lblTo" runat="server"></asp:Label>

                            <hr />
                        </div>
                    </div>
                    <div class="row">
                        <asp:GridView ID="gvLedger" runat="server"
                            AutoGenerateColumns="false"
                            CssClass="table"
                            ShowFooter="true" OnRowDataBound="gvLedger_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="DocType" HeaderText="Ty" />

                                <asp:BoundField DataField="DocNo" HeaderText="Number" />

                                <asp:BoundField DataField="Description" HeaderText="Document Description" />

                                <asp:BoundField DataField="TransactionDate" HeaderText="Invoice Date"
                                    DataFormatString="{0:dd/MM/yy}" />

                                <asp:BoundField DataField="Debit" HeaderText="Debit Amount"
                                    DataFormatString="{0:N2}" />

                                <asp:BoundField DataField="Credit" HeaderText="Credit Amount"
                                    DataFormatString="{0:N2}" />

                                <asp:BoundField DataField="Balance" HeaderText="Balance"
                                    DataFormatString="{0:N2}" />

                            </Columns>

                        </asp:GridView>
                    </div>


                </div>
            </div>
        </div>
    </div>
</asp:Content>

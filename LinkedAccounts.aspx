<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="LinkedAccounts.aspx.cs" Inherits="LIMS.LinkedAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card {
            background: #fff;
            border: 1px solid #ccc;
            margin-bottom: 20px;
            border-radius: 4px;
        }

            .card h3 {
                margin-top: 0;
                background: #3f6fa5;
                color: #fff;
                padding: 8px;
                font-size: 15px;
            }

        .row {
            display: flex;
            margin-bottom: 10px;
        }

            .row label {
                width: 320px;
                font-size: 13px;
            }

            .row select {
                width: 180px;
            }

        .checkbox-row {
            margin: 8px 0;
        }

        .actions {
            text-align: right;
            margin-top: 15px;
        }

        .btn {
            padding: 6px 14px;
            background: #3f6fa5;
            color: white;
            border: none;
            cursor: pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet" style="margin-top:0px">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>LINKED ACCOUNTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-6" style="vertical-align:top">
                            <!-- SALES LINKED ACCOUNTS -->
                            <div class="card">
                                <div class="header bg-violet">
                                    <h2 style="color: white !important">Sales Linked Accounts</h2>
                                </div>
                                <div class="body">

                                    <div class="row" style="align-items:start !important">
                                        <label>Asset Account for Tracking Receivables</label>
                                        <asp:DropDownList ID="ddlSalesReceivable" runat="server" CssClass="form-control" Width="300" />
                                    </div>

                                    <div class="row">
                                        <label>Bank Account for Customer Receipts</label>
                                        <asp:DropDownList ID="ddlSalesBank" runat="server" CssClass="form-control" Width="300" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkSalesFreight" runat="server" Text=" I charge freight on sales" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkSalesDeposit" runat="server"
                                            Text=" I track deposits collected from customers"
                                            AutoPostBack="true"
                                            OnCheckedChanged="chkSalesDeposit_CheckedChanged" />
                                    </div>

                                    <div class="row">
                                        <label>Liability Account for Customer Deposits</label>
                                        <asp:DropDownList ID="ddlCustomerDeposit" runat="server" Enabled="false" CssClass="form-control" Width="300" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkSalesDiscount" runat="server" Text=" I give discounts for early payment" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkSalesLateFee" runat="server" Text=" I assess charges for late payment" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-sm-6">
                            <!-- PURCHASE LINKED ACCOUNTS -->
                            <div class="card">
                                <div class="header bg-violet">
                                    <h2 style="color: white !important">Purchases Linked Accounts</h2>
                                </div>
                                <div class="body">

                                    <div class="row">
                                        <label>Liability Account for Tracking Payables</label>
                                        <asp:DropDownList ID="ddlPurchasePayable" runat="server" CssClass="form-control" Width="250" />
                                    </div>

                                    <div class="row">
                                        <label>Bank Account for Paying Bills</label>
                                        <asp:DropDownList ID="ddlPurchaseBank" runat="server" CssClass="form-control" Width="250" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkItemReceipt" runat="server"
                                            Text=" I can receive items without a supplier bill"
                                            AutoPostBack="true"
                                            OnCheckedChanged="chkItemReceipt_CheckedChanged" />
                                    </div>

                                    <div class="row">
                                        <label>Liability Account for Item Receipts</label>
                                        <asp:DropDownList ID="ddlItemReceipt" runat="server" Enabled="false" CssClass="form-control" Width="250" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkPurchaseFreight" runat="server" Text=" I pay freight on purchases" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkSupplierDeposit" runat="server"
                                            Text=" I track deposits paid to suppliers"
                                            AutoPostBack="true"
                                            OnCheckedChanged="chkSupplierDeposit_CheckedChanged" />
                                    </div>

                                    <div class="row">
                                        <label>Asset Account for Supplier Deposits</label>
                                        <asp:DropDownList ID="ddlSupplierDeposit" runat="server" Enabled="false" CssClass="form-control" Width="250" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkPurchaseDiscount" runat="server" Text=" I take discounts for early payment" />
                                    </div>

                                    <div class="checkbox-row">
                                        <asp:CheckBox ID="chkPurchaseLateFee" runat="server" Text=" I pay charges for late payment" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row" style="text-align:center">
                        <div class="col-sm-12 actions" style="text-align:center">
                            <asp:Button ID="btnSave" runat="server" Text="Update" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

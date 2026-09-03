<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Menus.aspx.cs" Inherits="LIMS.Menus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">

    <style>
        body {
            background: #f5f7fb;
            font-family: "Segoe UI", sans-serif;
        }

        .dashboard {
            max-width: 1200px;
            margin: auto;
        }

        .section {
            margin-bottom: 28px;
        }

        .section-header {
            display: flex;
            align-items: center;
            font-size: 18px;
            font-weight: 600;
            color: #2c3e50;
        }

            .section-header i {
                margin-right: 8px;
                color: #4a73ff;
            }

            .section-header.green i {
                color: #2ecc71;
            }

            .section-header.blue i {
                color: #3498db;
            }

            .section-header.teal i {
                color: #1abc9c;
            }

        .divider {
            height: 1px;
            background: #dfe3eb;
            margin: 8px 0 16px;
        }

        .grid {
            display: grid;
            grid-template-columns: repeat(5, 1fr);
            gap: 14px;
        }

        .grid2 {
            display: grid;
            grid-template-columns: repeat(2, 1fr);
            gap: 14px;
        }

        .grid3 {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 14px;
        }

        .grid4 {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 14px;
        }

        .card1 {
            background: #fff;
            border: 1px solid #e1e6ef;
            border-radius: 8px;
            padding: 14px;
            display: flex;
            align-items: center;
            font-size: 14px;
            font-weight: 500;
            color: #2c3e50;
            cursor: pointer;
            transition: all .2s ease;
        }

            .card1 i {
                margin-right: 10px;
                font-size: 18px;
                color: #b34788;
            }

            .card1:hover {
                box-shadow: 0 6px 14px rgba(0,0,0,.08);
                transform: translateY(-2px);
                background-color: #d4a6c2;
            }
    </style>
    <script>
        document.addEventListener("DOMContentLoaded", () => {
            const role = document.body.dataset.userRole;

            document.querySelectorAll(".shortcut-btn").forEach(btn => {
                const roles = btn.dataset.roles.split(",");
                if (!roles.includes(role)) {
                    btn.style.display = "none";
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="page-wrapper">
        <div class="page-content">

            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">MENU LIST</h2>
                </div>
                <div class="body">

                    <div class="row clearfix">
                        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Accounting -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-book"></i>
                                        <span>ACCOUNTING</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid4">
                                        <div class="card1"><a href="#"><i class="fa fa-book-open"></i>General Ledger</a></div>
                                        <div class="card1"><a href="COA.aspx" target="_blank"><i class="fa fa-chart-pie"></i>Chart of Accounts</a></div>
                                        <div class="card1"><a href="JournalEntries.aspx" target="_blank"><i class="fa fa-pen"></i>Journal Entries</a></div>
                                        <div class="card1"><a href="#" target="_blank"><i class="fa fa-wallet"></i>Opening Balances</a></div>
                                    </div>
                                    <br />
                                    <div class="grid4">
                                        <div class="card1"><a href="#" target="_blank"><i class="fa fa-scale-balanced"></i>Trial Balance</a></div>
                                        <div class="card1"><a href="RevenueMapping.aspx" target="_blank"><i class="fa fa-chart-line"></i>Revenue Mapping</a></div>
                                        <div class="card1"><a href="LinkedAccounts.aspx" target="_blank"><i class="fa fa-right-left"></i>Linked Accounts</a></div>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <!-- Receivables & Payables -->
                            <div class="card" style="padding: 10px">
                                <br />
                                <div class="section">
                                    <div class="section-header green">
                                        <i class="fa fa-receipt"></i>
                                        <span>SALES & RECEIVABLES</span>
                                    </div>
                                    <div class="divider"></div>
                                    <div class="grid4">
                                        <div class="card1"><a href="InvoiceList.aspx" target="_blank"><i class="fa fa-file-invoice-dollar"></i>Sales Invoices</a></div>
                                        <div class="card1"><a href="DebitNote.aspx" target="_blank"><i class="fa fa-file-circle-minus"></i>Debit Note</a></div>
                                        <div class="card1"><a href="ReceiptAdvance.aspx" target="_blank"><i class="fa fa-file-invoice-dollar"></i>Customer Advance</a></div>
                                    </div><br />
                                    <div class="grid4">
                                        <div class="card1"><a href="AccountsReceivable.aspx" target="_blank"><i class="fa fa-angles-down"></i>Accounts Receivable</a></div>
                                        <div class="card1"><a href="Receipts.aspx" target="_blank"><i class="fa fa-cash-register"></i>Receipts</a></div>
                                        <div class="card1"><a href="CustomerList.aspx" target="_blank"><i class="fa fa-users"></i>Customer List</a></div>
                                        <div class="card1"><a href="CustomerLedgerReport.aspx" target="_blank"><i class="fa fa-file"></i>Customer Ledger</a></div>
                                    </div>
                                    <br />

                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Invoicing & Sales -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-file-invoice"></i>
                                        <span>PURCHASE & PAYABLES</span>
                                    </div>
                                    <div class="divider"></div>
                                    <div class="grid4">
                                        <div class="card1"><a href="AccountsPayable.aspx" target="_blank"><i class="fa fa-angles-up"></i>Accounts Payable</a></div>
                                         <div class="card1"><a href="#" target="_blank"><i class="fa fa-file-lines"></i>Vendor Invoices</a></div>
                                        <div class="card1"><a href="Payments.aspx" target="_blank"><i class="fa fa-money-bill"></i>Vendor Payments</a></div>
                                        <div class="card1"><a href="#" target="_blank"><i class="fa fa-file"></i>Vendor Ledger</a></div>
                                    </div>
                                    <br />
                                    <div class="grid2">
                                        <div class="card1"><a href="PaymentRequisitionList.aspx" target="_blank"><i class="fa fa-cash-register"></i>Payment Requisition</a></div>
                                        <div class="card1"><a href="PaymentRequisitionList.aspx" target="_blank"><i class="fa fa-cash-register"></i>Payment Requisition Approval</a></div>
                                    </div>

                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Banking & Cash -->
                                <div class="section">
                                    <div class="section-header blue">
                                        <i class="fa fa-university"></i>
                                        <span>BANKING & CASH</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid4">
                                        <div class="card1"><a href="BankAndCashAccounts.aspx" target="_blank"><i class="fa fa-building-columns"></i>Bank & Cash Mgmt</a></div>
                                        <div class="card1"><a href="Banking.aspx" target="_blank"><i class="fa fa-credit-card"></i>Banking</a></div>
                                        <div class="card1"><a href="ChequeRegister.aspx" target="_blank"><i class="fa fa-money-check"></i>Cheque Register</a></div>
                                        <div class="card1"><a href="BankTransfers.aspx" target="_blank"><i class="fa fa-money-check"></i>Bank Transfers</a></div>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Purchase & Inventory -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-box"></i>
                                        <span>PURCHASE & INVENTORY</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid4">
                                        <div class="card1"><a href="PurchaseOrderList.aspx" target="_blank"><i class="fa fa-cart-shopping"></i>Purchase Order List</a></div>
                                        <div class="card1"><a href="PurchaseOrderListPending.aspx" target="_blank"><i class="fa fa-clock"></i>Pending PO List</a></div>
                                        <div class="card1"><a href="GRNShowforApproved.aspx" target="_blank"><i class="fa fa-truck"></i>GRN Approval List</a></div>
                                        <div class="card1"><a href="GRNGenerateShow.aspx" target="_blank"><i class="fa fa-truck"></i>GRN List</a></div>
                                       
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Vendor Management -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-briefcase"></i>
                                        <span>Vendor Management</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid4">
                                        <div class="card1"><a href="VendorList.aspx" target="_blank"><i class="fa fa-user-gear"></i>Vendor Manager</a></div>
                                        <div class="card1"><a href="VendorNew.aspx" target="_blank"><i class="fa fa-user-edit"></i>Add Vendor</a></div>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Reports -->
                                <div class="section">
                                    <div class="section-header teal">
                                        <i class="fa fa-chart-column"></i>
                                        <span>REPORTS & MANAGEMENT</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid2">
                                        <div class="card1"><a href="#" target="_blank"><i class="fa fa-chart-bar"></i>Reports</a></div>
                                        <div class="card1"><a href="ManagementReportList.aspx" target="_blank"><i class="fa fa-chart-pie"></i>Management Report</a></div>
                                    </div>
                                    <br />
                                    <div class="grid2">
                                        <div class="card1"><a href="ReportPaymentRequisitionList.aspx" target="_blank"><i class="fa fa-chart-bar"></i>Payment Requisition Report</a></div>
                                        <div class="card1"><a href="ReportPurchaseRequisitionList.aspx" target="_blank"><i class="fa fa-chart-bar"></i>Purchase Requisition Report</a></div>
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MenusSales.aspx.cs" Inherits="LIMS.MenusSales" %>

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
                                        <span>PRE-SALES</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="CustomerAdd.aspx" target="_blank"><i class="fa fa-user"></i>New Customer</a></div>
                                        <div class="card1"><a href="CustomerCompanyView.aspx" target="_blank"><i class="fa fa-users"></i>Search Customers</a></div>
                                        <div class="card1"><a href="customerInquiry.aspx" target="_blank"><i class="fa fa-question-circle"></i>New Enquiry</a></div>
                                        <div class="card1"><a href="CustomerInquiryView.aspx" target="_blank"><i class="fa fa-file-lines"></i>View Enquiries</a></div>
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
                                        <span>QUOTATIONS</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="QuotationList.aspx" target="_blank"><i class="fa fa-eye"></i>View Quotation</a></div>
                                        <!--<div class="card1"><a href="QuotationApproval.aspx" target="_blank"><i class="fa fa-square-check"></i>Quotation Approval</a></div>-->

                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Invoicing & Sales -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-file-invoice-dollar"></i>
                                        <span>INVOICING</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="InvoiceList.aspx" target="_blank"><i class="fa fa-file-invoice-dollar"></i>Sales Invoices</a></div>
                                        <div class="card1"><a href="DebitNote.aspx" target="_blank"><i class="fa fa-file-circle-minus"></i>Debit Note</a></div>
                                        <div class="card1"><a href="ReceiptAdvance.aspx" target="_blank"><i class="fa fa-file-invoice-dollar"></i>Customer Advance</a></div>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Invoicing & Sales -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-folder"></i>
                                        <span>SAMPLE STATUS</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="AllSamples.aspx" target="_blank"><i class="fa fa-folder"></i>All Samples</a></div>
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

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MenusPurchase.aspx.cs" Inherits="LIMS.MenusPurchase" %>

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
            padding: 5px;
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


                            <div class="row">
                                <div class="col-md-12">
                                    <div class="card" style="padding: 10px">
                                        <br />
                                        <div class="section">
                                            <div class="section-header green">
                                                <i class="fa fa-receipt"></i>
                                                <span>PURCHASE REQUISITION (PR)</span>
                                            </div>
                                            <div class="divider"></div>

                                            <div class="grid3">
                                                <div class="card1"><a href="PurchaseRequisitionForm.aspx" target="_blank"><i class="fa fa-plus"></i>New PR</a></div>
                                                <div class="card1"><a href="PRList.aspx" target="_blank"><i class="fa fa-file-lines"></i>List PR</a></div>
                                                <div class="card1"><a href="PRToPOTracking.aspx" target="_blank"><i class="fa fa-file-lines"></i>PR-PO-GRN Tracker</a></div>

                                            </div>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="card" style="padding: 10px">
                                        <br />
                                        <!-- Invoicing & Sales -->
                                        <div class="section">
                                            <div class="section-header">
                                                <i class="fa fa-file-invoice"></i>
                                                <span>ENQUIRY MANAGEMENT</span>
                                            </div>
                                            <div class="divider"></div>

                                            <div class="grid3">
                                                <div class="card1"><a href="FloatEnquiry.aspx" target="_blank"><i class="fa fa-paper-plane" style="font-size: 16px"></i>Float Enquiry</a></div>
                                                <div class="card1"><a href="EnquiryList.aspx" target="_blank"><i class="fa fa-file-lines"></i>Enquiry List</a></div>
                                                <div class="card1"><a href="VendorQuotationManagement.aspx" target="_blank"><i class="fa fa-file-lines"></i>Vendor Quotation</a></div>
                                            </div>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="card" style="padding: 10px">
                                        <br />
                                        <!-- Banking & Cash -->
                                        <div class="section">
                                            <div class="section-header blue">
                                                <i class="fa fa-briefcase"></i>
                                                <span>VENDOR MANAGEMENT</span>
                                            </div>
                                            <div class="divider"></div>

                                            <div class="grid3">
                                                <div class="card1"><a href="GenrateCode.aspx" target="_blank"><i class="fa fa-building-columns"></i>Register Vendor</a></div>
                                                <div class="card1"><a href="VendorNew.aspx" target="_blank"><i class="fa fa-money-check"></i>New Vendor</a></div>
                                                <div class="card1"><a href="VendorList.aspx" target="_blank"><i class="fa fa-money-check"></i>Vendor List</a></div>
                                            </div>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="card" style="padding: 10px">
                                        <br />
                                        <!-- Invoicing & Sales -->
                                        <div class="section">
                                            <div class="section-header">
                                                <i class="fa fa-file-invoice"></i>
                                                <span>PURCHASE ORDER</span>
                                            </div>
                                            <div class="divider"></div>

                                            <div class="grid3">
                                                <div class="card1"><a href="POMasterList.aspx" target="_blank"><i class="fa fa-check"></i>PO Approval</a></div>
                                                <div class="card1"><a href="PurchaseOrderList.aspx" target="_blank"><i class="fa fa-file-lines"></i>PO List</a></div>
                                                <div class="card1"><a href="PurchaseOrderListPending.aspx" target="_blank"><i class="fa fa-file-lines"></i>Pending PO</a></div>
                                            </div>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="card" style="padding: 10px">
                                        <br />
                                        <!-- Purchase & Inventory -->
                                        <div class="section">
                                            <div class="section-header">
                                                <i class="fa fa-box"></i>
                                                <span>INVENTORY MANAGEMENT</span>
                                            </div>
                                            <div class="divider"></div>

                                            <div class="grid">
                                                <div class="card1"><a href="PurchaseItemAdd.aspx" target="_blank"><i class="fa fa-plus"></i>Add Item</a></div>
                                                <div class="card1"><a href="PurchaseItemList.aspx" target="_blank"><i class="fa fa-file-lines"></i>Item Master</a></div>
                                                <div class="card1"><a href="AddPurchaseGroup.aspx" target="_blank"><i class="fa fa-clock"></i>Add Grp/Sub Grp</a></div>
                                                <div class="card1"><a href="ItemInward.aspx" target="_blank"><i class="fa fa-angles-down"></i>Item Inward</a></div>
                                                <div class="card1"><a href="GRNGenerateShow.aspx" target="_blank"><i class="fa fa-file-lines"></i>GRN List</a></div>
                                            </div>
                                        </div>
                                        <br />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
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
        </div>
    </div>
</asp:Content>

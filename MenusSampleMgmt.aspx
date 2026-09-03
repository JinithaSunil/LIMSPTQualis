<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MenusSampleMgmt.aspx.cs" Inherits="LIMS.MenusSampleMgmt" %>

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
            grid-template-columns: repeat(3, 1fr);
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
                                        <i class="fa fa-user fa-2xl" style="color:#b34788"></i>
                                        <span>Customer & Enquiry Management</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="CustomerInquiryView.aspx" target="_blank"><i class="fa fa-question-circle"></i>Customer Requests</a></div>
                                        <div class="card1"><a href="CustomerCRFList.aspx" target="_blank"><i class="fa fa-file"></i>Test Application Forms [TAF]</a></div>
                                        <div class="card1"><a href="EnquiryCRFStatus.aspx" target="_blank"><i class="fa fa-question-circle"></i>Enquiries v/s TAF Status</a></div>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <!-- Receivables & Payables -->
                            <div class="card" style="padding: 10px">
                                <br />
                                <div class="section">
                                    <div class="section-header green">
                                        <i class="fa fa-receipt fa-2xl" style="color:#b34788"></i>
                                        <span>Testing & Reporting</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="CustomerInquiryView.aspx" target="_blank"><i class="fa fa fa-check-circle"></i>Clone TAF</a></div>
                                        <div class="card1"><a href="CustomerCRFList.aspx?t=c" target="_blank"><i class="fa fa fa-check-circle"></i>Receive Sample</a></div>
                                        <div class="card1"><a href="CustomerCRFList.aspx?t=s" target="_blank"><i class="fa fa fa-check-circle"></i>Sample Verification</a></div>
                                        <div class="card1"><a href="AllSamples.aspx" target="_blank"><i class="fa fa-solid fa-file-lines"></i>Sample List</a></div>
                                        <div class="card1"><a href="GenerateReport.aspx" target="_blank"><i class="fa fa-solid fa-file-lines"></i>Generate Report</a></div>
                                        <div class="card1"><a href="QRAnalyzer.aspx" target="_blank"><i class="fa fa-solid fa-qrcode"></i>QR Analyzer</a></div>
                                        <div class="card1"><a href="SendReport.aspx" target="_blank"><i class="fa fa-paper-plane"></i>Send Report</a></div>

                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="card" style="padding: 10px">
                                <br />
                                <!-- Invoicing & Sales -->
                                <div class="section">
                                    <div class="section-header">
                                        <i class="fa fa-cog fa-2xl" style="color:#b34788"></i>
                                        <span>System Configuration</span>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="ManageProductGroup.aspx" target="_blank"><i class="fa fa-folder"></i>Product Group</a></div>
                                        <div class="card1"><a href="ManageGroupSubGroup.aspx" target="_blank"><i class="fa fa-folder-tree"></i>Product Sub Group</a></div>
                                    </div>
                                    <div class="divider"></div>

                                    <div class="grid">
                                        <div class="card1"><a href="TestParameterShow.aspx" target="_blank"><i class="fa fa-file-lines"></i>Test Parameter</a></div>
                                        <div class="card1"><a href="TestSubParameterShow.aspx" target="_blank"><i class="fa fa-file-lines"></i>Test Sub Parameter</a></div>
                                        <div class="card1"><a href="TestSubSubParameterShow.aspx" target="_blank"><i class="fa fa-file-lines"></i>Test Sub Sub Parameter</a></div>
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

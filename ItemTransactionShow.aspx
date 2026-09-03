<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ItemTransactionShow.aspx.cs" Inherits="LIMS.ItemTransactionShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Set fixed widths for each column */
        #tblE2 th:nth-child(1), #tblE2 td:nth-child(1) {
            width: 40px; /* for # */
        }

        #tblE2 th:nth-child(2), #tblE2 td:nth-child(2) {
            width: 60px; /* for QR Code */
        }

        #tblE2 th:nth-child(3), #tblE2 td:nth-child(3) {
            width: 120px; /* for PO No. */
        }

        #tblE2 th:nth-child(4), #tblE2 td:nth-child(4) {
            width: 80px; /* for Item Code */
        }

        /* for Name */
        #tblE2 th:nth-child(5), #tblE2 td:nth-child(5) {
            width: 200px;
        }

        #tblE2 th:nth-child(6), #tblE2 td:nth-child(6) {
            width: 100px; /* for Unit */
        }

        #tblE2 th:nth-child(7), #tblE2 td:nth-child(7) {
            width: 70px; /* for Rack */
        }

        #tblE2 th:nth-child(8), #tblE2 td:nth-child(8) {
            width: 70px; /* for Order Qty */
        }

        #tblE2 th:nth-child(9), #tblE2 td:nth-child(9) {
            width: 70px; /* for Pre.Rec.Qty */
        }

        #tblE2 th:nth-child(10), #tblE2 td:nth-child(10) {
            width: 70px; /* for Received Qty */
        }

        #tblE2 th:nth-child(11), #tblE2 td:nth-child(11) {
            width: 70px; /* for ShortFall Qty */
        }

        #tblE2 th:nth-child(12), #tblE2 td:nth-child(12) {
            width: 50px; /* for Balance Qty */
        }

        #tblE2 th:nth-child(13), #tblE2 td:nth-child(13) {
            width: 60px; /* for Price(₹) */
        }

        #tblE2 th:nth-child(14), #tblE2 td:nth-child(14) {
            width: 60px; /* for GST(%) */
        }

        #tblE2 th:nth-child(15), #tblE2 td:nth-child(15) {
            width: 10px; /* for Actions */
        }

        /* Wrapping text for long column headers */
        #tblE2 th {
            word-wrap: break-word;
            white-space: normal; /* Allows wrapping */
            max-width: 200px; /* Optional: to prevent columns from becoming too wide */
            text-align: center; /* Optional: center text for better readability */
        }

        /* Ensures the table takes up the full width of the container */
        #tblE2 {
            table-layout: fixed;
            width: 100%;
        }

            /* Optional: style for the QR Code image */
            #tblE2 td img {
                width: 50px; /* Adjust the size of the QR Code image */
                height: 50px;
                cursor: pointer;
            }
    </style>

    <style>
        /* Set the width for the "PO No." column */
        #tblE2 td:nth-child(5) {
            width: 75px; /* Set a fixed width for PO No. column */
            word-wrap: break-word; /* Wrap text in case the value is too long */
            white-space: normal; /* Allow normal text wrapping */
        }

        /* Add general styling for the table cells (optional) */
        #tblE2 td {
            padding: 8px; /* Add padding for better spacing */
            text-align: left; /* Center-align text for readability */
        }

            /* Optional: style for other columns (Example: IT_PoNumber column) */
            #tblE2 td:nth-child(5) {
                width: 75px; /* Item Code column */
            }

            #tblE2 td:nth-child(6) {
                width: 75px; /* Item Name column */
                word-wrap: break-word;
            }
    </style>

    <style type="text/css">
        /* Compact Table Styles */
        .compact-table {
            margin-top: 15px;
        }

            .compact-table table {
                margin-bottom: 5px;
                width: 100%;
            }

            .compact-table th,
            .compact-table td {
                padding: 8px 10px;
                white-space: nowrap;
            }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">ITEM TRANSACTION</h2>
                </div>
                <div class="body">

                    <!-- Search bar -->
                    <div class="row mt-4 mb-4" style="margin-top: 50px; margin-bottom: 50px;">
                        <div class="col-md-3">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name or Code" Style="margin-top: 17px;" />
                        </div>

                        <div class="col-md-2">
                            <asp:Label ID="DateStart" runat="server" Text="Start Date"></asp:Label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>

                        <div class="col-md-2">
                            <asp:Label ID="DateEnd" runat="server" Text="End Date"></asp:Label>
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>

                        <div class="col-md-2 text-end">
                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" Style="margin-top: 17px;">
                                <i class="fa fa-search me-1" style="font-size:20px;"></i>
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-secondary ms-2" OnClick="btnReset_Click" Style="margin-top: 17px;">
                                <i class="fa fa-rotate-right me-1"></i> 
                            </asp:LinkButton>
                        </div>

                        <div class="col-md-3 text-end">
                            <div class="btn-group" role="group" style="margin-top: 17px; float: right;">
                                <a href="ItemTransactionAdd.aspx" class="btn btn-secondary mb-3 mb-md-0"><i class='fa fa-plus'></i>NEW</a>
                            </div>

                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-secondary px-5 mt-3" OnClick="btnDownloadExcelFile_Click" Style="margin-top: 17px; float: right;">
                                <i class='fa fa-download me-2'></i> Excel
                            </asp:LinkButton>
                        </div>
                    </div>

                    <!-- Repeater Table -->
                    <div class="table-responsive compact-table">
                        <%--<table class="table table-striped table-bordered" id="tblE2">--%>
                        <table id="tblE2" class="table table-bordered table-striped table-hover table-condensed">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>QR Code</th>
                                    <th>PO No.</th>
                                    <th>Item Code</th>
                                    <th>Name</th>
                                    <th>Rack</th>
                                    <th>Order Qty</th>
                                    <th>Pre.Rec. Qty</th>
                                    <th>Received Qty</th>
                                    <th>ShortFall Qty</th>
                                    <th>Balance Qty</th>
                                    <th><i class="bx bx-menu" style="margin-left: 10px;"></i></th>
                                </tr>
                            </thead>

                            <tbody>
                                <asp:Repeater ID="RepDataShow" runat="server" OnItemCommand="RepDataShow_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# ((Convert.ToInt32(ViewState["PageIndex"]) - 1) * PageSize) + Container.ItemIndex + 1 %></td>
                                            <td>
                                                <img src='<%# ResolveUrl("/QRCodeSave/" + Eval("IT_QRCodeNumber") + "_QR.png") %>' alt='QR Code' onclick='printQRCode(this)' style="cursor: pointer;" />
                                            </td>
                                            <td><%# Eval("IT_PoNumber") %></td>
                                            <td><%# Eval("ItemCode") %></td>
                                            <td style="word-wrap: break-word; white-space: normal; width: 400px;"><%# Eval("ItemName") %></td>
                                            <td style="word-wrap: break-word; white-space: normal; width: 50px;"><%# Eval("RackType") %></td>
                                            <td><%# Eval("IT_OrderedQty") %></td>
                                            <td><%# Eval("IT_PreviousReceivedQtyInvoice") %></td>
                                            <td><%# Eval("IT_ReceivedQty") %></td>
                                            <td><%# Eval("IT_ShortFall") %></td>
                                            <td><%# Eval("IT_BalanceQty") %></td>
                                            <td class="text-center">
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                    CommandArgument='<%# Eval("IT_Id") %>'
                                                    ToolTip="Edit" CssClass="btn btn-sm btn-default waves-effect">
                                                        <i class="material-icons">edit</i>
                                                </asp:LinkButton>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>

                        <!-- Paging Controls -->
                        <div class="d-flex justify-content-center mt-3" style="margin-top: 15px; margin-bottom: 50px; float: right;">
                            <asp:Button ID="btnFirst" runat="server" Text="« First" CssClass="btn btn-outline-primary me-2" OnClick="btnFirst_Click" />
                            <asp:Button ID="btnPrev" runat="server" Text="‹ Prev" CssClass="btn btn-outline-primary me-2" OnClick="btnPrev_Click" />
                            <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkPage" runat="server" CommandName="Page"
                                        CommandArgument='<%# Eval("PageIndex") %>'
                                        CssClass='<%# Convert.ToInt32(Eval("PageIndex")) == Convert.ToInt32(ViewState["PageIndex"]) ? "btn btn-primary mx-1" : "btn btn-outline-primary mx-1" %>'>
                                                     <%# Eval("PageNumber") %>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:Repeater>
                            <asp:Button ID="btnNext" runat="server" Text="Next ›" CssClass="btn btn-outline-primary ms-2" OnClick="btnNext_Click" />
                            <asp:Button ID="btnLast" runat="server" Text="Last »" CssClass="btn btn-outline-primary ms-2" OnClick="btnLast_Click" />
                        </div>
                    </div>


                </div>
            </div>
        </div>
    </div>


    <script type="text/javascript">
        function printQRCode(image) {
            var imgSrc = image.src;
            var img = new Image();
            img.src = imgSrc;
            var windowContent = '<!DOCTYPE html>';
            windowContent += '<html>';
            windowContent += '<head><title>Print QR Code</title></head>';
            windowContent += '<body>';
            windowContent += '<img src="' + imgSrc + '" style="max-width: 100%;" />';
            windowContent += '</body>';
            windowContent += '</html>';

            var printWin = window.open('', '', 'width=1366,height=768');
            printWin.document.open();
            printWin.document.write(windowContent);
            printWin.document.close();
            printWin.print();
        }
    </script>
</asp:Content>

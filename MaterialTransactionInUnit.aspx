<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialTransactionInUnit.aspx.cs" Inherits="LIMS.MaterialTransactionInUnit" %>

<%@ Import Namespace="System.Data" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Material Transaction</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!--favicon-->
    <link rel="icon" href="NewAssets/images/favicon-32x32.png" type="image/png" />
    <!--plugins-->
    <link href="NewAssets/plugins/vectormap/jquery-jvectormap-2.0.2.css" rel="stylesheet" />

    <link href="NewAssets/plugins/simplebar/css/simplebar.css" rel="stylesheet" />
    <link href="NewAssets/plugins/perfect-scrollbar/css/perfect-scrollbar.css" rel="stylesheet" />
    <link href="NewAssets/plugins/metismenu/css/metisMenu.min.css" rel="stylesheet" />
    <link href="NewAssets/plugins/datatable/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <!-- loader-->
    <link href="NewAssets/css/pace.min.css" rel="stylesheet" />
    <script src="NewAssets/js/pace.min.js"></script>
    <!-- Bootstrap CSS -->
    <link href="NewAssets/css/bootstrap.min.css" rel="stylesheet">
    <link href="NewAssets/css/bootstrap-extended.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@400;500&amp;display=swap" rel="stylesheet">
    <link href="NewAssets/css/app.css" rel="stylesheet">
    <link href="NewAssets/css/icons.css" rel="stylesheet">
    <!-- Theme Style CSS -->
    <link rel="stylesheet" href="NewAssets/css/dark-theme.css" />
    <link rel="stylesheet" href="NewAssets/css/semi-dark.css" />
    <link rel="stylesheet" href="NewAssets/css/header-colors.css" />

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css">
    <style>
        .custom-checkboxbindDB input[type="checkbox"] {
            margin-right: 5px;
        }
    </style>

    <style>
        .custom-dropdown {
            opacity: 0.5;
            pointer-events: none;
            cursor: not-allowed;
        }
    </style>
    <style>
        .gridview-header th {
            background-color: #e7f2ff;
            color: #333;
            font-weight: bold;
            border-color: #ccc;
        }

        @media print {
            body * {
                visibility: hidden;
            }

            #printableArea, #printableArea * {
                visibility: visible;
            }

            #printableArea {
                position: absolute;
                left: 0;
                top: 0;
            }

            /*@page {
                size: A4 landscape;                
            }*/
        }
    </style>

    <style>
        .table-heading {
            background-color: #cfe2ff;
            color: #333;
            font-weight: bold;
            border-color: #ccc;
        }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div class="wrapper">
            <div class="page-content">
                <div class="row">
                    <div class="col-xl-12 mx-auto">
                        <div class="card">

                            <div class="card-body p-4">
                                <asp:Label runat="server" ID="lblPageID"></asp:Label>
                                <asp:Label runat="server" ID="lblPageData"></asp:Label>

                                <%--print start --%>
                                <div id="printableArea">
                                    <h3 style="text-align: center; color: #008cff;">Material Transaction</h3>

                                    <div class="table-responsive" style="margin-top: 22px;">
                                        <table class="table table-borderless">
                                            <tr>
                                                <td class="col-md-4">
                                                    <b>MR No : </b>
                                                    <asp:Label runat="server" ID="lblMRNo"></asp:Label>
                                                    <br />
                                                    <b>Unit : </b>
                                                    <asp:Label runat="server" ID="lblCompanyUnit"></asp:Label>
                                                    <br />
                                                </td>

                                                <td class="col-md-6"></td>

                                                <td class="col-md-2">
                                                    <b>MR-Generated Date : </b>
                                                    <asp:Label runat="server" ID="lblMRGeneratedDate"></asp:Label>
                                                    <br />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    
                                    <div class="row mt-4 mb-4">
                                        <div class="table-responsive">
                                            <table class="table table-bordered mb-0">
                                                <thead class="table-heading">
                                                    <tr>
                                                        <th scope="col">#</th>
                                                        <th scope="col">Item Code</th>
                                                        <th scope="col">Item Name</th>
                                                        <th scope="col">Request Qty.</th>
                                                        <th scope="col">Received Qty </th>
                                                        <th scope="col">Balance Qty.</th>
                                                        <th scope="col">Batch Details </th>
                                                        <th scope="col">UOM</th>
                                                    </tr>
                                                </thead>

                                                <asp:Repeater runat="server" ID="RepDataShow" OnItemDataBound="RepDataShow_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tbody>
                                                            <tr>
                                                                <td><%# Container.ItemIndex + 1 %></td>
                                                                <td><%# Eval("MR_ItemCode") %> </td>
                                                                <td style="word-wrap: break-word; white-space: normal; width: 500px;"><%# Eval("MR_ItemName") %> </td>
                                                                <td><%# Eval("MR_EmployeeRequireItemQty") %> </td>
                                                                <td><%# Eval("MR_ReceiveQty") %> </td>
                                                                <td><%# Eval("MR_BalanceQty") %> </td>
                                                                <td>
                                                                    <table class="table-responsive table table-bordered mb-0">
                                                                        <thead class="table-heading">
                                                                            <tr>
                                                                                <th scope="col">#</th>
                                                                                <th scope="col">Batch No </th>
                                                                                <th scope="col">Stock</th>
                                                                                <th scope="col">Expiry</th>
                                                                            </tr>
                                                                        </thead>

                                                                        <asp:Repeater runat="server" ID="TagDataRepeater">
                                                                            <ItemTemplate>
                                                                                <tbody>
                                                                                    <tr>
                                                                                        <td><%# Container.ItemIndex + 1 %></td>
                                                                                        <td>
                                                                                            <span class="badge bg-warning text-dark" style="font-size:12px; font-weight:100;">
                                                                                                <%# Eval("MTS_BatchNo") %>
                                                                                            </span>
                                                                                        </td>
                                                                                        <td>
                                                                                            <%--<span class="badge bg-info text-dark">--%>
                                                                                                <%# Eval("MTS_ItemStock") %>
                                                                                            <%--</span>--%>
                                                                                        </td>
                                                                                        <td>
                                                                                            <%--<span class="badge bg-danger text-dark">--%>
                                                                                                <%# Eval("MTS_ExpiryDate") %>
                                                                                            <%--</span>--%>
                                                                                        </td>
                                                                                    </tr>
                                                                                </tbody>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>

                                                                    </table>
                                                                </td>
                                                                <td><%# Eval("MR_UOM") %></td>
                                                            </tr>
                                                        </tbody>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </table>
                                        </div>
                                    </div>


                                    <div class="table-responsive">
                                        <table class="table table-borderless mt-4">
                                            <tr>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">Prepared By</td>

                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">Approved By</td>

                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">Released By</td>
                                            </tr>

                                            <tr>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblPreparedBy" runat="server"></asp:Label>
                                                </td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblApprovedBy" runat="server"></asp:Label>
                                                </td>

                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblReleasedBy" runat="server"></asp:Label>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblPreparedDate" runat="server"></asp:Label>
                                                </td>

                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblApprovedDate" runat="server"></asp:Label>
                                                </td>

                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblReleasedDate" runat="server"></asp:Label>
                                                </td>
                                            </tr>

                                        </table>
                                    </div>
                                </div>
                                <%-- print end --%>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
            <!--end row-->
        </div>

    </form>
</body>
</html>


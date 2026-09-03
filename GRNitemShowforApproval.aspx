<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GRNitemShowforApproval.aspx.cs" Inherits="LIMS.GRNitemShowforApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GRN Approval</title>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!--favicon-->
    <link rel="icon" href="favicon.png" type="image/png" />
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
                                <asp:Label runat="server" ID="lblPageData1"></asp:Label>
                                <%--print start --%>
                                <div id="printableArea">
                                    <h3 style="text-align: center; color: #008cff;">GOODS RECEIVED NOTE </h3>

                                    <div class="table-responsive" style="margin-top: 62px;">
                                        <table class="table table-borderless">
                                            <!-- Utilize table-borderless class to remove borders -->
                                            <tr>
                                                <td class="col-md-4">
                                                    <b>GRN No : </b>
                                                    <asp:Label runat="server" ID="lblGRNShow"></asp:Label>
                                                    <br />
                                                    <b>PO No : </b>
                                                    <asp:Label runat="server" ID="lblPoNo"></asp:Label>
                                                    <br />
                                                    <b>Vendor Name : </b>
                                                    <asp:Label runat="server" ID="lblVendorName"></asp:Label>
                                                    <br />
                                                    <b>Vendor Company : </b>
                                                    <asp:Label runat="server" ID="lblVendorCompany"></asp:Label>

                                                    <br />
                                                    <b>Bill Doc.No. : </b>
                                                    <div runat="server" id="fileLinksContainer">
                                                    </div>
                                                </td>

                                                <td class="col-md-6"></td>

                                                <td class="col-md-2">
                                                    <b>GRN Date : </b>
                                                    <asp:Label runat="server" ID="lblGRNDate"></asp:Label>
                                                    <br />
                                                    <b>PO Date : </b>
                                                    <asp:Label runat="server" ID="lblPODate"></asp:Label>
                                                    <br />
                                                    <b>Bill Date : </b>
                                                    <asp:Label runat="server" ID="lblBillDate"></asp:Label>
                                                    <br />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="row mt-4 mb-4">
                                        <div class="table-responsive">
                                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                                HeaderStyle-CssClass="gridview-header" DataKeyNames="IT_Id">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="#">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex + 1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="ItemCode" HeaderText="Item Code" SortExpression="ItemCode" />
                                                    <asp:BoundField DataField="ItemName" HeaderText="Item Name" SortExpression="ItemName" />
                                                    <asp:BoundField DataField="IT_OrderedQty" HeaderText="Order Qty." SortExpression="OrderQty" />
                                                    <%--<asp:BoundField DataField="IT_PreviousReceivedQty" HeaderText="Pre.Rec.Qty.(Total)" />--%>
                                                    <%--<asp:BoundField DataField="IT_PreviousShortFall" HeaderText="Pre.Shortfall (Total)" />--%>
                                                    <asp:BoundField DataField="IT_ReceivedQty" HeaderText="Received Qty." SortExpression="PoReceivedQty" />
                                                   <%-- <asp:BoundField DataField="IT_AcceptedQty" HeaderText="Accepted Qty." SortExpression="PoReceivedQty" />
                                                    <asp:BoundField DataField="IT_RejectedQty" HeaderText="Rejected Qty." SortExpression="PoReceivedQty" />
                                                    <asp:BoundField DataField="IT_ShortFall" HeaderText="Shortfall" />--%>
                                                    <asp:BoundField DataField="IT_BalanceQty" HeaderText="Balance Qty." SortExpression="PoBalanceQty" />
                                                    <asp:BoundField DataField="IT_BasicPrice" HeaderText="Basic Price" SortExpression="PoBasicPrice" />
                                                    <asp:BoundField DataField="Percentage" HeaderText="Tax(%)" SortExpression="Percentage" />

                                                    <asp:TemplateField HeaderText="Total Price">
                                                        <ItemTemplate>
                                                            <%# String.Format("{0:0.00}", Convert.ToDecimal(Eval("IT_BasicPrice")) * Convert.ToDecimal(Eval("IT_AcceptedQty")) * (1 + Convert.ToDecimal(Eval("Percentage")) / 100)) %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>


                                    <div class="table-responsive">
                                        <table class="table table-borderless mt-4">
                                            <tr>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">Prepared By</td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <%--Inspected By--%>
                                                </td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">Approved By</td>
                                            </tr>

                                            <tr>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblPreparedBy" runat="server"></asp:Label>
                                                </td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3"></td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblApprovedBy" runat="server"></asp:Label>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblPreparedDate" runat="server"></asp:Label>
                                                </td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3"></td>
                                                <td class="col-md-1"></td>
                                                <td class="col-md-3">
                                                    <asp:Label ID="lblApprovedDate" runat="server"></asp:Label>
                                                </td>
                                            </tr>

                                        </table>
                                    </div>
                                </div>
                                <%-- print end --%>
                            </div>

                            <div class="card-footer">
                                <div class="row">
                                    <div class="col-md-2">
                                        <asp:Button ID="btnApproved" runat="server" class="btn btn-primary px-5" Text="Approve" OnClick="btnApproved_Click" />
                                    </div>
                                    <div class="col-md-1"></div>
                                    <div class="col-md-2">
                                        <asp:Button ID="btnRejected" runat="server" class="btn btn-danger px-5" Text="Reject" OnClick="btnRejected_Click" />
                                    </div>
                                    <div class="col-md-4"></div>
                                    <div class="col-md-2">
                                        <asp:Button ID="btnCancle" runat="server" class="btn btn-secondary px-5" Text="Cancel" OnClick="btnCancle_Click" />
                                    </div>
                                </div>
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

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MRFormShowforApproval.aspx.cs" Inherits="LIMS.MRFormShowforApproval" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MR Approval</title>
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

                                <%--print start --%>
                                <div id="printableArea">
                                    <h3 style="text-align: center; color: #008cff;">Material Request</h3>

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
                                                    <%-- <b>Vendor Name : </b>
                                                    <asp:Label runat="server" ID="lblVendorName"></asp:Label>
                                                    <br />
                                                    <b>Vendor Company : </b>
                                                    <asp:Label runat="server" ID="lblVendorCompany"></asp:Label>

                                                    <br />
                                                    <b>Bill Doc.No. : </b>
                                                    <div runat="server" id="fileLinksContainer">
                                                    </div>--%>
                                                </td>

                                                <td class="col-md-6"></td>

                                                <td class="col-md-2">
                                                    <b>MR-Generated Date : </b>
                                                    <asp:Label runat="server" ID="lblMRGeneratedDate"></asp:Label>
                                                    <br />

                                                    <b>Project : </b>
                                                    <asp:Label runat="server" ID="lblProjectCode"></asp:Label>
                                                    <br />


                                                    <%--<b>PO Date : </b>
                                                    <asp:Label runat="server" ID="lblPODate"></asp:Label>
                                                    <br />
                                                    <b>Bill Date : </b>
                                                    <asp:Label runat="server" ID="lblBillDate"></asp:Label>
                                                    <br />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>




                                    <div class="row mt-4 mb-4">
                                        <div class="table-responsive">
                                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                                HeaderStyle-CssClass="gridview-header" OnRowDataBound="GridView1_RowDataBound" DataKeyNames="MRM_Id">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="#">
                                                        <ItemTemplate>
                                                            <%# Container.DataItemIndex + 1 %>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:BoundField DataField="MR_ItemCode" HeaderText="Item Code" SortExpression="MR_ItemCode" />

                                                    <asp:TemplateField HeaderText="Item Name">
                                                        <ItemTemplate>
                                                            <div style="word-wrap: break-word; white-space: normal; width: 400px;">
                                                                <%# Eval("MR_ItemName") %>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <%--<asp:BoundField DataField="MR_ItemName" HeaderText="Item Name" SortExpression="MR_ItemName" />--%>
                                                    <asp:BoundField DataField="MR_EmployeeRequireItemQty" HeaderText="Require Qty." />
                                                    <asp:BoundField DataField="MR_UOM" HeaderText="UOM" SortExpression="uom" />
                                                    <asp:BoundField DataField="MR_RequireDate" HeaderText="Require Date" />
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
                                        <asp:Button ID="btnApproved" runat="server" CssClass="btn btn-primary px-5" Text="Approve" OnClick="btnApproved_Click" />
                                    </div>
                                    <div class="col-md-1"></div>
                                    <div class="col-md-2">
                                        <asp:Button ID="btnRejected" runat="server" CssClass="btn btn-danger px-5" Text="Reject" OnClick="btnRejected_Click" />
                                    </div>
                                    <div class="col-md-4"></div>
                                    <div class="col-md-2">
                                        <asp:Button ID="btnCancle" runat="server" class="btn btn-secondary px-5" Text="Cancle" OnClick="btnCancle_Click" />
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
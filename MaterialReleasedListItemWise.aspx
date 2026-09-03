<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MaterialReleasedListItemWise.aspx.cs" Inherits="LIMS.MaterialReleasedListItemWise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<style>
        table {
            border-collapse: collapse !important;
            border: none !important;
        }
            table td, table th {
                border: none !important;
            }
    </style>--%>

    <style>
        .gridview-header th {
            background-color: #e7f2ff;
            color: #333;
            font-weight: bold;
            border-color: #000;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">


            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">MATERIAL RECEIVED</h2>
                </div>
                <div class="body">

                    <asp:Label runat="server" ID="lblPageID"></asp:Label>
                    <asp:Label runat="server" ID="lblPageData"></asp:Label>
                    <asp:Label runat="server" ID="lblPageCode"></asp:Label>

                    <%--print start --%>
                    <div id="printableArea">
                        <%--<h3 style="text-align: center; color: #008cff;">Material Received</h3>--%>

                        <div class="table-responsive" style="margin-top: 22px;">
                            <table class="table table-borderless">
                                <tr>
                                    <td class="col-md-4" style="border: none !important;">
                                        <b>MR No : </b>
                                        <asp:Label runat="server" ID="lblMRNo"></asp:Label>
                                        <br />
                                        <%--<b>Unit : </b>
                                            <asp:Label runat="server" ID="lblCompanyUnit"></asp:Label>
                                            <br />--%>
                                    </td>

                                    <td class="col-md-5" style="border: none !important;"></td>

                                    <td class="col-md-3" style="border: none !important;">
                                        <b>MR-Generated Date : </b>
                                        <asp:Label runat="server" ID="lblMRGeneratedDate"></asp:Label>
                                        <br />

                                        <%-- <b>Project : </b>
                                            <asp:Label runat="server" ID="lblProjectCode"></asp:Label>
                                            <br />--%>
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
                                                    <td><%# Container.ItemIndex + 1 %> </td>
                                                    <td><%# Eval("MR_ItemCode") %> </td>
                                                    <td style="word-wrap: break-word; white-space: normal; width: 250px;"><%# Eval("MR_ItemName") %> </td>
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
                                                                                <span class="badge bg-warning text-dark" style="font-size: 12px; font-weight: 100;">
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
                            <table class="table table-borderless mt-4" style="border-collapse: collapse !important; border: none !important;">
                                <tr>
                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="font-weight: bold; border: none !important;">Prepared By</td>

                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="font-weight: bold; border: none !important;">Approved By</td>

                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="font-weight: bold; border: none !important;">Released By</td>
                                </tr>

                                <tr>
                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="border: none !important;">
                                        <asp:Label ID="lblPreparedBy" runat="server"></asp:Label>
                                    </td>
                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="border: none !important;">
                                        <asp:Label ID="lblApprovedBy" runat="server"></asp:Label>
                                    </td>

                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="border: none !important;">
                                        <asp:Label ID="lblReleasedBy" runat="server"></asp:Label>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="border: none !important;">
                                        <asp:Label ID="lblPreparedDate" runat="server"></asp:Label>
                                    </td>

                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="border: none !important;">
                                        <asp:Label ID="lblApprovedDate" runat="server"></asp:Label>
                                    </td>

                                    <td class="col-md-1" style="border: none !important;"></td>
                                    <td class="col-md-3" style="border: none !important;">
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
</asp:Content>

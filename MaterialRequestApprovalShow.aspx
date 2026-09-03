<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MaterialRequestApprovalShow.aspx.cs" Inherits="LIMS.MaterialRequestApprovalShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .gridview-header th{
            background-color: #e7f2ff;
            color: #333;
            font-weight: bold;
            border-color: #000;
            
        }
    </style>

    <style>
        table {
            border-collapse: collapse !important;
            border: none !important;
        }

            table td, table th {
                border: none!important;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            <div class="row">
                <div class="col-xl-12 mx-auto">
                    <div class="card">

                        <div class="body">
                            <asp:Label runat="server" ID="lblPageID"></asp:Label>

                            <%--print start --%>
                            <div id="printableArea">
                                <h3 style="text-align: center; color: #008cff;">Material Request</h3>

                                <div class="table-responsive" style="margin-top: 50px; margin-left: 35px;">
                                    <table>
                                        <tr>
                                            <td class="col-md-4">
                                                <b>MR No : </b>
                                                <asp:Label runat="server" ID="lblMRNo"></asp:Label>
                                                <br />                                                
                                                <b>MR-Generated Date : </b>
                                                <asp:Label runat="server" ID="lblMRGeneratedDate"></asp:Label>
                                                <br />
                                            </td>
                                        </tr>
                                    </table>
                                </div>


                                <div class="row mt-4 mb-4" style="margin-top: 50px;">
                                    <div class="container table-responsive">
                                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                                            HeaderStyle-CssClass="gridview-header" OnRowDataBound="GridView1_RowDataBound" DataKeyNames="MRM_Id">
                                            <Columns>
                                                <asp:TemplateField HeaderText="#">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="MR_ItemCode" HeaderText="Item Code" SortExpression="MR_ItemCode" />
                                                <%--<asp:BoundField DataField="MR_ItemName" HeaderText="Item Name" SortExpression="MR_ItemName" />--%>
                                                <asp:TemplateField HeaderText="Item Name">
                                                    <ItemTemplate>
                                                        <div style="word-wrap: break-word; white-space: normal; width: 500px">
                                                            <%# Eval("MR_ItemName") %>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="MR_EmployeeRequireItemQty" HeaderText="Require Qty." />
                                                <asp:BoundField DataField="MR_UOM" HeaderText="UOM" SortExpression="uom" />
                                                <asp:BoundField DataField="MR_RequireDate" HeaderText="Require Date" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>


                                <div class="table-responsive">
                                    <table class="table table-borderless mt-4" style="margin-top:100px;">
                                        <tr>
                                            <td class="col-md-1"></td>
                                            <td class="col-md-3" style="font-weight:bold;">Prepared By</td>
                                            <td class="col-md-1"></td>
                                            <td class="col-md-3">
                                                <%--Inspected By--%>
                                            </td>
                                            <td class="col-md-1"></td>
                                            <td class="col-md-3" style="font-weight:bold;">Approved By</td>
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

                    </div>
                </div>
            </div>
        </div>
        <!--end row-->
    </div>
</asp:Content>

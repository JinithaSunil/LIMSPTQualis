<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MRShowforApproval.aspx.cs" Inherits="LIMS.MRShowforApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <!-- Table -->
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">MATERIAL REQUISITION LIST FOR APPROVAL</h2>
                </div>
                <div class="body">
                    <div style="margin-top: 50px; margin-bottom: 50px;">
                        <div id="divGrid" runat="server" class="table-responsive">
                            <table id="example2" class="table table-striped table-bordered">
                                <thead>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">MR No.</th>
                                        <th scope="col">Employee ID</th>
                                        <th scope="col">Employee Name</th>
                                        <th scope="col">Status</th>
                                        <th scope="col">
                                            <i class="bx bx-menu" style="margin-left: 10px;"></i>
                                        </th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <asp:Repeater runat="server" ID="RepDataShow" OnItemCommand="RepDataShow_ItemCommand">

                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Container.ItemIndex + 1 %></td>
                                                <td><%# Eval("MRM_NUMBER") %></td>
                                                <td><%# "EMP_" + Eval("MRM_EmployeeID") %></td>
                                                <td><%# Eval("FirstName") + " " + Eval("LastName") %></td>
                                                <td>
                                                    <asp:Label runat="server" Text='<%# Eval("StatusTitle") %>' ForeColor='<%# GetStatusColor(Eval("StatusTitle")) %>'></asp:Label>
                                                </td>

                                                <td class="text-center">
                                                    <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                        CommandArgument='<%# Eval("MRM_ID") %>'
                                                        ToolTip="Approval" CssClass="btn btn-sm btn-default waves-effect">
                                                            <i class="material-icons">edit</i>
                                                    </asp:LinkButton>
                                                </td>


                                            </tr>
                                        </ItemTemplate>

                                    </asp:Repeater>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">MR No.</th>
                                        <th scope="col">Employee ID</th>
                                        <th scope="col">Employee Name</th>
                                        <th scope="col">Status</th>
                                        <th scope="col">
                                            <i class="bx bx-menu" style="margin-left: 10px;"></i>
                                        </th>
                                    </tr>
                                </tfoot>
                            </table>

                        </div>
                    </div>

                </div>

            </div>

        </div>
    </div>
</asp:Content>

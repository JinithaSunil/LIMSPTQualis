<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GRNShowforApproved.aspx.cs" Inherits="LIMS.GRNShowforApproved" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <!-- Table -->
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">GRN APPROVAL LIST</h2>
                </div>

                <div class="body">
                    <div class="table-responsive">
                        <div style="margin-top: 50px; margin-bottom: 50px; margin-left: 50px; margin-right: 50px;">
                            <table id="example2" class="table table-striped table-bordered">
                                <thead>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">GRN </th>
                                        <th scope="col">PO No.</th>
                                        <th scope="col">Created Persson</th>
                                        <th scope="col">Date Time</th>
                                        <th scope="col">Staus</th>
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
                                                <td><%# Eval("GRN_NUMBER") %></td>
                                                <td><%# Eval("GRN_PoNumber") %></td>
                                                <td><%# Eval("FirstName") %></td>
                                                <td><%# Eval("GRN_CreatedDateTime") %></td>

                                                <td>
                                                    <asp:Label runat="server" Text='<%# Eval("StatusTitle") %>'
                                                        ForeColor='<%# GetStatusColor(Eval("StatusTitle")) %>'></asp:Label>
                                                </td>


                                                <td class="text-center">
                                                    <asp:LinkButton ID="lnkShow" runat="server" CommandName="ItemShow"
                                                        CommandArgument='<%# Eval("GRN_ID") + ";" + Eval("GRN_NUMBER") + ";" + Eval("GRN_PoNumber") %>'
                                                        ToolTip="Show" CssClass="btn btn-sm btn-default waves-effect">
                                                    <i class="material-icons">visibility</i>
                                                    </asp:LinkButton>

                                                    <%-- <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                        CommandArgument='<%# Eval("GRN_ID") + ";" + Eval("GRN_NUMBER") + ";" + Eval("GRN_PoNumber") %>'
                                                        ToolTip="Edit" CssClass="btn btn-sm btn-default waves-effect">
                                                    <i class="material-icons">edit</i>
                                                    </asp:LinkButton>--%>
                                                </td>


                                            </tr>
                                        </ItemTemplate>

                                    </asp:Repeater>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th scope="col">#</th>
                                        <th scope="col">GRN </th>
                                        <th scope="col">PO No.</th>
                                        <th scope="col">GRN Created Persson</th>
                                        <th scope="col">Date Time</th>
                                        <th scope="col">Staus</th>
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

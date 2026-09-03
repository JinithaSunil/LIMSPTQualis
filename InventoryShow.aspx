<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="InventoryShow.aspx.cs" Inherits="LIMS.InventoryShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            

            <!-- Table -->
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">INVENTORY SHOW</h2>
                </div>
                <div class="body">

                    <!-- Search bar -->
                    <div class="row mt-4 mb-4" style="margin-top: 50px; margin-bottom: 50px;">
                        <div class="col-md-3">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Item Name or Code" Style="margin-top: 20px;" />
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
                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-secondary" OnClick="btnDownloadExcelFile_Click" Style="margin-top: 17px; float:right;">
                                   <i class="fa fa-download"></i> Excel </asp:LinkButton>

                            <div class="btn-group" role="group" Style="margin-top: 17px; float:right;">
                                <a href="InventoryAdd.aspx" class="btn btn-secondary"><i class='fa fa-plus'></i> NEW</a>
                            </div>
                        </div>
                    </div>
                    <!-- Search bar -->

                    <!-- Inventory Repeater -->
                    <div class="table-responsive">
                        <table class="table table-bordered table-striped">
                            <thead class="table-light">
                                <tr>
                                    <th scope="col" style="width: 50px;">#</th>
                                    <th scope="col" style="width: 150px;">Item Code</th>
                                    <th scope="col" style="width: 200px;">Item Name</th>
                                    <%-- <th scope="col" style="width: 100px;">Unit</th>--%>
                                    <th scope="col" style="width: 200px;">Rack</th>
                                    <th scope="col" style="width: 120px;">Current Stock</th>
                                    <th scope="col" style="width: 100px;">Min.Stock</th>
                                    <th scope="col" style="width: 100px;">TAX(%)</th>
                                    <th scope="col" style="width: 150px;">Date</th>
                                    <th scope="col" style="width: 50px;">
                                        <i class="bx bx-menu" style="margin-left: 10px;"></i>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater ID="RepDataShow" runat="server" OnItemCommand="RepDataShow_ItemCommand">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# ((Convert.ToInt32(ViewState["PageIndex"]) - 1) * PageSize) + Container.ItemIndex + 1 %></td>
                                            <td><%# Eval("ItemCode") %></td>
                                            <td style="word-wrap: break-word; white-space: normal; width: 200px;"><%# Eval("ItemName") %></td>
                                            <%--<td><%# Eval("UnitName") %></td>--%>
                                            <td><%# Eval("RackType") + " --> " + Eval("RackSerialNumber") %></td>
                                            <td><%# Eval("InventoryCurrentStock") %></td>
                                            <td><%# Eval("InventoryMinStock") %></td>
                                            <td><%# Eval("Percentage") %></td>
                                            <td><%# Eval("InventoryCreatedDateTime") %></td>
                                            <td class="text-center">
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                    CommandArgument='<%# Eval("InventoryId") %>'
                                                    ToolTip="Edit" CssClass="btn btn-sm btn-default waves-effect">
                                                        <i class="material-icons">edit</i>
                                                </asp:LinkButton>
                                            </td>

                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- Paging Controls -->
                    <div class="d-flex justify-content-center mt-3" style="margin-bottom: 50px;">
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
                    <!-- Paging Controls -->

                </div>
            </div>

        </div>
    </div>
</asp:Content>

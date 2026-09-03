<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ItemArrangementShow.aspx.cs" Inherits="LIMS.ItemArrangementShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            <!-- Table -->
            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">ITEM STORE DATA</h2>
                </div>
                <div class="body">

                    <!-- Search bar -->
                    <div class="row mt-4 mb-4" style="margin-top: 50px; margin-bottom: 50px;">
                        <div class="col-md-4">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Item Name or Code" Style="margin-top: 17px;" />
                        </div>

                        <div class="col-md-2">
                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" Style="margin-top: 17px;">
                            <i class="fa fa-search me-1" style="font-size:20px;"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-secondary ms-2" OnClick="btnReset_Click" Style="margin-top: 17px;">
                            <i class="fa fa-rotate-right me-1"></i> 
                            </asp:LinkButton>
                        </div>

                        <div class="col-md-3 text-end"></div>

                        <div class="col-md-3 text-end">
                            <div class="btn-group" role="group" style="margin-top: 17px; float: right;">
                                <a href="ItemArrangementAdd.aspx" class="btn btn-secondary mb-3 mb-md-0"><i class='fa fa-plus'></i>NEW</a>
                            </div>

                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-secondary" OnClick="btnDownloadExcelFile_Click" Style="margin-top: 17px; float: right;">
                                <i class='fa fa-download'></i> Excel
                            </asp:LinkButton>
                        </div>
                    </div>
                    <!-- Search bar -->

                    <div class="table-responsive">
                        <table class="table table-striped table-bordered">
                            <thead>
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Item Code</th>
                                    <th scope="col">Item Name</th>
                                    <th scope="col">Rack</th>
                                    <th scope="col">Rack Series</th>
                                    <th scope="col">Item Stored</th>
                                    <th scope="col">Current Stock</th>

                                </tr>
                            </thead>

                            <tbody>
                                <asp:Repeater runat="server" ID="RepDataShow">
                                    <ItemTemplate>
                                        <tr>
                                            <%--<td><%# Container.ItemIndex + 1 %></td>--%>
                                            <td><%# ((Convert.ToInt32(ViewState["PageIndex"]) - 1) * PageSize) + Container.ItemIndex + 1 %></td>
                                            <td><%# Eval("ItemCode") %></td>
                                            <td style="word-wrap: break-word; white-space: normal; width: 500px;"><%# Eval("ItemName") %></td>
                                            <td><%# Eval("RackType") %></td>
                                            <td><%# Eval("RackSerialNumber") %></td>
                                            <td><%# Eval("TotalItemInRack") + "-" + Eval("UOMName")%></td>
                                            <td><%# Eval("InventoryCurrentStock") + "-" + Eval("UOMName")%></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>

                            <tfoot>
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Item Code</th>
                                    <th scope="col">Item Name</th>
                                    <th scope="col">Rack</th>
                                    <th scope="col">Rack Series</th>
                                    <th scope="col">Item Stored</th>
                                    <th scope="col">Current Stock</th>
                                </tr>
                            </tfoot>
                        </table>

                        <!-- Paging Controls -->
                        <div class="d-flex justify-content-center mt-3" style="margin-bottom: 50px; float: right;">
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
    </div>
</asp:Content>

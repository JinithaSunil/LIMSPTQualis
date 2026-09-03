<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="GRNGenerateShow.aspx.cs" Inherits="LIMS.GRNGenerateShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">GRN LIST SHOW</h2>
                </div>
                <div class="body">

                    <!-- Search bar -->
                    <div class="row mt-4 mb-4" style="margin-top: 50px; margin-bottom: 50px;">
                        <div class="col-md-3">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by GRN or PO Number" Style="margin-top: 17px;" />
                        </div>

                        <div class="col-md-2 col-xs-2">
                            <asp:Label ID="DateStart" runat="server" Text="Start Date"></asp:Label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>

                        <div class="col-md-2 col-xs-2">
                            <asp:Label ID="DateEnd" runat="server" Text="End Date"></asp:Label>
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                        </div>

                        <div class="col-md-2 col-xs-2 text-end">
                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" Style="margin-top: 17px;">
                                 <i class="fa fa-search " style="font-size:20px;"></i>
                            </asp:LinkButton>
                            <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-secondary ms-2" OnClick="btnReset_Click" Style="margin-top: 17px;">
                                 <i class="fa fa-rotate-right "></i> 
                            </asp:LinkButton>
                        </div>

                        <div class="col-md-3 text-end">


                            <div class="btn-group" role="group" style="margin-top: 17px; float: right;">
                                <a href="ItemInward.aspx" class="btn btn-secondary mb-3 mb-md-0"><i class='fa fa-plus'></i>NEW</a>
                            </div>

                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-secondary px-5 mt-3" OnClick="btnDownloadExcelFile_Click" Style="margin-top: 17px; float: right;">
                                <i class='fa fa-download me-2'></i> Excel
                            </asp:LinkButton>
                        </div>
                    </div>
                    <!-- Search bar -->

                    <!-- Inventory Repeater -->
                    <div class="table-responsive compact-table">
                        <table class="table table-bordered table-striped">
                            <thead class="table-light">
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">GRN </th>
                                    <th scope="col">PO No.</th>
                                    <th>Vendor Name</th>
                                    <th scope="col">Created Person</th>

                                    <th scope="col">Date Time</th>
                                    <th scope="col" style="width: 100px !important;">Staus</th>
                                    <th scope="col">
                                        <i class="bx bx-menu" style="margin-left: 10px;"></i>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="RepDataShow" OnItemCommand="RepDataShow_ItemCommand">

                                    <ItemTemplate>
                                        <tr>
                                            <td><%# ((Convert.ToInt32(ViewState["PageIndex"]) - 1) * PageSize) + Container.ItemIndex + 1 %></td>
                                            <td><%# Eval("GRN_NUMBER") %></td>
                                            <td><%# Eval("GRN_PoNumber") %></td>
                                            <td><%# Eval("VendorName") %></td>
                                            <td><%# Eval("GRN_Employee") %></td>
                                            <td><%# Eval("GRN_CreatedDateTime") == DBNull.Value 
                                                    ? "" 
                                                    : Convert.ToDateTime(Eval("GRN_CreatedDateTime")).ToString("dd-MM-yyyy") %></td>
                                            <td style="width: 100px !important;">
                                                <asp:Label runat="server" Text='<%# Eval("GRN_Status") %>' ForeColor='<%# GetStatusColor(Eval("GRN_Status")) %>'></asp:Label>
                                            </td>

                                            <td class="text-center">
                                                <asp:LinkButton ID="lnkShow" runat="server" CommandName="ItemShow"
                                                    CommandArgument='<%# Eval("GRN_ID") + ";" + Eval("GRN_NUMBER") + ";" + Eval("GRN_PoNumber") %>'
                                                    ToolTip="Show" CssClass="btn btn-sm btn-default waves-effect">
                                                        <i class="material-icons">visibility</i>
                                                </asp:LinkButton>

    <%--                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                    CommandArgument='<%# Eval("GRN_ID") + ";" + Eval("GRN_NUMBER") + ";" + Eval("GRN_PoNumber") %>'
                                                     Visible='<%# Eval("GRN_Status").ToString() == "GRN Generated" %>'
                                                    ToolTip="Edit" CssClass="btn btn-sm btn-default waves-effect">
                                                    <i class="material-icons">edit</i>
                                                </asp:LinkButton>--%>
                                            </td>

                                        </tr>
                                    </ItemTemplate>

                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>

                    <!-- Paging Controls -->
                    <div class="d-flex justify-content-center mt-3" style="margin-bottom: 50px; margin-top: 15px;">
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
</asp:Content>

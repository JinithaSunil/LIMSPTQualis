<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="MaterialRequestShow.aspx.cs" Inherits="LIMS.MaterialRequestShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <div class="row">
                <div class="col-xl-12 mx-auto">

                    <div class="card">
                        <div class="header bg-violet">
                            <h2 style="color: white !important">MATERIAL REQUISITION</h2>
                        </div>

                        <div class="body">
                            <!-- Search bar -->
                            <div class="row clearfix mt-4" style="margin-top: 50px;">

                                <div class="col-md-3">
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by MR.No or PROJECT" Style="margin-top: 17px;" />
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
                                    <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-secondary" OnClick="btnReset_Click" Style="margin-top: 17px;">
                                <i class="fa fa-rotate-right me-1"></i> 
                                    </asp:LinkButton>
                                </div>


                                <div class="col-md-3">
                                    <div class="btn-group" role="group" style="margin-top: 17px; float: right;">
                                        <a href="MaterialRequestForm.aspx" class="btn btn-secondary mb-3 mb-md-0"><i class='fa fa-plus'></i>NEW</a>
                                    </div>

                                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-secondary" OnClick="btnDownloadExcelFile_Click" Style="margin-top: 17px; float: right;">
                                    <i class="fa fa-download me-1"></i> Excel</asp:LinkButton>
                                </div>
                            </div>
                            <!-- Search bar -->

                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <div id="divGrid" runat="server" class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th scope="col">#</th>
                                                    <th scope="col">MR No.</th>

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
                                                            <td><%# ((Convert.ToInt32(ViewState["PageIndex"]) - 1) * PageSize) + Container.ItemIndex + 1 %></td>
                                                            <td><%# Eval("MRM_NUMBER") %></td>
                                                            <td>
                                                                <asp:Label runat="server" Text='<%# Eval("StatusTitle") %>' ForeColor='<%# GetStatusColor(Eval("StatusTitle")) %>'></asp:Label>
                                                            </td>
                                                            <td class="text-center">
                                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit" CommandArgument='<%# Eval("MRM_ID") + ";" + Eval("MRM_StatusId")%>'
                                                                    ToolTip="Edit">
                                                                <i class="fa fa-edit" style="font-size:20px;"></i>
                                                                </asp:LinkButton>
                                                                &nbsp;&nbsp;&nbsp;                                                   
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                            <tfoot>
                                                <tr>
                                                    <th scope="col">#</th>
                                                    <th scope="col">MR No.</th>

                                                    <th scope="col">Status</th>
                                                    <th scope="col">
                                                        <i class="bx bx-menu" style="margin-left: 10px;"></i>
                                                    </th>
                                                </tr>
                                            </tfoot>
                                        </table>

                                        <!-- Paging Controls -->
                                        <div class="d-flex justify-content-center mt-3" style="margin-bottom: 50px; float: right;">
                                            <asp:Button ID="btnFirst" runat="server" Text="« First" CssClass="btn btn-outline-primary me-2" OnClick="btnFirst_Click" />
                                            <asp:Button ID="btnPrev" runat="server" Text="‹ Prev" CssClass="btn btn-outline-primary me-2" OnClick="btnPrev_Click" />

                                            <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkPage" runat="server" CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' CssClass='<%# Convert.ToInt32(Eval("PageIndex")) == Convert.ToInt32(ViewState["PageIndex"]) ? "btn btn-primary mx-1" : "btn btn-outline-primary mx-1" %>'>
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
                </div>
            </div>
        </div>
    </div>
</asp:Content>

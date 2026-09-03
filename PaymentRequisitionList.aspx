<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PaymentRequisitionList.aspx.cs" Inherits="LIMS.PaymentRequisitionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">

            <div class="card">
                <div class="header bg-violet">
                    <h2 style="color: white !important">PAYMENT REQUISITION LIST</h2>
                </div>
                <div class="body">

                    <!-- Search bar -->
                    <div class="row mt-4 mb-4" style="margin-top: 50px; margin-bottom: 50px;">
                        <div class="col-md-3">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Request Number" Style="margin-top: 17px;" />
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

                            <div id="divNew" runat="server" class="btn-group" role="group" style="margin-top: 17px; float: right;">
                                <a href="PaymentRequisition.aspx" class="btn btn-secondary mb-3 mb-md-0"><i class='fa fa-plus'></i>NEW</a>
                            </div>

                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-secondary px-5 mt-3" OnClick="btnDownloadExcelFile_Click" Style="margin-top: 17px; float: right;">
                                <i class='fa fa-download me-2'></i> Excel
                            </asp:LinkButton>
                            <asp:LinkButton ID="lnkPayments" runat="server" CssClass="btn btn-secondary px-5 mt-3" OnClick="lnkPayments_Click" Style="margin-top: 17px; float: right;">
                                    <i class="fa fa-hand-holding-dollar"></i> Payments
                            </asp:LinkButton>
                        </div>
                    </div>
                    <!-- Search bar -->
                    <div class="row">
                        <asp:RadioButtonList ID="radChoice" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radChoice_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Value="1" Selected="True">New</asp:ListItem>
                            <asp:ListItem Value="1015">Approved</asp:ListItem>
                            <asp:ListItem Value="1016">Rejected</asp:ListItem>
                            <asp:ListItem Value="1017">Paid</asp:ListItem>
                            <asp:ListItem Value="0">All</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <br />
                    <!-- Inventory Repeater -->
                    <div class="row">
                        Record(s): <span id="spanCount" runat="server"></span>
                    </div>
                    <div class="table-responsive compact-table">
                        <table class="table table-bordered table-striped">
                            <thead class="table-light">
                                <tr>
                                    <th scope="col">#</th>
                                    <th scope="col">Requisition# </th>
                                    <th scope="col">Date Time</th>
                                    <th scope="col">Request By</th>
                                    <th scope="col">Request Type</th>
                                    <th scope="col">TotalAmount </th>
                                    <th scope="col" style="width: 100px !important;">Staus</th>
                                    <th scope="col">
                                        <i class="bx bx-menu" style="margin-left: 10px;"></i>
                                    </th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="RepDataShow" OnItemDataBound="RepDataShow_ItemDataBound" OnItemCommand="RepDataShow_ItemCommand">

                                    <ItemTemplate>
                                        <tr>
                                            <td><%# ((Convert.ToInt32(ViewState["PageIndex"]) - 1) * PageSize) + Container.ItemIndex + 1 %></td>
                                            <td>
                                                <asp:HyperLink ID="hypPR" runat="server" Text='<%#Eval("RequisitionNumber") %>' NavigateUrl='<%#"PaymentRequisition/" + Eval("PaymentRequisitionID") + ".htm" %>' Target="_blank"></asp:HyperLink>
                                            </td>
                                            <td><%# Eval("RequisitionDate") == DBNull.Value 
                                                    ? "" 
                                                    : Convert.ToDateTime(Eval("RequisitionDate")).ToString("dd-MM-yyyy") %></td>
                                            <td><%# Eval("RequisitionBy") %></td>
                                            <td><%# Eval("RequisitionType") %></td>
                                            <td><%# Eval("TotalAmount") %></td>
                                            <td style="width: 100px !important;">
                                                <asp:Label ID="lblRequisitionStatus" runat="server" Text='<%# Eval("RequisitionStatus").ToString() %>' ForeColor='<%# GetStatusColor(Eval("RequisitionStatus")) %>'></asp:Label>
                                            </td>

                                            <td class="text-center">
                                                <asp:LinkButton ID="lnkShow" runat="server" CommandName="ItemShow"
                                                    CommandArgument='<%# Eval("PaymentRequisitionID") + ";" + Eval("RequisitionNumber") + ";" + Eval("RequisitionBy") %>'
                                                    ToolTip="Show" CssClass="btn btn-sm btn-default waves-effect" Visible="false">
                                                        <i class="material-icons">visibility</i>
                                                </asp:LinkButton>
                                               
                                                <asp:LinkButton ID="lnkPay" runat="server" CommandName="Pay" 
                                                    CommandArgument='<%# Eval("PaymentRequisitionID") + ";" + Eval("RequisitionNumber") + ";" + Eval("RequisitionBy") %>'
                                                    ToolTip="Pay" CssClass="btn btn-sm btn-default waves-effect"
                                                     Visible='<%# Eval("RequisitionStatus").ToString() == "1015" %>'>
                                                    <i class="bank-card-line">Pay</i>
                                                </asp:LinkButton>

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

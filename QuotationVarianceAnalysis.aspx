<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="QuotationVarianceAnalysis.aspx.cs" Inherits="LIMS.QuotationVarianceAnalysis" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .group-start td{
    border-top:5px solid #000 !important;
}
        .card-counter {
            border-left: 5px solid #0d6efd;
        }

        .variance-high {
            background: #ffe5e5;
            color: #b30000;
            font-weight: 600;
        }

        .variance-low {
            background: #fff7db;
            color: #856404;
        }

        .variance-ok {
            background: #e8fff1;
            color: #198754;
        }

        .summary-value {
            font-size: 28px;
            font-weight: bold;
        }

        .table th {
            white-space: nowrap;
        }

        .search-panel {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>QUOTATION VARIANCE ANALYSIS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">


                        <!-- SEARCH -->

                        <div class="search-panel mb-4">

                            <div class="row">

                                <div class="col-md-3">
                                    <label>Enquiry No</label>
                                    <asp:DropDownList ID="ddlEnquiry"
                                        runat="server"
                                        CssClass="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label>PR No</label>
                                    <asp:TextBox ID="txtPR"
                                        runat="server"
                                        CssClass="form-control">
                                    </asp:TextBox>
                                </div>

                                <div class="col-md-2">
                                    <label>Vendor</label>
                                    <asp:DropDownList ID="ddlVendor"
                                        runat="server"
                                        CssClass="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">
                                    <label>Revision</label>
                                    <asp:DropDownList ID="ddlRevision"
                                        runat="server"
                                        CssClass="form-control">

                                        <asp:ListItem Text="Latest" Value="0" />
                                        <asp:ListItem Text="All" Value="-1" />

                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-2">

                                    <label>&nbsp;</label>

                                    <div>
                                        <asp:CheckBox ID="chkVarianceOnly"
                                            runat="server"
                                            Text="Variance Only" />
                                    </div>

                                </div>

                                <div class="col-md-1">

                                    <label>&nbsp;</label>

                                    <asp:Button ID="btnSearch"
                                        runat="server"
                                        Text="Search"
                                        CssClass="btn btn-primary w-100"
                                        OnClick="btnSearch_Click" />

                                </div>

                            </div>

                        </div>


                        <!-- KPI -->

                        <div class="row mb-4">

                            <div class="col-md-3">

                                <div class="card card-counter">

                                    <div class="card-body text-center">

                                        <div>Total Vendors</div>

                                        <div class="summary-value">
                                            <asp:Label ID="lblVendorCount"
                                                runat="server"
                                                Text="0"></asp:Label>
                                        </div>

                                    </div>

                                </div>

                            </div>

                            <div class="col-md-3">

                                <div class="card card-counter">

                                    <div class="card-body text-center">

                                        <div>Total Quotations</div>

                                        <div class="summary-value">

                                            <asp:Label ID="lblQuotationCount"
                                                runat="server"
                                                Text="0"></asp:Label>

                                        </div>

                                    </div>

                                </div>

                            </div>

                            <div class="col-md-3">

                                <div class="card card-counter">

                                    <div class="card-body text-center">

                                        <div>Variance Found</div>

                                        <div class="summary-value text-danger">

                                            <asp:Label ID="lblVariance"
                                                runat="server"
                                                Text="0"></asp:Label>

                                        </div>

                                    </div>

                                </div>

                            </div>

                            <div class="col-md-3">

                                <div class="card card-counter">

                                    <div class="card-body text-center">

                                        <div>Highest Variance</div>

                                        <div class="summary-value text-warning">

                                            <asp:Label ID="lblHighestVariance"
                                                runat="server"
                                                Text="0%"></asp:Label>

                                        </div>

                                    </div>

                                </div>

                            </div>

                        </div>


                        <!-- GRID -->

                        <div class="card">

                            <div class="card-header bg-primary text-white" style="height:30px; text-align:center; vertical-align:middle">
                                <h5>VENDOR QUOTATION COMPARISON</h5>
                            </div>

                            <div class="card-body">

                                <asp:GridView ID="gvQuotationVariance"
                                    runat="server"
                                    CssClass="table table-bordered table-hover table-striped"
                                    AutoGenerateColumns="False"
                                    DataKeyNames="VendorQuotationDetailsID"
                                    OnRowDataBound="gvQuotationVariance_RowDataBound"
                                    OnRowCommand="gvQuotationVariance_RowCommand">

                                    <Columns>

                                        <asp:BoundField DataField="VendorName"
                                            HeaderText="Vendor" />

                                        <asp:BoundField DataField="ItemName"
                                            HeaderText="Item" />

                                        <asp:BoundField DataField="Revision"
                                            HeaderText="Revision" />

                                        <asp:BoundField DataField="PreviousRate"
                                            HeaderText="Original Rate"
                                            DataFormatString="{0:N2}" />

                                        <asp:BoundField DataField="CurrentRate"
                                            HeaderText="ERP Rate"
                                            DataFormatString="{0:N2}" />

                                        <asp:BoundField DataField="VariancePercent"
                                            HeaderText="Variance %" />

                                        <asp:BoundField DataField="DeliveryTime"
                                            HeaderText="Delivery" />

                                        <asp:BoundField DataField="QuoteCurrencyID"
                                            HeaderText="Currency" />

                                       <%-- <asp:TemplateField HeaderText="Quotation">

                                            <ItemTemplate>

                                                <asp:HyperLink ID="lnkDocument"
                                                    runat="server"
                                                    NavigateUrl='<%# Eval("QuotationFile") %>'
                                                    Target="_blank"
                                                    Text="View PDF" />

                                            </ItemTemplate>

                                        </asp:TemplateField>--%>

                                        <asp:BoundField DataField="Status"
                                            HeaderText="Status" />

                                        <asp:TemplateField>

                                            <ItemTemplate>

                                                <asp:LinkButton ID="btnView"
                                                    runat="server"
                                                    CssClass="btn btn-sm btn-info"
                                                    CommandName="ViewDetail"
                                                    CommandArgument='<%# Eval("VendorQuotationDetailsID") %>'>

                                Details

                                                </asp:LinkButton>

                                            </ItemTemplate>

                                        </asp:TemplateField>

                                    </Columns>

                                </asp:GridView>

                            </div>

                        </div>


                        <!-- DETAIL -->

                        <div class="card mt-4">

                            <div class="card-header bg-primary text-white">
                                Variance Detail

                            </div>

                            <div class="card-body">

                                <asp:DetailsView ID="dvVariance"
                                    runat="server"
                                    CssClass="table table-bordered"
                                    AutoGenerateRows="true">
                                </asp:DetailsView>

                            </div>

                        </div>


                        <!-- APPROVAL -->

                        <div class="card mt-4">

                            <div class="card-header bg-warning">
                                Variance Approval

                            </div>

                            <div class="card-body">

                                <div class="row">

                                    <div class="col-md-8">

                                        <label>Remarks</label>

                                        <asp:TextBox ID="txtRemarks"
                                            runat="server"
                                            TextMode="MultiLine"
                                            Rows="4"
                                            CssClass="form-control">
                                        </asp:TextBox>

                                    </div>

                                    <div class="col-md-4">

                                        <label>&nbsp;</label>

                                        <div>

                                            <asp:Button ID="btnApprove"
                                                runat="server"
                                                Text="Approve Variance"
                                                CssClass="btn btn-success"
                                                OnClick="btnApprove_Click" />

                                            <asp:Button ID="btnReject"
                                                runat="server"
                                                Text="Reject"
                                                CssClass="btn btn-danger"
                                                OnClick="btnReject_Click" />

                                        </div>

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

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PRToPOTracking.aspx.cs" Inherits="LIMS.PRToPOTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet" />

    <style>

        body {
            background: #f5f6fa;
            font-size: 12px;
        }

        .page-title {
            font-size: 26px;
            font-weight: 600;
        }

        .card {
            background: #fff;
            border: 1px solid #ccc;
            margin-bottom: 20px;
            border-radius: 4px;
        }

            .card h3 {
                margin-top: 0;
                background: #3f6fa5;
                color: #fff;
                padding: 8px;
                font-size: 15px;
            }

        .card-dashboard {
            border: none;
            border-radius: 12px;
            box-shadow: 0 3px 12px rgba(0,0,0,.08);
        }

        .kpi-number {
            font-size: 22px;
            font-weight: bold;
        }

        .status-pill {
            border-radius: 30px;
            padding: 5px 15px;
            color: #fff;
            font-size: 12px;
        }

        .pending {
            background: #dc3545;
        }

        .partial {
            background: #ffc107;
            color: #000;
        }

        .completed {
            background: #198754;
        }

        .section-title {
            font-weight: 600;
            font-size: 18px;
        }

        .progress {
            height: 18px;
        }

        .timeline {
            display: flex;
            justify-content: space-between;
            margin-top: 20px;
        }

        .timeline-step {
            width: 16%;
            text-align: center;
        }

            .timeline-step i {
                width: 45px;
                height: 45px;
                border-radius: 50%;
                background: #0d6efd;
                color: white;
                line-height: 45px;
                font-size: 22px;
            }

        .right-card {
            position: sticky;
            top: 20px;
        }

        .loading-overlay {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: rgba(255,255,255,.7);
            z-index: 99999;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager
        ID="ScriptManager1"
        runat="server"
        EnablePartialRendering="true"
        AsyncPostBackTimeout="300">
    </asp:ScriptManager>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet" style="margin-top: 0px">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>PURCHASE REQUISITION TRACKING</h2>
                            <div class="text-muted">
                                Purchase Requisition → Enquiry → Vendor Quotation → Purchase Order → GRN → Purchase Invoice
                            </div>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <asp:Panel ID="pnlMessage"
                            runat="server"
                            Visible="false"
                            CssClass="alert alert-success alert-dismissible fade show">

                            <asp:Label ID="Label1"
                                runat="server">
                            </asp:Label>

                            <button type="button"
                                class="btn-close"
                                data-bs-dismiss="alert"
                                aria-label="Close">
                            </button>

                        </asp:Panel>
                        <asp:UpdateProgress
                            ID="UpdateProgress1"
                            runat="server"
                            AssociatedUpdatePanelID="updMain">

                            <ProgressTemplate>

                                <div class="loading-overlay">

                                    <div class="spinner-border text-primary"></div>

                                    <br />

                                    Loading Procurement Data...

                                </div>

                            </ProgressTemplate>

                        </asp:UpdateProgress>

                        <asp:UpdatePanel ID="updMain"
                            runat="server"
                            UpdateMode="Conditional">

                            <ContentTemplate>

                                <div class="container-fluid mt-3">

                                    <div class="row">
                                        <div class="card shadow-sm mb-3">
                                            <div class="card-body">

                                                <div class="d-flex align-items-center flex-wrap gap-2">

                                                    <asp:LinkButton ID="lnkBtnFilter"
                                                        runat="server"
                                                        CssClass="btn btn-primary border"
                                                        OnClick="btnFilter_Click">
                                                    <i class="material-icons">filter_alt</i>
                                                    </asp:LinkButton>

                                                    <label class="mb-0 fw-bold">From</label>

                                                    <asp:TextBox ID="txtFrom"
                                                        runat="server"
                                                        TextMode="Date"
                                                        CssClass="form-control"
                                                        Style="width: 180px;" />

                                                    <label class="mb-0 fw-bold">To</label>

                                                    <asp:TextBox ID="txtTo"
                                                        runat="server"
                                                        TextMode="Date"
                                                        CssClass="form-control"
                                                        Style="width: 180px;" />

                                                    <asp:Button ID="btnSearch"
                                                        runat="server"
                                                        Text="Search"
                                                        CssClass="btn btn-primary"
                                                        OnClick="btnSearch_Click" />

                                                    <asp:Button ID="btnReset"
                                                        runat="server"
                                                        Text="Reset"
                                                        CssClass="btn btn-secondary"
                                                        OnClick="btnReset_Click" />

                                                    <asp:Button ID="btnExport"
                                                        runat="server"
                                                        Text="Export Excel"
                                                        CssClass="btn btn-success"
                                                        OnClick="btnExport_Click" />

                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                    <asp:Panel ID="pnlFilters"
                                        runat="server"
                                        Visible="false">

                                        <div class="card mt-2">
                                            <div class="card-body">

                                                <div class="row g-3">

                                                    <div class="col-md-3">
                                                        <label>PR No</label>
                                                        <asp:TextBox
                                                            ID="txtPRNo"
                                                            runat="server"
                                                            CssClass="form-control" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <label>Department</label>
                                                        <asp:DropDownList
                                                            ID="ddlDepartment"
                                                            runat="server"
                                                            CssClass="form-control" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <label>Vendor</label>
                                                        <asp:DropDownList
                                                            ID="ddlVendor"
                                                            runat="server"
                                                            CssClass="form-control" />
                                                    </div>

                                                    <div class="col-md-3">
                                                        <label>Status</label>
                                                        <asp:DropDownList
                                                            ID="ddlStatus"
                                                            runat="server"
                                                            CssClass="form-control">

                                                            <asp:ListItem Text="All" />

                                                            <asp:ListItem Text="Pending" />

                                                            <asp:ListItem Text="Partial" />

                                                            <asp:ListItem Text="Completed" />

                                                        </asp:DropDownList>
                                                    </div>

                                                </div>

                                            </div>
                                        </div>

                                    </asp:Panel>
                                    <hr />
                                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                    <!-- SEARCH -->

                                    <div class="card card-dashboard mb-4">

                                        <div class="card-body">
                                            <!-- MAIN CONTENT -->

                                            <div class="row mt-4">

                                                <div class="col-lg-12">

                                                    <div class="card card-dashboard">

                                                        <div class="card-header bg-white">

                                                            <div class="section-title">
                                                                Purchase Requisition Status

                                                            </div>

                                                        </div>

                                                        <div class="card-body">
                                                            <div class="row">
                                                                <div>
                                                                    Total records: <span id="spanCount" runat="server"></span>
                                                                    <span style="float: right">
                                                                        <div class="d-flex align-items-center gap-2">
                                                                            <span>Show</span>

                                                                            <asp:DropDownList ID="ddlPageSize" runat="server"
                                                                                CssClass="form-select"
                                                                                Style="width: 80px;"
                                                                                AutoPostBack="true"
                                                                                OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                                                                                <asp:ListItem Text="10" Value="10" />
                                                                                <asp:ListItem Text="25" Value="25" />
                                                                                <asp:ListItem Text="50" Value="50" />
                                                                                <asp:ListItem Text="100" Value="100" />
                                                                            </asp:DropDownList>

                                                                            <span>records</span>
                                                                        </div>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                            <div class="row">

                                                                <!-- GridView comes here -->

                                                                <asp:GridView
                                                                    ID="gvTracking"
                                                                    runat="server"
                                                                    AutoGenerateColumns="False"
                                                                    CssClass="table table-bordered table-striped table-hover"
                                                                    OnRowCommand="gvTracking_RowCommand"
                                                                    AllowPaging="true"
                                                                    AllowCustomPaging="true"
                                                                    PageSize="10" OnPageIndexChanging="gvTracking_PageIndexChanging">

                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="1%">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="1%" HorizontalAlign="Center" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="PR#">
                                                                            <ItemTemplate>

                                                                                <asp:LinkButton
                                                                                    ID="lnkPR"
                                                                                    runat="server"
                                                                                    Text='<%# Eval("PRNo") %>'
                                                                                    CommandName="PRDetails"
                                                                                    CommandArgument='<%# Eval("PurchaseRequisitionID") %>'
                                                                                    CssClass="fw-bold text-primary">
                                                                                </asp:LinkButton>

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:BoundField DataField="Department" HeaderText="Department" />

                                                                        <asp:BoundField DataField="ItemName" HeaderText="Item" />

                                                                        <asp:BoundField DataField="RequestedQty" HeaderText="Req Qty" />

                                                                        <asp:BoundField DataField="OrderedQty" HeaderText="Ordered" />

                                                                        <asp:BoundField DataField="ReceivedQty" HeaderText="Received" />

                                                                        <asp:BoundField DataField="PendingPOQty" HeaderText="Pending PO" />

                                                                        <asp:BoundField DataField="PendingReceiptQty" HeaderText="Pending Receipt" />

                                                                        <asp:TemplateField HeaderText="Progress">

                                                                            <ItemTemplate>

                                                                                <div class="progress">

                                                                                    <div class="progress-bar bg-success"
                                                                                        role="progressbar"
                                                                                        style='<%# "width:" + Eval("Progress") + "%" %>'>

                                                                                        <%# Eval("Progress") %>%           
                                                                                    </div>

                                                                                </div>

                                                                            </ItemTemplate>

                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Status">

                                                                            <ItemTemplate>

                                                                                <span class='<%# GetStatusCss(Eval("Status").ToString()) %>'>
                                                                                    <%# Eval("Status") %>
                                                                                </span>


                                                                            </ItemTemplate>

                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Action">

                                                                            <ItemTemplate>

                                                                                <asp:HyperLink
                                                                                    ID="lnkView"
                                                                                    runat="server"
                                                                                    CssClass="btn btn-primary btn-sm"
                                                                                    Text="View"
                                                                                    NavigateUrl='<%# "GeneratePurchaseRequisitionReport.aspx?ID=" + Eval("PRID") %>'
                                                                                    Target="_blank">
                                                                                </asp:HyperLink>

                                                                            </ItemTemplate>

                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                    <FooterStyle BackColor="White" ForeColor="#333333" />
                                                                    <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                                                    <PagerSettings Mode="NumericFirstLast"
                                                                        FirstPageText="First"
                                                                        LastPageText="Last" />
                                                                    <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                    <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                                                    <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                                                    <SortedAscendingHeaderStyle BackColor="#487575" />
                                                                    <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                                                    <SortedDescendingHeaderStyle BackColor="#275353" />
                                                                </asp:GridView>
                                                            </div>
                                                            <div class="row">
                                                                <asp:Panel ID="pnlPRDetails"
                                                                    runat="server"
                                                                    Visible="false"
                                                                    CssClass="card shadow mt-3">
                                                                    <div class="card-header text-black">
                                                                        <div class="row">
                                                                            <div class="col-md-6">
                                                                                <asp:Label ID="lblPRHeader"
                                                                                    runat="server" Font-Bold="true" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-md-6">
                                                                                <asp:Label ID="lblItem"
                                                                                    runat="server" Font-Bold="true" />
                                                                            </div>
                                                                        </div>
                                                                        <div class="row">
                                                                            <div class="col-md-2">
                                                                                Requested Qty:
                                                <asp:Label ID="lblReqQty" runat="server" />
                                                                            </div>
                                                                            <div class="col-md-2">
                                                                                Ordered Qty :
                                                <asp:Label ID="lblOrderedQty" runat="server" />
                                                                            </div>
                                                                            <div class="col-md-2">
                                                                                Received Qty:
                                                <asp:Label ID="lblReceivedQty" runat="server" />
                                                                            </div>

                                                                            <div class="col-md-2">
                                                                                Pending PO:
                                                <asp:Label ID="lblPendingPO" runat="server" />
                                                                            </div>
                                                                            <div class="col-md-2">
                                                                                Pending Receipt
                                                <asp:Label ID="lblPendingReceipt" runat="server" />
                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                    <div class="card-body">
                                                                        <asp:Repeater ID="rptPO" runat="server" OnItemDataBound="rptPO_ItemDataBound">

                                                                            <HeaderTemplate>
                                                                                <div class="ms-4">

                                                                                    <strong>Purchase Orders</strong>
                                                                                    <table class="table table-bordered table-hover">
                                                                                        <thead class="table-light">
                                                                                            <tr>
                                                                                                <th>PO No</th>
                                                                                                <th>PO Date</th>
                                                                                                <th>Vendor</th>
                                                                                                <th>Ordered Qty</th>
                                                                                                <th>Received Qty</th>
                                                                                                <th>Pending Qty</th>
                                                                                                <th>Status</th>
                                                                                            </tr>
                                                                                        </thead>
                                                                            </HeaderTemplate>

                                                                            <ItemTemplate>

                                                                                <!-- PO Row -->

                                                                                <tr>

                                                                                    <td>
                                                                                        <asp:HyperLink ID="hypPO" runat="server" Text='<%# Eval("PONumber").ToString() %>' Target="_blank" NavigateUrl='<%# Eval("PurchaseOrderID","GeneratePO.aspx?POID={0}") %>'></asp:HyperLink>
                                                                                    </td>

                                                                                    <td><%# Convert.ToDateTime(Eval("PODate")).ToString("dd-MMM-yyyy") %></td>

                                                                                    <td><%# Eval("VendorName") %></td>

                                                                                    <td><%# Eval("OrderedQty") %></td>

                                                                                    <td><%# Eval("ReceivedQty") %></td>

                                                                                    <td><%# Eval("PendingQty") %></td>

                                                                                    <td>
                                                                                        <span class="badge bg-success">
                                                                                            <%# Eval("Status") %>
                                                                                        </span>
                                                                                    </td>

                                                                                </tr>

                                                                                <!-- GRN Row -->

                                                                                <tr>

                                                                                    <td colspan="7">

                                                                                        <div class="ms-4">

                                                                                            <strong>GRN Details for <%# Eval("PONumber") %>
                                                                                            </strong>

                                                                                            <asp:Repeater
                                                                                                ID="rptGRN"
                                                                                                runat="server"
                                                                                                OnItemCommand="rptGRN_ItemCommand">

                                                                                                <HeaderTemplate>

                                                                                                    <table class="table table-sm table-bordered mt-2">

                                                                                                        <tr class="table-secondary">

                                                                                                            <th>GRN No</th>

                                                                                                            <th>GRN Date</th>

                                                                                                            <th>Received Qty</th>

                                                                                                            <th>Received By</th>

                                                                                                        </tr>
                                                                                                </HeaderTemplate>

                                                                                                <ItemTemplate>

                                                                                                    <tr>

                                                                                                        <td>
                                                                                                            <asp:LinkButton ID="lnkShow" runat="server" Text='<%# Eval("GRNNumber") %>' CommandName="ItemShow"
                                                                                                                CommandArgument='<%# Eval("GRNID") %>'>    
                                                                                                            </asp:LinkButton>
                                                                                                        </td>

                                                                                                        <td>
                                                                                                            <%# Convert.ToDateTime(Eval("ReceivedDate")).ToString("dd-MMM-yyyy") %>
                                                                                                        </td>

                                                                                                        <td><%# Eval("Quantity") %></td>
                                                                                                        <td><%# Eval("EmployeeName") %></td>
                                                                                                    </tr>

                                                                                                </ItemTemplate>

                                                                                                <FooterTemplate>
                                                                                                    </table>

                                                                                                </FooterTemplate>

                                                                                            </asp:Repeater>
                                                                                            <br />
                                                                                            <asp:Label ID="lblNoGRN"
                                                                                                runat="server"
                                                                                                CssClass="text-muted fst-italic"
                                                                                                Visible="false"
                                                                                                Text="No GRN has been created for this PO." />
                                                                                        </div>

                                                                                    </td>

                                                                                </tr>

                                                                            </ItemTemplate>

                                                                            <FooterTemplate>
                                                                                </table>

                                                                            </FooterTemplate>

                                                                        </asp:Repeater>
                                                                        <br />
                                                                        <asp:Label ID="lblNoPO"
                                                                            runat="server"
                                                                            CssClass="text-muted fst-italic"
                                                                            Visible="false"
                                                                            Text="No PO has been created for this Purchase Requisition." />
                                                                    </div>

                                                                </asp:Panel>
                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>



                                            </div>

                                            <!-- TIMELINE -->

                                            <div class="card card-dashboard mt-4">

                                                <div class="card-header">
                                                    Procurement Timeline

                                                </div>

                                                <div class="card-body">

                                                    <div class="timeline">

                                                        <div class="timeline-step">

                                                            <i class="material-icons">assignment</i>

                                                            <div>PR</div>

                                                        </div>

                                                        <div class="timeline-step">

                                                            <i class="material-icons">mail</i>

                                                            <div>Enquiry</div>

                                                        </div>

                                                        <div class="timeline-step">

                                                            <i class="material-icons">description</i>

                                                            <div>Quotation</div>

                                                        </div>

                                                        <div class="timeline-step">

                                                            <i class="material-icons">shopping_cart</i>

                                                            <div>PO</div>

                                                        </div>

                                                        <div class="timeline-step">

                                                            <i class="material-icons">local_shipping</i>

                                                            <div>GRN</div>

                                                        </div>

                                                        <div class="timeline-step">

                                                            <i class="material-icons">receipt_long</i>

                                                            <div>Invoice</div>

                                                        </div>

                                                    </div>

                                                </div>

                                            </div>

                                        </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnExport" />
                            </Triggers>

                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>




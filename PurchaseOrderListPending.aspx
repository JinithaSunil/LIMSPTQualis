<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PurchaseOrderListPending.aspx.cs" Inherits="LIMS.PurchaseOrderListPending" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-control {
            width: 100%;
        }

        .auto-style1 {
            display: block;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            -o-transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
            background-image: none;
        }

        .card {
            margin-bottom: 30px;
            border: none;
            border-radius: 5px;
            box-shadow: 0 0.46875rem 2.1875rem rgba(90,97,105,0.1), 0 0.9375rem 1.40625rem rgba(90,97,105,0.1), 0 0.25rem 0.53125rem rgba(90,97,105,0.12), 0 0.125rem 0.1875rem rgba(90,97,105,0.1);
        }

        .violet {
            border-top: 3px solid #9c27b0;
            margin: 0;
        }

        .header {
            padding: 20px;
            margin-bottom: 20px;
        }

            .header h2 {
                margin: 0;
                color: #444;
                font-size: 24px;
            }

        .body {
            padding: 20px;
        }

        .btn-group {
            margin-bottom: 20px;
            margin-right: 10px;
        }

        .btn-secondary {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
            margin-right: 5px;
        }

            .btn-secondary:hover {
                background-color: #5a6268;
                border-color: #545b62;
            }

        .btn-primary.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
            background-color: #2196F3;
            border-color: #2196F3;
        }

        .btn-danger.btn-xs {
            padding: 1px 5px;
            font-size: 12px;
            line-height: 1.5;
            border-radius: 3px;
        }

        .table {
            margin-bottom: 0;
        }

        .table-hover tbody tr:hover {
            background-color: rgba(0,0,0,.075);
        }

        .table-responsive {
            min-height: .01%;
            overflow-x: auto;
            max-height: 400px;
            overflow-y: auto;
        }

        .hyperlink-column a {
            color: #2196F3;
            text-decoration: none;
        }

            .hyperlink-column a:hover {
                text-decoration: underline;
            }

        .btn-primary {
            color: #fff;
            background-color: #2196F3;
            border-color: #2196F3;
            margin-right: 5px;
        }

            .btn-primary:hover {
                background-color: #0b7dda;
                border-color: #0b7dda;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>PO LIST</h2>
                </div>
                <div class="body">
                    <div class="row clearfix">

                        <div class="col-sm-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <label for="txtFromDate">From Date</label>
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control"></asp:TextBox>
                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" TargetControlID="txtFromDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <label for="txtToDate">To Date</label>
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control"></asp:TextBox>
                                    <ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" TargetControlID="txtToDate" Format="dd/MMM/yyyy"></ajaxToolkit:CalendarExtender>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4">
                            <div class="form-group">
                                <div class="form-line">
                                    <label for="txtAmountGreaterThan">Amount > </label>
                                    <asp:TextBox ID="txtAmountGreaterThan" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-sm-3">
                            <div class="form-group">
                                <div class="form-line">

                                    <asp:DropDownList ID="ddlVendor" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <div class="form-line">
                                    <asp:TextBox ID="txtponumber" runat="server" placeholder="PO Number" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-3">
                            <div class="btn-group">
                                <asp:Button ID="btnGet" runat="server" Text="Find" OnClick="btnGet_Click" CssClass="btn btn-primary" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" CssClass="btn btn-secondary" />
                                <asp:Button ID="btnDownload" runat="server" Text="Download Excel" CssClass="btn btn-success" OnClick="btnDownload_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="row clearfix">
                        <div class="col-12">
                            <asp:Label ID="lblRecords" runat="server"></asp:Label>
                            <div class="table-responsive">
                                <asp:GridView ID="gvPO" runat="server" class="table table-bordered table-striped table-hover"
                                    AllowSorting="true" OnSorting="gvPO_Sorting" DataKeyNames="PurchaseOrderID" OnRowDataBound="gvPO_RowDataBound"
                                    AutoGenerateColumns="false" AllowPaging="true" PageSize="200" OnRowCommand="gvPO_RowCommand">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="1%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO#" SortExpression="PONumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPurchaseOrderID" runat="server" Text='<%# Eval("PurchaseOrderID").ToString() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblPONumber" runat="server" Text='<%# Eval("PONumber").ToString() %>' Visible="false"></asp:Label>
                                                <asp:Label ID="lblReportFileName" runat="server" Text='<%# Eval("ReportFileName").ToString() %>' Visible="false"></asp:Label>
                                                <asp:HyperLink ID="hypPO" runat="server" Text='<%# Eval("PONumber").ToString() %>' Target="_blank" NavigateUrl='<%# Eval("PurchaseOrderID","GeneratePO.aspx?POID={0}") %>'></asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle Width="13%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO Date" SortExpression="PODate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPODate" runat="server" Text='<%# Eval("PODate","{0:dd MMM yyyy}").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Delivery Date(Max)" SortExpression="DeliveryDate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDeliveryDate" runat="server" Text='<%# Eval("DeliveryDate","{0:dd MMM yyyy}").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Vendor Name" SortExpression="VendorName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval("VendorName").ToString() %>'></asp:Label>
                                                <asp:Label ID="lblVendorID" runat="server" Text='<%# Eval("VendorID").ToString() %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="15%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPOstatus" runat="server" Text='<%# Eval("POStatus").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ItemDescription" HeaderText="Item" ItemStyle-Width="15%" />
                                        <asp:TemplateField HeaderText="Quantity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="SecurityReceived" HeaderText="Sec.Recvd" ItemStyle-Width="5%" />
                                        <asp:TemplateField HeaderText="PORecvdQty">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPORecvdQty" runat="server" Text='<%# Eval("POReceivedQty").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Price" HeaderText="Price" ItemStyle-Width="5%" />
                                        <asp:TemplateField HeaderText="Days Left" ItemStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDaysLeft" runat="server"></asp:Label><br />
                                                <asp:Button ID="btnReminder" runat="server" CommandName="Reminder" Text="Reminder" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' CssClass="btn btn-danger btn-xs" Visible="false" OnClientClick="return confirm('Are you sure you want to send a Reminder for Pending PO?');" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                    <asp:GridView ID="gvDump" runat="server" Visible="false"></asp:GridView>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


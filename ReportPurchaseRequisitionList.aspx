<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="ReportPurchaseRequisitionList.aspx.cs" Inherits="LIMS.ReportPurchaseRequisitionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-size: 12px;
        }

        .status-dot {
            display: inline-block;
            width: 18px;
            height: 18px;
            border-radius: 50%;
            margin-right: 10px;
            vertical-align: middle;
            box-shadow: inset 0 1px 3px rgba(255,255,255,.5), 0 1px 3px rgba(0,0,0,.25);
        }

        .dot-red {
            background: radial-gradient(circle at 30% 30%, #ff8d8d, #e53935);
        }

        .dot-yellow {
            background: radial-gradient(circle at 30% 30%, #ffe98a, #f4b400);
        }

        .dot-green {
            background: radial-gradient(circle at 30% 30%, #8df0a8, #2ecc71);
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
                    <div class="row">
                        <div class="col-sm-7">
                            <h2>PURCHASE REQUISITION LIST</h2>
                        </div>
                    </div>
                </div>
                <div class="row-clearfix">
                    <div class="well" style="background: #fafafa; border: 1px solid #e5e5e5; padding: 20px; border-radius: 8px;">
                        <!-- Filters -->
                        <div class="row row-clearfix mb-3">

                            <div class="col-md-3">
                                <label>From Date</label>
                                <asp:TextBox ID="txtFromDate" runat="server"
                                    TextMode="Date"
                                    CssClass="form-control" />
                            </div>

                            <div class="col-md-3">
                                <label>To Date</label>
                                <asp:TextBox ID="txtToDate" runat="server"
                                    TextMode="Date"
                                    CssClass="form-control" />
                            </div>

                            <div class="col-md-3 d-flex align-items-end">
                                <br />
                                <asp:LinkButton ID="btnSearch" runat="server"
                                    CssClass="btn btn-primary"
                                    OnClick="btnSearch_Click">
                                    <i class="material-icons" style="font-size:18px;vertical-align:middle;">search</i>
                                    Search
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnReset" runat="server"
                                    CssClass="btn btn-default"
                                    OnClick="btnReset_Click">
                                    <i class="material-icons" style="font-size:18px;vertical-align:middle;">refresh</i>
                                    Reset
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnExport" runat="server"
                                    CssClass="btn btn-success"
                                    OnClick="btnExport_Click">
                                <i class="material-icons" style="font-size:18px;vertical-align:middle;">download</i>
                                Export
                                </asp:LinkButton>
                            </div>
                            <div class="col-md-3">
                                <label>Search</label>
                                <div class="input-group">

                                    <span class="input-group-addon">
                                        <i class="material-icons">search</i>
                                    </span>

                                    <asp:TextBox ID="txtSearch"
                                        runat="server"
                                        CssClass="form-control"
                                        placeholder="Search..."
                                        AutoPostBack="true"
                                        OnTextChanged="txtSearch_TextChanged" />

                                </div>
                            </div>
                            <br />
                            <div class="row">

                                <div class="col-md-3">
                                    <label>Current Tier</label>
                                    <asp:DropDownList ID="ddlTier" runat="server"
                                        CssClass="form-control"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="FilterChanged">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <label>Process Status</label>
                                    <asp:DropDownList ID="ddlProcessStatus"
                                        runat="server"
                                        CssClass="form-control"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="FilterChanged">

                                        <asp:ListItem Text="--All--" Value=""></asp:ListItem>
                                        <asp:ListItem Text="PR Pending" Value="PR Pending"></asp:ListItem>
                                        <asp:ListItem Text="PO Created" Value="PO Created"></asp:ListItem>
                                        <asp:ListItem Text="GRN Completed" Value="GRN Completed"></asp:ListItem>

                                    </asp:DropDownList>

                                </div>
                                <div class="col-md-3">
                                    <label>PO Number</label>
                                    <asp:DropDownList ID="ddlPO" runat="server"
                                        CssClass="form-control"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="FilterChanged">
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-3">
                                    <label>GRN Number</label>
                                    <asp:DropDownList ID="ddlGRN" runat="server"
                                        CssClass="form-control"
                                        AutoPostBack="true"
                                        OnSelectedIndexChanged="FilterChanged">
                                    </asp:DropDownList>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row-clearfix">
                    <div>
                        Total records: <span id="spanCount" runat="server"></span>
                        <span style="float: right">
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
                        </span>
                    </div>
                </div>
                <div class="row-clearfix table-responsive">
                    <div class="col-12">
                        <asp:GridView ID="gvList" runat="server" Width="100%"  OnRowDataBound="gvList_RowDataBound"
                            class="table table-bordered table-striped table-hover"
                            AutoGenerateColumns="false" AllowSorting="true"
                            OnSorting="gvList_Sorting"
                            AllowPaging="true"
                            AllowCustomPaging="true"
                            PageSize="10"
                            OnPageIndexChanging="gvList_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField HeaderText="Sl#" HeaderStyle-Width="1%">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle Width="1%" HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Process Status">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkProcessStatus"
                                            runat="server"
                                            CssClass="status-link"
                                            OnCommand="lnkProcessStatus_Command">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="PRNumber" HeaderText="PRNumber" SortExpression="PRNumber" />
                                <asp:BoundField DataField="RequestDate" HeaderText="RequestDate" SortExpression="RequestDate" DataFormatString="{0:dd-MM-yyyy}" />
                                <asp:BoundField DataField="RequestBy" HeaderText="RequestBy" SortExpression="RequestBy" />
                                <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="EmployeeDepartmentName" SortExpression="EmployeeDepartmentName" />
                                <asp:BoundField DataField="ItemCode" HeaderText="ItemCode" SortExpression="ItemCode" />
                                <asp:BoundField DataField="ItemName" HeaderText="ItemName" SortExpression="ItemName" />
                                <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
                                <asp:BoundField DataField="UOMName" HeaderText="UOMName" SortExpression="UOMName" />
                                <asp:BoundField DataField="RequiredOn" HeaderText="RequiredOn" SortExpression="RequiredOn" DataFormatString="{0:dd-MM-yyyy}" />
                                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                                <asp:BoundField DataField="CurrentTier" HeaderText="CurrentTier" SortExpression="CurrentTier" />
                                <asp:BoundField DataField="CurrentApproverRole" HeaderText="CurrentApproverRole" SortExpression="CurrentApproverRole" />
                                <asp:BoundField DataField="Remarks" HeaderText="Remarks" SortExpression="Remarks" ItemStyle-Width="5%" />
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
                </div>
            </div>
        </div>
    </div>
</asp:Content>

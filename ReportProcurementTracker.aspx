<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="ReportProcurementTracker.aspx.cs" Inherits="LIMS.ReportProcurementTracker" %>

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
                            <h2>PURCHASE TRACKER</h2>
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
                        </div>
                        <div class="row">
                            <div class="dol-md-6">
                                Pending: 
                                <asp:RadioButtonList ID="radOption" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radOption_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Selected="True">ENQUIRY</asp:ListItem>
                                    <asp:ListItem>QUOTE</asp:ListItem>
                                    <asp:ListItem>PO</asp:ListItem>
                                </asp:RadioButtonList>
                                <asp:HiddenField ID="hdType" runat="server" />
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
                                <asp:ListItem Text="500" Value="500" />
                            </asp:DropDownList>
                            <span>records</span>
                        </span>
                    </div>
                </div>
                <div class="row-clearfix table-responsive">
                    <div class="col-12">
                        <asp:GridView ID="gvList" runat="server" Width="100%" OnRowDataBound="gvList_RowDataBound"
                            class="table table-bordered table-striped table-hover"
                            AutoGenerateColumns="false" AllowSorting="true"
                            OnSorting="gvList_Sorting"
                            AllowPaging="true"
                            AllowCustomPaging="true"
                            PageSize="10"
                            OnPageIndexChanging="gvList_PageIndexChanging">
                            <Columns>
                                <asp:BoundField DataField="PRNumber" HeaderText="PRNumber" SortExpression="PRNumber" />
                                <asp:BoundField DataField="RequestDate" HeaderText="RequestDate" SortExpression="RequestDate" DataFormatString="{0:dd-MM-yyyy}" />
                                <asp:BoundField DataField="RequiredOn" HeaderText="RequiredOn" SortExpression="RequiredOn" DataFormatString="{0:dd-MM-yyyy}" />
                                <asp:BoundField DataField="EnquiryID" HeaderText="Enquiry#" SortExpression="EnquiryID" />
                                <asp:BoundField DataField="QuotationCount" HeaderText="QuotationCount" SortExpression="QuotationCount" />
                                <asp:BoundField DataField="POCount" HeaderText="POCount" SortExpression="POCount" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

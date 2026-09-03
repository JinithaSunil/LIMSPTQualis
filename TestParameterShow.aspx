<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="TestParameterShow.aspx.cs" Inherits="LIMS.TestParameterShow" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .table-container {
            overflow-x: auto;
            overflow-y: visible;
            max-width: 100%;
            border: 1px solid #dee2e6;
            border-radius: 0.375rem;
        }
        
        .table-container::-webkit-scrollbar {
            height: 8px;
        }
        
        .table-container::-webkit-scrollbar-track {
            background: #f1f1f1;
            border-radius: 10px;
        }
        
        .table-container::-webkit-scrollbar-thumb {
            background: #888;
            border-radius: 10px;
        }
        
        .table-container::-webkit-scrollbar-thumb:hover {
            background: #555;
        }
        
        .table-fixed {
            min-width: 1200px; /* Adjust based on your total column widths */
            margin-bottom: 0;
        }
        
        .table-fixed th,
        .table-fixed td {
            white-space: nowrap;
            vertical-align: middle;
        }
        
        /* Make the first column (Serial #) sticky */
        .table-fixed th:first-child,
        .table-fixed td:first-child {
            position: sticky;
            left: 0;
            background-color: #fff;
            z-index: 10;
            border-right: 2px solid #dee2e6;
        }
        
        .table-fixed th:first-child {
            background-color: #f8f9fa;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="page-wrapper">
        <div class="page-content">
            <!-- Menu Card -->
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>TEST PARAMETER LIST</h2>
                </div>

                <!-- Table -->
                <div class="card">
                    <div class="card-body">
                        <div class="container-fluid mt-4">

                            <!-- Search bar -->
                            <div class="row mt-4 mb-4" style="margin-top: 23px; margin-bottom: 5px;">
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Department:</label>
                                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control"
                                                AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                                                <asp:ListItem Text="--Select Department--" Value="" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Parameter Name" Style="margin-top: 28px;" />
                                </div>

                                <div class="col-md-2 text-end">
                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" Style="margin-top: 28px;">
                                <i class="fa fa-search me-1" style="font-size:20px;"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnReset" runat="server" CssClass="btn btn-secondary ms-2" OnClick="btnReset_Click" Style="margin-top: 28px; margin-left: 20px;">
                                <i class="fa fa-rotate-right me-1"></i> 
                                    </asp:LinkButton>
                                </div>
                                <div class="col-md-2">
                                    <div class="btn-group" role="group" style="margin-top: 22px; margin-bottom: -3px;">
                                        <a href="AddTestParameters.aspx" class="btn btn-primary mb-3 mb-md-0"><i class='bx bxs-plus-square'></i>ADD NEW</a>
                                    </div>
                                </div>
                            </div>

                            <!-- Grid View with Horizontal Scroll -->
                            <div class="table-container">
                                <asp:GridView ID="GridView1" runat="server"
                                    AutoGenerateColumns="false"
                                    AllowPaging="true"
                                    PageSize="10"
                                    CssClass="table table-bordered table-striped table-fixed"
                                    OnPageIndexChanging="GridView1_PageIndexChanging"
                                    OnRowCommand="GridView1_RowCommand"
                                    HeaderStyle-CssClass="table-light">
                                    <Columns>
                                        <asp:TemplateField HeaderText="#" ItemStyle-Width="50px" HeaderStyle-Width="50px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="TestParameterId" HeaderText="ID" ItemStyle-Width="60px" HeaderStyle-Width="60px" />
                                        <asp:BoundField DataField="TestParameter" HeaderText="Test Parameter" ItemStyle-Width="200px" HeaderStyle-Width="200px" />
                                        <asp:BoundField DataField="Department" HeaderText="Department" ItemStyle-Width="120px" HeaderStyle-Width="120px" />
                                        <asp:BoundField DataField="UOMName" HeaderText="UOM" ItemStyle-Width="80px" HeaderStyle-Width="80px" />
                                        <asp:BoundField DataField="Protocol" HeaderText="Test Method" ItemStyle-Width="140px" HeaderStyle-Width="140px" />
                                        <asp:BoundField DataField="Specification" HeaderText="Specification" ItemStyle-Width="120px" HeaderStyle-Width="120px" />
                                        <asp:BoundField DataField="Requirement" HeaderText="Requirement" ItemStyle-Width="140px" HeaderStyle-Width="140px" />
                                        <asp:TemplateField HeaderText="Other Details" ItemStyle-Width="180px" HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <div style="white-space: pre-line; font-size: 12px; margin: 0; padding: 5px; line-height: 1.2; max-width: 180px; overflow-wrap: break-word;">
                                                    <%# (Eval("FormattedOtherDetails") ?? "").ToString().Trim() %>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Edit" ItemStyle-Width="60px" HeaderStyle-Width="60px">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEdit" runat="server" CommandName="ItemEdit"
                                                    CommandArgument='<%# Eval("TestParameterId") %>'
                                                    ToolTip="Edit" CssClass="btn btn-sm btn-default waves-effect">
                                                <i class="material-icons">edit</i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle CssClass="pagination-ys" HorizontalAlign="Center" />
                                    <PagerSettings Mode="NumericFirstLast" FirstPageText="« First" LastPageText="Last »" Position="Bottom" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

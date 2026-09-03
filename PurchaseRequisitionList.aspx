<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PurchaseRequisitionList.aspx.cs" Inherits="LIMS.PurchaseRequisitionList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .status-circle {
            width: 32px;
            height: 32px;
            border-radius: 50%;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            color: #fff;
        }

        .approved {
            background: #2ecc71;
        }

        .reject {
            background: #e74c3c;
        }

        .new {
            background: #f1c40f;
            color: #000;
        }

        .disabled {
            background: #ddd;
            color: #666;
        }

        .status-text.green {
            color: #2ecc71;
            font-weight: 600;
        }

        .status-text.red {
            color: #e74c3c;
            font-weight: 600;
        }

        .status-box {
            border: 1px solid #ddd;
            border-radius: 12px;
            padding: 10px;
        }

        .status-title {
            font-weight: 600;
            color: #7b3fe4;
            margin-bottom: 6px;
        }

        .status-row {
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .form-control {
            width: 100%;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            border-radius: 4px;
            -webkit-box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, .075);
            -webkit-transition: border-color ease-in-out .15s, -webkit-box-shadow ease-in-out .15s;
            transition: border-color ease-in-out .15s, box-shadow ease-in-out .15s;
            border: 1px solid #ccc;
            padding: 6px 12px;
            background-color: #fff;
        }

        body {
            font-size: 14px;
        }

        .card {
            background: #fff;
            min-height: 50px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
            position: relative;
            margin-bottom: 30px;
            border-radius: 2px;
        }

            .card .header {
                padding: 20px;
                border-bottom: 1px solid rgba(204, 204, 204, 0.35);
            }

            .card .body {
                padding: 20px;
            }

        hr.violet {
            border-top: 3px solid #7b1fa2;
            margin-top: 0;
        }

        .btn-secondary {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
            margin-right: 5px;
        }

        .btn-primary {
            color: #fff;
            background-color: #007bff;
            border-color: #007bff;
        }

        .table {
            width: 100%;
            margin-bottom: 1rem;
            background-color: transparent;
        }

        .table-striped tbody tr:nth-of-type(odd) {
            background-color: rgba(0, 0, 0, 0.05);
        }

        .padding-0 {
            padding: 0;
        }

        /* Define general pill style */
        .pill {
            display: inline-block;
            padding: 6px 16px;
            border-radius: 20px;
            font-size: 13px;
            font-weight: 500;
            color: white;
            text-align: center;
            min-width: 120px;
            white-space: nowrap;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }

            /* Specific status colors */
            .pill.New {
                background-color: #f0ad4e; /* Orange */
                color: white;
            }

            .pill.Approved {
                background-color: #3a753a; /* Green */
            }

            .pill.Rejected {
                background-color: #d9534f; /* Red */
            }

            .pill.ApprovedByGM {
                background-color: #5cb85c; /* Light Green */
            }

            .pill.ApprovedByStores {
                background-color: #43d4b5; /* Cyan */
            }

            .pill.ApprovedByHOD {
                background-color: #185794; /* Bright Blue */
                color: white;
            }
    </style>
    <script>
        function imgrefresh_Click() {
            // Your refresh logic here
            console.log("Refresh clicked!"); // Test if it works
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">

                    <div class="row">
                        <div class="col-sm-6">
                            <h2>PURCHASE REQUISITION LIST</h2>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBox1" runat="server" class="form-control" placeholder="Search Box"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="assets\img\search.png" OnClick="imgSearch_Click" />
                        </div>
                    </div>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <!-- Project and PR Type Selection -->
                            <div class="row clearfix">
                                <div class="col-sm-8">
                                    <div class="form-group">
                                        <div class="form-line" style="visibility: hidden">
                                            <label>Select PR Type:</label>
                                            <asp:DropDownList ID="ddlPRType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPR_SelectedIndexChanged">
                                                <asp:ListItem Value="All" Selected="True">All</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-1">
                                    <div class="form-group" style="margin-top: 25px; margin-left: 60px;">
                                        <a href="<%: Request.RawUrl %>">
                                            <i class="fa-solid fa-rotate" style="color: purple; font-size: 30px;"></i>
                                        </a>
                                    </div>
                                </div>

                                <div class="col-sm-1">
                                    <div class="form-group" style="margin-top: 24px; margin-left: 35px;">
                                        <asp:Button ID="btnDownload" runat="server" Text="Download" OnClick="btnDownload_Click" CssClass="btn btn-primary" />
                                    </div>
                                </div>
                            </div>

                            <!-- GridView Section -->

                            <div style="justify-content: center; margin: 10px 0;">
                                <asp:GridView ID="gvRequests" runat="server"
                                    AutoGenerateColumns="False"
                                    Font-Bold="True"
                                    AllowPaging="true"
                                    PageSize="5"
                                    AllowSorting="true"
                                    CssClass="table table-bordered table-striped table-hover js-basic-example dataTable"
                                    Width="100%"
                                    OnRowDataBound="gvRequests_RowDataBound"
                                    OnPageIndexChanging="gvRequests_PageIndexChanging"
                                    OnSorting="gvRequests_Sorting">

                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="Bottom" />
                                    <PagerStyle CssClass="pagination-style" HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 + (gvRequests.PageIndex * gvRequests.PageSize) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PR#">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypPRID" runat="server"
                                                    Text='<%# Eval("PR") %>'
                                                    NavigateUrl='<%# Eval("PRID", "GeneratePurchaseRequisitionReport.aspx?ID={0}") %>'
                                                    Target="_blank" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Emp. Name" ItemStyle-Width="16%" SortExpression="EmployeeName" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-Width="25%" SortExpression="Description" />
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="Dpt. Name" ItemStyle-Width="12%" SortExpression="EmployeeDepartmentName" />
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd-MMM-yyyy}" ItemStyle-Width="12%" SortExpression="RequestDate" />
                                        <asp:TemplateField HeaderText="Status" HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hdPR" runat="server" Value='<%#Eval("PRID") %>' />
                                                <asp:HiddenField ID="hdPRD" runat="server" Value='<%#Eval("PurchaseRequisitionID") %>' />
                                                <asp:Repeater ID="rptStatus" runat="server" OnItemDataBound="rptStatus_ItemDataBound">
                                                    <ItemTemplate>
                                                        <div class="status-box">
                                                            <table width="100%" style="text-align: center">
                                                                <tr>
                                                                    <td colspan="4">
                                                                        <div class="status-title">Status PR</div>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Dept</td>
                                                                    <td>IT/HCGA</td>
                                                                    <td>Dir Sales/Lab</td>
                                                                    <td>Director</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:HiddenField ID="hdStatusID" runat="server" Value='<%#Eval("StatusID") %>' />
                                                                        <asp:Label ID="lblDeptStatus" runat="server" Text='<%# Eval("DeptStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblITHCGAStatus" runat="server" Text='<%# Eval("ITHCGAStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblDirSalesStatus" runat="server" Text='<%# Eval("DirSalesStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Label ID="lblDirectorStatus" runat="server" Text='<%# Eval("DirectorStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <br />
                                                            <table width="100%">
                                                                <tr>
                                                                    <td colspan="3" style="text-align: center" class="status-title">Status PO</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>Quote</td>
                                                                    <td>PO</td>
                                                                    <td>GRN</td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="status-text <%# Eval("QuoteStatus").ToString()=="Create"?"green":"red" %>">
                                                                        <asp:Label ID="lblQuote" runat="server" Text='<%# Eval("QuoteStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                    <td class="status-text <%# Eval("POStatus").ToString()=="Create"?"green":"red" %>">
                                                                        <asp:Label ID="lblPO" runat="server" Text='<%# Eval("POStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                    <td class="status-text <%# Eval("GRNStatus").ToString()=="Create"?"green":"red" %>">
                                                                        <asp:Label ID="lblGRN" runat="server" Text='<%# Eval("GRNStatus") ?? "-" %>'></asp:Label>
                                                                    </td>
                                                                </tr>
                                                            </table>

                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>

                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Width="17%" SortExpression="Status" HtmlEncode="false" />--%>
                                        <%--<asp:BoundField DataField="PRType" HeaderText="PR Type" ItemStyle-Width="8%" SortExpression="PRType" />--%>
                                        <asp:BoundField DataField="StoreType" HeaderText="Item Type" ItemStyle-Width="10%" SortExpression="StoreType" />
                                    </Columns>
                                </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

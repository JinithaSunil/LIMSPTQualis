<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="StoreEnquiries.aspx.cs" Inherits="LIMS.StoreEnquiries" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- JavaScript and jQuery references -->
    <script src="assets/js/data-basic-custom.js" type="text/javascript"></script>
    <script type="text/javascript">
        function showEditOptions(btn) {
            var row = btn.closest('tr'); // Find the row where the Edit button was clicked
            var approveBtn = row.querySelector('[id$="lnkApprove"]');
            var rejectBtn = row.querySelector('[id$="lnkReject"]');
            var ddlStatus = row.querySelector('[id$="ddlStatus"]');
            var txtRemarks = row.querySelector('[id$="txtRemarks"]');

            // Show the hidden elements
            approveBtn.style.display = 'inline-block';
            rejectBtn.style.display = 'inline-block';
            ddlStatus.style.display = 'inline-block';
            txtRemarks.style.display = 'inline-block';
        }
    </script>

    <style>
        .form-control {
            width: 100%;
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

        table tbody tr th {
            text-align: center !important;
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

                .card .header h2 {
                    margin: 0;
                    font-size: 18px;
                    font-weight: normal;
                    color: #111;
                }

        hr.violet {
            border-top: 3px solid #7b1fa2;
            margin-top: 0;
        }

        .btn-primary {
            color: #fff;
            background-color: #3F51B5;
            border-color: #3F51B5;
        }

        .btn-warning {
            color: #fff;
            background-color: #ff9800;
            border-color: #ff9800;
        }

        .table-bordered {
            border: 1px solid #ddd;
        }

        .table-striped > tbody > tr:nth-of-type(odd) {
            background-color: #f9f9f9;
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
                    <h2>PURCHASE REQUISITION LIST</h2>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <!-- Filter Section -->
                            <div class="row clearfix">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>Purchase Group:</label>
                                            <asp:DropDownList ID="ddlPurchaseGroup" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlPurchaseGroup_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <div class="form-line">
                                            <label>PR ID:</label>
                                            <asp:TextBox ID="txtPRID" runat="server" CssClass="form-control" placeholder="Enter PRID" OnTextChanged="txtPRID_TextChanged" AutoPostBack="true">
                                            </asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkAll" runat="server" Text="Show All" OnCheckedChanged="chkAll_CheckedChanged" AutoPostBack="true" />
                                    </div>
                                </div>
                            </div>

                            <!--Grid-->
                            <div class="row clearfix">
                                <div class="col-sm-12">
                                    <!--1st grid()-->
                                    <asp:GridView ID="gvEnquiry" runat="server" AutoGenerateColumns="False"
                                        class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                        Width="100%" AllowPaging="true" PageSize="10"
                                        OnPageIndexChanging="gvEnquiry_PageIndexChanging" OnRowDataBound="gvEnquiry_RowDataBound">
                                        <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="Bottom" />
                                        <PagerStyle CssClass="pagination-style" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sl#">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="PR" HeaderText="PR#" />
                                            <asp:TemplateField HeaderText="PRID">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="hypPRID" runat="server"
                                                        Text='<%# Eval("PRID") %>'
                                                        NavigateUrl='<%# Eval("PRID", "GeneratePurchaseRequisitionReport.aspx?ID={0}") %>'
                                                        Target="_blank" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Department" HeaderText="Department" />
                                            <asp:BoundField DataField="Group" HeaderText="Group" />
                                            <asp:BoundField DataField="Sub Group" HeaderText="Sub Group" />
                                            <asp:BoundField DataField="Item" HeaderText="Item" />
                                            <asp:BoundField DataField="Purpose" HeaderText="Purpose" />
                                            <asp:BoundField DataField="UOM" HeaderText="UOM" />
                                            <asp:BoundField DataField="Stock" HeaderText="Stock" />

                                            <asp:TemplateField HeaderText="Status">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                                        <asp:ListItem Text="Pending Approval" Value="" />
                                                        <asp:ListItem Text="Approved" Value="1005" />
                                                        <asp:ListItem Text="Rejected" Value="1007" />
                                                    </asp:DropDownList>
                                                    <asp:Button ID="btnChangeStatus" runat="server"
                                                        CommandArgument='<%# Eval("PR") %>'
                                                        OnClick="btnChangeStatus_Click"
                                                        CssClass="btn btn-primary"
                                                        Text='Change Status' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div class="alert alert-info">No records found.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>


                                    <!--2nd grid-->
                                    <asp:GridView ID="gvAllEnquiries" runat="server" AutoGenerateColumns="False"
                                        class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                        Width="100%" AllowPaging="true"
                                        PageSize="10" OnPageIndexChanging="gvEnquiry_PageIndexChanging">
                                        <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="Bottom" />
                                        <PagerStyle CssClass="pagination-style" HorizontalAlign="Center" />
                                        <Columns>
                                            <asp:BoundField DataField="SrNo" HeaderText="Sl#" />
                                            <asp:BoundField DataField="Department" HeaderText="Department" />
                                            <asp:BoundField DataField="Group" HeaderText="Group" />
                                            <asp:BoundField DataField="Sub Group" HeaderText="Sub Group" />
                                            <asp:BoundField DataField="Item" HeaderText="Item" />
                                            <asp:BoundField DataField="PR#" HeaderText="PR#" />
                                            <asp:BoundField DataField="Purpose" HeaderText="Purpose" />
                                            <asp:BoundField DataField="UOM" HeaderText="UOM" />
                                            <asp:BoundField DataField="Stock" HeaderText="Stock" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" />
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div class="alert alert-info">No records found.</div>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

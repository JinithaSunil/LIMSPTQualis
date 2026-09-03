<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PRApproval.aspx.cs" Inherits="LIMS.PRApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
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
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <div class="row" id="divNextApprover" runat="server">
                                <div class="col-sm-2">
                                    Next Approval:
                                </div>
                                <div class="col-sm-3">
                                    <asp:DropDownList ID="ddl" runat="server" CssClass="form-control"></asp:DropDownList>
                                    <asp:HiddenField ID="hdCurrentTier" runat="server" />
                                    <asp:HiddenField ID="hdCurrentApprover" runat="server" />
                                    <asp:HiddenField ID="hdStatus" runat="server" />
                                    <asp:HiddenField ID="hdNextApprovalLevel" runat="server" />
                                </div>

                            </div>
                            <div class="row" id="divDir" runat="server">
                                <div class="col-sm-2">
                                    <asp:LinkButton ID="btnApproveAll" runat="server" Text="Approve All" CommandName="Approve" CssClass="btn btn-success" OnClick="btnApproveAll_Click" Visible="false" />&nbsp;
                                    <asp:LinkButton ID="btnRejectAll" runat="server" Text="Reject All" CommandName="Reject" CssClass="btn btn-danger" OnClick="btnRejectAll_Click" Visible="false" />
                                </div>
                            </div>
                            <br />
                            <div class="row clearfix">
                                <!-- On pageloade grid (HOD) -->
                                <asp:GridView ID="gvRequests" runat="server" DataKeyNames="PurchaseRequisitionID,PRID" Visible="false" AutoGenerateColumns="False" Font-Bold="True"
                                    AllowPaging="false"
                                    AllowCustomPaging="true"
                                    PageSize="5" AllowSorting="true"
                                    class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                    Width="100%" OnRowDataBound="gvRequests_RowDataBound"
                                    OnPageIndexChanging="gvRequests_PageIndexChanging">
                                    <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" Position="Bottom" />
                                    <PagerStyle CssClass="pagination-style" HorizontalAlign="Center" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--                                    <asp:BoundField DataField="PR" HeaderText="PR#" />--%>
                                        <asp:TemplateField HeaderText="PR#">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypPRID" runat="server"
                                                    Text='<%# Eval("PR") %>'
                                                    NavigateUrl='<%# Eval("PRID", "GeneratePurchaseRequisitionReport.aspx?ID={0}") %>' Target="_blank" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Emp. Name" />
                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="Dpt. Name" />
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:HiddenField ID="hfPRDetailID" runat="server"
                                                    Value='<%# Eval("PurchaseRequisitionID") %>' />
                                                <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="false" CssClass="form-control">
                                                </asp:DropDownList>
                                                <%-- <asp:Button ID="btnChangeStatus" runat="server"
                                                    CommandArgument='<%# Eval("PR") %>'
                                                    OnClick="btnChangeStatus_Click"
                                                    CssClass="btn btn-primary"
                                                    Text='Change Status' />--%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--<asp:BoundField DataField="PRType" HeaderText="PR Type" />--%>
                                        <asp:BoundField DataField="StoreType" HeaderText="Item Type" />
                                    </Columns>
                                </asp:GridView>
                                <asp:Button ID="btnSubmitStatus"
                                    runat="server"
                                    Text="Submit Status"
                                    CssClass="btn btn-success"
                                    OnClick="btnSubmitStatus_Click" />
                                <asp:HiddenField ID="hfTotalRows" runat="server" />
                                <asp:HiddenField ID="hfApprovedRows" runat="server" Value="0" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

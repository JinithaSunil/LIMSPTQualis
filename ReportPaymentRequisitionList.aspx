<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="ReportPaymentRequisitionList.aspx.cs" Inherits="LIMS.ReportPaymentRequisitionList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
                            <h2>PAYMENT REQUISITION LIST</h2>
                        </div>
                        </div>
                    </div>
                                  <div class="body">
                    <div class="row">
                        <!-- Filters -->
                        <table class="filterTable" style="text-align: center; font-weight: bold">
                            <tr>
                                <td>From Date</td>
                                <td>To Date</td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control" /></td>
                                <td>
                                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control" /></td>
                                <td>&nbsp;&nbsp;
                                    <asp:Button ID="btnSearch" runat="server"
                                        Text="Search" CssClass="btn btn-primary"
                                        OnClick="btnSearch_Click" />
                                </td>
                                <td>&nbsp;&nbsp;
                                    <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btn btn-primary" OnClick="btnExport_Click" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <br />
                    <div class="row clearfix">
                        <div class="col-12">
                            <div class="table-responsive">
                                <asp:GridView ID="gvList" runat="server" class="table table-bordered table-striped table-hover" AutoGenerateColumns="false" AllowSorting="true" OnSorting="gvList_Sorting">
                                    <Columns>
                                        <asp:BoundField DataField="RequisitionNumber" HeaderText="RequisitionNumber" SortExpression="RequisitionNumber" />
                                        <asp:BoundField DataField="RequisitionDate" HeaderText="RequestDate" SortExpression="RequisitionDate" DataFormatString="{0:dd-MM-yyyy}" />
                                        <asp:BoundField DataField="RequestBy" HeaderText="RequestBy" SortExpression="RequestBy" />
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="EmployeeDepartmentName" SortExpression="EmployeeDepartmentName" />
                                        <asp:BoundField DataField="itemDescription" HeaderText="ItemCode" SortExpression="Item" />
                                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" />
                                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                                        <asp:BoundField DataField="CurrentTier" HeaderText="CurrentTier" SortExpression="CurrentTier" />
                                        <asp:BoundField DataField="CurrentApproverRole" HeaderText="CurrentApproverRole" SortExpression="CurrentApproverRole" />


                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
                </div>
            </div>
        </div>
    

</asp:Content>

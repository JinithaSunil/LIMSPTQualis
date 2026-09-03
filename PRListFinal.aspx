<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PRListFinal.aspx.cs" Inherits="LIMS.PRListFinal" %>

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
                            <h2>PURCHASE REQUISITION LIST</h2>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="TextBox1" runat="server" class="form-control" placeholder="Search PR"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-4">
                            <asp:RadioButtonList ID="radChoice" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radChoice_SelectedIndexChanged" AutoPostBack="true">
                                <asp:ListItem Value="0" Selected="True">Pending</asp:ListItem>
                                <asp:ListItem Value="1">Completed</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <!-- Project and PR Type Selection -->
                            <div class="row clearfix">
                                <!-- On pageloade grid (HOD) -->
                                <asp:GridView ID="gvRequests" runat="server" DataKeyNames="PRID"
                                    Visible="false" AutoGenerateColumns="False" Font-Bold="True"
                                    AllowSorting="true" OnRowCommand="gvRequests_RowCommand"
                                    class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PR#">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypPRID" runat="server"
                                                    Text='<%# Eval("PR") %>'
                                                    NavigateUrl='<%# Eval("PRID", "GeneratePurchaseRequisitionReport.aspx?ID={0}") %>' Target="_blank" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Emp. Name" />
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="Dpt. Name" />
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btnApprove" runat="server"
                                                    Text="Approve"
                                                    CssClass="btn btn-success btn-sm"
                                                    CommandName="Approve"
                                                    CommandArgument='<%# Eval("PRID") %>' />

                                                <asp:LinkButton ID="btnReject" runat="server"
                                                    Text="Reject"
                                                    CssClass="btn btn-danger btn-sm"
                                                    CommandName="Reject"
                                                    CommandArgument='<%# Eval("PRID") %>'
                                                    OnClientClick="return confirm('Are you sure to reject this PR?');" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:GridView ID="gvCompleted" runat="server" DataKeyNames="PRID" Visible="false" AutoGenerateColumns="False" Font-Bold="True"
                                    AllowSorting="true"
                                    class="table table-bordered table-striped table-hover js-basic-example dataTable">
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
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="Dpt. Name" />
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                    </Columns>
                                </asp:GridView>
                                <asp:HiddenField ID="hdCurrentTier" runat="server" />
                                <asp:HiddenField ID="hdCurrentApprover" runat="server" />
                                <asp:HiddenField ID="hdStatus" runat="server" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

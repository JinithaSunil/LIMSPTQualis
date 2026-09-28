<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PRList.aspx.cs" Inherits="LIMS.PRList" %>

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
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="assets\img\search.png" OnClick="imgSearch_Click" />
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
                                <asp:GridView ID="gvRequests" runat="server" DataKeyNames="PRID" Visible="false" AutoGenerateColumns="False" Font-Bold="True"
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
                                                    NavigateUrl='<%# Eval("PRID", "PRApproval.aspx?p={0}") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Emp. Name" />
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="Dpt. Name" />
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                    </Columns>
                                </asp:GridView>
                                <asp:GridView ID="gvCompleted" runat="server" DataKeyNames="PRID" Visible="false" 
                                    AutoGenerateColumns="False" Font-Bold="True"
                                    AllowSorting="true" OnSorting="gvCompleted_Sorting"
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
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" SortExpression="RequestDate" DataFormatString="{0:dd-MMM-yyyy}" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

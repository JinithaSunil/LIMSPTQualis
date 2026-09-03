<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="PRListPending.aspx.cs" Inherits="LIMS.PRListPending" %>

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
                            <h2>PENDING PURCHASE REQUISITION LIST</h2>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>

                            <!-- Project and PR Type Selection -->
                            <div class="row clearfix">
                                <div class="col-md-3">Division</div>
                                <div class="col-md-3">From Date</div>
                                <div class="col-md-3">To Date</div>
                                <div class="col-md-3">PP#</div>
                            </div>
                            <div class="row clearfix">
                                <div class="col-md-3">
                                    <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" OnTextChanged="txtDate_TextChanged" AutoPostBack="true" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" OnTextChanged="txtToDate_TextChanged" AutoPostBack="true" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox ID="txtPPNo" runat="server" CssClass="form-control" OnTextChanged="txtPPNo_TextChanged" AutoPostBack="true" placeholder="PP#"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-4">
                                    <asp:RadioButtonList ID="radChoice" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="radChoice_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="4" Selected="True">Tier-4</asp:ListItem>
                                        <asp:ListItem Value="3">Tier-3</asp:ListItem>
                                        <asp:ListItem Value="2">Tier-2</asp:ListItem>
                                        <asp:ListItem Value="1">Tier-1</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                Records:
                                <asp:Label ID="lblRecords" runat="server" Text="0"></asp:Label>
                            </div>
                            <div class="row">
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
                                                    NavigateUrl='<%# Eval("PRID", "GeneratePurchaseRequisitionReport.aspx?ID={0}") %>'
                                                    Target="_blank" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="EmployeeName" HeaderText="Emp. Name" />
                                        <asp:BoundField DataField="EmployeeDepartmentName" HeaderText="Dept. Name" />
                                        <asp:BoundField DataField="RequestDate" HeaderText="Request Date" DataFormatString="{0:dd-MMM-yyyy}" />
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
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ChartOfAccounts.aspx.cs" Inherits="LIMS.ChartOfAccounts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>CHART OF ACCOUNTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <table style="width: 100%" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                    <tr>
                                        <td colspan="2">
                                            <div class="col-sm-2">
                                                <asp:Button ID="btnAdd" runat="server" Text="Add Account" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="vertical-align: top; width: 50%">
                                            <asp:Repeater ID="rptMain" runat="server" OnItemDataBound="rptMain_ItemDataBound">
                                                <HeaderTemplate>
                                                    <h4>Balance Sheet</h4>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="margin-left: 10px;">
                                                        <b><%# Eval("AccountCategory") %></b>
                                                        <asp:HiddenField ID="hdBS" runat="server" Value=' <%# Eval("AccountCategoryID") %>' />
                                                        <asp:Repeater ID="rptChild" runat="server">
                                                            <ItemTemplate>
                                                                <div style="margin-left: 30px;">
                                                                    <%# Eval("AccountName") %>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                        <td style="vertical-align: top; width: 50%">
                                            <asp:Repeater ID="rptPL" runat="server" OnItemDataBound="rptPL_ItemDataBound">
                                                <HeaderTemplate>
                                                    <h4>Profit & Loss</h4>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="margin-left: 10px;">
                                                        <b><%# Eval("AccountCategory") %></b>
                                                        <asp:HiddenField ID="hdPL" runat="server" Value=' <%# Eval("AccountCategoryID") %>' />
                                                        <asp:Repeater ID="rptChild" runat="server">
                                                            <ItemTemplate>
                                                                <div style="margin-left: 30px;">
                                                                    <%# Eval("AccountName") %>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </table>




                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

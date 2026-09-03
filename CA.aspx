<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CA.aspx.cs" Inherits="LIMS.CA" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:Repeater ID="rptAccounts" runat="server">
    <ItemTemplate>

        <!-- A single row -->
        <div>

            <strong><%# Eval("AccountTitle") %></strong>
            &nbsp;&nbsp; <%# Eval("AccountNumber") %>
            &nbsp;&nbsp; <%# Eval("BalanceType") %>

        </div>

        <!-- Nested repeater for children -->
        <asp:Repeater ID="rptChildren" runat="server">
            <ItemTemplate>
                <%-- This template is identical, and recursion continues --%>

                <div>

                    <%# Eval("AccountTitle") %>
                    &nbsp;&nbsp; <%# Eval("AccountNumber") %>
                    &nbsp;&nbsp; <%# Eval("BalanceType") %>

                </div>

                <asp:Repeater ID="rptChildren" runat="server"></asp:Repeater>

            </ItemTemplate>
        </asp:Repeater>

    </ItemTemplate>
</asp:Repeater>

</asp:Content>

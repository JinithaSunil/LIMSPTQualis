<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="AccessRights.aspx.cs" Inherits="LIMS.AccessRights" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:DropDownList ID="ddlRoles" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRoles_SelectedIndexChanged"></asp:DropDownList>
</asp:Content>

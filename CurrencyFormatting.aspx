<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CurrencyFormatting.aspx.cs" Inherits="LIMS.CurrencyFormatting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:TextBox ID="txtAmt" runat="server"></asp:TextBox>
    <asp:Button ID="btnConvert" runat="server" OnClick="btnConvert_Click" Text="Convert" CssClass="btn btn-primary"/><br />
    IDR:
    <asp:Label ID="lblIDR" runat="server"></asp:Label><br />
    USD:
    <asp:Label ID="lblUSD" runat="server"></asp:Label><br />
</asp:Content>

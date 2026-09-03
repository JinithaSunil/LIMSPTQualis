<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ShowMessage.aspx.cs" Inherits="LIMS.ShowMessage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div>
        From:
        <asp:Label ID="lblFrom" runat="server" Text="" />
        <br />
        Subject:
        <asp:Label ID="lblSubject" runat="server" Text="" />
        <br />
        Body:
        <asp:Label ID="lblBody" runat="server" Text="" />
    </div>
</asp:Content>

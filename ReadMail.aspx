<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ReadMail.aspx.cs" Inherits="LIMS.ReadMail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    Mail Server Name:
    <asp:TextBox ID="txtMailServer" runat="server" />
    <br />
    UserName:
    <asp:TextBox ID="txtUserName" runat="server" />
    <br />
    Password:
    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
    <br />
    Port:
    <asp:TextBox ID="txtPort" runat="server" Text="110" />
    <br />
    SSL:
    <asp:CheckBox ID="chkSSL" runat="server" />
    <br />
    <asp:Button ID="btnReadEmails" runat="server" Text="Read Emails" OnClick="Read_Emails" />
    <br />
    <hr />
    <asp:GridView ID="gvEmails" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField HeaderText="From" DataField="From" />
            <asp:HyperLinkField HeaderText="Subject" DataNavigateUrlFields="MessageNumber" DataNavigateUrlFormatString="~/ShowMessage.aspx?MessageNumber={0}" DataTextField="Subject" />
            <asp:BoundField HeaderText="Date" DataField="DateSent" />
        </Columns>
    </asp:GridView>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="LIMS.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div style="text-align:center">
        <h1>An Error Has Occured</h1><br /><br />
            <div class="alert alert-danger">
                <asp:Label ID="lblErrorMessage" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblDefaultErrorMessage" runat="server" Text="The error might be due to slow or interrupted connection. Please try again after sometime." Visible="false"></asp:Label>
                
            </div>
    </div>
</asp:Content>

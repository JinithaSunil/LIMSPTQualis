<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" Async="true" AutoEventWireup="true" CodeBehind="Sample.aspx.cs" Inherits="LIMS.Sample" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:GridView ID="gvSamples" runat="server" AutoGenerateColumns="false">
        <Columns>
            <asp:BoundField DataField="SampleID" HeaderText="ID" />
            <asp:BoundField DataField="SampleNo" HeaderText="SampleNo" />
            <asp:BoundField DataField="CustomerName" HeaderText="CustomerName" />
        </Columns>
    </asp:GridView>
</asp:Content>

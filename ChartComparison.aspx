<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ChartComparison.aspx.cs" Inherits="LIMS.ChartComparison" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <table>
        <tr>
            <td>
                <asp:GridView ID="gvChart" runat="server" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField HeaderText="Year" DataField="year" />
                        <asp:BoundField HeaderText="Q1" DataField="Quarter1" />
                        <asp:BoundField HeaderText="Q2" DataField="Quarter2" />
                        <asp:BoundField HeaderText="Q3" DataField="Quarter3" />
                        <asp:BoundField HeaderText="Q4" DataField="Quarter4" />
                    </Columns>
                </asp:GridView>
            </td>            
        </tr>
        <tr>
            <td>                
                <asp:Chart ID="Chart1" runat="server" BorderSkin-BorderWidth="0" Width="550px">
                    <Series>
                        <asp:Series Name="Series1" XValueMember="Year" YValueMembers="Quarter1" LegendText="Q1" 
                            IsValueShownAsLabel="false" ChartArea="ChartArea1" MarkerBorderColor="#DBDBDB">
                        </asp:Series>
                        <asp:Series Name="Series2" XValueMember="Year" YValueMembers="Quarter2" LegendText="Q2" 
                            IsValueShownAsLabel="false" ChartArea="ChartArea1" MarkerBorderColor="#DBDBDB">
                        </asp:Series>
                        <asp:Series Name="Series3" XValueMember="Year" YValueMembers="Quarter3" LegendText="Q3" 
                            IsValueShownAsLabel="false" ChartArea="ChartArea1" MarkerBorderColor="#DBDBDB">
                        </asp:Series>
                        <asp:Series Name="Series4" XValueMember="Year" YValueMembers="Quarter4" LegendText="Q4" 
                            IsValueShownAsLabel="false" ChartArea="ChartArea1" MarkerBorderColor="#DBDBDB">
                        </asp:Series>
                    </Series>
                    <Legends>
                        <asp:Legend Title="Quarter"></asp:Legend>
                    </Legends>
                    <Titles>
                        <asp:Title Docking="Bottom" Text="Sales Report Quarterly"></asp:Title>
                    </Titles>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                    </ChartAreas>
                </asp:Chart>
            </td>
        </tr>
    </table>
</asp:Content>

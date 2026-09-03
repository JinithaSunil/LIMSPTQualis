<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="COA1.aspx.cs" Inherits="LIMS.COA" %>

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
                                <asp:TreeView ID="tvCOA" runat="server" ShowExpandCollapse="true"
                                    NodeIndent="20"
                                    ExpandDepth="0"
                                    ImageSet="XPFileExplorer"
                                    ShowLines="true"
                                    Font-Names="Segoe UI"
                                    Font-Size="14px"
                                    Font-Bold="true">
                                </asp:TreeView>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="OrgTree.aspx.cs" Inherits="LIMS.OrgTree" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body{
            font-size:14px;
            font-weight:bold;
        }
        .uppercase {
    text-transform: uppercase;
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-6">
                            <h3>ORGANIZATION TREE</h3>
                        </div><br />
                        </div>
                     <div class="row">
                        <asp:TreeView ID="tvOrg" runat="server" CssClass="uppercase"
                            ExpandDepth="10"
                            ShowLines="true">
                        </asp:TreeView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="FixedAssets.aspx.cs" Inherits="LIMS.FixedAssets" %>

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
                            <h2>FIXED ASSETS</h2>
                        </div>
                    </div>
                    <br />
                                        </div>
<div class="body">
                    <div class="row">
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtAssetCode" runat="server" CssClass="form-control" Placeholder="Asset Code"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtAssetName" runat="server" CssClass="form-control" Placeholder="Asset Name"></asp:TextBox>
                        </div>
                    </div><br />
                    <div class="row">
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtPurchaseDate" runat="server" CssClass="form-control" Placeholder="Purchase Date [yyyy-mm-dd]" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtPurchaseCost" runat="server" CssClass="form-control" Placeholder="Cost"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtUsefulLife" runat="server" CssClass="form-control" Placeholder="Useful Life (Months)"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:Button ID="btnAddAsset" runat="server" CssClass="btn btn-primary" Text="Add Asset" OnClick="btnAddAsset_Click" />
                        </div>
                    </div><br />
                    <asp:GridView ID="gvAssets" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                        <Columns>
                            <asp:BoundField DataField="AssetCode" HeaderText="AssetCode" />
                            <asp:BoundField DataField="AssetName" HeaderText="AssetName" />
                            <asp:BoundField DataField="PurchaseDate" HeaderText="PurchaseDate" DataFormatString="{0:dd.MM.yyyy}" />
                            <asp:BoundField DataField="PurchaseCost" HeaderText="PurchaseCost" />
                            <asp:BoundField DataField="Usefullifemonths" HeaderText="Usefullifemonths" />
                        </Columns>
                        </asp:GridView>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

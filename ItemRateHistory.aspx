<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ItemRateHistory.aspx.cs" Inherits="LIMS.ItemRateHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <h2>ITEM RATE HISTORY</h2>
                </div>
                <div class="body">
                    <div class="row clearfix">
                        <div class="col-12">
                            <asp:Label ID="lblRecords" runat="server"></asp:Label>
                            <div class="table-responsive">
                                <asp:GridView ID="gvPO" runat="server" 
                                    CssClass="table table-bordered table-striped table-hover"
                                    AllowSorting="true" 
                                    OnSorting="gvPO_Sorting" 
                                    DataKeyNames="PurchaseOrderID" 
                                    AutoGenerateColumns="false" 
                                    ShowHeaderWhenEmpty="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ItemDescription" HeaderText="Item" ItemStyle-Width="15%" />
                                        <asp:BoundField DataField="Rate" HeaderText="Rate" ItemStyle-Width="5%" />
                                        <asp:TemplateField HeaderText="Quantity">
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO Date" SortExpression="PODate">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPODate" runat="server" Text='<%# Eval("PODate","{0:dd/MMM/yyyy}").ToString() %>'></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="PO#" SortExpression="PONumber">
                                            <ItemTemplate>
                                                <asp:Label ID="lblPurchaseOrderID" runat="server" Text='<%# Eval("PurchaseOrderID").ToString() %>' Visible="false"></asp:Label>
                                                <asp:HyperLink ID="hypPO" runat="server" 
                                                    Text='<%# Eval("PONumber").ToString() %>' 
                                                    NavigateUrl='<%# Eval("PurchaseOrderID","ViewPO.aspx?PurchaseOrderID={0}") %>' 
                                                    Target="_blank">
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                            <ItemStyle Width="13%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Vendor Name" SortExpression="VendorName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblVendorName" runat="server" Text='<%# Eval("VendorName").ToString() %>'></asp:Label>
                                                <asp:Label ID="lblVendorID" runat="server" Text='<%# Eval("VendorID").ToString() %>' Visible="false"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="15%"></ItemStyle>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="text-center">No records found.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

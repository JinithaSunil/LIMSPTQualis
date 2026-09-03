<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CustomerList.aspx.cs" Inherits="LIMS.CustomerList" %>

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
                        <div class="col-sm-6">
                            <h2>COMPANIES LIST</h2>
                        </div>
                    </div>
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtSearch" runat="server" class="form-control" placeholder="Search Customer" OnTextChanged="txtSearch_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                        <div class="col-sm-1">
                            <asp:ImageButton ID="imgSearch" runat="server" ImageUrl="images/search.png" OnClick="imgSearch_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="table-responsive">
                            <asp:Panel ID="pnlView" runat="server">
                                <asp:GridView ID="gvCustomers" runat="server" AutoGenerateColumns="False" Width="90%" Font-Bold="True"
                                    DataKeyNames="CustomerMasterID,CustomerID" class="table table-bordered table-striped table-hover js-basic-example dataTable"
                                    OnPageIndexChanging="gvCustomers_PageIndexChanging" AllowPaging="true" PageSize="5"
                                    OnRowDataBound="gvCustomers_RowDataBound" AllowSorting="true" OnSorting="gvCustomers_Sorting">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sl#">
                                            <ItemTemplate>
                                                <asp:Label ID="lblRowNumber" Text='<%# Container.DataItemIndex + 1 %>' runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle Width="5%"></ItemStyle>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Company Name" SortExpression="CompanyName">
                                            <ItemTemplate>
                                                <asp:HyperLink ID="hypCustomer" runat="server" Text='<%# Eval("CompanyName").ToString().ToUpper() %>' Target="_blank" NavigateUrl='<%# Eval("CustomerMasterID", "CustomerProfile.aspx?ID={0}") %>'></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Address" HeaderText="Address" />

                                    </Columns>
                                    <FooterStyle BackColor="White" ForeColor="#333333" />
                                    <PagerStyle BackColor="#b34788" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="White" ForeColor="#333333" Height="50px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    <SelectedRowStyle BackColor="#339966" Font-Bold="True" ForeColor="White" />
                                    <SortedAscendingCellStyle BackColor="#F7F7F7" />
                                    <SortedAscendingHeaderStyle BackColor="#487575" />
                                    <SortedDescendingCellStyle BackColor="#E5E5E5" />
                                    <SortedDescendingHeaderStyle BackColor="#275353" />
                                </asp:GridView>
                                <asp:Label ID="lblNoRecords" runat="server" Text="No Data!" Font-Bold="True" Font-Size="X-Large" ForeColor="Red" Visible="False"></asp:Label>
                                <br />
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- #END# Basic Examples -->



</asp:Content>

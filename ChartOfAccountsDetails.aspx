<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ChartOfAccountsDetails.aspx.cs" Inherits="LIMS.ChartOfAccountsDetails" %>

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
                            <h2>CHART OF ACCOUNTS - DETAILS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <fieldset>
                                <div class="row">
                                    <div class="col-sm-2">
                                        <asp:DropDownList ID="ddlAccountCategory" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlAccountCategory_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                    <div class="col-sm-4">
                                        <asp:TextBox ID="txtName" runat="server" CssClass="form-control" Placeholder="Account Name"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtAccountCode" runat="server" CssClass="form-control" Placeholder="Code"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-2">
                                        <asp:TextBox ID="txtBalance" runat="server" Width="80%" CssClass="form-control" Placeholder="Balance"></asp:TextBox>
                                    </div>

                                    <div class="col-sm-2">
                                        <asp:Button ID="btnAdd" runat="server" Text="Add Account" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:GridView ID="gvCOA" runat="server" ShowFooter="true" OnRowDataBound="gvCOA_RowDataBound" class="table table-bordered table-striped table-hover js-basic-example dataTable" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:BoundField DataField="AccountCategory" HeaderText="AccountCategory" />
                                                <asp:BoundField DataField="AccountName" HeaderText="AccountName" />
                                                <asp:BoundField DataField="AccountCode" HeaderText="AccountCode" />
                                                <asp:TemplateField HeaderText="Balance">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBalance" runat="server" Text='<%#Eval("Balance") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lbltotal" runat="server" Text="Label" Font-Bold="true"></asp:Label>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                    </div>
                                </div>
                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

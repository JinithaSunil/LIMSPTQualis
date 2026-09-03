<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="Banking.aspx.cs" Inherits="LIMS.Banking" %>

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
                            <h2>BANKING</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" Placeholder="Account Name"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" Placeholder="Account Number"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:TextBox ID="txtBalance" runat="server" CssClass="form-control" Placeholder="Balance"></asp:TextBox>
                        </div>
                        <div class="col-sm-3">
                            <asp:Button ID="btnAddBank" runat="server" Text="Add Account" CssClass="btn btn-primary" OnClick="btnAddBank_Click" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-4">
                            <asp:DropDownList ID="ddlBankAccounts" CssClass="form-control" runat="server"></asp:DropDownList>
                        </div>
                        <div class="col-sm-4">
                            <asp:TextBox ID="txtDepositAmount" CssClass="form-control" runat="server" Placeholder="Deposit (+) or Withdraw (-)"></asp:TextBox>
                        </div>
                        <div class="col-sm-4">
                            <asp:Button ID="btnUpdateBalance" CssClass="btn btn-primary" runat="server" Text="Update Balance" OnClick="btnUpdateBalance_Click" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-12">
                            <asp:GridView ID="gvBankAccounts" runat="server" OnRowCommand="gvBankAccounts_RowCommand" AutoGenerateColumns="false" DataKeyNames="BankAccountId" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                <Columns>
                                    <asp:BoundField DataField="BankName" HeaderText="Bank Name" />
                                    <asp:BoundField DataField="AccountNumber" HeaderText="Account#" />
                                    <asp:TemplateField HeaderText="Balance" ItemStyle-Width="20%" ItemStyle-Font-Bold="true">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkBankStatement" runat="server"
                                                Text='<%#Eval("Balance") %>' CommandName="BankStatement" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="30px"></ItemStyle>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <br />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

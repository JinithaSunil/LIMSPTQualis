<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="BankStatement.aspx.cs" Inherits="LIMS.BankStatement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <!-- Basic Examples -->
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <h2>BANK STATEMENT</h2>
                                        <br />
                                        <asp:Label ID="lblBankName" runat="server" Font-Bold="true"></asp:Label>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="body">
                                <div class="row">
                                    <asp:GridView ID="gvBankStatement" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                        <Columns>
                                            <asp:BoundField DataField="JournalDate" HeaderText="Date" DataFormatString="{0:dd.MM.yyyy}" />
                                            <asp:BoundField DataField="Txn" HeaderText="Transaction" />
                                            <asp:BoundField DataField="ReferenceNo" HeaderText="Account" />
                                            <asp:BoundField DataField="Description" HeaderText="Description" />
                                             <asp:BoundField DataField="OpeningBalance" HeaderText="OpeningBalance" />
                                            <asp:BoundField DataField="Debit" HeaderText="Debit" />
                                            <asp:BoundField DataField="Credit" HeaderText="Credit" />
                                            <asp:BoundField DataField="ClosingBalance" HeaderText="ClosingBalance" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

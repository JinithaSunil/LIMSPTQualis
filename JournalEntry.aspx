<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="JournalEntry.aspx.cs" Inherits="LIMS.JournalEntry" %>

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
                                        <h2>JOURNAL ENTRY</h2>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="body">
                                <div class="row">
                                    <div class="col-sm-3">
                                        <b>Payment Date</b>
                                        <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-sm-9">
                                        <b>Reference</b>
                                        <asp:TextBox ID="txtReference" runat="server" CssClass="form-control" Width="400px" Placeholder="Reference"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-3">
                                        <b>Currency</b>
                                        <asp:DropDownList ID="ddlCurrency" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="INR">IDR - Indian Rupiah</asp:ListItem>
                                            <asp:ListItem Value="USD">USD - Dollar</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-9">
                                        <b>Narration</b>
                                        <asp:TextBox ID="txtNarration" runat="server" CssClass="form-control" Width="400px"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <asp:GridView ID="gvLines" runat="server" OnRowDataBound="gvLines_RowDataBound" AutoGenerateColumns="false" ShowFooter="true" CssClass="table table-responsive">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Account">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlAccount" runat="server" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Debit">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtDebit" runat="server" Text="0" Width="100"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Credit">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtCredit" runat="server" Text="0" Width="100"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnRemove" runat="server" Text="X" CssClass="btn btn-primary" CommandName="Remove" CommandArgument='<%# Container.DataItemIndex %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <asp:Button ID="btnAddLine" runat="server" Text="+ Add Line" OnClick="btnAddLine_Click" CssClass="btn btn-primary" />
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-12" style="text-align: center">
                                        <asp:Button ID="btnSave" runat="server" Text="Save Journal" OnClick="btnSave_Click" CssClass="btn btn-primary" />
                                    </div>
                                    <br />
                                    <br />
                                    <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>
                                </div>
                            </div>
                            <br />

                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="COAAdd.aspx.cs" Inherits="LIMS.ManageCOA" %>

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
                            <h2>ADD - CHART OF ACCOUNTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-sm-12" style="vertical-align: top">
                            <!-- SALES LINKED ACCOUNTS -->
                            <div class="card">
                                <div class="header bg-violet">
                                    <h2 style="color: white !important">Add Header/Details</h2>
                                </div>
                                <div class="body">
                                    <asp:RadioButtonList ID="rbAccountType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                                        OnSelectedIndexChanged="rbAccountType_SelectedIndexChanged">
                                        <asp:ListItem Text="Header Account" Value="H" Selected="True" />
                                        <asp:ListItem Text="Detail Account" Value="D" />
                                    </asp:RadioButtonList>

                                    <hr />
                                    <div class="row">
                                        <div class="col-sm-2" style="vertical-align: top">
                                            <asp:Panel ID="pnlHeader" runat="server">
                                                Header Level<br />
                                                <asp:DropDownList ID="ddlHeaderLevel" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="1" />
                                                    <asp:ListItem Text="2" />
                                                    <asp:ListItem Text="3" />
                                                </asp:DropDownList><br />
                                                <br />
                                            </asp:Panel>
                                        </div>
                                        <div class="col-sm-4" style="vertical-align: top">
                                            Parent Account<br />
                                            <asp:DropDownList ID="ddlParent" runat="server" CssClass="form-control" /><br />
                                        </div>
                                        <div class="col-sm-2" style="vertical-align: top">
                                            <!-- Common Fields -->
                                            Account Name<br />
                                            <asp:TextBox ID="txtAccountName" runat="server" CssClass="form-control" /><br />
                                        </div>
                                        <div class="col-sm-2" style="vertical-align: top">
                                            Account Code<br />
                                            <asp:TextBox ID="txtAccountCode" runat="server" CssClass="form-control" /><br />
                                        </div>
                                        <div class="col-sm-2" style="vertical-align: top">
                                            Debit / Credit<br />
                                            <asp:DropDownList ID="ddlDrCr" runat="server" CssClass="form-control">
                                                <asp:ListItem Text="Debit" Value="Debit" />
                                                <asp:ListItem Text="Credit" Value="Credit" />
                                            </asp:DropDownList><br />
                                        </div>
                                    </div>


                                    <!-- HEADER ONLY -->


                                    <!-- DETAIL ONLY -->
                                    <asp:Panel ID="pnlDetail" runat="server" Visible="false">
                                        Tax Code<br />
                                        <asp:DropDownList ID="ddlTaxCode" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="N-T" Value="N-T" />
                                            <asp:ListItem Text="GST" Value="GST" />
                                            <asp:ListItem Text="VAT" Value="VAT" />
                                        </asp:DropDownList><br />
                                        <br />
                                    </asp:Panel>

                                    <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary"
                                        OnClick="btnSave_Click" />

                                    <br />
                                    <br />
                                    <asp:Label ID="lblMsg" runat="server" ForeColor="Green" />

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

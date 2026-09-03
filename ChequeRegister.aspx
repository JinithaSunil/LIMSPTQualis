<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="ChequeRegister.aspx.cs" Inherits="LIMS.ChequeRegister" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .cheque-dates {
            font-size: 12px;
            line-height: 1.6;
        }

            .cheque-dates div {
                white-space: nowrap;
            }
    </style>
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
                                        <h2>CHEQUE REGISTER</h2>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="body">
                                <div class="row">

                                    <!-- Filters -->
                                    <table class="filterTable" style="text-align: center; font-weight: bold">
                                        <tr>
                                            <td>From Date</td>
                                            <td>To Date</td>
                                            <td>Bank</td>
                                            <td>Status</td>
                                            <td>Cheque#</td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control" /></td>
                                            <td>
                                                <asp:TextBox ID="txtBank" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="All" Value="" />
                                                    <asp:ListItem Text="Received" />
                                                    <asp:ListItem Text="Issued" />
                                                    <asp:ListItem Text="Presented" />
                                                    <asp:ListItem Text="Cleared" />
                                                    <asp:ListItem Text="Bounced" />
                                                </asp:DropDownList>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtChequeNo" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnSearch" runat="server"
                                                    Text="Search" CssClass="btn btn-primary"
                                                    OnClick="btnSearch_Click" />
                                            </td>
                                        </tr>
                                    </table>

                                    <br />
                                    <asp:RadioButtonList ID="radChoice" runat="server" OnSelectedIndexChanged="radChoice_SelectedIndexChanged" AutoPostBack="true" RepeatDirection="Horizontal">
                                        <asp:ListItem Value="PAYMENT" Selected="True">Issued</asp:ListItem>
                                        <asp:ListItem Value="RECEIPT">Received</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <!-- Cheque Register Grid -->
                                    <asp:GridView ID="gvChequeRegister" runat="server"
                                        AutoGenerateColumns="false" DataKeyNames="ChequeID"
                                        CssClass="table table-bordered table-striped"
                                        OnRowDataBound="gvChequeRegister_RowDataBound"
                                        OnRowCommand="gvChequeRegister_RowCommand">

                                        <Columns>
                                            <asp:BoundField DataField="VoucherNo" HeaderText="Voucher No" />
                                            <asp:TemplateField HeaderText="Cheque Details">
                                                <ItemTemplate>
                                                    <div class="cheque-dates">
                                                        <div>
                                                            <strong>Cheque#</strong> :
                                                                <%# Eval("ChequeNo") %>
                                                        </div>

                                                        <div>
                                                            <strong>Cheque Dt.</strong> :
                                                             <%# Eval("ChequeDate", "{0:dd-MMM-yyyy}") %>
                                                        </div>
                                                        <div>
                                                            <strong>Bank</strong> :
                                                                 <%# Eval("BankName") %>
                                                        </div>
                                                        <div>
                                                            <strong>Presented</strong> :
                                                            <%# Eval("PresentedDate") == DBNull.Value
                                                                ? "-" 
                                                                : Eval("PresentedDate", "{0:dd-MMM-yyyy}") %>
                                                        </div>

                                                        <div>
                                                            <strong>Cleared</strong> :
                                                            <%# Eval("ClearedDate") == DBNull.Value
                                                                ? "-" 
                                                                : Eval("ClearedDate", "{0:dd-MMM-yyyy}") %>
                                                        </div>

                                                        <div>
                                                            <strong>Bounced</strong> :
                                                            <%# Eval("BounceDate") == DBNull.Value
                                                                ? "-" 
                                                                : Eval("BounceDate", "{0:dd-MMM-yyyy}") %>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="PartyName" HeaderText="Party" />
                                            <asp:BoundField DataField="Amount" HeaderText="Amount"
                                                DataFormatString="{0:N2}" ItemStyle-HorizontalAlign="Right" />

                                            <asp:TemplateField HeaderText="Status">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                                    <asp:HiddenField ID="hdChequeStatus" runat="server" Value='<%#Eval("ChequeStatus") %>' />
                                                    <asp:RadioButtonList ID="radChequeStatus" runat="server">
                                                        <asp:ListItem Text="Received" />
                                                        <asp:ListItem Text="Issued" />
                                                        <asp:ListItem Text="Presented" />
                                                        <asp:ListItem Text="Cleared" />
                                                        <asp:ListItem Text="Bounced" />
                                                    </asp:RadioButtonList>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:ButtonField Text="Update"
                                                CommandName="UpdateStatus"
                                                ButtonType="Button"
                                                ControlStyle-CssClass="btn btn-sm btn-info" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

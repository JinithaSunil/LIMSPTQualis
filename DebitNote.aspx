<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DebitNote.aspx.cs" Inherits="LIMS.DebitNote" %>

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
                            <h2>DEBIT NOTE</h2>
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
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>DN#</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtDebitNoteNo" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>DN Date</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtDebitNoteDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>Suppliers</b>
                                            <div class="form-line">
                                                <asp:DropDownList ID="ddlVendors" runat="server" CssClass="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <b>Remarks</b>
                                            <div class="form-line">
                                                <asp:TextBox ID="txtRemarks" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <br />
                                <div id="divAdd" runat="server" class="row">
                                    <table class="table table-bordered">
                                        <tr>
                                            <th>Item</th>
                                            <th>Qty</th>
                                            <th>Price</th>
                                            <th>Amount</th>
                                            <th></th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtItem" runat="server" CssClass="form-control"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtQty" runat="server" AutoPostBack="true" CssClass="form-control" OnTextChanged="QtyRate_TextChanged"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtRate" runat="server" AutoPostBack="true" CssClass="form-control" OnTextChanged="QtyRate_TextChanged"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblAmount" runat="server" Text="0.00"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>

                                    <br />
                                    <br />

                                    Total Amount:
                                    <asp:Label ID="lblTotal" runat="server" CssClass="font-weight-bold" />
                                    </div>
                                    <br />
                                    <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                                        <Columns>
                                            <asp:BoundField DataField="ItemName" HeaderText="ItemName" />
                                            <asp:BoundField DataField="Qty" HeaderText="Qty" />
                                            <asp:BoundField DataField="Rate" HeaderText="Rate" />
                                            <asp:BoundField DataField="Amount" HeaderText="Amount" />
                                        </Columns>
                                    </asp:GridView>
                                    <br />
                                    <asp:Button ID="btnSave" runat="server" Text="SAVE" CssClass="btn btn-success" OnClick="btnSave_Click" />
                              

                            </fieldset>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

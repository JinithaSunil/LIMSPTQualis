<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="CustomerLedger.aspx.cs" Inherits="LIMS.CustomerLedger" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <fieldset>
                <div class="row clearfix">
                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                        <div class="card">
                            <hr class="violet">
                            <div class="header">
                                <div class="row">
                                    <div class="col-sm-12">
                                        <h2>CUSTOMER LEDGER</h2>
                                    </div>
                                </div>
                                <br />
                            </div>
                            <div class="body">
                                <div class="row">
                                    <div class="col-md-4">
                                        <label>Customer</label>
                                        <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>

                                    <div class="col-md-3">
                                        <label>From Date</label>
                                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>

                                    <div class="col-md-3">
                                        <label>To Date</label>
                                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>

                                    <div class="col-md-2">
                                        <br />
                                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success w-100" OnClick="btnSearch_Click" />
                                    </div>
                                </div>
                                <asp:GridView ID="gvLedger" runat="server" CssClass="table table-bordered table-striped"
                                    AutoGenerateColumns="false">

                                    <Columns>

                                        <asp:BoundField DataField="TransactionDate" HeaderText="Date" DataFormatString="{0:dd-MM-yyyy}" />

                                        <asp:BoundField DataField="ReferenceType" HeaderText="Type" />

                                        <asp:BoundField DataField="ReferenceID" HeaderText="Ref No" />

                                        <asp:BoundField DataField="Description" HeaderText="Description" />

                                        <asp:BoundField DataField="Debit" HeaderText="Debit" />

                                        <asp:BoundField DataField="Credit" HeaderText="Credit" />

                                        <asp:BoundField DataField="Balance" HeaderText="Balance" />

                                    </Columns>

                                </asp:GridView>
                            </div>



                        </div>
                    </div>
                </div>
            </fieldset>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="DebitNotes.aspx.cs" Inherits="LIMS.DebitNotes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function toggleChild(id) {
            var elem = document.getElementById("child_" + id);
            if (elem.style.display === "none") {
                elem.style.display = "block";
            } else {
                elem.style.display = "none";
            }
        }
    </script>
    <style>
        body {
            font-size: 10px;
        }
    </style>
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
                            <h2>DEBIT NOTES</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="col-4">
                            <asp:Button ID="btnNewDebitNote" runat="server" Text="Add" CssClass="btn btn-primary" OnClick="btnNewDebitNote_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:GridView ID="gvDebitNote" runat="server" AutoGenerateColumns="false" OnRowCommand="gvDebitNote_RowCommand"
                            DataKeyNames="DebitNoteID" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                            <Columns>
                                <asp:TemplateField HeaderText="Debit Note No.">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDN" runat="server" Text='<%#Eval("DebitNoteNo") %>' CommandName="GetDebitNote" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>'></asp:LinkButton>
                                        <asp:HiddenField ID="hdDebitNoteID" runat="server" Value='<%#Eval("DebitNoteID") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="DebitNoteDate" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:BoundField DataField="VendorName" HeaderText="Supplier" />
                                <asp:BoundField DataField="TotalAmount" HeaderText="Credit" />
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkShowInvoice" runat="server" CommandName="View" CommandArgument='<%# Eval("DebitNoteID") %>'
                                            ToolTip="View Debit Note">
                                                <i class="fa fa-eye" style="font-size:medium; margin-right: 12px;"></i> 
                                        </asp:LinkButton>

                                    </ItemTemplate>
                                    <ItemStyle Width="6%"></ItemStyle>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

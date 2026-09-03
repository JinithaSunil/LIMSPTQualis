<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="JournalEntries.aspx.cs" Inherits="LIMS.JournalEntries" %>

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

        .month-bar {
            margin-bottom: 15px;
        }

        .month-btn {
            margin: 3px;
            padding: 6px 12px;
            border: 1px solid #ccc;
            background: #f8f8f8;
            cursor: pointer;
        }

            .month-btn.active {
                background: #b34788;
                color: white;
                border-color: #b34788;
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
                            <h2>JOURNAL ENTRIES</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row">
                        <div class="month-bar">
                            <div class="col-sm-3">
                                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlYear_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                            </div>

                            <asp:HiddenField ID="hfMonth" runat="server" Value="0" />

                            <asp:Repeater ID="rptMonths" runat="server">
                                <ItemTemplate>
                                    <asp:Button
                                        ID="btnMonth"
                                        runat="server"
                                        Text='<%# Eval("Text") %>'
                                        CommandArgument='<%# Eval("Value") %>'
                                        CssClass="month-btn"
                                        OnCommand="Month_Command" />
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-sm-2">
                            <asp:Button ID="btnNewJournalEntry" runat="server" Text="Add" CssClass="btn btn-primary" OnClick="btnNewJournalEntry_Click" />
                        &nbsp;&nbsp;
                            <asp:Button ID="btnDownload" runat="server" Text="Download" CssClass="btn btn-primary" OnClick="btnDownload_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:GridView ID="gvJournalEntries" runat="server" AutoGenerateColumns="false" class="table table-bordered table-striped table-hover js-basic-example dataTable">
                            <Columns>
                                <asp:BoundField DataField="CreatedOn" HeaderText="Date" DataFormatString="{0:dd-MMM-yyyy}" />
                                <asp:BoundField DataField="ReferenceNo" HeaderText="Reference" />
                                <asp:BoundField DataField="Debit" HeaderText="Debit" />
                                <asp:BoundField DataField="Credit" HeaderText="Credit" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:BoundField DataField="LineDescription" HeaderText="Details" />
                            </Columns>
                        </asp:GridView>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

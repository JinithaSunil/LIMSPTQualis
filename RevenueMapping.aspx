<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMaster.Master" AutoEventWireup="true" CodeBehind="RevenueMapping.aspx.cs" Inherits="LIMS.RevenueMapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Segoe UI, Arial;
            background: #f5f6fa;
        }


        h2 {
            margin-bottom: 15px;
            color: #2c3e50;
        }

        label {
            font-weight: bold;
        }

        select {
            padding: 6px;
            width: 250px;
            margin: 10px 0;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }

        th {
            background: #b34788;
            color: #fff;
            padding: 8px;
        }

        td {
            padding: 8px;
            border-bottom: 1px solid #ddd;
        }

        tr:hover {
            background: #f1f1f1;
            cursor: pointer;
        }

        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

    <!-- Basic Examples -->
    <div class="row clearfix">
        <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
            <div class="card">
                <hr class="violet">
                <div class="header">
                    <div class="row">
                        <div class="col-sm-12">
                            <h2>REVENUE ACCOUNTS</h2>
                        </div>
                    </div>
                    <br />
                </div>
                <div class="body">
                    <div class="row clearfix">
                      <div class="col-sm-3">
                            <label>Division</label><br>
                        </div>
                    </div>
                    <div class="row">
                        <asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddldivision_SelectedIndexChanged"></asp:DropDownList>
                        <asp:GridView ID="gvRevenue" runat="server" CssClass="table" AutoGenerateColumns="false">
                            <Columns>
                                <asp:BoundField DataField="DepartmentName" HeaderText="Department" HeaderStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="Parameter" HeaderText="Parameter" HeaderStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="AccountName" HeaderText="Account" HeaderStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="AccountCode" HeaderText="AccountCode" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Dimensionofzinccoatingtest.aspx.cs" Inherits="LIMS.Dimensionofzinccoatingtest" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .card {
            background: white;
            border-radius: 3px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
        }

        .violet {
            border: 0;
            height: 4px;
            background-color: #b34788;
            margin: 0;
        }

        .header {
            padding: 15px 20px;
            border-bottom: 1px solid #e0e0e0;
        }

            .header h2 {
                margin: 0;
                color: #333;
                font-size: 18px;
                font-weight: bold;
            }

        .body {
            padding: 20px;
        }

        .table-responsive {
            overflow-x: auto;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            background-color: white;
        }

        table, th, td {
            border: 1px solid #ddd;
        }

        th {
            background-color: #f2f2f2;
            color: #333;
            padding: 10px;
            text-align: center;
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #f2f2f2;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 90%;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
        }

        .btn-primary {
            background-color: #b34788;
            color: white;
            border: none;
            padding: 10px 20px;
            border-radius: 3px;
            cursor: pointer;
            font-weight: bold;
        }

            .btn-primary:hover {
                background-color: #933375;
            }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }

        /* Make header cells and input aligned */
        .tests-section th, .test-row td {
            vertical-align: middle;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>DIMENSION OF ZINC COATING TEST</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:Label ID="cableType" runat="server" CssClass="form-control" /></td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:Label ID="refSpec" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:Label ID="cableSize" runat="server" CssClass="form-control" /></td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:Label ID="testMethodNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:Label ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" /></td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" /></td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>

                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">Item Type</th>
                                            <th rowspan="2">Unit</th>
                                            <th rowspan="2">1</th>
                                            <th rowspan="2">2</th>
                                            <th rowspan="2">3</th>

                                            <th rowspan="2" style="width: 120px">Avg
                                            </th>
                                            <th>Specification</th>

                                            <th rowspan="2">Add Data</th>
                                        </tr>

                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlItemType" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                    <asp:ListItem Text="Select" Value="Select" />
                                                    <asp:ListItem Text="Diameter of steel wire" Value="DoubleArmouring" />
                                                    <asp:ListItem Text="Weight / Thickness before immersed"
                                                        Value="Weight / Thickness before immersed" />
                                                    <asp:ListItem Text="Weight / Thickness after immersed"
                                                        Value="Weight / Thickness after immersed" />
                                                     <asp:ListItem Text="Weight / Thickness of zinc coating" Value="Weight / Thickness of zinc coating" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                             <asp:ListItem Text="Select Unit" Value="Select Unit" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="gr/m2" Value="gr/m2" />
                                                </asp:DropDownList>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtInput1" runat="server" AutoPostBack="true" OnTextChanged="txtInput_TextChanged" CssClass="form-control" Style="text-align: center;" />
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtInput2" runat="server" AutoPostBack="true" OnTextChanged="txtInput_TextChanged" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput3" runat="server" AutoPostBack="true" OnTextChanged="txtInput_TextChanged" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <!-- This TextBox will display Min, Max, and Avg values -->
                                                <asp:TextBox ID="txtAvg" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecifications" runat="server" AutoPostBack="true" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " onClick="btnAddToGrid_Click"
                                                    CssClass="btn btn-primary" />

                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>DIMENSION OF ZINC COATING TEST</h4>
                                    <div class="table-responsive">
                               <asp:GridView ID="gvDimension" runat="server" AutoGenerateColumns="false" 
        OnRowDataBound="gvDimension_RowDataBound" OnRowCommand="gvDimension_RowCommand"
        CssClass="table table-bordered table-striped">
        <Columns>
            <asp:BoundField DataField="ItemType" HeaderText="Item Type" />
            <asp:BoundField DataField="Unit" HeaderText="Unit" />
            <asp:BoundField DataField="Input1" HeaderText="1" />
            <asp:BoundField DataField="Input2" HeaderText="2" />
            <asp:BoundField DataField="Input3" HeaderText="3" />
            <asp:BoundField DataField="Average" HeaderText="Avg" />
            <asp:BoundField DataField="Specification" HeaderText="Specification" />
            <asp:TemplateField HeaderText="Actions">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete"
                        CommandName="DeleteRow" CommandArgument='<%# Container.DataItemIndex %>'
                        OnClientClick="return confirm('Are you sure you want to delete this record?');"
                        CssClass="btn btn-danger btn-sm" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear"  OnClick="btnClearData_Click"  />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

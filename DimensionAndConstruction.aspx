<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DimensionAndConstruction.aspx.cs" Inherits="LIMS.DimensionAndConstruction" %>


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
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>DIMENSION AND CONSTRUCTION</h2>
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

                            <br />
                            <table style="width: 70%; border-collapse: collapse; font-family: Arial; margin-left: auto; margin-right: auto;">
                                <!-- 1. Conductor Condition -->
                                <tr>
                                    <td style="text-align: left;">1. Conductor Condition</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtconductorcondition" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- 2. Insulation -->
                                <tr>
                                    <td style="text-align: left;">2. Insulation </td>
                                    <td style="text-align: left;"></td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">- Condition</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtCondition" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">- Color Scheme</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtcolorscheme" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">- Color Combination</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtcolorcombination" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- 3. Filler Condition -->
                                <tr>
                                    <td style="text-align: left;">3. Filler Condition<</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtfillercondition" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- 4. Innersheath Condition -->
                                <tr>
                                    <td style="text-align: left;">4. Innersheath Condition</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtinnersheath" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- 5. Armouring Condition -->
                                <tr>
                                    <td style="text-align: left;">5. Armouring Condition</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtarmouring" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- 6. Sheath Condition -->
                                <tr>
                                    <td style="text-align: left;">6. Sheath Condition</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtsheath" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- 7. Marking -->
                                <tr>
                                    <td style="text-align: left;">7. Marking</td>
                                    <td style="text-align: left;"></td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">- Condition</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtmarkingcondition" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">- Identification</td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtidentification" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: left;">- Distance </td>
                                    <td style="text-align: left;">
                                        <asp:TextBox ID="txtdistance" runat="server" CssClass="form-control" Style="width: 180px; text-align: center;" />
                                    </td>
                                </tr>

                                <!-- Add Data Button -->
                                <tr>
                                    <td style="text-align: left;"><b>Add Data</b></td>
                                    <td style="text-align: left;">
                                        <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                    </td>
                                </tr>
                            </table>

                            <%-- grid --%>
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Dimension Test  Results</h4>
                                    <div class="table-responsive">
                                   
                                        <asp:GridView ID="gvDimensionConstruction" runat="server" AutoGenerateColumns="False"
                                            CssClass="table table-bordered table-striped">
                                            <Columns>
                                                <asp:BoundField DataField="ConductorCondition" HeaderText="Conductor Condition" />
                                                <asp:BoundField DataField="InsulationCondition" HeaderText="Insulation Condition" />
                                                <asp:BoundField DataField="ColorScheme" HeaderText="Color Scheme" />
                                                <asp:BoundField DataField="ColorCombination" HeaderText="Color Combination" />
                                                <asp:BoundField DataField="FillerCondition" HeaderText="Filler Condition" />
                                                <asp:BoundField DataField="InnerSheathCondition" HeaderText="Inner Sheath Condition" />
                                                <asp:BoundField DataField="ArmouringCondition" HeaderText="Armouring Condition" />
                                                <asp:BoundField DataField="SheathCondition" HeaderText="Sheath Condition" />
                                                <asp:BoundField DataField="MarkingCondition" HeaderText="Marking Condition" />
                                                <asp:BoundField DataField="Identification" HeaderText="Identification" />
                                                <asp:BoundField DataField="Distance" HeaderText="Distance" />
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

                            </table>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>


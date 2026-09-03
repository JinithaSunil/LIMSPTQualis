<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="CopperandAluminiumWire.aspx.cs" Inherits="LIMS.Copper_AluminiumWire" %>


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
            padding: 5px;
            text-align: center;
        }

        .info-table td:first-child {
            background-color: #068001;
            width: 25%;
            font-weight: bold;
        }

        .test-table th {
            background-color: #e0e0e0;
            vertical-align: middle;
        }

        input[type="text"], input[type="number"], input[type="date"] {
            width: 100%;
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

        .invalid-input {
            border: 2px solid #ff9999 !important;
            background-color: #fff0f0 !important;
        }

        .table-bordered th,
        .table-bordered td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }

        .table-bordered th {
            background-color: #f2f2f2;
        }
        /* For all inputs in tests-section */
        .table-responsive.tests-section input[type="text"],
        .table-responsive.tests-section input[type="number"],
        .table-responsive.tests-section input[type="date"] {
            width: 100px !important; /* uniform width */
            max-width: 100px !important;
            box-sizing: border-box;
        }

        /* Override only the first date input in the first column to match grid */
        .table-responsive.tests-section tr td:first-child input[type="date"] {
            width: 100px !important;
            max-width: 100px !important;
        }

        .gvcopper th,
        .gvcopper td {
            width: 100px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            padding: 6px 8px;
            box-sizing: border-box;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>Copper & Aluminium Wire / Rod
                                <br />
                                Mechanical & Electrical Test
                            </h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" readonly="true"/>
                                          
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" readonly="true"/>
                                           
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" readonly="true"/>
                                          
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" readonly="true"/>
                                          
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" readonly="true"/>
                                            
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" readonly="true"/>
                                         
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                                                ControlToValidate="Date"
                                                ErrorMessage="Date is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" readonly="true"/>
                                           
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" readonly="true"/>
                                          
                                        </td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomHumidity" runat="server"
                                                ControlToValidate="roomhumidity"
                                                ErrorMessage="Room Humidity is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomTemp" runat="server"
                                                ControlToValidate="roomtemp"
                                                ErrorMessage="Room Temperature is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" readonly="true"/>
                                         
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th rowspan="3">Date</th>
                                            <th rowspan="3">Coil<br />
                                                No.</th>
                                            <th rowspan="3">Specimen<br />
                                                No.</th>
                                            <th colspan="7">Diameter (mm)</th>
                                            <th rowspan="3">Area<br />
                                                mm²</th>
                                            <th rowspan="3">BF<br />
                                                N</th>
                                            <th rowspan="3">TS<br />
                                                N/mm²</th>
                                            <th colspan="3">Elongation</th>
                                            <th colspan="2">Median</th>
                                            <th rowspan="3">Resistance<br />
                                                Ω/km</th>
                                            <th rowspan="3">Temp.<br />
                                                °C</th>
                                            <th rowspan="3">Kt<br />
                                                %</th>
                                            <th rowspan="3">Resistivity<br />
                                                at 20°C<br />
                                                Ω·mm²/m</th>
                                            <th rowspan="3">Conductivity<br />
                                                %</th>
                                            <th rowspan="3">Density<br />
                                                g/cm³</th>
                                            <th rowspan="3">Add Data</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Nom</th>
                                            <th rowspan="2">L0<br />
                                                mm</th>
                                            <th rowspan="2">L1<br />
                                                mm</th>
                                            <th rowspan="2">EL<br />
                                                %</th>
                                            <th rowspan="2">TS<br />
                                                N/mm²</th>
                                            <th rowspan="2">EL<br />
                                                %</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td rowspan="5">
                                                <asp:TextBox ID="txtDate1" runat="server" CssClass="input-field date-input" TextMode="Date" />
                                            </td>
                                            <td rowspan="5">
                                                <asp:TextBox ID="txtCoil1" runat="server" CssClass="input-field" AutoPostBack="true" OnTextChanged="txtCoil1_TextChanged" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="input-field" AutoPostBack="true" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput1" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtInput2" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtInput3" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtInput4" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtInput5" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtInput6" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtNom1" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtArea" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtBF" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtTS" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtL0" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtL1" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtEL" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMTS" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtMEL" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtRes" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtTemp" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtKt" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtResist" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtCond" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtDens" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
                                        </tr>

                                    </tbody>
                                </table>
                            </div>


                            <br />

                            <br />
                            <br />
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Copper & Aluminium Wire / Rod
                                        <br />
                                        Mechanical & Electrical Test
                                    </h4>
                                    <div class="table-responsive tests-section">

                                        <asp:GridView ID="gvcopper" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-striped gvcopper" OnRowCreated="gvcopper_RowCreated" OnRowDataBound="gvcopper_RowDataBound" OnRowCommand="gvcopper_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="Date" HeaderText="Date" />
                                                <asp:BoundField DataField="CoilNo" HeaderText="Coil No." />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />
                                                <asp:BoundField DataField="Diameter1" HeaderText="1" />
                                                <asp:BoundField DataField="Diameter2" HeaderText="2" />
                                                <asp:BoundField DataField="Diameter3" HeaderText="3" />
                                                <asp:BoundField DataField="Diameter4" HeaderText="4" />
                                                <asp:BoundField DataField="Diameter5" HeaderText="5" />
                                                <asp:BoundField DataField="Diameter6" HeaderText="6" />
                                                <asp:BoundField DataField="Nom" HeaderText="Nom" />
                                                <asp:BoundField DataField="Area" HeaderText="Area mm²" />
                                                <asp:BoundField DataField="BF" HeaderText="BF N" />
                                                <asp:BoundField DataField="TS" HeaderText="TS N/mm²" />
                                                <asp:BoundField DataField="L0" HeaderText="L0 mm" />
                                                <asp:BoundField DataField="L1" HeaderText="L1 mm" />
                                                <asp:BoundField DataField="EL" HeaderText="EL %" />
                                                <asp:BoundField DataField="MTS" HeaderText="TS N/mm² (Median)" />
                                                <asp:BoundField DataField="MEL" HeaderText="EL % (Median)" />
                                                <asp:BoundField DataField="Resistance" HeaderText="Resistance Ω/km" />
                                                <asp:BoundField DataField="Temp" HeaderText="Temp. °C" />
                                                <asp:BoundField DataField="Kt" HeaderText="Kt %" />
                                                <asp:BoundField DataField="Resistivity" HeaderText="Resistivity Ω·mm²/m" />
                                                <asp:BoundField DataField="Conductivity" HeaderText="Conductivity %" />
                                                <asp:BoundField DataField="Density" HeaderText="Density g/cm³" />

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
                            <div>
                                <br />
                                <div class="table-responsive">
                                    <table class="table table-bordered text-center" style="width: 100%; border-collapse: collapse;">
                                        <thead>
                                            <tr>
                                                <th rowspan="2">Date</th>
                                                <th rowspan="2">Coil<br>
                                                    No.</th>
                                                <th rowspan="2">Specimen<br>
                                                    No.</th>


                                                <th colspan="7">Diameter</th>

                                                <th rowspan="2">Area<br>
                                                    mm²</th>
                                                <th rowspan="2">BF<br>
                                                    N</th>
                                                <th rowspan="2">TS<br>
                                                    N/mm²</th>


                                                <th colspan="3">Elongation</th>


                                                <th colspan="2">Median</th>

                                                <th rowspan="2">Resistance<br>
                                                    Ω/km</th>
                                                <th rowspan="2">Temp.<br>
                                                    C</th>
                                                <th rowspan="2">Kt<br>
                                                </th>
                                                <th rowspan="2">Resistivity
at 200C
                                                <br>
                                                    Ω.mm2/m</th>
                                                <th rowspan="2">Conductivity<br>
                                                    %</th>
                                                <th rowspan="2">Density<br>
                                                    g/cm3</th>
                                            </tr>
                                            <tr>

                                                <th>1</th>
                                                <th>2</th>
                                                <th>3</th>
                                                <th>4</th>
                                                <th>5</th>
                                                <th>6</th>
                                                <th>Nom</th>


                                                <th>L0<br>
                                                    mm</th>
                                                <th>L1<br>
                                                    mm</th>
                                                <th>EL.<br>
                                                    %</th>


                                                <th>TS<br>
                                                    N/mm²</th>
                                                <th>EL.<br>
                                                    %</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td colspan="3" style="font-weight: bold;">Specified</td>
                                                <td colspan="6">-
                                                    <asp:TextBox ID="txtSpecifiedDiameter" runat="server" CssClass="input-field" /></td>
                                                </td>
            <td>
                <asp:TextBox ID="txtSpecifiedDiameterNom" runat="server" CssClass="input-field" /></td>
                                                </td>
            <td>
                <asp:TextBox ID="txtSpecifiedArea" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedBf" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedTS" runat="server" CssClass="input-field" readonly="true"/>Min:</td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedL0" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedL1" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedEL" runat="server" CssClass="input-field" readonly="true" />Min.</td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedMedianTS" runat="server" CssClass="input-field" readonly="true"/>Min.</td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedMedianEL" runat="server" CssClass="input-field"  readonly="true"/>Min.</td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedResistance" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedTemp" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedKt" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedResistivityat20C" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedConductivity" runat="server" CssClass="input-field" /></td>
                                                <td>
                                                    <asp:TextBox ID="txtSpecifiedDensity" runat="server" CssClass="input-field" /></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>

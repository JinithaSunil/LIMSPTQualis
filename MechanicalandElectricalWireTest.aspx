<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="MechanicalandElectricalWireTest.aspx.cs" Inherits="LIMS.MechanicalandElectricalWireTest" %>

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
                            <h2>MECHANICAL & ELECTRICAL WIRE TEST
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
                                            <th rowspan="3">No.</th>
                                            <th colspan="7">Diameter (mm)</th>
                                            <th rowspan="3">Cross-sectional<br />
                                                area<br />
                                                mm²</th>
                                            <th rowspan="3">Breaking force<br />
                                                Kg / N</th>
                                            <th rowspan="3">Tensile strength<br />
                                                Kg/mm²<br />
                                                N/mm²</th>
                                            <th colspan="2">Elongation</th>
                                            <th rowspan="3">Stress at 1%<br />
                                                Kg/mm²</th>
                                            <th rowspan="3">Resistance<br />
                                                Ω.m</th>
                                            <th rowspan="3">Resistivity<br />
                                                Ω·mm²</th>
                                            <th rowspan="3">Wrapping<br />
                                                …./…./….</th>
                                            <th rowspan="3">Torsion</th>
                                            <th rowspan="3">Add</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Nom</th>
                                            <th rowspan="2">L₀<br />
                                                mm</th>
                                            <th rowspan="2">mm<br />
                                                %</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtNo" runat="server" CssClass="input-field" /></td>
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
                                                <asp:TextBox ID="txtNom" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtCrossSectionArea" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtBreakingForce" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtTensileStrength" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtEL0" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtElmm" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtStressAt1Percent" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtResistance" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtResistivity" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtWrapping" runat="server" CssClass="input-field" /></td>
                                            <td>
                                                <asp:TextBox ID="txtTorsion" runat="server" CssClass="input-field" /></td>
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
                                    <h4>MECHANICAL & ELECTRICAL WIRE TEST
                                    </h4>
                                    <div class="table-responsive tests-section">

                                        <asp:GridView ID="gvcopper" runat="server" AutoGenerateColumns="false" OnRowDataBound="gvcopper_RowDataBound" OnRowCommand="gvcopper_RowCommand" OnRowCreated="gvcopper_RowCreated">
                                            <Columns>
                                                <asp:BoundField DataField="No" HeaderText="No." />
                                                <asp:BoundField DataField="Diameter1" HeaderText="Diameter 1" />
                                                <asp:BoundField DataField="Diameter2" HeaderText="Diameter 2" />
                                                <asp:BoundField DataField="Diameter3" HeaderText="Diameter 3" />
                                                <asp:BoundField DataField="Diameter4" HeaderText="Diameter 4" />
                                                <asp:BoundField DataField="Diameter5" HeaderText="Diameter 5" />
                                                <asp:BoundField DataField="Diameter6" HeaderText="Diameter 6" />
                                                <asp:BoundField DataField="Nom" HeaderText="Nom" />
                                                <asp:BoundField DataField="Area" HeaderText="Cross-sectional area" />
                                                <asp:BoundField DataField="BF" HeaderText="Breaking force" />
                                                <asp:BoundField DataField="TS" HeaderText="Tensile strength" />
                                                <asp:BoundField DataField="L0" HeaderText="L₀ (mm)" />
                                                <asp:BoundField DataField="Elmm" HeaderText="Elongation mm/%" />
                                                <asp:BoundField DataField="StressAt1Percent" HeaderText="Stress at 1%" />
                                                <asp:BoundField DataField="Resistance" HeaderText="Resistance" />
                                                <asp:BoundField DataField="Resistivity" HeaderText="Resistivity" />
                                                <asp:BoundField DataField="Wrapping" HeaderText="Wrapping" />
                                                <asp:BoundField DataField="Torsion" HeaderText="Torsion" />

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
                                    <h4>MECHANICAL & ELECTRICAL WIRE TEST Standard 
</h4>
                                    <table class="table table-bordered text-center" style="width: 100%; border-collapse: collapse;">
                                        <thead>
                                            <tr>
                                                <th rowspan="3">No.</th>
                                                <th colspan="7">Diameter (mm)</th>
                                                <th rowspan="3">Cross-sectional<br />
                                                    area<br />
                                                    mm²</th>
                                                <th rowspan="3">Breaking force<br />
                                                    Kg / N</th>
                                                <th rowspan="3">Tensile strength<br />
                                                    Kg/mm²<br />
                                                    N/mm²</th>
                                                <th colspan="2">Elongation</th>
                                                <th rowspan="3">Stress at 1%<br />
                                                    Kg/mm²</th>
                                                <th rowspan="3">Resistance<br />
                                                    Ω.m</th>
                                                <th rowspan="3">Resistivity<br />
                                                    Ω·mm²</th>
                                                <th rowspan="3">Wrapping<br />
                                                    …./…./….</th>
                                                <th rowspan="3">Torsion</th>
                                            </tr>
                                            <tr>
                                                <th>1</th>
                                                <th>2</th>
                                                <th>3</th>
                                                <th>4</th>
                                                <th>5</th>
                                                <th>6</th>
                                                <th>Nom</th>
                                                <th rowspan="2">L₀<br />
                                                    mm</th>
                                                <th rowspan="2">mm<br />
                                                    %</th>
                                            </tr>
                                            <tr>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>Standard</td>
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtStandardDiameterMin" runat="server" CssClass="input-field" />
                                                    Min:
                                                </td>
                                               
                                                <td colspan="3">
                                                    <asp:TextBox ID="txtStandardDiameterMax" runat="server" CssClass="input-field" readonly="true"/>
                                                    Max:
                                                </td>
                                                 <td colspan="1">
     <asp:TextBox ID="txtStandardDiameterNom" runat="server" CssClass="input-field"  placeholder="Enter Value"/>
     Nom:
 </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardcrosssectionarea" runat="server" CssClass="input-field" placeholder="Enter Value"/>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardbreakingforce" runat="server" CssClass="input-field"  readonly="true"/>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardTensilestrength" runat="server" CssClass="input-field"  readonly="true"/>
                                                    Min:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardElongationL0" runat="server" CssClass="input-field" placeholder="Enter Value"/>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardElongationmm" runat="server" CssClass="input-field"  readonly="true"/>
                                                    Min:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardstressat1" runat="server" CssClass="input-field"  readonly="true"/>
                                                    Min:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardresistance" runat="server" CssClass="input-field" placeholder="Enter Value"/>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardresistivity" runat="server" CssClass="input-field"  readonly="true"/>
                                                    Max:
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardwrapping" runat="server" CssClass="input-field" placeholder="Enter Value"/>
                                                    No Crack
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtStandardtorsion" runat="server" CssClass="input-field"  readonly="true"/>
                                                    Min:
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="5">Correction temperature:
      <asp:TextBox ID="txtcorrectionTemp" CssClass="input-field" runat="server" placeholder="Enter Value"/>
                                                </td>
                                                <td colspan="8">Minimum breaking load (calculation):
      <asp:TextBox ID="txtminimumbrekaingload" CssClass="input-field" runat="server" placeholder="Enter Value"/>
                                                </td>
                                                <td colspan="5">N
      <asp:TextBox ID="txtstandardfor" CssClass="input-field" runat="server" placeholder="Enter Value" />
                                                </td>
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


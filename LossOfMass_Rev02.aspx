<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="LossOfMass_Rev02.aspx.cs" Inherits="LIMS.LossOfMass_Rev02" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f5f5f5;
        }

        .card  {
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
                            <h2>LOSS OF MASS</h2>
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
                                        <td><b>Drum No.</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvVoltage" runat="server"
                                                ControlToValidate="voltage"
                                                ErrorMessage="drum number is required"
                                                Display="Dynamic" ForeColor="Red" />
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
                                            <th style="width:7%" rowspan="3">Core Phase</th>
                                            <th style="width:1%" rowspan="3">Specimen No.</th>
                                            <th colspan="7">Thickness/Weight</th>
                                            <th colspan="7">Width/Diameter/Length/Density</th>
                                            <th rowspan="2" colspan="1">Cross<br>
                                                sectional<br>
                                                area</th>
                                            <th colspan="2">Weight</th>
                                            <th rowspan="2">Loss of<br>
                                                mass</th>

                                            <th rowspan="3">Add Data</th>
                                        </tr>
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th style="width:7%">Avg.<br>
                                                min.</th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Avg.<br>
                                                min.</th>
                                            <th>Before<br>
                                                ageing</th>
                                            <th>After<br>
                                                ageing</th>

                                        </tr>
                                        <tr>
                                            <th colspan="7">mm/gr</th>
                                            <th colspan="7">mm/[gr/cm²]</th>
                                            <th>cm²</th>
                                            <th colspan="2">mg</th>
                                            <th>mg/cm²</th>


                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control"
                                                    AutoPostBack="true"
                                                    Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput4" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput5" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInput6" runat="server" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" CssClass="form-control" />

                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtthicknessInputAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput4" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput5" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInput6" runat="server" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" CssClass="form-control" />

                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdiameterInputAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <asp:HiddenField ID="hiddenThicknessAvg" runat="server" />
                                            <asp:HiddenField ID="hiddenThicknessMin" runat="server" />
                                            <asp:HiddenField ID="hiddenDiameterAvg" runat="server" />
                                            <asp:HiddenField ID="hiddenDiameterMin" runat="server" />

                                            <td>
                                                <asp:TextBox ID="txtCrossSectionarea" runat="server" CssClass="form-control" />
                                            </td>


                                            <td>
                                                <asp:TextBox ID="txtbeforeageing" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtafterkageing" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtloadofmass" runat="server" CssClass="form-control" />
                                            </td>

                                            <td style="text-align: center;">
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>


                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>LOSS OF MASS Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvPressureTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped"
                                            OnRowDataBound="gvPressureTest_RowDataBound"
                                            OnRowCreated="gvPressureTest_RowCreated"
                                            OnRowCommand="gvPressureTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core" />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />

                                                <asp:BoundField DataField="InputthicknessInput1" HeaderText="1" />
                                                <asp:BoundField DataField="InputthicknessInput2" HeaderText="2" />
                                                <asp:BoundField DataField="InputthicknessInput3" HeaderText="3" />
                                                <asp:BoundField DataField="InputthicknessInput4" HeaderText="4" />
                                                <asp:BoundField DataField="InputthicknessInput5" HeaderText="5" />
                                                <asp:BoundField DataField="InputthicknessInput6" HeaderText="6" />
                                                <asp:TemplateField HeaderText="Thickness (Avg / Min)">
                                                    <ItemTemplate>
                                                        Avg: <%# Eval("InputthicknessInputAvg", "{0:F2}") %>, Min: <%# Eval("InputthicknessInputMin", "{0:F2}") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:BoundField DataField="InputdiameterInput1" HeaderText="1" />
                                                <asp:BoundField DataField="InputdiameterInput2" HeaderText="2" />
                                                <asp:BoundField DataField="InputdiameterInput3" HeaderText="3" />
                                                <asp:BoundField DataField="InputdiameterInput4" HeaderText="4" />
                                                <asp:BoundField DataField="InputdiameterInput5" HeaderText="5" />
                                                <asp:BoundField DataField="InputdiameterInput6" HeaderText="6" />
                                                <asp:TemplateField HeaderText="Diameter (Avg & Min)">
                                                    <ItemTemplate>
                                                        Avg: <%# Eval("InputdiameterInputAvg", "{0:F2}") %>, Min: <%# Eval("InputdiameterInputMin", "{0:F2}") %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="CrossSectionarea" HeaderText="Cross Section Area" />
                                                <asp:BoundField DataField="BeforeAgeing" HeaderText="Before Ageing" />
                                                <asp:BoundField DataField="AfterAgeing" HeaderText="After Ageing" />
                                                <asp:BoundField DataField="LossOfMass" HeaderText="Loss Of Mass" />

                                                <asp:BoundField DataField="Median" HeaderText="Median" />


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

                            <!-- Standard Row - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Standard Values</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th rowspan="2">Specified</th>
                                                    <th rowspan="2">Thickness<br>
                                                    </th>
                                                    <th rowspan="2">Diameter<br>
                                                    </th>
                                                    <th rowspan="2">Corss Section Area<br>
                                                    </th>
                                                    <th colspan="2">Weight</th>
                                                    <th rowspan="2">Loss Of Mass<br>
                                                    </th>
                                                    <th rowspan="2">Median</th>
                                                </tr>
                                                <tr>
                                                    <th>Before Ageing</th>
                                                    <th>After Ageing</th>
                                                </tr>
                                                <tr>
                                                    <th></th>
                                                    <th>mm/gr</th>
                                                    <th>mm/(gr/cm²)</th>
                                                    <th>cm²</th>
                                                    <th colspan="2">mg</th>
                                                    <th colspan="2">mg/cm2</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>Specified</td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificThickness" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificDiameter" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificcrosssecctionarea" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificbeforeageing" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="temperature at:" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificafterageing" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="temperature at:" />
                                                    </td>
                                                    <td colspan="2">
                                                        <asp:TextBox ID="txtSpecificMedian" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="Max:" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <table class="table table-bordered">
                                <thead>
                                    <tr>
                                        <th colspan="4" class="text-center">Conditioning in desiccators before ageing</th>
                                        <th colspan="4" class="text-center">Ageing in air oven</th>
                                    </tr>
                                    <tr>
                                        <th colspan="2" class="text-center">Date</th>
                                        <th colspan="2" class="text-center">Time</th>
                                        <th colspan="2" class="text-center">Date</th>
                                        <th colspan="2" class="text-center">Time</th>
                                    </tr>
                                    <tr>
                                        <th class="text-center">Start</th>
                                        <th class="text-center">Finish</th>
                                        <th class="text-center">Start</th>
                                        <th class="text-center">Finish</th>
                                        <th class="text-center">Start</th>
                                        <th class="text-center">Finish</th>
                                        <th class="text-center">Start</th>
                                        <th class="text-center">Finish</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtDesicBeforeDateStart" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtDesicBeforeDateFinish" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtDesicBeforeTimeStart" runat="server" CssClass="form-control" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtDesicBeforeTimeFinish" runat="server" CssClass="form-control" TextMode="Time" /></td>

                                        <td>
                                            <asp:TextBox ID="txtAirOvenDateStart" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtAirOvenDateFinish" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtAirOvenTimeStart" runat="server" CssClass="form-control" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtAirOvenTimeFinish" runat="server" CssClass="form-control" TextMode="Time" /></td>
                                    </tr>
                                    <tr>
                                        <th colspan="4" class="text-center">Conditioning in desiccators after ageing</th>
                                        <th colspan="2">Airflow</th>
                                        <th colspan="2">Oven Temperature</th>

                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtDesicAfterDateStart" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtDesicAfterDateFinish" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtDesicAfterTimeStart" runat="server" CssClass="form-control" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtDesicAfterTimeFinish" runat="server" CssClass="form-control" TextMode="Time" /></td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtAirflow" runat="server" CssClass="form-control" /></td>
                                        <td colspan="2">
                                            <asp:TextBox ID="txtOvenTemp" runat="server" CssClass="form-control" /></td>

                                    </tr>
                                </tbody>
                            </table>

                            </table>
                            <div>
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

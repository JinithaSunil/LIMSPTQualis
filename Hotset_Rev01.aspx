<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Hotset_Rev01.aspx.cs" Inherits="LIMS.Hotset_Rev01" %>


<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
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
                            <h2>HOTSET</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" ReadOnly="true" />

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
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" ReadOnly="true" />

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
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />

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
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" ReadOnly="true" />

                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <div class="table-responsive">
                                <div style="overflow-x: auto; overflow-y: auto; max-height: 500px;">
                                    <table class="table table-bordered text-center">
                                        <thead>
                                            <tr>

                                                <th rowspan="3" style="width: 120px">Material</th>
                                                <th rowspan="3" style="width: 90px">Core Phase</th>
                                                <th rowspan="3" style="width: 120px">Type of Sample Test</th>
                                                <th rowspan="3">Specimen number</th>
                                                <th colspan="2">Thickness</th>
                                                <th colspan="2">Diameter/Width</th>
                                                <th rowspan="2">Area</th>
                                                <th rowspan="2">WeigthtLoad</th>
                                                <th rowspan="2">Initial Length</th>
                                                <th colspan="3">After: 
                                            <asp:TextBox runat="server" ID="txtaftertemp" Style="text-align: center" />
                                                    under load at temperature: 
                                            <asp:TextBox runat="server" ID="txtunderloadatTemp" Style="width: 40px; text-align: center;" />°C
                                                </th>
                                                <th colspan="3">After Cooling</th>
                                                <th rowspan="3">Add Data</th>
                                            </tr>
                                            <tr>
                                                <th>inputes</th>
                                                <th>Min Max</th>

                                                <th>inputes</th>
                                                <th>Min Max</th>
                                                <th>Length</th>
                                                <th>EL</th>
                                                <th>Median</th>
                                                <th>Length</th>
                                                <th>EL</th>
                                                <th>Median</th>
                                            </tr>
                                            <tr>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm</th>
                                                <th>mm square</th>
                                                <th>gr</th>
                                                <th>mm2</th>
                                                <th>mm</th>
                                                <th>%</th>
                                                <th>%</th>
                                                <th>mm</th>
                                                <th>%</th>
                                                <th>%</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtmaterial" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtsampletest" runat="server" CssClass="form-control" Style="text-align: center;">
                                                    </asp:TextBox>
                                                </td>

                                                <td>
                                                    <asp:TextBox ID="txtSpecimenNumber" runat="server" CssClass="form-control"
                                                        AutoPostBack="true"
                                                        Style="text-align: center;" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtThickness" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                                </td>

                                                <td>
                                                    <asp:TextBox ID="txtthicknessInputAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtdiameterInputs" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtdiameterInputAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                </td>

                                                <td>
                                                    <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtWeightload" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtInitiallength" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtlengthafter" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtELafter" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtmedianafter" runat="server" CssClass="form-control" ReadOnly="true" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtaftercoolinglength" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtaftercoolingEL" runat="server" CssClass="form-control" />
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtaftercoolingMedian" runat="server" CssClass="form-control" ReadOnly="true" />
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
                                        <h4>HOTSET</h4>
                                        <div class="table-responsive">
                                            <asp:GridView ID="gvPressureTest" runat="server" AutoGenerateColumns="false"
                                                CssClass="table table-bordered table-striped"
                                                OnRowDataBound="gvPressureTest_RowDataBound"
                                                OnRowCreated="gvPressureTest_RowCreated"
                                                OnRowCommand="gvPressureTest_RowCommand">
                                                <Columns>
                                                    <asp:BoundField DataField="Material" HeaderText="Material" />
                                                    <asp:BoundField DataField="CorePhase" HeaderText="Core" />
                                                    <asp:BoundField DataField="SampleType" HeaderText="SampleType" />
                                                    <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />
                                                    <asp:BoundField DataField="Thickness" HeaderText="Thickness" />
                                                    <asp:BoundField DataField="ThicknessAvgMin" HeaderText="ThicknessAvgMin" />
                                                    <asp:BoundField DataField="WidthDiameter" HeaderText="WidthDiameter" />
                                                    <asp:BoundField DataField="WidthAvgMin" HeaderText="Width Avg Min" />
                                                    <asp:BoundField DataField="Area" HeaderText="Area" />
                                                    <asp:BoundField DataField="Weightload" HeaderText="Weight Load" />
                                                    <asp:BoundField DataField="Initiallength" HeaderText="Initial length" />
                                                    <asp:BoundField DataField="Length" HeaderText="Length" />
                                                    <asp:BoundField DataField="EL" HeaderText="EL" />
                                                    <asp:BoundField DataField="Median" HeaderText="Median" />
                                                    <asp:BoundField DataField="coolingLength" HeaderText="coolingLength" />
                                                    <asp:BoundField DataField="coolingEL" HeaderText="coolingEL" />
                                                    <asp:BoundField DataField="coolingMedian" HeaderText="coolingMedian" />
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
                                <div class="row clearfix mt-3">
                                    <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                        <h4>Standard Values</h4>
                                        <div class="table-responsive">
                                            <table class="table table-bordered">
                                                <thead>
                                                    <tr>
                                                        <th rowspan="3">Specified</th>
                                                        <th rowspan="2">Thickness<br>
                                                        </th>
                                                        <th rowspan="2">Diameter<br>
                                                        </th>
                                                        <th rowspan="2">Area<br>
                                                        </th>

                                                        <th rowspan="2">Weight Load<br>
                                                        </th>
                                                        <th rowspan="2">Initial length<br>
                                                        </th>
                                                        <th colspan="3">After __ minutes under load at temperature _oC</th>
                                                        <th colspan="3">After Cooling<br>
                                                    </tr>
                                                    <tr>
                                                        <th>Length</th>
                                                        <th>EL</th>
                                                        <th>Median</th>
                                                        <th>Length</th>
                                                        <th>EL</th>
                                                        <th>Median</th>
                                                    </tr>
                                                    <tr>

                                                        <th>mm</th>
                                                        <th>mm</th>
                                                        <th>mmsquare</th>
                                                        <th>gr</th>
                                                        <th>mm</th>
                                                        <th>mm</th>
                                                        <th>%</th>
                                                        <th>%</th>
                                                        <th>mm</th>
                                                        <th>%</th>
                                                        <th>%</th>
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
                                                            <asp:TextBox ID="txtSpecificArea" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                        </td>

                                                        <td>
                                                            <asp:TextBox ID="txtSpecificWeightload" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSpecificInitiallength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                        </td>

                                                        <td>
                                                            <asp:TextBox ID="txtSpecificLength" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="temperature at:" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSpecificEL" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="temperature at:" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSpecificMedian" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="Max:" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSpecificoolingLength" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="Max:" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSpecificoolingEL" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="Max:" />
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSpecificoolingMedian" runat="server" CssClass="form-control" Style="text-align: center;" placeholder="Max:" />
                                                        </td>


                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>


                                <div>
                                </div>
                            </div>
                        </div>
                        <div class="form-actions">
                            <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                            <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" />
                            <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                        </div>
                    </div>
                </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

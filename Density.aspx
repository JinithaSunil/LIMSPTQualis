<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Density.aspx.cs" Inherits="LIMS.Density" %>



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
                            <h2>DENSITY</h2>
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
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" ReadOnly="true" />

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
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th>No</th>
                                            <th>Item Test</th>
                                            <th>Unit</th>
                                            <th>Specified</th>
                                            <th colspan="5">Measured</th>

                                        </tr>

                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>1</td>
                                            <td>Weight of sample</td>
                                            <td>
                                                <asp:DropDownList ID="ddlweight_sample1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="gr/cm3">gr/cm³</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleUnit1" ControlToValidate="ddlweight_sample1" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sampleSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSpecified" ControlToValidate="weight_sampleSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sampleinput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput1" ControlToValidate="weight_sampleinput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sampleinput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput2" ControlToValidate="weight_sampleinput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sampleinput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput3" ControlToValidate="weight_sampleinput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sampleinput4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput4" ControlToValidate="weight_sampleinput4" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sampleinput5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleInput5" ControlToValidate="weight_sampleinput5" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>2</td>
                                            <td>Solution</td>
                                            <td>
                                                <asp:DropDownList ID="ddlsolution2" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="gr/cm3">gr/cm³</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvSolutionUnit2" ControlToValidate="ddlsolution2" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="solutionSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSolutionSpecified" ControlToValidate="solutionSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="solutioninput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSolutionInput1" ControlToValidate="solutioninput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="solutioninput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSolutionInput2" ControlToValidate="solutioninput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="solutioninput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSolutionInput3" ControlToValidate="solutioninput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="solutioninput4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSolutionInput4" ControlToValidate="solutioninput4" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="solutioninput5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSolutionInput5" ControlToValidate="solutioninput5" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>3</td>
                                            <td>Weight of sample in solution</td>
                                            <td>
                                                <asp:DropDownList ID="ddlweight_sample_solution3" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="gr/cm3">gr/cm³</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionUnit3" ControlToValidate="ddlweight_sample_solution3" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sample_solutionSpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionSpecified" ControlToValidate="weight_sample_solutionSpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sample_solutioninput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionInput1" ControlToValidate="weight_sample_solutioninput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sample_solutioninput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionInput2" ControlToValidate="weight_sample_solutioninput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sample_solutioninput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionInput3" ControlToValidate="weight_sample_solutioninput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sample_solutioninput4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionInput4" ControlToValidate="weight_sample_solutioninput4" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="weight_sample_solutioninput5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvWeightSampleSolutionInput5" ControlToValidate="weight_sample_solutioninput5" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>4</td>
                                            <td>Density</td>
                                            <td>
                                                <asp:DropDownList ID="ddldensity4" runat="server" CssClass="form-control">
                                                    <asp:ListItem Value="">--Select--</asp:ListItem>
                                                    <asp:ListItem Value="gr">gr</asp:ListItem>
                                                    <asp:ListItem Value="gr/cm3">gr/cm³</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvDensityUnit4" ControlToValidate="ddldensity4" runat="server"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="densitySpecified" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDensitySpecified" ControlToValidate="densitySpecified" runat="server"
                                                    ErrorMessage="Specified Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="densityinput1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDensityInput1" ControlToValidate="densityinput1" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="densityinput2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDensityInput2" ControlToValidate="densityinput2" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="densityinput3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDensityInput3" ControlToValidate="densityinput3" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="densityinput4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDensityInput4" ControlToValidate="densityinput4" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="densityinput5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvDensityInput5" ControlToValidate="densityinput5" runat="server"
                                                    ErrorMessage="Value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>
                                    </tbody>


                                </table>
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

<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="CreepTest.aspx.cs" Inherits="LIMS.CreepTest" %>

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

     .input-row {
    display: flex;
    justify-content: flex-start;
    gap: 25px; /* Increased space between boxes */
    margin-bottom: 25px; /* Increased vertical space */
    flex-wrap: wrap;
}

.input-box {
    display: flex;
    flex-direction: column-reverse;
    align-items: center;
    width: 140px; /* Same width */
}

.input-box label {
    margin-top: 4px;
    font-size: 13px;
    text-align: center;
    color: #333;
    line-height: 1.2;
}

.form-control {
    width: 100%;
    padding: 5px;
    border: 1px solid #ddd;
    border-radius: 3px;
    font-size: 14px;
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
                            <h2>CREEP TEST</h2>
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
                            <br />
                            <div class="input-row">
                                <div class="input-box">
                                    <asp:TextBox ID="txtStarttime" runat="server" AutoPostBack="true" OnTextChanged="txtStarttime_TextChanged" CssClass="form-control" />
                                    <label for="txtStarttime">Start Time</label>
                                </div>

                                <div class="input-box">
                                    <asp:TextBox runat="server" ID="txtappliedload" CssClass="form-control" />
                                    <label for="txtappliedload">Applied Load (kN)<br />
                                        (20% of RTS)</label>
                                </div>

                                <div class="input-box">
                                    <asp:TextBox runat="server" ID="txtgaugelength" CssClass="form-control" />
                                    <label for="txtgaugelength">Gauge Length (mm)</label>
                                </div>
                            </div>

                            <!-- Validators -->
                            <asp:RequiredFieldValidator ID="rfvAppliedLoad" runat="server" ControlToValidate="txtappliedload"
                                ErrorMessage="Applied load is required." ForeColor="Red" Display="Dynamic" />

                            <asp:RequiredFieldValidator ID="rfvGaugeLength" runat="server" ControlToValidate="txtgaugelength"
                                ErrorMessage="Gauge length is required." ForeColor="Red" Display="Dynamic" />
                            <br />
                            <div class="table-responsive">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">Date of test</th>
                                            <th rowspan="2">Duration</th>
                                            <th rowspan="2">Time</th>
                                            <th colspan="2">Measurement</th>
                                            <th rowspan="2">average</th>
                                        </tr>
                                        <tr>
                                            <th>Extension I</th>
                                            <th>Extension II</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest1" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest1" runat="server" ControlToValidate="txtDateOfTest1" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration1" runat="server" CssClass="form-control" ReadOnly="true" Text="1 minute" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime1" runat="server" ControlToValidate="txtTime1" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_1" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_1" runat="server" ControlToValidate="txtextension1_1" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_1" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_1" runat="server" ControlToValidate="txtextension2_1" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_1" runat="server" CssClass="form-control"  ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg1" runat="server" ControlToValidate="txtavg_1" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest2" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest2" runat="server" ControlToValidate="txtDateOfTest2" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration2" runat="server" CssClass="form-control" ReadOnly="true" Text="2 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime2" runat="server" ControlToValidate="txtTime2" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_2" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_2" runat="server" ControlToValidate="txtextension1_2" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_2" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_2" runat="server" ControlToValidate="txtextension2_2" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_2" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg2" runat="server" ControlToValidate="txtavg_2" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest3" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest3" runat="server" ControlToValidate="txtDateOfTest3" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration3" runat="server" CssClass="form-control" ReadOnly="true" Text="3 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime3" runat="server" ControlToValidate="txtTime3" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_3" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_3" runat="server" ControlToValidate="txtextension1_3" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_3" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_3" runat="server" ControlToValidate="txtextension2_3" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_3" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg3" runat="server" ControlToValidate="txtavg_3" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest4" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest4" runat="server" ControlToValidate="txtDateOfTest4" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration4" runat="server" CssClass="form-control" ReadOnly="true" Text="4 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime4" runat="server" ControlToValidate="txtTime4" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_4" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_4" runat="server" ControlToValidate="txtextension1_4" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_4" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_4" runat="server" ControlToValidate="txtextension2_4" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_4" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg4" runat="server" ControlToValidate="txtavg_4" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest5" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest5" runat="server" ControlToValidate="txtDateOfTest5" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration5" runat="server" CssClass="form-control" ReadOnly="true" Text="5 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime5" runat="server" ControlToValidate="txtTime5" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_5" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_5" runat="server" ControlToValidate="txtextension1_5" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_5" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_5" runat="server" ControlToValidate="txtextension2_5" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_5" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg5" runat="server" ControlToValidate="txtavg_5" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest6" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest6" runat="server" ControlToValidate="txtDateOfTest6" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration6" runat="server" CssClass="form-control" ReadOnly="true" Text="10 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime6" runat="server" ControlToValidate="txtTime6" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_6" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_6" runat="server" ControlToValidate="txtextension1_6" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_6" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_6" runat="server" ControlToValidate="txtextension2_6" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_6" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg6" runat="server" ControlToValidate="txtavg_6" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest7" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest7" runat="server" ControlToValidate="txtDateOfTest7" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration7" runat="server" CssClass="form-control" ReadOnly="true" Text="15 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime7" runat="server" ControlToValidate="txtTime7" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_7" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_7" runat="server" ControlToValidate="txtextension1_7" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_7" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_7" runat="server" ControlToValidate="txtextension2_7" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_7" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg7" runat="server" ControlToValidate="txtavg_7" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest8" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest8" runat="server" ControlToValidate="txtDateOfTest8" Display="Dynamic"
                                                    ErrorMessage="* Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration8" runat="server" CssClass="form-control" ReadOnly="true" Text="30 minutes" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime8" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime8" runat="server" ControlToValidate="txtTime8" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_8" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_8" runat="server" ControlToValidate="txtextension1_8" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_8" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_8" runat="server" ControlToValidate="txtextension2_8" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_8" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg8" runat="server" ControlToValidate="txtavg_8" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest9" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest9" runat="server" ControlToValidate="txtDateOfTest9" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration9" runat="server" CssClass="form-control" ReadOnly="true" Text="1 hour" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime9" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime9" runat="server" ControlToValidate="txtTime9" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_9" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_9" runat="server" ControlToValidate="txtextension1_9" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_9" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_9" runat="server" ControlToValidate="txtextension2_9" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_9" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg9" runat="server" ControlToValidate="txtavg_9" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest10" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest10" runat="server" ControlToValidate="txtDateOfTest10" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration10" runat="server" CssClass="form-control" ReadOnly="true" Text="2 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime10" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime10" runat="server" ControlToValidate="txtTime10" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_10" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_10" runat="server" ControlToValidate="txtextension1_10" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_10" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_10" runat="server" ControlToValidate="txtextension2_10" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_10" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg10" runat="server" ControlToValidate="txtavg_10" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest11" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest11" runat="server" ControlToValidate="txtDateOfTest11" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration11" runat="server" CssClass="form-control" ReadOnly="true" Text="3 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime11" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime11" runat="server" ControlToValidate="txtTime11" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_11" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_11" runat="server" ControlToValidate="txtextension1_11" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_11" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_11" runat="server" ControlToValidate="txtextension2_11" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_11" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg11" runat="server" ControlToValidate="txtavg_11" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest12" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest12" runat="server" ControlToValidate="txtDateOfTest12" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration12" runat="server" CssClass="form-control" ReadOnly="true" Text="4 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime12" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime12" runat="server" ControlToValidate="txtTime12" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_12" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_12" runat="server" ControlToValidate="txtextension1_12" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_12" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_12" runat="server" ControlToValidate="txtextension2_12" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_12" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg12" runat="server" ControlToValidate="txtavg_12" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest13" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest13" runat="server" ControlToValidate="txtDateOfTest13" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration13" runat="server" CssClass="form-control" ReadOnly="true" Text="5 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime13" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime13" runat="server" ControlToValidate="txtTime13" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_13" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_13" runat="server" ControlToValidate="txtextension1_13" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_13" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_13" runat="server" ControlToValidate="txtextension2_13" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_13" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg13" runat="server" ControlToValidate="txtavg_13" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest14" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest14" runat="server" ControlToValidate="txtDateOfTest14" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration14" runat="server" CssClass="form-control" ReadOnly="true" Text="6 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime14" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime14" runat="server" ControlToValidate="txtTime14" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_14" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_14" runat="server" ControlToValidate="txtextension1_14" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_14" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_14" runat="server" ControlToValidate="txtextension2_14" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_14" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg14" runat="server" ControlToValidate="txtavg_14" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest15" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest15" runat="server" ControlToValidate="txtDateOfTest15" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration15" runat="server" CssClass="form-control" ReadOnly="true" Text="7 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime15" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime15" runat="server" ControlToValidate="txtTime15" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_15" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_15" runat="server" ControlToValidate="txtextension1_15" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_15" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_15" runat="server" ControlToValidate="txtextension2_15" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_15" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg15" runat="server" ControlToValidate="txtavg_15" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest16" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest16" runat="server" ControlToValidate="txtDateOfTest16" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration16" runat="server" CssClass="form-control" ReadOnly="true" Text="8 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime16" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime16" runat="server" ControlToValidate="txtTime16" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_16" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_16" runat="server" ControlToValidate="txtextension1_16" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_16" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_16" runat="server" ControlToValidate="txtextension2_16" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_16" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg16" runat="server" ControlToValidate="txtavg_16" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest17" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest17" runat="server" ControlToValidate="txtDateOfTest17" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration17" runat="server" CssClass="form-control" ReadOnly="true" Text="9 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime17" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime17" runat="server" ControlToValidate="txtTime17" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_17" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_17" runat="server" ControlToValidate="txtextension1_17" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_17" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_17" runat="server" ControlToValidate="txtextension2_17" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_17" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg17" runat="server" ControlToValidate="txtavg_17" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest18" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest18" runat="server" ControlToValidate="txtDateOfTest18" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration18" runat="server" CssClass="form-control" ReadOnly="true" Text="10 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime18" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime18" runat="server" ControlToValidate="txtTime18" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_18" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_18" runat="server" ControlToValidate="txtextension1_18" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_18" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_18" runat="server" ControlToValidate="txtextension2_18" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_18" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg18" runat="server" ControlToValidate="txtavg_18" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest19" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest19" runat="server" ControlToValidate="txtDateOfTest19" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration19" runat="server" CssClass="form-control" ReadOnly="true" Text="11 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime19" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime19" runat="server" ControlToValidate="txtTime19" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_19" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_19" runat="server" ControlToValidate="txtextension1_19" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_19" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_19" runat="server" ControlToValidate="txtextension2_19" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_19" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg19" runat="server" ControlToValidate="txtavg_19" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest20" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest20" runat="server" ControlToValidate="txtDateOfTest20" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration20" runat="server" CssClass="form-control" ReadOnly="true" Text="12 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime20" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime20" runat="server" ControlToValidate="txtTime20" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_20" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_20" runat="server" ControlToValidate="txtextension1_20" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_20" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_20" runat="server" ControlToValidate="txtextension2_20" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_20" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg20" runat="server" ControlToValidate="txtavg_20" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest21" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest21" runat="server" ControlToValidate="txtDateOfTest21" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration21" runat="server" CssClass="form-control" ReadOnly="true" Text="13 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime21" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime21" runat="server" ControlToValidate="txtTime21" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_21" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_21" runat="server" ControlToValidate="txtextension1_21" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_21" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_21" runat="server" ControlToValidate="txtextension2_21" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_21" runat="server" CssClass="form-control" ReadOnly="true"/>
                                                <asp:RequiredFieldValidator ID="rfvAvg21" runat="server" ControlToValidate="txtavg_21" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtDateOfTest22" runat="server" CssClass="form-control" TextMode="Date" />
                                                <asp:RequiredFieldValidator ID="rfvDateOfTest22" runat="server" ControlToValidate="txtDateOfTest22" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDuration22" runat="server" CssClass="form-control" ReadOnly="true" Text="14 hours" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTime22" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTime22" runat="server" ControlToValidate="txtTime22" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension1_22" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt1_22" runat="server" ControlToValidate="txtextension1_22" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtextension2_22" runat="server" CssClass="form-control" OnTextChanged="Extension_TextChanged" AutoPostBack="true" />
                                                <asp:RequiredFieldValidator ID="rfvExt2_22" runat="server" ControlToValidate="txtextension2_22" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtavg_22" runat="server" CssClass="form-control" ReadOnly="true" />
                                                <asp:RequiredFieldValidator ID="rfvAvg22" runat="server" ControlToValidate="txtavg_22" Display="Dynamic"
                                                    ErrorMessage="Value is required" ForeColor="Red" />
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

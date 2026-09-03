<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PengujianGlassTransitionTemperature.aspx.cs" Inherits="LIMS.Tg_Test" %>

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
                            <h2>Pengujian Glass Transition Temperature  
(Tg – Test )
                            </h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control"  readonly="true"/>
                                           
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control"  readonly="true"/>
                                        
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control"   readonly="true"/>
                                          
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" readonly="true"/>
                                          
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Grade</b></td>
                                        <td>
                                            <asp:TextBox ID="grade" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvGrade" runat="server"
                                                ControlToValidate="grade"
                                                ErrorMessage="Grade is required"
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
                            <div class="table-responsive">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th>No</th>
                                            <th>Description</th>
                                            <th>Unit</th>
                                            <th>Specified</th>
                                            <th>Value test</th>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <!-- Section 1 -->
                                        <tr>
                                            <td>1</td>
                                            <td>Dimension</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit1" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit1" runat="server" ControlToValidate="ddlUnit1"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified1" runat="server" ControlToValidate="txtSpecified1"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest1" runat="server" ControlToValidate="txtValueTest1"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                            <td>Thickness</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit2" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit2" runat="server" ControlToValidate="ddlUnit2"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified2" runat="server" ControlToValidate="txtSpecified2"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest2" runat="server" ControlToValidate="txtValueTest2"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                            <td>Width</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit3" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit3" runat="server" ControlToValidate="ddlUnit3"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified3" runat="server" ControlToValidate="txtSpecified3"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest3" runat="server" ControlToValidate="txtValueTest3"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                            <td>Length</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit4" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit4" runat="server" ControlToValidate="ddlUnit4"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified4" runat="server" ControlToValidate="txtSpecified4"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest4" runat="server" ControlToValidate="txtValueTest4"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                            <td>Weight of sample</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit5" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit5" runat="server" ControlToValidate="ddlUnit5"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified5" runat="server" ControlToValidate="txtSpecified5"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest5" runat="server" ControlToValidate="txtValueTest5"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <!-- Section 2 -->
                                        <tr>
                                            <td>2</td>
                                            <td>Condition of specimen</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit6" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit6" runat="server" ControlToValidate="ddlUnit6"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified6" runat="server" ControlToValidate="txtSpecified6"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest6" runat="server" ControlToValidate="txtValueTest6"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <tr>
                                            <td></td>
                                            <td>Temperature</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit7" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit7" runat="server" ControlToValidate="ddlUnit7"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified7" runat="server" ControlToValidate="txtSpecified7"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest7" runat="server" ControlToValidate="txtValueTest7"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>
                                                                       
                                        <tr>
                                            <td></td>
                                            <td>Duration test</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit8" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddlUnit8"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified8" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtSpecified8"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest8" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtValueTest8"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <!-- Row 9 -->
                                        <tr>
                                            <td></td>
                                            <td>Weight of sample after condition</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit9" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit9" runat="server" ControlToValidate="ddlUnit9"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified9" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified9" runat="server" ControlToValidate="txtSpecified9"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest9" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest9" runat="server" ControlToValidate="txtValueTest9"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <!-- Row 10 -->
                                        <tr>
                                            <td>3</td>
                                            <td>Thermoset Polymer</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit10" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit10" runat="server" ControlToValidate="ddlUnit10"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified10" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified10" runat="server" ControlToValidate="txtSpecified10"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest10" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest10" runat="server" ControlToValidate="txtValueTest10"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                        </tr>

                                        <!-- Row 11 -->
                                        <tr>
                                            <td></td>
                                            <td>Weight of sample after test</td>
                                            <td>
                                                <asp:DropDownList ID="ddlUnit11" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="--Select--" Value="" />
                                                    <asp:ListItem Text="gr" Value="gr" />
                                                    <asp:ListItem Text="°C" Value="°C" />
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="hours" Value="hours" />
                                                    <asp:ListItem Text="mm²" Value="mm²" />
                                                    <asp:ListItem Text="kN" Value="kN" />
                                                    <asp:ListItem Text="Mpa" Value="Mpa" />
                                                    <asp:ListItem Text="-" Value="-" />
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvUnit11" runat="server" ControlToValidate="ddlUnit11"
                                                    InitialValue="" ErrorMessage="Unit is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecified11" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvSpecified11" runat="server" ControlToValidate="txtSpecified11"
                                                    ErrorMessage="Specified value is required" ForeColor="Red" Display="Dynamic" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtValueTest11" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvValueTest11" runat="server" ControlToValidate="txtValueTest11"
                                                    ErrorMessage="Value test is required" ForeColor="Red" Display="Dynamic" />
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

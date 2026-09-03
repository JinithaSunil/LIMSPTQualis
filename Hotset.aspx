<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="HOTSET.aspx.cs" Inherits="LIMSStaging.HOTSET" %>

<<<<<<< .mine

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
||||||| .r236

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
=======
<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
>>>>>>> .r267


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
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="updt" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>DIMENSION TYPE TEST MEDIUM-HIGH VOLTAGE</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtcableType" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvCableType" runat="server" ControlToValidate="txtcableType" ErrorMessage="Type is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="cableType" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" readonly="true"/>
                                            <asp:RequiredFieldValidator ID="rfvCableType" runat="server"
                                                ControlToValidate="cableType"
                                                ErrorMessage="Type is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtrefSpec" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server" ControlToValidate="txtrefSpec" ErrorMessage="Ref.Spec. is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="refSpec" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control"  readonly="true"/>
                                            <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server"
                                                ControlToValidate="refSpec"
                                                ErrorMessage="Ref. Spec. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtcableSize" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvCableSize" runat="server" ControlToValidate="txtcableSize" ErrorMessage="Size is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="cableSize" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvCableSize" runat="server"
                                                ControlToValidate="cableSize"
                                                ErrorMessage="Size is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                        <td><b>Test Method No.</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server" ControlToValidate="txttestMethodNo" ErrorMessage="Test Method No. is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="testMethodNo" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server"
                                                ControlToValidate="testMethodNo"
                                                ErrorMessage="Test Method No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvVoltage" runat="server" ControlToValidate="voltage" ErrorMessage="Voltage is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="voltage" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvVoltage" runat="server"
                                                ControlToValidate="voltage"
                                                ErrorMessage="Voltage is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                        <td><b>Job Order No.</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtjobOrderNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server" ControlToValidate="txtjobOrderNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server"
                                                ControlToValidate="jobOrderNo"
                                                ErrorMessage="Job Order No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtdate" runat="server" CssClass="form-control" TextMode="Date" /></td>
                                        <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtdate" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" /></td>
=======
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" TextMode="Date" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvDate" runat="server"
                                                ControlToValidate="Date"
                                                ErrorMessage="Date is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtWorksheetNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server" ControlToValidate="txtWorksheetNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server"
                                                ControlToValidate="worksheetno"
                                                ErrorMessage="Worksheet No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                    </tr>
                                    <tr>
                                        <td><b>Room No. </b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtRoomNo" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server" ControlToValidate="txtRoomNo" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
                                        <td><b>Room Humidity</b></td>
||||||| .r236
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" /></td>
                                        <td><b>Room Humidity (%)</b></td>
=======
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server"
                                                ControlToValidate="roomno"
                                                ErrorMessage="Room No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Room Humidity (%)</b></td>
>>>>>>> .r267
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtRoomhumidity" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvroomHumidity" runat="server" ControlToValidate="txtRoomhumidity" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />

                                        </td>
=======
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomHumidity" runat="server"
                                                ControlToValidate="roomhumidity"
                                                ErrorMessage="Room Humidity is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp.</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtRoomTemp" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvJRoomTemp" runat="server" ControlToValidate="txtRoomTemp" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                        </td>
=======
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomTemp" runat="server"
                                                ControlToValidate="roomtemp"
                                                ErrorMessage="Room Temperature is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                        <td><b>Equipment No.</b></td>
                                        <td>
<<<<<<< .mine
                                            <asp:TextBox ID="txtEquipment" runat="server" CssClass="form-control" /></td>
                                        <asp:RequiredFieldValidator ID="rfvEquipment" runat="server" ControlToValidate="txtEquipment" ErrorMessage="Date is required" CssClass="text-danger" Display="Dynamic" />
||||||| .r236
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
=======
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvEquipmentNo" runat="server"
                                                ControlToValidate="equipmentno"
                                                ErrorMessage="Equipment No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
>>>>>>> .r267
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <div class="table-responsive">
                                <div style="overflow-x: auto; overflow-y: auto; max-height: 500px;">
                                <table class="table table-bordered text-center">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">Material</th>
                                            <th rowspan="2">Core phase</th>
                                            <th rowspan="2">Type of sample test</th>
                                            <th rowspan="2">Specimen No.</th>
                                            <th colspan="7">Thickness</th>
                                            <th colspan="7">Width</th>
                                            <th rowspan="2">Area</th>
                                            <th rowspan="2">Weight</th>
                                            <th rowspan="2">Initial length</th>
                                            <th colspan="3">After 15 minute under Load at temperature 200 °C</th>
                                            <th colspan="3">After cooling</th>
                                        </tr>
                                        <!-- Sub-header row with numbers and units -->
                                        <tr>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min</th>
                                            <th>1</th>
                                            <th>2</th>
                                            <th>3</th>
                                            <th>4</th>
                                            <th>5</th>
                                            <th>6</th>
                                            <th>Min</th>
                                            <th>Length</th>
                                            <th>EL</th>
                                            <th>Median</th>
                                            <th>Length</th>
                                            <th>EL</th>
                                            <th>Median</th>
                                        </tr>
                                        <!-- Units row -->

<<<<<<< .mine
                                        <tr>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td></td>
                                            <td colspan="7">mm</td>
                                            <td colspan="7">mm</td>
                                            <td>mm²</td>
                                            <td>gr</td>
                                            <td>mm</td>
                                            <td>%</td>
                                            <td>%</td>
                                            <td>%</td>
                                            <td>mm</td>
                                            <td>%</td>
                                            <td>%</td>
||||||| .r236
                                            <th rowspan="2">Item Type</th>
                                            <th rowspan="2">Core Phase</th>
                                            <th>Type of sample test</th>
                                            <th rowspan="2">TestCategory</th>
                                            <th rowspan="2">Specimen number</th>
                                            <th rowspan="2">1</th>
                                            <th rowspan="2">2</th>
                                            <th rowspan="2">3</th>
                                            <th rowspan="2">4</th>
                                            <th rowspan="2">5</th>
                                            <th rowspan="2">6</th>
                                            <th rowspan="2">Min</th>
                                            <th rowspan="2">Add Data</th>
=======
                                            <th rowspan="3" style="width:120px">Material</th>
                                            <th rowspan="3" style="width:90px">Core Phase</th>
                                            <th rowspan="3" style="width:120px">Type of Sample Test</th>
                                            <th rowspan="3" >Specimen number</th>
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
>>>>>>> .r267
                                        </tr>
<<<<<<< .mine
||||||| .r236

=======
                                        <tr>
                                            <th>inputes</th>
                                            <th>Min Max</th>

>>>>>>> .r267
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
                                    <!-- Data rows for RED -->
                                    <tbody>
                                        <tr>
                                            <td rowspan="9">XLPE</td>
                                            <td rowspan="3">RED</td>
                                            <td rowspan="3">Dumbell</td>
                                            <td>1</td>
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:DropDownList ID="ddlitemtype" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlItemtype_SelectedIndexChanged" AutoPostBack="true" Style="text-align: center;">
                                                    <asp:ListItem Text="Select" Value="Select" />
                                                    <asp:ListItem Text="XLPE" Value="XLPE" />
                                                </asp:DropDownList>
                                            </td>
=======
                                                <asp:TextBox ID="txtmaterial" runat="server"></asp:TextBox>
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                    <asp:ListItem Text="Sheath" Value="Sheath" />
                                                    <asp:ListItem Text="Red" Value="Red" />
                                                    <asp:ListItem Text="Blue" Value="Blue" />
                                                    <asp:ListItem Text="Yellow" Value="Yellow" />
                                                </asp:DropDownList>
                                            </td>
=======
                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                </asp:DropDownList>
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:DropDownList ID="ddlSampleType" runat="server" CssClass="form-control" AutoPostBack="true"  Style="text-align: center;">
                                                    <asp:ListItem Text="Select" Value="Select" />
                                                    <asp:ListItem Text="Dumbell" Value="Dumbell" />                                                                                                  
                                                </asp:DropDownList>
                                            </td>
=======
                                                <asp:TextBox ID="txtsampletest" runat="server" CssClass="form-control" Style="text-align: center;">
                                                </asp:TextBox>
                                            </td>
>>>>>>> .r267

                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:DropDownList ID="ddlTestCategory" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTestCategory_SelectedIndexChanged" Style="text-align: center;">
                                                    <asp:ListItem Text="Select" Value="Select" />
                                                    <asp:ListItem Text="Thickness" Value="Thickness" />
                                                    <asp:ListItem Text="Width" Value="Width" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control"
                                                    AutoPostBack="true"
                                                    Style="text-align: center;" />
                                            </td>
=======
                                                <asp:TextBox ID="txtSpecimenNumber" runat="server" CssClass="form-control"
                                                    AutoPostBack="true"
                                                    Style="text-align: center;" />
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
=======
                                                <asp:TextBox ID="txtThickness" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="ThicknessInputs_TextChanged" Style="text-align: center;" />
                                            </td>
>>>>>>> .r267

                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
=======
                                                <asp:TextBox ID="txtthicknessInputAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
=======
                                                <asp:TextBox ID="txtdiameterInputs" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="DiameterInputs_TextChanged" Style="text-align: center;" />
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtInput4" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
=======
                                                <asp:TextBox ID="txtdiameterInputAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
>>>>>>> .r267

                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtInput5" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
=======
                                                <asp:TextBox ID="txtArea" runat="server" CssClass="form-control" />
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtInput6" runat="server" AutoPostBack="true" OnTextChanged="Input_TextChanged" CssClass="form-control" />

                                            </td>
=======
                                                <asp:TextBox ID="txtWeightload" runat="server" CssClass="form-control" />
                                            </td>
>>>>>>> .r267
                                            <td>
<<<<<<< .mine
                                                <asp:TextBox ID="txtRed1_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td rowspan="3">
                                                <asp:TextBox ID="txtRed_LoadMedian" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed1_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td rowspan="3">
                                                <asp:TextBox ID="txtRed_CoolMedian" runat="server" CssClass="textbox"></asp:TextBox></td>
||||||| .r236
                                                <asp:TextBox ID="txtMin" runat="server" CssClass="form-control" ReadOnly="true" Style="text-align: center;" />
                                            </td>
                                            <td style="text-align: center;">
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
=======
                                                <asp:TextBox ID="txtInitiallength" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtlengthafter" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtELafter" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtmedianafter" runat="server" CssClass="form-control" readonly="true"/>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtaftercoolinglength" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtaftercoolingEL" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtaftercoolingMedian" runat="server" CssClass="form-control" readonly="true"/>
                                            </td>
                                            <td style="text-align: center;">
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
>>>>>>> .r267
                                        </tr>
<<<<<<< .mine
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed2_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtRed3_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <!-- Data rows for BLUE -->
                                        <tr>
                                            <td rowspan="3">BLUE</td>
                                            <td rowspan="3">Dumbell</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td rowspan="3">
                                                <asp:TextBox ID="txtBlue_LoadMedian" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue1_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td rowspan="3">
                                                <asp:TextBox ID="txtBlue_CoolMedian" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue2_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtBlue3_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <!-- Data rows for YELLOW -->
                                        <tr>
                                            <td rowspan="3">YELLOW</td>
                                            <td rowspan="3">Dumbell</td>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td rowspan="3">
                                                <asp:TextBox ID="txtYellow_LoadMedian" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow1_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td rowspan="3">
                                                <asp:TextBox ID="txtYellow_CoolMedian" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_T2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_W2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow2_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_T1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_T3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_T4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_T5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_T6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_TMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_W1" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_W3" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="TextBox2" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_W4" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_W5" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_W6" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_WMin" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_Area" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_Weight" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_InitialLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_LoadLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_LoadEL" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_CoolLength" runat="server" CssClass="textbox"></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtYellow3_CoolEL" runat="server" CssClass="textbox"></asp:TextBox></td>


                                        </tr>

                                        <tr>
                                            <td>Standard</td>
                                            <td colspan="2"></td>
                                            <td></td>
                                            <td colspan="7">-</td>
                                            <td colspan="7">-</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>at Temp. 200±3°C</td>
                                            <td>Max:175</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>-</td>
                                            <td>Max: 15</td>
                                        </tr>
                                    </tbody>
                                </table>

||||||| .r236
                                    </tbody>
                                </table>
                            </div>



                            <!-- Presuure at high temprature Test Results Grid -->

=======
                                    </tbody>
                                </table>
                         </div>
>>>>>>> .r267
<<<<<<< .mine
||||||| .r236
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>HotSet Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvHotsetTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvHotsetTest_RowDataBound" OnRowCommand="gvHotsetTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="ItemType" HeaderText="Item Type" />
                                                <asp:BoundField DataField="ItemCategory" HeaderText="Item Category" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core" />
                                                <asp:BoundField DataField="SampleType" HeaderText="Type of sample test" />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />
                                                <asp:BoundField DataField="Input1" HeaderText="1" />
                                                <asp:BoundField DataField="Input2" HeaderText="2" />
                                                <asp:BoundField DataField="Input3" HeaderText="3" />
                                                <asp:BoundField DataField="Input4" HeaderText="4" />
                                                <asp:BoundField DataField="Input5" HeaderText="5" />
                                                <asp:BoundField DataField="Input6" HeaderText="6" />
                                                <asp:BoundField DataField="InputMin" HeaderText="Min" />
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
=======
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
>>>>>>> .r267
                            </div>
<<<<<<< .mine

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn-primary" Text="Save Data" />
                            </div>
||||||| .r236
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Add Other Data</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered text-center">
                                            <thead>
                                                <tr>
                                                    <th rowspan="2">Core Phase</th>
                                                    <th rowspan="2">Specimen No</th>
                                                    <th rowspan="2">Area<br />
                                                        (mm²)</th>
                                                    <th rowspan="2">Weight Load<br />
                                                        (gr)</th>
                                                    <th rowspan="2">Initial Length<br />
                                                        (mm)</th>
                                                    <th colspan="2">After 15 min under Load at 200°C</th>
                                                    <th colspan="2">After Cooling</th>
                                                    <th rowspan="2">Add Data</th>
                                                </tr>
                                                <tr>
                                                    <th>Length<br />
                                                        (mm)</th>
                                                    <th>EL<br />
                                                        (%)</th>
                                                    <th>Length<br />
                                                        (mm)</th>
                                                    <th>EL<br />
                                                        (%)</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblPhaseForWeightload" runat="server" CssClass="form-control" Style="text-align: center; background-color: #f9f9f9;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspeicimentnumebrForweghightload" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtarea" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtWeightload" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtInitiallength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtAfter15Minlength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtAfter15MinEL" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtaftercoolinglength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtaftercoolingEL" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="Button1" runat="server" Text="Add" OnClick="btnAddWeightloadData_Click" CssClass="btn btn-primary" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>


                            <div class="row clearfix mt-4">
                                <asp:GridView ID="gvWeightloadData" runat="server"
                                    AutoGenerateColumns="False"
                                    CssClass="table table-bordered mt-3"
                                    OnRowDeleting="gvWeightloadData_RowDeleting"
                                    DataKeyNames="SpecimenNo,Weightload"
                                    OnRowDataBound="gvWeightloadData_RowDataBound"
                                    OnRowCreated="gvWeightloadData_RowCreated">
                                    <Columns>
                                        <asp:BoundField DataField="CorePhase" HeaderText="Core Phase" />
                                        <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No" SortExpression="SpecimenNo" />
                                        <asp:BoundField DataField="Area" HeaderText="Area" SortExpression="SpecimenNo" />
                                        <asp:BoundField DataField="Weightload" HeaderText="Weightload" SortExpression="Weightload" />
                                        <asp:BoundField DataField="Initiallength" HeaderText="Initial Length" />
                                        <asp:BoundField DataField="After15MinLength" HeaderText="15 Min Length" />
                                        <asp:BoundField DataField="After15MinEL" HeaderText="15 Min EL" />
                                        <asp:BoundField DataField="After15MinMedian" HeaderText="15 Min Median" />
                                        <asp:BoundField DataField="AftercoolingLength" HeaderText="Cooling Length" />
                                        <asp:BoundField DataField="AftercoolingEL" HeaderText="Cooling EL" />
                                        <asp:BoundField DataField="AftercoolingMedian" HeaderText="Cooling Median" />
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDelete" runat="server"
                                                    Text="Delete"
                                                    CommandName="Delete"
                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                    CssClass="btn btn-danger btn-sm" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>

                                </asp:GridView>

                            </div>


                            <!-- Standard Row - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Standard Values</h4>
                                    <div class="table-responsive">

                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th rowspan="2">Thickness</th>
                                                    <th rowspan="2">Width</th>
                                                    <th rowspan="2">Area</th>
                                                    <th rowspan="2">Weight load</th>
                                                    <th rowspan="2">Initial length</th>
                                                    <th colspan="3">After 15 min under Load at temperature 200°C</th>
                                                    <th colspan="3">After cooling</th>
                                                </tr>
                                                <tr>
                                                    <th>Length</th>
                                                    <th>EL</th>
                                                    <th>Median</th>
                                                    <th>Length</th>
                                                    <th>EL</th>
                                                    <th>Median</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificThickness" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificWidth" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificArea" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificWeightload" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificInitialLength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAfter15MinLength" runat="server" CssClass="form-control" Style="text-align: center;"  placeholder="at Temp.200+-°C "/>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAfter15MinEL" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAfter15MinMedian" runat="server" CssClass="form-control" Style="text-align: center;" placeholeder="MAX:"/>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAftercoolingLength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAftercoolingEL" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificAftercoolingMedian" runat="server" CssClass="form-control" Style="text-align: center;"  placeholder="MAX:"/>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                </div>
                            </div>
                            <div>
                            </div>
=======
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
>>>>>>> .r267
                        </div>
                    </div>
<<<<<<< .mine
||||||| .r236
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />
                    </div>
=======
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                    </div>
>>>>>>> .r267
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>


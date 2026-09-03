<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="LelehGrease.aspx.cs" Inherits="LIMS.LelehGrease" %>

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
                            <h2>LELEH GREASE</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvCableType" runat="server"
                                                ControlToValidate="cableType"
                                                ErrorMessage="Type is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server"
                                                ControlToValidate="refSpec"
                                                ErrorMessage="Ref. Spec. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvCableSize" runat="server"
                                                ControlToValidate="cableSize"
                                                ErrorMessage="Size is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Drum No.</b></td>
                                        <td>
                                            <asp:TextBox ID="drumno" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvdrumno" runat="server"
                                                ControlToValidate="drumno"
                                                ErrorMessage="Drum No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Length</b></td>
                                        <td>
                                            <asp:TextBox ID="length" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvlength" runat="server"
                                                ControlToValidate="length"
                                                ErrorMessage="Length is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvJobOrderNo" runat="server"
                                                ControlToValidate="jobOrderNo"
                                                ErrorMessage="Job Order No. is required"
                                                Display="Dynamic" ForeColor="Red" />
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
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvWorksheetNo" runat="server"
                                                ControlToValidate="worksheetno"
                                                ErrorMessage="Worksheet No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvRoomNo" runat="server"
                                                ControlToValidate="roomno"
                                                ErrorMessage="Room No. is required"
                                                Display="Dynamic" ForeColor="Red" />
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
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" Style="text-align: center;"/>
                                            <asp:RequiredFieldValidator ID="rfvEquipmentNo" runat="server"
                                                ControlToValidate="equipmentno"
                                                ErrorMessage="Equipment No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Marking of Cable</b></td>
                                        <td>
                                            <asp:TextBox ID="markingofcable" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            <asp:RequiredFieldValidator ID="rfvmarkingofcable" runat="server"
                                                ControlToValidate="markingofcable"
                                                ErrorMessage="Marking of Cable is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                </table>
                                
                            </div>
                            <div class="table-responsive">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th>No</th>
                                            <th>Berat Grease (kg/km)</th>
                                            <th>Temperatur (°C)</th>
                                            <th>Kondisi Grease</th>
                                            <th>Keterangan</th>
                                        </tr>
                                    </thead>
                                    <tbody>

                                        <!-- Row 1 -->
                                        <tr>
                                            <td>1</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease1" runat="server" ControlToValidate="txtBeratGrease1"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur1" runat="server" ControlToValidate="txtTemperatur1"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease1" runat="server" ControlToValidate="txtKondisiGrease1"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan1" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan1" runat="server" ControlToValidate="txtKeterangan1"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 2 -->
                                        <tr>
                                            <td>2</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease2" runat="server" ControlToValidate="txtBeratGrease2"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur2" runat="server" ControlToValidate="txtTemperatur2"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease2" runat="server" ControlToValidate="txtKondisiGrease2"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan2" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan2" runat="server" ControlToValidate="txtKeterangan2"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 3 -->
                                        <tr>
                                            <td>3</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease3" runat="server" ControlToValidate="txtBeratGrease3"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur3" runat="server" ControlToValidate="txtTemperatur3"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease3" runat="server" ControlToValidate="txtKondisiGrease3"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan3" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan3" runat="server" ControlToValidate="txtKeterangan3"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 4 -->
                                        <tr>
                                            <td>4</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease4" runat="server" ControlToValidate="txtBeratGrease4"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur4" runat="server" ControlToValidate="txtTemperatur4"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease4" runat="server" ControlToValidate="txtKondisiGrease4"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan4" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan4" runat="server" ControlToValidate="txtKeterangan4"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 5 -->
                                        <tr>
                                            <td>5</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease5" runat="server" ControlToValidate="txtBeratGrease5"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur5" runat="server" ControlToValidate="txtTemperatur5"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease5" runat="server" ControlToValidate="txtKondisiGrease5"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan5" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan5" runat="server" ControlToValidate="txtKeterangan5"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 6 -->
                                        <tr>
                                            <td>6</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease6" runat="server" ControlToValidate="txtBeratGrease6"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur6" runat="server" ControlToValidate="txtTemperatur6"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease6" runat="server" ControlToValidate="txtKondisiGrease6"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan6" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan6" runat="server" ControlToValidate="txtKeterangan6"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Row 7 -->
                                        <tr>
                                            <td>7</td>
                                            <td>
                                                <asp:TextBox ID="txtBeratGrease7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvBeratGrease7" runat="server" ControlToValidate="txtBeratGrease7"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtTemperatur7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvTemperatur7" runat="server" ControlToValidate="txtTemperatur7"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKondisiGrease7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKondisiGrease7" runat="server" ControlToValidate="txtKondisiGrease7"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtKeterangan7" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvKeterangan7" runat="server" ControlToValidate="txtKeterangan7"
                                                    ErrorMessage="Required" Display="Dynamic" ForeColor="Red" />
                                            </td>
                                        </tr>

                                        <!-- Footer -->
                                        <tr>
                                            <td colspan="5" style="text-align: left">Persyaratan : suhu leleh min : 120⁰C</td>
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
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

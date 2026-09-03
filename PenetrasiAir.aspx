<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="PenetrasiAir.aspx.cs" Inherits="LIMS.PenetrasiAir" %>




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
            width: 100%;
            overflow-x: auto;
            margin-bottom: 20px;
        }

        table {
            min-width: 600px;
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
            /*text-align: center;*/
            font-weight: bold;
        }

        td {
            padding: 8px;
            text-align: left;
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

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            max-width: 280px;
            padding: 6px;
            border: 1px solid #ddd;
            border-radius: 3px;
            margin: 0 auto;
            display: block;
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

        /* Validation styles */
        .validator-error {
            color: red;
            font-size: smaller;
            display: block;
            margin-top: 5px;
        }

        /* Responsive adjustments */
        @media screen and (max-width: 768px) {
            .body {
                padding: 10px;
            }

            th, td {
                padding: 6px;
                font-size: 14px;
            }

            input[type="text"], input[type="number"], input[type="date"],
            .form-control {
                max-width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>PENETRASI AIR Test</h2>
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
                                <table>
                                    <tr>
                                        <td colspan="6" style="text-align: center;">Siklus pemanasan, dikenakan pada penghantar
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" style="text-align: center;">Mulai </td>
                                        <td style="text-align: center;">Tercapai
                                            <br />
                                            95⁰-100⁰</td>
                                        <td style="text-align: center;">Aktual
                                            <br />
                                            Temp.</td>
                                        <td style="text-align: center;">Selesai</td>
                                        <td rowspan="2" style="text-align: center;">Add Data</td>
                                    </tr>

                                    <tr>
                                        <td style="text-align: center;">Tanggal</td>
                                        <td style="text-align: center;">Jam</td>
                                        <td style="text-align: center;">Jam</td>
                                        <td style="text-align: center;">⁰C</td>
                                        <td style="text-align: center;">Jam</td>
                                    </tr>

                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" placeholder="Test ITem" TextMode="Date" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                ControlToValidate="txtStartDate" ErrorMessage="Required" ValidationGroup="HeatingGroup" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                ControlToValidate="txtStartTime" ErrorMessage="Required" ValidationGroup="HeatingGroup" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtReachedTime" runat="server" CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                                ControlToValidate="txtReachedTime" ErrorMessage="Required" ValidationGroup="HeatingGroup" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtActualTemp" runat="server" CssClass="form-control" TextMode="Number" />
                                            <asp:RequiredFieldValidator ID="rfvMeasured1" runat="server"
                                                ControlToValidate="txtActualTemp" ErrorMessage="Required" ValidationGroup="HeatingGroup" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:TextBox ID="txtCooling" runat="server" CssClass="form-control" TextMode="Time" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server"
                                                ControlToValidate="txtCooling" ErrorMessage="Required" ValidationGroup="HeatingGroup" ForeColor="Red" Display="Dynamic">
                                            </asp:RequiredFieldValidator>
                                        </td>

                                        <td>
                                            <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="ADD" OnClick="btnAdd_Click" ValidationGroup="HeatingGroup" CausesValidation="true" />
                                        </td>
                                    </tr>
                                </table>
                            </div>


                            <br />

                            <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting">
                                <Columns>
                                    <asp:TemplateField HeaderText="Siklus">
                                        <ItemTemplate>
                                            <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" />
                                    <asp:BoundField DataField="StartTime" HeaderText="Start Time" />
                                    <asp:BoundField DataField="ReachedTime" HeaderText="Reached Time" />
                                    <asp:BoundField DataField="ActualTemp" HeaderText="Actual Temp (°C)" />
                                    <asp:BoundField DataField="CoolingTime" HeaderText="Cooling Time" />
                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete"
                                                CommandName="Delete" CommandArgument='<%# Container.DataItemIndex %>'
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                CssClass="btn btn-danger btn-sm" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <br />


                            <table class="table" border="1" style="margin-top: 10px; border-collapse: collapse;">
                                <tr>
                                    <td colspan="2" rowspan="2"><strong>Persyaratan (Specification):</strong>
                                        <asp:TextBox runat="server" ID="txtspecification" CssClass="form-control"></asp:TextBox>
                                    </td>
                                    <td colspan="1" style="text-align: center;">Min.:    
                                        <asp:TextBox runat="server" ID="txtspecMin" CssClass="form-control"></asp:TextBox></td>

                                    <td colspan="1" style="text-align: center;">Min.:    
                                        <asp:TextBox runat="server" ID="txtspecMin2" CssClass="form-control"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td colspan="5" style="text-align: center;">Setiap satu siklus pemanasan 8 jam                            </td>
                                </tr>
                            </table>

                            <br />

                            <br />

                            <table>
                                <tr>
                                    <td colspan="5" style="text-align: center;">Uji tekuk </td>
                                    <td colspan="4" style="text-align: center;">Penetrasi air </td>
                                    <td rowspan="4" style="text-align: center;">Add Data</td>
                                </tr>
                                <tr>
                                    <td rowspan="2">Tanggal</td>
                                    <td rowspan="2">Diameter kabel</td>
                                    <td rowspan="2">Diameter penghantar d</td>
                                    <td rowspan="2">Maks.diameter bending</td>
                                    <td rowspan="2">Diameter silinder</td>
                                    <td colspan="4">Pada temp. ruang 24 jam </td>
                                </tr>
                                <tr>
                                    <td colspan="2">Mulai</td>
                                    <td colspan="2">Selesai</td>
                                </tr>
                                <tr>
                                    <td style="text-align: center;">mm</td>
                                    <td style="text-align: center;">mm</td>
                                    <td style="text-align: center;">mm</td>
                                    <td style="text-align: center;">mm</td>
                                    <td style="text-align: center;">mm</td>
                                    <td style="text-align: center;">Tanggal</td>
                                    <td style="text-align: center;">Jam</td>
                                    <td style="text-align: center;">Tanggal</td>
                                    <td style="text-align: center;">Jam</td>
                                </tr>

                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtbendingtestdate" runat="server" CssClass="form-control" placeholder="Test ITem" TextMode="Date" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server"
                                            ControlToValidate="txtbendingtestdate" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>


                                    <td>
                                        <asp:TextBox ID="txtDiametercable" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server"
                                            ControlToValidate="txtDiametercable" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:TextBox ID="txtconductordiameter" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server"
                                            ControlToValidate="txtconductordiameter" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:TextBox ID="txtMaxbendingdiameter" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server"
                                            ControlToValidate="txtMaxbendingdiameter" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:TextBox ID="txtCylinderdiameter" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server"
                                            ControlToValidate="txtCylinderdiameter" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:TextBox ID="txtstartdatewater" runat="server" CssClass="form-control" TextMode="Date" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server"
                                            ControlToValidate="txtstartdatewater" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtstarttimewater" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server"
                                            ControlToValidate="txtstarttimewater" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtfinisdatewater" runat="server" CssClass="form-control" TextMode="Date" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server"
                                            ControlToValidate="txtstartdatewater" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:TextBox ID="txtfinishtimewater" runat="server" CssClass="form-control" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server"
                                            ControlToValidate="txtfinishtimewater" ErrorMessage="Required" ValidationGroup="BendingGroup" ForeColor="Red" Display="Dynamic">
                                        </asp:RequiredFieldValidator>
                                    </td>

                                    <td>
                                        <asp:Button ID="btnSubmit2" runat="server" CssClass="btn btn-primary" ValidationGroup="BendingGroup" Text="ADD" OnClick="btnAdd2_Click" CausesValidation="true" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:GridView ID="gvBendingWater" runat="server" AutoGenerateColumns="False" OnRowCreated="gvBendingWater_RowCreated" ShowHeader="False"
                                 OnRowDeleting="gvBendingWater_RowDeleting">
                                <Columns>
                                    <asp:BoundField DataField="dateforbending" HeaderText="Tanggal" />
                                    <asp:BoundField DataField="Diametercabelforbending" HeaderText="Diameter kabel" />
                                    <asp:BoundField DataField="Conductordiameterforbending" HeaderText="Diameter penghantar" />
                                    <asp:BoundField DataField="MaxDiameterforBending" HeaderText="Maks. diameter bending" />
                                    <asp:BoundField DataField="Diametercylinderforbending" HeaderText="Diameter silinder" />
                                    <asp:BoundField DataField="StartDateWaterforwater" HeaderText="Start Date" />
                                    <asp:BoundField DataField="StartTimeforwaterWater" HeaderText="Start Time" />
                                    <asp:BoundField DataField="FinishDateforwaterWater" HeaderText="Finish Date" />
                                    <asp:BoundField DataField="FinishTimeforwaterWater" HeaderText="Finish Time" />

                                    <asp:TemplateField HeaderText="Actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server" Text="Delete"
                                                CommandName="Delete" CommandArgument='<%# Container.DataItemIndex %>'
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                CssClass="btn btn-danger btn-sm" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                            <br />

                            <table border="1" style="border-collapse: collapse; width: 100%;">
                                <tr>
                                    <td colspan="6" style="font-weight: bold;">Hasil pengujian penetrasi air</td>
                                </tr>
                                <tr>
                                    <td style="text-align: center; font-weight: bold;">Panjang perembesan air</td>
                                    <td colspan="2" style="text-align: center; font-weight: bold;">Ujung  1 </td>
                                    <td colspan="2" style="text-align: center; font-weight: bold;">Ujung  2</td>

                                </tr>
                                <tr>
                                    <td style="text-align: center; font-weight: bold;">Pada penghantar </td>
                                    <td colspan="2" style="text-align: center;">
                                        <asp:TextBox ID="txtonconductor1" runat="server" CssClass="form-control" /></td>
                                    <td colspan="2" style="text-align: center;">
                                        <asp:TextBox ID="txtonconductor2" runat="server" CssClass="form-control" /></td>

                                </tr>
                                <tr>
                                    <td>Pada WBT konduktif tipe</td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtonconductiveWBTtype1" runat="server" CssClass="form-control" /></td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtonconductiveWBTtype2" runat="server" CssClass="form-control" /></td>
                                </tr>
                                <tr>
                                    <td>Pada WBT non konduktif tipe<br />
                                    </td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtonnonconductiveWBTtype1" runat="server" CssClass="form-control" /></td>
                                    <td colspan="2">
                                        <asp:TextBox ID="txtonnonconductiveWBTtype2" runat="server" CssClass="form-control" /></td>

                                </tr>
                                <tr>
                                    <td>Persyaratan </td>
                                    <td colspan="5">Selama pengujian tidak boleh ada air keluar dari ujung potongan sampel uji
            <br />

                                    </td>
                                </tr>
                            </table>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" CausesValidation="false" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClear_Click" CausesValidation="false" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" Visible="false" OnClick="btnUpdate_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>



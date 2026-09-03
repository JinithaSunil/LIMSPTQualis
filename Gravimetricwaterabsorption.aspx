<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="Gravimetricwaterabsorption.aspx.cs" Inherits="LIMS.Gravimetricwaterabsorption" %>

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

        .vertical-align-middle {
            vertical-align: middle !important;
        }

        .d-flex {
            display: flex;
        }

        .justify-content-between {
            justify-content: space-between;
        }

        .px-2 {
            padding-left: 0.5rem;
            padding-right: 0.5rem;
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
                            <h2>GRAVIMETRIC WATER ABSORPTION</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:Label ID="cableType" runat="server" CssClass="form-control" /></td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:Label ID="refSpec" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:Label ID="cableSize" runat="server" CssClass="form-control" /></td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:Label ID="testMethodNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:Label ID="voltage" runat="server" CssClass="form-control" /></td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:Label ID="jobOrderNo" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Date</b></td>
                                        <td>
                                            <asp:TextBox ID="Date" runat="server" CssClass="form-control" type="date" Style="text-align: center;" /></td>
                                        <td><b>Work Sheet No.</b></td>
                                        <td>
                                            <asp:Label ID="worksheetno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                    <tr>
                                        <td><b>Room No.</b></td>
                                        <td>
                                            <asp:TextBox ID="roomno" runat="server" CssClass="form-control" Style="text-align: center;" /></td>
                                        <td><b>Room Humidity (%)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomhumidity" runat="server" CssClass="form-control" Style="text-align: center;" />

                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Room Temp. (°C)</b></td>
                                        <td>
                                            <asp:TextBox ID="roomtemp" runat="server" CssClass="form-control" Style="text-align: center;" />
                                        </td>
                                        <td><b>Equipment No.</b></td>
                                        <td>
                                            <asp:Label ID="equipmentno" runat="server" CssClass="form-control" /></td>
                                    </tr>
                                </table>
                            </div>
                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">TestCategory</th>
                                            <th rowspan="2">Core Phase</th>
                                            <th rowspan="2">Specimen number</th>
                                            <th rowspan="2">1</th>
                                            <th rowspan="2">2</th>
                                            <th rowspan="2">3</th>
                                            <th rowspan="2">4</th>
                                            <th rowspan="2">5</th>
                                            <th rowspan="2">6</th>
                                            <th rowspan="2">Avg.</th>

                                            <th rowspan="2">Add Data</th>
                                        </tr>

                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlTestCategory" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTestCategory_SelectedIndexChanged" Style="text-align: center;">
                                                 
                                                    <asp:ListItem Text="Thickness" Value="Thickness" />
                                                    <asp:ListItem Text="Width" Value="Diameter" />
                                                    <asp:ListItem Text="Lenght" Value="Lenght" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCorePhase_SelectedIndexChanged" Style="text-align: center;">
                                                    <asp:ListItem Text="Red" Value="Red" />
                                                    <asp:ListItem Text="Blue" Value="Blue" />
                                                    <asp:ListItem Text="Yellow" Value="Yellow" />
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control"
                                                    AutoPostBack="true"
                                                    Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput1" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput2" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput3" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput4" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput5" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="Input_TextChanged" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtInput6" runat="server" AutoPostBack="true" OnTextChanged="Input_TextChanged" CssClass="form-control" Style="text-align: center;" />

                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtAvg" runat="server" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td style="text-align: center;">
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + " CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Add Other Information</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th rowspan="2" style="vertical-align: middle">Core Phase</th>
                                                    <th rowspan="2" style="vertical-align: middle">Specimen No</th>
                                                    <th style="vertical-align: middle">Total Surface<br />
                                                        Area</th>
                                                    <th colspan="3" style="vertical-align: middle">Weight</th>
                                                    <th rowspan="2" style="vertical-align: middle">Add Data</th>

                                                </tr>
                                                <tr>

                                                    <th style="vertical-align: middle">(cm)<sup>2</sup></th>
                                                    <th style="vertical-align: middle">M1<br />
                                                        (mg)</th>
                                                    <th style="vertical-align: middle">M2<br />
                                                        (mg)</th>
                                                    <th style="vertical-align: middle">M3<br />
                                                        (mg)</th>


                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtcorephase2" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtspeicimentnumebrForweghightload" runat="server" CssClass="form-control" Style="text-align: center;" ReadOnly="true" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txttotalsurfacearea" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtweightM1" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtweightM2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtweightM3" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnAddMassVariation" runat="server" Text=" + " OnClick="btnAddMassVariation_Click" CssClass="btn btn-primary mt-3" />
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <%--   <asp:GridView ID="gvMassVariation" runat="server"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered mt-3"
                                OnRowDeleting="gvMassVariation_RowDeleting"
                                DataKeyNames="SpecimenNo,TotalSurfaceArea,M1,M2,M3"
                                OnRowDataBound="gvMassVariation_RowDataBound">

                                <Columns>
               
                                    <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No" ItemStyle-VerticalAlign="Middle" />
                                    <asp:BoundField DataField="TotalSurfaceArea" HeaderText="Total Surface area"
                                        ItemStyle-VerticalAlign="Middle" HeaderStyle-CssClass="vertical-align-middle"
                                        DataFormatString="{0} cm²" HtmlEncode="false" />
                                    <asp:TemplateField HeaderText="Weight">
                                        <HeaderTemplate>
                                            <div>Weight</div>
                                            <div class="d-flex justify-content-between">
                                                <span>M1(mg)</span>
                                                <span>M2(mg)</span>
                                                <span>M3(mg)</span>
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <div class="d-flex justify-content-between">
                                                <asp:Label runat="server" Text='<%# Eval("M1") %>' CssClass="px-2" />
                                     
                                            <asp:Label runat="server" Text='<%# Eval("M2") %>' CssClass="px-2" />
                                                
                                            <asp:Label runat="server" Text='<%# Eval("M3") %>' CssClass="px-2" />
                                                
                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle CssClass="text-center" />
                                        <ItemStyle VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Variation of mass">
                                        <HeaderTemplate>
                                            <div>Variation of mass</div>
                                            <div class="d-flex justify-content-between">
                                                <span>mg/cm²</span>
                                                <span>Mean</span>
                                            </div>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <div class="d-flex justify-content-between">
                                                <asp:Label runat="server" Text='<%# CalculateMgPerCm2(Container.DataItem) %>' CssClass="px-2" />
                                                <asp:Label runat="server" Text='<%# CalculateMean(Container.DataItem) %>' CssClass="px-2" />
                                            </div>
                                        </ItemTemplate>
                                        <HeaderStyle CssClass="text-center" />
                                        <ItemStyle VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server"
                                                Text="Delete"
                                                CommandName="Delete"
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                CssClass="btn btn-danger btn-sm" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>

                                <HeaderStyle CssClass="thead-dark" />
                            </asp:GridView>--%>


                            <!-- Presuure at high temprature Test Results Grid -->

                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Gravimetric Test Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvPressureTest" runat="server" AutoGenerateColumns="false"
                                            CssClass="table table-bordered table-striped" OnRowDataBound="gvPressureTest_RowDataBound" OnRowCommand="gvPressureTest_RowCommand">
                                            <Columns>
                                                <asp:BoundField DataField="ItemCategory" HeaderText="Item Category" />
                                                <asp:BoundField DataField="CorePhase" HeaderText="Core" />
                                                <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No." />

                                                <asp:BoundField DataField="Input1" HeaderText="1" />
                                                <asp:BoundField DataField="Input2" HeaderText="2" />
                                                <asp:BoundField DataField="Input3" HeaderText="3" />
                                                <asp:BoundField DataField="Input4" HeaderText="4" />
                                                <asp:BoundField DataField="Input5" HeaderText="5" />
                                                <asp:BoundField DataField="Input6" HeaderText="6" />
                                                <asp:BoundField DataField="InputAvg" HeaderText="Avg" />
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

                            <asp:GridView ID="gvMassVariation" runat="server"
                                AutoGenerateColumns="False"
                                CssClass="table table-bordered mt-3"
                                OnRowDeleting="gvMassVariation_RowDeleting"
                                DataKeyNames="SpecimenNo,TotalSurfaceArea,M1,M2,M3"
                                OnRowDataBound="gvMassVariation_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="CorePhase" HeaderText="Core phase" ItemStyle-VerticalAlign="Middle" />

                                    <asp:BoundField DataField="SpecimenNo" HeaderText="Specimen No" ItemStyle-VerticalAlign="Middle" />

                                    <asp:BoundField DataField="TotalSurfaceArea" HeaderText="Total Surface area (cm²)"
                                        ItemStyle-VerticalAlign="Middle" HeaderStyle-CssClass="vertical-align-middle"
                                        DataFormatString="{0}" HtmlEncode="false" />

                                    <asp:BoundField DataField="M1" HeaderText="M1 (mg)"
                                        ItemStyle-VerticalAlign="Middle" HeaderStyle-CssClass="text-center"
                                        DataFormatString="{0}" />

                                    <asp:BoundField DataField="M2" HeaderText="M2 (mg)"
                                        ItemStyle-VerticalAlign="Middle" HeaderStyle-CssClass="text-center"
                                        DataFormatString="{0}" />

                                    <asp:BoundField DataField="M3" HeaderText="M3 (mg)"
                                        ItemStyle-VerticalAlign="Middle" HeaderStyle-CssClass="text-center"
                                        DataFormatString="{0}" />

                                    <asp:TemplateField HeaderText="mg/cm²" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# CalculateMgPerCm2(Container.DataItem) %>' />
                                        </ItemTemplate>
                                        <HeaderStyle CssClass="text-center" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Mean" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
                                            <asp:Label runat="server" Text='<%# CalculateMean(Container.DataItem) %>' />
                                        </ItemTemplate>
                                        <HeaderStyle CssClass="text-center" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action" ItemStyle-VerticalAlign="Middle">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDelete" runat="server"
                                                Text="Delete"
                                                CommandName="Delete"
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');"
                                                CssClass="btn btn-danger btn-sm" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="thead-dark" />
                            </asp:GridView>



                            <!-- Standard Row - Horizontal Layout -->
                            <div class="row clearfix mt-3">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Specified Values</h4>
                                    <div class="table-responsive">
                                        <table class="table table-bordered">
                                            <thead>
                                                <tr>
                                                    <th>Thickness</th>
                                                    <th>Width</th>
                                                     <th>Length</th>
                                                    <th>Total Surface Area</th>
                                                    <th>mg/cm<sup>2</sup></th>
                                                    <th>Mean</th>
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
                                                        <asp:TextBox ID="txtSpecificLength" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecifictotalsurfacearea" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificmgcm2" runat="server" CssClass="form-control" Style="text-align: center;" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSpecificMean" runat="server" CssClass="form-control" Style="text-align: center;" />
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

                        <div>
                          <table class="table table-bordered">
                                <thead>
                                    <tr>
                                        <th colspan="4">Vacuum temperature at 70°C</th>
                                        <th colspan="4">Conditioning in desiccator after Vacuum</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="2">Date</td>
                                        <td colspan="2">Time</td>
                                        <td colspan="2">Date</td>
                                        <td colspan="2">Time</td>
                                    </tr>
                                    <tr>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtVacuumDateStart" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuumDateFinish" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuumTimeStart" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuumTimeFinish" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond1DateStart" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond1DateFinish" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond1TimeStart" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond1TimeFinish" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                    </tr>
                                </tbody>
                            </table>

                            <table class="table table-bordered align-middle text-center">
                                <thead>
                                    <tr>
                                        <th colspan="4">Immersed at 85°C</th>
                                        <th colspan="4">Vacuum temperature at 70°C</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="2">Date</td>
                                        <td colspan="2">Time</td>
                                        <td colspan="2">Date</td>
                                        <td colspan="2">Time</td>
                                    </tr>
                                    <tr>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtImmDateStart" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtImmDateFinish" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtImmTimeStart" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtImmTimeFinish" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuum2DateStart" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuum2DateFinish" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuum2TimeStart" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtVacuum2TimeFinish" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                    </tr>
                                </tbody>
                            </table>

                            <table class="table table-bordered align-middle text-center">
                                <thead>
                                    <tr>
                                        <th colspan="4">Condition in Desiccator</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="2">Date</td>
                                        <td colspan="2">Time</td>
                                    </tr>
                                    <tr>
                                        <td>Start</td>
                                        <td>Finish</td>
                                        <td>Start</td>
                                        <td>Finish</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtCond2DateStart" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond2DateFinish" runat="server" CssClass="form-control text-center" TextMode="Date" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond2TimeStart" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                        <td>
                                            <asp:TextBox ID="txtCond2TimeFinish" runat="server" CssClass="form-control text-center" TextMode="Time" /></td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                        <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" />
                        <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" Visible="false" />
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>

</asp:Content>

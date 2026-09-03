<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="DimensionTypeTestMediumHighVoltage.aspx.cs" Inherits="LIMS.DimensionTypeTestMediumHighVoltage" %>


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
                            <h2>Dimension Low Voltage Cable</h2>
                        </div>
                        <div class="body">
                            <div class="table-responsive">
                                <table>
                                    <tr>
                                        <td><b>Type</b></td>
                                        <td>
                                            <asp:TextBox ID="cableType" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvCableType" runat="server"
                                                ControlToValidate="cableType"
                                                ErrorMessage="Type is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Ref.Spec.</b></td>
                                        <td>
                                            <asp:TextBox ID="refSpec" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvRefSpec" runat="server"
                                                ControlToValidate="refSpec"
                                                ErrorMessage="Ref. Spec. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Size</b></td>
                                        <td>
                                            <asp:TextBox ID="cableSize" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvCableSize" runat="server"
                                                ControlToValidate="cableSize"
                                                ErrorMessage="Size is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Test Method No.</b></td>
                                        <td>
                                            <asp:TextBox ID="testMethodNo" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvTestMethodNo" runat="server"
                                                ControlToValidate="testMethodNo"
                                                ErrorMessage="Test Method No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Voltage</b></td>
                                        <td>
                                            <asp:TextBox ID="voltage" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvVoltage" runat="server"
                                                ControlToValidate="voltage"
                                                ErrorMessage="Voltage is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                        <td><b>Job Order No.</b></td>
                                        <td>
                                            <asp:TextBox ID="jobOrderNo" runat="server" CssClass="form-control" />
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
                                            <asp:TextBox ID="worksheetno" runat="server" CssClass="form-control" />
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
                                            <asp:TextBox ID="equipmentno" runat="server" CssClass="form-control" />
                                            <asp:RequiredFieldValidator ID="rfvEquipmentNo" runat="server"
                                                ControlToValidate="equipmentno"
                                                ErrorMessage="Equipment No. is required"
                                                Display="Dynamic" ForeColor="Red" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <br />
                            <div class="table-responsive">
                                <table class="table table-bordered">
                                    <thead>
                                        <tr>
                                            <th rowspan="2">Number Of Sample</th>
                                            <th rowspan="2">Item Type</th>
                                            <th rowspan="2">Test Subitem</th>
                                            <th rowspan="2">Core Phase</th>
                                            <th rowspan="2">Unit</th>
                                           <%-- <th rowspan="2">Specimen number</th>--%>
                                            <th rowspan="2">Input(comma separated)</th>
                                            <th rowspan="2">Min,Avg</th>
                                            <th rowspan="2">Specifications</th>
                                            <th rowspan="2">Add Data</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:textbox ID="txttypeofsample" runat="server" CssClass="form-control" TextMode="Number"></asp:textbox></td>
                                            <td>
                                                <asp:TextBox ID="txtItemType" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">                                                                                               
                                                </asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtsubitem" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">                                                                                               
                                                </asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlCorePhase" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">                                                  
                                                </asp:DropDownList>
                                            </td>

                                            <td>
                                                <asp:DropDownList ID="ddlUnit" runat="server" CssClass="form-control" AutoPostBack="true" Style="text-align: center;">
                                                    <asp:ListItem Text="mm" Value="mm" />
                                                    <asp:ListItem Text="pcs" Value="pcs" />
                                                    <asp:ListItem Text="mm/d" Value="mm/d" />
                                                </asp:DropDownList>
                                            </td>
                                            <%--<td>
                                                <asp:TextBox ID="txtSpecimenNo" runat="server" CssClass="form-control"
                                                    AutoPostBack="true"
                                                    Style="text-align: center;" />
                                            </td>--%>
                                            <td>
                                                <asp:TextBox ID="txtInputs" runat="server" AutoPostBack="true" OnTextChanged="txtInput_TextChanged" CssClass="form-control" Style="text-align: center;" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMinMax" runat="server" CssClass="form-control" ReadOnly="true" />
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtSpecifications" runat="server" CssClass="form-control" />
                                            </td>
                                            <td>
                                                <asp:Button ID="btnAddToGrid" runat="server" Text=" + "
                                                    CssClass="btn btn-primary" OnClick="btnAddToGrid_Click" />
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                            <div class="row clearfix mt-4">
                                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                                    <h4>Dimension Test  Results</h4>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvDimension" runat="server" AutoGenerateColumns="false" 
                                             OnRowDataBound="gvDimension_RowDataBound"
                                            OnRowCommand="gvDimension_RowCommand"
                                            CssClass="table table-bordered table-striped">
                                            <Columns>
                                                
                                                <asp:BoundField DataField="typeofsample" HeaderText="Type Of Sample" />
                                                <asp:BoundField DataField="ItemType" HeaderText="Item Type" />
                                                <asp:BoundField DataField="ItemSubType" HeaderText="Item Subtype" />

                                                <asp:BoundField DataField="CorePhase" HeaderText="Core" />
                                                <asp:BoundField DataField="Unit" HeaderText="Unit" />
                                                <asp:BoundField DataField="Inputs" HeaderText="Inputs (comma sepreted)" />
                                                <asp:BoundField DataField="InputMinMax" HeaderText="Min / Max / Avg" />
                                                <asp:BoundField DataField="Specification" HeaderText="Specification" />
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

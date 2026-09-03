<%@ Page Title="" Language="C#" MasterPageFile="~/PTQualisMasterFull.Master" AutoEventWireup="true" CodeBehind="BendingTest.aspx.cs" Inherits="LIMS.BendingTest" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

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
    </style>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderBody" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="row clearfix">
                <div class="col-lg-12 col-md-12 col-sm-12 col-xs-12">
                    <div class="card">
                        <hr class="violet">
                        <div class="header">
                            <h2>BENDING TEST</h2>
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
                                            <asp:Label ID="lblvoltage" runat="server" CssClass="form-control" /></td>
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

                            <br />
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <b>a) for cable with a lead sheath, corrugated metallic,<br>
                                            sheath or metal foil,</b>
                                    </td>
                                    <td colspan="2">
                                        <b>b) for other cables</b>
                                    </td>
                                </tr>

                                <tr>
                                    <td><b>for single-core cables:-</b></td>
                                    <td>
                                        <asp:TextBox ID="txtsinglecore1" runat="server" CssClass="form-control" style="text-align:center"/></td>
                                    <td><b>for single-core cables:-</b></td>
                                    <td>
                                        <asp:TextBox ID="txtsinglecore2" runat="server" CssClass="form-control" style="text-align:center" /></td>
                                </tr>
                                <tr>

                                    <td><b>for three-core cables:-</b></td>
                                    <td>
                                        <asp:TextBox ID="txtthreecore1" runat="server" CssClass="form-control" style="text-align:center" /></td>
                                    <td><b>for three-core cables:-</b></td>
                                    <td>
                                        <asp:TextBox ID="txtthreecore2" runat="server" CssClass="form-control" style="text-align:center" /></td>
                                </tr>

                            </table>


                            <div class="table-responsive tests-section" style="margin-top: 20px;">
                                <table class="table table-bordered" >
                                    <thead>
                                        <tr>

                                            <th>Outer Diameter of cable
                                                <br>D</br>
                                            </th>
                                            <th>Diameter of cable conductor
                                                <br>d</br>
                                            </th>
                                            <th>Maximum diameter bending diameter
                                                <br>Dr</br>
                                            </th>
                                            <th>Diameter of test Cylinder<br>Dt</br>
                                            </th>
                                            <th>Result</th>
                                        </tr>

                                        <tr>
                                            <th>(mm)</th>
                                            <th>(mm)</th>
                                            <th>(mm)</th>
                                            <th>(mm)</th>
                                            <th rowspan="2"></th>
                                        </tr>
                                    </thead>
                                    <tbody >
                                        <tr>
                                            <td>
                                                <asp:TextBox ID="txtOuterDiameter" runat="server" CssClass="form-control" style="text-align:center"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtConductorDiameter" runat="server" CssClass="form-control" style="text-align:center"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBendingDiameter" runat="server" CssClass="form-control" style="text-align:center"></asp:TextBox>
                                            </td>

                                            <td>
                                                <asp:TextBox ID="txtTestCylinder" runat="server" CssClass="form-control" style="text-align:center"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlresult" runat="server" CssClass="form-control">
                                                    <asp:ListItem Text="PASS" Value="1" />
                                                    <asp:ListItem Text="FAIL" Value="2" />
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>

                            </div>

                            <div class="form-actions">
                                <asp:Button ID="btnSaveData" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btnSubmit_Click" />
                                <asp:Button ID="btnClearData" runat="server" CssClass="btn btn-primary" Text="Clear" OnClick="btnClearData_Click" />
                                <asp:Button ID="btnUpdateData" runat="server" CssClass="btn btn-primary" Text="Update" OnClick="btnUpdate_Click" visible="false"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>


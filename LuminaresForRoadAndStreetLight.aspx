<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LuminaresForRoadAndStreetLight.aspx.cs" Inherits="LIMS.LuminaresForRoadAndStreetLight" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Results Form</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css" rel="stylesheet" />

    <style type="text/css">
        body {
            font-family: "Calibri", "Segoe UI", sans-serif;
            /*font-size: 14px;*/
        }

        .form-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        .clause-table {
            border-collapse: collapse;
            width: 100%;
            margin-bottom: 20px;
            border: 2px solid #333;
        }

        .header-row {
            color: black;
            font-weight: bold;
        }

        .sub-header-row {
            background-color: #d1d0d0;
            font-weight: bold;
        }

        .clause-table td, .clause-table th {
            border: 1px solid black;
            padding: 8px;
            text-align: left;
            vertical-align: top;
        }

        .clause-number {
            width: 10%;
            font-weight: bold;
        }

        .requirement {
            width: 45%;
        }

        .result {
            width: 25%;
        }

        .remark {
            width: 20%;
        }

        .checkbox-group {
            display: flex;
            flex-direction: column;
            gap: 10px; /* spacing between rows */
        }

        .checkbox-row {
            display: flex;
            gap: 20px; /* spacing between items in a row */
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            white-space: nowrap;
        }

        .checkbox-item {
            display: flex;
            align-items: center;
            white-space: nowrap;
            font-size: 18px; /* Increase font size */
        }

            .checkbox-item input[type="checkbox"] {
                width: 20px; /* Increase checkbox width */
                height: 20px; /* Increase checkbox height */
                margin-right: 8px; /* Space between checkbox and label */
            }

        .btn-container {
            text-align: center;
            margin-top: 20px;
        }

        .btn {
            padding: 10px 20px;
            margin: 5px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-save {
            /*background-color: #4CAF50;*/
            background-color: #b34788 !important;
            color: white;
        }

        .btn-report {
            background-color: #b34788 !important;
            color: black;
        }

        .btn:hover {
            opacity: 0.8;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
            margin-bottom: 20px;
        }

        table {
            width: 100%;
            min-width: 800px; /* More room for fields */
            border-collapse: collapse;
            background-color: #fff;
        }

        table, th, td {
            border: 2px solid #333;
            text-align: left;
        }

        td {
            padding: 8px 10px;
            text-align: left;
            vertical-align: middle;
            text-align: left;
        }

        .info-table td:first-child,
        .info-table td:nth-child(3) {
            width: 20%;
        }

        input[type="text"], input[type="number"], input[type="date"],
        .form-control {
            width: 100%;
            max-width: 100%;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            display: block;
            box-sizing: border-box;
        }

        .center-text {
            text-align: center;
        }

        .form-actions {
            margin-top: 20px;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <div class="form-container">

            <div class="header">
                <h2>Test Result Entry Form </h2>
                <asp:Label ID="lblstandardID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblReportID" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="lblSampleID" runat="server" Visible="false"></asp:Label>
            </div>
            <div class="table-responsive">
                <table class="info-table">
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Work Order</td>
                        <td>
                            <asp:TextBox ID="txtworkorder" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Product</td>
                        <td>
                            <asp:TextBox ID="txtproduct" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Brand</td>
                        <td>
                            <asp:TextBox ID="txtbrand" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Model</td>
                        <td>
                            <asp:TextBox ID="txtmodel" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Standard</td>
                        <td>
                            <asp:TextBox ID="txtstandard" runat="server" CssClass="form-control" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Method No.</td>
                        <td>
                            <asp:TextBox ID="txttestMethodNo" runat="server" CssClass="form-control" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Temp</td>
                        <td>
                            <asp:TextBox ID="txttemp" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvTemp" runat="server"
                                ControlToValidate="txttemp" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Temperature is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Humidity (%)</td>
                        <td>
                            <asp:TextBox ID="txthumidity" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvHumidity" runat="server"
                                ControlToValidate="txthumidity" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Humidity is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Starting Date</td>
                        <td>
                            <asp:TextBox ID="txtstartingdate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvStartingDate" runat="server"
                                ControlToValidate="txtstartingdate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Starting Date is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Test Completion Date</td>
                        <td>
                            <asp:TextBox ID="txtcompletiondate" runat="server" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator ID="rfvCompletionDate" runat="server"
                                ControlToValidate="txtcompletiondate" ValidationGroup="DefaultDataUpperDetails"
                                ErrorMessage="Test Completion Date is required"
                                Display="Dynamic"
                                ForeColor="Red"
                                Text="Required" />
                        </td>
                    </tr>
                    <tr>
                        <td style="background-color: #d1d0d0; font-weight: bold;">Equipment</td>
                        <td colspan="3">
                            <asp:CheckBoxList ID="chkEquipmentList" runat="server" RepeatColumns="1" RepeatDirection="Horizontal">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </div>

            <br />
            <br />
            <br />

            <div id="clauseContainer" runat="server" style="margin-bottom: 100px;"></div>
            <hr />
            <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <h2><u>LAMPIRAN A – DAFTAR KOMPONEN LUMINER </u></h2>
                    <div class="table-responsive">
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtObjectName" runat="server" CssClass="form-control" placeholder="Object Name" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtObjectName" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtManufacturer" runat="server" CssClass="form-control" placeholder="Manufacturer" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtManufacturer" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtTypeOrModel" runat="server" CssClass="form-control" placeholder="Type/Model" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtTypeOrModel" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtTechnicalData" runat="server" CssClass="form-control" placeholder="Technical Data" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtTechnicalData" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtStandard1" runat="server" CssClass="form-control" placeholder="Standard" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtStandard1" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:TextBox ID="txtSignOfConformity" runat="server" CssClass="form-control" placeholder="Sign Of Conformity" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ValidationGroup="ObjectDetails"
                                        ControlToValidate="txtSignOfConformity" ErrorMessage="Required" ForeColor="Red" Display="Dynamic">
                                    </asp:RequiredFieldValidator>
                                </td>

                                <td>
                                    <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary" Text="ADD" Style="background-color: #2196F3;" OnClick="btnAdd_Click" CausesValidation="true" ValidationGroup="ObjectDetails" />
                                </td>

                            </tr>
                        </table>
                    </div>
                    <hr />

                    <asp:GridView ID="gvTests" runat="server" AutoGenerateColumns="False" OnRowDeleting="gvTests_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="No.">
                                <ItemTemplate>
                                    <%# (gvTests.PageIndex * gvTests.PageSize) + Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="ObjectName" HeaderText="Object Name" />
                            <asp:BoundField DataField="Manufacturer" HeaderText="Manufacturer" />
                            <asp:BoundField DataField="TypeOrModel" HeaderText="Type/Model" />
                            <asp:BoundField DataField="TechnicalData" HeaderText="Technical Data" />
                            <asp:BoundField DataField="Standard" HeaderText="Standard" />
                            <asp:BoundField DataField="SignOfConformity" HeaderText="Sign Of Conformity" />

                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkDelete" runat="server" Style="color: red;" CommandName="Delete"
                                        OnClientClick="return confirm('Are you sure you want to delete this item?');"
                                        CssClass="text-danger" ToolTip="Delete">
                                        <i class="fas fa-trash-alt"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>


                    <h2 style="margin-top: 100px;">LAMPIRAN B – BAHAYA FOTOBIOLOGIS </h2>
                    <div class="table-responsive">
                        <table style="width: 100%; border-collapse: collapse;">
                            <tr style="background-color: #d1d0d0; font-weight: bold;">
                                <th style="width: 15%; text-align: center; padding: 10px; border: 1px solid #999;">Risk<br />
                                    Bahaya Optik
                                </th>
                                <th style="width: 10%; text-align: center; padding: 10px; border: 1px solid #999;">Symbol<br />
                                    Simbol
                                </th>
                                <th style="width: 10%; text-align: center; padding: 10px; border: 1px solid #999;">Unit<br />
                                    Satuan
                                </th>
                                <th style="width: 20%; text-align: center; padding: 10px; border: 1px solid #999;">Measurement Result<br />
                                    Hasil Pengukuran
                                </th>
                                <th style="width: 20%; text-align: center; padding: 10px; border: 1px solid #999;">Limit<br />
                                    Batas
                                </th>
                                <th style="width: 25%; text-align: center; padding: 10px; border: 1px solid #999;">Remark<br />
                                    Keterangan
                                </th>
                            </tr>

                            <!-- Actinic UV -->
                            <tr>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">Actinic UV</td>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">E<sub>S</sub></td>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">W.m<sup>-2</sup></td>

                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtActinicUV1" runat="server"></asp:TextBox>
                                </td>

                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">0,001 (Exempt/RG0)</td>
                                <td rowspan="3" style="text-align: left; padding: 10px; border: 1px solid #999;">Risk group classification:<br />
                                    RG0 (Exempt) /<br />
                                    RG1 (Low Risk) /<br />
                                    RG2 (Mod Risk) /<br />
                                    RG3 (High Risk) /<br />
                                    Test Result:<br />
                                    (Pass / Fail)
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtActinicUV2" runat="server"></asp:TextBox>
                                </td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">0,003 (Low Risk/RG1)</td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtActinicUV3" runat="server"></asp:TextBox>
                                </td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">0,03 (Mod Risk/RG2)</td>
                            </tr>

                            <!-- Near UV -->
                            <tr>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">Near UV</td>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">E<sub>UVA</sub></td>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">W.m<sup>-2</sup></td>

                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtNearUV1" runat="server"></asp:TextBox>
                                </td>

                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">10 (Exempt/RG0)</td>
                                <td rowspan="3" style="text-align: left; padding: 10px; border: 1px solid #999;">Risk group classification:<br />
                                    RG0 (Exempt) /<br />
                                    RG1 (Low Risk) /<br />
                                    RG2 (Mod Risk) /<br />
                                    RG3 (High Risk) /<br />
                                    Test Result:<br />
                                    (Pass / Fail)
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtNearUV2" runat="server"></asp:TextBox>
                                </td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">33 (Low Risk/RG1)</td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtNearUV3" runat="server"></asp:TextBox>
                                </td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">100 (Mod Risk/RG2)</td>
                            </tr>

                            <!-- Blue Light -->
                            <tr>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">Blue light</td>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">L<sub>B</sub></td>
                                <td rowspan="3" style="text-align: center; padding: 10px; border: 1px solid #999;">W.m<sup>-2</sup>.sr<sup>-1</sup></td>

                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtBlueLight1" runat="server"></asp:TextBox>
                                </td>

                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">100 (Exempt/RG0)</td>
                                <td rowspan="3" style="text-align: left; padding: 10px; border: 1px solid #999;">Risk group classification:<br />
                                    RG0 (Exempt) /<br />
                                    RG1 (Low Risk) /<br />
                                    RG2 (Mod Risk) /<br />
                                    RG3 (High Risk) /<br />
                                    Test Result:<br />
                                    (Pass / Fail)
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtBlueLight2" runat="server"></asp:TextBox>
                                </td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">10000 (Low Risk/RG1)</td>
                            </tr>
                            <tr>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtBlueLight3" runat="server"></asp:TextBox>
                                </td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">4000000 (Mod Risk/RG2)</td>
                            </tr>

                            <tr>
                                <td colspan="6">Notes:<br />
                                    RG: Risk Group<br />
                                    RG0 (Exempt group): The lamp does not pose any photobiological hazard.<br />
                                    RG1 (Low Risk): The lamp does not pose a hazard due to normal behavioral limitations on exposure.<br />
                                    RG2 (Moderate Risk): The lamp does not pose a hazard due to the aversion response to very bright light sources or due to thermal discomfort.<br />
                                    RG3 (High Risk): The lamp may pose a hazard even for momentary or brief exposure.<br />
                                    The lamps which exceed the limits for RG2 (Moderate Risk) are in RG3 (High Risk).<br />
                                </td>
                            </tr>
                        </table>

                        <table style="width: 100%; margin-top: 50px; border-collapse: collapse; font-family: Arial, sans-serif; border: 1px solid #ddd;">
                            <tr style="background-color: #d1d0d0; font-weight: bold;">
                                <th style="padding: 12px; text-align: left; border: 1px solid #ddd; font-weight: bold;">Item Test<br>
                                    Item uji</th>
                                <th style="padding: 12px; text-align: left; border: 1px solid #ddd; font-weight: bold;">Symbol<br>
                                    Simbol</th>
                                <th style="padding: 12px; text-align: left; border: 1px solid #ddd; font-weight: bold;">Unit<br>
                                    Satuan</th>
                                <th style="padding: 12px; text-align: left; border: 1px solid #ddd; font-weight: bold;">Results<br>
                                    Hasil uji</th>
                            </tr>
                            <tr>
                                <td style="padding: 12px; border: 1px solid #ddd;">Blue light hazard radiance</td>
                                <td style="padding: 12px; border: 1px solid #ddd;">L<sub>B</sub></td>
                                <td style="padding: 12px; border: 1px solid #ddd;">W/m²·sr</td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtBlueLightHazardRadiance" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr style="background-color: #f9f9f9;">
                                <td style="padding: 12px; border: 1px solid #ddd;">Luminance</td>
                                <td style="padding: 12px; border: 1px solid #ddd;">L<sub>v</sub></td>
                                <td style="padding: 12px; border: 1px solid #ddd;">cd/m²</td>
                                <td style="text-align: center; padding: 10px; border: 1px solid #999;">
                                    <asp:TextBox ID="txtLuminance" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>

                    </div>


                    <%--// {LAMPIRAN_C-TERMINAL} //--%>
                    <%--<h2 style="margin-top: 100px;">LAMPIRAN C – TERMINAL</h2> 
                    <div id="clauseContainerT1" runat="server" style="margin-bottom: 100px;"></div>--%>

                    <h2 style="margin-top: 100px;">LAMPIRAN D – UJI TERMAL</h2>
                    <table style="width: 100%; border-collapse: collapse; font-family: Arial, sans-serif; border: 1px solid #999;">
                        <tr style="background-color: #d1d0d0; font-weight: bold;">
                            <th style="padding: 10px; border: 1px solid #999; text-align: center; width: 10%;">Klausul<br>
                                clause</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center; width: 40%;">Persyaratan<br>
                                Requirement</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center; width: 25%;">Hasil<br>
                                Result</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center; width: 25%;">*Keterangan<br>
                                Remark</th>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">1.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Tipe produk.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtProductTypeResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtProductTypeRemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">2.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Lampu yang digunakan.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTheLightsUsedResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTheLightsUsedRemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">3.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Tipe LED driver yang digunakan.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTypeOfLEDDriverUsedResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTypeOfLEDDriverUsedRemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">4.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Posisi pemasangan saat pengujian.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtMountingPositionDuringTestingResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txttxtMountingPositionDuringTestingRemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">5.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Daya (W).</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtPowerWResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtPowerWRemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">6.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Arus (A).</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtCurrentAResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtCurrentARemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">7.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Uji 1:<br>
                                Tegangan pengenal (V).
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest1RatedVoltageResult" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest1RatedVoltageRemark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>

                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">8.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Uji 2:<br>
                                1,06 x tegangan pengenal atau 1,05 x daya.
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest2Result" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest2Remark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">9.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Uji 3:<br>
                                Bakan dengan dilengkapi kotak kontak 1,06 x tegangan pengenal atau 1,05 x daya.
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest3Result" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest3Remark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">10.</td>
                            <td style="padding: 10px; border: 1px solid #999;">Uji 4:<br>
                                1,1 x tegangan pengenal atau 1,05 x daya.
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest4Result" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTest4Remark" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>
                    </table>



                    <table style="width: 100%; border-collapse: collapse; font-family: Arial, sans-serif; border: 1px solid #999;">
                        <tr style="background-color: #d1d0d0; font-weight: bold;">
                            <th rowspan="2" style="padding: 10px; border: 1px solid #999; text-align: center;">Bagian yang diuji</th>
                            <th colspan="4" style="padding: 10px; border: 1px solid #999; text-align: center;">Klausul 12.4 – Uji termal
                                <br />
                                (Operasi normal)</th>
                            <th colspan="2" style="padding: 10px; border: 1px solid #999; text-align: center;">Klausul 12.5 – Uji termal
                                <br />
                                (Operasi abnormal)</th>
                        </tr>
                        <tr style="background-color: #d1d0d0; font-weight: bold;">
                            <th style="padding: 10px; border: 1px solid #999; text-align: center;">Uji 1</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center;">Uji 2</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center;">Uji 3</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center;">Limit Maks.</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center;">Uji 4</th>
                            <th style="padding: 10px; border: 1px solid #999; text-align: center;">Limit Maks.</th>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">Suhu ruang uji.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTestChamberTemperatureTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTestChamberTemperatureTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTestChamberTemperatureTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTestChamberTemperatureLimitsMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTestChamberTemperatureTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtTestChamberTemperatureLimitsMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">Permukaan luar luminer</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtOuterSurfaceOfTheLuminaireTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtOuterSurfaceOfTheLuminaireTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtOuterSurfaceOfTheLuminaireTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtOuterSurfaceOfTheLuminaireLimitMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtOuterSurfaceOfTheLuminaireTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtOuterSurfaceOfTheLuminaireLimitMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">Permukaan LED Driver.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtSurfaceLEDDriverTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtSurfaceLEDDriverTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtSurfaceLEDDriverTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtSurfaceLEDDriverLimitMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtSurfaceLEDDriverTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtSurfaceLEDDriverLimitMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">LED module / PCB.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModulePCBTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModulePCBTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModulePCBTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModulePCBLimitMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModulePCBTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModulePCBLimitMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">Selungkup LED module (Plastik)</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModuleEnclosurePlasticTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModuleEnclosurePlasticTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModuleEnclosurePlasticTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModuleEnclosurePlasticLimitMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModuleEnclosurePlasticTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtLEDModuleEnclosurePlasticLimitMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">Kabel eksternal.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtExternalCableTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtExternalCableTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtExternalCableTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtExternalCableLimitMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtExternalCableTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtExternalCableLimitMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>

                        <tr>
                            <td style="padding: 10px; border: 1px solid #999;">Kabel internal.</td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtInternalWiringTest1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtInternalWiringTest2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtInternalWiringTest3" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtInternalWiringLimitMax1" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtInternalWiringTest4" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                            <td style="padding: 10px; border: 1px solid #999; text-align: center;">
                                <asp:TextBox ID="txtInternalWiringLimitMax2" runat="server" Width="90%" Style="text-align: center;"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="btn-container">
                <asp:Button ID="btnSave" runat="server" Text="Save Results" OnClick="btnSave_Click" CssClass="btn btn-save" CausesValidation="true" ValidationGroup="DefaultDataUpperDetails" />
                <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" OnClick="btnExport_Click" CssClass="btn btn-report" Visible="false" />
            </div>

        </div>
    </form>
</body>
</html>
